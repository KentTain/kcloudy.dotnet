using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Util;
using KC.Model.Component.DistributedMsg;
using KC.Service.Component;
using KC.UnitTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace KC.UnitTest.Storage.Component
{
    
    public class AllDistributedMessageTest : KC.UnitTest.Storage.StorageTestBase
    {
        private static ITopicService topicService;//ServicBus
        private static ISubscriptionService subscriptionService;//ServicBus

        private static List<string> subcriptions = new List<string>() { "SubScription-1", "SubScription-2" };

        private ILogger _logger;
        public AllDistributedMessageTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(AllDistributedMessageTest));
        }


        protected override void SetUp()
        {
            base.SetUp();

            topicService = Services.BuildServiceProvider().GetService<ITopicService>();
            subscriptionService = Services.BuildServiceProvider().GetService<ISubscriptionService>();
        }

        [Xunit.Fact]
        public void Test_Default_AddTestTopic()
        {
            var topic = new TestTopic()
            {
                Id = 1,
                Name = "test1",
            };
            var success = topicService.AddTestTopic(topic);
            Assert.True(success);
        }
        [Xunit.Fact]
        public void Test_Default_ProcessTestTopic()
        {
            bool success = subscriptionService.ProcessTestTopic(
                subcriptions,
                result =>
                {
                    bool isSuccess = false;
                    var qInfo = result;
                    _logger.LogInformation(string.Format("TestTopic id: {0}, name: {1}", qInfo.Id, qInfo.Name));

                    return isSuccess;
                },
                failResult =>
                {
                    _logger.LogDebug("Test_ServiceBus_ProcessTestTopic", " fail to process TestTopic message: " + failResult + Environment.NewLine);
                });
            Assert.True(success);
        }

        [Xunit.Fact]
        public void Test_ServiceBus_AddTestTopic()
        {
            var topic2 = new TestTopic()
            {
                Id = 1,
                Name = "test2",
            };
            InjectTenant(BuyTenant);
            Services.AddScoped<ITopicService, TenantTopicService>();
            var topicService = Services.BuildServiceProvider().GetService<ITopicService>();

            var success1 = topicService.AddTestTopic(topic2);
            Assert.True(success1);
        }
        [Xunit.Fact]
        public void Test_ServiceBus_ProcessTestTopic()
        {
            InjectTenant(TestTenant);
            Services.AddScoped<ISubscriptionService, TenantSubscriptionService>();
            var subscriptionService = Services.BuildServiceProvider().GetService<ISubscriptionService>();

            bool success1 = subscriptionService.ProcessTestTopic(
                subcriptions,
                result =>
                {
                    bool isSuccess = false;
                    var qInfo = result;
                    _logger.LogInformation(string.Format("customer1: TestTopic id: {0}, name: {1}", qInfo.Id, qInfo.Name));

                    return isSuccess;
                },
                failResult =>
                {
                    _logger.LogDebug("customer1: Test_ServiceBus_ProcessTestTopic", " fail to process TestTopic message: " + failResult + Environment.NewLine);
                });
            Assert.True(success1);
        }

        [Xunit.Fact]
        public void Test_Redis_AddTestTopic()
        {
            InjectTenant(SaleTenant);
            Services.AddScoped<ITopicService, TenantTopicService>();
            var devdbTopicService = Services.BuildServiceProvider().GetService<ITopicService>();

            var topic1 = new TestTopic()
            {
                Id = 1,
                Name = "test1",
            };
            var success = devdbTopicService.AddTestTopic(topic1);
            Assert.True(success);

            var topic2 = new TestTopic()
            {
                Id = 1,
                Name = "test2",
            };
            var success1 = devdbTopicService.AddTestTopic(topic2);
            Assert.True(success1);
        }
        [Xunit.Fact]
        public void Test_Redis_ProcessTestTopic()
        {
            InjectTenant(SaleTenant);
            Services.AddScoped<ISubscriptionService, TenantSubscriptionService>();
            var subscrService = Services.BuildServiceProvider().GetService<ISubscriptionService>();

            bool success = subscrService.ProcessTestTopic(
                subcriptions,
                result =>
                {
                    bool isSuccess = false;
                    var qInfo = result;
                    _logger.LogInformation(string.Format("TestTopic id: {0}, name: {1}", qInfo.Id, qInfo.Name));

                    return isSuccess;
                },
                failResult =>
                {
                    _logger.LogDebug("Test_ServiceBus_ProcessTestTopic", " fail to process TestTopic message: " + failResult + Environment.NewLine);
                });
            Assert.True(success);

            bool success1 = subscrService.ProcessTestTopic(
                subcriptions,
                result =>
                {
                    bool isSuccess = false;
                    var qInfo = result;
                    _logger.LogInformation(string.Format("customer1: TestTopic id: {0}, name: {1}", qInfo.Id, qInfo.Name));

                    return isSuccess;
                },
                failResult =>
                {
                    _logger.LogDebug("customer1: Test_ServiceBus_ProcessTestTopic", " fail to process TestTopic message: " + failResult + Environment.NewLine);
                });
            Assert.True(success1);
        }

        [Xunit.Fact]
        public void Test_Kafka_AddTestTopic()
        {
            var topic1 = new TestTopic()
            {
                Id = 1,
                Name = "test1",
            };
            InjectTenant(DbaTenant);
            Services.AddScoped<ITopicService, TenantTopicService>();
            var topicService = Services.BuildServiceProvider().GetService<ITopicService>();

            var success = topicService.AddTestTopic(topic1);
            Assert.True(success);

            var topic2 = new TestTopic()
            {
                Id = 1,
                Name = "test2",
            };
            var success1 = topicService.AddTestTopic(topic2);
            Assert.True(success1);
        }
        [Xunit.Fact]
        public void Test_Kafka_ProcessTestTopic()
        {
            InjectTenant(DbaTenant);
            Services.AddScoped<ISubscriptionService, TenantSubscriptionService>();
            var subscriptionService = Services.BuildServiceProvider().GetService<ISubscriptionService>();

            bool success = subscriptionService.ProcessTestTopic(
                subcriptions,
                result =>
                {
                    bool isSuccess = false;
                    var qInfo = result;
                    _logger.LogInformation(string.Format("TestTopic id: {0}, name: {1}", qInfo.Id, qInfo.Name));

                    return isSuccess;
                },
                failResult =>
                {
                    _logger.LogDebug("Test_Kafka_ProcessTestTopic", " fail to process TestTopic message: " + failResult + Environment.NewLine);
                });
            Assert.True(success);

            bool success1 = subscriptionService.ProcessTestTopic(
                subcriptions,
                result =>
                {
                    bool isSuccess = false;
                    var qInfo = result;
                    _logger.LogInformation(string.Format("dba: TestTopic id: {0}, name: {1}", qInfo.Id, qInfo.Name));

                    return isSuccess;
                },
                failResult =>
                {
                    _logger.LogDebug("dba: Test_Kafka_ProcessTestTopic", " fail to process TestTopic message: " + failResult + Environment.NewLine);
                });
            Assert.True(success1);
        }
    }
}
