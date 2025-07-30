# 添加基础镜像
FROM mcr.microsoft.com/dotnet/aspnet:5.0

#netcore使用System.Drawing.Common绘图、解决中文乱码
RUN sed -i "s@http://deb.debian.org@http://mirrors.aliyun.com@g" /etc/apt/sources.list 
RUN apt-get update -y && apt-get install -y libgdiplus locales fontconfig && apt-get clean && ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
RUN sed -ie 's/# zh_CN.UTF-8 UTF-8/zh_CN.UTF-8 UTF-8/g' /etc/locale.gen && locale-gen && mkdir /usr/share/fonts/truetype/deng/
ADD ./Fonts/* /usr/share/fonts/truetype/deng/
RUN fc-cache -vf && fc-list
ENV LANG zh_CN.UTF-8

#容器中系统的工作空间
WORKDIR /app
ARG env=Production
ARG port

#设置Docker容器对外暴露的端口
EXPOSE ${port}

ENV ASPNETCORE_ENVIRONMENT=${env} \
	env="--environment="${env} 

#COPY build.sh /usr/local/bin/
#RUN chmod +x /usr/local/bin/build.sh

#拷贝当前文件夹下的文件到容器中系统的工作空间
COPY . /app

#ENTRYPOINT ["dotnet", "KC.Job.Basic.dll", "--environment=Production"]
ENTRYPOINT ["dotnet", "KC.Job.Basic.dll"]
CMD ["$env"]
