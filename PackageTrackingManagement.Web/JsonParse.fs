﻿module JsonParse

open System.Text
open Chiron
open Chiron.Mapping
open Chiron.Operators
open Sentences

let inline private deserializeJson builder bytes =
    try
        bytes |> Encoding.UTF8.GetString
              |> Json.parse
              |> builder
              |> function | Value r,_ -> Railroad.Success r 
                          | Error e,_ -> 
                            Railroad.Error 
                                (Railroad.TitleSentenceMessagesStr(Sentence.InvalidInputContent, [e]))
    with
        | ex -> Railroad.Error 
                    (Railroad.TitleSentenceMessagesStr(Sentences.InvalidInputContent, [ex.Message]))

module CreateUserCommand =
    open Commands.User.Create

    let deserialize currentUserId = 
        deserializeJson <| json { let! userName = Json.read "userName"
                                  let! name = Json.read "name"
                                  let! email = Json.read "email"
                                  let! password = Json.read "password"
                                  let! accessType = Json.read "accessType"
                                  
                                  return { UserName = userName
                                           Name = name
                                           Email = email
                                           Password = password
                                           AccessType = accessType
                                           CreatorId = currentUserId } }

module RemoveManualPoint =
    open Commands.Package.RemoveManualPoint

    let deserialize currentUserId =
        deserializeJson <| json { let! pointId = Json.read "pointId"

                                  return { PointId = pointId
                                           UserId  = currentUserId  } }

module UpdateUserCommand=
    open Commands.User.Update

    let deserialize currentUserId = 
        deserializeJson <| json { let! userName = Json.read "userName"
                                  let! name = Json.read "name"
                                  let! email = Json.read "email"
                                  let! accessType = Json.read "accessType"
                                  let! id = Json.read "id"
                                  
                                  return { UserName = userName
                                           Name = name
                                           Email = email
                                           Id = id
                                           AccessType = accessType
                                           CurrentUserId = currentUserId } }

module UpdatePackageCommand =
    open Commands.Package.Update
        
    let deserialize currentUserId = 
        deserializeJson <| json { let! name = Json.read "name"
                                  let! description = Json.tryRead "description"
                                  let! id = Json.read "id"
                                  
                                  return { Name = name
                                           Description = description
                                           CreatorId = currentUserId
                                           Id = id } }

module CreatePackageCommand =
    open Commands.Package.Create
    
    let deserialize currentUserId = 
        deserializeJson <| json { let! name = Json.read "name"
                                  let! description = Json.tryRead "description"
                                  
                                  return { Name = name
                                           Description = description
                                           CreatorId = currentUserId } }

module CreateManualPointCommand =
    open Commands.Package.AddManualPoint
    
    let deserialize currentUserId = 
        deserializeJson <| json { let! packageId = Json.read "packageId"
                                  let! latitude = Json.read "latitude"
                                  let! longitude = Json.read "longitude"
                                                                    
                                  return { PackageId = packageId
                                           Latitude = latitude
                                           Longitude = longitude
                                           CreatorId = currentUserId } }

module QueryResult =
    let inline serializeList (input : 'a list) =
        input |> Json.serialize
              |> Json.formatWith JsonFormattingOptions.Compact

    let inline serializeSeq input =
        input |> Seq.toList
              |> Json.serialize
              |> Json.formatWith JsonFormattingOptions.Compact

    let inline serializeOption (input : 'a option) =
        input |> Json.serialize
              |> Json.formatWith JsonFormattingOptions.Compact

    let inline serializeObj (input) =
        input |> Json.serialize
              |> Json.formatWith JsonFormattingOptions.Compact

module GrantPermission =
    open Commands.User.GrantPermission
    
    let deserialize = 
        deserializeJson <| json { let! packageId = Json.read "packageId"
                                  let! userId = Json.read "userId"
                                                                    
                                  return { PackageId = packageId
                                           UserId = userId } }

module RevokePermission =
    open Commands.User.RevokePermission
    
    let deserialize = 
        deserializeJson <| json { let! packageId = Json.read "packageId"
                                  let! userId = Json.read "userId"
                                                                    
                                  return { PackageId = packageId
                                           UserId = userId } }

type InvalidDataResponse = {Title : string 
                            Errors : string list }
        with static member ToJson(x : InvalidDataResponse) =
               Json.write "title" x.Title
            *> Json.write "errors" x.Errors