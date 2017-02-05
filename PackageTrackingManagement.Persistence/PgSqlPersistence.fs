module PgSqlPersistence

open System.IO
open Railroad
open FSharp.Data.Sql
open System

[<Literal>]
let internal ConnectionString = @"User ID=homestead;Password=secret;Host=192.168.36.36;Port=5432;Database=package_tracking_management;"
                                     
let internal GetConnectionString() =
    let connString = Environment.GetEnvironmentVariable("package_tracking_management_conn")
    match String.IsNullOrWhiteSpace connString with
        | true -> ConnectionString
        | false -> connString
                                               
