Write-Output "Checking Tag Version"
$env:SEMVER_VERSION = $env:APPVEYOR_BUILD_VERSION
$env:DOTNET_VERSION = $env:APPVEYOR_BUILD_VERSION
$tagName = & git describe --abbrev=0 --tags

if ($env:APPVEYOR_REPO_TAG -eq $true) {
  $tagName = $env:APPVEYOR_REPO_TAG_NAME
}

Write-Output "Checking tag for SemVer naming"
$regex = new-object System.Text.RegularExpressions.Regex ('v(?<versionwithprerelease>(?<version>(?<major>[0-9]+).(?<minor>[0-9]+).(?<patch>[0-9]+))(-(?<prerelease>.+))?)', [System.Text.RegularExpressions.RegexOptions]::MultiLine)
$match = $regex.Match($tagName)
if($match.Success) {
  Write-Output "SemVer naming found: $tagName"
  $version = $match.Groups["version"].Value
  $versionwithprerelease = $match.Groups["versionwithprerelease"].Value

  $env:DOTNET_VERSION = "$version.$env:APPVEYOR_BUILD_NUMBER"

  if ($env:APPVEYOR_REPO_TAG -eq $true) {
    $env:SEMVER_VERSION = $versionwithprerelease
  } else {
    $env:SEMVER_VERSION = "$versionwithprerelease+dev.build.$env:APPVEYOR_BUILD_NUMBER"
  }

  Write-Output "Changing version '$env:APPVEYOR_BUILD_VERSION' to '$env:SEMVER_VERSION' based on tag"
  Update-AppveyorBuild -Version "$env:SEMVER_VERSION"
  Write-Output "DotNet version is: '$env:DOTNET_VERSION'"
}
