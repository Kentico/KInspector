#r @"packages/FAKE/tools/FakeLib.dll"
open Fake

// Properties
let outputDir = "./Bin/"

// Targets
Target "Clean" (fun _ ->
    CleanDir outputDir
)

Target "Compile" (fun _ ->
    !!"./KInspector.sln"
    |> MSBuildRelease "" "Rebuild"
    |> ignore
)

Target "Default" DoNothing

// Dependencies
"Clean"
  ==> "Compile"
  ==> "Default"

RunTargetOrDefault "Default"