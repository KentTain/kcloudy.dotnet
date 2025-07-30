#!/bin/bash

# === 获取当前主机名 ===
CURRENT_HOST=$(hostname)
# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
BLUE='\033[0;36m'
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
deploy_dotnet_web() {
    local solutionName=${1}
    local versionNum=${2:-1}
    local httpPort=${3}
    local httpsPort=${4:-0}
    local env=${5:-"Production"}

    # 定义变量
    local containerName=$(echo "$solutionName" | tr '[:upper:]' '[:lower:]')
    local acrUrl="registry.cn-zhangjiakou.aliyuncs.com"
    local acrName="kcloudy-netcore"  # ACR 名称
    local imageName="${acrName}/${containerName}"
    local newVersion="1.0.0.${versionNum}"
    local lastNum=$((versionNum - 1))
    local lastVersion="1.0.0.${lastNum}"

    # 设置环境变量
    export ASPNETCORE_ENVIRONMENT="$env"

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

    # 使用阿里云镜像加速器拉取镜像
    print_info "正在从阿里云镜像仓库（${acrUrl}）拉取镜像 ${imageName}:${newVersion}..."
    if ! docker pull "${acrUrl}/${imageName}:${newVersion}"; then
        print_warn "从阿里云拉取镜像失败，尝试从原始仓库拉取..."
        docker pull "${imageName}:${newVersion}" || {
            print_error "拉取镜像失败，请检查网络连接和镜像是否存在"
            exit 1
        }
    else
        # 如果从阿里云拉取成功，重新标记镜像
        docker tag "${acrUrl}/${imageName}:${newVersion}" "${imageName}:${newVersion}"
    fi

    # 检查端口是否被占用
    function is_port_available() {
        local port=$1
        if command -v netstat &> /dev/null; then
            if netstat -tuln | grep -q ":$port "; then
                return 1  # 端口被占用
            fi
        elif command -v ss &> /dev/null; then
            if ss -tuln | grep -q ":$port "; then
                return 1  # 端口被占用
            fi
        fi
        return 0  # 端口可用
    }

    # 检查原始端口是否可用
    if is_port_available "$httpPort"; then
        print_info "端口 $httpPort 可用"
    else
        print_info "端口 $httpPort 被占用，尝试查找可用端口..."
        local originalPort=$httpPort
        local maxAttempts=10
        local attempt=0
        
        while [ $attempt -lt $maxAttempts ]; do
            httpPort=$((httpPort + 1))
            if is_port_available "$httpPort"; then
                print_info "找到可用端口: $httpPort"
                break
            fi
            print_info "端口 $httpPort 不可用，继续尝试..."
            attempt=$((attempt + 1))
        done

        if [ $attempt -eq $maxAttempts ]; then
            print_error "错误: 在 $originalPort 到 $httpPort 范围内找不到可用端口"
            exit 1
        fi
    fi

    # 运行容器
    print_info "正在启动容器 ${containerName}..."
    print_info "映射端口: ${httpPort}:${httpPort}"
    
    dockerRunCmd=("run" "-d" "--restart=always")
    
    # 添加HTTP端口映射
    dockerRunCmd+=("-p" "${httpPort}:${httpPort}")
    
    # 如果需要HTTPS，添加HTTPS端口映射
    if [ "$httpsPort" -gt 0 ]; then
        print_info "映射HTTPS端口: ${httpsPort}:${httpsPort}"
        dockerRunCmd+=("-p" "${httpsPort}:${httpsPort}")
    fi

    # 指定日志目录
    dockerRunCmd+=("-v" "/share/logs/:/app/logs/")
    
    # 添加容器名称和镜像
    dockerRunCmd+=("--name" "${containerName}" "${imageName}:${newVersion}")

    print_info "运行容器: docker ${dockerRunCmd[*]}"
    docker "${dockerRunCmd[@]}"

	print_info "打印运行日志: docker logs ${containerName}"
    docker logs ${containerName}

    print_success "${containerName}部署成功！"
}

# === 主程序逻辑 ===
# 检查Docker是否运行
if ! docker info >/dev/null 2>&1; then
    print_error "Docker 未运行或当前用户没有权限访问Docker"
    exit 1
fi

# 执行部署
set -e  # 如果任何命令失败则退出
print_success "===== 开始部署 ====="
# deploy_dotnet_web "$1" "$2" "$3" "$4" "$5"

case "$CURRENT_HOST" in
    "k8s-master")
        # Basic
        deploy_dotnet_web "KC.Web.Resource" 1 9999 0 "Production"
        deploy_dotnet_web "KC.WebApi.Admin" 1 1004 0 "Production"
		
		# Web
        #deploy_dotnet_web "KC.Web.SSO" 1 1001 0 "Production"
        #deploy_dotnet_web "KC.Web.Admin" 1 1003 0 "Production"
        #deploy_dotnet_web "KC.Web.Blog" 1 1005 0 "Production"
        #deploy_dotnet_web "KC.Web.CodeGenerate" 1 1007 0 "Production"
        ;;
    "k8s-worker01")
	    # Api
        #deploy_dotnet_web "KC.WebApi.Config" 1 1102 0 "Production"
        #deploy_dotnet_web "KC.WebApi.Dict" 1 1104 0 "Production"
        #deploy_dotnet_web "KC.WebApi.App" 1 1106 0 "Production"
        #deploy_dotnet_web "KC.WebApi.Message" 1 1108 0 "Production"
		
        # Web
        #deploy_dotnet_web "KC.Web.Config" 1 1101 0 "Production"
        #deploy_dotnet_web "KC.Web.Dict" 1 1103 0 "Production"
        #deploy_dotnet_web "KC.Web.App" 1 1105 0 "Production"
        #deploy_dotnet_web "KC.Web.Message" 1 1107 0 "Production"
        ;;
    "k8s-worker02")
        # Api
        deploy_dotnet_web "KC.WebApi.Account" 1 2002 0 "Production"
        #deploy_dotnet_web "KC.WebApi.Doc" 1 2006 0 "Production"
        #deploy_dotnet_web "KC.WebApi.Customer" 1 3002 0 "Production"
        #deploy_dotnet_web "KC.WebApi.Portal" 1 4002 0 "Production"
        #deploy_dotnet_web "KC.WebApi.WorkFlow" 1 7002 0 "Production"
		
        # Web
        deploy_dotnet_web "KC.Web.Account" 1 2001 0 "Production"
        #deploy_dotnet_web "KC.Web.Doc" 1 2005 0 "Production"
        #deploy_dotnet_web "KC.Web.Portal" 1 2007 0 "Production"
        ;;
    *)
        print_error "当前主机不在支持列表中，仅支持：k8s-master, k8s-worker01, k8s-worker02"
        exit 1
        ;;
esac

# 清理未用的镜像
docker system prune -a -f

print_success "===== 部署完成 ====="