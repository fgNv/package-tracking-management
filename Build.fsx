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

let replaceConnString () =
    ReplaceInFile 
        (fun content -> 
            if not isWindows then
                content.Replace("User ID=homestead;Password=secret;Host=192.168.36.36;Port=5432;Database=package_tracking_management;",
                                PgSqlPersistence.GetConnectionString())
            else
                content
        ) 
        (Path.Combine(__SOURCE_DIRECTORY__, 
                      "PackageTrackingManagement.Persistence", 
                      "PgSqlPersistence.fs"))

Target "BuildApp" (fun _ ->
   !! projectsSearchPattern
     |> MSBuildRelease buildDir "Build"
     |> Log "AppBuild-Output: "
)
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "AdaptToEnv" (fun _ ->
    replaceConnString ()
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

Target "UndoChanges" (fun _ ->
    let gitCmd  = tryFindFileOnPath "git"
    let modifiedFile = tryFindFileOnPath(
                             Path.Combine(__SOURCE_DIRECTORY__,
                                    "PackageTrackingManagement.Persistence",
                                    "PgSqlPersistence.fs"))
    match gitCmd, modifiedFile with 
        | None, _ 
        | _, None -> trace ("could not") 
        | Some f, Some g -> 
            Shell.Exec(g, "checkout",  f) |> ignore
)

Target "Default" (fun _ ->
    trace "Building application..."
)

"AdaptToEnv"
  ==> "Clean"
  ==> "BuildApp"
  ==> "CopyDlls"
  ==> "UndoChanges"
  ==> "Default"

RunTargetOrDefault "Default"