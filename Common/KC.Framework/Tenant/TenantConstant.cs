
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using System;
using System.Collections.Generic;

namespace KC.Framework.Tenant
{
    /// <summary>
    /// 与租户相关的常量定义
    /// </summary>
    public class TenantConstant
    {
        #region 系统相关的常量

        public const CloudType DefaultCloudType = CloudType.Azure;
        public const TenantVersion DefaultVersion = TenantVersion.Standard | TenantVersion.Professional | TenantVersion.Customized;
        public const TenantType DefaultTenantType = TenantType.Enterprise | TenantType.Institution | TenantType.Bank;

        /// <summary>
        /// 域名替换字符串：subdomain
        /// </summary>
        public const string SubDomain = "subdomain";

        public const string ClaimTypes_TenantName = "tenantname";
        #endregion

        #region 租户设置

        /// <summary>
        /// 是否开通邮件服务：KC.Enums.Core.ConfigType.EmailConfig = 1
        /// </summary>
        public const string PropertyName_EmailSetting = "EnableEmail";
        /// <summary>
        /// 是否开通短信服务：KC.Enums.Core.ConfigType.SmsConfig = 2
        /// </summary>
        public const string PropertyName_SmsSetting = "EnableSms";
        /// <summary>
        /// 是否开通支付服务：KC.Enums.Core.ConfigType.PaymentMethod = 3
        /// </summary>
        public const string PropertyName_PaymentSetting = "EnablePayment";
        /// <summary>
        /// 是否开通身份认证服务：KC.Enums.Core.ConfigType.ID5 = 4
        /// </summary>
        public const string PropertyName_IDSetting = "EnableId5";
        /// <summary>
        /// 是否开通呼叫中心服务：KC.Enums.Core.ConfigType.CallConfig = 5
        /// </summary>
        public const string PropertyName_CallSetting = "EnableCallCenter";
        /// <summary>
        /// 是否开通物流服务：KC.Enums.Core.ConfigType.LogisticsPlatform = 6
        /// </summary>
        public const string PropertyName_LogisticsSetting = "EnableLogistics";
        /// <summary>
        /// 是否开通微信服务：KC.Enums.Core.ConfigType.WeixinConfig = 7
        /// </summary>
        public const string PropertyName_WeixinSetting = "EnableWeixin";
        /// <summary>
        /// 是否开通电子签章服务：KC.Enums.Core.ConfigType.ContractConfig = 8
        /// </summary>
        public const string PropertyName_ContractSetting = "EnableContract";
        /// <summary>
        /// 是否开通独立域名服务：KC.Enums.Core.ConfigType.OwnDomain = 9
        /// </summary>
        public const string PropertyName_OwnDomainSetting = "EnableOwnDomainName";

        public const string Default_EmailLimit = "10-200-500";
        public const string Default_SmsLimit = "5-50-500";
        public const string Default_PaymentLimit = "1000-5000-100000";
        public const string Default_IDLimit = "5-10-100";
        public const string Default_CallLimit = "10-30-120";
        public const string Default_LogisticsLimit = "10-100-500";
        public const string Default_ContractLimit = "5-20-100";

        #endregion

        #region 测试租户的定义：cDba、cTest、cBug、cSale
        #region Dba: 超级系统客户用例
        public const int DbaTenantId = 1;
        /// <summary>
        /// cDba: 后台管理
        /// </summary>
        public const string DbaTenantName = "cDba";
        public const string DbaTenantDisplayName = "租户管理";
        public static string DbaTenantSignature = new ServiceRequestToken(DbaTenantId, DbaTenantName, DefaultPrivateEncryptKey).GetEncrptSignature();
        /// <summary>
        /// Dba's signature to access Webapi
        /// </summary>
        public static Tenant DbaTenantApiAccessInfo
        {
            get
            {
                #region 默认连接字符串设置

                var mssqlConnItem = GetDatabaseConnectionItems();
                var server = mssqlConnItem.Item1;
                var database = mssqlConnItem.Item2;
                var user = mssqlConnItem.Item3;
                var password = mssqlConnItem.Item4;
                var port = mssqlConnItem.Item5;

                var mysqlConnItem = GetMySqlDatabaseConnectionItems();
                var mysqlServer = mysqlConnItem.Item1;
                var mysqlDatabase = mysqlConnItem.Item2;
                var mysqlUser = mysqlConnItem.Item3;
                var mysqlPassword = mysqlConnItem.Item4;
                var mysqlPort = mysqlConnItem.Item5;

                var storageItem = GetStorageConnectionItems();
                var endpont = storageItem.Item1;
                var accessName = storageItem.Item2;
                var accessKey = storageItem.Item3;

                var queueItem = GetQueueConnectionItems();
                var queueEndpont = queueItem.Item1;
                var queueName = queueItem.Item2;
                var queueKey = queueItem.Item3;

                var nosqlItem = GetNoSqlConnectionItems();
                var noSqlEndpont = nosqlItem.Item1;
                var noSqlName = nosqlItem.Item2;
                var noSqlKey = nosqlItem.Item3;

                var sblItem = GetServiceBusConnectionItems();
                var serviceBusEndpont = sblItem.Item1;
                var serviceBusName = sblItem.Item2;
                var serviceBusKey = sblItem.Item3;

                var vodItem = GetVodConnectionItems();
                var vodEndpont = vodItem.Item1;
                var vodName = vodItem.Item2;
                var vodKey = vodItem.Item3;

                var codeItem = GetCodeConnectionItems();
                var codeEndpont = codeItem.Item1;
                var codeName = codeItem.Item2;
                var codeKey = codeItem.Item3;
                #endregion

                return new Tenant()
                {
                    TenantId = DbaTenantId,
                    TenantName = DbaTenantName,
                    TenantDisplayName = DbaTenantDisplayName,
                    TenantSignature = DbaTenantSignature,
                    PrivateEncryptKey = DefaultPrivateEncryptKey,
                    DatabaseType = DefaultDatabaseType,
                    Server = DefaultDatabaseType == DatabaseType.SqlServer 
                        ? server 
                        : mysqlServer,
                    Database = DefaultDatabaseType == DatabaseType.SqlServer 
                        ? database 
                        : mysqlDatabase,
                    DatabasePasswordHash = DefaultDatabaseType == DatabaseType.SqlServer 
                        ? password 
                        : mysqlPassword,
                    StorageType = DefaultStorageType,
                    StorageEndpoint = endpont,
                    StorageAccessName = accessName,
                    StorageAccessKeyPasswordHash = accessKey,
                    QueueType = DefaultQueueType,
                    QueueEndpoint = queueEndpont,
                    QueueAccessName = queueName,
                    QueueAccessKeyPasswordHash = queueKey,
                    NoSqlType = DefaultNoSqlType,
                    NoSqlEndpoint = noSqlEndpont,
                    NoSqlAccessName = noSqlName,
                    NoSqlAccessKeyPasswordHash = noSqlKey,
                    ServiceBusType = DefaultServiceBusType,
                    ServiceBusEndpoint = serviceBusEndpont,
                    ServiceBusAccessName = serviceBusName,
                    ServiceBusAccessKeyPasswordHash = serviceBusKey,
                    VodEndpoint = vodEndpont,
                    VodAccessName = vodName,
                    VodAccessKeyPasswordHash = vodKey,
                    CodeEndpoint = codeEndpont,
                    CodeAccessName = codeName,
                    CodeAccessKeyPasswordHash = codeKey,
                    ContactName = "田长军",
                    ContactEmail = "tianchangjun@outlook.com",
                    ContactPhone = "17744949695",
                    CloudType = DefaultCloudType,
                    TenantType = DefaultTenantType,
                    Version = DefaultVersion,
                    //Hostnames = dbaHosts.ToArray()
                };
            }
        }
        #endregion

        #region cTest: 测试租户-核心企业
        public const int TestTenantId = 2;
        /// <summary>
        /// cTest: 测试用户
        /// </summary>
        public const string TestTenantName = "cTest";
        public const string TestTenantDisplayName = "测试租户-核心企业";
        public static string TestTenantSignature = new ServiceRequestToken(TestTenantId, TestTenantName, DefaultPrivateEncryptKey).GetEncrptSignature();
        /// <summary>
        /// cTest's signature to access Webapi
        /// </summary>
        public static Tenant TestTenantApiAccessInfo
        {
            get
            {
                #region 默认连接字符串设置

                var mssqlConnItem = GetDatabaseConnectionItems();
                var server = mssqlConnItem.Item1;
                var database = mssqlConnItem.Item2;
                var user = mssqlConnItem.Item3;
                var password = mssqlConnItem.Item4;
                var port = mssqlConnItem.Item5;

                var mysqlConnItem = GetMySqlDatabaseConnectionItems();
                var mysqlServer = mysqlConnItem.Item1;
                var mysqlDatabase = mysqlConnItem.Item2;
                var mysqlUser = mysqlConnItem.Item3;
                var mysqlPassword = mysqlConnItem.Item4;
                var mysqlPort = mysqlConnItem.Item5;

                var storageItem = GetStorageConnectionItems();
                var endpont = storageItem.Item1;
                var accessName = storageItem.Item2;
                var accessKey = storageItem.Item3;

                var queueItem = GetQueueConnectionItems();
                var queueEndpont = queueItem.Item1;
                var queueName = queueItem.Item2;
                var queueKey = queueItem.Item3;

                var nosqlItem = GetNoSqlConnectionItems();
                var noSqlEndpont = nosqlItem.Item1;
                var noSqlName = nosqlItem.Item2;
                var noSqlKey = nosqlItem.Item3;

                var sblItem = GetServiceBusConnectionItems();
                var serviceBusEndpont = sblItem.Item1;
                var serviceBusName = sblItem.Item2;
                var serviceBusKey = sblItem.Item3;

                var vodItem = GetVodConnectionItems();
                var vodEndpont = vodItem.Item1;
                var vodName = vodItem.Item2;
                var vodKey = vodItem.Item3;

                var codeItem = GetCodeConnectionItems();
                var codeEndpont = codeItem.Item1;
                var codeName = codeItem.Item2;
                var codeKey = codeItem.Item3;
                #endregion

                return new Tenant()
                {
                    TenantId = TestTenantId,
                    TenantName = TestTenantName,
                    TenantDisplayName = TestTenantDisplayName,
                    TenantSignature = TestTenantSignature,
                    PrivateEncryptKey = DefaultPrivateEncryptKey,
                    DatabaseType = DefaultDatabaseType,
                    Server = DefaultDatabaseType == DatabaseType.SqlServer
                        ? server
                        : mysqlServer,
                    Database = DefaultDatabaseType == DatabaseType.SqlServer
                        ? database
                        : mysqlDatabase,
                    DatabasePasswordHash = DefaultDatabaseType == DatabaseType.SqlServer
                        ? password
                        : mysqlPassword,
                    StorageType = DefaultStorageType,
                    StorageEndpoint = endpont,
                    StorageAccessName = accessName,
                    StorageAccessKeyPasswordHash = accessKey,
                    QueueType = DefaultQueueType,
                    QueueEndpoint = queueEndpont,
                    QueueAccessName = queueName,
                    QueueAccessKeyPasswordHash = queueKey,
                    NoSqlType = DefaultNoSqlType,
                    NoSqlEndpoint = noSqlEndpont,
                    NoSqlAccessName = noSqlName,
                    NoSqlAccessKeyPasswordHash = noSqlKey,
                    ServiceBusType = DefaultServiceBusType,
                    ServiceBusEndpoint = serviceBusEndpont,
                    ServiceBusAccessName = serviceBusName,
                    ServiceBusAccessKeyPasswordHash = serviceBusKey,
                    VodEndpoint = vodEndpont,
                    VodAccessName = vodName,
                    VodAccessKeyPasswordHash = vodKey,
                    CodeEndpoint = codeEndpont,
                    CodeAccessName = codeName,
                    CodeAccessKeyPasswordHash = codeKey,
                    ContactName = "田长军",
                    ContactEmail = "tianchangjun@outlook.com",
                    ContactPhone = "17744949695",
                    CloudType = DefaultCloudType,
                    TenantType = DefaultTenantType,
                    Version = DefaultVersion,
                    //Hostnames = GetTenantHosts(TestTenantName)
                };
            }
        }
        #endregion

        #region cBuy: 测试租户-客户
        public const int BuyTenantId = 3;
        /// <summary>
        /// cBuy: 测试租户-客户
        /// </summary>
        public const string BuyTenantName = "cBuy";
        public const string BuyTenantDisplayName = "测试租户-客户";
        public static string BuyTenantSignature = new ServiceRequestToken(BuyTenantId, BuyTenantName, DefaultPrivateEncryptKey).GetEncrptSignature();

        /// <summary>
        /// Buy's signature to access Webapi
        /// </summary>
        public static Tenant BuyTenantApiAccessInfo
        {
            get
            {
                #region 默认连接字符串设置

                var mssqlConnItem = GetDatabaseConnectionItems();
                var server = mssqlConnItem.Item1;
                var database = mssqlConnItem.Item2;
                var user = mssqlConnItem.Item3;
                var password = mssqlConnItem.Item4;
                var port = mssqlConnItem.Item5;

                var mysqlConnItem = GetMySqlDatabaseConnectionItems();
                var mysqlServer = mysqlConnItem.Item1;
                var mysqlDatabase = mysqlConnItem.Item2;
                var mysqlUser = mysqlConnItem.Item3;
                var mysqlPassword = mysqlConnItem.Item4;
                var mysqlPort = mysqlConnItem.Item5;

                var storageItem = GetStorageConnectionItems();
                var endpont = storageItem.Item1;
                var accessName = storageItem.Item2;
                var accessKey = storageItem.Item3;

                var queueItem = GetQueueConnectionItems();
                var queueEndpont = queueItem.Item1;
                var queueName = queueItem.Item2;
                var queueKey = queueItem.Item3;

                var nosqlItem = GetNoSqlConnectionItems();
                var noSqlEndpont = nosqlItem.Item1;
                var noSqlName = nosqlItem.Item2;
                var noSqlKey = nosqlItem.Item3;

                var sblItem = GetServiceBusConnectionItems();
                var serviceBusEndpont = sblItem.Item1;
                var serviceBusName = sblItem.Item2;
                var serviceBusKey = sblItem.Item3;

                var vodItem = GetVodConnectionItems();
                var vodEndpont = vodItem.Item1;
                var vodName = vodItem.Item2;
                var vodKey = vodItem.Item3;

                var codeItem = GetCodeConnectionItems();
                var codeEndpont = codeItem.Item1;
                var codeName = codeItem.Item2;
                var codeKey = codeItem.Item3;
                #endregion

                return new Tenant()
                {
                    TenantId = BuyTenantId,
                    TenantName = BuyTenantName,
                    TenantDisplayName = BuyTenantDisplayName,
                    TenantSignature = BuyTenantSignature,
                    PrivateEncryptKey = DefaultPrivateEncryptKey,
                    DatabaseType = DefaultDatabaseType,
                    Server = DefaultDatabaseType == DatabaseType.SqlServer
                        ? server
                        : mysqlServer,
                    Database = DefaultDatabaseType == DatabaseType.SqlServer
                        ? database
                        : mysqlDatabase,
                    DatabasePasswordHash = DefaultDatabaseType == DatabaseType.SqlServer
                        ? password
                        : mysqlPassword,
                    StorageType = DefaultStorageType,
                    StorageEndpoint = endpont,
                    StorageAccessName = accessName,
                    StorageAccessKeyPasswordHash = accessKey,
                    QueueType = DefaultQueueType,
                    QueueEndpoint = queueEndpont,
                    QueueAccessName = queueName,
                    QueueAccessKeyPasswordHash = queueKey,
                    NoSqlType = DefaultNoSqlType,
                    NoSqlEndpoint = noSqlEndpont,
                    NoSqlAccessName = noSqlName,
                    NoSqlAccessKeyPasswordHash = noSqlKey,
                    ServiceBusType = DefaultServiceBusType,
                    ServiceBusEndpoint = serviceBusEndpont,
                    ServiceBusAccessName = serviceBusName,
                    ServiceBusAccessKeyPasswordHash = serviceBusKey,
                    VodEndpoint = vodEndpont,
                    VodAccessName = vodName,
                    VodAccessKeyPasswordHash = vodKey,
                    CodeEndpoint = codeEndpont,
                    CodeAccessName = codeName,
                    CodeAccessKeyPasswordHash = codeKey,
                    ContactName = "田长军",
                    ContactEmail = "tianchangjun@outlook.com",
                    ContactPhone = "17744949695",
                    CloudType = DefaultCloudType,
                    TenantType = DefaultTenantType,
                    Version = DefaultVersion,
                    //Hostnames = GetTenantHosts(BuyTenantName)
                };
            }
        }
        #endregion

        #region cSale: 测试租户-供应商
        public const int SaleTenantId = 4;
        /// <summary>
        ///  cSale: 测试租户-供应商
        /// </summary>
        public const string SaleTenantName = "cSale";
        public const string SaleTenantDisplayName = "测试租户-供应商";
        public static string SaleTenantSignature = new ServiceRequestToken(SaleTenantId, SaleTenantName, DefaultPrivateEncryptKey).GetEncrptSignature();

        /// <summary>
        /// Buy's signature to access Webapi
        /// </summary>
        public static Tenant SaleTenantApiAccessInfo
        {
            get
            {
                #region 默认连接字符串设置

                var mssqlConnItem = GetDatabaseConnectionItems();
                var server = mssqlConnItem.Item1;
                var database = mssqlConnItem.Item2;
                var user = mssqlConnItem.Item3;
                var password = mssqlConnItem.Item4;
                var port = mssqlConnItem.Item5;

                var mysqlConnItem = GetMySqlDatabaseConnectionItems();
                var mysqlServer = mysqlConnItem.Item1;
                var mysqlDatabase = mysqlConnItem.Item2;
                var mysqlUser = mysqlConnItem.Item3;
                var mysqlPassword = mysqlConnItem.Item4;
                var mysqlPort = mysqlConnItem.Item5;

                var storageItem = GetStorageConnectionItems();
                var endpont = storageItem.Item1;
                var accessName = storageItem.Item2;
                var accessKey = storageItem.Item3;

                var queueItem = GetQueueConnectionItems();
                var queueEndpont = queueItem.Item1;
                var queueName = queueItem.Item2;
                var queueKey = queueItem.Item3;

                var nosqlItem = GetNoSqlConnectionItems();
                var noSqlEndpont = nosqlItem.Item1;
                var noSqlName = nosqlItem.Item2;
                var noSqlKey = nosqlItem.Item3;

                var sblItem = GetServiceBusConnectionItems();
                var serviceBusEndpont = sblItem.Item1;
                var serviceBusName = sblItem.Item2;
                var serviceBusKey = sblItem.Item3;

                var vodItem = GetVodConnectionItems();
                var vodEndpont = vodItem.Item1;
                var vodName = vodItem.Item2;
                var vodKey = vodItem.Item3;

                var codeItem = GetCodeConnectionItems();
                var codeEndpont = codeItem.Item1;
                var codeName = codeItem.Item2;
                var codeKey = codeItem.Item3;
                #endregion

                return new Tenant()
                {
                    TenantId = SaleTenantId,
                    TenantName = SaleTenantName,
                    TenantDisplayName = SaleTenantDisplayName,
                    TenantSignature = SaleTenantSignature,
                    PrivateEncryptKey = DefaultPrivateEncryptKey,
                    DatabaseType = DefaultDatabaseType,
                    Server = DefaultDatabaseType == DatabaseType.SqlServer
                        ? server
                        : mysqlServer,
                    Database = DefaultDatabaseType == DatabaseType.SqlServer
                        ? database
                        : mysqlDatabase,
                    DatabasePasswordHash = DefaultDatabaseType == DatabaseType.SqlServer
                        ? password
                        : mysqlPassword,
                    StorageType = DefaultStorageType,
                    StorageEndpoint = endpont,
                    StorageAccessName = accessName,
                    StorageAccessKeyPasswordHash = accessKey,
                    QueueType = DefaultQueueType,
                    QueueEndpoint = queueEndpont,
                    QueueAccessName = queueName,
                    QueueAccessKeyPasswordHash = queueKey,
                    NoSqlType = DefaultNoSqlType,
                    NoSqlEndpoint = noSqlEndpont,
                    NoSqlAccessName = noSqlName,
                    NoSqlAccessKeyPasswordHash = noSqlKey,
                    ServiceBusType = DefaultServiceBusType,
                    ServiceBusEndpoint = serviceBusEndpont,
                    ServiceBusAccessName = serviceBusName,
                    ServiceBusAccessKeyPasswordHash = serviceBusKey,
                    VodEndpoint = vodEndpont,
                    VodAccessName = vodName,
                    VodAccessKeyPasswordHash = vodKey,
                    CodeEndpoint = codeEndpont,
                    CodeAccessName = codeName,
                    CodeAccessKeyPasswordHash = codeKey,
                    ContactName = "田长军",
                    ContactEmail = "tianchangjun@outlook.com",
                    ContactPhone = "17744949695",
                    CloudType = DefaultCloudType,
                    TenantType = DefaultTenantType,
                    Version = DefaultVersion,
                    //Hostnames = GetTenantHosts(SaleTenantName)
                };
            }
        }
        #endregion
        #endregion

        #region 获取配置的链接字符串，形如：Tuple<Server, DataBase, ConnectionPasswordHash>

        #region 获取测试租户的数据库连接字符串
        public const DatabaseType DefaultDatabaseType = DatabaseType.SqlServer;
        public static string DefaultPrivateEncryptKey
        {
            get
            {
                return string.IsNullOrEmpty(GlobalConfig.EncryptKey)
                       ? EncryptPasswordUtil.Key
                       : GlobalConfig.EncryptKey;
            }
        }
        public static string DefaultDatabaseConnectionString
        {
            get
            {
                if (DefaultDatabaseType == DatabaseType.MySql)
                {
                    return string.IsNullOrEmpty(GlobalConfig.GetDecryptMySqlConnectionString())
                        ? "Server=localhost;Database=MSSqlKCContext;User ID=sa;Password=P@ssw0rd;MultipleActiveResultSets=true;"
                        : GlobalConfig.GetDecryptMySqlConnectionString();
                } else {
                    return string.IsNullOrEmpty(GlobalConfig.GetDecryptDatabaseConnectionString())
                        ? "Server=localhost;Database=MSSqlKCContext;User ID=sa;Password=P@ssw0rd;MultipleActiveResultSets=true;"
                        : GlobalConfig.GetDecryptDatabaseConnectionString();
                }
            }
        }

        /// <summary>
        /// 获取数据库服务器的连接信息：Tuple<server, database, password, port>
        /// </summary>
        /// <param name="isEncrypPassword">是否将数据库登录密码进行加密处理：秘钥为配置文件中的EncryptKey</param>
        /// <returns></returns>
        public static Tuple<string, string, string, string, string> GetDatabaseConnectionItems(bool isEncrypPassword = true)
        {
            var conntionString = DefaultDatabaseConnectionString;
            if (string.IsNullOrEmpty(conntionString))
                return new Tuple<string, string, string, string, string>(null, null, null, null, null);

            var dicCon = conntionString.KeyValuePairFromConnectionString();
            var server = string.Empty;
            if (dicCon.ContainsKey(ConnectionKeyConstant.DatabaseEndpoint))
            {
                server = dicCon[ConnectionKeyConstant.DatabaseEndpoint];
            }
            var database = string.Empty;
            if (dicCon.ContainsKey(ConnectionKeyConstant.DatabaseName))
            {
                database = dicCon[ConnectionKeyConstant.DatabaseName];
            }
            var userId = string.Empty;
            if (dicCon.ContainsKey(ConnectionKeyConstant.DatabaseUserID))
            {
                userId = dicCon[ConnectionKeyConstant.DatabaseUserID];
            }
            var password = string.Empty;
            if (dicCon.ContainsKey(ConnectionKeyConstant.DatabasePassword))
            {
                password = dicCon[ConnectionKeyConstant.DatabasePassword];
            }
            var port = string.Empty;
            if (dicCon.ContainsKey(ConnectionKeyConstant.DatabasePort))
            {
                port = dicCon[ConnectionKeyConstant.DatabasePort];
            }

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database) || string.IsNullOrEmpty(password))
                return new Tuple<string, string, string, string, string>(null, null, null, null, null);

            var passwordHash = isEncrypPassword ?
                SecurityHelper.EncryptPasswordUtil.EncryptPassword(password, DefaultPrivateEncryptKey)
                : password;
            return new Tuple<string, string, string, string, string>(server, database, userId, passwordHash, port);
        }
        #endregion

        #region 获取测试租户的MySql数据库连接字符串
        public static string MySqlPrivateEncryptKey
        {
            get
            {
                return string.IsNullOrEmpty(GlobalConfig.EncryptKey)
                       ? EncryptPasswordUtil.Key
                       : GlobalConfig.EncryptKey;
            }
        }
        public static string MySqlDatabaseConnectionString
        {
            get
            {
                return string.IsNullOrEmpty(GlobalConfig.GetDecryptMySqlConnectionString())
                    ? "Server=localhost;Database=MSSqlKCContext;User ID=sa;Password=P@ssw0rd;Port=3306;MultipleActiveResultSets=true;"
                    : GlobalConfig.GetDecryptMySqlConnectionString();
            }
        }

        /// <summary>
        /// 获取数据库服务器的连接信息：Tuple<server, database, password, port>
        /// </summary>
        /// <param name="isEncrypPassword">是否将数据库登录密码进行加密处理：秘钥为配置文件中的EncryptKey</param>
        /// <returns></returns>
        public static Tuple<string, string, string, string, string> GetMySqlDatabaseConnectionItems(bool isEncrypPassword = true)
        {
            var conntionString = MySqlDatabaseConnectionString;
            if (string.IsNullOrEmpty(conntionString))
                return new Tuple<string, string, string, string, string>(null, null, null, null, null);

            var dicCon = conntionString.KeyValuePairFromConnectionString();
            var server = string.Empty;
            if (dicCon.ContainsKey(ConnectionKeyConstant.DatabaseEndpoint))
            {
                server = dicCon[ConnectionKeyConstant.DatabaseEndpoint];
            }
            var database = string.Empty;
            if (dicCon.ContainsKey(ConnectionKeyConstant.DatabaseName))
            {
                database = dicCon[ConnectionKeyConstant.DatabaseName];
            }
            var userId = string.Empty;
            if (dicCon.ContainsKey(ConnectionKeyConstant.DatabaseUserID))
            {
                userId = dicCon[ConnectionKeyConstant.DatabaseUserID];
            }
            var password = string.Empty;
            if (dicCon.ContainsKey(ConnectionKeyConstant.DatabasePassword))
            {
                password = dicCon[ConnectionKeyConstant.DatabasePassword];
            }
            var port = string.Empty;
            if (dicCon.ContainsKey(ConnectionKeyConstant.DatabasePort))
            {
                port = dicCon[ConnectionKeyConstant.DatabasePort];
            }

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database) || string.IsNullOrEmpty(password))
                return new Tuple<string, string, string, string, string>(null, null, null, null, null);

            var passwordHash = isEncrypPassword ?
                SecurityHelper.EncryptPasswordUtil.EncryptPassword(password, DefaultPrivateEncryptKey)
                : password;
            return new Tuple<string, string, string, string, string>(server, database, userId, passwordHash, port);
        }
        #endregion

        #region 获取测试租户的存储连接字符串
        public const StorageType DefaultStorageType = StorageType.AliOSS;
/// <summary>
        /// 获取存储服务器的连接信息：Tuple<endpont, accessName, accessKey>
        /// </summary>
        /// <param name="isEncrypPassword">是否将存储服务器的访问秘钥进行加密处理：加密Key为配置文件中的EncryptKey</param>
        /// <returns></returns>
        public static Tuple<string, string, string> GetStorageConnectionItems(bool isEncrypPassword = true)
        {
            var dataConnString = string.Empty;
            if (!string.IsNullOrEmpty(GlobalConfig.GetDecryptStorageConnectionString()))
                dataConnString = GlobalConfig.GetDecryptStorageConnectionString();

            if (string.IsNullOrEmpty(dataConnString))
                return new Tuple<string, string, string>(null, null, null);

            var keyValues = dataConnString.KeyValuePairFromConnectionString();
            var endpont = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.BlobEndpoint))
                endpont = keyValues[ConnectionKeyConstant.BlobEndpoint];
            var accessName = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.AccessName))
                accessName = keyValues[ConnectionKeyConstant.AccessName];
            var accessKey = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.AccessKey))
                accessKey = keyValues[ConnectionKeyConstant.AccessKey];

            if (string.IsNullOrEmpty(endpont) || string.IsNullOrEmpty(accessName) || string.IsNullOrEmpty(accessKey))
                return new Tuple<string, string, string>(null, null, null);

            var encryptKey = isEncrypPassword
                ? SecurityHelper.EncryptPasswordUtil.EncryptPassword(accessKey, DefaultPrivateEncryptKey)
                : accessKey;

            return new Tuple<string, string, string>(endpont, accessName, encryptKey);
        }
        #endregion

        #region 获取测试租户的队列连接字符串
        public const QueueType DefaultQueueType = QueueType.Redis;
        /// <summary>
        /// 获取队列服务器的连接信息：Tuple<endpont, accessName, accessKey>
        /// </summary>
        /// <param name="isEncrypPassword">是否将队列服务器的访问秘钥进行加密处理：加密Key为配置文件中的EncryptKey</param>
        /// <returns></returns>
        public static Tuple<string, string, string> GetQueueConnectionItems(bool isEncrypPassword = true)
        {
            var dataConnString = string.Empty;
            if (!string.IsNullOrEmpty(GlobalConfig.GetDecryptQueueConnectionString()))
                dataConnString = GlobalConfig.GetDecryptQueueConnectionString();

            if (string.IsNullOrEmpty(dataConnString))
                return new Tuple<string, string, string>(null, null, null);

            var keyValues = dataConnString.KeyValuePairFromConnectionString();
            var endpont = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.QueueEndpoint))
                endpont = keyValues[ConnectionKeyConstant.QueueEndpoint];
            var accessName = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.AccessName))
                accessName = keyValues[ConnectionKeyConstant.AccessName];
            var accessKey = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.AccessKey))
                accessKey = keyValues[ConnectionKeyConstant.AccessKey];

            if (string.IsNullOrEmpty(endpont) || string.IsNullOrEmpty(accessName) || string.IsNullOrEmpty(accessKey))
                return new Tuple<string, string, string>(null, null, null);

            var encryptKey = isEncrypPassword
                ? SecurityHelper.EncryptPasswordUtil.EncryptPassword(accessKey, DefaultPrivateEncryptKey)
                : accessKey;

            return new Tuple<string, string, string>(endpont, accessName, encryptKey);
        }
        #endregion

        #region 获取测试租户的noSql连接字符串
        public const NoSqlType DefaultNoSqlType = NoSqlType.Redis;
        /// <summary>
        /// 获取NoSql服务器的连接信息：Tuple<endpont, accessName, accessKey>
        /// </summary>
        /// <param name="isEncrypPassword">是否将NoSql服务器的访问秘钥进行加密处理：加密Key为配置文件中的EncryptKey</param>
        /// <returns></returns>
        public static Tuple<string, string, string> GetNoSqlConnectionItems(bool isEncrypPassword = true)
        {
            var dataConnString = string.Empty;
            if (!string.IsNullOrEmpty(GlobalConfig.GetDecryptNoSqlConnectionString()))
                dataConnString = GlobalConfig.GetDecryptNoSqlConnectionString();

            if (string.IsNullOrEmpty(dataConnString))
                return new Tuple<string, string, string>(null, null, null);

            var keyValues = dataConnString.KeyValuePairFromConnectionString();
            var endpont = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.NoSqlEndpoint))
                endpont = keyValues[ConnectionKeyConstant.NoSqlEndpoint];
            var accessName = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.AccessName))
                accessName = keyValues[ConnectionKeyConstant.AccessName];
            var accessKey = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.AccessKey))
                accessKey = keyValues[ConnectionKeyConstant.AccessKey];

            if (string.IsNullOrEmpty(endpont) || string.IsNullOrEmpty(accessName) || string.IsNullOrEmpty(accessKey))
                return new Tuple<string, string, string>(null, null, null);

            var encryptKey = isEncrypPassword
                ? SecurityHelper.EncryptPasswordUtil.EncryptPassword(accessKey, DefaultPrivateEncryptKey)
                : accessKey;

            return new Tuple<string, string, string>(endpont, accessName, encryptKey);
        }
        #endregion

        #region 获取测试租户的Servicebus连接字符串
        public const ServiceBusType DefaultServiceBusType = ServiceBusType.Redis;
/// <summary>
        /// 获取分布式服务服务器的连接信息：Tuple<endpont, accessName, accessKey>
        /// </summary>
        /// <param name="isEncrypPassword">是否将分布式服务服务器的访问秘钥进行加密处理：加密Key为配置文件中的EncryptKey</param>
        /// <returns></returns>
        public static Tuple<string, string, string> GetServiceBusConnectionItems(bool isEncrypPassword = true)
        {
            var dataConnString = string.Empty;
            if (!string.IsNullOrEmpty(GlobalConfig.GetDecryptServiceBusConnectionString()))
                dataConnString = GlobalConfig.GetDecryptServiceBusConnectionString();

            if (string.IsNullOrEmpty(dataConnString))
                return new Tuple<string, string, string>(null, null, null);

            var keyValues = dataConnString.KeyValuePairFromConnectionString();
            var endpont = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.ServiceBusEndpoint))
                endpont = keyValues[ConnectionKeyConstant.ServiceBusEndpoint];
            var accessName = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.ServiceBusAccessName))
                accessName = keyValues[ConnectionKeyConstant.ServiceBusAccessName];
            var accessKey = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.ServiceBusAccessKey))
                accessKey = keyValues[ConnectionKeyConstant.ServiceBusAccessKey];

            if (string.IsNullOrEmpty(endpont) || string.IsNullOrEmpty(accessName) || string.IsNullOrEmpty(accessKey))
                return new Tuple<string, string, string>(null, null, null);

            var encryptKey = isEncrypPassword
                ? SecurityHelper.EncryptPasswordUtil.EncryptPassword(accessKey, DefaultPrivateEncryptKey)
                : accessKey;

            return new Tuple<string, string, string>(endpont, accessName, encryptKey);
        }
        #endregion

        #region 获取测试租户的Vod连接字符串
        public const VodType DefaultVodType = VodType.Aliyun;
        /// <summary>
        /// 获取分布式服务服务器的连接信息：Tuple<endpont, accessName, accessKey>
        /// </summary>
        /// <param name="isEncrypPassword">是否将分布式服务服务器的访问秘钥进行加密处理：加密Key为配置文件中的EncryptKey</param>
        /// <returns></returns>
        public static Tuple<string, string, string> GetVodConnectionItems(bool isEncrypPassword = true)
        {
            var dataConnString = string.Empty;
            if (!string.IsNullOrEmpty(GlobalConfig.GetDecryptVodConnectionString()))
                dataConnString = GlobalConfig.GetDecryptVodConnectionString();

            if (string.IsNullOrEmpty(dataConnString))
                return new Tuple<string, string, string>(null, null, null);

            var keyValues = dataConnString.KeyValuePairFromConnectionString();
            var endpont = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.VodEndpoint))
                endpont = keyValues[ConnectionKeyConstant.VodEndpoint];
            var accessName = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.AccessName))
                accessName = keyValues[ConnectionKeyConstant.AccessName];
            var accessKey = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.AccessKey))
                accessKey = keyValues[ConnectionKeyConstant.AccessKey];

            if (string.IsNullOrEmpty(endpont) || string.IsNullOrEmpty(accessName) || string.IsNullOrEmpty(accessKey))
                return new Tuple<string, string, string>(null, null, null);

            var encryptKey = isEncrypPassword
                ? SecurityHelper.EncryptPasswordUtil.EncryptPassword(accessKey, DefaultPrivateEncryptKey)
                : accessKey;

            return new Tuple<string, string, string>(endpont, accessName, encryptKey);
        }
        #endregion

        #region 获取测试租户的代码仓库连接字符串
        public const CodeType DefaultCodeType = CodeType.Gitlab;
        /// <summary>
        /// 获取代码仓库的连接信息：Tuple<endpont, accessName, accessKey>
        /// </summary>
        /// <param name="isEncrypPassword">是否将代码仓库的访问秘钥进行加密处理：加密Key为配置文件中的EncryptKey</param>
        /// <returns></returns>
        public static Tuple<string, string, string> GetCodeConnectionItems(bool isEncrypPassword = true)
        {
            var dataConnString = string.Empty;
            if (!string.IsNullOrEmpty(GlobalConfig.GetDecryptCodeConnectionString()))
                dataConnString = GlobalConfig.GetDecryptCodeConnectionString();

            if (string.IsNullOrEmpty(dataConnString))
                return new Tuple<string, string, string>(null, null, null);

            var keyValues = dataConnString.KeyValuePairFromConnectionString();
            var endpont = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.CodeEndpoint))
                endpont = keyValues[ConnectionKeyConstant.CodeEndpoint];
            var accessName = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.AccessName))
                accessName = keyValues[ConnectionKeyConstant.AccessName];
            var accessKey = string.Empty;
            if (keyValues.ContainsKey(ConnectionKeyConstant.AccessKey))
                accessKey = keyValues[ConnectionKeyConstant.AccessKey];

            if (string.IsNullOrEmpty(endpont) || string.IsNullOrEmpty(accessName) || string.IsNullOrEmpty(accessKey))
                return new Tuple<string, string, string>(null, null, null);

            var encryptKey = isEncrypPassword
                ? SecurityHelper.EncryptPasswordUtil.EncryptPassword(accessKey, DefaultPrivateEncryptKey)
                : accessKey;

            return new Tuple<string, string, string>(endpont, accessName, encryptKey);
        }
        #endregion
        #endregion

        public static string GetClientIdByTenant(Tenant tenant)
        {
            if (string.IsNullOrEmpty(tenant.TenantName))
                return string.Empty;

            return Base64Provider.EncodeString(tenant.TenantName);
        }
        public static string GetClientSecretByTenant(Tenant tenant)
        {
            if (string.IsNullOrEmpty(tenant.TenantName))
                return string.Empty;

            var key = tenant.TenantName.ToLower() + ":" + tenant.PrivateEncryptKey;
            var md5Key = MD5Provider.Hash(key);

            return Base64Provider.EncodeString(md5Key);

            //return Sha256(tenant.TenantName.ToLower() + ":" + tenant.PrivateEncryptKey);
        }

        public static string GetClientIdByTenantName(string tenantName)
        {
            if (string.IsNullOrEmpty(tenantName))
                return string.Empty;

            return Base64Provider.EncodeString(tenantName);
        }

        public static string GetClientSecretByTenantNameAndKey(string tenantName, string privateEncryptKey)
        {
            if (string.IsNullOrEmpty(tenantName))
                return string.Empty;

            var key = tenantName.ToLower() + ":" + privateEncryptKey;
            var md5Key = MD5Provider.Hash(key);

            return Base64Provider.EncodeString(md5Key);
        }

        /// <summary>
        /// Creates a SHA256 hash of the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A hash</returns>
        public static string Sha256(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Creates a SHA512 hash of the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A hash</returns>
        public static string Sha512(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (var sha = System.Security.Cryptography.SHA512.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

        public static string GetEncodeClientId(string encodeClientId)
        {
            if (string.IsNullOrEmpty(encodeClientId))
                return string.Empty;

            return Base64Provider.EncodeString(encodeClientId);
        }

        public static string GetDecodeClientId(string encodeClientId)
        {
            if (string.IsNullOrEmpty(encodeClientId))
                return string.Empty;
            try
            {
                var clientId = Base64Provider.DecodeString(encodeClientId);
                return clientId;
            }
            catch
            {
                return encodeClientId;
            }
        }
    }
}