#r "packages/FAKE.Lib/lib/net451/FakeLib.dll"

open Fake

let buildDir = __SOURCE_DIRECTORY__ + "/fake-dist/"

let projectsSearchPattern = __SOURCE_DIRECTORY__ + "/**/*.fsproj"

Fake.MSBuildHelper.MSBuildLoggers <- []

let replaceConnString () =
    ReplaceInFile 
        (fun content -> 
            if not isWindows then
                content.Replace("User ID=homestead;Password=secret;Host=192.168.36.36;Port=5432;Database=package_tracking_management;",
                                @"User ID=qouqfyqvryymys;Password=3c24dfa6b67ec6f4fe0f5ce974d92db99aca53c4c37c92c7f73e92d0be176526;Host=ec2-184-72-252-69.compute-1.amazonaws.com;Port=5432;Database=df7djtoqireah4;")
            else
                content
        ) 
        (System.IO.Path.Combine(__SOURCE_DIRECTORY__, 
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

Target "Default" (fun _ ->
    trace "Building application..."
)

//"AdaptToEnv"
//  ==> 
"Clean"
  ==> "BuildApp"
  ==> "Default"

RunTargetOrDefault "Default"