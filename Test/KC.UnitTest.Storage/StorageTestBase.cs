using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KC.Framework.Tenant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace KC.UnitTest.Storage
{
    public abstract class StorageTestBase : CommonTestBase
    {
        protected IServiceProvider ServiceProvider;

        private ILogger _logger;
        public StorageTestBase(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(StorageTestBase));
        }

        protected override void SetUp()
        {
            base.SetUp();

            KC.Service.Component.DependencyInjectUtil.InjectService(Services);

            ServiceProvider = Services.BuildServiceProvider();

            #region Init Dba's Tenant  SqlServer、FTP、MSMQ、AzureTable、Kafka
            DbaTenant = new Tenant()
            {
                TenantId = TenantConstant.DbaTenantId,
                CloudType = CloudType.FileSystem,
                Version = TenantVersion.Standard,
                TenantType = TenantType.Enterprise,

                TenantName = TenantConstant.DbaTenantName,
                TenantDisplayName = TenantConstant.DbaTenantDisplayName,
                TenantSignature = TenantConstant.DbaTenantSignature,
                PrivateEncryptKey = "N4d8775824n",
                ContactEmail = "tianchangjun@cfwin.com",
                //DataBase
                DatabaseType = DatabaseType.SqlServer,
                Server = "localhost",
                Database = "MSSqlKCContext",
                DatabasePasswordHash = "T5TpUcgaTDVBLwN8EQS9UA==",
                //Storage
                StorageType = StorageType.FTP,
                StorageEndpoint = "ftp://192.168.2.141",
                StorageAccessName = "ftpuser",
                StorageAccessKeyPasswordHash = "TlmPz46KPI7zt/TLnjd6fmmNaSY6qNSn1MbKP7gpOr0=",
                //Queue
                QueueType = QueueType.Redis,
                QueueEndpoint = ".",
                QueueAccessName = "localMsmqQueue",
                QueueAccessKeyPasswordHash = "oeWnC1Gb/Po=",
                //NoSql
                NoSqlType = NoSqlType.AzureTable,
                NoSqlEndpoint = "https://cfwinstorage.table.core.chinacloudapi.cn/",
                NoSqlAccessName = "cfwinstorage",
                NoSqlAccessKeyPasswordHash = "hEwbGcfmKPgpOz+kq0B7G3bfKBpkwjYgU05eZ96kgyJqmeCBbqvYgwVuf3uhUsxzDhs6dYTS+e+DSfD3nMcDtPsBqlb/Are3zZqE2R2aDlDYS/F+R1XRRUI+TD46Spd2",
                //ServiceBus
                ServiceBusType = ServiceBusType.Kafka,
                ServiceBusEndpoint = "192.168.2.80:9092,192.168.2.80:9093,192.168.2.80:9094",
                ServiceBusAccessName = "localKafkaQueue",
                ServiceBusAccessKeyPasswordHash = "Zc0i/S7QPjo=",
                //Vod
                VodType = VodType.Aliyun,
                VodEndpoint = "cn-shanghai",
                VodAccessName = "LTAI4GBeijVm43GrG42P1AqZ",
                VodAccessKeyPasswordHash = "6gre8MTFLwhQnbKPaITDNfHZ3JgWvu/U/Wg5Px908RM=",
            };
            #endregion

            #region Init Test's Tenant SqlServer、File、MSMQ、AzureTable、Kafka
            TestTenant = new Tenant()
            {
                TenantId = TenantConstant.TestTenantId,
                CloudType = CloudType.FileSystem,
                Version = TenantVersion.Standard,
                TenantType = TenantType.Bank,//商业保理

                TenantName = TenantConstant.TestTenantName,
                TenantDisplayName = TenantConstant.TestTenantDisplayName,
                TenantSignature = TenantConstant.TestTenantSignature,
                PrivateEncryptKey = "N4d8775824n",
                ContactEmail = "tianchangjun@cfwin.com",
                //DataBase
                DatabaseType = DatabaseType.SqlServer,
                Server = "localhost",
                Database = "MSSqlKCContext",
                DatabasePasswordHash = "T5TpUcgaTDVBLwN8EQS9UA==",
                //Storage
                StorageType = StorageType.File,
                StorageAccessName = "localStorage",
                StorageEndpoint = "D:\\Business\\blobstorage",
                StorageAccessKeyPasswordHash = "TlmPz46KPI7zt/TLnjd6fmmNaSY6qNSn1MbKP7gpOr0=",
                //Queue
                QueueType = QueueType.Redis,
                QueueAccessName = "localMsmqQueue",
                QueueEndpoint = ".",
                QueueAccessKeyPasswordHash = "oeWnC1Gb/Po=",
                //NoSql
                NoSqlType = NoSqlType.AzureTable,
                NoSqlAccessName = "cfwinstorage",
                NoSqlEndpoint = "https://cfwinstorage.table.core.chinacloudapi.cn/",
                NoSqlAccessKeyPasswordHash = "hEwbGcfmKPgpOz+kq0B7G3bfKBpkwjYgU05eZ96kgyJqmeCBbqvYgwVuf3uhUsxzDhs6dYTS+e+DSfD3nMcDtPsBqlb/Are3zZqE2R2aDlDYS/F+R1XRRUI+TD46Spd2",
                //ServiceBus
                ServiceBusType = ServiceBusType.Kafka,
                ServiceBusAccessName = "localKafkaQueue",
                ServiceBusEndpoint = "192.168.2.80:9092,192.168.2.80:9093,192.168.2.80:9094",
                ServiceBusAccessKeyPasswordHash = "Zc0i/S7QPjo=",
                //Vod
                VodType = VodType.Aliyun,
                VodEndpoint = "cn-shanghai",
                VodAccessName = "LTAI4GBeijVm43GrG42P1AqZ",
                VodAccessKeyPasswordHash = "6gre8MTFLwhQnbKPaITDNfHZ3JgWvu/U/Wg5Px908RM=",

            };
            #endregion

            #region Init Buy's Tenant  MySql、Blob、AzureQueue、AzureTable、ServiceBus
            BuyTenant = new Tenant()
            {
                TenantId = TenantConstant.BuyTenantId,
                CloudType = CloudType.Azure,
                Version = TenantVersion.Standard,
                TenantType = TenantType.Institution,//店铺赊销

                TenantName = TenantConstant.BuyTenantName,
                TenantDisplayName = TenantConstant.BuyTenantDisplayName,
                TenantSignature = TenantConstant.BuyTenantSignature,
                PrivateEncryptKey = "K7ef0139cbk",
                ContactEmail = "tianchangjun@cfwin.com",
                //DataBase
                DatabaseType = DatabaseType.MySql,
                Server = "localhost",
                Database = "MySqlKCContext",
                DatabasePasswordHash = "R+YUBQCJ3CqLVWSYsQEWlg==",
                //Storage
                StorageType = StorageType.Blob,
                StorageAccessName = "cfwinstorage",
                StorageEndpoint = "https://cfwinstorage.blob.core.chinacloudapi.cn/",
                StorageAccessKeyPasswordHash = "msPXQJhZYAXqMVdqtqH0c32ORWBZJ0IKB55WOL2SsPPbPQPiLqliWdUDLVAIHR+F/hxOz+Z7POzv6yVP+/82uzzDwIfe3owjJGhvU179nu9ADw3NNWjZumHY6zpOg7f0",
                //Queue
                QueueType = QueueType.AzureQueue,
                QueueAccessName = "cfwinstorage",
                QueueEndpoint = "https://cfwinstorage.queue.core.chinacloudapi.cn/",
                QueueAccessKeyPasswordHash = "msPXQJhZYAXqMVdqtqH0c32ORWBZJ0IKB55WOL2SsPPbPQPiLqliWdUDLVAIHR+F/hxOz+Z7POzv6yVP+/82uzzDwIfe3owjJGhvU179nu9ADw3NNWjZumHY6zpOg7f0",
                //NoSql
                NoSqlType = NoSqlType.AzureTable,
                NoSqlAccessName = "cfwinstorage",
                NoSqlEndpoint = "https://cfwinstorage.table.core.chinacloudapi.cn/",
                NoSqlAccessKeyPasswordHash = "msPXQJhZYAXqMVdqtqH0c32ORWBZJ0IKB55WOL2SsPPbPQPiLqliWdUDLVAIHR+F/hxOz+Z7POzv6yVP+/82uzzDwIfe3owjJGhvU179nu9ADw3NNWjZumHY6zpOg7f0",
                //ServiceBus
                ServiceBusType = ServiceBusType.ServiceBus,
                ServiceBusAccessName = "RootManageSharedAccessKey",
                ServiceBusEndpoint = "sb://devcfwin.servicebus.chinacloudapi.cn/",
                ServiceBusAccessKeyPasswordHash = "rhIVOASJncr/HciOebECPxa7FgcoRo85fUV5inBVKnlAPmlUDKSHY3MAszNCO4py",
                //Vod
                VodType = VodType.Aliyun,
                VodEndpoint = "cn-shanghai",
                VodAccessName = "LTAI4GBeijVm43GrG42P1AqZ",
                VodAccessKeyPasswordHash = "D/tC+LOs1AZj2qD3AtX1r6wp8p0V+vyh9CBdA5MCSkk=",
            };
            #endregion

            #region Init Sale's Tenant SqlServer、OSS、AliQueue、AzureTable、Redis
            SaleTenant = new Tenant()
            {
                TenantId = TenantConstant.SaleTenantId,
                CloudType = CloudType.Azure,
                Version = TenantVersion.Standard,
                TenantType = TenantType.Enterprise,//店铺赊销

                TenantName = TenantConstant.SaleTenantName,
                TenantDisplayName = TenantConstant.SaleTenantDisplayName,
                TenantSignature = TenantConstant.SaleTenantSignature,
                PrivateEncryptKey = "K7ef0139cbk",
                ContactEmail = "tianchangjun@cfwin.com",
                //DataBase
                DatabaseType = DatabaseType.SqlServer,
                Server = "localhost",
                Database = "MSSqlKCContext",
                DatabasePasswordHash = "R+YUBQCJ3CqLVWSYsQEWlg==",
                //Storage
                StorageType = StorageType.AliOSS,
                //StorageEndpoint = "http://oss-cn-shenzhen.aliyuncs.com",
                //StorageAccessName = "LTAI4G3LCfpphDx3pLBgbHmY",
                //StorageAccessKeyPasswordHash = "dZUhvLovTUjBfqhF/hsxtwwR869kaCw83DAGIcny1cI=",
                StorageEndpoint = "http://oss-cn-zhangjiakou.aliyuncs.com",
                StorageAccessName = "LTAI5tDVtF774CwX5jMxisU9",
                StorageAccessKeyPasswordHash = "Kn7VTR/C6mmOyy9Bh4ltS/CVZ72wxtR0yor3TCCmWzQ=",
                //Queue
                QueueType = QueueType.Redis,
                QueueAccessName = "r-wz9282bf3ce75dd4",
                QueueEndpoint = "kcloudy-redis.redis.rds.aliyuncs.com:6379",
                QueueAccessKeyPasswordHash = "Wc491BgUyKJ0rMgU5Vx1JS7iyoek6oYp",
                //NoSql
                NoSqlType = NoSqlType.AzureTable,
                NoSqlAccessName = "cfwinstorage",
                NoSqlEndpoint = "https://cfwinstorage.table.core.chinacloudapi.cn/",
                NoSqlAccessKeyPasswordHash = "msPXQJhZYAXqMVdqtqH0c32ORWBZJ0IKB55WOL2SsPPbPQPiLqliWdUDLVAIHR+F/hxOz+Z7POzv6yVP+/82uzzDwIfe3owjJGhvU179nu9ADw3NNWjZumHY6zpOg7f0",
                //ServiceBus
                ServiceBusType = ServiceBusType.Redis,
                ServiceBusAccessName = "r-wz9282bf3ce75dd4",
                ServiceBusEndpoint = "kcloudy-redis.redis.rds.aliyuncs.com:6379",
                ServiceBusAccessKeyPasswordHash = "Wc491BgUyKJ0rMgU5Vx1JS7iyoek6oYp",
                //Vod
                VodType = VodType.Aliyun,
                VodEndpoint = "cn-shanghai",
                VodAccessName = "LTAI4GBeijVm43GrG42P1AqZ",
                VodAccessKeyPasswordHash = "D/tC+LOs1AZj2qD3AtX1r6wp8p0V+vyh9CBdA5MCSkk=",
            };
            #endregion

        }

    }
}
