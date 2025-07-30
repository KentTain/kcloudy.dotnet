using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Com.Framework.Util;
using Com.Framework.Extension;
using Com.Framework.Tenant;
using Com.Model.Storage.Base;
using Com.Storage.Core.IRepository;
using System.Messaging;
using Newtonsoft.Json;

namespace Com.Storage.Core.QueueRepository
{
    public class MsmqQueueRepository<T> : IQueueRepository<T> where T : QueueEntity
    {
        private const string ServiceName = "Com.Storage.Core.QueueRepository.MsmqQueueRepository";
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

        //public MessageQueue Queue { get; private set; }
        private TimeSpan _timeOut = new TimeSpan(2000);
        public string QueueName { get; private set; }
        public string QueuePath { get; private set; }
        public IMessageFormatter QueueFormatter { get; private set; }
            
        public Tenant Tenant { get; private set; }

        protected MsmqQueueRepository(string connectionString)
            : this(connectionString, null)
        {
        }

        protected MsmqQueueRepository(Tenant tenant)
            : this(tenant.GetQueueConnectionString(), tenant.TenantName)
        {
            Tenant = tenant;
        }

        private MsmqQueueRepository(string connectionString, string tenantName)
        {
            QueueName = !string.IsNullOrWhiteSpace(tenantName)
                ? (tenantName + "-" + typeof(T).Name).ToLower()
                : "com-" + typeof (T).Name.ToLower();

            QueueFormatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            QueuePath = @".\private$\" + QueueName;
            if (!string.IsNullOrEmpty(connectionString))
            {
                var keyValues = connectionString.KeyValuePairFromConnectionString();
                var endpoint = keyValues[ConnectionKeyConstant.QueueEndpoint];
                var topic = keyValues[ConnectionKeyConstant.AccessName];
                if (string.IsNullOrWhiteSpace(endpoint))
                    throw new ArgumentException("Msmq Queue string is wrong. It can't set the Endpoint Value from service: " + ServiceName, "connectionString");

                QueuePath = endpoint + @"\Private$\" + QueueName;
            }

            //Queue = CreateIfNotExist();
        }

        /// <summary>
        /// 通过Create方法创建使用指定路径的新消息队列
        /// </summary>
        private MessageQueue CreateIfNotExist()
        {
            try
            {
                if (!MessageQueue.Exists(QueuePath))
                {
                    var result = MessageQueue.Create(QueuePath);  //创建事务性的专用消息队列
                    result.SetPermissions("Administrators",MessageQueueAccessRights.FullControl);
                    //result.QueueName = QueueName;
                    //result.DefaultPropertiesToSend = new DefaultPropertiesToSend()
                    //{
                    //    AttachSenderId = false,
                    //    UseAuthentication = false,
                    //    UseEncryption = false,
                    //    AcknowledgeType = AcknowledgeTypes.None,
                    //    UseJournalQueue = false
                    //};
                    //result.SetPermissions("Everyone", System.Messaging.MessageQueueAccessRights.FullControl);
                    return result;
                }
                else
                {
                    //设置当应用程序向消息对列发送消息时默认情况下使用的消息属性值
                    var result = new MessageQueue(QueuePath);
                    //result.QueueName = QueueName;
                    //result.DefaultPropertiesToSend = new DefaultPropertiesToSend()
                    //{
                    //    AttachSenderId = false,
                    //    UseAuthentication = false,
                    //    UseEncryption = false,
                    //    AcknowledgeType = AcknowledgeTypes.None,
                    //    UseJournalQueue = false
                    //};
                    //result.SetPermissions("Everyone", System.Messaging.MessageQueueAccessRights.FullControl);
                    return result;
                }
            }
            catch (MessageQueueException e)
            {
                LogUtil.LogError(string.Format("{0}: 创建队列失败，路径为：{1}。", ServiceName, QueuePath), e.Message);

                throw;
            }
        }

        public bool ProcessQueueList(Func<T, bool> callback, Action<T> failCallback)
        {
            //LogUtil.LogInfo(ServiceName + " start to process Queue List....... ");
            var success = true;
            using (var queue = CreateIfNotExist())
            {
                var iMessageCount = GetMessageCount();
                if (iMessageCount <= 0) return true;

                Message message;
                while ((message = queue.Receive(_timeOut)) != null)
                {
                    if (callback == null) continue;

                    message.Formatter = QueueFormatter;
                    var strObject = message.Body.ToString();
                    var queueObject = JsonConvert.DeserializeObject<T>(strObject);

                    if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) continue;

                    //LogUtil.LogDebug(ServiceName + " begin to process Queue....... " + queueObject.QueueName);
                    var isSuccess = callback(queueObject);
                    //LogUtil.LogDebug(ServiceName + " end to process Queue....... " + queueObject.QueueName);
                    if (isSuccess)
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
                        //LogUtil.LogInfo(ServiceName + " process queue has happend errors....... " + queueObject.QueueName);
                        queueObject.ErrorCount++;
                        var jsonObject = JsonConvert.SerializeObject(queueObject);
                        queue.Send(jsonObject);
                        success = false;

                        //最后一次执行Queue操作失败后，执行回调函数
                        if (failCallback != null && queueObject.ErrorCount == queueObject.MaxProcessErrorCount)
                        {
                            failCallback(queueObject);
                        }
                    }
                }

                //LogUtil.LogInfo(ServiceName + " end to process Queue List...... ");
            }

            return success;
        }

        public bool ProcessQueue(Func<T, bool> callback, Action<T> failCallback)
        {
            if (callback == null) return true;

            using (var queue = CreateIfNotExist())
            {
                var iMessageCount = GetMessageCount();
                if (iMessageCount <= 0) return true;

                var message = queue.Receive(_timeOut);
                if (message == null) return true;

                message.Formatter = QueueFormatter;
                var queueObject = JsonConvert.DeserializeObject<T>(message.Body.ToString());
                //三次执行队列方法出错后，不再执行处理消息队列的方法（callback）
                if (queueObject.ErrorCount >= queueObject.MaxProcessErrorCount) return true;

                var success = callback(queueObject);
                if (success)
                {
                    if (queueObject.IsManuallyDelete)
                    {
                        //LogUtil.LogInfo(ServiceName + " begin to delete Queue....... " + queueObject.QueueName);
                        queueObject.ErrorCount = 0;
                        var jsonObject = JsonConvert.SerializeObject(queueObject);
                        queue.Send(jsonObject);
                        //LogUtil.LogInfo(ServiceName + " end to delete Queue....... " + queueObject.QueueName);
                    }
                }
                else
                {
                    queueObject.ErrorCount++;
                    var jsonObject = JsonConvert.SerializeObject(queueObject);
                    queue.Send(jsonObject);

                    //最后一次执行Queue操作失败后，执行回调函数
                    if (failCallback != null && queueObject.ErrorCount == queueObject.MaxProcessErrorCount - 1)
                    {
                        failCallback(queueObject);
                    }
                }
                return success;
            }
        }

        public long GetMessageCount()
        {
            using (var queue = CreateIfNotExist())
            {
                var messages = queue.GetAllMessages();
                return messages.Count();
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
            entity.QueueType = QueueType.Msmq;
            var jsonObject = JsonConvert.SerializeObject(entity, serializerSettings);

            //try
            //{
                using (var queue = CreateIfNotExist())
                {
                    queue.Send(jsonObject);
                }
            //}
            //catch (MessageQueueException e)
            //{
            //    LogUtil.LogError(string.Format("{0}: 创建队列失败，路径为：{1}。", ServiceName, QueueName), e.Message);
            //}

            LogUtil.LogDebug(string.Format("{0}: Tenant（{1}） insert Queue: {2} is success! ", ServiceName, TenantName, QueueName));
        }

        public void ModifyMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));
            using (var queue = CreateIfNotExist())
            {
                foreach (var message in queue.GetAllMessages())
                {
                    message.Formatter = QueueFormatter;
                    var strObject = message.ToString();
                    var queueObject = JsonConvert.DeserializeObject<T>(strObject);
                    if (queueObject.Id == entity.Id)
                    {
                        entity.QueueName = QueueName;
                        entity.QueueType = QueueType.Msmq;
                        var jsonObject = JsonConvert.SerializeObject(entity);
                        queue.ReceiveById(message.Id, _timeOut);
                        queue.Send(jsonObject);
                    }
                }
            }
        }

        public void RemoveAllMessage()
        {
            using (var queue = CreateIfNotExist())
            {
                queue.Purge();
            }
        }

        public void RemoveTopMessage()
        {
            using (var queue = CreateIfNotExist())
            {
                var messge = queue.Receive(_timeOut);
            }
        }

        public void RemoveMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));
            using (var queue = CreateIfNotExist())
            {
                foreach (var message in queue.GetAllMessages())
                {
                    message.Formatter = QueueFormatter;
                    var strObject = message.ToString();
                    var queueObject = JsonConvert.DeserializeObject<T>(strObject);
                    if (queueObject.Id == entity.Id)
                    {
                        queue.ReceiveById(message.Id, _timeOut);
                    }
                }
            }
        }
    }

    public class MsmqCommonQueueRepository<T> : MsmqQueueRepository<T> where T : QueueEntity
    {
        private static readonly string queueConnectionString = ConfigUtil.GetConfigItem("QueueConnectionString");
        public MsmqCommonQueueRepository()
            : base(queueConnectionString)
        {
        }
        public MsmqCommonQueueRepository(string queueConnectionString)
            : base(queueConnectionString)
        {
        }
        public MsmqCommonQueueRepository(Tenant tenant)
            : base(tenant)
        {

        }
    }
}
