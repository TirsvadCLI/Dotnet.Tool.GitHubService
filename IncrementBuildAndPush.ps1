param(
    # Path to the project file; adjust this default value if needed.
    [string]$ProjectFilePath = "src\GitHubService\TirsvadCLI.GitHubService\TirsvadCLI.GitHubService.csproj"
)

# Verify the project file exists.
if (!(Test-Path $ProjectFilePath)) {
    Write-Error "Project file does not exist at path: $ProjectFilePath"
    exit 1
}

# Load the project file as XML.
[xml]$projXml = Get-Content $ProjectFilePath

# Find the first PropertyGroup element that contains a VersionPrefix element.
$propertyGroup = $projXml.Project.PropertyGroup | Where-Object { $_.VersionPrefix }
if (-not $propertyGroup) {
    Write-Error "No <VersionPrefix> element found in the project file."
    exit 1
}

# Get the old version string.
$oldVersion = $propertyGroup.VersionPrefix
Write-Output "Current version: $oldVersion"

# Assume the version is in the format Major.Minor.Build (e.g., 0.1.0)
$versionParts = $oldVersion -split "\."
if ($versionParts.Length -ne 3) {
    Write-Error "Version format is not recognized. Expected format: Major.Minor.Build (e.g., 0.1.0)"
    exit 1
}

# Convert the Build part (third component) to an integer and increment it.
$major = $versionParts[0]
$minor = $versionParts[1]
$build = [int]$versionParts[2]
$build++

# Create a new version string.
$newVersion = "$major.$minor.$build"
$propertyGroup.VersionPrefix = $newVersion
Write-Output "Updated version: $newVersion"

# Save the updated project file.
$projXml.Save($ProjectFilePath)
Write-Output "Project file updated with new version."

# Now commit and push the changes using Git.
Write-Output "Staging changes..."
& git add $ProjectFilePath

Write-Output "Creating commit..."
& git commit -m "Bump build number to $newVersion"

Write-Output "Pushing to remote repository..."
& git push

Write-Output "Build number updated, committed, and pushed successfully."
