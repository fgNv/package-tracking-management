module PgSqlProvider

open PgSqlPersistence
open System.IO
open Railroad
open FSharp.Data.Sql
open System
open System.IO


type internal PgsqlAccess = SqlDataProvider<Common.DatabaseProviderTypes.POSTGRESQL,
                                            ConnectionString>
                                               
let internal getContext() =
    let resolutionPath = Environment.GetEnvironmentVariable("npgsql_resolution_path")
    System.Console.WriteLine("npgsql resolution path -> " + resolutionPath)
    let connString = Environment.GetEnvironmentVariable("package_tracking_management_conn")
    match String.IsNullOrWhiteSpace connString with
        | true ->  System.Console.WriteLine("mano ):")
                   PgsqlAccess.GetDataContext()
        | false -> PgsqlAccess.GetDataContext(connString, resolutionPath)
        
let internal handleDatabaseException f input =
    try
        let result = f input
        Success result
    with
        | ex -> 
                System.Console.WriteLine(ex.Message)
                System.Console.WriteLine(ex.StackTrace)

                System.Console.WriteLine(Error.getExceptionMessages(ex))
                Error ("Sentences.Error.DatabaseFailure", 
                       Error.getExceptionMessages ex)
