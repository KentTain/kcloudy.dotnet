#----------1. shell脚本的变量读取
#要构建的解决方案名称
solutionName=$1;
#.csproj文件全路径
csprojDir=$2;
#镜像前缀
imagesPrx=$3;
#容器名
containerName=$4;
#制定run的端口
port=$5;
#项目发布的版本
newVersion=$6;
#上一个版本
lastVersion=$7;

echo "----kcloudy: upgrade lastVersion: "${lastVersion}" to newVersion: "${newVersion}

#----------2. 定义好存放发布好的项目代码的目录和备份发布内容的目录
#项目发布的目录
webDir=/usr/sftpdata/web/${solutionName}/v-${newVersion};
oldwebDir=/usr/sftpdata/web/${solutionName}/v-${lastVersion};
#归档目录
archivesDir=/usr/sftpdata/web/archives;

#----------3. publish发布项目到准备好的目录
echo "----kcloudy: publish dotnetcore project--"${solutionName};
#清空文件夹，使用sudo命令需要将jenkins用户赋予NOPASSWD权限（visudo -f /etc/sudoers）
sudo rm -rf ${webDir}/*;
#发布网站到webDir
dotnet publish $JENKINS_HOME/workspace/$JOB_NAME/${csprojDir} -c Release -o ${webDir} /p:Version=${newVersion};

#----------4. 复制需要的配置到发布目录
#复制配置文件
#cp -rf /usr/sftpdata/web/${solutionName}/configs/* ${webDir}/;

cd ${webDir} 

#----------5. 将发布后的项目进行打包归档
#发布后的文件进行归档，使用脚本前利用下面命令，创建归档文件夹及将该文件的读写权限授予jenkins用户
#sudo mkdir -p ${archivesDir}
#sudo setfacl -Rm u:jenkins:rwx ${archivesDir}
sudo tar -zcvf ${archivesDir}/${solutionName}-${newVersion}.tar.gz .

#----------6. 使用Docker容器命令创建镜像、启动容器
#使用Docker命令前，使用下列命令，将Docker用户组授予jenkins用户
#sudo usermod -a -G docker jenkins

#镜像名称
imageName=${imagesPrx}/${containerName}; 
if [ -n "$(docker images -aq ${imageName}:${newVersion})" ]; then
	#删除镜像
	echo "----kcloudy: delete the repeated image--"${imageName}:${newVersion};
	docker rmi -f ${imageName}:${newVersion};
fi
#通过Dockerfile重新构建镜像
echo "----kcloudy: build docker image with Dockerfile--"${imageName}:${newVersion};
docker build -t ${imageName}:${newVersion} .;
docker images;

if [ -n "$(docker ps -aq -f name=${containerName})" ]; then
	#停止docker容器
	echo "----kcloudy: stop docker container--"${containerName};
	docker stop ${containerName};
    if [ -n "$(docker ps -aq -f status=exited -f name=${containerName})" ]; then
        #删除容器
        echo "----kcloudy: delete container--"${containerName};
		docker rm -f ${containerName};
    fi
fi
echo "----kcloudy: run container["${containerName}"] with port ["${port}:${port}"]--restart always in backend";
#docker 运行容器并绑定到端口
docker run --name ${containerName} --restart always -d -p ${port}:${port} ${imageName}:${newVersion};
docker logs ${containerName}

#----------7. 清理上个版本相关的项目发布目录及镜像
echo "----kcloudy: clear the old publish web dir--"${oldwebDir};
sudo rm -rf ${oldwebDir};

if [ -n "$(docker images -aq ${imageName}:${lastVersion})" ]; then
	#删除镜像
	echo "----kcloudy: delete the last version docker image--"${imageName}:${lastVersion};
	docker rmi -f ${imageName}:${lastVersion};
fi

echo "----kcloudy: success!";
