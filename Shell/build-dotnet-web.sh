#!/bin/bash

# === 获取当前主机名 ===
CURRENT_HOST=$(hostname)

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

    echo "----kcloudy: 更新项目: ${solutionName} 从上一个版本: ${lastVersion} 到新版本: ${newVersion}"

    # 创建并清理发布目录
    echo "----kcloudy: 发布dotnetcore项目: ${solutionName} 至目录: ${webDir}"
    mkdir -p "${webDir}"
    rm -rf "${webDir:?}/"*

    # 还原 NuGet 包并发布项目
    echo "----kcloudy: 还原 NuGet 包: dotnet restore ${csprojDir} -s ${localNuget} -s ${publicNuget}"
    dotnet restore "${csprojDir}" -s "${localNuget}" -s "${publicNuget}"

    echo "----kcloudy: 发布项目: dotnet publish ${csprojDir} -c Release -o ${webDir} /p:Version=${newVersion}"
    dotnet publish "${csprojDir}" -c Release -o "${webDir}" /p:Version="${newVersion}"

    # 归档发布后的文件
    mkdir -p "${archivesDir}"
    echo "----kcloudy: 创建归档文件..."
    tar -zcvf "${archivesDir}/${solutionName}-${newVersion}.tar.gz" -C "${webDir}" .

    # 检查并停止/删除现有容器
    if [ "$(docker ps -aq -f name=${containerName})" ]; then
        echo "----kcloudy: 停止并删除 docker 容器: ${containerName}"
        docker stop "${containerName}"
        docker rm -f "${containerName}"
    fi

    # 删除旧镜像
    if [ "$(docker images -q ${imageName}:${lastVersion})" ]; then
        echo "----kcloudy: 删除镜像: ${imageName}:${lastVersion}"
        docker rmi -f "${imageName}:${lastVersion}"
    fi

    # 构建新镜像
    echo "----kcloudy: 构建新镜像: docker build -t ${imageName}:${newVersion} --build-arg env=${env} ."
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
        echo "----kcloudy: 端口 $httpPort 不可用，尝试下一个端口..."
        httpPort=$((httpPort + 1))
        attempt=$((attempt + 1))
    done

    if [ "$portAvailable" = false ]; then
        echo "----kcloudy: 无法找到可用端口，最后尝试的端口: $httpPort"
        exit 1
    fi

    if [ "$httpPort" -ne "$originalPort" ]; then
        echo "----kcloudy: 使用端口 $httpPort 替代 $originalPort"
    fi

    # 运行容器
    dockerRunCmd=("run" "-d" "-p" "${httpPort}:${originalPort}")
    if [ "$httpsPort" -ge 0 ]; then
        dockerRunCmd+=("-p" "${httpsPort}:${httpsPort}")
    fi
    dockerRunCmd+=("--name" "${containerName}" "${imageName}:${newVersion}")

    echo "----kcloudy: 运行容器: docker ${dockerRunCmd[*]}"
    docker "${dockerRunCmd[@]}"

    # 清理旧版本
    if [ -d "$oldwebDir" ]; then
        echo "----kcloudy: 清理旧版本文件夹: $oldwebDir"
        rm -rf "$oldwebDir"
    fi

    # Tag and push to Alibaba Cloud Container Registry
    registryImage = "registry.cn-zhangjiakou.aliyuncs.com/$acrName/$containerName`:$newVersion"
    Write-Info "Tagging image for ACR: docker tag $imageName`:$newVersion $registryImage"
    docker tag "$imageName`:$newVersion" $registryImage
    docker push "$registryImage"

    cd "D:\Project\kcloudy\core\dotnet\kcloudy.business\Main\Source"
    echo "----kcloudy: 部署成功！"
}

# 主程序逻辑
build_dotnet_web "$1" "$2" "$3" "$4" "$5"
