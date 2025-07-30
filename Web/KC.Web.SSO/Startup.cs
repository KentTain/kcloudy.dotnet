using KC.IdentityModel;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Reflection;

using KC.DataAccess.Account;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Account;
using KC.Web.SSO.Services;
using KC.Web.Extension;
using Microsoft.IdentityModel.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using KC.Framework.Extension;
using KC.Service.WebApiService.Business;
using KC.Framework.Util;
using System.Linq;

namespace KC.Web.SSO
{
    public class Startup
    {
        public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            var envFile = $"appsettings.{env.EnvironmentName}.json";
            if (env.EnvironmentName.Equals("Production", System.StringComparison.OrdinalIgnoreCase))
            {
                envFile = "appsettings.json";
            }
            //增加环境配置文件，新建项目默认有
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(envFile, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region 相关服务器的配置
            //Hosting in IIS
            services.Configure<IISOptions>(iis =>
            {
                iis.ForwardClientCertificate = false;
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });
            #endregion

            #region 配置文件的读取及静态类GlobalConfig的设置
            //services.Configure<GlobalConfig>(Configuration.GetSection("AppSettings"));
            //services.Configure<UploadConfig>(Configuration.GetSection("UploadConfig"));

            GlobalConfig.InitGlobalConfig(Configuration);
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var httpClient = scope.ServiceProvider.GetService<System.Net.Http.IHttpClientFactory>();
                var globalConfigData = new GlobalConfigApiService(httpClient).GetGlobalConfigData();
                GlobalConfig.InitGlobalConfigWithApiData(Configuration, globalConfigData);

                var message = string.Format("GlobalConfig SystemType: {0}; EncryptKey: {1}; Resource WebDomain: {2}; SSO WebDomain: {3}; Current WebDomain: {4}; Get GlobalData from Resource is success? {5}",
            GlobalConfig.SystemType, GlobalConfig.EncryptKey, GlobalConfig.ResWebDomain, GlobalConfig.SSOWebDomain, GlobalConfig.CurrentApplication?.AppDomain, (globalConfigData != null));

                Service.Extension.ConsoleExtensions.ConsoleGreen(message);
            }
            #endregion

            #region 缓存设置
            switch (GlobalConfig.SystemType)
            {
                case SystemType.Product:
                case SystemType.Beta:
                    services.AddDistributedServiceStackRedisCache(options =>
                    {
                        options.Configuration = GlobalConfig.GetDecryptRedisConnectionString();
                    }).AddSession();
                    break;
                default:
                    services.AddDistributedMemoryCache().AddSession();
                    break;
            }
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var cache = scope.ServiceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                Service.CacheUtil.Cache = cache;
            }
            #endregion

            #region Service的注入

            services.AddHttpClient();
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            // 获取Tenant对象
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //解决ViewBag的中文编码问题
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            //对象映射
            var profiles = KC.Service.AutoMapper.AutoMapperConfiguration.GetAllProfiles()
                .Union(KC.Service.Account.AutoMapper.AutoMapperConfiguration.GetAllProfiles())
                .Union(KC.Service.Admin.AutoMapper.AutoMapperConfiguration.GetAllProfiles()); 
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(profiles);
            });
            services.AddSingleton(config);
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            // 相关Reposito及Service
            KC.Service.Util.DependencyInjectUtil.InjectService(services);
            KC.Service.Account.DependencyInjectUtil.InjectService(services);
            KC.Service.Admin.DependencyInjectUtil.InjectService(services);

            //services.AddSingleton<ITenantUserService, TenantUserService>();
            //services.AddHttpClient<ITenantUserService, TenantUserService>()
            //    .SetHandlerLifetime(TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut));

            services.AddMultitenancy<Tenant, TenantResolver>();

            #endregion

            #region 认证服务器的设置
            services
                .AddMemoryCache()
                .AddEntityFrameworkSqlServer()
                .AddScoped<IMigrationsSqlGenerator, ComAccountMigrationsSqlGenerator>()
                //.AddScoped<IModelCacheKeyFactory, ComAccountModelCacheKeyFactory>()
                .AddDbContext<ComAccountContext>();
            services.Replace(ServiceDescriptor.Singleton<IModelCacheKeyFactory, ComAccountModelCacheKeyFactory>());

            services.AddIdentity<User, Role>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredUniqueChars = 6;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.AllowedForNewUsers = false;

                    // User settings
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters = null;

                    options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                    options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                    options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
                })
                .AddClaimsPrincipalFactory<TenantUserClaimsPrincipal>()
                //.AddUserStore<TenantUserStore>()
                .AddUserManager<TenantUserManager>()
                .AddRoleManager<TenantRoleManager>()
                .AddSignInManager<TenantSignInManager>()
                .AddEntityFrameworkStores<ComAccountContext>()
                .AddDefaultTokenProviders();

            #endregion

            services.ConfigureNonBreakingSameSiteCookies();

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            //Configure KC.IdentityServer4 autenticazione
            ConfigureIdentityServer4AuthService(services);

            var message1 = string.Format("Current DbConnect: {0}; Storage Connect: {1}; Queue Connect: {2}; ServiceBus Connect: {3}; NoSql Connect: {4}",
GlobalConfig.DatabaseConnectionString, GlobalConfig.StorageConnectionString, GlobalConfig.QueueConnectionString, GlobalConfig.ServiceBusConnectionString, GlobalConfig.NoSqlConnectionString);
            Service.Extension.ConsoleExtensions.ConsoleYellow(message1);

            services.AddControllersWithViews(config =>
            {
                config.Filters.Add<GlobalViewModelActionFilter>();
                //config.Filters.Add<CompanyAuthActionFilter>();
            });
            IMvcBuilder builder = services.AddRazorPages();
#if DEBUG
            builder.AddRazorRuntimeCompilation();
#endif

            //WebApi客户端：GlobalConfig对象的设置、认证及授权配置
            //StaticFactoryUtil.ConfigureApiClientService(services, Configuration, ApplicationConstant.SsoScope);

        }

        /// <summary>
        /// 配置KC.IdentityServer4认证服务器
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureIdentityServer4AuthService(IServiceCollection services)
        {
            //添加KC.IdentityServer4对EFCore数据库的支持
            //但是这里需要初始化数据 默认生成的数据库中是没有配置数据
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Authentication Server Uri
            //var optionsAccessor = services.BuildServiceProvider().GetService<IOptions<GlobalConfig>>();
            //var settings = optionsAccessor.Value;
            var authUrl = Web.Constants.OpenIdConnectConstants.GetAuthUrlByConfig(null);

            //var tenantService = services.BuildServiceProvider().GetService<ITenantUserService>();
            //var tenants = tenantService.GetInitalTenants();

            #region KC.IdentityServer4  
            //结合EFCore生成KC.IdentityServer4数据库
            // 项目工程文件最后添加 <ItemGroup><DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" /></ItemGroup>

            //dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
            //dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb

            #region 添加对IdentiyServer4配置内容处理
            services.AddIdentityServer(idroptions =>
            {
                //设置将在发现文档中显示的颁发者名称和已发布的JWT令牌。建议不要设置此属性，该属性从客户端使用的主机名中推断颁发者名称
                //idroptions.IssuerUri = "";
                //设置认证
                idroptions.Authentication = new KC.IdentityServer4.Configuration.AuthenticationOptions
                {
                    //监控浏览器cookie不难发现sso.oauth.cookies=8660972474e55224ff37f7421c79a530 实际是cookie记录服务器session的名称
                    // "sso.SessionId", // CookieAuthenticationDefaults.AuthenticationScheme,//用于检查会话端点的cookie的名称
                    CheckSessionCookieName = Web.Constants.OpenIdConnectConstants.ClientCookieName, 
                    //身份验证Cookie生存期（仅在使用IdentityServer提供的Cookie处理程序时有效）
                    CookieLifetime = TimeSpan.FromMinutes(Service.Constants.TimeOutConstants.AccessTokenTimeOut),
                    //指定cookie是否应该滑动（仅在使用IdentityServer提供的cookie处理程序时有效）
                    CookieSlidingExpiration = true,
                    //指示是否必须对用户进行身份验证才能接受参数以结束会话端点。默认为false
                    RequireAuthenticatedUserForSignOutMessage = true 
                };
                //活动事件 允许配置是否应该将哪些事件提交给注册的事件接收器
                idroptions.Events = new KC.IdentityServer4.Configuration.EventsOptions
                {
                    RaiseErrorEvents = true,
                    RaiseFailureEvents = true,
                    RaiseSuccessEvents = true,
                    RaiseInformationEvents = true
                };
                //允许设置各种协议参数（如客户端ID，范围，重定向URI等）的长度限制
                //idroptions.InputLengthRestrictions = new KC.IdentityServer4.Configuration.InputLengthRestrictions
                //{
                //    //可以看出下面很多参数都是对长度的限制 
                //    AcrValues = 100,
                //    AuthorizationCode = 100,
                //    ClientId = 100,
                //    /*
                //    ..
                //    ..
                //    ..
                //    */
                //    ClientSecret = 1000
                //};
                //用户交互页面定向设置处理
                idroptions.UserInteraction = new KC.IdentityServer4.Configuration.UserInteractionOptions
                {
                    //【必备】登录地址  
                    LoginUrl = authUrl + "/Account/Login",
                    //【必备】退出地址 
                    LogoutUrl = authUrl + "/Account/Logout",
                    //【必备】允许授权同意页面地址
                    ConsentUrl = authUrl + "/Consent/Index",
                    //【必备】错误页面地址
                    ErrorUrl = authUrl + "/Error/Index",
                    //【必备】设置传递给登录页面的返回URL参数的名称。默认为returnUrl 
                    LoginReturnUrlParameter = "returnUrl",
                    //【必备】设置传递给注销页面的注销消息ID参数的名称。缺省为logoutId 
                    LogoutIdParameter = "logoutId",
                    //【必备】设置传递给同意页面的返回URL参数的名称。默认为returnUrlConsentReturnUrlParameter = "returnUrl", 
                    //【必备】设置传递给错误页面的错误消息ID参数的名称。缺省为errorId
                    ErrorIdParameter = "errorId",
                    //【必备】设置从授权端点传递给自定义重定向的返回URL参数的名称。默认为returnUrlCustomRedirectReturnUrlParameter = "returnUrl", 
                    //【必备】由于浏览器对Cookie的大小有限制，设置Cookies数量的限制，有效的保证了浏览器打开多个选项卡，一旦超出了Cookies限制就会清除以前的Cookies值
                    CookieMessageThreshold = 6 
                };
                //缓存参数处理  缓存起来提高了效率 不用每次从数据库查询
                idroptions.Caching = new KC.IdentityServer4.Configuration.CachingOptions
                {
                    //设置Client客户端存储加载的客户端配置的数据缓存的有效时间 
                    ClientStoreExpiration = new TimeSpan(0, Service.Constants.TimeOutConstants.AccessTokenTimeOut, 0),
                    // 设置从资源存储加载的身份和API资源配置的缓存持续时间
                    ResourceStoreExpiration = new TimeSpan(0, Service.Constants.TimeOutConstants.AccessTokenTimeOut, 0),
                    //设置从资源存储的跨域请求数据的缓存时间
                    CorsExpiration = new TimeSpan(0, Service.Constants.TimeOutConstants.AccessTokenTimeOut, 0)  
                };
                //IdentityServer支持一些端点的CORS。底层CORS实现是从ASP.NET Core提供的，因此它会自动注册在依赖注入系统中
                idroptions.Cors = new KC.IdentityServer4.Configuration.CorsOptions
                {
                    //支持CORS的IdentityServer中的端点。默认为发现，用户信息，令牌和撤销终结点
                    CorsPaths = new List<PathString>() {
                           new PathString("/")
                       },
                    //【必备】将CORS请求评估为IdentityServer的CORS策略的名称（默认为"KC.IdentityServer4"）。处理这个问题的策略提供者是ICorsPolicyService在依赖注入系统中注册的。如果您想定制允许连接的一组CORS原点，则建议您提供一个自定义的实现ICorsPolicyService
                    CorsPolicyName = "default",
                    //可为空的<TimeSpan>，指示要在预检Access-Control-Max-Age响应标题中使用的值。默认为空，表示在响应中没有设置缓存头
                    PreflightCacheDuration = new TimeSpan(0, Service.Constants.TimeOutConstants.AccessTokenTimeOut, 0)
                };
            })
            #endregion

            #region 添加KC.IdentityServer4 认证证书相关处理
            //AddSigningCredential 添加登录证书 这个是挂到KC.IdentityServer4中间件上  提供多种证书处理  RsaSecurityKey\SigningCredentials
            //这里可以采用IdentiServe4的证书封装出来
            //添加一个签名密钥服务，该服务将指定的密钥材料提供给各种令牌创建/验证服务。您可以从证书存储中传入X509Certificate2一个SigningCredential或一个证书引用
            //.AddSigningCredential(new System.Security.Cryptography.X509Certificates.X509Certificate2()
            //{
            //    Archived = true,
            //    FriendlyName = "",
            //    PrivateKey = System.Security.Cryptography.AsymmetricAlgorithm.Create("key")
            //})
            //AddDeveloperSigningCredential在启动时创建临时密钥材料。这是仅用于开发场景，当您没有证书使用。
            //生成的密钥将被保存到文件系统，以便在服务器重新启动之间保持稳定（可以通过传递来禁用false）。
            //这解决了在开发期间client / api元数据缓存不同步的问题
            .AddDeveloperSigningCredential()
            //添加验证令牌的密钥。它们将被内部令牌验证器使用，并将显示在发现文档中。
            //您可以从证书存储中传入X509Certificate2一个SigningCredential或一个证书引用。这对于关键的转换场景很有用
            //.AddValidationKeys(new AsymmetricSecurityKey[] {

            //}) 
            #endregion

            #region 添加KC.IdentityServer4用户缓存数据

            //添加配置数据全部配置到内存中 如果有EFCore数据库持久化这里不会配置 只需要配置 AddConfigurationStore、AddOperationalStore 数据仓储服务
            //寄存器IClientStore和ICorsPolicyService实现基于内存中的Client配置对象集合。
            //.AddInMemoryClients(IdentityServerConfig.GetClients())
            //IResourceStore基于IdentityResource配置对象的内存中收集来注册实现。
            //.AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
            //IResourceStore基于ApiResource配置对象的内存中收集来注册实现。
            //.AddInMemoryApiResources(IdentityServerConfig.GetApiResources())

            //添加测试用户
            //.AddTestUsers(new List<KC.IdentityServer4.Test.TestUser>() {

            //    new KC.IdentityServer4.Test.TestUser{
            //        SubjectId=Guid.NewGuid().ToString(),
            //        Username="liyouming",
            //        Password="liyouming"

            //    }
            //}) 
            #endregion

            #region 添加对KC.IdentityServer4 EF数据库持久化支持 

            .AddResourceStore<CustomResourceStore>()
            .AddClientStore<CustomClientStore>()

            //.AddConfigurationStore(options =>
            //{
            //    options.ConfigureDbContext = builder =>
            //    {
            //        builder.UseSqlServer(connectionString,
            //            builderoptions =>
            //            {
            //                builderoptions.MigrationsAssembly(migrationsAssembly);
            //            });
            //    };
            //})
            //.AddOperationalStore(options =>
            //{
            //    options.ConfigureDbContext = builder =>
            //    {
            //        builder.UseSqlServer(connectionString, builderoptions =>
            //        {
            //            builderoptions.MigrationsAssembly(migrationsAssembly);
            //        });

            //    };

            //    options.EnableTokenCleanup = true;  //允许对Token的清理
            //    options.TokenCleanupInterval = 1800;  //清理周期时间Secends
            //})
            #endregion

            .AddAspNetIdentity<User>()

            .AddJwtBearerClientAuthentication()
            
            .AddResourceOwnerValidator<TenantResourceOwnerPasswordValidator>()
            .AddProfileService<TenantProfileService>()

            //.AddCoreServices()
            //.AddSecretParser()
            //.AddSecretValidator()

            .AddDefaultSecretValidators()
            
            ;
            
            services.AddTransient<KC.IdentityServer4.Stores.IClientStore, CustomClientStore>();
            services.AddTransient<KC.IdentityServer4.Stores.IResourceStore, CustomResourceStore>();
            //services.AddTransient<KC.IdentityServer4.Services.IIdentityServerInteractionService, CustomIdentityServerInteractionService>();
            //services.AddTransient<KC.IdentityServer4.Validation.IClientSecretValidator, CustomClientSecretValidator>();
            //services.AddTransient<KC.IdentityServer4.Validation.ISecretValidator, CustomHashedSharedSecretValidator>();
            //services.AddTransient<KC.IdentityServer4.Validation.IApiSecretValidator, CustomApiSecretValidator>();
            //services.AddTransient<KC.IdentityServer4.Validation.ICustomTokenRequestValidator, CustomTokenRequestValidator>();


            #endregion


        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            //ASP.NET Core中如何显示[PII is hidden]的隐藏信息
            IdentityModelEventSource.ShowPII = true;

            //使用Nginx反向代理时，添加下面的代码
            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };
            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardOptions);

            app.UseDeveloperExceptionPage();

            //app.UseHttpsRedirection();
            //app.UseHsts();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //多租户访问
            app.UseMultitenancy<Tenant>();

            app.UseIdentityServer();

            app.UseRouting();

            //跨域访问
            app.UseCors(MyAllowSpecificOrigins);

            //认证、授权
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            //StaticFactoryUtil.UseApiClientService(app);

            //app.UseMvcWithDefaultRoute();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
                //endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
