@echo off
".nuget\NuGet.exe" "Install" "FAKE.Core" "-OutputDirectory" "packages" "-ExcludeVersion" "-Source" "https://www.nuget.org/api/v2"
"packages\FAKE.Core\tools\Fake.exe" build.fsx
pause