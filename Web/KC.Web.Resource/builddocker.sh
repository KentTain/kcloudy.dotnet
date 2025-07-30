#sh build.sh env path project port versionNum;
echo "----kcloudy: start build!";
#1. shell脚本的变量读取
#发布环境
env="Production"
#要构建的解决方案名称
solutionName="KC.Web.Resource"
#制定run的端口
port=9999
#版本号
versionNum=1

#容器名
containerName=$(echo "$solutionName" | tr '[:upper:]' '[:lower:]');
#镜像名
imageName="kcloudy/"$containerName

#项目发布的版本
newVersion=1.0.0.${versionNum}
#上一个版本
lastNum=$((${versionNum}-1))
lastVersion=1.0.0.${lastNum}

echo "----kcloudy: update project: "${solutionName}" from lastVersion: "${lastVersion}" to newVersion: "${newVersion}
#1. 定义好存放发布好的项目代码的目录和备份发布内容的目录
#项目发布的目录
webDir=/usr/sftpdata/web/${solutionName}
oldwebDir=/usr/sftpdata/web/${solutionName}

#5. Docker容器命令详解
export ASPNETCORE_ENVIRONMENT='${env}'
cd ${webDir} 

if [ -n "$(docker ps -aq -f name=${containerName})" ]; then
    #停止docker容器
    echo "----kcloudy: 停止docker容器--"${containerName};
    docker stop ${containerName};
    if [ -n "$(docker ps -aq -f status=exited -f name=${containerName})" ]; then
        #删除容器
        echo "----kcloudy: 删除容器--"${containerName};
        docker rm -f ${containerName};
    fi
fi
    
if [ -n "$(docker images -aq ${imageName}:${lastVersion})" ]; then
    #删除镜像
    echo "----kcloudy: 删除镜像--"${imageName}:${lastVersion};
    docker rmi -f ${imageName}:${lastVersion};
fi

#通过Dockerfile重新构建镜像
echo "----kcloudy: 通过Dockerfile重新构建镜像--"${imageName}:${newVersion}"--build-arg="${env};
docker build -t ${imageName}:${newVersion} --build-arg env=${env} .;
docker images;
    
echo "----kcloudy: 运行容器并绑定到端口";
#docker run容器并绑定到端口
docker run -d -p ${port}:${port} --name ${containerName} ${imageName}:${newVersion};
docker logs ${containerName}

echo "----kcloudy: success!";
