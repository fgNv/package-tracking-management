﻿module Migrations

open PgSqlPersistence
open Npgsql
open System
open System.IO
open Railroad

type private ExecutedMigrationItem = {
    Name : string
    ExecutedAt : DateTime
}

type private PendingMigrationItem = {
    Name : string
    Command : string
}

let private getExecutedMigrations conn =
    use cmd = new NpgsqlCommand()
    cmd.Connection <- conn
    cmd.CommandText <- """
        CREATE TABLE IF NOT EXISTS public.__migrations (
                        name varchar(100) NOT NULL,
						executedAt timestamp without time zone NOT NULL default CURRENT_TIMESTAMP,
						command text NOT NULL,
						CONSTRAINT "PK___migrations" PRIMARY KEY (name) );
    """
    cmd.ExecuteNonQuery() |> ignore

    cmd.CommandText <- """
        SELECT name, executedAt FROM public.__migrations;
    """

    use reader = cmd.ExecuteReader()
    [ while reader.Read() 
         do yield { Name = reader.GetString(0)
                    ExecutedAt = reader.GetTimeStamp(1).DateTime } ]  |> Seq.toList

type FolderDiscovery = | Absolute | Relative | Fixed of string

let private getPendingMigrations conn migrationsPath = 
    let executedMigrations = getExecutedMigrations conn

    let availableMigrations =          
         Directory.GetFiles migrationsPath |>
         Seq.map (fun path -> { Name = Path.GetFileName path
                                Command = File.ReadAllText path })

    availableMigrations |> Seq.filter (fun a -> not (executedMigrations |> 
                                                      Seq.exists(fun e -> e.Name = a.Name) ))

let private executePendingMigrations conn pendingMigrations =
    pendingMigrations |> Seq.iter (fun m ->
                  use insertMigrationCmd = new NpgsqlCommand()
                  insertMigrationCmd.Connection <- conn
                  insertMigrationCmd.CommandText <- """ 
                    INSERT INTO public.__migrations (name, command)
                    VALUES (@Name, @Command)
                  """

                  let nameParam = insertMigrationCmd.Parameters.AddWithValue("@Name", m.Name)
                  let commandParam = insertMigrationCmd.Parameters.AddWithValue("@Command", m.Command)
                  insertMigrationCmd.ExecuteNonQuery() |> ignore
                  insertMigrationCmd.Parameters.Remove(nameParam) |> ignore
                  insertMigrationCmd.Parameters.Remove(commandParam) |> ignore

                  insertMigrationCmd.CommandText <- m.Command
                  insertMigrationCmd.ExecuteNonQuery() |> ignore )

let updateDatabase(migrationsPath) =
    let connString = getConnectionString()
    match connString with
        | Some connString ->
            use conn = new NpgsqlConnection(connString)
            conn.Open()
            getPendingMigrations conn migrationsPath |> 
            executePendingMigrations conn 
            conn.Close()
            Success() 
        | None -> Error (TitleMessages("failure on database migration", [|"no connection string defined"|]))
    