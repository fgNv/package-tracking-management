module PgSqlProvider

open PgSqlPersistence
open System.IO
open Railroad
open FSharp.Data.Sql
open System
open System.IO

[<Literal>]
let private ResolutionPath = @"/app/packages/Npgsql/lib/net451/Npgsql.dll"

type internal PgsqlAccess = SqlDataProvider<Common.DatabaseProviderTypes.POSTGRESQL,
                                            ConnectionString,
                                            ResolutionPath = ResolutionPath>
                                               
let internal getContext() =
    let connString = Environment.GetEnvironmentVariable("package_tracking_management_conn")
    match String.IsNullOrWhiteSpace connString with
        | true ->  System.Console.WriteLine("mano ):")
                   PgsqlAccess.GetDataContext()
        | false ->  let pathSeparator = Path.PathSeparator.ToString()
                    let resolutionPath = __SOURCE_DIRECTORY__ + pathSeparator +
                                              "packages" + pathSeparator +
                                              "Npgsql" + pathSeparator +
                                              "lib" + pathSeparator +
                                              "net451" + pathSeparator +
                                              "Npgsql.dll"
                    System.Console.WriteLine("relative path : =>" + resolutionPath)
                    PgsqlAccess.GetDataContext(connString, resolutionPath )
        
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
