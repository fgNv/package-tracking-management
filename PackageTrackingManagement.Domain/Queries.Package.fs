module Queries.Package

open System

module List =
    open Railroad

    type Query = { ItemsPerPage : int; 
                   Page : int
                   NameFilter : string option }

    type Package = { Name: string
                     Id : Guid
                     Description: string option
                     CreatedAt : DateTime
                     UpdatedAt : DateTime }

    type QueryResult =  { Items : Package list
                          Total : int }

    let handle getPackageList (query : Query) : Result<QueryResult> = 
        getPackageList query

module Details =
    open Railroad
    open Chiron
    open Chiron.Operators

    type Query = { PackageId : Guid}    

    type ManualPoint = { CreatedAt : DateTime
                         Latitude : double
                         Longitude : double }
        with static member ToJson(x : ManualPoint) =
               Json.write "createdAt" x.CreatedAt
            *> Json.write "latitude" x.Latitude
            *> Json.write "longitude" x.Longitude

    type DevicePoint = { CreatedAt : DateTime
                         Latitude : double
                         Longitude : double
                         DeviceId : Guid }
        with static member ToJson(x : DevicePoint) =
               Json.write "createdAt" x.CreatedAt
            *> Json.write "latitude" x.Latitude
            *> Json.write "longitude" x.Longitude
            *> Json.write "deviceId" x.DeviceId

    type QueryResult = 
                   { Name: string
                     Id : Guid
                     Description: string option
                     CreatedAt : DateTime
                     UpdatedAt : DateTime
                     ManualPoints : ManualPoint seq
                     DevicePoints : DevicePoint seq }
        with static member ToJson(x : QueryResult) =
               Json.write "id" x.Id
            *> Json.write "description" x.Description
            *> Json.write "createdAt" x.CreatedAt
            *> Json.write "name" x.Name
            *> Json.write "updatedAt" x.UpdatedAt
            *> Json.write "manualPoints" (x.ManualPoints |> Seq.toList)
            *> Json.write "devicePoints" (x.DevicePoints |> Seq.toList)  

    let handle getPackageDetails (query : Query) : Result<QueryResult option> = 
        getPackageDetails query
