using System;
using System.Collections.Generic;
using System.Linq;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Component.Base;
using KC.Model.Component.Queue;
using KC.Model.Component.Table;
using KC.Service.Component;
using KC.UnitTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using KC.Model.Job.Table;

namespace KC.UnitTest.Storage.Component
{
    
    public class AzureTableTest : KC.UnitTest.Storage.StorageTestBase
    {
        private static INoSqlService azureTableService;
        private static INoSqlService BuyAzureTableService;
        private ILogger _logger;
        public AzureTableTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(AzureTableTest));
        }

        protected override void SetUp()
        {
            base.SetUp();

            azureTableService = Services.BuildServiceProvider().GetService<INoSqlService>();//返回调用者

            InjectTenant(TestTenant);
            Services.AddScoped<INoSqlService, NoSqlService>();
            BuyAzureTableService = Services.BuildServiceProvider().GetService<INoSqlService>();
        }

        [Xunit.Fact]
        public void AddQueueErrorMessage_Test()
        {
            #region EmailInfo
            var emailInfo = new EmailInfo()
            {
                QueueType = QueueType.AzureQueue,
                QueueName = typeof(EmailInfo).Name,
                Tenant = TenantConstant.DbaTenantName,
                UserId = RoleConstants.AdminUserId,
                EmailTitle = "Tenant: dba--测试QueueErrorMessage",
                EmailBody = "Tenant: dba--测试：EmailInfo对象生成的QueueErrorMessage",
                SendTo = new List<string>(){"tianchangjun@cfwin.com"},
                SendFrom = "cfwin@cfwin.com",
                IsBodyHtml = true,
                IsManuallyDelete = false,
            };
            var qErrorMessage = new QueueErrorMessageTable()
            {
                Tenant = emailInfo.Tenant,
                SourceFrom = typeof(EmailInfo).FullName,
                QueueType = emailInfo.QueueType.ToString(),
                QueueName = emailInfo.QueueName,
                QueueMessage = emailInfo.GetQueueObjectXml(),
                ErrorMessage = emailInfo.ErrorMessage,
                ErrorFrom = "Tenant: dba--测试EmailInfo"
            };

            var result = azureTableService.AddQueueErrorMessage(qErrorMessage);
            Assert.True(result);
            #endregion

            #region SmsInfo
            var smsInfo = new SmsInfo()
            {
                QueueType = QueueType.AzureQueue,
                QueueName = typeof(SmsInfo).Name,
                Tenant = TenantConstant.BuyTenantName,
                Type = SmsType.Notice,
                SmsContent = "Tenant: Buy--测试: SmsInfo对象生成的QueueErrorMessage",
                Phone = new List<long>() { 17744949695 },
                IsManuallyDelete = false,
            };
            qErrorMessage = new QueueErrorMessageTable()
            {
                Tenant = smsInfo.Tenant,
                SourceFrom = typeof(SmsInfo).FullName,
                QueueType = smsInfo.QueueType.ToString(),
                QueueName = smsInfo.QueueName,
                QueueMessage = smsInfo.GetQueueObjectXml(),
                ErrorMessage = smsInfo.ErrorMessage,
                ErrorFrom = "Tenant: Buy--测试SmsInfo"
            };
            var result1 = BuyAzureTableService.AddQueueErrorMessage(qErrorMessage);
            Assert.True(result1);
            #endregion

            #region UncallEntity
            var uncall = new UncallEntity()
            {
                QueueType = QueueType.AzureQueue,
                QueueName = typeof(UncallEntity).Name,
                Tenant = TenantConstant.DbaTenantName,
                SessionId = Guid.NewGuid().ToString(),
                Caller = "17744949695",
                CallerName = "超级系统管理员",
                CallerExt = "803",
                Becaller = "18011111111",
                BecallerName = "测试",
                IsManuallyDelete = false,
            };
            qErrorMessage = new QueueErrorMessageTable()
            {
                Tenant = uncall.Tenant,
                SourceFrom = typeof(UncallEntity).FullName,
                QueueType = uncall.QueueType.ToString(),
                QueueName = uncall.QueueName,
                QueueMessage = uncall.GetQueueObjectXml(),
                ErrorMessage = uncall.ErrorMessage,
                ErrorFrom = "Tenant: Dba--测试UncallEntity"
            };
            var result3 = azureTableService.AddQueueErrorMessage(qErrorMessage);
            Assert.True(result3);
            #endregion

            #region TenantApplications
            var tenantApps = new TenantApplications()
            {
                QueueType = QueueType.AzureQueue,
                QueueName = typeof (TenantApplications).Name,
                Tenant = TenantConstant.DbaTenantName,
                TenantId = 1,
                TenantName = TenantConstant.DbaTenantName,
                TenantDisplayName = "超级系统管理员",
                AppIds = new List<int>() {1, 2, 3, 4},
                Email = "tianchangjun@cfwin.com",
                EmailTitle = "Tenant: dba--测试QueueErrorMessage",
                EmailContent = "Tenant: dba--测试：EmailInfo对象生成的QueueErrorMessage",
                AdminNewPassword = "123456",
                AdminEmail = "tianchangjun@cfwin.com",
                AdminPhone = "17744949695",
                IsManuallyDelete = false,
            };
            qErrorMessage = new QueueErrorMessageTable()
            {
                Tenant = tenantApps.Tenant,
                SourceFrom = typeof(TenantApplications).FullName,
                QueueType = tenantApps.QueueType.ToString(),
                QueueName = tenantApps.QueueName,
                QueueMessage = tenantApps.GetQueueObjectXml(),
                ErrorMessage = tenantApps.ErrorMessage,
                ErrorFrom = "Tenant: Dba--测试TenantApplications"
            };
            var result2 = azureTableService.AddQueueErrorMessage(qErrorMessage);
            Assert.True(result2);
            #endregion

            #region PenaltyInterestInfo
            var penltyInfo = new PenaltyInterestInfo()
            {
                QueueType = QueueType.AzureQueue,
                QueueName = typeof(PenaltyInterestInfo).Name,
                Tenant = TenantConstant.DbaTenantName,
                TenantId = 1,
                TenantDisplayName = "超级系统管理员",
                OrderType = 1,
                RepaymentRemindId = 1,
                IsLastCaculate = false,
                IsManuallyDelete = false,
            };
            qErrorMessage = new QueueErrorMessageTable()
            {
                Tenant = penltyInfo.Tenant,
                SourceFrom = typeof(PenaltyInterestInfo).FullName,
                QueueType = penltyInfo.QueueType.ToString(),
                QueueName = penltyInfo.QueueName,
                QueueMessage = penltyInfo.GetQueueObjectXml(),
                ErrorMessage = penltyInfo.ErrorMessage,
                ErrorFrom = "Tenant: Dba--测试PenaltyInterestInfo"
            };
            var result4 = azureTableService.AddQueueErrorMessage(qErrorMessage);
            Assert.True(result4);
            #endregion
        }

        [Xunit.Fact]
        public void GetAllQueueErrorMessages_Test()
        {
            var result1 = azureTableService.GetAllQueueErrorMessages();
            Assert.True(result1.Any());

            var result2 = BuyAzureTableService.GetAllQueueErrorMessages();
            Assert.True(result2.Any());
        }

        [Xunit.Fact]
        public void ClearAllQueueErrorMessages_Test()
        {
            var result = azureTableService.ClearAllQueueErrorMessages();
            Assert.True(result);

            var result1 = BuyAzureTableService.ClearAllQueueErrorMessages();
            Assert.True(result1);
        }
    }
}
