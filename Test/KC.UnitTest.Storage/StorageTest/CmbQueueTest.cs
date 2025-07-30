using System;
using System.Collections.Generic;
using KC.Model.Component.Queue;
using KC.Service.Component;
using KC.UnitTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace KC.UnitTest.Storage.Component
{
    
    public class CmbQueueTest : KC.UnitTest.Storage.StorageTestBase
    {
        private static IStorageQueueService storageQueueService;
        private static IStorageQueueService BuyStorageQueueService;
        private ILogger _logger;
        public CmbQueueTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(CmbQueueTest));
        }

        protected override void SetUp()
        {
            base.SetUp();

            storageQueueService = Services.BuildServiceProvider().GetService<IStorageQueueService>();//返回调用者

            InjectTenant(BuyTenant);
            Services.AddScoped<IStorageQueueService, StorageQueueService>();
            BuyStorageQueueService = Services.BuildServiceProvider().GetService<IStorageQueueService>();//返回调用者
        }

        [Xunit.Fact]
        public void InserEmailQueue_Test()
        {
            var email = new EmailInfo()
            {
                Tenant = TestTenant.TenantName,
                SendTo = new List<string>() { "tianchangjun@cfwin.com" },
                EmailTitle = "[测试邮件]测试EmailQueue",
                EmailBody = "[测试邮件]测试EmailQueue",
                SendFrom = "tianchangjun@cfwin.com",
                IsBodyHtml = false,
            };

            var result = storageQueueService.InsertEmailQueue(email);
            Assert.True(result);

            email.EmailTitle = email.EmailTitle + "--Tenant: DevDB";
            email.EmailBody = email.EmailBody + "--Tenant: DevDB";
            var result1 = BuyStorageQueueService.InsertEmailQueue(email);
            Assert.True(result1);
        }

        [Xunit.Fact]
        public void InserSmsQueue_Test()
        {
            var sms = new SmsInfo()
            {
                Tenant = TestTenant.TenantName,
                Phone = new List<long>() { 17744949695 },
                SmsContent = "测试短信--测试SmsQueue",
            };

            var result = storageQueueService.InsertSmsQueue(sms);
            Assert.True(result);

            sms.SmsContent = sms.SmsContent + "--Tenant: DevDB";
            var result1 = BuyStorageQueueService.InsertSmsQueue(sms);
            Assert.True(result1);
        }

        [Xunit.Fact]
        public void ClearAllEmailQueue_Test()
        {
            var result = storageQueueService.ClearAllEmailQueue();
            Assert.True(result);

            var result1 = BuyStorageQueueService.ClearAllEmailQueue();
            Assert.True(result1);
        }

        [Xunit.Fact]
        public void ClearAllSmsQueue_Test()
        {
            var result = storageQueueService.ClearAllSmsQueue();
            Assert.True(result);

            var result1 = BuyStorageQueueService.ClearAllSmsQueue();
            Assert.True(result1);
        }
    }
}
