# Script to update all Dockerfiles in web projects to match the template
# Template file: Web/KC.Web.Resource/Dockerfile
<#
.SYNOPSIS
Update all files in web projects to other projects

.DESCRIPTION
This script is used to copy the stardard files to other projects, such as docerfile from KC.Web.Resource to other Web projects.

.PARAMETER solutionType
Type of the solution to build, default is "Web"

.PARAMETER solutionName
Name of the solution need to copy

.PARAMETER fileFullPath
Abstract full path of the stardard file need to copy, such as "Dockerfile" or "src\main\resources\logback.xml"

.PARAMETER isLowercase
is lowecase the project name, default is "false"

.EXAMPLE
.\update-files.ps1 -solutionType "Web" -solutionName "KC.Web.Resource" -fileFullPath "Dockerfile" -isLowercase $false
.\update-files.ps1 -solutionType "Web" -solutionName "KC.Web.Resource" -fileFullPath "nlog.config" -isLowercase $true
#>

param(
    [Parameter(Mandatory=$false)]
    [string]$solutionType,
    
    [Parameter(Mandatory=$false)]
    [string]$solutionName,
    
    [Parameter(Mandatory=$false)]
    [string]$fileFullPath,
    
    [Parameter(Mandatory=$false)]
    [bool]$isLowercase = $false
)

# Set console output encoding to UTF-8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
[System.Console]::OutputEncoding = [System.Text.Encoding]::UTF8
[System.Console]::InputEncoding = [System.Text.Encoding]::UTF8

$ErrorActionPreference = "Stop"

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

# copy the stardard of the solution to the other projects
function Update-Files {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$solutionType,

        [Parameter(Mandatory=$true)]
        [string]$solutionName,
        
        [Parameter(Mandatory=$true)]
        [string]$fileFullPath,
    
        [Parameter(Mandatory=$false)]
        [bool]$isLowercase = $false 
    )

    # Get the template file content with correct encoding
    $fileName = Split-Path -Path $fileFullPath -Leaf
    $filePath = Split-Path -Path $fileFullPath -Parent
    $projectRoot = Split-Path -Path $PSScriptRoot -Parent
    $copyFullPath = "$projectRoot\Web\$solutionName\$fileFullPath"
    $copyFileContent = [System.IO.File]::ReadAllText($copyFullPath, [System.Text.Encoding]::UTF8)

    # Get all solutionType project directories
    $solutionProjects = Get-ChildItem -Path (Join-Path $projectRoot "$solutionType") -Directory | 
        Where-Object { $_.Name -ne "$solutionName" -and (Test-Path (Join-Path $_.FullName "$fileFullPath")) }

    foreach ($project in $solutionProjects) {
        $projectName = $project.Name
        $lowcaseType = $solutionType.ToLower()
        $pasteFullPath = Join-Path $project.FullName "$fileFullPath"
        $mainName = $projectName -replace "^KC\.$solutionType\.", ""
        
        Write-Info "Copying $copyFullPath to $pasteFullPath in the project: $mainName of $projectName"

        # Customize the template for this project
        if ($isLowercase) {
            $customizedContent = $copyFileContent -replace 'kc\.web\.resource', $projectName.ToLower()
            $customizedContent = $customizedContent -replace [regex]::Escape('resource-'), ($mainName.ToLower() + '-')
        } else {
            $customizedContent = $copyFileContent -replace 'KC\.Web\.Resource', $projectName
            $customizedContent = $customizedContent -replace [regex]::Escape('Resource-'), ($mainName + '-')
        }

        # Fix line breaks in RUN commands
        $lines = $customizedContent -split "`n"
        $newContent = @()
        
        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]
            if ($line.TrimStart().StartsWith('RUN set -eux;')) {
                # For RUN set -eux; lines, ensure the next line starts with proper indentation
                $newContent += $line
                $nextLine = $lines[$i + 1]
                if (-not [string]::IsNullOrWhiteSpace($nextLine) -and $nextLine.Trim() -eq '\') {
                    # Skip the backslash line and add proper indentation to the next line
                    $i++
                    $newContent += "    $($lines[$i])"
                }
            } else {
                $newContent += $line
            }
        }
        
        # Join the lines back together
        $content = $newContent -join "`n"
        
        # Save the updated Dockerfile with UTF-8 without BOM encoding
        $utf8NoBom = New-Object System.Text.UTF8Encoding $false
        [System.IO.File]::WriteAllText($pasteFullPath, $content, $utf8NoBom)
        
        Write-Info "---Copied---> $pasteFullPath"
    }

    Write-Success "All $fileFullPath have been copied successfully!"
}

# Main script
try {
    # copy the Dockerfile of the KC.Web.Resource to all Web/WebApi projects
    Update-Files -solutionType "Web" -solutionName "KC.Web.Resource" -fileFullPath "Dockerfile" -isLowercase $true
    Update-Files -solutionType "WebApi" -solutionName "KC.Web.Resource" -fileFullPath "Dockerfile" -isLowercase $true
    
    # copy the nlog.config of the KC.Web.Resource to all Web/WebApi projects
    Update-Files -solutionType "Web" -solutionName "KC.Web.Resource" -fileFullPath "nlog.config" -isLowercase $true
    Update-Files -solutionType "WebApi" -solutionName "KC.Web.Resource" -fileFullPath "nlog.config" -isLowercase $true
    Set-Location "D:\Project\kcloudy\dotnet\Shell"
    exit 0
} catch {
    Write-Error "Script execution failed: $_"
    Set-Location "D:\Project\kcloudy\dotnet\Shell"
    exit 1
}