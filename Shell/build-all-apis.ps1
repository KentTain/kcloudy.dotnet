# Main.ps1

# Load the Build-DotnetWeb function (Assume build-dotnet-web.ps1 is in the same directory)
. "$PSScriptRoot\build-dotnet-web.ps1"

# Define project list (Structured format)
$projects = @(
    [PSCustomObject]@{ Name = "KC.WebApi.Account";     HttpPort = 2002; HttpsPort = 0; Enabled = $true }
    [PSCustomObject]@{ Name = "KC.WebApi.Admin";        HttpPort = 1004; HttpsPort = 0; Enabled = $true }
    [PSCustomObject]@{ Name = "KC.WebApi.Config";       HttpPort = 1102; HttpsPort = 0; Enabled = $false }
    [PSCustomObject]@{ Name = "KC.WebApi.Dict";         HttpPort = 1104; HttpsPort = 0; Enabled = $false }
    [PSCustomObject]@{ Name = "KC.WebApi.Message";      HttpPort = 1108; HttpsPort = 0; Enabled = $false }
    [PSCustomObject]@{ Name = "KC.WebApi.App";          HttpPort = 1106; HttpsPort = 0; Enabled = $false }
    [PSCustomObject]@{ Name = "KC.WebApi.Doc";          HttpPort = 2006; HttpsPort = 0; Enabled = $false }
    [PSCustomObject]@{ Name = "KC.WebApi.Customer";     HttpPort = 3002; HttpsPort = 0; Enabled = $false }
    [PSCustomObject]@{ Name = "KC.WebApi.Portal";       HttpPort = 4002; HttpsPort = 0; Enabled = $false }
    [PSCustomObject]@{ Name = "KC.WebApi.WorkFlow";     HttpPort = 7002; HttpsPort = 0; Enabled = $false }
)

# Filter out only enabled projects
$targetProjects = $projects | Where-Object { $_.Enabled }

# Loop and build each project
foreach ($project in $targetProjects) {
    Write-Host "🚀 Building project: $($project.Name)" -ForegroundColor Cyan
    try {
        Build-DotnetWeb `
            -solutionType "WebApi" `
            -solutionName $project.Name `
            -httpPort $project.HttpPort `
            -httpsPort $project.HttpsPort `
            -versionNum 1 `
            -env "Production"

        Write-Host "✅ Successfully built and deployed: $($project.Name)`n" -ForegroundColor Green
        Set-Location "D:\Project\kcloudy\core\dotnet\kcloudy.business\Main\Source"
    } catch {
        Write-Host "❌ Build failed: $($project.Name) - Error: $_`n" -ForegroundColor Red
        Set-Location "D:\Project\kcloudy\core\dotnet\kcloudy.business\Main\Source"
    }
}
Write-Host "✅ all projects built successfully" -ForegroundColor Green