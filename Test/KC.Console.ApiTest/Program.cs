using KC.Common;
using KC.Common.LogHelper;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Service.Extension;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Account;
using KC.Service.Constants;
using KC.IdentityModel.Client;
using Microsoft.Extensions.Logging;
using System.IO;
using NLog.Extensions.Logging;
using AspectCore.Extensions.DependencyInjection;

namespace KC.Console.ApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            "\n初始化相关测试租户信息:".ConsoleGreen();
            Initialize();

            Test_AopCached_String();

            Test_Account_TestApi_Get();
            //Test_Account_AccountApi_GetUserMenusByRoleIds();

            //Test_Account_AccountApiService_GetUserMenusByRoleIds();

            //Test_Account_AccountApiService_SavePermissionsAsync();

            //Test_Admin_TestApi_Get();

            //Test_Admin_TenantApi_ExistTenantName();
            //Test_Admin_TenantApi_GetTenantUserByName();
            //Test_Admin_TenantApi_GetTenantByTenantNames();

            //Test_Admin_TenantApiService_GetTenantUserByName();
            //Test_Admin_TenantApiService_GetTenantByTenantNames();

            System.Console.WriteLine(TenantConstant.GetClientIdByTenant(DbaTenant));
            System.Console.WriteLine(TenantConstant.GetClientSecretByTenant(DbaTenant));

            System.Console.ReadLine();
        }

        #region 初始化
        /// <summary>
        /// Local Storage（FTP）、SupplyChainFinance（供应链金融）
        /// </summary>
        protected static Tenant DbaTenant;
        /// <summary>
        /// Local Storage（Local Disk）、CommercialFactoring（商业保理）
        /// </summary>
        protected static Tenant TestTenant;
        /// <summary>
        /// Azure Storage、StoreCredit（店铺赊销）
        /// </summary>
        protected static Tenant BuyTenant;
        /// <summary>
        /// Azure Storage、StoreCredit（店铺赊销）
        /// </summary>
        protected static Tenant SaleTenant;

        protected static IServiceCollection Services;

        protected static IServiceProvider _serviceProvider;

        protected static ITenantUserApiService TenantUserApiService => _serviceProvider.GetService<ITenantUserApiService>();
        protected static IAccountApiService AccountApiService => _serviceProvider.GetService<IAccountApiService>();
        protected static IMyCachedTest MyCachedTest => _serviceProvider.GetService<IMyCachedTest>();

        protected static void Initialize()
        {
            if (LogUtil.Logger == null)
                LogUtil.Logger = new NlogLoggingService();

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables();

            var Configuration = builder.Build();

            Services = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .AddDistributedMemoryCache()
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

            // 配置AspectCore动态代理
            Services.ConfigureDynamicProxy();

            #region 配置文件的读取及静态类GlobalConfig的设置
            Framework.Base.GlobalConfig.InitGlobalConfig(Configuration);
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var httpClient = scope.ServiceProvider.GetService<System.Net.Http.IHttpClientFactory>();
                var globalConfigData = new GlobalConfigApiService(httpClient).GetGlobalConfigData();
                GlobalConfig.InitGlobalConfigWithApiData(Configuration, globalConfigData);
            }
            #endregion

            DbaTenant = TenantConstant.DbaTenantApiAccessInfo;
            TestTenant = TenantConstant.TestTenantApiAccessInfo;
            BuyTenant = TenantConstant.BuyTenantApiAccessInfo;
            SaleTenant = TenantConstant.SaleTenantApiAccessInfo;

            Services.AddScoped(serviceProvider =>
            {
                return TestTenant;
            });

            #region AutoMapper对象注入
            var profiles = KC.Service.AutoMapper.AutoMapperConfiguration.GetAllProfiles()
                .Union(KC.Service.Account.AutoMapper.AutoMapperConfiguration.GetAllProfiles());

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(profiles);
            });
            Services.AddSingleton(config);
            var mapper = config.CreateMapper();
            Services.AddSingleton(mapper);

            #endregion

            Services.AddSingleton<IMyCachedTest, MyCachedTest>();

            KC.Service.Util.DependencyInjectUtil.InjectService(Services);

            _serviceProvider = Services.BuildDynamicProxyProvider();

            using (var scope = _serviceProvider.CreateScope())
            {
                var cache = scope.ServiceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                Service.CacheUtil.Cache = cache;
            }

        }

        protected static void InjectTenant(Tenant tenant)
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

        protected static void Cleanup()
        {
        }

        protected string ToAssertableString(IDictionary<string, List<int>> dictionary)
        {
            var pairStrings = dictionary.OrderBy(p => p.Key)
                                        .Select(p => p.Key + ": " + string.Join(", ", p.Value));
            return string.Join("; ", pairStrings);
        }

        protected string ToAssertableString(IDictionary<string, List<string>> dictionary)
        {
            var pairStrings = dictionary.OrderBy(p => p.Key)
                                        .Select(p => p.Key + ": " + string.Join(", ", p.Value));
            return string.Join("; ", pairStrings);
        }
        #endregion

        private const string _clientId = "testclient";
        private const string _clientSecret = "secret";
        private const string _contentType = "application/json";

        #region  AOP Cache Test
        static void Test_AopCached_String()
        {
            var testTenant = TenantConstant.TestTenantApiAccessInfo;
            var testTenantHash = testTenant.GetHashCode();

            var firstString = MyCachedTest.GetCachedString("firstString", testTenant);
            string.Format("---outer---GetCachedString: 【{0}】, 【{1}】", "firstString", testTenantHash).ConsoleGreen();
            var secondString = MyCachedTest.GetCachedString("secondString", testTenant); 
            string.Format("---outer---GetCachedString: 【{0}】, 【{1}】", "secondString", testTenantHash).ConsoleGreen();
            
            var thirdString = MyCachedTest.GetCachedString("firstString", testTenant);
            string.Format("---no inner---GetCachedString: 【{0}】, 【{1}】", "firstString", testTenantHash).ConsoleGreen();
            string.Format("Set except: {0}; Cached result: {1}", firstString, thirdString).ConsoleGreen();

            MyCachedTest.GetDeleteCachedString("secondString", testTenant);
            string.Format("---outer---GetDeleteCachedString: 【{0}】, 【{1}】", "secondString", testTenantHash).ConsoleGreen();
            var fourString = MyCachedTest.GetCachedString("secondString", testTenant); 
            string.Format("---outer---GetCachedString: 【{0}】, 【{1}】", "secondString", testTenantHash).ConsoleGreen();
            var fiveString = MyCachedTest.GetCachedString("secondString", testTenant);
            string.Format("---no inner---GetCachedString: 【{0}】, 【{1}】", "secondString", testTenantHash).ConsoleGreen(); 
            string.Format("Set except: {0}; result: {1}", fourString, fiveString).ConsoleGreen();

            var firstObj = MyCachedTest.GetCachedObject("firstString", testTenant);
            string.Format("---outer---GetCachedObject: 【{0}】, 【{1}】", "firstString", testTenantHash).ConsoleGreen();
            var secondObj = MyCachedTest.GetCachedObject("secondString", testTenant).Result;
            string.Format("---outer---GetCachedObject: 【{0}】, 【{1}】", "secondString", testTenantHash).ConsoleGreen();
            var thirdObj = MyCachedTest.GetCachedObject("secondString", testTenant).Result;
            string.Format("---no inner---GetCachedObject: 【{0}】, 【{1}】", secondObj.ApplicationId, testTenantHash).ConsoleGreen();
            string.Format("Set except: {0}; result: {1}", secondObj.ApplicationId, thirdObj.ApplicationId).ConsoleGreen();

            MyCachedTest.GetDeleteCachedString("firstString", testTenant);
            string.Format("---outer---GetDeleteCachedString: 【{0}】, 【{1}】", "firstString", testTenantHash).ConsoleGreen();
            var fourObj = MyCachedTest.GetCachedObject("firstString", testTenant);
            string.Format("---outer---GetCachedObject: 【{0}】, 【{1}】", "firstString", testTenantHash).ConsoleGreen();
        }
        #endregion

        #region KC.WebApi.Account
        private const string _accountScope = ApplicationConstant.AccScope;
        private const string _accountApiUri = "http://ctest.localhost:2002";
        #region TestApi
        static async Task Test_Account_TestApi_Get()
        {
            InjectTenant(TestTenant);
            try
            {
                System.Console.Title = "Console Client Credentials Flow";

                var clientId = TenantConstant.GetClientIdByTenant(TestTenant);
                var clientSecret = TenantConstant.GetClientSecretByTenant(TestTenant);

                var accessToken = await GetAccessTokenAsync(clientId, clientSecret, _accountScope);
                var client = new HttpClient();
                client.BaseAddress = new Uri(_accountApiUri);
                client.Timeout = TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut);
                client.SetBearerToken(accessToken);
                var result = await client.GetStringAsync("/api/TestApi");

                "\n\nService claims:".ConsoleGreen();
                System.Console.WriteLine(SerializeHelper.ToJson(result));
                LogUtil.LogDebug("\n\nService claims:" + result);
            }
            catch (Exception ex)
            {
                Framework.Util.LogUtil.LogException(ex);
            }
        }
        #endregion

        #region AccountApi
        #region AccountApi with Test Client
        static async Task Test_Account_AccountApi_LoadUserMenusByRoleIds()
        {
            try
            {
                InjectTenant(TestTenant);

                var clientId = TenantConstant.GetClientIdByTenant(TestTenant);
                var clientSecret = TenantConstant.GetClientSecretByTenant(TestTenant);

                var roleIds = new List<string>() {
                    RoleConstants.AdminRoleId
                };
                var jsonData = SerializeHelper.ToJson(roleIds);

                System.Console.Title = "Console Client Credentials Flow";

                var accessToken = await GetAccessTokenAsync(clientId, clientSecret, _accountScope);
                var client = new HttpClient();
                client.BaseAddress = new Uri(_accountApiUri);
                client.Timeout = TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut);
                client.SetBearerToken(accessToken);

                var apiUri = "/api/AccountApi/LoadUserMenusByRoleIds";
                var request = new HttpRequestMessage(HttpMethod.Post, apiUri);
                var content = new StringContent(jsonData, Encoding.UTF8, _contentType);
                var response = await client.PostAsync(apiUri, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorString = await response.Content.ReadAsStringAsync();
                    errorString.ConsoleGreen();
                    LogUtil.LogDebug(string.Format("apiUri【{0}】 errors: {1}", apiUri, errorString));
                    return;
                }
                //内容
                var responseString = await response.Content.ReadAsStringAsync();
                "\n\nService claims:".ConsoleGreen();
                System.Console.WriteLine(responseString);
                LogUtil.LogDebug(string.Format("apiUri【{0}】 result: {1}", apiUri, responseString));

            }
            catch (Exception ex)
            {
                Framework.Util.LogUtil.LogException(ex);
            }

        }
        #endregion

        #region AccountApiService
        static async Task Test_Account_AccountApiService_LoadUserMenusByRoleIds()
        {
            try
            {
                var roleIds = new List<string>()
                {
                    RoleConstants.AdminRoleId
                };
                var result = await AccountApiService.LoadUserMenusByRoleIds(roleIds);
            }
            catch (Exception ex)
            {
                Framework.Util.LogUtil.LogException(ex);
            }
        }

        //static async Task Test_Account_AccountApiService_SavePermissionsAsync()
        //{
        //    #region jsonData
        //    var jsonObj = @"[{'$id':'1','IsEditMode':false,'AreaName':'','ActionName':'Index','ControllerName':'CacheManager','Parameters':'','ResultType':0,'Description':'页面【缓存管理】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'4986A170-4B42-47D1-9596-4EE3EA53EC65','id':0,'text':'缓存管理-缓存管理','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':3,'children':[{'$id':'2','IsEditMode':false,'AreaName':'','ActionName':'RemoveDatabasePool','ControllerName':'CacheManager','Parameters':'','ResultType':1,'Description':'页面【缓存管理】下的权限【删除单个缓存】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'1'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'13AD5E0B-6588-4FEA-BCFD-57A4719791AA','id':0,'text':'缓存管理-删除单个缓存','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':2,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6584123Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6584123Z'},{'$id':'3','IsEditMode':false,'AreaName':'','ActionName':'RemoveDatabasePool','ControllerName':'CacheManager','Parameters':'','ResultType':1,'Description':'页面【缓存管理】下的权限【删除所有缓存】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'1'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'AD86A155-3777-4087-AB21-C8A0E22D383C','id':0,'text':'缓存管理-删除所有缓存','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6592936Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6592936Z'}],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6476055Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.647606Z'},{'$id':'4','IsEditMode':false,'AreaName':'','ActionName':'Index','ControllerName':'DatabasePool','Parameters':'','ResultType':0,'Description':'页面【数据库池管理】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'7EED6978-DA5D-4168-984E-B0070B20DA60','id':0,'text':'数据库池管理-数据库池管理','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':1,'children':[{'$id':'5','IsEditMode':false,'AreaName':'','ActionName':'SaveDatabasePoolForm','ControllerName':'DatabasePool','Parameters':'','ResultType':1,'Description':'页面【数据库池管理】下的权限【保存数据库连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'4'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'13914A9C-49AB-4001-BA3B-C1C1D66969CB','id':0,'text':'数据库池管理-保存数据库连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':2,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6600232Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6600232Z'},{'$id':'6','IsEditMode':false,'AreaName':'','ActionName':'RemoveDatabasePool','ControllerName':'DatabasePool','Parameters':'','ResultType':1,'Description':'页面【数据库池管理】下的权限【删除数据库连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'4'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'F6169808-FC59-49FD-99C5-9B98FF99F7E4','id':0,'text':'数据库池管理-删除数据库连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.660695Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6606955Z'},{'$id':'7','IsEditMode':false,'AreaName':'','ActionName':'TestDatabaseConnection','ControllerName':'DatabasePool','Parameters':'','ResultType':1,'Description':'页面【数据库池管理】下的权限【测试数据库连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'4'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'26F67515-370C-4E65-A71E-0E78B31B8E4E','id':0,'text':'数据库池管理-测试数据库连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':4,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.66144Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.66144Z'}],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6497263Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6497263Z'},{'$id':'8','IsEditMode':false,'AreaName':'','ActionName':'Index','ControllerName':'Home','Parameters':'','ResultType':0,'Description':'页面【后台管理首页】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'2722E7B6-9439-434C-858D-290156A25D49','id':0,'text':'后台管理首页-首页页面','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':1,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6504644Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6504644Z'},{'$id':'9','IsEditMode':false,'AreaName':'','ActionName':'Index','ControllerName':'NLog','Parameters':'','ResultType':0,'Description':'页面【系统日志管理】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'783B54AA-D623-46F5-9D8F-2D1D83127049','id':0,'text':'系统日志管理-系统日志管理','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':1,'children':[{'$id':'10','IsEditMode':false,'AreaName':'','ActionName':'DeleteNLog','ControllerName':'NLog','Parameters':'','ResultType':1,'Description':'页面【系统日志管理】下的权限【删除系统日志】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'9'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'1BF23319-EBEF-4E31-845E-8F1A1EC87529','id':0,'text':'系统日志管理-删除系统日志','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':2,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6623194Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6623199Z'}],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6511722Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6511722Z'},{'$id':'11','IsEditMode':false,'AreaName':'','ActionName':'Index','ControllerName':'NoSqlPool','Parameters':'','ResultType':0,'Description':'页面【NoSql池管理】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'5046814D-2DA6-4D3A-82AE-D7009A7B916E','id':0,'text':'NoSql池管理-NoSql池管理','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':1,'children':[{'$id':'12','IsEditMode':false,'AreaName':'','ActionName':'SaveNoSqlPoolForm','ControllerName':'NoSqlPool','Parameters':'','ResultType':1,'Description':'页面【NoSql池管理】下的权限【保存NoSql连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'11'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'927EEC2D-B61C-48BA-8D79-D6FC4CC1059D','id':0,'text':'NoSql池管理-保存NoSql连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':2,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6634383Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6634383Z'},{'$id':'13','IsEditMode':false,'AreaName':'','ActionName':'RemoveNoSqlPool','ControllerName':'NoSqlPool','Parameters':'','ResultType':1,'Description':'页面【NoSql池管理】下的权限【删除NoSql连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'11'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'5052BB38-F1A8-4940-8E4B-C62E9F61186F','id':0,'text':'NoSql池管理-删除NoSql连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6644955Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6644959Z'},{'$id':'14','IsEditMode':false,'AreaName':'','ActionName':'TestNoSqlConnection','ControllerName':'NoSqlPool','Parameters':'','ResultType':1,'Description':'页面【NoSql池管理】下的权限【测试NoSql连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'11'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'D1DF0FB3-FA24-496C-B59A-6D50C6B6D5A4','id':0,'text':'NoSql池管理-测试NoSql连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':4,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6653674Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6653674Z'}],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6522821Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6522821Z'},{'$id':'15','IsEditMode':false,'AreaName':'','ActionName':'Index','ControllerName':'QueueErrorMessage','Parameters':'','ResultType':0,'Description':'页面【队列日志管理】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'2C1ECB6D-C62C-480D-8EB2-FC2B8510DF59','id':0,'text':'队列日志管理-队列日志管理','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':1,'children':[{'$id':'16','IsEditMode':false,'AreaName':'','ActionName':'DeleteQueueErrorMessage','ControllerName':'QueueErrorMessage','Parameters':'','ResultType':1,'Description':'页面【队列日志管理】下的权限【删除队列日志】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'15'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'7CA9D1F5-236E-4890-8A68-0C2B504FF79F','id':0,'text':'队列日志管理-删除队列日志','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6663172Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6663172Z'},{'$id':'17','IsEditMode':false,'AreaName':'','ActionName':'ResetQueue','ControllerName':'QueueErrorMessage','Parameters':'','ResultType':1,'Description':'页面【队列日志管理】下的权限【重置队列】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'15'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'91A26722-C66D-4FCE-A631-327054070C93','id':0,'text':'队列日志管理-重置队列','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6673202Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6673202Z'}],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6531848Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6531848Z'},{'$id':'18','IsEditMode':false,'AreaName':'','ActionName':'Index','ControllerName':'QueuePool','Parameters':'','ResultType':0,'Description':'页面【队列池管理】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'71DF9D2D-A75F-47C9-B933-BC55BC731D7C','id':0,'text':'队列池管理-队列池管理','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':1,'children':[{'$id':'19','IsEditMode':false,'AreaName':'','ActionName':'SaveQueuePoolForm','ControllerName':'QueuePool','Parameters':'','ResultType':1,'Description':'页面【队列池管理】下的权限【保存队列连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'18'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'F1823EF3-2726-473E-97D4-2A4D2956E84D','id':0,'text':'队列池管理-保存队列连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':2,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6682771Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6682771Z'},{'$id':'20','IsEditMode':false,'AreaName':'','ActionName':'RemoveQueuePool','ControllerName':'QueuePool','Parameters':'','ResultType':1,'Description':'页面【队列池管理】下的权限【删除队列连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'18'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'36497733-7F9B-43D4-A7A6-A31ABC11CB4D','id':0,'text':'队列池管理-删除队列连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6694015Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6694015Z'},{'$id':'21','IsEditMode':false,'AreaName':'','ActionName':'TestQueueConnection','ControllerName':'QueuePool','Parameters':'','ResultType':1,'Description':'页面【队列池管理】下的权限【测试队列连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'18'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'04A2A9A2-5236-47FA-998F-3187B3BAF49C','id':0,'text':'队列池管理-测试队列连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':4,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6704609Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6704614Z'}],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6539163Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6539163Z'},{'$id':'22','IsEditMode':false,'AreaName':'','ActionName':'Index','ControllerName':'ServiceBusPool','Parameters':'','ResultType':0,'Description':'页面【服务总线池管理】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'9DA15AD8-6725-489B-82D2-82413ADC7B96','id':0,'text':'服务总线池管理-服务总线池管理','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':1,'children':[{'$id':'23','IsEditMode':false,'AreaName':'','ActionName':'SaveServiceBusPoolForm','ControllerName':'ServiceBusPool','Parameters':'','ResultType':1,'Description':'页面【服务总线池管理】下的权限【保存服务总线连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'22'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'0AB446BD-F380-43EB-B920-6CAE1A8A6AC0','id':0,'text':'服务总线池管理-保存服务总线连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':2,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6714631Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6714631Z'},{'$id':'24','IsEditMode':false,'AreaName':'','ActionName':'DeleteServiceBusPool','ControllerName':'ServiceBusPool','Parameters':'','ResultType':1,'Description':'页面【服务总线池管理】下的权限【删除服务总线连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'22'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'A900D687-435B-4748-BF9F-A4D809CAB3E9','id':0,'text':'服务总线池管理-删除服务总线连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6724679Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6724679Z'}],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6546235Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6546235Z'},{'$id':'25','IsEditMode':false,'AreaName':'','ActionName':'Index','ControllerName':'StoragePool','Parameters':'','ResultType':0,'Description':'页面【存储池管理】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'FA07B9D9-014E-477E-B5AA-3B46C92B2438','id':0,'text':'存储池管理-存储池管理','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':1,'children':[{'$id':'26','IsEditMode':false,'AreaName':'','ActionName':'SaveStoragePoolForm','ControllerName':'StoragePool','Parameters':'','ResultType':1,'Description':'页面【存储池管理】下的权限【保存存储连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'25'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'F258F8A9-B2A1-4779-AA39-D0079541E5DA','id':0,'text':'存储池管理-保存存储连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':2,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6737677Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6737677Z'},{'$id':'27','IsEditMode':false,'AreaName':'','ActionName':'RemoveStoragePool','ControllerName':'StoragePool','Parameters':'','ResultType':1,'Description':'页面【存储池管理】下的权限【删除存储连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'25'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'C10FB77B-DEEC-4609-91C2-A8B8079A1135','id':0,'text':'存储池管理-删除存储连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6749467Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6749467Z'},{'$id':'28','IsEditMode':false,'AreaName':'','ActionName':'TestStorageConnection','ControllerName':'StoragePool','Parameters':'','ResultType':1,'Description':'页面【存储池管理】下的权限【测试存储连接】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'25'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'961622D0-4F5C-4A9A-8E75-936860C22FCB','id':0,'text':'存储池管理-测试存储连接','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':4,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6757439Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6757439Z'}],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6552389Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6552389Z'},{'$id':'29','IsEditMode':false,'AreaName':'','ActionName':'Index','ControllerName':'Tenant','Parameters':'','ResultType':0,'Description':'页面【租户列表】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'2A58BE7C-CE95-451F-9501-5F277C34B561','id':0,'text':'租户列表-租户列表','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':2,'children':[{'$id':'30','IsEditMode':false,'AreaName':'','ActionName':'SaveTenantUserForm','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【保存租户信息】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'BDA3F21F-16D9-4DC6-99F8-0F7A57DC4980','id':0,'text':'租户列表-保存租户信息','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':2,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6764372Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6764372Z'},{'$id':'31','IsEditMode':false,'AreaName':'','ActionName':'RemoveTenantUser','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【删除租户列表数据】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'D3D82D15-B144-4A28-8DE6-C06CFA870ED0','id':0,'text':'租户列表-删除租户列表数据','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6769957Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6769957Z'},{'$id':'32','IsEditMode':false,'AreaName':'','ActionName':'SavaTenantUserApplications','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【添加租户应用】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'57165E12-3DE5-4B88-B16A-ED5392E4AA57','id':0,'text':'租户列表-添加租户应用','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':4,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6775336Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.677534Z'},{'$id':'33','IsEditMode':false,'AreaName':'','ActionName':'SavaTenantUserApplications','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【开通租户应用】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'381C3080-BD0B-47C7-B3AE-CADD6B17A5BE','id':0,'text':'租户列表-开通租户应用','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':5,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6780901Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6780901Z'},{'$id':'34','IsEditMode':false,'AreaName':'','ActionName':'OpenSmsService','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【开通短信服务】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'B0F53C31-8974-48EF-B161-33B6BC01D7A9','id':0,'text':'租户列表-开通短信服务','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':6,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6786332Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6786336Z'},{'$id':'35','IsEditMode':false,'AreaName':'','ActionName':'OpenEmailService','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【开通邮件服务】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'5CC95124-4311-49D3-A286-D82D401E8474','id':0,'text':'租户列表-开通邮件服务','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':7,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6791767Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6791767Z'},{'$id':'36','IsEditMode':false,'AreaName':'','ActionName':'OpenCallCenterService','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【开通呼叫中心服务】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'CAD430DF-8C50-4222-8174-758C2141121E','id':0,'text':'租户列表-开通呼叫中心服务','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':8,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6797272Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6797272Z'},{'$id':'37','IsEditMode':false,'AreaName':'','ActionName':'UpgradeTenantDatabase','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【更新租户数据库】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'EB81D087-51C9-41C3-AE87-1B4ED196FC75','id':0,'text':'租户列表-更新租户数据库','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':9,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6802693Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6802698Z'},{'$id':'38','IsEditMode':false,'AreaName':'','ActionName':'RollBackAllTenantDatabase','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【回滚所有租户数据库】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'79C48640-AA83-4D35-AADD-FD284619E355','id':0,'text':'租户列表-回滚所有租户数据库','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':10,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6810844Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6810844Z'},{'$id':'39','IsEditMode':false,'AreaName':'','ActionName':'RollBackDatabaseService','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【回滚单个租户数据库】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'B9A63EA0-BEEF-4437-B2D9-BEC51896C4AF','id':0,'text':'租户列表-回滚单个租户数据库','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':11,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6817832Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6817832Z'},{'$id':'40','IsEditMode':false,'AreaName':'','ActionName':'SendOpenSms','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【发送开通短信】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'6FCA4967-CE65-4B54-994A-D90F7C25C633','id':0,'text':'租户列表-发送开通短信','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':12,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6824461Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6824461Z'},{'$id':'41','IsEditMode':false,'AreaName':'','ActionName':'SendOpenEmail','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【发送开通邮件】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'8436972E-B2B6-45F3-A4CA-29251B46201C','id':0,'text':'租户列表-发送开通邮件','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':13,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6830848Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6830848Z'},{'$id':'42','IsEditMode':false,'AreaName':'','ActionName':'SendAllOpenEmail','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户列表】下的权限【发送多个租户的开通邮件】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':{'$ref':'29'},'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'B6C6C595-CB8C-443D-B07C-53B2861A553C','id':0,'text':'租户列表-发送多个租户的开通邮件','ParentId':null,'TreeCode':null,'Leaf':true,'Level':2,'Index':14,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6837454Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6837454Z'}],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6559299Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6559299Z'},{'$id':'43','IsEditMode':false,'AreaName':'','ActionName':'TenantDetail','ControllerName':'Tenant','Parameters':'','ResultType':0,'Description':'页面【租户列表】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'DF22607F-3B56-42C8-BAD4-8CF275438F48','id':0,'text':'租户列表-租户详情','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6566791Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6566791Z'},{'$id':'44','IsEditMode':false,'AreaName':'','ActionName':'TenantErrorLog','ControllerName':'Tenant','Parameters':'','ResultType':0,'Description':'页面【租户错误日志】的权限','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'14EC24AD-E7C7-4D11-98E3-C770B96D22C2','id':0,'text':'租户错误日志-租户错误日志','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':4,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6572744Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6572744Z'},{'$id':'45','IsEditMode':false,'AreaName':'','ActionName':'ModifyTenantUserContactPhone','ControllerName':'Tenant','Parameters':'','ResultType':1,'Description':'页面【租户详情】下的权限【修改租户邮箱或手机】','ApplicationId':'98e6825f-7702-4a83-b194-a25442a25d7a','ApplicationName':'后台管理','Parent':null,'DefaultRoleId':'B779D882-68ED-4298-B233-AA68065C447B','AuthorityId':'43377989-D92F-4166-BF5E-0894BB3D6315','id':0,'text':'租户详情-修改租户邮箱或手机','ParentId':null,'TreeCode':null,'Leaf':false,'Level':2,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-07-12T08:08:59.6844028Z','ModifiedBy':null,'ModifiedDate':'2019-07-12T08:08:59.6844028Z'}] ";
        //    #endregion 
        //    try
        //    {
        //        var permissions = Common.SerializeHelper.FromJson<List<PermissionDTO>>(jsonObj);
        //        var result = await AccountApiService.SavePermissionsAsync(permissions, new Guid("98e6825f-7702-4a83-b194-a25442a25d7a"));
        //    }
        //    catch (Exception ex)
        //    {
        //        Framework.Util.LogUtil.LogException(ex);
        //    }
        //}
        #endregion
        #endregion
        #endregion

        #region KC.WebApi.Admin

        private const string _adminScope = "adminapi";
        private const string _adminApiUri = "http://localhost:1009/";

        #region TestApi
        static async Task Test_Admin_TestApi_Get()
        {
            try
            {
                System.Console.Title = "Console Client Credentials Flow";

                var accessToken = await GetAccessTokenAsync(_clientId, _clientSecret, _adminScope);
                var client = new HttpClient();
                client.BaseAddress = new Uri(_adminApiUri);
                client.Timeout = TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut);
                client.SetBearerToken(accessToken);

                var result = await client.GetStringAsync("/api/TestApi");

                "\n\nService claims:".ConsoleGreen();
                System.Console.WriteLine(result);
                LogUtil.LogDebug("\n\nService claims:" + result);
            }
            catch (Exception ex)
            {
                Framework.Util.LogUtil.LogException(ex);
            }
        }
        #endregion

        #region TenantApi

        #region TenantApi with Test Client
        static async Task Test_Admin_TenantApi_ExistTenantName()
        {
            try
            {
                System.Console.Title = "Console Client Credentials Flow";

                var accessToken = await GetAccessTokenAsync(_clientId, _clientSecret, _adminScope);
                var client = new HttpClient();
                client.BaseAddress = new Uri(_adminApiUri);
                client.Timeout = TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut);
                client.SetBearerToken(accessToken);
                var result = await client.GetStringAsync("/api/TenantApi/ExistTenantName?tenantName=" + TenantConstant.DbaTenantName);

                "\n\nService claims:".ConsoleGreen();
                System.Console.WriteLine(result);
                LogUtil.LogDebug("\n\nService claims:" + result);
            }
            catch (Exception ex)
            {
                Framework.Util.LogUtil.LogException(ex);
            }

        }

        static async Task Test_Admin_TenantApi_GetTenantUserByName()
        {
            try
            {
                System.Console.Title = "Console Client Credentials Flow";
                var accessToken = await GetAccessTokenAsync(_clientId, _clientSecret, _adminScope);
                var client = new HttpClient();
                client.BaseAddress = new Uri(_adminApiUri);
                client.Timeout = TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut);
                client.SetBearerToken(accessToken);
                var result = await client.GetStringAsync("/api/TenantApi/GetTenantUserByName?tenantName=" + TenantConstant.DbaTenantName);

                "\n\nService claims:".ConsoleGreen();
                System.Console.WriteLine(result);
                LogUtil.LogDebug("\n\nService claims:" + result);
            }
            catch (Exception ex)
            {
                Framework.Util.LogUtil.LogException(ex);
            }

        }

        static async Task Test_Admin_TenantApi_GetTenantByTenantNames()
        {
            try
            {
                var testTenant = Framework.Tenant.TenantConstant.DbaTenantApiAccessInfo;
                var clientId = KC.Framework.Tenant.TenantConstant.GetClientIdByTenant(testTenant);
                var clientSecrets = KC.Framework.Tenant.TenantConstant.GetClientSecretByTenant(testTenant);

                var tenantNames = new List<string>() {
                    TenantConstant.DbaTenantName,
                    TenantConstant.TestTenantName
                };
                var jsonData = SerializeHelper.ToJson(tenantNames);

                System.Console.Title = "Console Client Credentials Flow";

                var accessToken = await GetAccessTokenAsync(_clientId, _clientSecret, _adminScope);
                var client = new HttpClient();
                client.BaseAddress = new Uri(_adminApiUri);
                client.Timeout = TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut);
                client.SetBearerToken(accessToken);

                var apiUri = "/api/TenantApi/GetTenantByTenantNames";
                var request = new HttpRequestMessage(HttpMethod.Post, apiUri);
                var content = new StringContent(jsonData, Encoding.UTF8, _contentType);
                var response = await client.PostAsync(apiUri, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorString = await response.Content.ReadAsStringAsync();
                    errorString.ConsoleGreen();
                    LogUtil.LogDebug(string.Format("apiUri【{0}】 errors: {1}", apiUri, errorString));
                    return;
                }
                //内容
                var responseString = await response.Content.ReadAsStringAsync();
                "\n\nService claims:".ConsoleGreen();
                System.Console.WriteLine(responseString);
                LogUtil.LogDebug(string.Format("apiUri【{0}】 result: {1}", apiUri, responseString));
            }
            catch (Exception ex)
            {
                Framework.Util.LogUtil.LogException(ex);
            }

        }
        #endregion

        #region TenantApiService
        static async Task Test_Admin_TenantApiService_GetTenantUserByName()
        {
            try
            {
                var result = await TenantUserApiService.GetTenantByName(TenantConstant.TestTenantName);
                "\n\nGetTenantUserByName:".ConsoleGreen();
                LogUtil.LogDebug("\n\nGetTenantByName:" + result.TenantName);
            }
            catch (Exception ex)
            {
                Framework.Util.LogUtil.LogException(ex);
            }
        }

        #endregion
        #endregion

        #endregion

        #region HttpClient
        /// <summary>
        /// 获取accesstoken
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="baseAddress"></param>
        /// <param name="clientScope"></param>
        /// <returns></returns>
        static async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string clientScope)
        {
            var client = new HttpClient();

            var tenantName = TenantConstant.GetDecodeClientId(clientId);
            var tokenEndpointCacheKey = CacheKeyConstants.Prefix.TenantAuthTokenEndpoint + clientId;
            var tokenEndpoint = Service.CacheUtil.GetCache<string>(tokenEndpointCacheKey);
            if (string.IsNullOrEmpty(tokenEndpoint))
            {
                var tenantSsoWeb = GlobalConfig.SSOWebDomain;
                //var tenantSsoWeb = GetTenantTokenEndpoint(GlobalConfig.SSOWebDomain, tenantName);
                var docRequest = new DiscoveryDocumentRequest
                {
                    Address = tenantSsoWeb
                };
                if (tenantSsoWeb.StartsWith("https"))
                {
                    docRequest.Policy.RequireHttps = true;
                    docRequest.Policy.ValidateIssuerName = false;
                    docRequest.Policy.ValidateEndpoints = false;
                    var disco = await client.GetDiscoveryDocumentAsync(docRequest);
                    if (disco.IsError) throw new Exception(disco.Error);
                    tokenEndpoint = disco.TokenEndpoint;
                }
                else
                {
                    docRequest.Policy.RequireHttps = false;
                    docRequest.Policy.ValidateIssuerName = false;
                    docRequest.Policy.ValidateEndpoints = false;
                    var disco = await client.GetDiscoveryDocumentAsync(docRequest);
                    if (disco.IsError) throw new Exception(disco.Error);
                    tokenEndpoint = disco.TokenEndpoint;
                }

                Service.CacheUtil.SetCache(tokenEndpointCacheKey, tokenEndpoint, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                LogUtil.LogDebug("----KC.Service.Extension.ClientExtensions tokenEndpoint: " + tokenEndpoint);
            }

            var accessTokenCacheKey = CacheKeyConstants.Prefix.TenantAccessToken + clientId;
            var accessToken = Service.CacheUtil.GetCache<string>(accessTokenCacheKey);
            if (string.IsNullOrEmpty(accessToken))
            {
                var response = await client.RequestClientCredentialsTokenAsync(
                    new ClientCredentialsTokenRequest
                    {
                        Address = tokenEndpoint,
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Scope = clientScope,
                        Parameters = new Dictionary<string, string>() { { TenantConstant.ClaimTypes_TenantName, tenantName } }
                    });
                if (response.IsError) throw new Exception(response.Error);

                accessToken = response.AccessToken;
                var expiredTimeSpan = response.ExpiresIn - 100;
                Service.CacheUtil.SetCache(accessTokenCacheKey, accessToken, TimeSpan.FromSeconds(expiredTimeSpan));

                LogUtil.LogDebug("--KC.Service.Extension.ClientExtensions accessToken: " + accessToken);
            }

            return accessToken;
        }

        private static string GetTenantTokenEndpoint(string url, string tenantName)
        {
            var uri = new Uri(url);
            if (uri.Port > 0)
                return string.Format("{0}://{1}.{2}:{3}{4}", uri.Scheme, tenantName, uri.Host, uri.Port, uri.PathAndQuery);
            return string.Format("{0}://{1}.{2}{3}", uri.Scheme, tenantName, uri.Host, uri.PathAndQuery);
        }

        #endregion
    }

    public interface IMyCachedTest
    {
        [CachingCallHandler()]
        string GetCachedString(string userId, Tenant tenant);

        [CachingCallHandler()]
        Task<UserLoginDTO> GetCachedObject(string userId, Tenant tenant);

        [CachingDeleteHandler(
            new Type[] { typeof(IMyCachedTest) , typeof(IMyCachedTest) },
            new string[] { "GetCachedString", "GetCachedObject" })]
        string GetDeleteCachedString(string userId, Tenant tenant);
    }

    public class MyCachedTest : IMyCachedTest, Service.EFService.IEFService
    {
        public Tenant Tenant { get; set; }

        public MyCachedTest()
        {
            Tenant = TenantConstant.TestTenantApiAccessInfo;
        }

        public string GetCachedString(string userId, Tenant tenant)
        {
            string.Format("---inner---GetCachedString: 【{0}】, 【{1}】", userId, tenant.GetHashCode()).ConsoleYellow();
            return userId;
        }

        public async Task<UserLoginDTO> GetCachedObject(string userId, Tenant tenant)
        {
            string.Format("---inner---GetCachedObject: 【{0}】, 【{1}】", userId, tenant.GetHashCode()).ConsoleYellow();
            return new UserLoginDTO()
            {
                ApplicationId = userId,
                Email = "test@test.com",
                Password = "password",
            };
        }

        public string GetDeleteCachedString(string userId, Tenant tenant)
        {
            string.Format("---inner---GetDeleteCachedString: 【{0}】, 【{1}】", userId, tenant.GetHashCode()).ConsoleYellow();
            return userId;
        }
    }
}
