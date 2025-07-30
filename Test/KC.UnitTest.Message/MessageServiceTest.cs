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
using KC.Service.DTO.Message;
using KC.Framework.Tenant;

namespace KC.UnitTest.Message
{
    public class MessageServiceTest : MessageTestBase
    {
        private static ILogger _logger;
        public MessageServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(MessageServiceTest));
        }

        [Xunit.Fact]
        public async Task Test_SaveMessageCategory()
        {
            InjectTenant(DbaTenant);

            int rootCategoryId = 0;
            //测试新增顶级分类
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var messageService = scope.ServiceProvider.GetService<IMessageService>();
                var category = new MessageCategoryDTO()
                {
                    ParentId = null,
                    Text = "【测试Category】" + Guid.NewGuid(),
                };
                var result = await messageService.SaveMessageCategoryAsync(category, RoleConstants.AdminUserId, RoleConstants.AdminUserName);
                Assert.True(result);
                rootCategoryId = category.Id;

                var except = messageService.GetMessageCategoryById(rootCategoryId);
                Assert.Equal(rootCategoryId.ToString(), except.TreeCode);
                Assert.Equal(1, except.Level);
            }

            int level2Id = 0;
            //测试新增二级分类
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var messageService = scope.ServiceProvider.GetService<IMessageService>();
                var category = new MessageCategoryDTO()
                {
                    ParentId = rootCategoryId,
                    Text = "【测试Category】" + Guid.NewGuid(),
                };
                var result = await messageService.SaveMessageCategoryAsync(category, RoleConstants.AdminUserId, RoleConstants.AdminUserName);
                Assert.True(result);
                level2Id = category.Id;

                var except = messageService.GetMessageCategoryById(level2Id);
                Assert.Equal(rootCategoryId + "-" + level2Id, except.TreeCode);
                Assert.Equal(2, except.Level);
            }

            //测试修改
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var messageService = scope.ServiceProvider.GetService<IMessageService>();
                var category = new MessageCategoryDTO()
                {
                    Id = rootCategoryId,
                    ParentId = null,
                    Text = "【修改-测试Category】" + Guid.NewGuid(),
                };
                var result = await messageService.SaveMessageCategoryAsync(category, RoleConstants.AdminUserId, RoleConstants.AdminUserName);
                Assert.True(result);

                var except = messageService.GetMessageCategoryById(rootCategoryId);
                Assert.Equal(category.Id.ToString(), except.TreeCode);
                Assert.Equal(1, except.Level);
            }

            //测试删除
            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var messageService = scope.ServiceProvider.GetService<IMessageService>();
                var result = await messageService.RemoveCategoryByIdAsync(level2Id);
                Assert.True(result);

                result = await messageService.RemoveCategoryByIdAsync(rootCategoryId);
                Assert.True(result);
            }
        }
    }
}
