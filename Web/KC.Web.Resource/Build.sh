#!/bin/sh -l

 #sh build.sh env path project port versionNum;

#1. shell脚本的变量读取
#发布环境
env="Production"
#要构建的解决方案路径
path=""
#要构建的解决方案名称
solutionName="KC.Web.Resource"
#.csproj文件全路径
csprojDir=$path""$solutionName".csproj"
#容器名
containerName=$(echo "$solutionName" | tr '[:upper:]' '[:lower:]');
#镜像名
imageName="kcloudy/"$containerName
#制定run的端口
port=9999
#版本号
versionNum=1

#本地Nuget服务
#localNuget="http://192.168.2.112:1000/nuget/"; #Nuget Server in Windows
#localNuget="https://www.myget.org/F/kcloudy-core/auth/3ed885f4-5ba8-45c4-afd4-f7233fbbe880/api/v3/index.json"
#localNuget1="https://www.myget.org/F/kcloudy-storage/auth/3ed885f4-5ba8-45c4-afd4-f7233fbbe880/api/v3/index.json"
localNuget="https://www.myget.org/F/kcloudy/api/v3/index.json"
nugetUrl="https://nuget.cnblogs.com/v3/index.json"

#项目发布的版本
newVersion=1.0.0.${versionNum}
#上一个版本
lastNum=$((${versionNum}-1))
lastVersion=1.0.0.${lastNum}
    
echo "----kcloudy: update project: "${csprojDir}" from lastVersion: "${lastVersion}" to newVersion: "${newVersion}
    
#2. 定义好存放发布好的项目代码的目录和备份发布内容的目录
#项目发布的目录
webDir=/usr/sftpdata/web/${solutionName}/v-${newVersion}
oldwebDir=/usr/sftpdata/web/${solutionName}/v-${lastVersion}
#归档目录
archivesDir=/usr/sftpdata/web/archives
    
#3. publish发布项目到准备好的目录
echo "----kcloudy: 发布dotnetcore项目: "${solutionName}"至目录: "${webDir}
#清空文件夹
sudo mkdir -p ${webDir}
sudo setfacl -Rm u:jenkins:rwx ${webDir}
sudo rm -rf ${webDir}/*

#还原Nuget包
dotnet restore ${csprojDir} -s ${localNuget} -s ${nugetUrl}
#发布网站到webDir
dotnet publish ${csprojDir} -c Release -o ${webDir} /p:Version=${newVersion}

#4. 复制需要的配置到发布目录
#复制配置文件
#cp -rf /usr/sftpdata/web/${solutionName}/configs/* ${webDir}/

#5. Docker容器命令详解
export ASPNETCORE_ENVIRONMENT='${env}'
cd ${webDir} 
    
#发布后的文件进行归档
#sudo mkdir -p ${archivesDir}
#sudo setfacl -Rm u:jenkins:rwx ${archivesDir}
sudo tar -zcvf ${archivesDir}/${solutionName}-${newVersion}.tar.gz .
    
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
    
#清空发布的老版本文件夹
echo "----kcloudy: 清空发布的老版本文件夹--"${oldwebDir};
sudo rm -rf ${oldwebDir};
    
echo "----kcloudy: success!";


