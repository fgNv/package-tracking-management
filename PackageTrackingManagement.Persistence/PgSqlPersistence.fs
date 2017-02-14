module PgSqlPersistence

open System.IO
open FSharp.Data.Sql
open System

[<Literal>]
let internal ConnectionString = @"User ID=homestead;Password=secret;Host=192.168.36.36;Port=5432;Database=package_tracking_management;"
                                     
let internal GetConnectionString() =
    let connString = Environment.GetEnvironmentVariable("package_tracking_management_conn")
    match String.IsNullOrWhiteSpace connString with
        | true -> 
             Console.WriteLine("NO CONN IN ENV FOUND")
             // ConnectionString
             "User ID=gglwzecvflucps;Password=f4b3f542c0ce03e46ca4b48fe7bc43b8ab5fcb0b3930baa1b58ae7213c3ff887;Host=ec2-50-19-125-201.compute-1.amazonaws.com;Port=5432;Database=d27mor9hjptli4;"
        | false -> 
            Console.WriteLine("conn in env was successfully found")
            connString
                                               
