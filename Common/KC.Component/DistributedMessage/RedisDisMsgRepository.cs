using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Component.Base;
using KC.Component.IRepository;
using KC.Component.QueueRepository;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KC.Component.Util;

namespace KC.Component.DistributedMessage
{
    public class RedisDisMsgRepository<T, V> : ITopicRepository<T, V>, ISubscriptionRepository<V>
        where T : TopicEntity<V>
        where V : EntityBase
    {
        private const string ServiceName = "KC.Component.DistributedMessage.RedisDisMsgRepository";

        private static RedisHelper redisHelper;

        private static string ConnectionString { get; set; }
        private string TenantName
        {
            get
            {
                return Tenant != null
                    ? Tenant.TenantName
                    : string.Empty;
            }
        }
        public string TopicName { get; private set; }
        public Tenant Tenant { get; private set; }
        public object SerializeHelper { get; private set; }

        protected RedisDisMsgRepository(string connectionString)
            : this(connectionString, null)
        {
        }
        protected RedisDisMsgRepository(Tenant tenant)
            : this(tenant.GetDecryptServiceBusConnectionString(), tenant.TenantName)
        {
            Tenant = tenant;
        }
        private RedisDisMsgRepository(string connectionString, string tenantName)
        {
            TopicName = !string.IsNullOrWhiteSpace(tenantName)
                ? (tenantName + "-topic-" + typeof(V).FullName).ToLower()
                : "com-topic-" + typeof(V).FullName.ToLower();

            //var regex = new Regex(@"^[A-Za-z][A-Za-z0-9]{2,62}$");
            //if (!regex.IsMatch(QueueName))
            //    throw new ArgumentException(string.Format("the QueueName {0} is not match the Azure name rule.", QueueName), "QueueName");

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString", "Redis connection string is Empty.");

            var keyValues = connectionString.KeyValuePairFromConnectionString();
            if (!keyValues.ContainsKey(ConnectionKeyConstant.ServiceBusEndpoint))
                throw new ArgumentException("Redis connection string is wrong. It can't set the Endpoint Value from service: " + ServiceName, "connectionString");

            var endpoint = keyValues[ConnectionKeyConstant.ServiceBusEndpoint];
            var topic = keyValues[ConnectionKeyConstant.ServiceBusAccessName];
            var key = keyValues[ConnectionKeyConstant.ServiceBusAccessKey];

            //ConnectionString = string.Format("{0},password={1},ssl=True,abortConnect=False,allowAdmin=true", endpoint, key);

            ConnectionString = key.IsNullOrEmpty() ? endpoint : key + "@" + endpoint;

            var readWriteHosts = new List<string>() { ConnectionString };
            redisHelper = new RedisHelper(12, readWriteHosts);
        }

        public void CreateTopic(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Topic:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        TopicName, MethodBase.GetCurrentMethod()));

            try
            {
                entity.TopicType = ServiceBusType.Redis;
                entity.TopicName = TopicName;

                var jsonObject = JsonConvert.SerializeObject(entity,
                    new Newtonsoft.Json.JsonSerializerSettings
                    {
                        MaxDepth = 5,
                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                        MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                        ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Reuse,
                        DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore,
                        ContractResolver = new CustomResolver(),
                        Formatting = Formatting.Indented
                    });

                var result = redisHelper.Publish(TopicName, jsonObject);

                LogUtil.LogDebug(string.Format("{0}: Tenant（{1}） insert Topic: {2} is success! " + result,
                    ServiceName, "dbo", TopicName));
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("{0}: Tenant（{1}） insert Topic: {2} is failure! Error message: {3}",
                    ServiceName, "dbo", TopicName, ex.Message));
            }
        }

        public bool ProcessTopic(List<string> subscriptions, Func<V, bool> callback, Action<string> failCallback)
        {
            if (callback == null) return true;

            try
            {
                foreach (var substriptionUser in subscriptions)
                {
                    Subscribe(substriptionUser, (channel, message) =>
                    {
                        var strObject = message;
                        var queueObject = JsonConvert.DeserializeObject<V>(strObject);

                        LogUtil.LogInfo(string.Format("{0}: {1} begin to process Queue....... ", ServiceName, substriptionUser));
                        var issuccess = callback(queueObject);
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                if (failCallback != null)
                    failCallback(ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<string, string> handler = null)
        {
            redisHelper.Subscribe(subChannel, handler);
        }
    }

    public class RedisCommonDisMsgRepository<T, V> : RedisDisMsgRepository<T, V>
        where T : TopicEntity<V>
        where V : EntityBase
    {
        public RedisCommonDisMsgRepository()
            : base(GlobalConfig.GetDecryptServiceBusConnectionString())
        {
        }
        public RedisCommonDisMsgRepository(string serviceBusConnectionString)
            : base(serviceBusConnectionString)
        {
        }

        public RedisCommonDisMsgRepository(Tenant tenant)
            : base(tenant)
        {
        }
    }
}
