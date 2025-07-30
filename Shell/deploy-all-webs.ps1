# Main.ps1

# 加载 Deploy-DotnetWeb 函数（假设 build-dotnet-web.ps1 在同一目录下）
. "$PSScriptRoot\deploy-dotnet-web.ps1"

# Get current hostname
$CURRENT_HOST = $env:COMPUTERNAME

# === 主程序逻辑 ===
switch ($CURRENT_HOST) {
    "k8s-master" {
        Deploy-DotnetWeb -solutionName "KC.Web.Resource" -versionNum 1 -httpPort 9999 -httpsPort 10000 -env "Production"
        Deploy-DotnetWeb -solutionName "KC.Web.SSO" -versionNum 1 -httpPort 1001 -httpsPort 1011 -env "Production"
        Deploy-DotnetWeb -solutionName "KC.Web.Admin" -versionNum 1 -httpPort 1003 -httpsPort 1013 -env "Production"
        Deploy-DotnetWeb -solutionName "KC.Web.Blog" -versionNum 1 -httpPort 1005 -httpsPort 1015 -env "Production"
        Deploy-DotnetWeb -solutionName "KC.Web.CodeGenerate" -versionNum 1 -httpPort 1007 -httpsPort 1017 -env "Production"
    }
    "k8s-worker01" {
        Deploy-DotnetWeb -solutionName "KC.Web.Config" -versionNum 1 -httpPort 1101 -httpsPort 1111 -env "Production"
        Deploy-DotnetWeb -solutionName "KC.Web.Dict" -versionNum 1 -httpPort 1103 -httpsPort 1113 -env "Production"
        Deploy-DotnetWeb -solutionName "KC.Web.App" -versionNum 1 -httpPort 1105 -httpsPort 1115 -env "Production"
    }
    "k8s-worker02" {
        Deploy-DotnetWeb -solutionName "KC.Web.Message" -versionNum 1 -httpPort 1107 -httpsPort 1117 -env "Production"
        Deploy-DotnetWeb -solutionName "KC.Web.Account" -versionNum 1 -httpPort 2001 -httpsPort 2011 -env "Production"
        Deploy-DotnetWeb -solutionName "KC.Web.Doc" -versionNum 1 -httpPort 2005 -httpsPort 2015 -env "Production"
        Deploy-DotnetWeb -solutionName "KC.Web.Portal" -versionNum 1 -httpPort 2007 -httpsPort 2017 -env "Production"
    }
    default {
        Write-Error "❌ 当前主机不在支持列表中，仅支持：k8s-master, k8s-worker01, k8s-worker02"
        exit
    }
}