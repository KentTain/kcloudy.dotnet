<#
.SYNOPSIS
Build and deploy .NET Web application to Docker container

.DESCRIPTION
This script is used to build .NET Web application, create Docker image, and run container.

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
.\build-dotnet-web.ps1 -solutionName "KC.Web.Resource" -versionNum 1 -httpPort 9999 -httpsPort 10000 -env "Production"
#>

param(
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
    Write-Host "----kcloudy: $message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$message)
    Write-Host "[SUCCESS] $message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$message)
    Write-Host "[WARNING] $message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$message)
    Write-Host "[ERROR] $message" -ForegroundColor Red
}

# Define deploy function
function Deploy-DotnetWeb {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$solutionName,
        
        [Parameter(Mandatory=$true)]
        [int]$versionNum,
        
        [Parameter(Mandatory=$true)]
        [int]$httpPort,
        
        [Parameter(Mandatory=$false)]
        [int]$httpsPort = 0,
        
        [Parameter(Mandatory=$false)]
        [string]$env = "Production"
    )

    # Define variables
    $csprojDir = ".\Web\$solutionName\$solutionName.csproj"
    $localNuget = "http://nexus.kcloudy.com/repository/nuget-hosted/index.json"
    $containerName = $solutionName.ToLower()
    $acrName = "kcloudy-netcore"  # Your ACR name
    $imageName = "$acrName/$containerName"
    $newVersion = "1.0.0.$versionNum"
    $lastNum = $versionNum - 1
    $lastVersion = "1.0.0.$lastNum"
    $webDir = "D:\Publish\Release\$solutionName\v-$newVersion"
    $oldwebDir = "D:\Publish\Release\$solutionName\v-$lastVersion"
    $archivesDir = "D:\Publish\Release\archives"

    Write-Info "Deploy project: $solutionName from lastVersion: $lastVersion to newVersion: $newVersion"

    # Set environment variable
    $env:ASPNETCORE_ENVIRONMENT = $env

    # Check and stop/remove existing container
    $existingContainer = docker ps -aq -f "name=$containerName"
    if ($existingContainer) {
        Write-Warning "Stopping and removing docker container: docker stop $containerName"
        docker stop $containerName | Out-Null
        docker rm -f $containerName | Out-Null
    }

    # Remove old image before Build new image
    $existingImage = docker images -q "$imageName`:$lastVersion"
    if ($existingImage) {
        Write-Warning "Removing image: docker rmi -f $imageName`:$lastVersion"
        docker rmi -f "$imageName`:$lastVersion" | Out-Null
    }

    $registryImage = "registry.cn-zhangjiakou.aliyuncs.com/$acrName/$containerName`:$newVersion"
    
    # Pull the latest image
    Write-Info "Pull image: docker pull $registryImage"
    docker pull "$registryImage"

    # Function to check if port is available
    function Test-PortAvailable {
        param([int]$Port)
        try {
            $listener = [System.Net.Sockets.TcpListener]::new([System.Net.IPAddress]::Any, $Port)
            $listener.Start()
            $listener.Stop()
            return $true
        } catch {
            return $false
        }
    }

    # Check if port is available, if not try next port
    $originalPort = $httpPort
    $maxAttempts = 10
    $attempt = 0
    $portAvailable = $false

    do {
        $attempt++
        if (Test-PortAvailable -Port $httpPort) {
            $portAvailable = $true
            break
        }
        Write-Warning "Port $httpPort is not available, trying next port..."
        $httpPort++
    } while ($attempt -lt $maxAttempts -and -not $portAvailable)

    if (-not $portAvailable) {
        throw "Could not find an available port after $maxAttempts attempts. Last port tried: $httpPort"
    }

    # Notify if port was changed
    if ($httpPort -ne $originalPort) {
        Write-Warning "Using port $httpPort instead of $originalPort"
    }

    # Add HTTPS port mapping if explicitly specified (not default value)
    if ($httpsPort -ge 0) {
        $dockerRunCmd = @("run", "-d", "-p", "${httpPort}:${originalPort}", "-p", "${httpsPort}:${httpsPort}")
    } else {
        $dockerRunCmd = @("run", "-d", "-p", "${httpPort}:${originalPort}")
    }

    # Add container name and image
    $dockerRunCmd += @("--name", $containerName, "$registryImage")
    
    # Log the command and run it
    $commandString = "docker " + ($dockerRunCmd -join " ")
    Write-Info "Running container: $commandString"
    & docker $dockerRunCmd
    
    # Show container logs
    docker logs $containerName
    
    Write-Success "Deployment completed successfully!"
}

# Main script
try {
    Deploy-DotnetWeb -solutionName $solutionName -versionNum $versionNum -httpPort $httpPort -httpsPort $httpsPort -env $env
    exit 0
} catch {
    Write-Error "Script execution failed: $_"
    exit 1
}