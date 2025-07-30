<#
.SYNOPSIS
Build and deploy .NET Web application to Docker container

.DESCRIPTION
This script is used to build .NET Web application, create Docker image, and run container.

.PARAMETER solutionType
Type of the solution to build, default is "Web"

.PARAMETER solutionName
Name of the solution to build

.PARAMETER versionNum
Version number, default is 1

.PARAMETER httpPort
http port to run the container on

.PARAMETER httpsPort
HTTPS port to run the container on, default is 0 (not specified)

.PARAMETER env
Deployment environment, default is "Production"

.EXAMPLE
.\build-dotnet-web.ps1 -solutionType "Web" -solutionName "KC.Web.Resource" -versionNum 1 -httpPort 9999 -httpsPort 10000 -env "Production"
#>

param(
    [Parameter(Mandatory=$false)]
    [string]$solutionType,
    
    [Parameter(Mandatory=$false)]
    [string]$solutionName,
    
    [Parameter(Mandatory=$false)]
    [int]$versionNum,
    
    [Parameter(Mandatory=$false)]
    [int]$httpPort,
    
    [Parameter(Mandatory=$false)]
    [int]$httpsPort = 0,

    [Parameter(Mandatory=$false)]
    [string]$env = "Production"
)

# Set console output encoding to UTF-8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
[System.Console]::OutputEncoding = [System.Text.Encoding]::UTF8
[System.Console]::InputEncoding = [System.Text.Encoding]::UTF8

# Set error action preference
$ErrorActionPreference = "Stop"

# Get current hostname
$CURRENT_HOST = $env:COMPUTERNAME

# Define output functions
function Write-Info {
    param([string]$message)
    Write-Host "💡 [INFO]: $message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$message)
    Write-Host "✅ [SUCCESS] $message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$message)
    Write-Host "⚠️ [WARNING] $message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$message)
    Write-Host "❌ [ERROR] $message" -ForegroundColor Red
}

# Define build and deploy function
function Restore-DotnetWeb {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$solutionType,

        [Parameter(Mandatory=$true)]
        [string]$solutionName,
        
        [Parameter(Mandatory=$true)]
        [int]$versionNum,

        [Parameter(Mandatory=$false)]
        [string]$env = "Production"
    )

    
    $solutionName = "KC.$solutionType.$solutionName"
    $versionNum = 1

    # Define variables
    if ($solutionType -eq "UnitTest") {
        $csprojDir = ".\Test\$solutionName\$solutionName.csproj"
    } else {
        $csprojDir = ".\$solutionType\$solutionName\$solutionName.csproj"
    }
    $localNuget = "https://nexus.kcloudy.com/repository/nuget-hosted/index.json"
    $publicNuget = "https://nuget.cnblogs.com/v3/index.json"
    $containerName = $solutionName.ToLower()
    $acrUrl = "registry.cn-zhangjiakou.aliyuncs.com"
    $acrName = "kcloudy-netcore"  # Your ACR name
    $imageName = "$acrName/$containerName"
    $newVersion = "1.0.0.$versionNum"
    $lastNum = $versionNum - 1
    $lastVersion = "1.0.0.$lastNum"
    $webDir = "D:\Publish\Release\$solutionName\v-$newVersion"
    $oldwebDir = "D:\Publish\Release\$solutionName\v-$lastVersion"
    $archivesDir = "D:\Publish\Release\archives"

    Write-Info "Update project: $solutionName from lastVersion: $lastVersion to newVersion: $newVersion"

    # Create and clean publish directory
    Write-Info "Publishing dotnetcore project: $solutionName to directory: $webDir"
    if (!(Test-Path -Path $webDir)) {
        New-Item -ItemType Directory -Path $webDir -Force | Out-Null
    } else {
        Remove-Item -Path "$webDir\*" -Recurse -Force
    }

    # Check and create global.json if it doesn't exist
    $globalJsonPath = Join-Path -Path $PSScriptRoot -ChildPath "global.json"
    if (-not (Test-Path -Path $globalJsonPath)) {
        Write-Info "Creating global.json with .NET 5.0.400 SDK"
        dotnet new globaljson --force --sdk-version 5.0.400 --output $PSScriptRoot
    }

    # Restore NuGet packages and publish project
    Write-Info "Restoring NuGet packages: dotnet restore $csprojDir -s $localNuget -s $publicNuget"
    dotnet restore $csprojDir -s $localNuget -s $publicNuget
    
    #Write-Info "Publishing project: dotnet publish $csprojDir -c Release -o $webDir /p:Version=$newVersion"
    #dotnet publish $csprojDir -c Release -o $webDir /p:Version=$newVersion

    Set-Location "D:\Project\kcloudy\core\dotnet\kcloudy.business\Main\Source"
    Write-Success "Deployment completed successfully!"
}

# Main script
try {

    # Job项目
    Restore-DotnetWeb -solutionType "Job" -solutionName "Basic" -versionNum 1 -env "Production"
    
    # Test项目
    dotnet restore .\Test\KC.Console.ApiTest\KC.Console.ApiTest.csproj -s https://nexus.kcloudy.com/repository/nuget-hosted/index.json -s https://nuget.cnblogs.com/v3/index.json
    dotnet restore .\Test\KC.WebTest.Multitenancy\KC.WebTest.Multitenancy.csproj -s https://nexus.kcloudy.com/repository/nuget-hosted/index.json -s https://nuget.cnblogs.com/v3/index.json

    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Account" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Admin" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "App" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Blog" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "CodeGenerate" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Common" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Config" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Contract" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Customer" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Dict" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Doc" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Framework" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Message" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Offering" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Pay" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Portal" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Storage" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Supplier" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "ThirdParty" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "Training" -versionNum 1 -env "Production"
    Restore-DotnetWeb -solutionType "UnitTest" -solutionName "WorkFlow" -versionNum 1 -env "Production"

    # # Web项目
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Account" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Admin" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "App" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Blog" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "CodeGenerate" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Config" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Contract" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Customer" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Dict" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Doc" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Message" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Offering" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Pay" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Portal" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Resource" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "SSO" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Supplier" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Training" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "WeChat" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "Web" -solutionName "Workflow" -versionNum 1 -env "Production"
    
    # # WebApi项目
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Account" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Admin" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "App" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "CodeGenerate" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Config" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Contract" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Customer" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Dict" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Doc" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Job" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Message" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Offering" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Pay" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Portal" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Supplier" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "Training" -versionNum 1 -env "Production"
    # Restore-DotnetWeb -solutionType "WebApi" -solutionName "WorkFlow" -versionNum 1 -env "Production"
    exit 0
} catch {
    Write-Error "Script execution failed: $_"
    Set-Location "D:\Project\kcloudy\core\dotnet\kcloudy.business\Main\Source"
    exit 1
}