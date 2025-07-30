# 生成应用的整个流程
## 1. 部署目录结构
```bash
.
|-- Domain
|   |-- KC.DataAccess.Account
|   |-- KC.DataAccess.Admin
|   |-- KC.DataAccess.App
|   |-- KC.DataAccess.Blog
|   |-- KC.DataAccess.CodeGenerate
|   |-- KC.DataAccess.Config
|   |-- KC.DataAccess.Contract
|   |-- KC.DataAccess.Customer
|   |-- KC.DataAccess.Dict
|   |-- KC.DataAccess.Doc
|   |-- KC.DataAccess.Job
|   |-- KC.DataAccess.Message
|   |-- KC.DataAccess.Offering
|   |-- KC.DataAccess.Pay
|   |-- KC.DataAccess.Portal
|   |-- KC.DataAccess.Supplier
|   |-- KC.DataAccess.Training
|   |-- KC.DataAccess.Workflow
|-- Service
|   |-- KC.Service.Account
|   |-- KC.Service.Admin
|   |-- KC.Service.App
|   |-- KC.Service.Blog
|   |-- KC.Service.CodeGenerate
|   |-- KC.Service.Component
|   |-- KC.Service.Config
|   |-- KC.Service.Contract
|   |-- KC.Service.Customer
|   |-- KC.Service.Dict
|   |-- KC.Service.Doc
|   |-- KC.Service.Job
|   |-- KC.Service.Message
|   |-- KC.Service.Offering
|   |-- KC.Service.Pay
|   |-- KC.Service.Portal
|   |-- KC.Service.Supplier
|   |-- KC.Service.Training
|   |-- KC.Service.Workflow
|-- Test
|   |-- KC.Console.ApiTest
|   |-- KC.UnitTest
|   |-- KC.UnitTest.Account
|   |-- KC.UnitTest.Admin
|   |-- KC.UnitTest.App
|   |-- KC.UnitTest.Blog
|   |-- KC.UnitTest.CodeGenerate
|   |-- KC.UnitTest.Common
|   |-- KC.UnitTest.Config
|   |-- KC.UnitTest.Contract
|   |-- KC.UnitTest.Customer
|   |-- KC.UnitTest.Dict
|   |-- KC.UnitTest.Doc
|   |-- KC.UnitTest.Framework
|   |-- KC.UnitTest.Message
|   |-- KC.UnitTest.Offering
|   |-- KC.UnitTest.Pay
|   |-- KC.UnitTest.Portal
|   |-- KC.UnitTest.Storage
|   |-- KC.UnitTest.Supplier
|   |-- KC.UnitTest.ThirdParty
|   |-- KC.UnitTest.Training
|   |-- KC.UnitTest.WorkFlow
|   |-- KC.WebTest.Multitenancy
|-- Web
|   |-- KC.Web.Account
|   |-- KC.Web.Admin
|   |-- KC.Web.App
|   |-- KC.Web.Blog
|   |-- KC.Web.CodeGenerate
|   |-- KC.Web.Config
|   |-- KC.Web.Contract
|   |-- KC.Web.Customer
|   |-- KC.Web.Dict
|   |-- KC.Web.Doc
|   |-- KC.Web.Message
|   |-- KC.Web.Offering
|   |-- KC.Web.Pay
|   |-- KC.Web.Portal
|   |-- KC.Web.Resource
|   |-- KC.Web.SSO
|   |-- KC.Web.Supplier
|   |-- KC.Web.Training
|   |-- KC.Web.WeChat
|   |-- KC.Web.WorkFlow
|-- WebApi
|   |-- KC.WebApi.Account
|   |-- KC.WebApi.Admin
|   |-- KC.WebApi.App
|   |-- KC.WebApi.CodeGenerate
|   |-- KC.WebApi.Config
|   |-- KC.WebApi.Contract
|   |-- KC.WebApi.Customer
|   |-- KC.WebApi.Dict
|   |-- KC.WebApi.Doc
|   |-- KC.WebApi.Job
|   |-- KC.WebApi.Message
|   |-- KC.WebApi.Offering
|   |-- KC.WebApi.Pay
|   |-- KC.WebApi.Portal
|   |-- KC.WebApi.Supplier
|   |-- KC.WebApi.Training
|   |-- KC.WebApi.WorkFlow
```
## 2. 部署说明
有三台阿里云的服务器且都有公网IP，使用docker部署相关应用，按以下需求对所有应用进行打包、推送至阿里云仓库，生成一键打包发布shell脚本。
### 2.1 相关服务器的名称、IP及配置目录如下：
| 主机名      | 公网IP       | 内网IP        | 配置文件目录                      | 日志文件目录                      | 证书文件目录                      |
|-------------|--------------|---------------|----------------------------------|----------------------------------|----------------------------------|
| k8s-master  | 121.89.220.143 | 172.23.160.12 | /share/k8s-master/nginx/conf    | /share/k8s-master/nginx/logs    | /share/k8s-master/nginx/certs    |
| k8s-worker01| 47.92.221.208 | 172.23.160.15 | /share/k8s-worker01/nginx/conf  | /share/k8s-worker01/nginx/logs  | /share/k8s-worker01/nginx/certs  |
| k8s-worker02| 47.92.192.48  | 172.23.160.17 | /share/k8s-worker02/nginx/conf  | /share/k8s-worker02/nginx/logs  | /share/k8s-worker02/nginx/certs  |

###	 2.2 相关应用的部署如下：
| 部署服务器   | 应用编码   |应用名称   | 域名/泛域名              | 80端口   | 443端口   | 
|----------|----------|-----------|---------------------------|---------|---------|
| k8s-master  | KC.Web.Resource   | 资源应用   | resource.kcloudy.com    | 9999  | 10000  | 
| k8s-master  | KC.Web.SSO | SSO登录应用 | sso.kcloudy.com        | 1001  | 1011  | 
| k8s-master  | KC.Web.Admin | 后台管理应用 | admin.kcloudy.com        | 1003  | 1013  | 
| k8s-master  | KC.Web.Blog | 博客应用 | blog.kcloudy.com        | 1005  | 1015  | 
| k8s-master  | KC.Web.CodeGenerate | 代码生成应用 | code.kcloudy.com        | 1007  | 1017  | 
| k8s-master  | KC.Web.SSO | 登录应用 | *.sso.kcloudy.com        | 1001  | 1011  | 
| k8s-worker01  | KC.Web.Config | 配置中心 | *.cfg.kcloudy.com        | 1101  | 1111  | 
| k8s-worker01  | KC.Web.Dict | 字典管理 | *.dic.kcloudy.com        | 1103  | 1113  | 
| k8s-worker01  | KC.Web.App | 应用管理 | *.app.kcloudy.com        | 1105  | 1115  | 
| k8s-worker01  | KC.Web.Message | 消息中心 | *.msg.kcloudy.com        | 1107  | 1117  | 
| k8s-worker02  | KC.Web.Account | 用户权限应用 | *.acc.kcloudy.com      | 2001  | 2011  | 
| k8s-worker02  | KC.Web.Doc | 文件管理应用 | *.doc.kcloudy.com      | 2005  | 2015  | 
| k8s-worker02  | KC.Web.Portal | 网站应用 | *.portal.kcloudy.com      | 2007  | 2017  | 

###	 2.3 相关Api应用的部署如下：
| 部署服务器   | 应用编码   |应用名称   | 域名/泛域名              | 80端口   | 443端口   | 
|----------|----------|-----------|---------------------------|---------|---------|
| k8s-master  | KC.WebApi.Admin | 后台管理Api | adminapi.kcloudy.com        | 1004  | 1014  | 
| k8s-master  | KC.WebApi.Config | 配置中心Api | *.cfgapi.kcloudy.com        | 1102  | 1112  | 
| k8s-master  | KC.WebApi.Dict | 字典管理Api | *.dicapi.kcloudy.com        | 1104  | 1114  | 
| k8s-master  | KC.WebApi.App | 应用管理Api | *.appapi.kcloudy.com        | 1106  | 1116  | 
| k8s-worker01  | KC.WebApi.Message | 消息中心Api | *.msgapi.kcloudy.com        | 1108  | 1118  | 
| k8s-worker01  | KC.WebApi.Account | 用户权限Api | *.accapi.kcloudy.com      | 2002  | 2012  | 
| k8s-worker01  | KC.WebApi.Doc | 文件管理Api | *.docapi.kcloudy.com      | 2006  | 2016  | 
| k8s-worker02  | KC.WebApi.Customer | 客户管理Api | *.crmapi.kcloudy.com      | 3002  | 3012  | 
| k8s-worker02  | KC.WebApi.Portal | 网站管理Api | *.portalapi.kcloudy.com      | 4002  | 4012  | 
| k8s-worker02  | KC.WebApi.WorkFlow | 工作流Api | *.flowapi.kcloudy.com      | 7002  | 7012  | 


### 2.4 生成package并推送至阿里云私人仓库
将Web项目（项目编码）以版本号（版本号）打包至文件夹：D:\Publish\DotNet\{项目编码}，生成docker镜像并推送到阿里云私人仓库的过程，具体步骤如下：
#### 2.4.1 生成package
恢复Nuget包，发布项目到准备好的目录，复制字体文件和Dockerfile
* 进入项目目录
cd D:\Project\kcloudy\core\dotnet\kcloudy.business\Main\Source
* 恢复Nuget包
dotnet restore .\Web\{项目编码}\{项目编码}.csproj -s http://nexus.kcloudy.com/repository/nuget-hosted/index.json -s https://nuget.cnblogs.com/v3/index.json
* 发布项目到准备好的目录
dotnet publish .\Web\{项目编码}\{项目编码}.csproj -c Release -o D:\Publish\Release\{项目编码} /p:Version={版本号}
* 复制字体文件和Dockerfile
copy .\Fonts\* D:\Publish\Release\{项目编码}\Fonts
copy .\Web\{项目编码}\Dockerfile D:\Publish\Release\{项目编码}

#### 2.4.2 构建docker镜像
cd D:\Publish\Release\{项目编码}
docker build -t {项目编码}:{版本号}  --build-arg env=Production .
docker run -d -p 9999:9999 -p 10000:10000 --name {项目编码} {项目编码}:{版本号}

#### 2.4.3 推送docker镜像
docker tag {项目编码}:{版本号} registry.cn-zhangjiakou.aliyuncs.com/kcloudy-netcore/{项目编码}:{版本号}
docker push registry.cn-zhangjiakou.aliyuncs.com/kcloudy-netcore/{项目编码}:{版本号}

#### 2.4.4 具体示例
```bash
# 1.生成package
# 1.1 设置dotnet sdk版本
cd D:\Project\kcloudy\core\dotnet\kcloudy.business\Main\Source
dotnet new globaljson --sdk-version 5.0.400
# 1.2 恢复Nuget包
dotnet restore .\Web\KC.Web.Resource\KC.Web.Resource.csproj -s http://nexus.kcloudy.com/repository/nuget-hosted/index.json -s https://nuget.cnblogs.com/v3/index.json
# 1.3 publish发布项目到准备好的目录
dotnet publish .\Web\KC.Web.Resource\KC.Web.Resource.csproj -c Release -o D:\Publish\Release\KC.Web.Resource /p:Version=1.0.0.1
copy .\Fonts\* D:\Publish\Release\KC.Web.Resource\Fonts
copy .\Web\KC.Web.Resource\Dockerfile D:\Publish\Release\KC.Web.Resource

# 2.构建docker镜像
cd D:\Publish\Release\KC.Web.Resource
docker build -t kc.web.resource:1.0.0.1  --build-arg env=Production .
docker run -d -p 9999:9999 -p 10000:10000 --name kc.web.resource kc.web.resource:1.0.0.1

# 3.推送docker镜像
docker tag kc.web.resource:1.0.0.1 registry.cn-zhangjiakou.aliyuncs.com/kcloudy-netcore/kc.web.resource:1.0.0.1
docker push registry.cn-zhangjiakou.aliyuncs.com/kcloudy-netcore/kc.web.resource:1.0.0.1

```
