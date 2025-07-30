using KC.Common;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Service.Constants;
using KC.Service.Extension;
using KC.IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KC.Service.WebApiService.Business;

namespace KC.UnitTest.Account.WebApiTest.Business
{
    
    public class AccountApiServiceTest : AccountTestBase
    {
        private ILogger _logger;
        public AccountApiServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(AccountApiServiceTest));
        }

        private const string _clientId= "testclient";
        private const string _clientSecret = "secret";
        private const string _contentType = "application/json";
        private const string _accountScope = ApplicationConstant.AccScope;
        private const string _accountApiUri = "http://ctest.localhost:2002";

        #region AccountApi with Test Client
        [Xunit.Fact]
        public async Task Test_AccountApi_LoadUserMenusByRoleIds()
        {
            try
            {
                InjectTenant(TestTenant);
                var accountApiService = ServiceProvider.GetService<IAccountApiService>();

                var roleIds = new List<string>() {
                    RoleConstants.AdminRoleId
                };
                var jsonData = SerializeHelper.ToJson(roleIds);

                System.Console.Title = "Console Client Credentials Flow";

                var accessToken = await GetAccessTokenAsync(_clientId, _clientSecret, _accountScope);
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
                    _logger.LogDebug(string.Format("apiUri【{0}】 errors: {1}" , apiUri, errorString));
                    return;
                }
                //内容
                var responseString = await response.Content.ReadAsStringAsync();
                "\n\nService claims:".ConsoleGreen();
                _logger.LogDebug(string.Format("apiUri【{0}】 result: {1}", apiUri, responseString));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + ex.StackTrace);
            }

        }
        #endregion

        #region AccountApiService
        [Xunit.Fact]
        public async Task Test_AccountApiService_LoadUserMenusByRoleIds()
        {
            InjectTenant(TestTenant);
            var accountApiService = ServiceProvider.GetService<IAccountApiService>();

            var roleIds = new List<string>()
            {
                RoleConstants.AdminRoleId
            };
            var result = await accountApiService.LoadUserMenusByRoleIds(roleIds);
            Assert.True(result != null && result.Count > 0);
        }

        [Xunit.Fact]
        public async Task Test_AccountApiService_LoadUserPermissionsByRoleIds()
        {
            InjectTenant(TestTenant);
            var accountApiService = ServiceProvider.GetService<IAccountApiService>();

            var roleIds = new List<string>()
            {
                RoleConstants.AdminRoleId
            };
            
            var result = await accountApiService.LoadUserPermissionsByRoleIds(roleIds);
            Assert.True(result != null && result.Count > 0);
        }

        [Xunit.Fact]
        public async Task Test_AccountApiService_LoadUserManagersByUserId()
        {
            InjectTenant(TestTenant);
            var accountApiService = ServiceProvider.GetService<IAccountApiService>();

            var result = await accountApiService.LoadUserManagersByUserId(RoleConstants.AdminUserId);
            Assert.True(result.Count > 0);
        }

        [Xunit.Fact]
        public async Task Test_AccountApiService_LoadOrgTreesWithUsersByOrgIds()
        {
            InjectTenant(TestTenant);
            var accountApiService = ServiceProvider.GetService<IAccountApiService>();

            var searchModel = new Service.DTO.Search.OrgTreesAndRolesWithUsersSearchDTO();
            searchModel.OrgIds.Add(OrganizationConstants.IT部_Id);
            var result = await accountApiService.LoadOrgTreesWithUsersByOrgIds(searchModel);
            Assert.True(result.Count > 0);
        }

        [Xunit.Fact]
        public async Task Test_AccountApiService_LoadRolesWithUsersByRoleIds()
        {
            InjectTenant(DbaTenant);
            var accountApiService = ServiceProvider.GetService<IAccountApiService>();

            var searchModel = new Service.DTO.Search.OrgTreesAndRolesWithUsersSearchDTO();
            //searchModel.RoleIds.Add(RoleConstants.AdminUserId);
            var result = await accountApiService.LoadRolesWithUsersByRoleIds(searchModel);
            Assert.True(result.Count > 0);
        }
        #endregion

    }
}
