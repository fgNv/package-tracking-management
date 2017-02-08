module PgSqlProvider

open PgSqlPersistence
open System.IO
open Railroad
open FSharp.Data.Sql
open System


[<Literal>]
let private ResolutionPath = @"../packages/Npgsql/lib/net451/Npgsql.dll"

type internal PgsqlAccess = SqlDataProvider<Common.DatabaseProviderTypes.POSTGRESQL,
                                            ConnectionString,
                                            ResolutionPath = ResolutionPath>
                                               
let internal getContext() =
    let connString = Environment.GetEnvironmentVariable("package_tracking_management_conn")
    match String.IsNullOrWhiteSpace connString with
        | true -> PgsqlAccess.GetDataContext()
        | false -> PgsqlAccess.GetDataContext(connString)
        
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
