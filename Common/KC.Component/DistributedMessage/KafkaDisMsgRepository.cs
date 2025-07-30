using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Component.Base;
using KC.Component.IRepository;
using KC.Component.QueueRepository;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace KC.Component.DistributedMessage
{
    /*
    public class KafkaDisMsgRepository<T, V> : ITopicRepository<T, V>, ISubscriptionRepository<V>
        where T : TopicEntity<V>
        where V : EntityBase
    {
        private const string ServiceName = "KC.Component.DistributedMessage.KafkaDisMsgRepository";
        private const int MaxThreadCount = 100;
        private const int TreadSleepMinuteTime = 100;
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
        public string Endpoint { get; private set; }
        public Tenant Tenant { get; private set; }

        protected KafkaDisMsgRepository(string connectionString)
            : this(connectionString, null)
        {
        }

        protected KafkaDisMsgRepository(Tenant tenant)
            : this(tenant.GetServiceBusConnectionString(), tenant.TenantName)
        {
            Tenant = tenant;
        }

        private KafkaDisMsgRepository(string connectionString, string tenantName)
        {
            TopicName = !string.IsNullOrWhiteSpace(tenantName)
                ? (tenantName + "-" + typeof(V).FullName).ToLower()
                : "com-" + typeof(V).FullName.ToLower();

            var keyValues = connectionString.KeyValuePairFromConnectionString();
            var endpoint = keyValues[ConnectionKeyConstant.ServiceBusEndpoint];
            var topic = keyValues[ConnectionKeyConstant.ServiceBusAccessName];
            var key = keyValues[ConnectionKeyConstant.ServiceBusAccessKey];

            Endpoint = endpoint;
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Kafka connection string is wrong. It can't set the Endpoint Value from service: " + ServiceName, "connectionString");

            //if (string.IsNullOrWhiteSpace(topic))
            //    throw new ArgumentException("Kafka connection string is wrong. It can't set the TopicName Value from service: " + ServiceName, "connectionString");
        }

        public void CreateTopic(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Topic:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        TopicName, MethodBase.GetCurrentMethod()));

            try
            {
                // Create a message and add it to the Topic.
                entity.TopicName = TopicName;
                entity.TopicType = ServiceBusType.Kafka;
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
                var producerConfig = new Dictionary<string, object>
                {
                    {"group.id", TopicName},
                    {"bootstrap.servers", Endpoint},
                    {"api.version.request", false},
                    {"broker.version.fallback", "0.9.0.1"}
                };
                using (var producer = new Producer<Null, string>(producerConfig, null, new StringSerializer(Encoding.UTF8)))
                {
                    var deliveryReport = producer.ProduceAsync(TopicName, null, jsonObject).Result;
                    // Tasks are not waited on synchronously (ContinueWith is not synchronous),
                    // so it's possible they may still in progress here.
                    producer.Flush(1000);
                }

                LogUtil.LogDebug(string.Format("{0}: Tenant[{1}] insert Topic: {2} {3} is success!",
                    ServiceName, TenantName, TopicName, entity.Id));
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("{0}: Tenant[{1}] insert Topic: {2} {3} is success! Error message: {4}.",
                    ServiceName, TenantName, TopicName, entity.Id, ex.Message));
            }
        }

        public bool ProcessTopic(List<string> subscriptions, Func<V, bool> callback, Action<string> failCallback)
        {
            if (callback == null) return true;

            try
            {
                foreach (var substriptionUser in subscriptions)
                {
                    var consumerConfig = new Dictionary<string, object>
                    {
                        {"group.id", TopicName},
                        {"bootstrap.servers", Endpoint},
                        {"enable.auto.commit", true},
                        {"api.version.request", false},
                        {"broker.version.fallback", "0.9.0.1"},
                        {"auto.commit.interval.ms", 5000},
                        {"statistics.interval.ms", 60000},
                        {
                            "default.topic.config", new Dictionary<string, object>()
                            {
                                {"auto.offset.reset", "smallest"}
                            }
                        }
                    };
                    using (var consumer = new Consumer<Null, string>(consumerConfig, null, new StringDeserializer(Encoding.UTF8)))
                    {
                        var threadId = Thread.CurrentThread.ManagedThreadId;

                        #region Consumer Event Handler

                        consumer.OnError += (sender, error) =>
                            LogUtil.LogError(
                                string.Format(ServiceName + ": Thread [{0}] Kafka Error: {1} from topic: {2}",
                                    threadId,
                                    error != null
                                        ? string.Format("Error code is {0}, Reason is {1}", error.Code, error.Reason)
                                        : string.Empty,
                                    TopicName));
                        consumer.OnConsumeError += (sender, msg) =>
                            LogUtil.LogError(
                                string.Format(
                                    ServiceName + ": Thread [{0}]  Kafka Error consuming from topic/partition/offset {1}/{2}/{3}: {4}",
                                    threadId, msg.Topic, msg.Partition, msg.Offset,
                                    msg.Error != null
                                        ? string.Format("Error code is {0}, Reason is {1}", msg.Error.Code, msg.Error.Reason)
                                        : string.Empty));
                        //consumer.OnPartitionEOF += (sender, end) =>
                        //    LogUtil.LogDebug(
                        //        string.Format(
                        //            ServiceName + ": Thread [{0}]  Reached end of topic [{1}] partition [{2}], next message will be at offset [{3}]",
                        //            threadId, end.Topic, end.Partition, end.Offset));
                        consumer.OnOffsetsCommitted += (sender, commit) =>
                        {
                            if (commit.Error)
                            {
                                LogUtil.LogError(string.Format(ServiceName + ": Thread [{0}]  Failed to commit offsets: {1}",
                                    threadId, commit.Error != null
                                        ? string.Format("Error code is {0}, Reason is {1}", commit.Error.Code, commit.Error.Reason)
                                        : string.Empty));
                            }
                            LogUtil.LogDebug(string.Format(ServiceName + ": Thread [{0}]  Successfully committed offsets: [{1}]",
                                threadId, commit.Offsets.ToCommaSeparatedStringByFilter(m => m.Offset.Value.ToString())));
                        };
                        #endregion

                        consumer.Subscribe(TopicName);

                        #region //方法一
                        //consumer.OnMessage += (sender, message) =>
                        //{
                        //    LogUtil.LogDebug(string.Format("Topic: {0} Partition: {1} Offset: {2} {3}",
                        //        message.Topic, message.Partition, message.Offset, message.Value));
                        //    var strObject = message.Value;
                        //    var queueObject = JsonConvert.DeserializeObject<T>(strObject);


                        //    if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) return;

                        //    //LogUtil.LogInfo(ServiceName + " begin to process Queue....... " + queueObject.QueueName);
                        //    success = callback(queueObject);
                        //    //LogUtil.LogInfo(ServiceName + " end to process Queue....... " + queueObject.QueueName);
                        //    if (!success)
                        //    {
                        //        //LogUtil.LogInfo(ServiceName + " process queue has happend errors....... " + queueObject.QueueName);
                        //        queueObject.ErrorCount++;
                        //        AddMessage(queueObject);

                        //        //最后一次执行Queue操作失败后，执行回调函数
                        //        if (failCallback != null
                        //            && queueObject.ErrorCount == queueObject.MaxProcessErrorCount - 1)
                        //        {
                        //            failCallback(queueObject);
                        //        }
                        //    }
                        //};
                        //while (true)
                        //{
                        //    consumer.Poll(TimeSpan.FromMilliseconds(100));
                        //}
                        #endregion

                        #region //方法二

                        var threadProcess = 1;
                        while (MaxThreadCount > threadProcess)
                        {
                            threadProcess++;
                            Message<Null, string> message;
                            if (!consumer.Consume(out message, TimeSpan.FromMilliseconds(100)))
                                continue;

                            var strObject = message.Value;
                            var queueObject = JsonConvert.DeserializeObject<V>(strObject);

                            var committedOffsets = consumer.CommitAsync(message).Result;
                            //LogUtil.LogDebug(string.Format(ServiceName + ": Topic: {0} Committed offset: {1} {2}",
                            //    message.Topic, committedOffsets, queueObject.Id));

                            LogUtil.LogInfo(string.Format("{0}: {1} begin to process Queue....... ", ServiceName, substriptionUser));
                            var issuccess = callback(queueObject);

                        }
                        #endregion
                    }
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
    }
    */

    public class KafkaDisMsgRepository<T, V> : ITopicRepository<T, V>, ISubscriptionRepository<V>
    where T : TopicEntity<V>
    where V : EntityBase
    {
        private const string ServiceName = "KC.Component.DistributedMessage.KafkaDisMsgRepository";
        private const int MaxThreadCount = 100;
        private const int TreadSleepMinuteTime = 100;
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
        public string Endpoint { get; private set; }
        public Tenant Tenant { get; private set; }

        protected KafkaDisMsgRepository(string connectionString)
            : this(connectionString, null)
        {
        }

        protected KafkaDisMsgRepository(Tenant tenant)
            : this(tenant.GetDecryptServiceBusConnectionString(), tenant.TenantName)
        {
            Tenant = tenant;
        }

        private KafkaDisMsgRepository(string connectionString, string tenantName)
        {
            TopicName = !string.IsNullOrWhiteSpace(tenantName)
                ? (tenantName + "-" + typeof(V).FullName).ToLower()
                : "com-" + typeof(V).FullName.ToLower();

            var keyValues = connectionString.KeyValuePairFromConnectionString();
            var endpoint = keyValues[ConnectionKeyConstant.ServiceBusEndpoint];
            var topic = keyValues[ConnectionKeyConstant.ServiceBusAccessName];
            var key = keyValues[ConnectionKeyConstant.ServiceBusAccessKey];

            Endpoint = endpoint;
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Kafka connection string is wrong. It can't set the Endpoint Value from service: " + ServiceName, "connectionString");

            //if (string.IsNullOrWhiteSpace(topic))
            //    throw new ArgumentException("Kafka connection string is wrong. It can't set the TopicName Value from service: " + ServiceName, "connectionString");
        }

        public void CreateTopic(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Topic:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        TopicName, MethodBase.GetCurrentMethod()));

            try
            {
                // Create a message and add it to the Topic.
                entity.TopicName = TopicName;
                entity.TopicType = ServiceBusType.Kafka;
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
                var producerConfig = new ProducerConfig
                {
                    ClientId = TopicName,
                    BootstrapServers = Endpoint,
                    BrokerVersionFallback = "0.10.0.0",
                    ApiVersionFallbackMs = 0,
                    //SaslMechanism = SaslMechanism.Plain,
                    //SecurityProtocol = SecurityProtocol.SaslSsl,
                    // On Windows, default trusted root CA certificates are stored in the Windows Registry.
                    // They are not automatically discovered by Confluent.Kafka and it's not possible to
                    // reference them using the `ssl.ca.location` property. You will need to obtain these
                    // from somewhere else, for example use the cacert.pem file distributed with curl:
                    // https://curl.haxx.se/ca/cacert.pem and reference that file in the `ssl.ca.location`
                    // property:
                    //SslCaLocation = "/usr/local/etc/openssl/cert.pem", // suitable configuration for linux, osx.
                    // SslCaLocation = "c:\\path\\to\\cacert.pem", // windows
                    //SaslUsername = "<ccloud key>",
                    //SaslPassword = "<ccloud secret>"
                };
                using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
                {
                    producer.ProduceAsync("dotnet-test-topic", new Message<Null, string> { Value = "test value" })
                    .ContinueWith(task => task.IsFaulted
                        ? string.Format("{0}: Tenant[{1}] delivered message: {2} {3}  Error: {4}",
                    ServiceName, TenantName, TopicName, entity.Id, task.Exception.Message)
                        : string.Format("{0}: Tenant[{1}] delivered message: {2} {3}  to {4}",
                    ServiceName, TenantName, TopicName, entity.Id, task.Result.TopicPartitionOffset));

                    // wait for up to 10 milliseconds for any inflight messages to be delivered.
                    producer.Flush(TimeSpan.FromSeconds(10));
                }

                LogUtil.LogDebug(string.Format("{0}: Tenant[{1}] insert Topic: {2} {3} is success!",
                    ServiceName, TenantName, TopicName, entity.Id));
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("{0}: Tenant[{1}] insert Topic: {2} {3} is success! Error message: {4}.",
                    ServiceName, TenantName, TopicName, entity.Id, ex.Message));
            }
        }

        public bool ProcessTopic(List<string> subscriptions, Func<V, bool> callback, Action<string> failCallback)
        {
            if (callback == null) return true;

            try
            {
                foreach (var substriptionUser in subscriptions)
                {
                    var consumerConfig = new ConsumerConfig
                    {
                        GroupId = TopicName,
                        BootstrapServers = Endpoint,

                        BrokerVersionFallback = "0.10.0.0",
                        ApiVersionFallbackMs = 0,
                        // Note: The AutoOffsetReset property determines the start offset in the event
                        // there are not yet any committed offsets for the consumer group for the
                        // topic/partitions of interest. By default, offsets are committed
                        // automatically, so in this example, consumption will only start from the
                        // earliest message in the topic 'my-topic' the first time you run the program.
                        AutoOffsetReset = AutoOffsetReset.Earliest,
                        //SaslMechanism = SaslMechanism.Plain,
                        //SecurityProtocol = SecurityProtocol.SaslSsl,
                        //SslCaLocation = "/usr/local/etc/openssl/cert.pem", // suitable configuration for linux, osx.
                        // SslCaLocation = "c:\\path\\to\\cacert.pem",     // windows
                        //SaslUsername = "<confluent cloud key>",
                        //SaslPassword = "<confluent cloud secret>",
                    };
                    using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
                    {
                        var threadId = Thread.CurrentThread.ManagedThreadId;
                        
                        consumer.Subscribe(TopicName);

                        try
                        {
                            var message = consumer.Consume();
                            var strObject = message.Value;
                            var queueObject = JsonConvert.DeserializeObject<V>(strObject);
                            
                            //LogUtil.LogDebug(string.Format(ServiceName + ": Topic: {0} Committed offset: {1} {2}",
                            //    message.Topic, committedOffsets, queueObject.Id));

                            LogUtil.LogInfo(string.Format("{0}: {1} begin to process Queue....... ", ServiceName, substriptionUser));
                            var issuccess = callback(queueObject);
                        }
                        catch (ConsumeException e)
                        {
                            LogUtil.LogError(
                                string.Format(ServiceName + ": Thread [{0}] Kafka Error: {1} from topic: {2}",
                                    threadId,
                                    e.Error != null
                                        ? string.Format("Error code is {0}, Reason is {1}", e.Error.Code, e.Error.Reason)
                                        : string.Empty,
                                    TopicName));
                        }

                        consumer.Close();
                        
                    }
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
    }

    public class KafkaCommonDisMsgRepository<T, V> : KafkaDisMsgRepository<T, V>
        where T : TopicEntity<V>
        where V : EntityBase
    {
        private static readonly string serviceBusConnectionString = KC.Framework.Base.GlobalConfig.ServiceBusConnectionString;
        public KafkaCommonDisMsgRepository()
            : base(serviceBusConnectionString)
        {
        }
        public KafkaCommonDisMsgRepository(string serviceBusConnectionString)
            : base(serviceBusConnectionString)
        {
        }
        public KafkaCommonDisMsgRepository(Tenant tenant)
            : base(tenant)
        {

        }
    }
}
