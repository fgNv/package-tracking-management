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
                       | Error e,_ -> Railroad.Error(Sentences.Error.InvalidInputContent, [e])

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