using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using KC.Framework.SecurityHelper;
using Xunit;
using KC.Framework.Tenant;
using Microsoft.Extensions.Logging;
using KC.Framework.Base;

namespace KC.Framework.UnitTest
{
    /// <summary>
    /// EncryptPasswordUtil 的摘要说明
    /// </summary>
    public class EncryptPasswordUtil_Test : KC.UnitTest.Framework.FrameworkTestBase
    {
        private ILogger _logger;
        public EncryptPasswordUtil_Test(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(EncryptPasswordUtil_Test));
        }

       
        [Xunit.Fact]
        public void Test_EncryptPasswordUtil()
        {
            //var key = EncryptPasswordUtil.GetRandomString();
            //var pwd = "P@ssw0rd";

            var key = "KCloudy-Microsoft-EncryptKey";
            var pwd = "XdA9caoq";

            _logger.LogInformation("--Test_EncryptPasswordUtil pwd: " + pwd);
            var encryptString = EncryptPasswordUtil.EncryptPassword(pwd, key);
            _logger.LogInformation("--Test_EncryptPasswordUtil key: " + key);
            _logger.LogInformation("--Test_EncryptPasswordUtil encryptString: " + encryptString);

            var decryptString = EncryptPasswordUtil.DecryptPassword(encryptString, key);
            _logger.LogInformation("--Test_EncryptPasswordUtil decryptString: " + decryptString);

            Assert.Equal(pwd, decryptString);
        }

        [Xunit.Fact]
        public void Test_TenantEncryptPassword()
        {
            var decryptString = EncryptPasswordUtil.DecryptPassword("4+jbEzR3oUB9k8s+n1gOTifJ40FnghwhobaekoLzruTR0GM6sDnKzZiIHDzE8eN7grq5e5QwQxwcvzoTZeTh/kOTrJWSwGcaroHrEwQ4qPnYDWmzRHd/zoj1x218NLeI", "L3c132f119l");
            _logger.LogInformation("--Test_TenantEncryptPassword decryptString: " + decryptString);

            var encryptString = EncryptPasswordUtil.EncryptPassword(decryptString, "L3c65fd29");
            _logger.LogInformation("--Test_TenantEncryptPassword encryptString: " + encryptString);

            var decryptString2 = EncryptPasswordUtil.DecryptPassword(encryptString, "L3c65fd29");
            _logger.LogInformation("--Test_TenantEncryptPassword decryptString2: " + decryptString2);

            var database = EncryptPasswordUtil.DecryptPassword("ld9km+/hXXgxD9YgsPSnrA==", "Ad9525565");
            _logger.LogInformation("--Test_TenantEncryptPassword database: " + database);
        }

        [Xunit.Fact]
        public void Test_EncryptString_Tenant()
        {
            // MsSql用户密码加密秘钥
            var databaseKey = "P@ssw0rd";
            var encryptKey = "dev-cfwin-EncryptKey";
            var decryptDatabaseKey = EncryptPasswordUtil.EncryptPassword(databaseKey, encryptKey);
            _logger.LogInformation("MsSql decrypt key: " + decryptDatabaseKey + " with encrypt: " + encryptKey);
            encryptKey = "KCloudy-Microsoft-EncryptKey";
            decryptDatabaseKey = EncryptPasswordUtil.EncryptPassword(databaseKey, encryptKey);
            _logger.LogInformation("MsSql decrypt key: " + decryptDatabaseKey + " with encrypt: " + encryptKey);

            var databaseConnString = GlobalConfig.GetDecryptDatabaseConnectionString();
            _logger.LogInformation("MsSql ConnectionString: " + databaseConnString);

            // MySql用户密码加密秘钥
            var mysqlKey = "P@ssw0rd";
            encryptKey = "dev-cfwin-EncryptKey";
            var decryptMySqlKey = EncryptPasswordUtil.EncryptPassword(mysqlKey, encryptKey);
            _logger.LogInformation("mySql decrypt key: " + decryptMySqlKey + " with encrypt: " + encryptKey);
            encryptKey = "KCloudy-Microsoft-EncryptKey";
            decryptMySqlKey = EncryptPasswordUtil.EncryptPassword(mysqlKey, encryptKey);
            _logger.LogInformation("mySql decrypt key: " + decryptMySqlKey + " with encrypt: " + encryptKey);

            var mySqlConnString = GlobalConfig.GetDecryptMySqlConnectionString();
            _logger.LogInformation("mySql ConnectionString: " + mySqlConnString);

            // storage用户密码加密秘钥
            var storageKey = "P@ssw0rd";
            encryptKey = "dev-cfwin-EncryptKey";
            var decryptStorageKey = EncryptPasswordUtil.EncryptPassword(storageKey, encryptKey);
            _logger.LogInformation("storage decrypt key: " + decryptStorageKey + " with encrypt: " + encryptKey);
            encryptKey = "KCloudy-Microsoft-EncryptKey";
            decryptStorageKey = EncryptPasswordUtil.EncryptPassword(storageKey, encryptKey);
            _logger.LogInformation("storage decrypt key: " + decryptStorageKey + " with encrypt: " + encryptKey);
            
            var storageConnString = GlobalConfig.GetDecryptStorageConnectionString();
            _logger.LogInformation("storage ConnectionString: " + storageConnString);

            // 队列用户密码加密秘钥
            var queueKey = "P@ssw0rd";
            encryptKey = "dev-cfwin-EncryptKey";
            var decryptQueueKey = EncryptPasswordUtil.EncryptPassword(queueKey, encryptKey);
            _logger.LogInformation("queue decrypt key: " + decryptQueueKey + " with encrypt: " + encryptKey);
            encryptKey = "KCloudy-Microsoft-EncryptKey";
            decryptQueueKey = EncryptPasswordUtil.EncryptPassword(queueKey, encryptKey);
            _logger.LogInformation("queue decrypt key: " + decryptQueueKey + " with encrypt: " + encryptKey);
            
            var queueConnString = GlobalConfig.GetDecryptQueueConnectionString();
            _logger.LogInformation("queue ConnectionString: " + queueConnString);

            // redis用户密码加密秘钥
            var redisKey = "P@ssw0rd";
            encryptKey = "dev-cfwin-EncryptKey";
            var decryptRedisKey = EncryptPasswordUtil.EncryptPassword(redisKey, encryptKey);
            _logger.LogInformation("redis decrypt key: " + decryptRedisKey + " with encrypt: " + encryptKey);
            encryptKey = "KCloudy-Microsoft-EncryptKey";
            decryptRedisKey = EncryptPasswordUtil.EncryptPassword(redisKey, encryptKey);
            _logger.LogInformation("redis decrypt key: " + decryptRedisKey + " with encrypt: " + encryptKey);

            var redisConnString = GlobalConfig.GetDecryptRedisConnectionString();
            _logger.LogInformation("redis ConnectionString: " + redisConnString);

            // gitlab用户（tenant-admin)Token加密秘钥
            var gitlabKey = "xpkmXq6Nx_EdtS8PQiS4";
            encryptKey = "dev-cfwin-EncryptKey";
            var encryptGitlabKey = EncryptPasswordUtil.EncryptPassword(gitlabKey, encryptKey);
            var decryptGitlabKey = EncryptPasswordUtil.DecryptPassword(encryptGitlabKey, encryptKey);
            _logger.LogInformation("gitlab decrypt key: " + encryptGitlabKey + " with encrypt: " + encryptKey + " with decrypt: " + decryptGitlabKey);
            encryptKey = "KCloudy-Microsoft-EncryptKey";
            encryptGitlabKey = EncryptPasswordUtil.EncryptPassword(gitlabKey, encryptKey);
            decryptGitlabKey = EncryptPasswordUtil.DecryptPassword(encryptGitlabKey, encryptKey);
            _logger.LogInformation("gitlab decrypt key: " + encryptGitlabKey + " with encrypt: " + encryptKey + " with decrypt: " + decryptGitlabKey);

            var gitlabConnString = GlobalConfig.GetDecryptCodeConnectionString();
            _logger.LogInformation("gitlab ConnectionString: " + gitlabConnString);
        }

        [Xunit.Fact]
        public void Test_DatabasePasswordHash()
        {
            var dbaDatabasePassword = EncryptPasswordUtil.DecryptPassword(DbaTenant.DatabasePasswordHash,
                DbaTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(dbaDatabasePassword));
            _logger.LogInformation("----Dba's Database password: " + dbaDatabasePassword);
            var dbaStorageConnect = EncryptPasswordUtil.DecryptPassword(DbaTenant.StorageAccessKeyPasswordHash,
                DbaTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(dbaStorageConnect));
            _logger.LogInformation("----cDba's Storage connect string: " + dbaStorageConnect);
            var dbaQueueConnect = EncryptPasswordUtil.DecryptPassword(DbaTenant.QueueAccessKeyPasswordHash,
                DbaTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(dbaQueueConnect));
            _logger.LogInformation("----cDba's Queue connect string: " + dbaQueueConnect);

            var devdbDatabasePassword = EncryptPasswordUtil.DecryptPassword(TestTenant.DatabasePasswordHash,
                TestTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(devdbDatabasePassword));
            _logger.LogInformation("----cTest's Database password: " + devdbDatabasePassword);
            var devdbStorageConnect = EncryptPasswordUtil.DecryptPassword(TestTenant.StorageAccessKeyPasswordHash,
                TestTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(devdbStorageConnect));
            _logger.LogInformation("----cTest's Storage connect string: " + devdbStorageConnect);
            var devdbQueueConnect = EncryptPasswordUtil.DecryptPassword(TestTenant.QueueAccessKeyPasswordHash,
                TestTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(devdbQueueConnect));
            _logger.LogInformation("----cTest's Queue connect string: " + devdbQueueConnect);

            var BuyDatabasePassword = EncryptPasswordUtil.DecryptPassword(BuyTenant.DatabasePasswordHash,
                BuyTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(BuyDatabasePassword));
            _logger.LogInformation("----cBuy's Database password: " + BuyDatabasePassword);
            var BuyStorageConnect = EncryptPasswordUtil.DecryptPassword(BuyTenant.StorageAccessKeyPasswordHash,
                BuyTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(BuyStorageConnect));
            _logger.LogInformation("----cBuy's Storage connect string: " + BuyStorageConnect);
            var BuyQueueConnect = EncryptPasswordUtil.DecryptPassword(BuyTenant.QueueAccessKeyPasswordHash,
                BuyTenant.PrivateEncryptKey);
            Assert.True(!string.IsNullOrEmpty(BuyQueueConnect));
            _logger.LogInformation("----cBuy's Queue connect string: " + BuyQueueConnect);
        }

        [Xunit.Fact]
        public void Test_TenantSignature()
        {
            //var DbaSignatureObj = new KC.Framework.Base.ServiceRequestToken(TenantConstant.DbaTenantId.ToString(), TenantConstant.DbaTenantName, TenantConstant.DefaultPrivateEncryptKey);
            //var DbaSignature = DbaSignatureObj.GetEncrptSignature();
            var DbaSignature = TenantConstant.DbaTenantApiAccessInfo.GenerateSignature();
            Assert.True(!string.IsNullOrEmpty(DbaSignature));
            _logger.LogInformation("----Dba's Signature: " + DbaSignature);
            //var exceptDbaSignature = new KC.Framework.Base.ServiceRequestToken(DbaSignature, TenantConstant.DefaultPrivateEncryptKey);
            var exceptDbaSignature = TenantConstant.DbaTenantApiAccessInfo.GetServiceToken();
            Assert.True(string.IsNullOrEmpty(exceptDbaSignature.IsValid(TenantConstant.DbaTenantName)));
            Assert.Equal(TenantConstant.DbaTenantApiAccessInfo.TenantName, exceptDbaSignature.MemberId);

            //var DevDbSignatureObj = new KC.Framework.Base.ServiceRequestToken(TenantConstant.TestTenantId.ToString(), TenantConstant.TestTenantName, TenantConstant.DefaultPrivateEncryptKey);
            //var DevDbSignature = DevDbSignatureObj.GetEncrptSignature();
            var DevDbSignature = TenantConstant.TestTenantApiAccessInfo.GenerateSignature();
            Assert.True(!string.IsNullOrEmpty(DevDbSignature));
            _logger.LogInformation("----DevDb's Signature: " + DevDbSignature);
            //var exceptDevDBSignature = new KC.Framework.Base.ServiceRequestToken(DevDbSignature, TenantConstant.DefaultPrivateEncryptKey);
            var exceptDevDBSignature = TenantConstant.TestTenantApiAccessInfo.GetServiceToken();
            Assert.True(string.IsNullOrEmpty(exceptDevDBSignature.IsValid(TenantConstant.TestTenantName)));
            Assert.Equal(TenantConstant.TestTenantApiAccessInfo.TenantName, exceptDevDBSignature.MemberId);

            //var BuySignatureObj = new KC.Framework.Base.ServiceRequestToken(TenantConstant.BuyTenantId.ToString(), TenantConstant.BuyTenantName, TenantConstant.DefaultPrivateEncryptKey);
            //var BuySignature = BuySignatureObj.GetEncrptSignature();
            var BuySignature = TenantConstant.BuyTenantApiAccessInfo.GenerateSignature();
            Assert.True(!string.IsNullOrEmpty(BuySignature));
            _logger.LogInformation("----Buy's Signature: " + BuySignature);
            //var exceptBuySignature = new KC.Framework.Base.ServiceRequestToken(BuySignature, TenantConstant.DefaultPrivateEncryptKey);
            var exceptBuySignature = TenantConstant.BuyTenantApiAccessInfo.GetServiceToken();
            Assert.True(string.IsNullOrEmpty(exceptBuySignature.IsValid(TenantConstant.BuyTenantName)));
            Assert.Equal(TenantConstant.BuyTenantApiAccessInfo.TenantName, exceptBuySignature.MemberId);

            //var SaleSignatureObj = new KC.Framework.Base.ServiceRequestToken(TenantConstant.SaleTenantId.ToString(), TenantConstant.SaleTenantName, TenantConstant.DefaultPrivateEncryptKey);
            //var SaleSignature = SaleSignatureObj.GetEncrptSignature();
            var SaleSignature = TenantConstant.SaleTenantApiAccessInfo.GenerateSignature();
            Assert.True(!string.IsNullOrEmpty(SaleSignature));
            _logger.LogInformation("----Sale's Signature: " + SaleSignature);
            //var exceptSaleSignature = new KC.Framework.Base.ServiceRequestToken(SaleSignature, TenantConstant.DefaultPrivateEncryptKey);
            var exceptSaleSignature = TenantConstant.SaleTenantApiAccessInfo.GetServiceToken();
            Assert.True(string.IsNullOrEmpty(exceptSaleSignature.IsValid(TenantConstant.SaleTenantName)));
            Assert.Equal(TenantConstant.SaleTenantApiAccessInfo.TenantName, exceptSaleSignature.MemberId);
        }

        [Xunit.Fact]
        public void Test_GetRandomString()
        {
            var len = 11;
            var randomStr = EncryptPasswordUtil.GetRandomString(len);
            _logger.LogInformation("----randomStr: " + randomStr);
            Assert.Equal(len, randomStr.Length);
        }

    }
}
