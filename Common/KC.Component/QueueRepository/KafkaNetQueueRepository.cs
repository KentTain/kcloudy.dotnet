using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Com.Common.Extension;
using Com.Common.Util;
using Com.Model.Core.Tenant;
using Com.Model.Storage.Base;
using Com.Storage.Core.IRepository;
using KafkaNet;
using KafkaNet.Model;
using KafkaNet.Protocol;
using Newtonsoft.Json;

namespace Com.Storage.Core.QueueRepository
{
    public abstract class KafkaNetQueueRepository<T> : IQueueRepository<T> where T : QueueEntity
    {
        private const string ServiceName = "Com.Domain.Core.QueueRepository.KafkaNetQueueRepository";
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

        public KafkaOptions Options { get; private set; }
        public Dictionary<string, object> ConsumerConfig { get; private set; }

        protected KafkaNetQueueRepository(string connectionString)
            : this(connectionString, null)
        {
        }

        protected KafkaNetQueueRepository(Tenant tenant)
            : this(tenant.QueueConnectionString, tenant.TenantName)
        {
            Tenant = tenant;
        }

        private KafkaNetQueueRepository(string connectionString, string tenantName)
        {
            QueueName = !string.IsNullOrWhiteSpace(tenantName)
                ? (tenantName + "-" + typeof(T).Name).ToLower()
                : "com-" + typeof(T).Name.ToLower();

            var keyValues = connectionString.KeyValuePairFromConnectionString();
            var endpoint = keyValues["queueendpoint"];
            var topic = keyValues["accountname"];
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("AWS S3's Kafka Queue string is wrong. It can't set the Endpoint Value from service: " + ServiceName, "connectionString");

            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("AWS S3's Kafka Queue string is wrong. It can't set the TopicName Value from service: " + ServiceName, "connectionString");

            Options = new KafkaOptions(new Uri(endpoint));
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
            var success = true;
            var router = new BrokerRouter(Options);
            var consumer = new Consumer(new ConsumerOptions(QueueName, router));
            foreach (var message in consumer.Consume())
            {
                if (callback == null) continue;

                LogUtil.LogDebug(string.Format(ServiceName + ": Topic: {0} Partition: {1} Offset: {2} {3}",
                        message.Key, message.Meta.PartitionId, message.Meta.Offset, message.Value));

                var strObject = System.Text.Encoding.UTF8.GetString(message.Value);
                var queueObject = JsonConvert.DeserializeObject<T>(strObject);

                if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) return true;

                //LogUtil.LogInfo(ServiceName + " begin to process Queue....... " + queueObject.QueueName);
                success = callback(queueObject);
                //LogUtil.LogInfo(ServiceName + " end to process Queue....... " + queueObject.QueueName);
                if (!success)
                {
                    //LogUtil.LogInfo(ServiceName + " process queue has happend errors....... " + queueObject.QueueName);
                    queueObject.ErrorCount++;
                    AddMessage(queueObject);

                    //最后一次执行Queue操作失败后，执行回调函数
                    if (failCallback != null
                        && queueObject.ErrorCount == queueObject.MaxProcessErrorCount - 1)
                    {
                        failCallback(queueObject);
                    }
                }
            }
            
            return success;
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

            if (callback == null) return true;

            var success = true;
            var router = new BrokerRouter(Options);
            var consumer = new Consumer(new ConsumerOptions(QueueName, router));
            var message = consumer.Consume().FirstOrDefault();
            if (message == null) return true;

            var strObject = System.Text.Encoding.UTF8.GetString(message.Value);
            var queueObject = JsonConvert.DeserializeObject<T>(strObject);

            if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) return true;

            //LogUtil.LogInfo(ServiceName + " begin to process Queue....... " + queueObject.QueueName);
            success = callback(queueObject);
            //LogUtil.LogInfo(ServiceName + " end to process Queue....... " + queueObject.QueueName);
            if (!success)
            {
                //LogUtil.LogInfo(ServiceName + " process queue has happend errors....... " + queueObject.QueueName);
                queueObject.ErrorCount++;
                AddMessage(queueObject);

                //最后一次执行Queue操作失败后，执行回调函数
                if (failCallback != null
                    && queueObject.ErrorCount == queueObject.MaxProcessErrorCount - 1)
                {
                    failCallback(queueObject);
                }
            }

            return success;
        }

        public long GetMessageCount()
        {
            var success = true;
            var router = new BrokerRouter(Options);
            var consumer = new Consumer(new ConsumerOptions(QueueName, router));

            return consumer.ConsumerTaskCount;
        }

        public void AddMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            var serializerSettings = new Newtonsoft.Json.JsonSerializerSettings
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

            // Create a message and add it to the queue.
            entity.QueueName = QueueName;
            string jsonObject = JsonConvert.SerializeObject(entity, serializerSettings);
            var router = new BrokerRouter(Options);
            var producer = new Producer(router);
            producer.SendMessageAsync(QueueName, new[] { new Message(jsonObject) }).Wait();

            LogUtil.LogDebug(string.Format("{0}: Tenant（{1}） insert Queue: {2} is success! ", ServiceName, TenantName, QueueName));
        }

        public void ModifyMessage(T entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllMessage()
        {
            var router = new BrokerRouter(Options);
            using (var consumer = new Consumer(new ConsumerOptions(QueueName, router)))
            {
                foreach (var message in consumer.Consume())
                {
                    LogUtil.LogDebug(string.Format("Topic: {0} Partition: {1} Offset: {2} {3}",
                            message.Key, message.Meta.PartitionId, message.Meta.Offset, message.Value));
                }
            }
        }

        public void RemoveTopMessage()
        {
            var router = new BrokerRouter(Options);
            using (var consumer = new Consumer(new ConsumerOptions(QueueName, router)))
            {
                Message message = consumer.Consume().FirstOrDefault();
                if (message != null)
                    LogUtil.LogDebug(string.Format("Topic: {0} Partition: {1} Offset: {2} {3}",
                        message.Key, message.Meta.PartitionId, message.Meta.Offset, message.Value));
            }
        }
    }

    public class KafkaNetCommonQueueRepository<T> : KafkaNetQueueRepository<T> where T : QueueEntity
    {
        private static readonly string QueueConnectionString = ConfigUtil.GetConfigItem("QueueConnectionString");
        public KafkaNetCommonQueueRepository()
            : base(QueueConnectionString)
        {
        }

        public KafkaNetCommonQueueRepository(Tenant tenant)
            : base(tenant)
        {

        }
    }
}
