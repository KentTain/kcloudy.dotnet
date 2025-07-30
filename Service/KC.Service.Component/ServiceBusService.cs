using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Util;
using KC.Framework;
using KC.Framework.Tenant;
using KC.Component.IRepository;
using KC.Component.DistributedMessage;
using KC.Model.Component.DistributedMsg;
using KC.Component.Base;

namespace KC.Service.Component
{
    public interface ITopicService
    {
        bool AddTestTopic(TestTopic topic);

        bool AddCiticElectronicBillTopic(MakeOutElectronicBill report);

        string TestServiceBusConnection(ServiceBusType storageType, string connectionString);
    }
    public class TopicService : ITopicService
    {
        private const string ServiceName = "KC.Service.Component.TopicService";

        #region repository
        protected ITopicRepository<TopicEntity<TestTopic>, TestTopic> TestTopicRepository { get; private set; }

        protected ITopicRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill> MakeOutElectronicBillTopicRepository { get; private set; }

        #endregion

        public TopicService()
        {
            var storageType = KC.Framework.Base.GlobalConfig.BlobStorage.ToLower();
            switch (storageType)
            {
                case "azure":
                    {
                        var testRepository = new ServicBusCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>();
                        TestTopicRepository = testRepository;

                        var makeOutElectronicBillRepository = new ServicBusCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>();
                        MakeOutElectronicBillTopicRepository = makeOutElectronicBillRepository;
                    }
                    break;
                case "cmb":
                    {
                        var testRepository = new KafkaCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>();
                        TestTopicRepository = testRepository;

                        var makeOutElectronicBillRepository = new KafkaCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>();
                        MakeOutElectronicBillTopicRepository = makeOutElectronicBillRepository;
                    }
                    break;
                default:
                    {
                        var testRepository = new RedisCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>();
                        TestTopicRepository = testRepository;

                        var makeOutElectronicBillRepository = new RedisCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>();
                        MakeOutElectronicBillTopicRepository = makeOutElectronicBillRepository;
                    }
                    break;
            }
        }

        protected TopicService(Tenant tenant)
        {
            var storageType = tenant.ServiceBusType;
            switch (storageType)
            {
                case ServiceBusType.ServiceBus:
                    {
                        var testRepository = new ServicBusCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>(tenant);
                        TestTopicRepository = testRepository;

                        var makeOutElectronicBillRepository = new ServicBusCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>(tenant);
                        MakeOutElectronicBillTopicRepository = makeOutElectronicBillRepository;
                    }
                    break;
                case ServiceBusType.Kafka:
                    {
                        var testRepository = new KafkaCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>(tenant);
                        TestTopicRepository = testRepository;

                        var makeOutElectronicBillRepository = new KafkaCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>(tenant);
                        MakeOutElectronicBillTopicRepository = makeOutElectronicBillRepository;
                    }
                    break;

                case ServiceBusType.Redis:
                default:
                    {
                        var testRepository = new RedisCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>(tenant);
                        TestTopicRepository = testRepository;

                        var makeOutElectronicBillRepository = new RedisCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>(tenant);
                        MakeOutElectronicBillTopicRepository = makeOutElectronicBillRepository;
                    }
                    break;
            }
        }

        public bool AddTestTopic(TestTopic topic)
        {
            try
            {
                TestTopicRepository.CreateTopic(new TopicEntity<TestTopic>()
                {
                    TopicContext = topic
                });
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public bool AddCiticElectronicBillTopic(MakeOutElectronicBill topic)
        {
            try
            {
                MakeOutElectronicBillTopicRepository.CreateTopic(new TopicEntity<MakeOutElectronicBill>()
                {
                    TopicContext = topic
                });
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public string TestServiceBusConnection(ServiceBusType storageType, string connectionString)
        {
            ITopicRepository<TopicEntity<TestTopic>, TestTopic> testRepository;
            switch (storageType)
            {
                case ServiceBusType.ServiceBus:
                    {
                        testRepository = new ServicBusCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>(connectionString);
                    }
                    break;
                case ServiceBusType.Kafka:
                    {
                        testRepository = new KafkaCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>(connectionString);
                    }
                    break;

                case ServiceBusType.Redis:
                default:
                    {
                        testRepository = new RedisCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>(connectionString);
                    }
                    break;
            }

            var email = new TestTopic()
            {
                Id = new Random().Next(1, 10000),
                Name = "[测试邮件]测试EmailQueue",
                CreatedTime = DateTime.UtcNow,
            };
            try
            {
                testRepository.CreateTopic(new TopicEntity<TestTopic>()
                {
                    TopicContext = email
                });
                return string.Empty;
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format(
                    "使用服务总线[队列：{0}--{1}]出错; " + Environment.NewLine +
                    "错误消息：{2}; " + Environment.NewLine +
                    "错误详情：{3}",
                    storageType, connectionString, ex.Message, ex.StackTrace);
                LogUtil.LogError(errorMsg);
                return errorMsg;
            }
        }
    }
    public class TenantTopicService : TopicService
    {
        private const string ServiceName = "KC.Service.Component.TenantTopicService";
        public Tenant Tenant { get; private set; }
        public TenantTopicService(Tenant tenant)
            : base(tenant)
        {
            Tenant = tenant;
        }
    }

    public interface ISubscriptionService
    {
        #region Test
        bool ProcessTestTopic(List<string> subscriptions, Func<TestTopic, bool> callback, Action<string> failCallback = null);
        #endregion

        bool ProcessCiticElectronicBillTopic(List<string> subscriptions, Func<MakeOutElectronicBill, bool> callback, Action<string> failCallback = null);
    }
    public class SubscriptionService : ISubscriptionService
    {
        private const string ServiceName = "KC.Service.Component.SubscriptionService";

        #region repository
        protected ISubscriptionRepository<TestTopic> TestSubscriptionRepository { get; private set; }

        protected ISubscriptionRepository<MakeOutElectronicBill> MakeOutElectronicBillSubRepository { get; private set; }

        #endregion

        public SubscriptionService()
        {
            var storageType = KC.Framework.Base.GlobalConfig.BlobStorage.ToLower();
            switch (storageType)
            {
                case "filesystem":
                    {
                        var testRepository = new KafkaCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>();
                        TestSubscriptionRepository = testRepository;

                        var makeOutElectronicBillRepository = new KafkaCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>();
                        MakeOutElectronicBillSubRepository = makeOutElectronicBillRepository;
                    }
                    break;
                case "azure":
                    {
                        var testRepository = new ServicBusCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>();
                        TestSubscriptionRepository = testRepository;

                        var makeOutElectronicBillRepository = new ServicBusCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>();
                        MakeOutElectronicBillSubRepository = makeOutElectronicBillRepository;
                    }
                    break;
                case "cmb":
                    {
                        var testRepository = new KafkaCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>();
                        TestSubscriptionRepository = testRepository;

                        var makeOutElectronicBillRepository = new KafkaCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>();
                        MakeOutElectronicBillSubRepository = makeOutElectronicBillRepository;
                    }
                    break;
                default:
                    {
                        var testRepository = new RedisCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>();
                        TestSubscriptionRepository = testRepository;

                        var makeOutElectronicBillRepository = new RedisCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>();
                        MakeOutElectronicBillSubRepository = makeOutElectronicBillRepository;
                    }
                    break;
            }
        }

        protected SubscriptionService(Tenant tenant)
        {
            var storageType = tenant.ServiceBusType;
            switch (storageType)
            {
                case ServiceBusType.ServiceBus:
                    {
                        var testRepository = new ServicBusCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>(tenant);
                        TestSubscriptionRepository = testRepository;

                        var makeOutElectronicBillRepository = new ServicBusCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>(tenant);
                        MakeOutElectronicBillSubRepository = makeOutElectronicBillRepository;
                    }
                    break;
                case ServiceBusType.Kafka:
                    {
                        var testRepository = new KafkaCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>(tenant);
                        TestSubscriptionRepository = testRepository;

                        var makeOutElectronicBillRepository = new KafkaCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>(tenant);
                        MakeOutElectronicBillSubRepository = makeOutElectronicBillRepository;
                    }
                    break;
                case ServiceBusType.Redis:
                    {
                        var testRepository = new RedisCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>(tenant);
                        TestSubscriptionRepository = testRepository;

                        var makeOutElectronicBillRepository = new RedisCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>(tenant);
                        MakeOutElectronicBillSubRepository = makeOutElectronicBillRepository;
                    }
                    break;
                default:
                    {
                        var testRepository = new ServicBusCommonDisMsgRepository<TopicEntity<TestTopic>, TestTopic>(tenant);
                        TestSubscriptionRepository = testRepository;

                        var makeOutElectronicBillRepository = new ServicBusCommonDisMsgRepository<TopicEntity<MakeOutElectronicBill>, MakeOutElectronicBill>(tenant);
                        MakeOutElectronicBillSubRepository = makeOutElectronicBillRepository;
                    }
                    break;
            }
        }

        #region Test
        public bool ProcessTestTopic(List<string> subscriptions, Func<TestTopic, bool> callback, Action<string> failCallback = null)
        {
            try
            {
                return TestSubscriptionRepository.ProcessTopic(subscriptions, callback, failCallback);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }
        #endregion

        public bool ProcessCiticElectronicBillTopic(List<string> subscriptions, Func<MakeOutElectronicBill, bool> callback, Action<string> failCallback = null)
        {
            try
            {
                return MakeOutElectronicBillSubRepository.ProcessTopic(subscriptions, callback, failCallback);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

    }
    public class TenantSubscriptionService : SubscriptionService
    {
        private const string ServiceName = "KC.Service.Component.TenantSubscriptionService";
        public Tenant Tenant { get; private set; }
        public TenantSubscriptionService(Tenant tenant)
            : base(tenant)
        {
            Tenant = tenant;
        }
    }
}
