﻿module Queries.Package

open System
open Chiron
open Chiron.Operators

module List =
    open Railroad

    type Query = { ItemsPerPage : int; 
                   Page : int
                   NameFilter : string option
                   CurrentUserId : Guid }

    type Package = { Name: string
                     Id : Guid
                     Description: string option
                     CreatedAt : DateTime
                     UpdatedAt : DateTime }
      with static member ToJson(x : Package) =
               Json.write "name" x.Name
            *> Json.write "id" x.Id
            *> Json.write "description" x.Description
            *> Json.write "createdAt" x.CreatedAt
            *> Json.write "updatedAt" x.UpdatedAt

    type QueryResult =  { Items : Package list
                          Total : int }
        with static member ToJson(x : QueryResult) =
               Json.write "items" x.Items
            *> Json.write "total" x.Total

    let handle getUserAccessType getPackageList getUserPackageList (query : Query) : Result<QueryResult> = 
        let userAccessType = getUserAccessType query.CurrentUserId
        match userAccessType with 
                | Models.AccessType.Administrator -> getPackageList query
                | Models.AccessType.User -> getUserPackageList query

module Details =
    open Railroad

    type Query = { PackageId : Guid}    

    type Position = {Latitude : double      
                     Longitude : double}
        with static member ToJson(x : Position) =
               Json.write "lat" x.Latitude
            *> Json.write "lng" x.Longitude

    type ManualPoint = { CreatedAt : DateTime
                         Position : Position
                         Id : Guid }
        with static member ToJson(x : ManualPoint) =
               Json.write "createdAt" x.CreatedAt
            *> Json.write "position" x.Position
            *> Json.write "id" x.Id

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
