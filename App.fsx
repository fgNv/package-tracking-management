#I "packages/Owin/lib/net40"
#r "packages/Owin/lib/net40/Owin.dll"

#I "packages/Suave/lib/net40"
#r "packages/Suave/lib/net40/Suave.dll"


#r "packages/Aether/lib/net35/Aether.dll"
#I "packages/Aether/lib/net35"

#r "packages/Chiron/lib/net40/Chiron.dll"

#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"

#I "packages/Microsoft.Owin.Security/lib/net45"
#r "packages/Microsoft.Owin.Security/lib/net45/Microsoft.Owin.Security.dll"

#I "packages/Newtonsoft.Json/lib/net45"
#r "packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

#I "packages/Microsoft.Owin/lib/net45"
#r "packages/Microsoft.Owin/lib/net45/Microsoft.Owin.dll"

#r "fake-dist/PackageTrackingManagement.Domain.dll"
#r "fake-dist/PackageTrackingManagement.Persistence.dll"
#r "fake-dist/Suave.BearerTokenAuthentication.dll"

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

let frontEndDirectory = Path.Combine(__SOURCE_DIRECTORY__, 
                                     "package-tracking-management-view",
                                     "dist")

let config = 
    let port = System.Environment.GetEnvironmentVariable("PORT")
    let ip127  = IPAddress.Parse("127.0.0.1")
    let ipZero = IPAddress.Parse("0.0.0.0")

    { defaultConfig with 
        bindings=[ (if port = null then HttpBinding.create HTTP ip127 (uint16 8080)
                    else HttpBinding.create HTTP ipZero (uint16 port)) ]
        homeFolder = Some(frontEndDirectory) }
        
startWebServer config Routes.apiRoutes
