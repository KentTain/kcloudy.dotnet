#----------1. shell脚本的变量读取
#要构建的解决方案名称
solutionName="TestWeb";
#.csproj文件全路径
csprojDir="TestWeb/TestWeb.csproj";
#镜像前缀
imagesPrx="kcloudy";
#容器名
containerName="testweb";
#制定run的端口
port=5000;
#项目发布的版本
newVersion=1.0.$BUILD_NUMBER;
#上一个版本
lastNum=$(($BUILD_NUMBER-1));
lastVersion=1.0.${lastNum};

echo "----kcloudy: upgrade lastVersion: "${lastVersion}" to newVersion: "${newVersion}

#----------2. 定义好存放发布好的项目代码的目录和备份发布内容的目录
#项目发布的目录
webDir=/usr/sftpdata/web/${solutionName}/v-${newVersion};
oldwebDir=/usr/sftpdata/web/${solutionName}/v-${lastVersion};
#归档目录
archivesDir=/usr/sftpdata/web/archives;

#----------3. publish发布项目到准备好的目录
echo "----kcloudy:发布dotnetcore项目--"${solutionName};
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
	echo "----kcloudy:删除名称及版本重复的镜像--"${imageName}:${newVersion};
	docker rmi -f ${imageName}:${newVersion};
fi
#通过Dockerfile重新构建镜像
echo "----kcloudy:通过Dockerfile重新构建镜像--"${imageName}:${newVersion};
docker build -t ${imageName}:${newVersion} .;
docker images;

if [ -n "$(docker ps -aq -f name=${containerName})" ]; then
	#停止docker容器
	echo "----kcloudy:停止docker容器--"${containerName};
	docker stop ${containerName};
    if [ -n "$(docker ps -aq -f status=exited -f name=${containerName})" ]; then
        #删除容器
        echo "----kcloudy:删除容器--"${containerName};
		docker rm -f ${containerName};
    fi
fi
echo "----kcloudy:运行容器["${containerName}"]并绑定到端口["${port}:${port}"]--容器自启，并使用后台运行方式";
#docker 运行容器并绑定到端口
docker run --name ${containerName} --restart always -d -p ${port}:${port} ${imageName}:${newVersion};
docker logs ${containerName}

#----------7. 清理上个版本相关的项目发布目录及镜像
echo "----kcloudy:清空发布的老版本文件夹--"${oldwebDir};
sudo rm -rf ${oldwebDir};

if [ -n "$(docker images -aq ${imageName}:${lastVersion})" ]; then
	#删除镜像
	echo "----kcloudy:删除上一个版本的镜像--"${imageName}:${lastVersion};
	docker rmi -f ${imageName}:${lastVersion};
fi

echo "----kcloudy:success!";
