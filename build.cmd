@echo off
".nuget\NuGet.exe" "Install" "FAKE.Core" "-OutputDirectory" "packages" "-ExcludeVersion"
"packages\FAKE.Core\tools\Fake.exe" build.fsx
pause