open Suave
open Models
open Commands.CreateUser
open System

[<EntryPoint>]
let main argv =     
    
    if not (Application.User.Exists "master") then
        Application.User.Create { UserName = "master"
                                  Name = "Master"
                                  Email = "felipegarcia156@hotmail.com"
                                  Password = "777888"
                                  AccessType = AccessType.Administrator
                                  CreatorId = Commands.CreateUser.machineId } 
        |> ignore

    let app = Routes.apiRoutes
    startWebServer defaultConfig app
    0 
