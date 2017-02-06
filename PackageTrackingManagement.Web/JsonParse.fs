module JsonParse

open System.Text
open Chiron
open Chiron.Mapping
open Chiron.Operators

let inline private deserializeJson builder bytes  =
     bytes |> Encoding.ASCII.GetString 
           |> Json.parse
           |> builder
           |> function | Value r,_ -> Railroad.Success r 
                       | Error e,_ -> Railroad.Error("Sentences.Error.InvalidInputContent", [e])

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