open Suave
open Models
open Commands.User.Create
open System
open System.Net
open Suave.Successful
open System.IO

[<EntryPoint>]
let main argv =     

    Migrations.updateDatabase("..\..\..\Migrations")
    
    let frontEndDirectory = "C:\Projects\PackageTrackingManagement\package-tracking-management-view\dist"

//    let frontEndDirectory = Path.Combine("..\..\..", 
//                                         "package-tracking-management-view",
//                                         "dist")
        
    if not (Application.User.Exists "master") then
        Application.User.Create { UserName = "master"
                                  Name = "Master"
                                  Email = "felipegarcia156@hotmail.com"
                                  Password = "777888"
                                  AccessType = AccessType.Administrator
                                  CreatorId = Commands.User.Create.machineId } 
        |> ignore

    let config = { defaultConfig with 
                    bindings=[HttpBinding.create HTTP IPAddress.Any 8090us] 
                    homeFolder = Some(frontEndDirectory)}

    startWebServer config Routes.apiRoutes
    0 
