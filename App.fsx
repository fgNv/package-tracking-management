#I "packages/FParsec/lib/net40-client"
#r "packages/FParsec/lib/net40-client/FParsec.dll"
#r "packages/FParsec/lib/net40-client/FParsecCS.dll"

#I "packages/Owin/lib/net40"
#r "packages/Owin/lib/net40/Owin.dll"

#I "packages/Microsoft.AspNet.Cors/lib/net45"
#r "packages/Microsoft.AspNet.Cors/lib/net45/System.Web.Cors.dll"

#I "packages/Microsoft.Owin.Cors/lib/net45"
#r "packages/Microsoft.Owin.Cors/lib/net45/Microsoft.Owin.Cors.dll"

#r "packages/SQLProvider/lib/FSharp.Data.SqlProvider.dll"

#I "packages/Npgsql/lib/net451"
#r "packages/Npgsql/lib/net451/Npgsql.dll"

#r "packages/Aether/lib/net35/Aether.dll"
#I "packages/Aether/lib/net35"

#I "packages/Chiron/lib/net40"
#r "packages/Chiron/lib/net40/Chiron.dll"

#I "packages/Owin.Security.AesDataProtectorProvider/lib/net45"
#r "packages/Owin.Security.AesDataProtectorProvider/lib/net45/Owin.Security.AesDataProtectorProvider.dll"

#I "packages/FSharp.Management/lib/net40"
#r "packages/FSharp.Management/lib/net40/FSharp.Management.dll"

#I "packages/Microsoft.Owin.Security/lib/net45"
#r "packages/Microsoft.Owin.Security/lib/net45/Microsoft.Owin.Security.dll"

#I "packages/Microsoft.Owin.Security.OAuth/lib/net45"
#r "packages/Microsoft.Owin.Security.OAuth/lib/net45/Microsoft.Owin.Security.OAuth.dll"

#I "packages/Suave/lib/net40"
#r "packages/Suave/lib/net40/Suave.dll"

#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"

#I "packages/Microsoft.Owin.Security/lib/net45"
#r "packages/Microsoft.Owin.Security/lib/net45/Microsoft.Owin.Security.dll"

#I "packages/Newtonsoft.Json/lib/net45"
#r "packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

#I "packages/Microsoft.Owin/lib/net45"
#r "packages/Microsoft.Owin/lib/net45/Microsoft.Owin.dll"

#r "ProjectDlls/PackageTrackingManagement.Domain.dll"
#r "ProjectDlls/PackageTrackingManagement.Persistence.dll"
#r "ProjectDlls/Suave.BearerTokenAuthentication.dll"

#load "PackageTrackingManagement.Web/Application.fs"
#load "PackageTrackingManagement.Web/JsonParse.fs"
#load "PackageTrackingManagement.Web/Claims.fs"
#load "PackageTrackingManagement.Web/Application.fs"

//#load "PackageTrackingManagement.Web/Hubs.fs"
//#load "PackageTrackingManagement.Web/SignalRConfiguration.fs"

#load "PackageTrackingManagement.Web/Routes.fs"

open Suave
open System
open System.Net
open System.IO

Migrations.updateDatabase(__SOURCE_DIRECTORY__ + "/Migrations")

if not (Application.User.Exists "master") then
        Application.User.Create { UserName = "master"
                                  Name = "Master"
                                  Email = "felipegarcia156@hotmail.com"
                                  Password = "777888"
                                  AccessType = Models.AccessType.Administrator
                                  CreatorId = Commands.User.Create.machineId } 
        |> ignore

let frontEndDirectory = Path.Combine(__SOURCE_DIRECTORY__, 
                                     "package-tracking-management-view",
                                     "dist")

let config = 
    let port = System.Environment.GetEnvironmentVariable("PORT")
    let ip127  = IPAddress.Parse("127.0.0.1")
    let ipZero = IPAddress.Parse("0.0.0.0")  

    { defaultConfig with 
        bindings=[ (if port = null then HttpBinding.create HTTP ip127 (uint16 8090)
                    else HttpBinding.create HTTP ipZero (uint16 port)) ]
        homeFolder = Some(frontEndDirectory) }
        
startWebServer config Routes.apiRoutes
