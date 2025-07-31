# 使用官方.NET 5.0运行时镜像
FROM mcr.microsoft.com/dotnet/aspnet:5.0-bullseye-slim

# 设置默认端口、环境变量
ARG port=80
ARG env=Production

# 设置时区、语言环境和编码
ENV LANG=zh_CN.UTF-8 \
    LC_ALL=zh_CN.UTF-8 \
    TZ=Asia/Shanghai \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_ENVIRONMENT=${env} \
    PORT=${port}

# 配置APT源为阿里云镜像并安装必要的依赖
RUN set -eux; \
    # 替换源地址
    sed -i 's/deb.debian.org/mirrors.aliyun.com/g' /etc/apt/sources.list && \
    sed -i 's/security.debian.org\/debian-security/mirrors.aliyun.com\/debian-security/g' /etc/apt/sources.list && \
    sed -i 's/updates.debian.org\/debian/mirrors.aliyun.com\/debian/g' /etc/apt/sources.list && \
    # 更新包索引并安装依赖
    apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
        libgdiplus \
        locales \
        fonts-wqy-zenhei \
        fontconfig && \
    # 创建 gdiplus.dll 软链接（.NET 需要）
    ln -sf /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll && \
    # 生成中文语言环境
    echo 'zh_CN.UTF-8 UTF-8' > /etc/locale.gen && \
    echo 'en_US.UTF-8 UTF-8' >> /etc/locale.gen && \
    locale-gen && \
    update-locale LANG=zh_CN.UTF-8 && \
    # 清理缓存
    rm -rf /var/lib/apt/lists/*

# 创建字体目录并复制字体
RUN mkdir -p /usr/share/fonts/truetype/deng/
COPY --chown=www-data:www-data ./Fonts/* /usr/share/fonts/truetype/deng/
RUN fc-cache -fv

# 设置工作目录
WORKDIR /app

# 复制发布文件到容器
COPY . .

# 暴露端口
EXPOSE ${PORT}

# 健康检查（可选启用）
# HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
#     CMD curl -f http://localhost/health || exit 1

# 设置容器入口点
ENTRYPOINT ["dotnet", "kc.webapi.portal.dll"]