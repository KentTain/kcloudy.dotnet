# build-all-webs.ps1

# load Build-DotnetWeb function
. "$PSScriptRoot\build-dotnet-web.ps1"

# define project list (structured way)
$projects = @(
    [PSCustomObject]@{ Name = "KC.Web.Resource"; HttpPort = 9999; HttpsPort = 10000; Enabled = $true },
    [PSCustomObject]@{ Name = "KC.Web.SSO";      HttpPort = 1001; HttpsPort = 1011;  Enabled = $true },
    [PSCustomObject]@{ Name = "KC.Web.Account";  HttpPort = 2001; HttpsPort = 2011;  Enabled = $true },
    [PSCustomObject]@{ Name = "KC.Web.Admin";    HttpPort = 1003; HttpsPort = 1013;  Enabled = $false },
    [PSCustomObject]@{ Name = "KC.Web.Blog";     HttpPort = 1005; HttpsPort = 1015;  Enabled = $false },
    [PSCustomObject]@{ Name = "KC.Web.CodeGenerate"; HttpPort = 1007; HttpsPort = 1017; Enabled = $false },
    [PSCustomObject]@{ Name = "KC.Web.Config";   HttpPort = 1101; HttpsPort = 1111;  Enabled = $false },
    [PSCustomObject]@{ Name = "KC.Web.Dict";     HttpPort = 1103; HttpsPort = 1113;  Enabled = $false },
    [PSCustomObject]@{ Name = "KC.Web.App";      HttpPort = 1105; HttpsPort = 1115;  Enabled = $false },
    [PSCustomObject]@{ Name = "KC.Web.Message";  HttpPort = 1107; HttpsPort = 1117;  Enabled = $false },
    [PSCustomObject]@{ Name = "KC.Web.Doc";      HttpPort = 2005; HttpsPort = 2015;  Enabled = $false },
    [PSCustomObject]@{ Name = "KC.Web.Portal";   HttpPort = 2007; HttpsPort = 2017;  Enabled = $false }
)

# filter enabled projects
$targetProjects = $projects | Where-Object { $_.Enabled }

# loop build
foreach ($project in $targetProjects) {
    Write-Host "🚀 build: $($project.Name)" -ForegroundColor Cyan
    try {
        Build-DotnetWeb `
            -solutionType "Web" `
            -solutionName $project.Name `
            -httpPort $project.HttpPort `
            -httpsPort $project.HttpsPort `
            -versionNum 1 `
            -env "Production"

        Write-Host "✅ success: $($project.Name)" -ForegroundColor Green
        Set-Location "D:\Project\kcloudy\core\dotnet\kcloudy.business\Main\Source"
    }
    catch {
        Write-Host "❌ failed: $($project.Name) - error: $_`n" -ForegroundColor Red
        Set-Location "D:\Project\kcloudy\core\dotnet\kcloudy.business\Main\Source"
    }
}
Write-Host "✅ all projects built successfully" -ForegroundColor Green