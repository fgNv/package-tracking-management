#r "packages/FAKE.Lib/lib/net451/FakeLib.dll"

open Fake

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

Target "Default" (fun _ ->
    trace "Building application..."
)

"Clean"
  ==> "BuildApp"
  ==> "Default"

RunTargetOrDefault "Default"