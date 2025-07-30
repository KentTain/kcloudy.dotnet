using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KC.IdentityModel.AspNetCore.OAuth2Introspection;
using KC.IdentityServer4.AccessTokenValidation;
using KC.Common.LogHelper;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Web.Extension;
using KC.Service.WebApiService.Business;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using KC.Framework.Extension;
using AspectCore.Extensions.DependencyInjection;

namespace KC.Web.Util
{
	public static class StaticFactoryUtil
	{
		public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
		/// <summary>
		/// 初始化基础组件服务：</br>
		///     日志服务：LogUtil </br>
		///     模型匹配组件：AutoMapper </br>
		///     EntityFramework扩展：Z.EntityFramework </br>
		/// </summary>
		public static void InitWeb()
		{
			//文档编码格式支持中文：GB2312，需要引入：System.Text.Encoding.CodePages
			//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			//ConfigUtil.Config = new AppSettingsConfigService();
			LogUtil.Logger = new NlogLoggingService();

			KC.Service.Util.ProtobufUtil.InitProtobufSerialize();
		}

		#region Configure Web Client with KC.IdentityServer4
		/// <summary>
		/// 配置Web站点客户端以下服务（同一个Cookies）：</br>
		///     基础接口访问服务（ITenantUserApiService、IAccountApiService）</br>
		///     认证服务（基于Cookies策略及KC.IdentityServer4的OpenId Connect服务）</br>
		///     授权服务（PermissionFilterModelProvider、PermissionFilterPolicyProvider）</br>
		/// </summary>
		/// <param name="services">IServiceCollection</param>
		/// <param name="configuration">读取的配置文件</param>
		/// <param name="scopes">Client Access Scopes</param>
		/// <param name="isTenantDba">是否为Dba租户的认证配置</param>
		public static void ConfigureIdentityServer4ClientService(IServiceCollection services, IConfiguration configuration, string[] scopes, IEnumerable<AutoMapper.Profile> autoProfiles, bool isTenantDba = false)
		{
			var authBuilder = GetBasicAuthServicesBuilder(services, configuration, autoProfiles);

			#region 添加授权验证方式 这里是OpenId Connect(AddOpenIdConnect)
			var defaultTenant = isTenantDba
				? TenantConstant.DbaTenantApiAccessInfo
				: TenantConstant.TestTenantApiAccessInfo;

			//监控浏览器Cookies不难发现有这样一个 .AspNetCore.sso.Cookies 记录了加密的授权信息 
			authBuilder.AddOpenIdConnect(Web.Constants.OpenIdConnectConstants.ChallengeScheme, options =>
			{
				InitOpenIdConnectOptions(options, defaultTenant);

				foreach (var scope in scopes)
				{
					options.Scope.Add(scope);
				}
			});
			#endregion

			services.AddControllersWithViews();
			services.AddRazorPages();
		}

		/// <summary>
		/// 配置Web站点客户端以下服务（不同租户使用不同的Cookies）</br>
		/// 方法一： 使用控制Option的方式：</br>
		///     基础接口访问服务（ITenantUserApiService、IAccountApiService）</br>
		///     认证服务（基于Cookies策略及KC.IdentityServer4的OpenId Connect服务）</br>
		///     授权服务（PermissionFilterModelProvider、PermissionFilterPolicyProvider）</br>
		/// </summary>
		/// <param name="services">IServiceCollection</param>
		/// <param name="configuration">读取的配置文件</param>
		/// <param name="scopes">Client Access Scopes</param>
		public static void ConfigureIdentityServer4ClientServiceWithOptionsExtension(IServiceCollection services, IConfiguration configuration, string[] scopes, IEnumerable<AutoMapper.Profile> autoProfiles)
		{
			var authBuilder = GetBasicAuthServicesBuilder(services, configuration, autoProfiles);

			//方法二： 使用控制Option的方式
			services.AddTransient<IConfigureOptions<CookieAuthenticationOptions>, MultiTenantCookieOptions>();
			services.AddTransient<IConfigureOptions<OpenIdConnectOptions>, MultiTenantOpenIdConnectOptions>();

			#region 添加授权验证方式 这里是OpenId Connect(AddOpenIdConnect)

			authBuilder.AddOpenIdConnect(Web.Constants.OpenIdConnectConstants.ChallengeScheme, options =>
			{
				foreach (var scope in scopes)
				{
					options.Scope.Add(scope);
				}
			});

			#endregion

			services.AddControllersWithViews();
			services.AddRazorPages();
		}

		/// <summary>
		/// 配置Web站点客户端以下服务（不同租户使用不同的Cookies）</br>
		/// 方法二： 使用Provider的方式：</br>
		///     基础接口访问服务（ITenantUserApiService、IAccountApiService）</br>
		///     认证服务（基于Cookies策略及KC.IdentityServer4的OpenId Connect服务）</br>
		///     授权服务（PermissionFilterModelProvider、PermissionFilterPolicyProvider）</br>
		/// </summary>
		/// <param name="services">IServiceCollection</param>
		/// <param name="configuration">读取的配置文件</param>
		/// <param name="scopes">Client Access Scopes</param>
		public static void ConfigureIdentityServer4ClientServiceWithProviderExtension(IServiceCollection services, IConfiguration configuration, string[] scopes, IEnumerable<AutoMapper.Profile> autoProfiles, bool hasApiAuth = false)
		{
			var authScope = scopes.LastOrDefault();
			var authBuilder = GetBasicAuthServicesBuilder(services, configuration, autoProfiles, hasApiAuth, authScope);

			//方法三： 使用Provider的方式
			services.AddSingleton<IOptionsMonitor<CookieAuthenticationOptions>, MultiTenantCookieOptionsProvider>();
			services.AddSingleton<IOptionsMonitor<OpenIdConnectOptions>, MultiTenantOpenIdConnectOptionsProvider>();

			#region 添加授权验证方式 这里是OpenId Connect(AddOpenIdConnect)

			//监控浏览器Cookies不难发现有这样一个.AspNetCore.sso.Cookies 记录了加密的授权信息 
			authBuilder.AddOpenIdConnect(Web.Constants.OpenIdConnectConstants.ChallengeScheme, options =>
			{
				//https://stackoverflow.com/questions/60565045/KC.IdentityServer4-on-docker-does-not-work-in-chrome-after-upgrading-net-core-v2-1
				//options.NonceCookie.SameSite = SameSiteMode.Lax;
				//options.CorrelationCookie.SameSite = SameSiteMode.Lax;

				foreach (var scope in scopes)
				{
					options.Scope.Add(scope);
				}
			});

			#endregion

			//https://github.com/IdentityServer/IdentityServer4/issues/861
			services.ConfigureApplicationCookie(options =>
			{
				options.Cookie.Name = "MultiTenant-Cookie";
				options.Cookie.Expiration = TimeSpan.FromDays(1);
				options.ExpireTimeSpan = TimeSpan.FromDays(1);
			});

			services.AddControllersWithViews();
			services.AddRazorPages();

			//services.AddMvc(option => {
			//	option.UseCentralRoutePrefix(new Microsoft.AspNetCore.Mvc.RouteAttribute("/api/v1"));//在各个控制器添加前缀（没有特定的路由前面添加前缀）
			//	//opt.UseCentralRoutePrefix(new RouteAttribute("api/[controller]/[action]"));
			//});

		}

		public static void UseWebClientService(IApplicationBuilder app, bool addDefaultRoute = true, bool hasApiAuth = false)
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
			app.UseRouting();
			//跨域访问
			app.UseCors(MyAllowSpecificOrigins);

			if (hasApiAuth) 
			{
				// Enable middleware to serve generated Swagger as a JSON endpoint.
				app.UseSwagger();

				// Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
				app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint("/swagger/api/swagger.json", GlobalConfig.CurrentApplication?.AppName + "接口文档");

					//当RoutePrefix值为空时，即设置Swagger页面为首页
					options.RoutePrefix = "apidoc";  // Set Swagger UI at apps root
					options.OAuthAppName("api's Swagger Test");
					options.OAuthClientId(Constants.OpenIdConnectConstants.WebApiTestClient_ClientId);
					options.OAuthAdditionalQueryStringParams(
						new Dictionary<string, string> { { TenantConstant.ClaimTypes_TenantName, "cdba" } });

					//options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
				});
			}

			//多租户访问
			app.UseMultitenancy<Tenant>();

			//认证、授权
			app.UseAuthentication();
			app.UseAuthorization();

			////异常处理
			//app.UseExceptionHandler(options =>
			//{
			//    options.Run(
			//    async context =>
			//    {
			//        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			//        context.Response.ContentType = "application/json;charset=utf-8"; //此处要加上utf-8编码

			//        //如果不加此句，服务器返回的数据到浏览器会拒绝
			//        context.Response.Headers["Access-Control-Allow-Origin"] = "*";

			//        var ex = context.Features.Get<IExceptionHandlerFeature>();
			//        if (ex != null)
			//        {
			//            var errObj = new
			//            {
			//                message = ex.Error.Message,
			//                stackTrace = ex.Error.StackTrace,
			//                exceptionType = ex.Error.GetType().Name
			//            };

			//            await context.Response.WriteAsync(JsonConvert.SerializeObject(errObj)).ConfigureAwait(false);
			//        }
			//    });
			//});

			if (addDefaultRoute)
				app.UseEndpoints(endpoints =>
				{
					endpoints.MapControllers();
					//endpoints.MapRazorPages();
					endpoints.MapControllerRoute(
						name: "defaultAreaRoute",
						pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
					endpoints.MapDefaultControllerRoute();
					//endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
				});

		}

		/// <summary>
		/// 构建以下基础服务：</br>
		///     读取配置文件，设置全局变量GlobalConfig </br>
		///     设置分布式缓存（AddDistributedMemoryCache或是AddDistributedRedisCache）</br>
		///     基础接口访问服务（ITenantUserApiService、IAccountApiService及多租户服务AddMultitenancy）</br>
		///     基础认证方式（AddAuthentication）这里是Cookies（AddCookie）</br>
		///     授权服务（PermissionFilterModelProvider、PermissionFilterPolicyProvider）</br>
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		private static AuthenticationBuilder GetBasicAuthServicesBuilder(IServiceCollection services, IConfiguration configuration, IEnumerable<AutoMapper.Profile> autoProfiles, bool hasApiAuth = false, string authScope = "")
		{
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

			//NetCore 3.1 默认同步读取请求流为异步方式，默认不支持同步读取
			//// If using Kestrel:
			//services.Configure<KestrelServerOptions>(options =>
			//{
			//    options.AllowSynchronousIO = true;
			//});

			//// If using IIS:
			//services.Configure<IISServerOptions>(options =>
			//{
			//    options.AllowSynchronousIO = true;
			//});

			//https://www.thinktecture.com/en/identity/samesite/prepare-your-identityserver/
			services.ConfigureNonBreakingSameSiteCookies();
			// 配置AspectCore动态代理
			services.ConfigureDynamicProxy();
			// 注入HttpClient
			services.AddHttpClient();
			// https://github.com/aspnet/KestrelHttpServer/issues/475
			//services.Configure<FormOptions>(x => {
			//    x.ValueLengthLimit = int.MaxValue;
			//    x.MultipartBodyLengthLimit = int.MaxValue;
			//    x.MultipartHeadersLengthLimit = int.MaxValue;
			//});

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			//解决ViewBag的中文编码问题
			services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

			#region 配置文件的读取及静态类GlobalConfig的设置
			//静态类GlobalConfig的设置
			GlobalConfig.InitGlobalConfig(configuration);
			//SSO站点未配置文件站点，无需从接口获取后进行重新设置
			if (GlobalConfig.ApplicationGuid != ApplicationConstant.SsoAppId)
			{
				using (var scope = services.BuildServiceProvider().CreateScope())
				{
					var httpClient = scope.ServiceProvider.GetService<System.Net.Http.IHttpClientFactory>();
					var globalConfigData = new GlobalConfigApiService(httpClient).GetGlobalConfigData();
					GlobalConfig.InitGlobalConfigWithApiData(configuration, globalConfigData);

					var message = string.Format("GlobalConfig SystemType: {0}; EncryptKey: {1}; Resource WebDomain: {2}; SSO WebDomain: {3}; Current WebDomain: {4}; Get GlobalData from Resource is success? {5}",
			GlobalConfig.SystemType, GlobalConfig.EncryptKey, GlobalConfig.ResWebDomain, GlobalConfig.SSOWebDomain, GlobalConfig.CurrentApplication?.AppDomain, (globalConfigData != null));
					Service.Extension.ConsoleExtensions.ConsoleGreen(message);
				}
			}

			#endregion

			#region 缓存设置
			//services.AddMemoryCache();
			if (GlobalConfig.IsDevelopment)
			{
				services.AddDistributedMemoryCache().AddSession();
			}
			else
			{
				services.AddDistributedServiceStackRedisCache(options =>
				{
					options.Configuration = GlobalConfig.GetDecryptRedisConnectionString();
				}).AddSession();
			}

			using (var scope = services.BuildServiceProvider().CreateScope())
			{
				var cache = scope.ServiceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
				Service.CacheUtil.Cache = cache;
			}
			#endregion

			#region AutoMapper对象注入

			var config = new AutoMapper.MapperConfiguration(cfg =>
			{
				cfg.AddProfiles(autoProfiles);
			});
			services.AddSingleton(config);
			var mapper = config.CreateMapper();
			services.AddSingleton(mapper);

			#endregion

			#region 基础接口访问服务（ITenantUserApiService、IAccountApiService及多租户服务AddMultitenancy）

			//Multi-tenancy设置
			services.AddMultitenancy<Tenant, TenantResolver>();

			KC.Service.Util.DependencyInjectUtil.InjectService(services);

			//集成Polly: https://www.cnblogs.com/runningsmallguo/p/9692001.html
			//https://docs.microsoft.com/zh-cn/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
			//services.AddHttpClient<ITenantUserApiService, TenantUserApiService>()
			//    .SetHandlerLifetime(TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut));
			//services.AddHttpClient<IAccountApiService, AccountApiService>()
			//    .SetHandlerLifetime(TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut));
			#endregion

			#region 授权方式（PermissionFilterModelProvider、PermissionFilterPolicyProvider）

			services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, Extension.PermissionFilterModelProvider>());
			services.TryAdd(ServiceDescriptor.Transient<IAuthorizationPolicyProvider, Extension.PermissionFilterPolicyProvider>());

			#endregion

			#region 全局配置Json序列化处理
			services.AddControllersWithViews(config =>
				{
					config.Filters.Add<GlobalViewModelActionFilter>();
				})
				.AddNewtonsoftJson(options =>
				{
					//忽略循环引用
					options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
					//忽略空值
					options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
					//设置时间格式
					//options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
					options.SerializerSettings.Converters.Add(new Common.DateTimeNewtonJsonConverter());
					options.SerializerSettings.Converters.Add(new Common.BooleanNewtonJsonConverter());
					var resolver = options.SerializerSettings.ContractResolver;
					if (resolver != null)
					{
						var res = resolver as Newtonsoft.Json.Serialization.DefaultContractResolver;
						//res.NamingStrategy = null;  // <<!-- this removes the camelcasing
					}
				});
			#endregion

			#region 添加基础认证方式（AddAuthentication）这里是Cookies、接口Bearer

			var builder = GetAuthenticationBuilder(services, hasApiAuth, authScope);

			//https://stackoverflow.com/questions/63503864/KC.IdentityServer4-set-expiration-date-to-idsrv-session-cookie
			services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = TimeSpan.FromMinutes(Service.Constants.TimeOutConstants.DefaultCacheTimeOut));
			#endregion

			#region 权限控制、跨越CORS设置
			services.AddCors(options =>
			{
				options.AddPolicy(MyAllowSpecificOrigins,
					builder => builder
						//.WithOrigins("")
						//.AllowCredentials()
						.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader()
				);
			});


			#endregion

			return builder;
		}

		private static AuthenticationBuilder GetAuthenticationBuilder(IServiceCollection services, bool hasApiAuth = false, string authScope = "")
		{
			if (!hasApiAuth)
				return services
				.AddAuthentication(
					options =>
					{
						//默认为（"Cookies"）：CookieAuthenticationDefaults.AuthenticationScheme
						//options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
						//options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
						//options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
						//options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

						options.DefaultScheme = Web.Constants.OpenIdConnectConstants.AuthScheme;
						options.DefaultSignInScheme = Web.Constants.OpenIdConnectConstants.AuthScheme;

						//默认为（"oidc"）:OpenIdConnectDefaults.AuthenticationScheme
						options.DefaultChallengeScheme = Web.Constants.OpenIdConnectConstants.ChallengeScheme;
					})
				.AddCookie(Web.Constants.OpenIdConnectConstants.AuthScheme, 
					options =>
					{
						options.SessionStore = new MemoryCacheTicketStore(Service.Constants.TimeOutConstants.CookieTimeOut);
						options.Cookie = new CookieBuilder() { Expiration = TimeSpan.FromMinutes(Service.Constants.TimeOutConstants.CookieTimeOut) };

						options.ExpireTimeSpan = TimeSpan.FromMinutes(Service.Constants.TimeOutConstants.CookieTimeOut);
						options.SlidingExpiration = true;
						options.AccessDeniedPath = "/Home/AccessDenied";
					});

			var authUri = Web.Constants.OpenIdConnectConstants.GetAuthUrlByConfig(null);
			var appAuthScheme = "CustomerScheme";
			var builder = services
				.AddAuthentication(
					options =>
					{
						//默认为（"Cookies"）：CookieAuthenticationDefaults.AuthenticationScheme
						//options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
						//options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
						//options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
						//options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

						//options.DefaultScheme = Web.Constants.OpenIdConnectConstants.AuthScheme;
						//options.DefaultSignInScheme = Web.Constants.OpenIdConnectConstants.AuthScheme;

						//自定义Scheme，用来同时支持Cookie（Web）和JWT（WebApi）认证
						options.DefaultScheme = appAuthScheme;

						//默认为（"oidc"）:OpenIdConnectDefaults.AuthenticationScheme
						options.DefaultChallengeScheme = Web.Constants.OpenIdConnectConstants.ChallengeScheme;
					})
				//解决同时使用Cookie（Web）和JWT（WebApi）认证：https://www.cnblogs.com/neozhu/p/13181074.html
				//https://stackoverflow.com/questions/46938248/asp-net-core-2-0-combining-cookies-and-bearer-authorization-for-the-same-endpoin
				//这里是关键,添加一个Policy来根据http head属性或是/api来确认使用cookie还是jwt chema
				.AddPolicyScheme(appAuthScheme, "CookiesOrBearer", 
					options =>
					{
						options.ForwardDefaultSelector = context =>
						{
							var bearerAuth = context.Request.Headers["Authorization"].FirstOrDefault()?.StartsWith("Bearer ") ?? false;
							// You could also check for the actual path here if that's your requirement:
							// eg: if (context.HttpContext.Request.Path.StartsWithSegments("/api", StringComparison.InvariantCulture))
							if (bearerAuth)
								return JwtBearerDefaults.AuthenticationScheme;
							else
								return CookieAuthenticationDefaults.AuthenticationScheme;
						};
					})
				.AddCookie(Web.Constants.OpenIdConnectConstants.AuthScheme, 
					options =>
					{
						options.SessionStore = new MemoryCacheTicketStore(Service.Constants.TimeOutConstants.CookieTimeOut);
						options.Cookie = new CookieBuilder() { Expiration = TimeSpan.FromMinutes(Service.Constants.TimeOutConstants.CookieTimeOut) };

						options.ExpireTimeSpan = TimeSpan.FromMinutes(Service.Constants.TimeOutConstants.CookieTimeOut);
						options.SlidingExpiration = true;
						options.AccessDeniedPath = "/Home/AccessDenied";
					})
				//接口授权
				.AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme,
					options =>
					{
						options.Authority = authUri;
						options.ApiName = authScope;
							//options.ApiSecret = "secret";

							options.RequireHttpsMetadata = false;
						options.Events = new OpenIdConnectEvents
						{
								// 未授权时，重定向到OIDC服务器时触发
								OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,

								// 获取到授权码时触发
								OnAuthorizationCodeReceived = OnAuthorizationCodeReceived,
								// 接收到OIDC服务器返回的认证信息（包含Code, ID Token等）时触发
								OnMessageReceived = OnMessageReceived,
								// 接收到UserInfoEndpoint返回的信息时触发
								OnUserInformationReceived = OnUserInformationReceived,
								// 接收到Ticket返回的信息时触发
								OnTicketReceived = OnTicketReceived,
								// 接收到TokenEndpoint返回的信息时触发
								OnTokenResponseReceived = OnTokenResponseReceived,
								// 验证Token时触发
								OnTokenValidated = OnTokenValidated,

								// 出现异常时触发
								OnAuthenticationFailed = OnAuthenticationFailed,

								// 退出时，重定向到OIDC服务器时触发
								OnRedirectToIdentityProviderForSignOut = OnRedirectToIdentityProviderForSignOut,
								// OIDC服务器退出后，服务端回调时触发
								OnRemoteSignOut = OnRemoteSignOut,
								// OIDC服务器退出后，服务端失败时触发
								OnRemoteFailure = OnRemoteFailure,
								// OIDC服务器退出后，客户端重定向时触发
								OnSignedOutCallbackRedirect = OnSignedOutCallbackRedirect,
						};
					});

			services.AddAuthorization(options =>
			{
				options.DefaultPolicy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser()
					.Build();
			});

			#region Api接口文档：Swagger设置
			//Github：https://github.com/domaindrivendev/Swashbuckle.AspNetCore
			//帮助文档：https://docs.microsoft.com/zh-cn/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-2.1
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("api", new OpenApiInfo
				{
					//Version = "v1",
					Title = "接口文档",
					Description = "RESTful API for User's Account",
					Contact = new OpenApiContact { Name = "Kent Tian", Email = "tianchangjun@outlook.com", Url = new Uri("http://www.kcloudy.com") }
				});

				//solve the problem: The same schemaId is already used for enum type
				//Github：https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1607#issuecomment-607170559
				options.CustomSchemaIds(type => type.ToString());

				//https://ctest.sso.kcloudy.com/connect/authorize?response_type=token&redirect_uri=http%3A%2F%2Flocalhost%3A39106%2Fswagger%2Fui%2Fo2c-html&realm=test-realm&client_id=api_test_api_flow&scope=all%20%20&state=oauth2
				options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.OAuth2,
					Flows = new OpenApiOAuthFlows
					{
						Implicit = new OpenApiOAuthFlow
						{
							AuthorizationUrl = new Uri(authUri + "/connect/authorize", UriKind.Absolute),
							TokenUrl = new Uri(authUri + "/connect/token", UriKind.Absolute),
							Scopes = new Dictionary<string, string>
							{
								{ authScope, "REST API - full access" }
							}
						}
					}
				});
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme
							{
								Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
							},
							new[] { authScope, "writeAccess" }
						}
					});

				//Set the comments path for the swagger json and ui.
				var xmlPath = Path.Combine(AppContext.BaseDirectory, "Xml/ApiSetting.xml");
				options.IncludeXmlComments(xmlPath);

				options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
				options.OperationFilter<Extension.SwaggerAuthorizeOperationFilter>(); // 添加httpHeader参数
			});
			#endregion

			return builder;
		}

		#endregion

		#region Init OpenIdConnect Options
		public static void InitOpenIdConnectOptions(OpenIdConnectOptions options, Tenant tenant)
		{
			options.Authority = Web.Constants.OpenIdConnectConstants.GetAuthUrlByConfig(tenant.TenantName);
			options.ClientId = TenantConstant.GetClientIdByTenant(tenant); //TODO: multi-tenant
			options.ClientSecret = TenantConstant.GetClientSecretByTenant(tenant);//TODO: multi-tenant
			options.ResponseType = OpenIdConnectResponseType.Code;
			// adding this line fixed my problem. and i can now logout permanently. 
			options.AuthenticationMethod = OpenIdConnectRedirectBehavior.FormPost;

			//布尔值来设置处理程序是否应该转到用户信息端点检索。额外索赔或不在id_token创建一个身份收到令牌端点。默认为“false”
			options.GetClaimsFromUserInfoEndpoint = true;
			options.RequireHttpsMetadata = false;
			options.SaveTokens = true;

			//默认是：AuthenticationOptions.DefaultSignInScheme(设置方式：services.AddAuthentication)
			options.SignInScheme = Web.Constants.OpenIdConnectConstants.AuthScheme;
			//不赋值的话，其中默认为SignInScheme的值
			options.SignOutScheme = Web.Constants.OpenIdConnectConstants.AuthScheme;

			//对应服务器端设置：KC.IdentityServer4.Models.Client.RedirectUris
			options.CallbackPath = new PathString(Web.Constants.OpenIdConnectConstants.CallbackPath);
			//对应服务器端设置：KC.IdentityServer4.Models.Client.PostLogoutRedirectUris
			//options.SignedOutCallbackPath = new PathString(Web.Constants.OpenIdConnectConstants.SignOutPath);
			options.RemoteSignOutPath = new PathString(Web.Constants.OpenIdConnectConstants.SignOutPath);
			options.RemoteAuthenticationTimeout = TimeSpan.FromSeconds(10);

			// 方法一：不去验证Issuer
			//options.TokenValidationParameters.ValidateIssuer = false;
			// 方法二：自定义验证Issuer，例如：http://ctest.localhost:1001
			//options.TokenValidationParameters.IssuerValidator = ValidateIssuerWithPlaceholder;

			// 在本示例中，使用的是IdentityServer，而它的ClaimType使用的是JwtClaimTypes。
			options.TokenValidationParameters.NameClaimType = KC.IdentityModel.JwtClaimTypes.Name;
			options.TokenValidationParameters.RoleClaimType = KC.IdentityModel.JwtClaimTypes.Role;
			//options.ResponseMode = OpenIdConnectResponseMode.FormPost; 

			options.Events = new OpenIdConnectEvents
			{
				// 未授权时，重定向到OIDC服务器时触发
				OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,

				// 获取到授权码时触发
				OnAuthorizationCodeReceived = OnAuthorizationCodeReceived,
				// 接收到OIDC服务器返回的认证信息（包含Code, ID Token等）时触发
				OnMessageReceived = OnMessageReceived,
				// 接收到UserInfoEndpoint返回的信息时触发
				OnUserInformationReceived = OnUserInformationReceived,
				// 接收到Ticket返回的信息时触发
				OnTicketReceived = OnTicketReceived,
				// 接收到TokenEndpoint返回的信息时触发
				OnTokenResponseReceived = OnTokenResponseReceived,
				// 验证Token时触发
				OnTokenValidated = OnTokenValidated,

				// 出现异常时触发
				OnAuthenticationFailed = OnAuthenticationFailed,

				// 退出时，重定向到OIDC服务器时触发
				OnRedirectToIdentityProviderForSignOut = OnRedirectToIdentityProviderForSignOut,
				// OIDC服务器退出后，服务端回调时触发
				OnRemoteSignOut = OnRemoteSignOut,
				// OIDC服务器退出后，服务端失败时触发
				OnRemoteFailure = OnRemoteFailure,
				// OIDC服务器退出后，客户端重定向时触发
				OnSignedOutCallbackRedirect = OnSignedOutCallbackRedirect,
			};
		}

		#region 自定义验证，未使用
		private static string ValidateIssuerWithPlaceholder(string issuer, SecurityToken token, TokenValidationParameters parameters)
		{
			// Accepts any issuer of the form "https://login.microsoftonline.com/{tenantid}/v2.0",
			// where tenantid is the tid from the token.

			if (token is JwtSecurityToken jwt)
			{
				if (jwt.Payload.TryGetValue("tid", out var value) &&
					value is string tokenTenantId)
				{
					var validIssuers = (parameters.ValidIssuers ?? Enumerable.Empty<string>())
						.Append(parameters.ValidIssuer)
						.Where(i => !string.IsNullOrEmpty(i));

					if (validIssuers.Any(i => i.Replace("{tenantid}", tokenTenantId) == issuer))
						return issuer;
				}
			}

			// Recreate the exception that is thrown by default
			// when issuer validation fails
			var validIssuer = parameters.ValidIssuer ?? "null";
			var validIssuersString = parameters.ValidIssuers == null
				? "null"
				: !parameters.ValidIssuers.Any()
					? "empty"
					: string.Join(", ", parameters.ValidIssuers);
			string errorMessage = FormattableString.Invariant(
				$"IDX10205: Issuer validation failed. Issuer: '{issuer}'. Did not match: validationParameters.ValidIssuer: '{validIssuer}' or validationParameters.ValidIssuers: '{validIssuersString}'.");

			throw new SecurityTokenInvalidIssuerException(errorMessage)
			{
				InvalidIssuer = issuer
			};
		}
		#endregion

		#region  OpenIdConnect Events事件

		// 获取到授权码时触发
		private static Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
		{
			WrapperContextWithTenant(context.Options, context.ProtocolMessage, context.HttpContext, "OnAuthorizationCodeReceived code: " + context.Result?.Ticket);

			return Task.CompletedTask;
		}
		// 接收到OIDC服务器返回的认证信息（包含Code, ID Token等）时触发
		private static Task OnMessageReceived(Microsoft.AspNetCore.Authentication.OpenIdConnect.MessageReceivedContext context)
		{
			WrapperContextWithTenant(context.Options, context.ProtocolMessage, context.HttpContext, "OnUserInformationReceived token: " + context.Token);

			return Task.CompletedTask;
		}
		// 接收到UserInfoEndpoint返回的信息时触发
		private static Task OnUserInformationReceived(UserInformationReceivedContext context)
		{
			WrapperContextWithTenant(context.Options, context.ProtocolMessage, context.HttpContext, "OnUserInformationReceived");

			return Task.CompletedTask;
		}
		// 接收到Ticket返回的信息时触发
		private static Task OnTicketReceived(TicketReceivedContext context)
		{
			var tenantContext = context.HttpContext.GetTenantContext<Tenant>();

			Framework.Util.LogUtil.LogDebug(string.Format("{0} OpenIdConnect OnTicketReceived: 【{1}】with callback path: [{2}]", tenantContext?.Tenant?.TenantName, context.Request?.Host.Value + context.Request?.Path.Value, context.Options.CallbackPath));
			return Task.CompletedTask;
		}
		// 接收到TokenEndpoint返回的信息时触发
		private static Task OnTokenResponseReceived(TokenResponseReceivedContext context)
		{
			WrapperContextWithTenant(context.Options, context.ProtocolMessage, context.HttpContext, "OnTokenResponseReceived");

			return Task.CompletedTask;
		}
		// 验证Token时触发
		private static Task OnTokenValidated(Microsoft.AspNetCore.Authentication.OpenIdConnect.TokenValidatedContext context)
		{
			WrapperContextWithTenant(context.Options, context.ProtocolMessage, context.HttpContext, "OnTokenValidated");

			return Task.CompletedTask;
		}
		// 出现异常时触发
		private static Task OnAuthenticationFailed(Microsoft.AspNetCore.Authentication.OpenIdConnect.AuthenticationFailedContext context)
		{
			var tenantContext = context.HttpContext.GetTenantContext<Tenant>();
			WrapperContextWithTenant(context.Options, context.ProtocolMessage, context.HttpContext, "OnAuthenticationFailed");

			Framework.Util.LogUtil.LogError(string.Format("{0} OpenIdConnect OnAuthenticationFailed throw error: {0}, stackTrace: {1}", context.Exception.Message, context.Exception.StackTrace));

			if (context.Exception.Message.Contains("Correlation failed"))
				context.Response.Redirect("/");
			else
				context.Response.Redirect("/Error");

			context.HandleResponse();

			return Task.CompletedTask;
		}
		// OIDC服务器退出后，服务端回调时触发
		private static Task OnRemoteSignOut(RemoteSignOutContext context)
		{
			WrapperContextWithTenant(context.Options, context.ProtocolMessage, context.HttpContext, "OnRemoteSignOut");

			return Task.CompletedTask;
		}
		// OIDC服务器退出后，服务端失败时触发
		private static Task OnRemoteFailure(RemoteFailureContext context)
		{
			var tenantContext = context.HttpContext.GetTenantContext<Tenant>();

			Framework.Util.LogUtil.LogDebug(string.Format("{0} OpenIdConnect OnRemoteFailure: 【{1}】", tenantContext?.Tenant?.TenantName, context.Request?.Host.Value + context.Request?.Path.Value));
			Framework.Util.LogUtil.LogError(string.Format("{0} OpenIdConnect OnRemoteSignOut throw error: {0}, stackTrace: {1}", context.Failure.Message, context.Failure.StackTrace));

			if (context.Failure.Message.Contains("Correlation failed"))
				context.Response.Redirect("/");
			else
				context.Response.Redirect("/Error");

			context.HandleResponse();

			return Task.CompletedTask;
		}
		// OIDC服务器退出后，客户端重定向时触发
		private static Task OnSignedOutCallbackRedirect(RemoteSignOutContext context)
		{
			WrapperContextWithTenant(context.Options, context.ProtocolMessage, context.HttpContext, "OnSignedOutCallbackRedirect");

			return Task.CompletedTask;
		}
		// 未授权时，重定向到OIDC服务器时触发
		private static Task OnRedirectToIdentityProvider(RedirectContext context)
		{
			WrapperContextWithTenant(context.Options, context.ProtocolMessage, context.HttpContext, "OnRedirectToIdentityProvider");

			return Task.CompletedTask;
		}
		// 退出时，重定向到OIDC服务器时触发
		private async static Task OnRedirectToIdentityProviderForSignOut(RedirectContext context)
		{
			WrapperContextWithTenant(context.Options, context.ProtocolMessage, context.HttpContext, "OnRedirectToIdentityProviderForSignOut");

			context.ProtocolMessage.IdTokenHint = await context.HttpContext.GetTokenAsync("id_token");
			context.ProtocolMessage.PostLogoutRedirectUri = context.Request.Scheme + "://" + context.Request.Host;

			//return Task.CompletedTask;
		}

		private static void WrapperContextWithTenant(OpenIdConnectOptions contextOptions, OpenIdConnectMessage contextMessage, HttpContext context, string logMessage = null)
		{
			var tenantContext = context.GetTenantContext<Tenant>();
			if (context != null && context.Items.ContainsKey("idp"))
			{
				var idp = context.Items["idp"];
				contextMessage.AcrValues = "idp:" + idp;
			}

			if (contextMessage.RequestType == OpenIdConnectRequestType.Authentication
				|| contextMessage.RequestType == OpenIdConnectRequestType.Token
				|| contextMessage.RequestType == OpenIdConnectRequestType.Logout)
			{
				var tenant = tenantContext?.Tenant;
				if (tenantContext == null || tenant == null)
				{
					tenant = TenantConstant.DbaTenantApiAccessInfo;
				}

				//if (!contextOptions.Authority.Contains(tenant.TenantName))
				//    contextOptions.Authority = contextOptions.Authority.Replace("http://", "http://" + tenant.TenantName + ".").Replace("https://", "https://" + tenant.TenantName + ".");
				contextOptions.ClientId = TenantConstant.GetClientIdByTenant(tenant);
				contextOptions.ClientSecret = TenantConstant.GetClientSecretByTenant(tenant);

				contextMessage.ClientId = TenantConstant.GetClientIdByTenant(tenant);
				contextMessage.ClientSecret = TenantConstant.GetClientSecretByTenant(tenant);
				contextMessage.AcrValues += string.IsNullOrEmpty(contextMessage.AcrValues) ? "tenant:" + tenant.TenantName : ",tenant:" + tenant.TenantName;
				if (!contextMessage.Parameters.Keys.Any(m => m.Equals(TenantConstant.ClaimTypes_TenantName, StringComparison.OrdinalIgnoreCase)))
					contextMessage.Parameters.Add(TenantConstant.ClaimTypes_TenantName, tenant.TenantName);
			}

			// if signing out, add the id_token_hint
			if (context != null && contextMessage.RequestType == OpenIdConnectRequestType.Logout)
			{
				var idTokenHint = context.User.FindFirst("id_token");
				if (idTokenHint != null)
				{
					contextMessage.IdTokenHint = idTokenHint.Value;
				}
			}

			Framework.Util.LogUtil.LogDebug(string.Format("{0} OpenIdConnect with message: {1} from request url:【{2}】 in authority: [{3}]", tenantContext?.Tenant?.TenantName, logMessage, context.Request?.Host.Value + context.Request?.Path.Value, contextOptions.Authority));
		}

		#endregion
		#endregion

		#region Configure Api Client with KC.IdentityServer4
		/// <summary>
		/// Configure Api Client with KC.IdentityServer4
		/// </summary>
		/// <param name="services"></param>
		/// <param name="authScope">Api provider scope</param>
		public static void ConfigureApiClientService(IServiceCollection services, IConfiguration configuration, string authScope, IEnumerable<AutoMapper.Profile> autoProfiles)
		{
			// 配置AspectCore动态代理
			services.ConfigureDynamicProxy();
			// 注入HttpClient对象
			services.AddHttpClient();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			//解决ViewBag的中文编码问题
			services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

			#region 静态类GlobalConfig的设置
			GlobalConfig.InitGlobalConfig(configuration);
			//SSO站点未配置文件站点，无需从接口获取后进行重新设置
			if (GlobalConfig.ApplicationGuid != ApplicationConstant.SsoAppId)
			{
				using (var scope = services.BuildServiceProvider().CreateScope())
				{
					var httpClient = scope.ServiceProvider.GetService<System.Net.Http.IHttpClientFactory>();
					var globalConfigData = new GlobalConfigApiService(httpClient).GetGlobalConfigData();
					GlobalConfig.InitGlobalConfigWithApiData(configuration, globalConfigData);

					var message = string.Format("GlobalConfig SystemType: {0}; EncryptKey: {1}; Resource WebDomain: {2}; SSO WebDomain: {3}; Current WebDomain: {4}; Get GlobalData from Resource is success? {5}",
				GlobalConfig.SystemType, GlobalConfig.EncryptKey, GlobalConfig.ResWebDomain, GlobalConfig.SSOWebDomain, GlobalConfig.CurrentApplication?.AppDomain, (globalConfigData != null));

					LogUtil.LogInfo(message);
					Service.Extension.ConsoleExtensions.ConsoleGreen(message);
				}
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

			#region AutoMapper对象注入

			var config = new AutoMapper.MapperConfiguration(cfg =>
			{
				cfg.AddProfiles(autoProfiles);
			});
			services.AddSingleton(config);
			var mapper = config.CreateMapper();
			services.AddSingleton(mapper);

			#endregion

			#region Service的注入

			KC.Service.Util.DependencyInjectUtil.InjectService(services);

			//集成Polly: https://www.cnblogs.com/runningsmallguo/p/9692001.html
			//https://docs.microsoft.com/zh-cn/dotnet/standard/microservices-architecture/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
			//services.AddHttpClient<ITenantUserApiService, TenantUserApiService>()
			//    .SetHandlerLifetime(TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut));
			//services.AddHttpClient<IAccountApiService, AccountApiService>()
			//    .SetHandlerLifetime(TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut));

			//Multi-tenancy设置
			if (authScope != ApplicationConstant.AdminScope)
				services.AddMultitenancy<Tenant, TenantResolver>();
			else
				services.AddMultitenancy<Tenant, DbaTenantResolver>();
			#endregion

			//#region 授权
			//services.TryAdd(ServiceDescriptor.Transient<IAuthorizationPolicyProvider, Extension.PermissionFilterPolicyProvider>());

			//services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, Extension.PermissionFilterModelProvider>());
			//#endregion

			//var optionsAccessor = services.BuildServiceProvider().GetService<IOptions<GlobalConfig>>();
			//var config = optionsAccessor.Value;
			var authUri = Web.Constants.OpenIdConnectConstants.GetAuthUrlByConfig(null);

			#region Api接口文档：Swagger设置
			//Github：https://github.com/domaindrivendev/Swashbuckle.AspNetCore
			//帮助文档：https://docs.microsoft.com/zh-cn/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-2.1
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("api", new OpenApiInfo
				{
					//Version = "v1",
					Title = "接口文档",
					Description = "RESTful API for User's Account",
					Contact = new OpenApiContact { Name = "Kent Tian", Email = "tianchangjun@outlook.com", Url = new Uri("http://www.kcloudy.com") }
				});

				//solve the problem: The same schemaId is already used for enum type
				//Github：https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1607#issuecomment-607170559
				options.CustomSchemaIds(type => type.ToString());

				//https://ctest.sso.kcloudy.com/connect/authorize?response_type=token&redirect_uri=http%3A%2F%2Flocalhost%3A39106%2Fswagger%2Fui%2Fo2c-html&realm=test-realm&client_id=api_test_api_flow&scope=all%20%20&state=oauth2
				options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.OAuth2,
					Flows = new OpenApiOAuthFlows
					{
						Implicit = new OpenApiOAuthFlow
						{
							AuthorizationUrl = new Uri(authUri + "/connect/authorize", UriKind.Absolute),
							TokenUrl = new Uri(authUri + "/connect/token", UriKind.Absolute),
							Scopes = new Dictionary<string, string>
							{
								{ authScope, "REST API - full access" }
							}
						}
					}
				});
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
						},
						new[] { authScope, "writeAccess" }
					}
				});

				//Set the comments path for the swagger json and ui.
				var xmlPath = Path.Combine(AppContext.BaseDirectory, "Xml/ApiSetting.xml");
				options.IncludeXmlComments(xmlPath);

				options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
				options.OperationFilter<Extension.SwaggerAuthorizeOperationFilter>(); // 添加httpHeader参数
			});
			#endregion

			#region 权限控制、跨越CORS设置
			services.AddCors(options =>
			{
				options.AddPolicy(MyAllowSpecificOrigins,
					builder => builder
						//.WithOrigins("")
						//.AllowCredentials()
						.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader()
				);
			});

			services
				.AddAuthentication(KC.Web.Constants.OpenIdConnectConstants.ApiAuthScheme)
				.AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme,
				options =>
				{
					options.Authority = authUri;
					options.ApiName = authScope;
					//options.ApiSecret = "secret";

					options.RequireHttpsMetadata = false;
					options.Events = new OpenIdConnectEvents
					{
						// 未授权时，重定向到OIDC服务器时触发
						OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,

						// 获取到授权码时触发
						OnAuthorizationCodeReceived = OnAuthorizationCodeReceived,
						// 接收到OIDC服务器返回的认证信息（包含Code, ID Token等）时触发
						OnMessageReceived = OnMessageReceived,
						// 接收到UserInfoEndpoint返回的信息时触发
						OnUserInformationReceived = OnUserInformationReceived,
						// 接收到Ticket返回的信息时触发
						OnTicketReceived = OnTicketReceived,
						// 接收到TokenEndpoint返回的信息时触发
						OnTokenResponseReceived = OnTokenResponseReceived,
						// 验证Token时触发
						OnTokenValidated = OnTokenValidated,

						// 出现异常时触发
						OnAuthenticationFailed = OnAuthenticationFailed,

						// 退出时，重定向到OIDC服务器时触发
						OnRedirectToIdentityProviderForSignOut = OnRedirectToIdentityProviderForSignOut,
						// OIDC服务器退出后，服务端回调时触发
						OnRemoteSignOut = OnRemoteSignOut,
						// OIDC服务器退出后，服务端失败时触发
						OnRemoteFailure = OnRemoteFailure,
						// OIDC服务器退出后，客户端重定向时触发
						OnSignedOutCallbackRedirect = OnSignedOutCallbackRedirect,
					};
				})
				;
			#endregion

			//方法三： 使用Provider的方式
			services.AddSingleton<IOptionsMonitor<CookieAuthenticationOptions>, MultiTenantCookieOptionsProvider>();
			services.AddSingleton<IOptionsMonitor<OpenIdConnectOptions>, MultiTenantOpenIdConnectOptionsProvider>();

			//services
			//    .AddControllers()
			//    .AddApiExplorer()
			//    .AddJsonFormatters()
			//    .AddAuthorization();

			#region 全局配置Json序列化处理
			services.AddControllersWithViews(config =>
				{
					config.Filters.Add<GlobalViewModelActionFilter>();
				})
				.AddNewtonsoftJson(options =>
				{
					//忽略循环引用
					options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
					// 设置为驼峰命名
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
					//设置时间格式
					//options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.SSSZ";
				});
			#endregion
		}

		public static void UseApiClientService(IApplicationBuilder app, bool isMultiTenant = true)
		{
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
			app.UseRouting();
			app.UseCors(MyAllowSpecificOrigins);

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/api/swagger.json", GlobalConfig.CurrentApplication?.AppName + "接口文档");

				//当RoutePrefix值为空时，即设置Swagger页面为首页
				options.RoutePrefix = string.Empty;  // Set Swagger UI at apps root
				options.OAuthAppName("api's Swagger Test");
				options.OAuthClientId(Constants.OpenIdConnectConstants.WebApiTestClient_ClientId);
				options.OAuthAdditionalQueryStringParams(
					new Dictionary<string, string> { { TenantConstant.ClaimTypes_TenantName, "cdba" } });

				//options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
			});

			//多租户访问
			if (isMultiTenant)
				app.UseMultitenancy<Tenant>();

			//认证、授权
			app.UseAuthentication();
			app.UseAuthorization();

			////https://stackoverflow.com/questions/49290683/how-to-redirect-root-to-swagger-in-asp-net-core-2-x
			//var option = new RewriteOptions();
			//option.AddRedirect("^$", "swagger");
			//app.UseRewriter(option);

			////异常处理
			//app.UseExceptionHandler(options =>
			//{
			//    options.Run(
			//    async context =>
			//    {
			//        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			//        context.Response.ContentType = "application/json;charset=utf-8"; //此处要加上utf-8编码

			//        //如果不加此句，服务器返回的数据到浏览器会拒绝
			//        context.Response.Headers["Access-Control-Allow-Origin"] = "*";

			//        var ex = context.Features.Get<IExceptionHandlerFeature>();
			//        if (ex != null)
			//        {
			//            var errObj = new
			//            {
			//                message = ex.Error.Message,
			//                stackTrace = ex.Error.StackTrace,
			//                exceptionType = ex.Error.GetType().Name
			//            };

			//            await context.Response.WriteAsync(JsonConvert.SerializeObject(errObj)).ConfigureAwait(false);
			//        }
			//    });
			//});

			//app.UseMvc();
			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
			});
		}

		#endregion

		/// <summary>
		/// to call CustomAuthenticationHandler
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="authenticationScheme"></param>
		/// <param name="configureOptions"></param>
		/// <returns></returns>
		public static AuthenticationBuilder AddIdentityServerAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<IdentityServerAuthenticationOptions> configureOptions)
		{
			builder.AddJwtBearer(authenticationScheme + CustomAuthenticationHandler.JwtAuthenticationScheme, configureOptions: null);
			builder.AddOAuth2Introspection(authenticationScheme + CustomAuthenticationHandler.IntrospectionAuthenticationScheme, configureOptions: null);

			builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(services =>
			{
				var monitor = services.GetRequiredService<IOptionsMonitor<IdentityServerAuthenticationOptions>>();
				return new ConfigureInternalOptions(monitor.Get(authenticationScheme), authenticationScheme);
			});

			builder.Services.AddSingleton<IConfigureOptions<OAuth2IntrospectionOptions>>(services =>
			{
				var monitor = services.GetRequiredService<IOptionsMonitor<IdentityServerAuthenticationOptions>>();
				return new ConfigureInternalOptions(monitor.Get(authenticationScheme), authenticationScheme);
			});

			return builder.AddScheme<IdentityServerAuthenticationOptions, CustomAuthenticationHandler>(authenticationScheme, configureOptions);
		}
	}

}
