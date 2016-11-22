// include Fake lib
#r "packages/build/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing
open System
open System.IO

let artifactDir = "artifacts"
let solutionFile = "src/Reactive.Config.sln"
let testAssemblies = "src/**/bin/Release/*Tests*.dll"
MSBuildDefaults <- { MSBuildDefaults with Verbosity = Some MSBuildVerbosity.Minimal }

Target "Clean" (fun _ ->
    CleanDirs [artifactDir]
)

Target "Build" (fun _ ->
    !! solutionFile
    |> MSBuildRelease "" "Clean;Rebuild"
    |> ignore
)

Target "RunTests" (fun _ ->
    let nunitRunnerPath = "packages/build/NUnit.ConsoleRunner/tools/nunit3-console.exe"

    !! testAssemblies
    |> NUnit3 (fun p ->
        { p with
            OutputDir = artifactDir + "\TestResults.xml"
            ToolPath = nunitRunnerPath
        })
)

"Clean"
  ==> "Build"
  ==> "RunTests"

// start build
RunTargetOrDefault "RunTests"
