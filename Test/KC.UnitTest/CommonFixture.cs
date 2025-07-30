using System;
using System.IO;
using System.Net.Http;
using KC.Common;
using KC.Common.LogHelper;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Framework.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace KC.UnitTest
{
    /// <summary>
    /// 在单元测试类中，只执行一次
    /// </summary>
    public class CommonFixture : IDisposable
    {
        /// <summary>
        /// Local Storage（FTP）、SupplyChainFinance（供应链金融）
        /// </summary>
        public Tenant DbaTenant;
        /// <summary>
        /// Local Storage（Local Disk）、CommercialFactoring（商业保理）
        /// </summary>
        public Tenant TestTenant;
        /// <summary>
        /// Azure Storage、StoreCredit（店铺赊销）
        /// </summary>
        public Tenant BuyTenant;
        /// <summary>
        /// Azure Storage、StoreCredit（店铺赊销）
        /// </summary>
        public Tenant SaleTenant;

        public IServiceCollection Services;

        public ILoggerFactory LoggerFactory;

        protected ILogger _logger;
        protected IServiceProvider _serviceProvider;

        public CommonFixture()
        {
            // Do "global" setup here; Only called once.
            Initilize();

            _logger = LoggerFactory.CreateLogger(nameof(CommonFixture));
            _serviceProvider = Services.BuildServiceProvider();

            SetUpData();
        }

        private void Initilize()
        {
            try
            {
                //if (ConfigUtil.Config == null)
                //    ConfigUtil.Config = new AppSettingsConfigService();
                if (LogUtil.Logger == null)
                    LogUtil.Logger = new NlogLoggingService();
                //if (CacheUtil.Cache == null)
                //{
                //    CacheUtil.Cache = new InMemoryCache();
                //    Service.CacheUtil.Cache = new InMemoryCache();
                //    //CacheUtil.Cache = new RedisCacheService();
                //    //Service.CacheUtil.Cache = new RedisCacheService();
                //}

                var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (envName.IsNullOrEmpty() || envName.Equals("Development"))
                    envName = "Dev";
                var envFile = $"appsettings.{envName}.json";
                if (envName.Equals("production", System.StringComparison.OrdinalIgnoreCase))
                {
                    envFile = "appsettings.json";
                }
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(envFile, optional: true)
                    .AddEnvironmentVariables();

                var Configuration = builder.Build();
                Services = new ServiceCollection()
                    //.AddEntityFrameworkSqlServer()
                    .AddDistributedMemoryCache()
                    //.AddMemoryCache()
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.SetMinimumLevel(LogLevel.Trace);

                        loggingBuilder
                           .AddConfiguration(Configuration.GetSection("Logging"))
                           .AddFilter("Microsoft", LogLevel.Debug)
                           .AddFilter("System", LogLevel.Debug)
                           .AddFilter("KC.UnitTest.TestFixture", LogLevel.Debug)
                           .AddConsole()
                           .AddDebug()
                           .AddNLog(Configuration);
                    });
                Services.AddHttpClient();

                LoggerFactory = Services.BuildServiceProvider().GetService<ILoggerFactory>();
                LogUtil.LogDebug("------Initialze UnitTest the Environment: " + envFile);

                #region 配置文件的读取及静态类GlobalConfig的设置
                Framework.Base.GlobalConfig.InitGlobalConfig(Configuration);

                using (var scope = Services.BuildServiceProvider().CreateScope())
                {
                    var clientFactory = scope.ServiceProvider.GetService<System.Net.Http.IHttpClientFactory>();
                    var url = GlobalConfig.ResWebDomain + "api/GlobalConfigApi/GetData";
                    var client = clientFactory.CreateClient();
                    client.BaseAddress = new Uri(GlobalConfig.ResWebDomain);
                    // for fiddler proxy setting
                    //var httpClientHandler = new HttpClientHandler
                    //{
                    //    // Does not work 
                    //    Proxy = new WebProxy("http://Kent-pc:8888", false),
                    //    UseProxy = true
                    //};

                    var response = client.GetAsync(url).Result;
                    if (response == null)
                        return;
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = response.Content.ReadAsStringAsync().Result;
                        if (string.IsNullOrEmpty(responseString))
                            return;
                        var globalConfigData = SerializeHelper.FromJson<GlobalConfigData>(responseString);
                        if (globalConfigData == null)
                            return;
                        GlobalConfig.InitGlobalConfigWithApiData(Configuration, globalConfigData);
                    } 
                    else
                    {
                        LogUtil.LogError(response.RequestMessage.ToString());
                    }
                  }
                #endregion
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ex.Message, ex.StackTrace);
            }

            DbaTenant = TenantConstant.DbaTenantApiAccessInfo;
            TestTenant = TenantConstant.TestTenantApiAccessInfo;
            BuyTenant = TenantConstant.BuyTenantApiAccessInfo;
            SaleTenant = TenantConstant.SaleTenantApiAccessInfo;
        }

        protected void InjectTenant(Tenant tenant)
        {
            Services.AddTransient(serviceProvider =>
            {
                switch (tenant.TenantName)
                {
                    case TenantConstant.DbaTenantName:
                        return DbaTenant;
                    case TenantConstant.SaleTenantName:
                        return SaleTenant;
                    case TenantConstant.BuyTenantName:
                        return BuyTenant;
                    default:
                        return TestTenant;
                }
            });
        }


        public virtual void SetUpData() { }

        public virtual void TearDownData() { }

        public void Dispose()
        {
            // Do "global" teardown here; Only called once.
            TearDownData();
        }
    }
}
