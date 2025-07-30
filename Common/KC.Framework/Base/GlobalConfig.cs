using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Framework.Util;
using Microsoft.Extensions.Configuration;

namespace KC.Framework.Base
{
    public class GlobalConfig
    {
        #region AppSetting configuration

        public static string ApplicationId { get; set; }
        public static Guid ApplicationGuid { get; set; }
        public static string ApplicationName { get; set; }
        public static string ApplicationWebSiteName { get; set; }
        public static string EncryptKey { get; set; }
        public static string BlobStorage { get; set; }
        public static string AdminEmails { get; set; }
        public static string TempFilePath { get; set; }

        public static string DatabaseConnectionString { get; set; }
        public static string MySqlConnectionString { get; set; }
        public static string StorageConnectionString { get; set; }
        public static string QueueConnectionString { get; set; }
        public static string NoSqlConnectionString { get; set; }
        public static string RedisConnectionString { get; set; }
        public static string ServiceBusConnectionString { get; set; }
        public static string VodConnectionString { get; set; }
        public static string CodeConnectionString { get; set; }
        /// <summary>
        /// 获取解密后的SqlServer连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDecryptDatabaseConnectionString()
        {
            if (string.IsNullOrEmpty(DatabaseConnectionString))
                return string.Empty;
            try
            {
                var keyValues = DatabaseConnectionString.KeyValuePairFromConnectionString();
                var endpoint = keyValues[ConnectionKeyConstant.DatabaseEndpoint];
                var name = keyValues[ConnectionKeyConstant.DatabaseName];
                var user = keyValues[ConnectionKeyConstant.DatabaseUserID];
                var pwd = keyValues[ConnectionKeyConstant.DatabasePassword];

                if (string.IsNullOrWhiteSpace(endpoint)
                    || string.IsNullOrWhiteSpace(name)
                    || string.IsNullOrWhiteSpace(user)
                    || string.IsNullOrWhiteSpace(pwd))
                    return string.Empty;

                var azureConn = @"Server={0};Database={1};User ID={2};Password={3};MultipleActiveResultSets=true;";
                return string.Format(azureConn, endpoint, name, user, EncryptPasswordUtil.DecryptPassword(pwd, EncryptKey));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取解密后的MySql连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDecryptMySqlConnectionString()
        {
            if (string.IsNullOrEmpty(MySqlConnectionString))
                return string.Empty;
            try
            {
                var keyValues = MySqlConnectionString.KeyValuePairFromConnectionString();
                var endpoint = keyValues[ConnectionKeyConstant.DatabaseEndpoint];
                var name = keyValues[ConnectionKeyConstant.DatabaseName];
                var user = keyValues[ConnectionKeyConstant.DatabaseUserID];
                var pwd = keyValues[ConnectionKeyConstant.DatabasePassword];
                var port = keyValues[ConnectionKeyConstant.DatabasePort];

                if (string.IsNullOrWhiteSpace(endpoint)
                    || string.IsNullOrWhiteSpace(name)
                    || string.IsNullOrWhiteSpace(user)
                    || string.IsNullOrWhiteSpace(pwd))
                    return string.Empty;

                var azureConn = @"Server={0};Database={1};Uid={2};Pwd={3};Port={4};CharSet=utf8;sslMode=None;AllowUserVariables=True;";
                return string.Format(azureConn, endpoint, name, user, EncryptPasswordUtil.DecryptPassword(pwd, EncryptKey), port);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取解密后的存储连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDecryptStorageConnectionString()
        {
            if (string.IsNullOrEmpty(StorageConnectionString))
                return string.Empty;
            try
            {
                var keyValues = StorageConnectionString.KeyValuePairFromConnectionString();
                var endpoint = keyValues[ConnectionKeyConstant.BlobEndpoint];
                var name = keyValues[ConnectionKeyConstant.AccessName];
                var key = keyValues[ConnectionKeyConstant.AccessKey];

                if (string.IsNullOrWhiteSpace(endpoint)
                    || string.IsNullOrWhiteSpace(name)
                    || string.IsNullOrWhiteSpace(key))
                    return string.Empty;

                var azureConn = @"DefaultEndpointsProtocol=https;BlobEndpoint={0};AccountName={1};AccountKey={2}";
                return string.Format(azureConn, endpoint, name, EncryptPasswordUtil.DecryptPassword(key, EncryptKey));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取解密后的队列连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDecryptQueueConnectionString()
        {
            if (string.IsNullOrEmpty(QueueConnectionString))
                return string.Empty;
            try
            {
                var keyValues = QueueConnectionString.KeyValuePairFromConnectionString();
                var endpoint = keyValues[ConnectionKeyConstant.QueueEndpoint];
                var name = keyValues[ConnectionKeyConstant.AccessName];
                var key = keyValues[ConnectionKeyConstant.AccessKey];

                if (string.IsNullOrWhiteSpace(endpoint)
                    || string.IsNullOrWhiteSpace(name)
                    || string.IsNullOrWhiteSpace(key))
                    return string.Empty;

                var azureConn = @"DefaultEndpointsProtocol=https;QueueEndpoint={0};AccountName={1};AccountKey={2}";
                return string.Format(azureConn, endpoint, name, EncryptPasswordUtil.DecryptPassword(key, EncryptKey));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取解密后的NoSql连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDecryptNoSqlConnectionString()
        {
            if (string.IsNullOrEmpty(NoSqlConnectionString))
                return string.Empty;
            try
            {
                var keyValues = NoSqlConnectionString.KeyValuePairFromConnectionString();
                var endpoint = keyValues[ConnectionKeyConstant.NoSqlEndpoint];
                var name = keyValues[ConnectionKeyConstant.AccessName];
                var key = keyValues[ConnectionKeyConstant.AccessKey];

                if (string.IsNullOrWhiteSpace(endpoint)
                    || string.IsNullOrWhiteSpace(name)
                    || string.IsNullOrWhiteSpace(key))
                    return string.Empty;

                var azureConn = @"DefaultEndpointsProtocol=https;TableEndpoint={0};AccountName={1};AccountKey={2}";
                return string.Format(azureConn, endpoint, name, EncryptPasswordUtil.DecryptPassword(key, EncryptKey));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取解密后的服务总线连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDecryptServiceBusConnectionString()
        {
            if (string.IsNullOrEmpty(ServiceBusConnectionString))
                return string.Empty;
            try
            {
                var keyValues = ServiceBusConnectionString.KeyValuePairFromConnectionString();
                var endpoint = keyValues[ConnectionKeyConstant.ServiceBusEndpoint];
                var name = keyValues[ConnectionKeyConstant.ServiceBusAccessName];
                var key = keyValues[ConnectionKeyConstant.ServiceBusAccessKey];

                if (string.IsNullOrWhiteSpace(endpoint)
                    || string.IsNullOrWhiteSpace(name)
                    || string.IsNullOrWhiteSpace(key))
                    return string.Empty;

                var azureConn = @"Endpoint={0};SharedAccessKeyName={1};SharedAccessKey={2}";
                return string.Format(azureConn, endpoint, name, EncryptPasswordUtil.DecryptPassword(key, EncryptKey));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取解密后的VOD连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDecryptVodConnectionString()
        {
            if (string.IsNullOrEmpty(VodConnectionString))
                return string.Empty;
            try
            {
                var keyValues = VodConnectionString.KeyValuePairFromConnectionString();
                var endpoint = keyValues[ConnectionKeyConstant.VodEndpoint];
                var name = keyValues[ConnectionKeyConstant.AccessName];
                var key = keyValues[ConnectionKeyConstant.AccessKey];

                if (string.IsNullOrWhiteSpace(endpoint)
                    || string.IsNullOrWhiteSpace(name)
                    || string.IsNullOrWhiteSpace(key))
                    return string.Empty;

                var azureConn = @"VodEndpoint={0};AccountName={1};AccountKey={2}";
                return string.Format(azureConn, endpoint, name, EncryptPasswordUtil.DecryptPassword(key, EncryptKey));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取解密后的代码仓库连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDecryptCodeConnectionString()
        {
            if (string.IsNullOrEmpty(CodeConnectionString))
                return string.Empty;
            try
            {
                var keyValues = CodeConnectionString.KeyValuePairFromConnectionString();
                var endpoint = keyValues[ConnectionKeyConstant.CodeEndpoint];
                var name = keyValues[ConnectionKeyConstant.AccessName];
                var key = keyValues[ConnectionKeyConstant.AccessKey];

                if (string.IsNullOrWhiteSpace(endpoint)
                    || string.IsNullOrWhiteSpace(name)
                    || string.IsNullOrWhiteSpace(key))
                    return string.Empty;

                var azureConn = @"CodeEndpoint={0};AccountName={1};AccountKey={2}";
                return string.Format(azureConn, endpoint, name, EncryptPasswordUtil.DecryptPassword(key, EncryptKey));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取解密后的Redis连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDecryptRedisConnectionString()
        {
            if (string.IsNullOrEmpty(RedisConnectionString))
                return string.Empty;
            try
            {
                var keyValues = RedisConnectionString.KeyValuePairFromConnectionString();
                var endpoint = keyValues[ConnectionKeyConstant.RedisEndpoint];
                var name = keyValues[ConnectionKeyConstant.AccessName];
                var key = keyValues[ConnectionKeyConstant.AccessKey];

                if (string.IsNullOrWhiteSpace(endpoint))
                    return string.Empty;

                var azureConn = @"RedisEndpoint={0};AccountName={1};AccountKey={2}";
                if (string.IsNullOrWhiteSpace(key)) 
                    return string.Format(azureConn, endpoint, name, string.Empty);
                
                return string.Format(azureConn, endpoint, name, EncryptPasswordUtil.DecryptPassword(key, EncryptKey));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
        public static string ClientId { get; set; }
        public static string ClientSecret { get; set; }

        public static string WeixinAppKey { get; set; }
        public static string WeixinAppSecret { get; set; }
        public static string WeixinAppToken { get; set; }

        public static string ZZZLicenseName { get; set; }
        public static string ZZZLicenseKey { get; set; }

        #endregion

        #region Domain
        public static List<ApplicationInfo> Applications { get; set; }
        public static ApplicationInfo CurrentApplication { get; set; }

        /// <summary>
        /// subdomain的sso地址：http://sso.kcloudy.com/  </br>
        ///     本地测试接口地址：http://localhost:1001/
        /// </summary>
        public static string SSOWebDomain { get; set; }
        /// <summary>
        /// subdomain的admin地址：http://admin.kcloudy.com/ </br>
        ///     本地测试接口地址：http://localhost:1003
        /// </summary>
        public static string AdminWebDomain { get; set; }
        /// <summary>
        /// subdomain的Blog地址：http://blog.kcloudy.com/ </br>
        ///     本地测试接口地址：http://localhost:1005
        /// </summary>
        public static string BlogWebDomain { get; set; }
        /// <summary>
        /// subdomain的lowcode地址：http://code.kcloudy.com/ </br>
        ///     本地测试接口地址：http://localhost:1007
        /// </summary>
        public static string CodeWebDomain { get; set; }

        /// <summary>
        /// subdomain的config地址：http://subdomain.cfg.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:1101/
        /// </summary>
        public static string CfgWebDomain { get; set; }
        /// <summary>
        /// subdomain的dictionary地址：http://subdomain.dic.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:1103/
        /// </summary>
        public static string DicWebDomain { get; set; }
        /// <summary>
        /// subdomain的app地址：http://subdomain.app.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:1105
        /// </summary>
        public static string AppWebDomain { get; set; }
        /// <summary>
        /// subdomain的message地址：http://subdomain.msg.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:1107/
        /// </summary>
        public static string MsgWebDomain { get; set; }

        /// <summary>
        /// subdomain的account地址：http://subdomain.acc.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:2001/
        /// </summary>
        public static string AccWebDomain { get; set; }
        /// <summary>
        /// subdomain的contract地址：http://subdomain.econ.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:2003/
        /// </summary>
        public static string EconWebDomain { get; set; }
        /// <summary>
        /// subdomain的document地址：http://subdomain.doc.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:2005/
        /// </summary>
        public static string DocWebDomain { get; set; }
        /// <summary>
        /// subdomain的Hr地址：http://subdomain.hr.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:2007/
        /// </summary>
        public static string HrWebDomain { get; set; }

        /// <summary>
        /// subdomain的crm地址：http://subdomain.crm.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:3001/
        /// </summary>
        public static string CrmWebDomain { get; set; }

        /// <summary>
        /// subdomain的srm地址：http://subdomain.srm.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:3003/
        /// </summary>
        public static string SrmWebDomain { get; set; }

        /// <summary>
        /// subdomain的prd地址：http://subdomain.prd.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:3005/
        /// </summary>
        public static string PrdWebDomain { get; set; }

        /// <summary>
        /// subdomain的pmc地址：http://subdomain.pmc.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:3007/
        /// </summary>
        public static string PmcWebDomain { get; set; }

        /// <summary>
        /// subdomain的电商地址：http://subdomain.portal.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:4001/
        /// </summary>
        public static string PortalWebDomain { get; set; }
        /// <summary>
        /// subdomain的som地址：http://subdomain.som.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:4003/
        /// </summary>
        public static string SomWebDomain { get; set; }

        /// <summary>
        /// subdomain的pom地址：http://subdomain.pom.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:4005/
        /// </summary>
        public static string PomWebDomain { get; set; }

        /// <summary>
        /// subdomain的仓储地址：http://subdomain.wms.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:4007/
        /// </summary>
        public static string WmsWebDomain { get; set; }

        /// <summary>
        /// subdomain的资金地址：http://subdomain.jr.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:5001/
        /// </summary>
        public static string JRWebDomain { get; set; }

        /// <summary>
        /// subdomain的项目管理地址：http://subdomain.prj.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:5003/
        /// </summary>
        public static string PrjWebDomain { get; set; }

        /// <summary>
        /// subdomain的会员管理地址：http://subdomain.mbr.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:5005/
        /// </summary>
        public static string MbrWebDomain { get; set; }

        /// <summary>
        /// subdomain的培训管理地址：http://subdomain.trn.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:6001/
        /// </summary>
        public static string TrainWebDomain { get; set; }

        /// <summary>
        /// subdomain的exam地址：http://subdomain.exam.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:6003/
        /// </summary>
        public static string ExamWebDomain { get; set; }

        /// <summary>
        /// subdomain的工作流地址：http://subdomain.flow.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:7001/
        /// </summary>
        public static string WorkflowWebDomain { get; set; }
        /// <summary>
        /// subdomain的sso地址：http://subdomain.pay.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:8001/
        /// </summary>
        public static string PayWebDomain { get; set; }
        /// <summary>
        /// subdomain的微信地址：http://subdomain.wx.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:9001/
        /// </summary>
        public static string WXWebDomain { get; set; }
        /// <summary>
        /// subdomain的资源地址：http://resource.kcloudy.com/ </br>
        ///     本地测试接口地址：http://localhost:9999/
        /// </summary>
        public static string ResWebDomain { get; set; }

        /// <summary>
        /// subdomain的接口地址，无api/后缀：http://subdomain.api.kcloudy.com/
        /// </summary>
        public static string ApiWebDomain { get; set; }

        /// <summary>
        /// 获取webDomain的租户域名地址：http://[tenantName].[webDomain].kcloudy.com/ </br>
        ///     本地测试地址：http://[tenantName].localhost:1002/
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public static string GetTenantWebDomain(string webDomain, string tenantName)
        {
            if (string.IsNullOrEmpty(webDomain))
                return null;

            var tenantWebDomain = webDomain.Replace(TenantConstant.SubDomain, tenantName);
            var busName = tenantWebDomain.GetBusNameByHost();
            var level2Domains = new List<string>() {
                "http://localhost:1001/",
                "http://sso.kcloudy.com/",
                "http://ssotest.kcloudy.com/",
                "http://ssobeta.kcloudy.com/",
                "http://ssodemo.kcloudy.com/"
            };
            var isLocal = tenantWebDomain.Contains("localhost", StringComparison.OrdinalIgnoreCase);
            if (level2Domains.Contains(webDomain))
            {
                return isLocal
                    ? tenantWebDomain.Replace("localhost", tenantName + ".localhost")
                    : tenantWebDomain.Replace(busName + ".", tenantName + "." + busName + ".");
            }

            return tenantWebDomain;
        }

        /// <summary>
        /// 获取webDomain的租户域名地址：http://[tenantName].[webapi].kcloudy.com/ </br>
        ///     本地测试地址：http://[tenantName].localhost:1002/
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public static string GetTenantWebApiDomain(string webDomain, string tenantName)
        {
            if (string.IsNullOrEmpty(webDomain))
                return null;
            var tenantWebDomain = webDomain.Replace(TenantConstant.SubDomain, tenantName);
            var busName = tenantWebDomain.GetBusNameByHost();
            var level2Domains = new List<string>() {
                "http://localhost:1001/",
                "http://sso.kcloudy.com/",
                "http://ssotest.kcloudy.com/",
                "http://ssobeta.kcloudy.com/",
                "http://ssodemo.kcloudy.com/"
            };
            var isLocal = tenantWebDomain.Contains("localhost", StringComparison.OrdinalIgnoreCase);
            if (level2Domains.Contains(webDomain))
            {
                tenantWebDomain = isLocal
                    ? tenantWebDomain.Replace("localhost", tenantName + ".localhost")
                    : tenantWebDomain.Replace(busName + ".", tenantName + "." + busName + ".");
            }
            
            if (IsDevelopment || isLocal)
            {
                if (busName.IsNumber())
                {
                    var apiPort = (busName.ObjToInt() + 1).ToString();
                    return tenantWebDomain.Replace(busName, apiPort).TrimEndSlash() + "/api/";
                }
            }
            return tenantWebDomain.Replace(busName, busName + "api").TrimEndSlash() + "/api/";
        }
        #endregion

        #region SSO

        private static StoreName? _certStoreName;
        public static StoreName CertStoreName
        {
            get
            {
                if (_certStoreName == null)
                    _certStoreName = (StoreName)Enum.Parse(typeof(StoreName), "My");
                return (StoreName)_certStoreName;
            }
            set { _certStoreName = value; }
        }

        private static StoreLocation? _certLocation;
        public static StoreLocation CertLocation
        {
            get
            {
                if (_certLocation == null)
                    _certLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), "LocalMachine");
                return (StoreLocation)_certLocation;
            }
            set { _certLocation = value; }
        }

        public static string CertThumbprint { get; set; }

        #endregion

        #region Upload config


        /// <summary>
        /// 上传配置
        /// </summary>
        public static UploadConfig UploadConfig { get; set; }

        #endregion

        /// <summary>
        /// subdomain的接口地址：http://subdomain.api.kcloudy.com/api/
        /// </summary>
        public static string ApiServerUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(ApiWebDomain))
                {
                    return ApiWebDomain + "api/";
                }
                return null;
            }
        }

        /// <summary>
        /// 系统环境类型：Dev、Test、Beta、Production
        /// </summary>
        public static SystemType SystemType { get; set; }

        public static bool IsDevelopment
        {
            get
            {
                return SystemType == SystemType.Dev;
            }
        }

        public static void InitGlobalConfig(IConfiguration configuration)
        {
            #region Basic Settings
            var config = configuration.GetSection("AppSettings");
            ApplicationId = config["ApplicationId"];
            if (!String.IsNullOrEmpty(ApplicationId))
            {
                Guid appid;
                var issccess = Guid.TryParse(ApplicationId, out appid);
                if (issccess)
                    ApplicationGuid = appid;
            }
            ApplicationName = config["ApplicationName"];
            ApplicationWebSiteName = config["ApplicationWebSiteName"];

            EncryptKey = config["EncryptKey"];
            BlobStorage = config["BlobStorage"];
            AdminEmails = config["AdminEmails"];
            TempFilePath = config["TempFilePath"];

            DatabaseConnectionString = config["DatabaseConnectionString"];
            MySqlConnectionString = config["MySqlConnectionString"];
            StorageConnectionString = config["StorageConnectionString"];
            QueueConnectionString = config["QueueConnectionString"];
            NoSqlConnectionString = config["NoSqlConnectionString"];
            RedisConnectionString = config["RedisConnectionString"];
            ServiceBusConnectionString = config["ServiceBusConnectionString"];
            VodConnectionString = config["VodConnectionString"];
            CodeConnectionString = config["CodeConnectionString"];

            SSOWebDomain = config["SSOWebDomain"].EndWithSlash();
            ResWebDomain = config["ResWebDomain"].EndWithSlash();

            ClientId = config["ClientId"];
            ClientSecret = config["ClientSecret"];

            WeixinAppKey = config["WeixinAppKey"];
            WeixinAppSecret = config["WeixinAppSecret"];
            WeixinAppToken = config["WeixinAppToken"];

            ZZZLicenseName = config["ZZZLicenseName"];
            ZZZLicenseKey = config["ZZZLicenseKey"];

            CertThumbprint = config["ZZZLicenseKey"];
            #endregion

            #region SystemType
            var webDomain = SSOWebDomain?.ToLower();
            if (webDomain == null) {
                SystemType = SystemType.Product;
            } else {
                if (webDomain.Contains("localhost")
                    || webDomain.Contains("127.0.0.1")
                    || webDomain.Contains("localsso.")
                    || webDomain.Contains("devsso."))
                {
                    SystemType = SystemType.Dev;
                }
                else if (webDomain.Contains("testsso."))
                {
                    SystemType = SystemType.Test;
                }
                else if (webDomain.Contains("betasso."))
                {
                    SystemType = SystemType.Beta;
                }
                else
                {
                    SystemType = SystemType.Product;
                }
            }
            #endregion

            InitGlobalConfigWithApiData(configuration, null);
        }

        public static void InitGlobalConfigWithApiData(IConfiguration configuration, GlobalConfigData data)
        {
            #region Basic Settings
            var config = configuration.GetSection("AppSettings");

            EncryptKey = config["EncryptKey"] ?? (data?.EncryptKey);
            BlobStorage = config["BlobStorage"] ?? (data?.BlobStorage);
            AdminEmails = config["AdminEmails"] ?? (data?.AdminEmails);
            TempFilePath = config["TempFilePath"] ?? (data?.TempFilePath);

            DatabaseConnectionString = config["DatabaseConnectionString"] ?? (data?.DatabaseConnectionString);
            MySqlConnectionString = config["MySqlConnectionString"] ?? (data?.MySqlConnectionString);
            StorageConnectionString = config["StorageConnectionString"] ?? (data?.StorageConnectionString);
            QueueConnectionString = config["QueueConnectionString"] ?? (data?.QueueConnectionString);
            NoSqlConnectionString = config["NoSqlConnectionString"] ?? (data?.NoSqlConnectionString);
            RedisConnectionString = config["RedisConnectionString"] ?? (data?.RedisConnectionString);
            ServiceBusConnectionString = config["ServiceBusConnectionString"] ?? (data?.ServiceBusConnectionString);
            VodConnectionString = config["VodConnectionString"] ?? (data?.VodConnectionString);
            CodeConnectionString = config["CodeConnectionString"] ?? (data?.CodeConnectionString);
            #endregion

            #region Domain

            SSOWebDomain = (config["SSOWebDomain"] ?? (data?.SSOWebDomain)).EndWithSlash();
            AdminWebDomain = (config["AdminWebDomain"] ?? (data?.AdminWebDomain)).EndWithSlash();
            BlogWebDomain = (config["BlogWebDomain"] ?? (data?.BlogWebDomain)).EndWithSlash();
            CodeWebDomain = (config["CodeWebDomain"] ?? (data?.CodeWebDomain)).EndWithSlash();

            AppWebDomain = (config["AppWebDomain"] ?? (data?.AppWebDomain)).EndWithSlash();
            CfgWebDomain = (config["CfgWebDomain"] ?? (data?.CfgWebDomain)).EndWithSlash();
            DicWebDomain = (config["DicWebDomain"] ?? (data?.DicWebDomain)).EndWithSlash();
            MsgWebDomain = (config["MsgWebDomain"] ?? (data?.MsgWebDomain)).EndWithSlash();

            AccWebDomain = (config["AccWebDomain"] ?? (data?.AccWebDomain)).EndWithSlash();
            EconWebDomain = (config["EconWebDomain"] ?? (data?.EconWebDomain)).EndWithSlash();
            DocWebDomain = (config["DocWebDomain"] ?? (data?.DocWebDomain)).EndWithSlash();
            HrWebDomain = (config["HrWebDomain"] ?? (data?.HrWebDomain)).EndWithSlash();

            CrmWebDomain = (config["CrmWebDomain"] ?? (data?.CrmWebDomain)).EndWithSlash();
            SrmWebDomain = (config["SrmWebDomain"] ?? (data?.SrmWebDomain)).EndWithSlash();
            PrdWebDomain = (config["PrdWebDomain"] ?? (data?.PrdWebDomain)).EndWithSlash();
            PmcWebDomain = (config["PmcWebDomain"] ?? (data?.PmcWebDomain)).EndWithSlash();

            PortalWebDomain = (config["PortalWebDomain"] ?? (data?.PortalWebDomain)).EndWithSlash();
            SomWebDomain = (config["SomWebDomain"] ?? (data?.SomWebDomain)).EndWithSlash();
            PomWebDomain = (config["PomWebDomain"] ?? (data?.PomWebDomain)).EndWithSlash();
            WmsWebDomain = (config["WmsWebDomain"] ?? (data?.WmsWebDomain)).EndWithSlash();

            JRWebDomain = (config["JRWebDomain"] ?? (data?.JRWebDomain)).EndWithSlash();
            PrjWebDomain = (config["PrjWebDomain"] ?? (data?.PrjWebDomain)).EndWithSlash();
            MbrWebDomain = (config["MbrWebDomain"] ?? (data?.MbrWebDomain)).EndWithSlash();
            
            TrainWebDomain = (config["TrainWebDomain"] ?? (data?.TrainWebDomain)).EndWithSlash();
            ExamWebDomain = (config["ExamWebDomain"] ?? (data?.ExamWebDomain)).EndWithSlash();

            WorkflowWebDomain = (config["FlowWebDomain"] ?? (data?.FlowWebDomain)).EndWithSlash();
            PayWebDomain = (config["PayWebDomain"] ?? (data?.PayWebDomain)).EndWithSlash();

            WXWebDomain = (config["WXWebDomain"] ?? (data?.WXWebDomain)).EndWithSlash();
            ResWebDomain = (config["ResWebDomain"] ?? (data?.ResWebDomain)).EndWithSlash();
            ApiWebDomain = (config["ApiWebDomain"] ?? (data?.ApiWebDomain)).EndWithSlash();

            Applications = null != data && null != data.Applications && data.Applications.Any() 
                ? data.Applications 
                : ApplicationConstant.GetAllApplications();

            CurrentApplication = Applications.FirstOrDefault(m => m.AppId == ApplicationGuid);
            #endregion

            #region UploadConfig
            var uploadConfig = configuration.GetSection("UploadConfig");
            var imageMaxSizeString = uploadConfig["ImageMaxSize"];
            var imageMaxSize = 10;
            if (!string.IsNullOrEmpty(imageMaxSizeString))
            {
                int size;
                var issccess = int.TryParse(imageMaxSizeString, out size);
                if (issccess)
                    imageMaxSize = size;
            }
            else if (data != null && data.UploadConfig != null)
            {
                imageMaxSize = data.UploadConfig.ImageMaxSize;
            }

            var fileMaxSizeString = uploadConfig["FileMaxSize"];
            var fileMaxSize = 10;
            if (!string.IsNullOrEmpty(fileMaxSizeString))
            {
                int size;
                var issccess = int.TryParse(fileMaxSizeString, out size);
                if (issccess)
                    fileMaxSize = size;
            }
            else if (data != null && data.UploadConfig != null)
            {
                fileMaxSize = data.UploadConfig.FileMaxSize;
            }

            UploadConfig = new UploadConfig()
            {
                ImageMaxSize = imageMaxSize,
                ImageExt = uploadConfig["ImageExt"] ?? data?.UploadConfig?.ImageExt,
                FileMaxSize = fileMaxSize,
                FileExt = uploadConfig["FileExt"] ?? data?.UploadConfig?.FileExt
            };

            #endregion
        }
    }

    public enum SystemType
    {
        Dev = 0,
        Test = 1,
        Beta = 2,
        Product = 3,
    }

    public class UploadConfig
    {
        public int ImageMaxSize { get; set; }

        public int FileMaxSize { get; set; }

        public string ImageExt { get; set; }

        public string FileExt { get; set; }
    }

    public class ApplicationInfo
    {
        public ApplicationInfo(Guid id, string code, string name, string domain, string scope, int index)
        {
            AppId = id;
            AppCode = code;
            AppName = name;
            AppDomain = domain;
            AppScope = scope;
            Index = index;
        }
        public Guid AppId { get; set; }

        public string AppCode { get; set; }

        public string AppName { get; set; }

        public string AppDomain { get; set; }

        public string AppScope { get; set; }
        public int Index { get; set; }
    }

    public class GlobalConfigData
    {
        #region AppSetting configuration
        public string EncryptKey { get; set; }
        public string BlobStorage { get; set; }
        public string AdminEmails { get; set; }
        public string TempFilePath { get; set; }

        public string DatabaseConnectionString { get; set; }
        public string MySqlConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
        public string QueueConnectionString { get; set; }
        public string NoSqlConnectionString { get; set; }
        public string RedisConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public string VodConnectionString { get; set; }
        public string CodeConnectionString { get; set; }
        #endregion

        #region Domain
        public List<ApplicationInfo> Applications { get; set; }

        /// <summary>
        /// subdomain的sso地址：http://sso.kcloudy.com/
        ///     本地测试接口地址：http://localhost:1001/
        /// </summary>
        public string SSOWebDomain { get; set; }
        /// <summary>
        /// subdomain的admin地址：http://admin.kcloudy.com/
        ///     本地测试接口地址：http://localhost:1003
        /// </summary>
        public string AdminWebDomain { get; set; }
        /// <summary>
        /// subdomain的blog地址：http://blog.kcloudy.com/
        ///     本地测试接口地址：http://localhost:1005
        /// </summary>
        public string BlogWebDomain { get; set; }
        /// <summary>
        /// subdomain的blog地址：http://subdomain.code.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:1007
        /// </summary>
        public string CodeWebDomain { get; set; }

        /// <summary>
        /// subdomain的config地址：http://subdomain.cfg.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:1101/
        /// </summary>
        public string CfgWebDomain { get; set; }
        /// <summary>
        /// subdomain的dictionary地址：http://subdomain.dic.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:1103/
        /// </summary>
        public string DicWebDomain { get; set; }
        /// <summary>
        /// subdomain的app地址：http://subdomain.app.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:1105
        /// </summary>
        public string AppWebDomain { get; set; }

        /// <summary>
        /// subdomain的dictionary地址：http://subdomain.msg.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:1109/
        /// </summary>
        public string MsgWebDomain { get; set; }

        /// <summary>
        /// subdomain的account地址：http://subdomain.acc.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:2001/
        /// </summary>
        public string AccWebDomain { get; set; }

        /// <summary>
        /// subdomain的account地址：http://subdomain.econ.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:2003/
        /// </summary>
        public string EconWebDomain { get; set; }
        /// <summary>
        /// subdomain的document地址：http://subdomain.doc.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:2005/
        /// </summary>
        public string DocWebDomain { get; set; }
        /// <summary>
        /// subdomain的Hr地址：http://subdomain.hr.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:2007/
        /// </summary>
        public string HrWebDomain { get; set; }

        /// <summary>
        /// subdomain的crm地址：http://subdomain.crm.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:3001/
        /// </summary>
        public string CrmWebDomain { get; set; }

        /// <summary>
        /// subdomain的crm地址：http://subdomain.srm.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:3003/
        /// </summary>
        public string SrmWebDomain { get; set; }

        /// <summary>
        /// subdomain的prd地址：http://subdomain.prd.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:3005/
        /// </summary>
        public string PrdWebDomain { get; set; }

        /// <summary>
        /// subdomain的pmc地址：http://subdomain.pmc.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:3007/
        /// </summary>
        public string PmcWebDomain { get; set; }

        /// <summary>
        /// subdomain的电商地址：http://subdomain.portal.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:4001/
        /// </summary>
        public string PortalWebDomain { get; set; }
        /// <summary>
        /// subdomain的som地址：http://subdomain.som.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:4003/
        /// </summary>
        public string SomWebDomain { get; set; }

        /// <summary>
        /// subdomain的pom地址：http://subdomain.pom.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:4005/
        /// </summary>
        public string PomWebDomain { get; set; }

        /// <summary>
        /// subdomain的wms地址：http://subdomain.wms.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:4007/
        /// </summary>
        public string WmsWebDomain { get; set; }

        /// <summary>
        /// subdomain的融资地址：http://subdomain.market.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:5001/
        /// </summary>
        public string JRWebDomain { get; set; }

        /// <summary>
        /// subdomain的项目地址：http://subdomain.Prj.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:5001/
        /// </summary>
        public string PrjWebDomain { get; set; }

        /// <summary>
        /// subdomain的会员管理地址：http://subdomain.mbr.kcloudy.com/ </br>
        ///     本地测试接口地址：http://subdomain.localhost:5005/
        /// </summary>
        public string MbrWebDomain { get; set; }

        /// <summary>
        /// subdomain的train地址：http://subdomain.trn.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:6001/
        /// </summary>
        public string TrainWebDomain { get; set; }

        /// <summary>
        /// subdomain的exam地址：http://subdomain.exam.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:6003/
        /// </summary>
        public string ExamWebDomain { get; set; }

        /// <summary>
        /// subdomain的工作流地址：http://subdomain.flow.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:7001/
        /// </summary>
        public string FlowWebDomain { get; set; }
        /// <summary>
        /// subdomain的sso地址：http://subdomain.pay.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:8001/
        /// </summary>
        public string PayWebDomain { get; set; }

        /// <summary>
        /// subdomain的微信地址：http://subdomain.wx.kcloudy.com/
        ///     本地测试接口地址：http://subdomain.localhost:9001/
        /// </summary>
        public string WXWebDomain { get; set; }

        /// <summary>
        /// subdomain的资源地址：http://resource.kcloudy.com/
        ///     本地测试接口地址：http://localhost:9999/
        /// </summary>
        public string ResWebDomain { get; set; }
        #endregion

        #region Api Url

        /// <summary>
        /// subdomain的接口地址，无api/后缀：http://subdomain.api.kcloudy.com/
        /// </summary>
        public string ApiWebDomain { get; set; }

        /// <summary>
        /// subdomain的接口地址：http://subdomain.api.kcloudy.com/api/
        /// </summary>
        public string ApiServerUrl { get; set; }

        #endregion

        #region Upload config


        /// <summary>
        /// 上传配置
        /// </summary>
        public UploadConfig UploadConfig { get; set; }

        #endregion


    }
}
