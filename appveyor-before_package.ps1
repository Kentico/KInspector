Write-Output "---Creating Artifacts---"

# Build web application
Set-Location .\KenticoInspector.WebApplication
dotnet publish KenticoInspector.WebApplication.csproj /p:PublishDir=..\publish -c Release -r win-x64 --self-contained true

# Copy compiled front-end to publish folder
Set-Location .\ClientApp
mkdir "..\..\publish\ClientApp\dist"
Copy-Item ".\dist\*" -Recurse -Destination "..\..\publish\ClientApp\dist\"

# Go back to root
Set-Location ..\..\

# Create archive & push to Appveyor
7z a "Kentico Inspector $Env:SEMVER_VERSION.zip" .\publish\*
appveyor PushArtifact "Kentico Inspector $Env:SEMVER_VERSION.zip"