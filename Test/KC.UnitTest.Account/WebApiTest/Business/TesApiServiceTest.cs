using KC.Common;
using KC.Framework.Util;
using KC.Service.Constants;
using KC.Service.Extension;
using Xunit;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KC.IdentityModel.Client;
using KC.Framework.Tenant;

namespace KC.UnitTest.Account.WebApiTest.Business
{
    
    public class TesApiServiceTest : AccountTestBase
    {
        private ILogger _logger;
        public TesApiServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(TesApiServiceTest));
        }

        private const string _clientId= "testclient";
        private const string _clientSecret = "secret";
        private const string _accountScope = ApplicationConstant.AccScope;
        private const string _accountApiUri = "http://ctest.localhost:2002";

        [Xunit.Fact]
        public async Task Test_TestApi_Get()
        {
            InjectTenant(TestTenant);
            System.Console.Title = "Console Client Credentials Flow";

            var clientId = TenantConstant.GetClientIdByTenant(TestTenant);
            var clientSecret = TenantConstant.GetClientSecretByTenant(TestTenant);

            var accessToken = await GetAccessTokenAsync(clientId, clientSecret, _accountScope);
            var client = new HttpClient();
            // for fiddler proxy
            //var httpClientHandler = new HttpClientHandler
            //{
            //    Proxy = new WebProxy("http://Kent-surface:8888", false),
            //    UseProxy = true
            //};
            //var client = new HttpClient(httpClientHandler);

            client.BaseAddress = new Uri(_accountApiUri);
            client.Timeout = TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut);
            client.SetBearerToken(accessToken);

            var result = await client.GetStringAsync("/api/TestApi");

            "\n\nService claims:".ConsoleGreen();
            _logger.LogInformation(SerializeHelper.ToJson(result));
            _logger.LogDebug("\n\nService claims:" + result);
        }
        
    }
}
