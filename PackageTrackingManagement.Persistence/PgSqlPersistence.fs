module PgSqlPersistence

open System.IO
open FSharp.Data.Sql
open System
                                     
let internal getConnectionString() =
    let connString = Environment.GetEnvironmentVariable("package_tracking_management_conn")
    match String.IsNullOrWhiteSpace connString with
        | true -> 
             Console.WriteLine("NO CONN IN ENV FOUND")
             None
        | false -> 
            Console.WriteLine("conn in env was successfully found")
            Some connString
                                               
