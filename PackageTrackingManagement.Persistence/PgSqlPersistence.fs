module PgSqlPersistence

open System.IO
open Railroad
open FSharp.Data.Sql

[<Literal>]
let private ConnectionString = @"User ID=homestead;Password=secret;
                                     Host=192.168.36.36;Port=5432;
                                     Database=ingresso_2;"

[<Literal>]
let private ResolutionPath = @"..\..\..\packages\Npgsql.3.1.9\lib\net451\Npgsql.dll"

type private PgsqlAccess = SqlDataProvider<Common.DatabaseProviderTypes.POSTGRESQL,
                                           ConnectionString,
                                           ResolutionPath = ResolutionPath>