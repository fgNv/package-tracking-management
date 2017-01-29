module PgSqlPersistence

open System.IO
open Railroad
open FSharp.Data.Sql
open System

[<Literal>]
let private ConnectionString = @"User ID=homestead;Password=secret;
                                     Host=192.168.36.36;Port=5432;
                                     Database=package_tracking_management;"

[<Literal>]
let private ResolutionPath = @"..\..\..\packages\Npgsql.3.1.9\lib\net451\Npgsql.dll"

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
        | ex -> Error (Sentences.Error.DatabaseFailure, 
                       Error.getExceptionMessages ex)
