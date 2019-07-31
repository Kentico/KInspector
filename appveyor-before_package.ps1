Write-Output "---Creating Artifacts---"

# Build web application
Set-Location .\KenticoInspector.WebApplication
dotnet publish KenticoInspector.WebApplication.csproj /p:PublishDir=..\publish -c Release -r win-x64 --self-contained true

# Copy compiled front-end to publish folder
Set-Location .\ClientApp
mkdir "..\..\publish\dist"
Copy-Item ".\dist\*" -Recurse -Destination "..\..\publish\dist\"

# Go back to root
Set-Location ..\..\

# Create archive & push to Appveyor
$ filename = "Kentico Inspector $Env:SEMVER_VERSION.zip"
7z a $filename .\publish\*
appveyor PushArtifact $filename