using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Component.Base;
using KC.Component.IRepository;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace KC.Component.QueueRepository
{
    /* for Confluent.Kafka 0.11.6
    public abstract class KafkaQueueRepository<T> : IQueueRepository<T> where T : QueueEntity
    {
        private const string ServiceName = "KC.Component.QueueRepository.KafkaQueueRepository";
        private const int MaxThreadCount = 100;
        private const int TreadSleepMinuteTime = 100;
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
                    ContractResolver = new CustomResolver(),
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
        public string QueueName { get; private set; }
        public Tenant Tenant { get; private set; }

        public Dictionary<string, object> ProducerConfig { get; private set; }
        public Dictionary<string, object> ConsumerConfig { get; private set; }

        protected KafkaQueueRepository(string connectionString)
            : this(connectionString, null)
        {
        }

        protected KafkaQueueRepository(Tenant tenant)
            : this(tenant.GetQueueConnectionString(), tenant.TenantName)
        {
            Tenant = tenant;
        }

        private KafkaQueueRepository(string connectionString, string tenantName)
        {
            QueueName = !string.IsNullOrWhiteSpace(tenantName)
                ? (tenantName + "-" + typeof(T).Name).ToLower()
                : "com-" + typeof(T).Name.ToLower();

            var keyValues = connectionString.KeyValuePairFromConnectionString();
            var endpoint = keyValues[ConnectionKeyConstant.QueueEndpoint];
            var topic = keyValues[ConnectionKeyConstant.AccessName];
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("AWS S3's Kafka Queue string is wrong. It can't set the Endpoint Value from service: " + ServiceName, "connectionString");

            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("AWS S3's Kafka Queue string is wrong. It can't set the TopicName Value from service: " + ServiceName, "connectionString");

            ProducerConfig = new Dictionary<string, object>
            {
                { "group.id", QueueName },
                { "bootstrap.servers", endpoint },
                { "api.version.request", false},
                { "broker.version.fallback", "0.9.0.1"}
            };

            ConsumerConfig = new Dictionary<string, object>
            {
                { "group.id", QueueName },
                { "bootstrap.servers", endpoint },
                { "enable.auto.commit", true },
                { "api.version.request", false},
                { "broker.version.fallback", "0.9.0.1"},
                { "auto.commit.interval.ms", 5000 },
                { "statistics.interval.ms", 60000 },
                { "default.topic.config", new Dictionary<string, object>()
                    {
                        { "auto.offset.reset", "smallest" }
                    }
                }
            };
        }

        /// <summary>
        /// 处理所有消息队列列表
        /// </summary>
        /// <param name="callback">处理每个消息队列的方法</param>
        /// <param name="failCallback">队列发生错误后的错误处理方法</param>
        /// <returns></returns>
        public bool ProcessQueueList(Func<T, bool> callback, Action<T> failCallback)
        {
            //LogUtil.LogInfo(ServiceName + " start to process Queue List....... ");

            using (var consumer = new Consumer<Null, string>(ConsumerConfig, null, new Confluent.Kafka.Serialization.StringDeserializer(Encoding.UTF8)))
            
            {
                //consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(QueueName, 0, 0) });
                var threadId = Thread.CurrentThread.ManagedThreadId;

                #region Consumer Event Handler

                consumer.OnError += (sender, error) =>
                    LogUtil.LogError(
                        string.Format(ServiceName + ": Thread [{0}] Kafka Error: {1} from topic: {2}",
                            threadId,
                            error != null
                                ? string.Format("Error code is {0}, Reason is {1}", error.Code, error.Reason)
                                : string.Empty,
                            QueueName));
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

                consumer.Subscribe(QueueName);

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

                    if (callback == null) break;

                    var strObject = message.Value;
                    var queueObject = JsonConvert.DeserializeObject<T>(strObject);

                    var committedOffsets = consumer.CommitAsync(message).Result;
                    //LogUtil.LogDebug(string.Format(ServiceName + ": Topic: {0} Committed offset: {1} {2}",
                    //    message.Topic, committedOffsets, queueObject.Id));

                    if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) continue;

                    LogUtil.LogInfo(ServiceName + " begin to process Queue....... " + queueObject.Id);
                    var success = callback(queueObject);
                    //LogUtil.LogInfo(ServiceName + " end to process Queue....... " + queueObject.Id);
                    if (success)
                    {
                        if (queueObject.IsManuallyDelete)
                        {
                            //LogUtil.LogInfo(ServiceName + " begin to delete Queue....... " + queueObject.QueueName);
                            queueObject.ErrorCount = 0;
                            AddMessage(queueObject);
                            //LogUtil.LogInfo(ServiceName + " end to delete Queue....... " + queueObject.QueueName);
                        }
                    }
                    else
                    {
                        //LogUtil.LogInfo(ServiceName + " process queue has happend errors....... " + queueObject.Id);
                        //queueObject.Id = Guid.NewGuid().ToString();
                        queueObject.ErrorCount = queueObject.ErrorCount + 1;
                        AddMessage(queueObject);

                        //最后一次执行Queue操作失败后，执行回调函数
                        if (failCallback != null
                            && queueObject.ErrorCount == queueObject.MaxProcessErrorCount - 1)
                        {
                            failCallback(queueObject);
                        }
                    }
                }
                #endregion

                return true;
            }
        }

        /// <summary>
        /// 处理单个消息队列
        /// </summary>
        /// <param name="callback">处理单个消息队列的方法</param>
        /// <param name="failCallback">队列发生错误后的错误处理方法</param>
        /// <returns></returns>
        public bool ProcessQueue(Func<T, bool> callback, Action<T> failCallback)
        {
            //LogUtil.LogInfo(ServiceName + " start to process Queue....... ");

            var success = true;
            using (var consumer = new Consumer<Null, string>(ConsumerConfig, null, new Confluent.Kafka.Serialization.StringDeserializer(Encoding.UTF8)))
            {
                var isTop = true;
                var threadId = Thread.CurrentThread.ManagedThreadId;

                #region Consumer Event Handler

                consumer.OnError += (sender, error) =>
                    LogUtil.LogError(
                        string.Format(ServiceName + ": Thread [{0}] Kafka Error: {1} from topic: {2}",
                            threadId,
                            error != null
                                ? string.Format("Error code is {0}, Reason is {1}", error.Code, error.Reason)
                                : string.Empty,
                            QueueName));
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

                consumer.Subscribe(QueueName);
                while (isTop)
                {
                    Message<Null, string> message;
                    if (!consumer.Consume(out message, TimeSpan.FromMilliseconds(100)))
                        continue;

                    isTop = false;

                    if (callback == null) break;

                    var strObject = message.Value;
                    var queueObject = JsonConvert.DeserializeObject<T>(strObject);

                    var committedOffsets = consumer.CommitAsync(message).Result;
                    //LogUtil.LogDebug(string.Format(ServiceName + ": Topic: {0} Committed offset: {1} {2}",
                    //    message.Topic, committedOffsets, queueObject.Id));

                    if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) break;

                    //LogUtil.LogInfo(ServiceName + " begin to process Queue....... " + queueObject.QueueName);
                    success = callback(queueObject);
                    //LogUtil.LogInfo(ServiceName + " end to process Queue....... " + queueObject.QueueName);

                    if (success)
                    {
                        if (queueObject.IsManuallyDelete)
                        {
                            //LogUtil.LogInfo(ServiceName + " begin to delete Queue....... " + queueObject.QueueName);
                            queueObject.ErrorCount = 0;
                            AddMessage(queueObject);
                            //LogUtil.LogInfo(ServiceName + " end to delete Queue....... " + queueObject.QueueName);
                        }
                    }
                    else
                    {
                        //LogUtil.LogInfo(ServiceName + " process queue has happend errors....... " + queueObject.Id);
                        //queueObject.Id = Guid.NewGuid().ToString();
                        queueObject.ErrorCount = queueObject.ErrorCount + 1;
                        AddMessage(queueObject);

                        //最后一次执行Queue操作失败后，执行回调函数
                        if (failCallback != null
                            && queueObject.ErrorCount == queueObject.MaxProcessErrorCount - 1)
                        {
                            failCallback(queueObject);
                        }
                    }
                }
            }

            return success;
        }

        public long GetMessageCount()
        {
            using (var producer = new Producer<Null, string>(ProducerConfig, null, new Confluent.Kafka.Serialization.StringSerializer(Encoding.UTF8)))
            {
                var metadata = producer.GetMetadata(true, QueueName);
                if (metadata != null)
                    return metadata.Topics.Count;

                return 0;
            }
        }

        public void AddMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            // Create a message and add it to the queue.
            entity.QueueName = QueueName;
            entity.QueueType = QueueType.Kafka;
            var jsonObject = JsonConvert.SerializeObject(entity, serializerSettings);

            using (var producer = new Producer<Null, string>(ProducerConfig, null, new Confluent.Kafka.Serialization.StringSerializer(Encoding.UTF8)))
            {
                var deliveryReport = producer.ProduceAsync(QueueName, null, jsonObject).Result;
                // Tasks are not waited on synchronously (ContinueWith is not synchronous),
                // so it's possible they may still in progress here.
                producer.Flush(1000);
            }

            LogUtil.LogDebug(string.Format("{0}: Tenant[{1}] insert Queue: {2} {3} is success! errorcount={4}.",
                ServiceName, TenantName, QueueName, entity.Id, entity.ErrorCount));
        }

        public void ModifyMessage(T entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllMessage()
        {
            using (var consumer = new Consumer<Null, string>(ConsumerConfig, null, new Confluent.Kafka.Serialization.StringDeserializer(Encoding.UTF8)))
            {
                consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(QueueName, 0, 0) });
                while (true)
                {
                    Message<Null, string> message;
                    if (consumer.Consume(out message, 5000))
                    {
                        LogUtil.LogDebug(string.Format("Topic: {0} Partition: {1} Offset: {2} {3}",
                            message.Topic, message.Partition, message.Offset, message.Value));
                    }
                }
            }
        }

        public void RemoveTopMessage()
        {
            using (var consumer = new Consumer<Null, string>(ConsumerConfig, null, new Confluent.Kafka.Serialization.StringDeserializer(Encoding.UTF8)))
            {
                var isTop = true;
                consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(QueueName, 0, 0) });
                while (isTop)
                {
                    Message<Null, string> message;
                    if (consumer.Consume(out message, 5000))
                    {
                        isTop = false;
                        LogUtil.LogDebug(string.Format("Topic: {0} Partition: {1} Offset: {2} {3}",
                            message.Topic, message.Partition, message.Offset, message.Value));
                    }
                }
            }
        }

        public void RemoveMessage(T entity)
        {
            throw new NotImplementedException();
        }
    }
    */

    /// <summary>
    /// https://github.com/confluentinc/confluent-kafka-dotnet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class KafkaQueueRepository<T> : IQueueRepository<T> where T : QueueEntity
    {
        private const string ServiceName = "KC.Component.QueueRepository.KafkaQueueRepository";
        private const int MaxThreadCount = 100;
        private const int TreadSleepMinuteTime = 100;
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
                    ContractResolver = new CustomResolver(),
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
        public string QueueName { get; private set; }
        public Tenant Tenant { get; private set; }

        public ProducerConfig ProducerConfig { get; private set; }
        public ConsumerConfig ConsumerConfig { get; private set; }

        protected KafkaQueueRepository(string connectionString)
            : this(connectionString, null)
        {
        }

        protected KafkaQueueRepository(Tenant tenant)
            : this(tenant.GetDecryptQueueConnectionString(), tenant.TenantName)
        {
            Tenant = tenant;
        }

        private KafkaQueueRepository(string connectionString, string tenantName)
        {
            QueueName = !string.IsNullOrWhiteSpace(tenantName)
                ? (tenantName + "-" + typeof(T).Name).ToLower()
                : "kc-" + typeof(T).Name.ToLower();

            var keyValues = connectionString.KeyValuePairFromConnectionString();
            var endpoint = keyValues[ConnectionKeyConstant.QueueEndpoint];
            var topic = keyValues[ConnectionKeyConstant.AccessName];
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("AWS S3's Kafka Queue string is wrong. It can't set the Endpoint Value from service: " + ServiceName, "connectionString");

            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("AWS S3's Kafka Queue string is wrong. It can't set the TopicName Value from service: " + ServiceName, "connectionString");


            ProducerConfig = new ProducerConfig
            {
                ClientId = QueueName,
                BootstrapServers = endpoint,
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

            ConsumerConfig = new ConsumerConfig
            {
                GroupId = QueueName,
                BootstrapServers = endpoint,
                
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
        }

        /// <summary>
        /// 处理所有消息队列列表
        /// </summary>
        /// <param name="callback">处理每个消息队列的方法</param>
        /// <param name="failCallback">队列发生错误后的错误处理方法</param>
        /// <returns></returns>
        public bool ProcessQueueList(Func<T, QueueActionType> callback, Action<T> failCallback)
        {
            throw new NotImplementedException();
        }
        public Task<bool> ProcessQueueListAsync(Func<T, QueueActionType> callback, Action<T> failCallback)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 处理单个消息队列
        /// </summary>
        /// <param name="callback">处理单个消息队列的方法</param>
        /// <param name="failCallback">队列发生错误后的错误处理方法</param>
        /// <returns></returns>
        public bool ProcessQueue(Func<T, QueueActionType> callback, Action<T> failCallback)
        {
            //LogUtil.LogInfo(ServiceName + " start to process Queue....... ");

            var success = true;
            using (var consumer = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;

                try
                {
                    var message = consumer.Consume();
                    
                    var strObject = message.Value;
                    var queueObject = JsonConvert.DeserializeObject<T>(strObject);

                    //LogUtil.LogDebug(string.Format(ServiceName + ": Topic: {0} Committed offset: {1} {2}",
                    //    message.Topic, committedOffsets, queueObject.Id));

                    if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) return true;

                    //LogUtil.LogInfo(ServiceName + " begin to process Queue....... " + queueObject.QueueName);
                    var queueActionType = callback(queueObject);
                    //LogUtil.LogInfo(ServiceName + " end to process Queue....... " + queueObject.QueueName);

                    if (queueActionType == QueueActionType.DeleteAfterExecuteAction)
                    {
                        if (queueObject.IsManuallyDelete)
                        {
                            //LogUtil.LogInfo(ServiceName + " begin to delete Queue....... " + queueObject.QueueName);
                            queueObject.ErrorCount = 0;
                            AddMessage(queueObject);
                            //LogUtil.LogInfo(ServiceName + " end to delete Queue....... " + queueObject.QueueName);
                        }
                    }
                    else if (queueActionType == QueueActionType.FailedRepeatActon)
                    {
                        //LogUtil.LogInfo(ServiceName + " process queue has happend errors....... " + queueObject.Id);
                        //queueObject.Id = Guid.NewGuid().ToString();
                        queueObject.ErrorCount = queueObject.ErrorCount + 1;
                        AddMessage(queueObject);

                        //最后一次执行Queue操作失败后，执行回调函数
                        if (failCallback != null
                            && queueObject.ErrorCount == queueObject.MaxProcessErrorCount - 1)
                        {
                            failCallback(queueObject);
                        }
                    }
                    else if (queueActionType == QueueActionType.KeepQueueAction)
                    {
                        queueObject.ErrorCount = 0;
                        AddMessage(queueObject);
                    }
                    
                }
                catch (ConsumeException e)
                {
                    LogUtil.LogError(
                        string.Format(ServiceName + ": Thread [{0}] Kafka Error: {1} from topic: {2}",
                            threadId,
                            e.Error != null
                                ? string.Format("Error code is {0}, Reason is {1}", e.Error.Code, e.Error.Reason)
                                : string.Empty,
                            QueueName));
                }

                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                consumer.Close();

            }

            return success;
        }
        public async Task<bool> ProcessQueueAsync(Func<T, QueueActionType> callback, Action<T> failCallback)
        {
            //LogUtil.LogInfo(ServiceName + " start to process Queue....... ");

            var success = true;
            using (var consumer = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;

                try
                {
                    var message = consumer.Consume();

                    var strObject = message.Value;
                    var queueObject = JsonConvert.DeserializeObject<T>(strObject);

                    //LogUtil.LogDebug(string.Format(ServiceName + ": Topic: {0} Committed offset: {1} {2}",
                    //    message.Topic, committedOffsets, queueObject.Id));

                    if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) return true;

                    //LogUtil.LogInfo(ServiceName + " begin to process Queue....... " + queueObject.QueueName);
                    var queueActionType = callback(queueObject);
                    //LogUtil.LogInfo(ServiceName + " end to process Queue....... " + queueObject.QueueName);

                    if (queueActionType == QueueActionType.DeleteAfterExecuteAction)
                    {
                        if (queueObject.IsManuallyDelete)
                        {
                            //LogUtil.LogInfo(ServiceName + " begin to delete Queue....... " + queueObject.QueueName);
                            queueObject.ErrorCount = 0;
                            AddMessage(queueObject);
                            //LogUtil.LogInfo(ServiceName + " end to delete Queue....... " + queueObject.QueueName);
                        }
                    }
                    else if (queueActionType == QueueActionType.FailedRepeatActon)
                    {
                        //LogUtil.LogInfo(ServiceName + " process queue has happend errors....... " + queueObject.Id);
                        //queueObject.Id = Guid.NewGuid().ToString();
                        queueObject.ErrorCount = queueObject.ErrorCount + 1;
                        AddMessage(queueObject);

                        //最后一次执行Queue操作失败后，执行回调函数
                        if (failCallback != null
                            && queueObject.ErrorCount == queueObject.MaxProcessErrorCount - 1)
                        {
                            failCallback(queueObject);
                        }
                    }
                    else if (queueActionType == QueueActionType.KeepQueueAction)
                    {
                        queueObject.ErrorCount = 0;
                        AddMessage(queueObject);
                    }

                }
                catch (ConsumeException e)
                {
                    LogUtil.LogError(
                        string.Format(ServiceName + ": Thread [{0}] Kafka Error: {1} from topic: {2}",
                            threadId,
                            e.Error != null
                                ? string.Format("Error code is {0}, Reason is {1}", e.Error.Code, e.Error.Reason)
                                : string.Empty,
                            QueueName));
                }

                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                consumer.Close();

            }

            return success;
        }
        public long GetMessageCount()
        {
            throw new NotImplementedException();
        }

        public void AddMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            // Create a message and add it to the queue.
            entity.QueueName = QueueName;
            entity.QueueType = QueueType.Kafka;
            var jsonObject = JsonConvert.SerializeObject(entity, serializerSettings);

            using (var producer = new ProducerBuilder<Null, string>(ProducerConfig).Build())
            {
                producer.ProduceAsync("dotnet-test-topic", new Message<Null, string> { Value = "test value" })
                    .ContinueWith(task => task.IsFaulted
                        ? string.Format("{0}: Tenant[{1}] delivered message: {2} {3}  Error: {4}",
                    ServiceName, TenantName, QueueName, entity.Id, task.Exception.Message)
                        : string.Format("{0}: Tenant[{1}] delivered message: {2} {3}  to {4}",
                    ServiceName, TenantName, QueueName, entity.Id, task.Result.TopicPartitionOffset));

                // wait for up to 10 milliseconds for any inflight messages to be delivered.
                producer.Flush(TimeSpan.FromSeconds(10));
            }

            LogUtil.LogDebug(string.Format("{0}: Tenant[{1}] insert Queue: {2} {3} is success! errorcount={4}.",
                ServiceName, TenantName, QueueName, entity.Id, entity.ErrorCount));
        }

        public void ModifyMessage(T entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllMessage()
        {
            throw new NotImplementedException();
        }

        public void RemoveTopMessage()
        {
            throw new NotImplementedException();
        }

        public void RemoveMessage(T entity)
        {
            throw new NotImplementedException();
        }
    }

    public class KafkaCommonQueueRepository<T> : KafkaQueueRepository<T> where T : QueueEntity
    {
        private static readonly string QueueConnectionString = KC.Framework.Base.GlobalConfig.QueueConnectionString;
        public KafkaCommonQueueRepository()
            : base(QueueConnectionString)
        {
        }

        public KafkaCommonQueueRepository(string queueConnectionString)
            : base(queueConnectionString)
        {
        }

        public KafkaCommonQueueRepository(Tenant tenant)
            : base(tenant)
        {

        }
    }
}
