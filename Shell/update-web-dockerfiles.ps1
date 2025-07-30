# Script to update all Dockerfiles in web projects to match the template
# Template file: Web/KC.Web.Resource/Dockerfile

$ErrorActionPreference = "Stop"

# Get the template Dockerfile content with correct encoding
$projectRoot = Split-Path -Path $PSScriptRoot -Parent
$templatePath = Join-Path $projectRoot "Web\KC.Web.Resource\Dockerfile"
$templateContent = [System.IO.File]::ReadAllText($templatePath, [System.Text.Encoding]::UTF8)

# Get all web project directories
$webProjects = Get-ChildItem -Path (Join-Path $projectRoot "Web") -Directory | 
    Where-Object { $_.Name -ne "KC.Web.Resource" -and (Test-Path (Join-Path $_.FullName "Dockerfile")) }

foreach ($project in $webProjects) {
    $dockerfilePath = Join-Path $project.FullName "Dockerfile"
    $projectName = $project.Name
    
    Write-Host "Updating Dockerfile for $projectName..." -ForegroundColor Cyan
    
    # Get the DLL name (usually the project name without 'KC.Web.' prefix)
    $dllName = $projectName -replace '^KC\.Web\.', ''
    
    # Customize the template for this project
    $customizedContent = $templateContent -replace 'KC\.Web\.Resource', $projectName
    
    # Update the entrypoint to use the correct DLL name
    $entrypointPattern = 'ENTRYPOINT \[\"dotnet\", \"KC\\.Web\\.Resource\\.dll\"\]'
    $newEntrypoint = 'ENTRYPOINT ["dotnet", "KC.Web.' + $dllName + '.dll"]'
    $customizedContent = $customizedContent -replace $entrypointPattern, $newEntrypoint
    
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
    [System.IO.File]::WriteAllText($dockerfilePath, $content, $utf8NoBom)
    
    Write-Host "  -> Updated: $dockerfilePath" -ForegroundColor Green
}

Write-Host "\nAll Dockerfiles have been updated successfully!" -ForegroundColor Green
