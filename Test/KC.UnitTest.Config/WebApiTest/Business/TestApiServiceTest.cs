using Xunit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KC.Service.Extension;
using KC.IdentityModel.Client;
using KC.Service.Constants;
using Microsoft.Extensions.Logging;

namespace KC.UnitTest.Config.WebApiTest.Business
{
    
    public class TestApiServiceTest : ConfigTestBase
    {
        private ILogger _logger;
        public TestApiServiceTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(TestApiServiceTest));
        }

        private const string _clientId = "testclient";
        private const string _clientSecret = "secret";
        private const string _adminScope = "adminapi";
        private const string _adminApiUri = "http://localhost:1009";

        [Xunit.Fact]
        public async Task Test_TestApi_Get()
        {
            System.Console.Title = "Console Client Credentials Flow";

            var accessToken = await GetAccessTokenAsync(_clientId, _clientSecret, _adminScope);
            var client = new HttpClient();
            client.BaseAddress = new Uri(_adminApiUri);
            client.Timeout = TimeSpan.FromMinutes(TimeOutConstants.CacheShortTimeOut);
            client.SetBearerToken(accessToken);

            var result = await client.GetStringAsync("/api/TestApi");

            "\n\nService claims:".ConsoleGreen();
            _logger.LogDebug("\n\nService claims:" + result);
        }
    }
}
