#r @"packages/FAKE.Core/tools/FakeLib.dll"
open Fake

// Properties
let outputDir = "./Bin/"
let slnFile = "./KInspector.sln"

let defaultPackageSource = "https://www.nuget.org/api/v2"

// Targets
Target "Clean" (fun _ ->
    CleanDir outputDir
)

Target "RestorePackages" (fun _ ->
    slnFile
    |> RestoreMSSolutionPackages (fun p ->
        { p with
            Sources = defaultPackageSource :: p.Sources
        })
)

Target "Compile" (fun _ ->
    !!slnFile
    |> MSBuildRelease "" "Rebuild"
    |> ignore
)

Target "Default" DoNothing

// Dependencies
"Clean"
  ==> "RestorePackages"
  ==> "Compile"
  ==> "Default"

RunTargetOrDefault "Default"
