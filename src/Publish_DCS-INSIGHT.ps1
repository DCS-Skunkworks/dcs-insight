#Declaring & setting some variables
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$publishPath = $scriptPath + "\_PublishTemp_"
$clientPublishPath = $scriptPath + "\_PublishTemp_\client"
$serverPublishPath = $scriptPath + "\_PublishTemp_\server"
$clientPath = $scriptPath + "\client\DCSInsight"
$serverPath = $scriptPath + "\server\Scripts"

#---------------------------------
# Pre-checks
#---------------------------------
#Checking destination folder first
if (($null -eq $env:dcsinsightReleaseDestinationFolderPath) -or (-not (Test-Path $env:dcsinsightReleaseDestinationFolderPath))) {
    Write-Host "Fatal error. Destination folder does not exists. Please set environment variable 'dcsinsightReleaseDestinationFolderPath' to a valid value" -foregroundcolor "Red"
    exit
}

$changeProjectVersion = Read-Host "Change Project Version? Y/N"

if($changeProjectVersion.Trim().ToLower().Equals("y"))
{
    #---------------------------------
    # Release version management
    #---------------------------------
    Write-Host "Starting release version management" -foregroundcolor "Green"
    #Get Path to csproj
    $projectFilePath = $clientPath + "\DCSInsight.csproj"
    If (-not(Test-Path $projectFilePath)) {
        Write-Host "Fatal error. Project path not found: $projectPath" -foregroundcolor "Red"
        exit
    }

    #Readind project file
    $xml = [xml](Get-Content $projectFilePath)
    [string]$assemblyVersion = $xml.Project.PropertyGroup.AssemblyVersion

    #Split the Version Numbers
    $avMajor, $avMinor, $avPatch = $assemblyVersion.Split('.')

    Write-Host "Current assembly version is: $assemblyVersion" -foregroundcolor "Green"

    #Sets new version into Project 
    #Warning: for this to work, since the PropertyGroup is indexed, AssemblyVersion must be in the FIRST Propertygroup (or else, change the index).
    Write-Host "What kind of release is this? If not minor then patch version will be incremented." -foregroundcolor "Green"
    $isMinorRelease = Read-Host "Minor release? Y/N"

    if ($isMinorRelease.Trim().ToLower().Equals("y")) {
        [int]$avMinor = [int]$avMinor + 1
        [int]$avPatch = 0
    }
    else {
        [int]$avPatch = [int]$avPatch + 1
    }

    #Sets new version into Project 
    #Warning: for this to work, since the PropertyGroup is indexed, AssemblyVersion must be in the FIRST Propertygroup (or else, change the index).
    $xml.Project.PropertyGroup.AssemblyVersion = "$avMajor.$avMinor.$avPatch".Trim()
    [string]$assemblyVersion = $xml.Project.PropertyGroup.AssemblyVersion
    Write-Host "New assembly version is $assemblyVersion" -foregroundcolor "Green"

    #Saving project file
    $xml.Save($projectFilePath)
    Write-Host "Project file updated" -foregroundcolor "Green"
    Write-Host "Finished release version management" -foregroundcolor "Green"
}
#---------------------------------
# Client publishing
#---------------------------------
#Cleaning previous publish
Write-Host "Starting cleaning previous build" -foregroundcolor "Green"
Set-Location -Path $clientPath

#Check that client folder exists
if (Test-Path -Path $clientPublishPath) {
    Write-Host "client folder exists" -foregroundcolor "Green"
}
else {
    Write-Host "Creating client folder" -foregroundcolor "Green"
    New-Item -ItemType Directory -Path $clientPublishPath
}

dotnet clean DCSInsight.csproj -o $clientPublishPath

Write-Host "Starting Publish" -foregroundcolor "Green"
Set-Location -Path $clientPath

Write-Host "Starting Publish DCS-INSIGHT" -foregroundcolor "Green"
dotnet publish DCSInsight.csproj --self-contained false -f net6.0-windows -r win-x64 -c Release -o $clientPublishPath /p:DebugType=None /p:DebugSymbols=false
$buildLastExitCode = $LastExitCode

Write-Host "Build DCS-INSIGHT LastExitCode: $buildLastExitCode" -foregroundcolor "Green"

if ( 0 -ne $buildLastExitCode ) {
    Write-Host "Fatal error. Build seems to have failed on DCS-INSIGHT. No Zip & copy will be done." -foregroundcolor "Red"
    exit
}

#Getting file info & remove revision from file_version
Write-Host "Getting file info" -foregroundcolor "Green"
$file_version = (Get-Command $clientPublishPath\dcs-insight.exe).FileVersionInfo.FileVersion
Write-Host "File version is $file_version" -foregroundcolor "Green"

#---------------------------------
# Server publishing
#---------------------------------
if (Test-Path -Path $serverPublishPath) {
    Write-Host "server folder exists" -foregroundcolor "Green"
}
else {
    Write-Host "Creating server folder" -foregroundcolor "Green"
    New-Item -ItemType Directory -Path $serverPublishPath
}

Write-Host "Removing old files from server folder" -foregroundcolor "Green"
Remove-Item $serverPublishPath\* -Recurse

Write-Host "Copying DCS-INSIGHT files to server folder" -foregroundcolor "Green"
Copy-Item -Path $serverPath\* -Destination $serverPublishPath -Recurse

#---------------------------------
# Zipping
#---------------------------------
#Compressing release folder to destination
Write-Host "Destination for zip file:" $env:dcsinsightReleaseDestinationFolderPath"\dcs-insight_x64_$file_version.zip" -foregroundcolor "Green"
Compress-Archive -Force -Path $publishPath\* -DestinationPath $env:dcsinsightReleaseDestinationFolderPath"\dcs-insight_x64_$file_version.zip"

Write-Host "Finished publishing release version" -foregroundcolor "Green"

Write-Host "Script end" -foregroundcolor "Green"
