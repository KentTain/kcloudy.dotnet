using System;
using System.Reflection;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Component.Base;
using KC.Component.IRepository;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace KC.Component.QueueRepository
{
    public abstract class AzureQueueRepository<T> : IQueueRepository<T> where T : QueueEntity
    {
        private const string ServiceName = "KC.Component.QueueRepository.AzureQueueRepository";
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
        public QueueClient QueueClient { get; private set; }
        public string QueueName { get; private set; }
        public Tenant Tenant { get; private set; }

        protected AzureQueueRepository(string connectionString)
            : this(connectionString, null)
        {
        }

        protected AzureQueueRepository(Tenant tenant)
            : this(tenant.GetDecryptQueueConnectionString(), tenant.TenantName)
        {
            Tenant = tenant;
        }

        private AzureQueueRepository(string connectionString, string tenantName)
        {
            QueueName = !string.IsNullOrWhiteSpace(tenantName)
                ? (tenantName + "-" + typeof(T).Name).ToLower()
                : "kc-" + typeof (T).Name.ToLower();

            //Queue name rule: https://msdn.microsoft.com/en-us/library/dd179349.aspx
            //var regex = new Regex(@"^[A-Za-z][A-Za-z0-9]{2,62}$");
            //if (!regex.IsMatch(QueueName))
            //    throw new ArgumentException(string.Format("the QueueName {0} is not match the Azure name rule.", QueueName), "QueueName");

            QueueClient = new QueueClient(connectionString, QueueName);
            QueueClient.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// 处理所有消息队列列表
        /// </summary>
        /// <param name="callback">处理每个消息队列的方法</param>
        /// <param name="failCallback">队列发生错误后的错误处理方法</param>
        /// <returns></returns>
        public bool ProcessQueueList(Func<T, QueueActionType> callback, Action<T> failCallback)
        {
            //LogUtil.LogInfo(ServiceName + " start to process Queue List....... ");
            var success = true;
            //Queue.FetchAttributesAsync();
            //var cachedMessageCount = Queue.ApproximateMessageCount;
            //if (!cachedMessageCount.HasValue) return true;

            QueueMessage[] messages = QueueClient.ReceiveMessages(10);
            foreach (var message in messages)
            {
                if (callback == null) continue;

                var queueObject = message.Body.ToObjectFromJson<T>();

                if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) continue;

                LogUtil.LogDebug(ServiceName + " begin to process Queue....... " + queueObject.QueueName);
                var queueActionType = callback(queueObject);
                LogUtil.LogDebug(ServiceName + " end to process Queue....... " + queueObject.QueueName);
                if (queueActionType == QueueActionType.DeleteAfterExecuteAction)
                {
                    LogUtil.LogDebug(string.Format(".......{0} begin to delete Queue[{1}] when QueueActionType is {2} ", ServiceName, queueObject.QueueName, QueueActionType.DeleteAfterExecuteAction));
                    QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                    LogUtil.LogDebug(string.Format(".......{0} end to delete Queue[{1}] when QueueActionType is {2} ", ServiceName, queueObject.QueueName, QueueActionType.DeleteAfterExecuteAction));
                    if (queueObject.IsManuallyDelete)
                    {
                        queueObject.ErrorCount = 0;
                        var jsonObject = JsonConvert.SerializeObject(queueObject, serializerSettings);
                        SendReceipt receipt = QueueClient.SendMessage(jsonObject);
                    }
                }
                else if (queueActionType == QueueActionType.FailedRepeatActon)
                {
                    queueObject.ErrorCount++;
                    var jsonObject = JsonConvert.SerializeObject(queueObject);
                    
                    LogUtil.LogDebug(string.Format(".......{0} begin to update Queue[{1}]'s ErrorCount when QueueActionType is {2} ", ServiceName, queueObject.QueueName, QueueActionType.FailedRepeatActon));
                    QueueClient.UpdateMessage(message.MessageId, message.PopReceipt, jsonObject, TimeSpan.FromSeconds(0.0));
                    LogUtil.LogDebug(string.Format(".......{0} end to update Queue[{1}]'s ErrorCount when QueueActionType is {2} ", ServiceName, queueObject.QueueName, QueueActionType.FailedRepeatActon));

                    success = false;

                    //最后一次执行Queue操作失败后，执行回调函数
                    if (failCallback != null && queueObject.ErrorCount == queueObject.MaxProcessErrorCount)
                    {
                        QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                        failCallback(queueObject);
                    }
                }
                else if (queueActionType == QueueActionType.KeepQueueAction)
                {
                    var jsonObject = JsonConvert.SerializeObject(queueObject);
                    LogUtil.LogDebug(string.Format(".......{0} begin to update Queue[{1}] when QueueActionType is {2} ", ServiceName, queueObject.QueueName, QueueActionType.KeepQueueAction));
                    QueueClient.UpdateMessage(message.MessageId, message.PopReceipt, jsonObject, new TimeSpan(0, 0, 2, 0));
                    LogUtil.LogDebug(string.Format(".......{0} end to update Queue[{1}] when QueueActionType is {2} ", ServiceName, queueObject.QueueName, QueueActionType.KeepQueueAction));

                    //Queue.DeleteMessage(message);

                    //queueObject.ErrorCount = 0;
                    //var jsonObject = JsonConvert.SerializeObject(queueObject, serializerSettings);
                    //var newMessage = new CloudQueueMessage(jsonObject);
                    //Queue.AddMessage(newMessage);
                }

            }

            //LogUtil.LogInfo(ServiceName + " end to process Queue List...... ");

            return success;
        }
        public async Task<bool> ProcessQueueListAsync(Func<T, QueueActionType> callback, Action<T> failCallback)
        {
            //LogUtil.LogInfo(ServiceName + " start to process Queue List....... ");
            var success = true;
            //await Queue.FetchAttributesAsync();
            //var cachedMessageCount = Queue.ApproximateMessageCount;
            //if (!cachedMessageCount.HasValue) return true;

            QueueMessage[] messages = await QueueClient.ReceiveMessagesAsync(10);
            foreach (var message in messages)
            {
                if (callback == null) continue;

                var queueObject = message.Body.ToObjectFromJson<T>();

                if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) continue;

                LogUtil.LogDebug(ServiceName + " begin to process Queue....... " + queueObject.QueueName);
                var queueActionType = callback(queueObject);
                LogUtil.LogDebug(ServiceName + " end to process Queue....... " + queueObject.QueueName);
                if (queueActionType == QueueActionType.DeleteAfterExecuteAction)
                {
                    //LogUtil.LogInfo(ServiceName + " begin to delete Queue....... " + queueObject.QueueName);
                    await QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                    //LogUtil.LogInfo(ServiceName + " end to delete Queue....... " + queueObject.QueueName);
                    if (queueObject.IsManuallyDelete)
                    {
                        queueObject.ErrorCount = 0;
                        var jsonObject = JsonConvert.SerializeObject(queueObject, serializerSettings);
                        await QueueClient.SendMessageAsync(jsonObject);
                    }
                }
                else if (queueActionType == QueueActionType.FailedRepeatActon)
                {
                    //LogUtil.LogInfo(ServiceName + " process queue has happend errors....... " + queueObject.QueueName);
                    queueObject.ErrorCount++;
                    var jsonObject = JsonConvert.SerializeObject(queueObject);
                    // Make it visible immediately;
                    await QueueClient.UpdateMessageAsync(message.MessageId, message.PopReceipt, jsonObject, TimeSpan.FromSeconds(0.0));
                    success = false;

                    //最后一次执行Queue操作失败后，执行回调函数
                    if (failCallback != null && queueObject.ErrorCount == queueObject.MaxProcessErrorCount)
                    {
                        await QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                        failCallback(queueObject);
                    }
                }
                else if (queueActionType == QueueActionType.KeepQueueAction)
                {
                    var jsonObject = JsonConvert.SerializeObject(queueObject);
                    await QueueClient.UpdateMessageAsync(message.MessageId, message.PopReceipt, jsonObject, new TimeSpan(0, 0, 2, 0));

                    //Queue.DeleteMessage(message);
                    //queueObject.ErrorCount = 0;
                    //var jsonObject = JsonConvert.SerializeObject(queueObject, serializerSettings);
                    //var newMessage = new CloudQueueMessage(jsonObject);
                    //Queue.AddMessage(newMessage);
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
        public bool ProcessQueue(Func<T, QueueActionType> callback, Action<T> failCallback)
        {
            if (callback == null) return true;

            QueueMessage message = QueueClient.ReceiveMessage();
            if (message == null) return true;

            var success = true;
            var queueObject = message.Body.ToObjectFromJson<T>();
            //三次执行队列方法出错后，不再执行处理消息队列的方法（callback）
            if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) return true;

            var queueActionType = callback(queueObject);
            if (queueActionType == QueueActionType.DeleteAfterExecuteAction)
            {
                //LogUtil.LogInfo(ServiceName + " begin to delete Queue....... " + queueObject.QueueName);
                QueueClient.DeleteMessage(message.MessageId, message.PopReceipt);
                //LogUtil.LogInfo(ServiceName + " end to delete Queue....... " + queueObject.QueueName);
                if (queueObject.IsManuallyDelete)
                {
                    queueObject.ErrorCount = 0;
                    var jsonObject = JsonConvert.SerializeObject(queueObject, serializerSettings);
                    QueueClient.SendMessage(jsonObject);
                }

                success = true;
            }
            else if (queueActionType == QueueActionType.FailedRepeatActon)
            {
                queueObject.ErrorCount++;
                var jsonObject = JsonConvert.SerializeObject(queueObject);
                QueueClient.UpdateMessage(message.MessageId, message.PopReceipt, jsonObject, TimeSpan.FromSeconds(0.0));

                //最后一次执行Queue操作失败后，执行回调函数
                if (failCallback != null && queueObject.ErrorCount == queueObject.MaxProcessErrorCount - 1)
                {
                    QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                    failCallback(queueObject);
                }
            }
            else if (queueActionType == QueueActionType.KeepQueueAction)
            {
                QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);

                queueObject.ErrorCount = 0;
                var jsonObject = JsonConvert.SerializeObject(queueObject, serializerSettings);
                QueueClient.SendMessage(jsonObject);
            }

            return success;
        }
        public async Task<bool> ProcessQueueAsync(Func<T, QueueActionType> callback, Action<T> failCallback)
        {
            if (callback == null) return true;

            QueueMessage message = QueueClient.ReceiveMessage();
            if (message == null) return true;

            var success = true;
            var queueObject = message.Body.ToObjectFromJson<T>();
            //三次执行队列方法出错后，不再执行处理消息队列的方法（callback）
            if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) return true;

            var queueActionType = callback(queueObject);
            if (queueActionType == QueueActionType.DeleteAfterExecuteAction)
            {
                //LogUtil.LogInfo(ServiceName + " begin to delete Queue....... " + queueObject.QueueName);
                await QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                //LogUtil.LogInfo(ServiceName + " end to delete Queue....... " + queueObject.QueueName);
                if (queueObject.IsManuallyDelete)
                {
                    queueObject.ErrorCount = 0;
                    var jsonObject = JsonConvert.SerializeObject(queueObject, serializerSettings);
                    await QueueClient.SendMessageAsync(jsonObject);
                }

                success = true;
            }
            else if (queueActionType == QueueActionType.FailedRepeatActon)
            {
                queueObject.ErrorCount++;
                var jsonObject = JsonConvert.SerializeObject(queueObject);
                await QueueClient.UpdateMessageAsync(message.MessageId, message.PopReceipt, jsonObject, TimeSpan.FromSeconds(0.0));

                //最后一次执行Queue操作失败后，执行回调函数
                if (failCallback != null && queueObject.ErrorCount == queueObject.MaxProcessErrorCount - 1)
                {
                    await QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                    failCallback(queueObject);
                }
            }
            else if (queueActionType == QueueActionType.KeepQueueAction)
            {
                await QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);

                queueObject.ErrorCount = 0;
                var jsonObject = JsonConvert.SerializeObject(queueObject, serializerSettings);
                await QueueClient.SendMessageAsync(jsonObject);
            }

            return success;
        }

        public long GetMessageCount()
        {
            // Fetch the queue attributes.
            QueueProperties properties = QueueClient.GetProperties();

            // Retrieve the cached approximate message count.
            return properties.ApproximateMessagesCount;
        }

        public async void AddMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            // Create a message and add it to the queue.
            entity.QueueName = QueueName;
            entity.QueueType = QueueType.AzureQueue;
            var jsonObject = JsonConvert.SerializeObject(entity, serializerSettings);
            await QueueClient.SendMessageAsync(jsonObject);

            LogUtil.LogDebug(string.Format("{0}: Tenant（{1}） insert Queue: {2} is success! ", ServiceName, TenantName, QueueName));
        }

        public async void ModifyMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            // Get the message from the queue and update the message contents.
            QueueProperties properties = QueueClient.GetProperties();
            QueueMessage[] messages = await QueueClient.ReceiveMessagesAsync(properties.ApproximateMessagesCount);
            foreach (var message in messages)
            {
                var queueObject = message.Body.ToObjectFromJson<T>();
                if (queueObject.Id == entity.Id)
                {
                    entity.QueueName = QueueName;
                    entity.QueueType = QueueType.AzureQueue;
                    var jsonObject = JsonConvert.SerializeObject(entity);
                    // Make it visible immediately.
                    await QueueClient.UpdateMessageAsync(message.MessageId, message.PopReceipt, jsonObject, TimeSpan.FromSeconds(0.0));
                }
            }
        }

        public async void RemoveAllMessage()
        {
            await QueueClient.ClearMessagesAsync();
        }

        public async void RemoveTopMessage()
        {
            // Get the next message
            QueueMessage retrievedMessage = await QueueClient.ReceiveMessageAsync();
            //Process the message in less than 30 seconds, and then delete the message
            if (retrievedMessage != null)
                await QueueClient.DeleteMessageAsync(retrievedMessage.MessageId, retrievedMessage.PopReceipt);
        }

        public async void RemoveMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            // Get the message from the queue and update the message contents.
            QueueProperties properties = QueueClient.GetProperties();
            QueueMessage[] messages = await QueueClient.ReceiveMessagesAsync(properties.ApproximateMessagesCount);
            foreach (var message in messages)
            {
                var queueObject = message.Body.ToObjectFromJson<T>();
                if (queueObject.Id == entity.Id)
                {
                    await QueueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                }
            }
        }
    }

    public class AzureCommonQueueRepository<T> : AzureQueueRepository<T> where T : QueueEntity
    {
        private static readonly string queueConnectionString = KC.Framework.Base.GlobalConfig.QueueConnectionString;
        public AzureCommonQueueRepository()
            : base(queueConnectionString)
        {
        }
        public AzureCommonQueueRepository(string queueConnectionString)
            : base(queueConnectionString)
        {
        }
        public AzureCommonQueueRepository(Tenant tenant)
            : base(tenant)
        {

        }
    }

    public class CustomResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            property.ShouldSerialize = instance =>
            {
                try
                {
                    var prop = (PropertyInfo)member;
                    if (prop.CanRead)
                    {
                        prop.GetValue(instance, null);
                        return true;
                    }
                }
                catch
                {
                }
                return false;
            };

            return property;
        }
    } 
}
