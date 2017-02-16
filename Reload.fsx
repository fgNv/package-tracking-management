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

#I "packages/FSharp.Core/lib/net40"

#I "packages/FAKE.Lib/lib/net451"
#r "packages/FAKE.Lib/lib/net451/FakeLib.dll"

open Fake
open System.IO

#load "PackageTrackingManagement.Persistence/PgSqlPersistence.fs"
#load "PackageTrackingManagement.Persistence/Migrations.fs"

Migrations.updateDatabase(Path.Combine(__SOURCE_DIRECTORY__ ,"Migrations"))

let buildDir = __SOURCE_DIRECTORY__ + "/fake-dist/"

let projectsSearchPattern = __SOURCE_DIRECTORY__ + "/**/*.fsproj"

Fake.MSBuildHelper.MSBuildLoggers <- []

Target "BuildApp" (fun _ ->
   !! projectsSearchPattern
     |> MSBuildRelease buildDir "Build"
     |> Log "AppBuild-Output: "
)
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)


Target "CopyDlls" (fun _ ->
    let dlls = [|"PackageTrackingManagement.Domain.dll"
                 "Suave.BearerTokenAuthentication.dll"
                 "PackageTrackingManagement.Persistence.dll"|]

    CreateDir ( Path.Combine(__SOURCE_DIRECTORY__, "ProjectDlls") )

    dlls |> Seq.iter (fun d ->
                        CopyFile ( Path.Combine(__SOURCE_DIRECTORY__, "ProjectDlls", d) ) 
                                 ( Path.Combine(__SOURCE_DIRECTORY__, buildDir, d) ))
)

Target "Default" (fun _ ->
    trace "Building application..."
)

Target "Watch" (fun _ ->
    use watcher = !! "PackageTrackingManagement.Domain/**/*.fs" |> WatchChanges (fun changes -> 
        tracefn "%A" changes
        RunTargetOrDefault "Default"
    )

    use watcher = !! "PackageTrackingManagement.Persistence/**/*.fs" |> WatchChanges (fun changes -> 
        tracefn "%A" changes
        RunTargetOrDefault "Default"
    )

    use watcher = !! "PackageTrackingManagement.Web/**/*.fs" |> WatchChanges (fun changes -> 
        tracefn "%A" changes
        RunTargetOrDefault "Default"
    )

    use watcher = !! "PackageTracking.BearerTokenAuthentication/**/*.fs" |> WatchChanges (fun changes -> 
        tracefn "%A" changes
        RunTargetOrDefault "Default"
    )

    System.Console.ReadLine() |> ignore //Needed to keep FAKE from exiting

    watcher.Dispose() // Use to stop the watch from elsewhere, ie another task.
)

"Clean"
  ==> "BuildApp"
  ==> "CopyDlls"
  ==> "Default"

RunTargetOrDefault "Watch"