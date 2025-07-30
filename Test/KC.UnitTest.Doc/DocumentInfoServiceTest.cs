using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Doc;
using System.Threading.Tasks;
using KC.Service.DTO.Doc;
using Xunit;
using KC.Framework.Tenant;

namespace KC.UnitTest.Doc
{
    public class DocumentInfoServiceTest : DocTestBase
    {
        private ILogger _logger;
        public DocumentInfoServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DocumentInfoServiceTest));
        }

        [Xunit.Fact]
        public async Task Test_SaveDocCategoryAsync()
        {
            InjectTenant(DbaTenant);

            var model = new DocCategoryDTO();
            model.ParentId = null;
            model.Text = "测试文档分类一";
            model.IsEditMode = false;
            model.IsDeleted = false;
            model.CreatedBy = RoleConstants.AdminUserId;
            model.CreatedName = RoleConstants.AdminUserName;
            model.CreatedDate = DateTime.UtcNow;
            model.ModifiedBy = RoleConstants.AdminUserId;
            model.ModifiedName = RoleConstants.AdminUserName;
            model.ModifiedDate = DateTime.UtcNow;

            //插入数据测试
            using (var scope = ServiceProvider.CreateScope())
            {
                var docService = scope.ServiceProvider.GetService<IDocumentInfoService>();
                var result = await docService.SaveDocCategoryAsync(model);
                var categoryId = model.Id;
                var categoryCode = model.TreeCode;
                _logger.LogDebug("insert CategoryId: " + categoryId + " Code: " + categoryCode + " isSuccess: " + result);
                Assert.True(result);

                var except = docService.GetDocCategoryById(categoryId);
                Assert.Equal(categoryId.ToString(), except.TreeCode);
                Assert.Equal(1, except.Level);
            }

            //更新数据
            using (var scope = ServiceProvider.CreateScope())
            {
                var docService = scope.ServiceProvider.GetService<IDocumentInfoService>();
                model.Text = "测试文档分类一（更新）";
                var result = await docService.SaveDocCategoryAsync(model);
                var categoryId = model.Id;
                var categoryCode = model.TreeCode;
                _logger.LogDebug("update CategoryId: " + categoryId + " Code: " + categoryCode + " isSuccess: " + result);
                Assert.True(result);
            }

            //删除数据
            using (var scope = ServiceProvider.CreateScope())
            {
                var categoryId = model.Id;
                var categoryCode = model.TreeCode;
                var docService = scope.ServiceProvider.GetService<IDocumentInfoService>();
                var success = await docService.RemoveDocCategoryByIdAsync(categoryId);
                Assert.True(success);
            }
        }

    }
}
