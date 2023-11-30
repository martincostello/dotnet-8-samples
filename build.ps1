#! /usr/bin/env pwsh

#Requires -PSEdition Core
#Requires -Version 7

param(
    [Parameter(Mandatory = $false)][string] $Configuration = "Release",
    [Parameter(Mandatory = $false)][string] $OutputPath = "",
    [Parameter(Mandatory = $false)][switch] $SkipTests
)

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

if ($null -eq $env:MSBUILDTERMINALLOGGER) {
    $env:MSBUILDTERMINALLOGGER = "auto"
}

dotnet build
dotnet test --configuration Release
dotnet publish
