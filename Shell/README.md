# .NET 项目构建和部署脚本

## 1. 脚本目录结构

```bash
kcloudy.business
|-- Shell
|   |-- restore-dotnet-web.ps1     # 还原 .NET 项目依赖
|   |-- update-files.ps1           # 更新项目文件，包括：Dockerfile、nlog.config
|   |-- build-dotnet-web.ps1       # Windows 环境构建单个 .NET Web 项目
|   |-- build-dotnet-web.sh        # Linux 环境构建脚本
|   |-- build-all-webs.ps1         # 构建所有 Web 项目
|   |-- build-all-apis.ps1         # 构建所有 API 项目
|   |-- deploy-dotnet-web.sh       # 部署 .NET Web 应用到 Docker
|   |-- jenkins-build.sh           # Jenkins 构建脚本
```

## 2. 脚本说明

### 2.1 restore-dotnet-web.ps1

用于还原 .NET 项目的 NuGet 包依赖。

* **参数说明**
  * `solutionName`：指定解决方案名称

* **使用示例**
  ```powershell
  .\restore-dotnet-web.ps1 -solutionName "KC.Web.Resource"
  ```

### 2.2 update-files.ps1

更新项目中的标准文件模板。

* **功能**
  - 将标准文件（如 Dockerfile）复制到其他项目
  - 支持项目名称大小写转换
  - 可指定源文件和目标文件路径

* **参数说明**
  * `solutionType`：解决方案类型
  * `solutionName`：目标解决方案名称
  * `fileFullPath`：要复制的标准文件路径
  * `isLowercase`：是否将项目名转换为小写，默认为 false

* **使用示例**
  ```powershell
  .\update-files.ps1 -solutionType "Web" -solutionName "KC.Web.Resource" -fileFullPath "Dockerfile"
  ```


### 2.3 build-dotnet-web.ps1

构建单个 .NET Web 项目并创建 Docker 镜像。

* **参数说明**
  * `solutionType`：解决方案类型，默认为 "Web"
  * `solutionName`：要构建的解决方案名称（必需）
  * `versionNum`：版本号，默认为 1
  * `httpPort`：HTTP 端口号
  * `httpsPort`：HTTPS 端口号，默认为 0（不指定）
  * `env`：环境变量，如 "Production"

* **使用示例**
  ```powershell
  .\build-dotnet-web.ps1 -solutionType "Web" -solutionName "KC.Web.Resource" -httpPort 9999 -env "Production"
  ```

### 2.4 build-all-webs.ps1

批量构建所有启用的 Web 项目。

* **功能**
  - 自动构建所有启用的 Web 项目
  - 为每个项目分配指定的 HTTP/HTTPS 端口
  - 支持选择性启用/禁用项目

* **使用示例**
  ```powershell
  .\build-all-webs.ps1
  ```

### 2.5 build-all-apis.ps1

批量构建所有启用的 API 项目。

* **功能**
  - 自动构建所有启用的 API 项目
  - 为每个项目分配指定的 HTTP 端口
  - 支持选择性启用/禁用项目

* **使用示例**
  ```powershell
  .\build-all-apis.ps1
  ```

### 2.6 deploy-dotnet-web.sh

Linux 环境下部署 .NET Web 应用到 Docker 容器。

* **功能**
  - 构建 Docker 镜像
  - 停止并移除旧容器
  - 启动新容器
  - 支持环境变量配置

* **使用示例**
  ```bash
  ./deploy-dotnet-web.sh KC.Web.Resource 1 9999 0 "Production"
  ```

### 2.7 jenkins_build.sh

用于 Jenkins 持续集成的构建脚本。

* **功能**
  - 定义项目构建参数
  - 构建 Docker 镜像
  - 管理容器版本
  - 支持回滚到上一个版本

* **配置参数**
  ```bash
  solutionName="TestWeb"
  csprojDir="TestWeb/TestWeb.csproj"
  imagesPrx="kcloudy"
  containerName="testweb"
  port=5000
  ```


## 3. 环境要求

- .NET SDK 6.0 或更高版本
- PowerShell 7.0 或更高版本（Windows）
- Bash 环境（Linux/macOS）
- Docker 20.10 或更高版本

## 4. 使用建议

1. 在开发环境中，建议先使用 `restore-dotnet-web.ps1` 还原依赖
2. 使用 `build-dotnet-web.ps1` 测试单个项目的构建
3. 使用 `build-all-webs.ps1` 或 `build-all-apis.ps1` 批量构建项目
4. 在 CI/CD 流程中集成 `jenkins_build.sh` 或 `deploy-dotnet-web.sh`
5. 使用 `update-files.ps1` 保持项目模板文件的一致性


