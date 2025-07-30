using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Util;
using KC.Service.Constants;
using KC.Service.WebApiService.Business;
using KC.IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using KC.Service.Util;
using KC.Service.Message;
using KC.Framework.Tenant;
using KC.Service.DTO.Message;
using KC.Service.Account.Message;

namespace KC.UnitTest.Account
{
    public class MessageUtilTest : AccountTestBase
    {
        private static ILogger _logger;
        public MessageUtilTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(MessageUtilTest));
        }

        [Xunit.Fact]
        public void Test_MessageUtil()
        {
            InjectTenant(TestTenant);

            var messageUtil = ServiceProvider.GetService<MessageUtil>();

            //创建用户成功发送邮件
            var inputParameter = new Dictionary<string, string> {
                {"UserId", "测试：用户Id"},
                {"UserName", "测试：admin"},
                {"DisplayName", "测试：管理员"},
                {"Email", "测试：test@126.com"},
                {"PhoneNumber", "17744949695"},
                {"Password", "test"},
                {"LoginUrl", "http://www.baidu.com"}
            };
            var user = new SendUserDTO() {
                UserId = RoleConstants.AdminUserId,
                UserName = RoleConstants.AdminUserName,
                DisplayName = "管理员",
                Email = "tianchangjun@outlook.com",
                PhoneNumber = "17744949695",
            };
            
            var result = messageUtil.SendMessage(TestTenant, UserTemplateGenerator.User_Created, inputParameter, new List<SendUserDTO>() { user });
            Assert.True(result);
        }
    }
}
