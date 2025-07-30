using Xunit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KC.Service.Extension;
using KC.IdentityModel.Client;
using KC.Framework.Tenant;
using KC.Service.Constants;
using Microsoft.Extensions.Logging;
using KC.Framework.Extension;

namespace KC.UnitTest.Admin.WebApiTest.Business
{
    
    public class TenantApiServiceTest : AdminTestBase
    {
        private ILogger _logger;
        public TenantApiServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(TenantApiServiceTest));
        }

        private const string _clientId = "testclient";
        private const string _clientSecret = "secret";
        private const string _adminScope = "adminapi";
        private const string _adminApiUri = "http://localhost:1009";

        [Xunit.Fact]
        public async Task Test_TenantApi_ExistTenantName()
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
                _logger.LogDebug("\n\nService claims:" + result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.StackTrace);
            }

        }

        [Xunit.Fact]
        public async Task Test_TenantApiService_ExistTenantName()
        {
            var result = await TenantUserApiService.ExistTenantName(TenantConstant.DbaTenantName);
            Assert.True(result.success && result.Result);
        }

        [Xunit.Fact]
        public async Task Test_TenantApiService_GetTenantByName()
        {
            var tenantDba = await TenantUserApiService.GetTenantByName(TenantConstant.DbaTenantName);
            Assert.Equal(DbaTenant.TenantId, tenantDba.TenantId);
            Assert.Equal(DbaTenant.TenantName, tenantDba.TenantName);
            _logger.LogDebug(tenantDba.DatabaseType.ToDescription() + "----Dba Database Connect: " + tenantDba.ConnectionString);
            _logger.LogDebug(tenantDba.StorageType.ToDescription() + "----Dba Storage Connect: " + tenantDba.GetDecryptStorageConnectionString());
            _logger.LogDebug(tenantDba.QueueType.ToDescription() + "----Dba Queue Connect: " + tenantDba.GetDecryptQueueConnectionString());
            _logger.LogDebug(tenantDba.ServiceBusType.ToDescription() + "----Dba ServiceBus Connect: " + tenantDba.GetDecryptServiceBusConnectionString());

            var tenantTest = await TenantUserApiService.GetTenantByName(TenantConstant.TestTenantName);
            Assert.Equal(TestTenant.TenantId, tenantTest.TenantId);
            Assert.Equal(TestTenant.TenantName, tenantTest.TenantName);
            _logger.LogDebug(tenantTest.DatabaseType.ToDescription() + "----Test Database Connect: " + tenantTest.ConnectionString);
            _logger.LogDebug(tenantTest.StorageType.ToDescription() + "----Test Storage Connect: " + tenantTest.GetDecryptStorageConnectionString());
            _logger.LogDebug(tenantTest.QueueType.ToDescription() + "----Test Queue Connect: " + tenantTest.GetDecryptQueueConnectionString());
            _logger.LogDebug(tenantTest.ServiceBusType.ToDescription() + "----Test ServiceBus Connect: " + tenantTest.GetDecryptServiceBusConnectionString());

            var tenantBuy = await TenantUserApiService.GetTenantByName(TenantConstant.BuyTenantName);
            Assert.Equal(BuyTenant.TenantId, tenantBuy.TenantId);
            Assert.Equal(BuyTenant.TenantName, tenantBuy.TenantName);
            _logger.LogDebug(tenantBuy.DatabaseType.ToDescription() + "----Buy Database Connect: " + tenantBuy.ConnectionString);
            _logger.LogDebug(tenantBuy.StorageType.ToDescription() + "----Buy Storage Connect: " + tenantBuy.GetDecryptStorageConnectionString());
            _logger.LogDebug(tenantBuy.QueueType.ToDescription() + "----Buy Queue Connect: " + tenantBuy.GetDecryptQueueConnectionString());
            _logger.LogDebug(tenantBuy.ServiceBusType.ToDescription() + "----Buy ServiceBus Connect: " + tenantBuy.GetDecryptServiceBusConnectionString());
        }
    }
}
