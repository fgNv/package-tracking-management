open Suave
open Models
open Commands.User.Create
open System
open System.Net

[<EntryPoint>]
let main argv =     
    
    if not (Application.User.Exists "master") then
        Application.User.Create { UserName = "master"
                                  Name = "Master"
                                  Email = "felipegarcia156@hotmail.com"
                                  Password = "777888"
                                  AccessType = AccessType.Administrator
                                  CreatorId = Commands.User.Create.machineId } 
        |> ignore

    let config = { defaultConfig with 
                    bindings=[HttpBinding.create HTTP IPAddress.Any 8090us] }

    let app = Routes.apiRoutes
    startWebServer config app
    0 
