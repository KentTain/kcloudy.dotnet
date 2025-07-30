using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Util;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Component.Base;
using KC.Component.IRepository;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;


namespace KC.Component.DistributedMessage
{
    public abstract class ServiceBusDisMsgRepository<T, V> : ITopicRepository<T, V>, ISubscriptionRepository<V> 
        where T : TopicEntity<V>
        where V : EntityBase
    {
        private const string ServiceName = "KC.Component.DistributedMessage.ServiceBusDisMsgRepository";
        private Newtonsoft.Json.JsonSerializerSettings serializerSettings
        {
            get
            {
                return new JsonSerializerSettings()
                {
                    MaxDepth = 5,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                    MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                    ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Reuse,
                    DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore,
                    ContractResolver = new KC.Component.QueueRepository.CustomResolver(),
                    Formatting = Formatting.Indented
                };
            }
        }
        private string TenantName
        {
            get
            {
                return Tenant != null
                    ? Tenant.TenantName
                    : string.Empty;
            }
        }

        public ServiceBusConnectionStringBuilder ConnectionString { get; private set; }
        public string TopicName { get; private set; }
        public Tenant Tenant { get; private set; }
        public QueueClient Queue { get; private set; }

        protected ServiceBusDisMsgRepository(string connectionString)
            : this(connectionString, null)
        {
        }

        protected ServiceBusDisMsgRepository(Tenant tenant)
            : this(tenant.GetDecryptServiceBusConnectionString(), tenant.TenantName)
        {
            Tenant = tenant;
        }

        private ServiceBusDisMsgRepository(string connectionString, string tenantName)
        {
            TopicName = !string.IsNullOrWhiteSpace(tenantName)
                ? (tenantName + "-topic-" + typeof(V).FullName).ToLower()
                : "com-topic-" + typeof(V).FullName.ToLower();

            //var regex = new Regex(@"^[A-Za-z][A-Za-z0-9]{2,62}$");
            //if (!regex.IsMatch(QueueName))
            //    throw new ArgumentException(string.Format("the QueueName {0} is not match the Azure name rule.", QueueName), "QueueName");

            // Configure Queue Settings
            ConnectionString = new ServiceBusConnectionStringBuilder(connectionString)
            {
                EntityPath = TopicName
            };
            var receiveMode = ReceiveMode.ReceiveAndDelete;
            Queue = new QueueClient(ConnectionString, receiveMode, GetRetryPolicy());

        }
        private RetryExponential GetRetryPolicy()
        {
            return new RetryExponential(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), 10);
        }

        public async void CreateTopic(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Topic:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        TopicName, MethodBase.GetCurrentMethod()));

            try
            {
                var client = new TopicClient(ConnectionString, GetRetryPolicy());
                entity.TopicType = ServiceBusType.ServiceBus;
                entity.TopicName = TopicName;
                // Create a message and add it to the Topic.
                var jsonObject = JsonConvert.SerializeObject(entity, serializerSettings);
                await client.SendAsync(new Message(Encoding.UTF8.GetBytes(jsonObject)));

                LogUtil.LogDebug(string.Format("{0}: Tenant（{1}） insert Topic: {2} is success! ", 
                    ServiceName, TenantName, TopicName));
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("{0}: Tenant（{1}） insert Topic: {2} is failure! Error message: {3}", 
                    ServiceName, TenantName, TopicName, ex.Message));
            }
        }

        public bool ProcessTopic(List<string> subscriptions, Func<V, bool> callback, Action<string> failCallback)
        {
            if (callback == null) return true;

            var messages = new List<string>();
            foreach (var substriptionUser in subscriptions)
            {
                var subscriptionClient = new SubscriptionClient(ConnectionString, substriptionUser, ReceiveMode.PeekLock, GetRetryPolicy());

                try
                {
                    subscriptionClient.RegisterMessageHandler(
                        async (message, token) =>
                        {
                            var messageJson = Encoding.UTF8.GetString(message.Body);
                            var entity = JsonConvert.DeserializeObject<V>(messageJson);

                            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

                            var isSuccess = callback(entity);
                        },
                        new MessageHandlerOptions(async args => LogUtil.LogException(args.Exception))
                        { MaxConcurrentCalls = 1, AutoComplete = false });
                }
                catch (Exception e)
                {
                    messages.Add(string.Format("订阅者（{0}）获取所订阅的消息（{1}）失败，错误消息内容为：{2}。/r/n",
                        substriptionUser, TopicName, e.Message));
                }
            }

            if (messages.Any())
            {
                failCallback(messages.ToCommaSeparatedString());
                return false;
            }

            return true;
        }
    }

    public class ServicBusCommonDisMsgRepository<T, V> : ServiceBusDisMsgRepository<T, V>
        where T : TopicEntity<V>
        where V : EntityBase
    {
        private static readonly string serviceBusConnectionString = KC.Framework.Base.GlobalConfig.ServiceBusConnectionString;
        public ServicBusCommonDisMsgRepository()
            : base(serviceBusConnectionString)
        {
        }
        public ServicBusCommonDisMsgRepository(string serviceBusConnectionString)
            : base(serviceBusConnectionString)
        {
        }
        public ServicBusCommonDisMsgRepository(Tenant tenant)
            : base(tenant)
        {
        }
    }
}
