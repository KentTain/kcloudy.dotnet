using System;
using System.Reflection;
using Com.Framework.Util;
using Com.Framework.Tenant;
using Com.Model.Storage.Base;
using Com.Storage.Core.IRepository;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;

namespace Com.Storage.Core.QueueRepository
{
    public abstract class ServiceBusQueueRepository<T> : IQueueRepository<T> where T : QueueEntity
    {
        private const string ServiceName = "Com.Storage.Core.QueueRepository.ServiceBusQueueRepository";
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
                    ContractResolver = new Com.Storage.Core.QueueRepository.CustomResolver(),
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
        public QueueClient Queue { get; private set; }
        public string QueueName { get; private set; }
        public Tenant Tenant { get; private set; }

        protected ServiceBusQueueRepository(string connectionString)
            : this(connectionString, null)
        {
        }
        protected ServiceBusQueueRepository(Tenant tenant)
            : this(tenant.GetServiceBusConnectionString(), tenant.TenantName)
        {
            Tenant = tenant;
        }
        private ServiceBusQueueRepository(string connectionString, string tenantName)
        {
            QueueName = !string.IsNullOrWhiteSpace(tenantName)
                ? (tenantName + "-" + typeof(T).Name).ToLower()
                : "com-" + typeof(T).Name.ToLower();

            //var regex = new Regex(@"^[A-Za-z][A-Za-z0-9]{2,62}$");
            //if (!regex.IsMatch(QueueName))
            //    throw new ArgumentException(string.Format("the QueueName {0} is not match the Azure name rule.", QueueName), "QueueName");

            // Configure Queue Settings
            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(connectionString)
            {
                EntityPath = QueueName
            };
            var receiveMode = ReceiveMode.ReceiveAndDelete;
            Queue = new QueueClient(connectionStringBuilder, receiveMode, GetRetryPolicy());
        }

        private static RetryExponential GetRetryPolicy()
        {
            return new RetryExponential(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), 10);
        }

        public static TopicClient GetTopicClient(string topicName = "stockupdated")
        {
            var topicClient = new TopicClient(ConfigurationHelper.ServiceBusConnectionString(), topicName, GetRetryPolicy());
            return topicClient;
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
            BrokeredMessage message;
            while ((message = Queue.Receive()) != null)
            {
                if (callback == null) continue;

                var strObject = message.GetBody<string>();
                var queueObject = JsonConvert.DeserializeObject<T>(strObject);

                //三次执行队列方法出错后，不再执行处理消息队列的方法（callback）
                if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) continue;

                //LogUtil.LogInfo(ServiceName + " begin to process Queue....... " + queueObject.QueueName);
                var isSuccess = callback(queueObject);
                //LogUtil.LogInfo(ServiceName + " end to process Queue....... " + queueObject.QueueName);
                if (isSuccess)
                {
                    //LogUtil.LogInfo(ServiceName + " begin to delete Queue....... " + queueObject.QueueName);
                    message.Complete();
                    //LogUtil.LogInfo(ServiceName + " end to delete Queue....... " + queueObject.QueueName);
                    if (queueObject.IsManuallyDelete)
                    {
                        queueObject.ErrorCount = 0;
                        var jsonObject = JsonConvert.SerializeObject(queueObject);
                        Queue.Send(new BrokeredMessage(jsonObject));
                    }
                }
                else
                {
                    //LogUtil.LogInfo(ServiceName + " process queue has happend errors....... " + queueObject.QueueName);

                    message.Complete();
                    queueObject.ErrorCount++;
                    string jsonObject = JsonConvert.SerializeObject(queueObject);
                    Queue.Send(new BrokeredMessage(jsonObject));

                    success = false;

                    //最后一次执行Queue操作失败后，执行回调函数
                    if (failCallback != null && queueObject.ErrorCount == queueObject.MaxProcessErrorCount)
                    {
                        message.Complete();
                        failCallback(queueObject);
                    }
                }
            }

            //LogUtil.LogInfo(ServiceName + " end to process Queue List...... ");

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
            if (callback == null) return true;

            BrokeredMessage message = Queue.Receive();
            if (message == null) return true;

            var strObject = message.GetBody<string>();
            var queueObject = JsonConvert.DeserializeObject<T>(strObject);

            if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) return true;

            var success = callback(queueObject);
            if (success)
            {
                //LogUtil.LogInfo(ServiceName + " begin to delete Queue....... " + queueObject.QueueName);
                message.Complete();
                //LogUtil.LogInfo(ServiceName + " end to delete Queue....... " + queueObject.QueueName);
                if (queueObject.IsManuallyDelete)
                {
                    queueObject.ErrorCount = 0;
                    var jsonObject = JsonConvert.SerializeObject(queueObject);
                    Queue.Send(new BrokeredMessage(jsonObject));
                }
            }
            else
            {
                message.Complete();
                queueObject.ErrorCount++;
                var jsonObject = JsonConvert.SerializeObject(queueObject);
                Queue.Send(new BrokeredMessage(jsonObject));

                //最后一次执行Queue操作失败后，执行回调函数
                if (failCallback != null && queueObject.ErrorCount == queueObject.MaxProcessErrorCount - 1)
                {
                    message.Complete();
                    failCallback(queueObject);
                }
            }

            return success;
        }

        public long GetMessageCount()
        {
            QueueDescription qd = new QueueDescription(QueueName);

            return qd.MessageCount;
        }

        public void AddMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            // Create a message and add it to the queue.
            entity.QueueName = QueueName;
            entity.QueueType = QueueType.ServiceBus;
            var jsonObject = JsonConvert.SerializeObject(entity, serializerSettings);
            var message = new BrokeredMessage(jsonObject);
            Queue.Send(message);

            LogUtil.LogDebug(string.Format("{0}: Tenant（{1}） insert Queue: {2} is success! ", ServiceName, TenantName, QueueName));
        }

        public void ModifyMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            // Get the message from the queue and update the message contents.
            BrokeredMessage message;
            while ((message = Queue.Receive()) != null)
            {
                var strObject = message.GetBody<string>();
                var queueObject = JsonConvert.DeserializeObject<T>(strObject);
                if (queueObject.Id == entity.Id)
                {
                    message.Complete();
                    entity.QueueName = QueueName;
                    entity.QueueType = QueueType.ServiceBus;
                    var jsonObject = JsonConvert.SerializeObject(entity);
                    Queue.Send(new BrokeredMessage(jsonObject));
                }
            }
        }

        public void RemoveAllMessage()
        {
            while (Queue.Peek() != null)
            {
                var message = Queue.Receive();
                message.Complete();
            }
        }

        public void RemoveTopMessage()
        {
            // Get the next message
            var retrievedMessage = Queue.Receive();
            //Process the message in less than 30 seconds, and then delete the message
            if (retrievedMessage != null)
                retrievedMessage.Complete();
        }


        public void RemoveMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            // Get the message from the queue and update the message contents.
            BrokeredMessage message;
            while ((message = Queue.Receive()) != null)
            {
                var strObject = message.GetBody<string>();
                var queueObject = JsonConvert.DeserializeObject<T>(strObject);
                if (queueObject.Id == entity.Id)
                {
                    message.Complete();
                }
            }
        }
    }

    public class ServicBusCommonQueueRepository<T> : ServiceBusQueueRepository<T> where T : QueueEntity
    {
        private static readonly string ServiceBusConnectionString = ConfigUtil.GetConfigItem("ServiceBusConnectionString");
        public ServicBusCommonQueueRepository()
            : base(ServiceBusConnectionString)
        {
        }
        public ServicBusCommonQueueRepository(string serviceBusConnectionString)
            : base(serviceBusConnectionString)
        {
        }
        public ServicBusCommonQueueRepository(Tenant tenant)
            : base(tenant)
        {

        }
    }
}
