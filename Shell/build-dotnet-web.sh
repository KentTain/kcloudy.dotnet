#!/bin/bash

# === 获取当前主机名 ===
CURRENT_HOST=$(hostname)

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m'
BOLD='\033[1m'

# 打印信息（蓝色）
print_info() {
    echo -e "${BLUE}💡 [INFO]: $1${NC}"
}
# 打印警告（黄色）
print_warn() {
    echo -e "${YELLOW}⚠️ [WARNING]: $1${NC}"
}
# 打印错误（红色）
print_error() {
    echo -e "${RED}❌ [ERROR]: $1${NC}"
}
# 打印成功（绿色）
print_success() {
    echo -e "${GREEN}✅ [SUCCESS]: $1${NC}"
}

# === 构建docker镜像 ===
# sh build-dotnet-web.sh KC.Web.Resource 1 9999 0 "Production"
# 参数:
#   $1: solutionName - 要构建的解决方案名称
#   $2: versionNum - 版本号
#   $3: httpPort - HTTP 端口
#   $4: httpsPort - HTTPS 端口
#   $5: env - 发布环境
build_dotnet_web() {
    local solutionName=${1}
    local versionNum=${2:-1}
    local httpPort=${3}
    local httpsPort=${4:-0}
    local env=${5:-"Production"}

    # 定义变量
    local csprojDir="../Web/${solutionName}/${solutionName}.csproj"
    local localNuget="http://nexus.kcloudy.com/repository/nuget-hosted/index.json"
    local publicNuget="https://api.nuget.org/v3/index.json"
    local containerName=$(echo "$solutionName" | tr '[:upper:]' '[:lower:]')
    local acrName="kcloudy-netcore"  # ACR 名称
    local imageName="${acrName}/${containerName}"
    local newVersion="1.0.0.${versionNum}"
    local lastNum=$((versionNum - 1))
    local lastVersion="1.0.0.${lastNum}"
    local webDir="D:/Publish/DotNet/${solutionName}/v-${newVersion}"
    local oldwebDir="D:/Publish/DotNet/${solutionName}/v-${lastVersion}"
    local archivesDir="D:/Publish/DotNet/archives"

    print_info "更新项目: ${solutionName} 从上一个版本: ${lastVersion} 到新版本: ${newVersion}"

    # 创建并清理发布目录
    print_info "发布dotnetcore项目: ${solutionName} 至目录: ${webDir}"
    mkdir -p "${webDir}"
    rm -rf "${webDir:?}/"*

    # 还原 NuGet 包并发布项目
    print_info "还原 NuGet 包: dotnet restore ${csprojDir} -s ${localNuget} -s ${publicNuget}"
    dotnet restore "${csprojDir}" -s "${localNuget}" -s "${publicNuget}"

    print_info "发布项目: dotnet publish ${csprojDir} -c Release -o ${webDir} /p:Version=${newVersion}"
    dotnet publish "${csprojDir}" -c Release -o "${webDir}" /p:Version="${newVersion}"

    # 归档发布后的文件
    mkdir -p "${archivesDir}"
    print_info "创建归档文件..."
    tar -zcvf "${archivesDir}/${solutionName}-${newVersion}.tar.gz" -C "${webDir}" .

    # 检查并停止/删除现有容器
    if [ "$(docker ps -aq -f name=${containerName})" ]; then
        print_info "停止并删除 docker 容器: ${containerName}"
        docker stop "${containerName}"
        docker rm -f "${containerName}"
    fi

    # 删除旧镜像
    if [ "$(docker images -q ${imageName}:${lastVersion})" ]; then
        print_info "删除镜像: ${imageName}:${lastVersion}"
        docker rmi -f "${imageName}:${lastVersion}"
    fi

    # 构建新镜像
    print_info "构建新镜像: docker build -t ${imageName}:${newVersion} --build-arg env=${env} ."
    cd "${webDir}" || exit
    docker build -t "${imageName}:${newVersion}" --build-arg "env=${env}" .

    # 检查端口可用性
    function test_port_available {
        local port=$1
        (echo > /dev/tcp/127.0.0.1/"$port") &>/dev/null && return 0 || return 1
    }

    originalPort=$httpPort
    maxAttempts=10
    attempt=0
    portAvailable=false

    while [ $attempt -lt $maxAttempts ]; do
        if test_port_available "$httpPort"; then
            portAvailable=true
            break
        fi
        print_warn "端口 $httpPort 不可用，尝试下一个端口..."
        httpPort=$((httpPort + 1))
        attempt=$((attempt + 1))
    done

    if [ "$portAvailable" = false ]; then
        print_error "无法找到可用端口，最后尝试的端口: $httpPort"
        exit 1
    fi

    if [ "$httpPort" -ne "$originalPort" ]; then
        print_warn "使用端口 $httpPort 替代 $originalPort"
    fi

    # 运行容器
    dockerRunCmd=("run" "-d" "-p" "${httpPort}:${originalPort}")
    if [ "$httpsPort" -ge 0 ]; then
        dockerRunCmd+=("-p" "${httpsPort}:${httpsPort}")
    fi
    dockerRunCmd+=("--name" "${containerName}" "${imageName}:${newVersion}")

    print_info "运行容器: docker ${dockerRunCmd[*]}"
    docker "${dockerRunCmd[@]}"

    # 清理旧版本
    if [ -d "$oldwebDir" ]; then
        print_info "清理旧版本文件夹: $oldwebDir"
        rm -rf "$oldwebDir"
    fi

    # Tag and push to Alibaba Cloud Container Registry
    registryImage="registry.cn-zhangjiakou.aliyuncs.com/$acrName/$containerName:$newVersion"
    print_info "Tagging image for ACR: docker tag $imageName:$newVersion $registryImage"
    docker tag "$imageName:$newVersion" "$registryImage"
    docker push "$registryImage"

    cd "D:\Project\kcloudy\core\dotnet\kcloudy.business\Main\Source" || exit
    print_success "部署成功！"
}

# 主程序逻辑
build_dotnet_web "$1" "$2" "$3" "$4" "$5"
