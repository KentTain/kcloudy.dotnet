using System;
using System.Reflection;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Component.Base;
using KC.Component.IRepository;
using Newtonsoft.Json;

using System.Threading.Tasks;
using KC.Component.Util;
using System.Collections.Generic;
using KC.Common;

namespace KC.Component.QueueRepository
{
    public abstract class RedisQueueRepository<T> : IQueueRepository<T> where T : QueueEntity
    {
        private const string _serviceName = "KC.Component.QueueRepository.RedisRepository";
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

        private static RedisHelper redisHelper;

        public string QueueName { get; private set; }
        public Tenant Tenant { get; private set; }

        protected RedisQueueRepository(string connectionString)
            : this(connectionString, null)
        {
        }

        protected RedisQueueRepository(Tenant tenant)
            : this(tenant.GetDecryptQueueConnectionString(), tenant.TenantName)
        {
            Tenant = tenant;
        }

        protected RedisQueueRepository(string connectionString, string tenantName)
        {
            QueueName = tenantName + "-" + typeof(T).Name.ToLower();

            //Queue name rule: https://msdn.microsoft.com/en-us/library/dd179349.aspx
            //var regex = new Regex(@"^[A-Za-z][A-Za-z0-9]{2,62}$");
            //if (!regex.IsMatch(QueueName))
            //    throw new ArgumentException(string.Format("the QueueName {0} is not match the Azure name rule.", QueueName), "QueueName");

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString", _serviceName + ": Redis connection string is Empty.");

            var keyValues = connectionString.KeyValuePairFromConnectionString();
            if (!keyValues.ContainsKey(ConnectionKeyConstant.QueueEndpoint))
                throw new ArgumentException("Redis connection string is wrong. It can't set the Endpoint Value from service: " + _serviceName, "connectionString");

            var endpoint = keyValues[ConnectionKeyConstant.QueueEndpoint];
            var topic = keyValues[ConnectionKeyConstant.AccessName];
            var key = keyValues[ConnectionKeyConstant.AccessKey];

            //ConnectionString = string.Format("{0},password={1},ssl=True,abortConnect=False,allowAdmin=true", endpoint, key);

            var connectString = key.IsNullOrEmpty() ? endpoint : key + "@" + endpoint;

            var readWriteHosts = new List<string>() { connectString };
            redisHelper = new RedisHelper(11, readWriteHosts);
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

            var queue = redisHelper.ListGet<T>(QueueName);
            foreach (var message in queue)
            {
                if (callback == null) continue;

                if (message.ErrorCount >= message.MaxProcessErrorCount) continue;

                LogUtil.LogDebug(_serviceName + " begin to process Queue....... " + message.QueueName);
                var queueActionType = callback(message);
                LogUtil.LogDebug(_serviceName + " end to process Queue....... " + message.QueueName);

                var jsonObject = SerializeHelper.ToJson(message);
                if (queueActionType == QueueActionType.DeleteAfterExecuteAction)
                {
                    LogUtil.LogDebug(string.Format(".......{0} begin to delete Queue[{1}] when QueueActionType is {2} ", _serviceName, message.QueueName, QueueActionType.DeleteAfterExecuteAction));
                    redisHelper.ListRemove(QueueName, jsonObject);
                    LogUtil.LogDebug(string.Format(".......{0} end to delete Queue[{1}] when QueueActionType is {2} ", _serviceName, message.QueueName, QueueActionType.DeleteAfterExecuteAction));
                    if (message.IsManuallyDelete)
                    {
                        message.ErrorCount = 0;
                        redisHelper.ListAdd(QueueName, message, null);
                    }
                }
                else if (queueActionType == QueueActionType.FailedRepeatActon)
                {
                    success = false;
                    redisHelper.ListRemove(QueueName, jsonObject);

                    message.ErrorCount++;
                    //最后一次执行Queue操作失败后，执行回调函数
                    if (failCallback != null && message.ErrorCount == message.MaxProcessErrorCount)
                    {
                        failCallback(message);
                    }
                    else
                    {
                        LogUtil.LogDebug(string.Format(".......{0} begin to update Queue[{1}]'s ErrorCount when QueueActionType is {2} ", _serviceName, message.QueueName, QueueActionType.FailedRepeatActon));
                        redisHelper.ListAdd(QueueName, message, null);
                        LogUtil.LogDebug(string.Format(".......{0} end to update Queue[{1}]'s ErrorCount when QueueActionType is {2} ", _serviceName, message.QueueName, QueueActionType.FailedRepeatActon));
                    }
                }
                else if (queueActionType == QueueActionType.KeepQueueAction)
                {
                    LogUtil.LogDebug(string.Format(".......{0} begin to update Queue[{1}] when QueueActionType is {2} ", _serviceName, message.QueueName, QueueActionType.KeepQueueAction));
                    redisHelper.ListRemove(QueueName, jsonObject);

                    message.ErrorCount = 0;
                    redisHelper.ListAdd(QueueName, message, null);
                    LogUtil.LogDebug(string.Format(".......{0} end to update Queue[{1}] when QueueActionType is {2} ", _serviceName, message.QueueName, QueueActionType.KeepQueueAction));
                }

            }

            //LogUtil.LogInfo(ServiceName + " end to process Queue List...... ");

            return success;
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
            if (callback == null) return true;

            var message = redisHelper.ListBlockingGet<T>(QueueName, null);
            if (message == null) return true;

            var success = true;
            //三次执行队列方法出错后，不再执行处理消息队列的方法（callback）
            if (message.ErrorCount >= message.MaxProcessErrorCount) return true;

            var jsonObject = SerializeHelper.ToJson(message);
            var queueActionType = callback(message);
            if (queueActionType == QueueActionType.DeleteAfterExecuteAction)
            {
                //LogUtil.LogInfo(ServiceName + " begin to delete Queue....... " + queueObject.QueueName);
                redisHelper.ListRemove(QueueName, jsonObject);
                //LogUtil.LogInfo(ServiceName + " end to delete Queue....... " + queueObject.QueueName);
                if (message.IsManuallyDelete)
                {
                    message.ErrorCount = 0;
                    redisHelper.ListAdd(QueueName, message, null);
                }

                success = true;
            }
            else if (queueActionType == QueueActionType.FailedRepeatActon)
            {
                message.ErrorCount++;
                redisHelper.ListRemove(QueueName, jsonObject);

                //最后一次执行Queue操作失败后，执行回调函数
                if (failCallback != null && message.ErrorCount == message.MaxProcessErrorCount - 1)
                {
                    failCallback(message);
                }
                else
                {
                    redisHelper.ListAdd(QueueName, message, null);
                }
            }
            else if (queueActionType == QueueActionType.KeepQueueAction)
            {
                redisHelper.ListRemove(QueueName, jsonObject);

                message.ErrorCount = 0;
                redisHelper.ListAdd(QueueName, message, null);
            }

            return success;
        }
        public Task<bool> ProcessQueueAsync(Func<T, QueueActionType> callback, Action<T> failCallback)
        {
            throw new NotImplementedException();
        }

        public long GetMessageCount()
        {
            return redisHelper.ListLength(QueueName);
        }

        public void AddMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            // Create a message and add it to the queue.
            entity.QueueName = QueueName;
            entity.QueueType = QueueType.Redis;
            redisHelper.ListAdd(QueueName, entity, null);

            LogUtil.LogDebug(string.Format("{0}: Tenant（{1}） insert Queue: {2} is success! ", _serviceName, "dbo", QueueName));
        }

        public void ModifyMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            foreach (var message in redisHelper.ListGet<T>(QueueName))
            {
                if (message.Id == entity.Id)
                {
                    var value = SerializeHelper.ToJson(message);
                    entity.QueueName = QueueName;
                    entity.QueueType = QueueType.Redis;
                    redisHelper.ListAdd(QueueName, entity, null);
                    redisHelper.ListRemove(QueueName, value);
                    break;
                }
            }
        }

        public void RemoveAllMessage()
        {
            redisHelper.ListRemoveAll(QueueName);
        }

        public void RemoveTopMessage()
        {
            redisHelper.ListRemoveTop(QueueName);
        }

        public void RemoveMessage(T entity)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Queue:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        QueueName, MethodBase.GetCurrentMethod()));

            var value = SerializeHelper.ToJson(entity);
            redisHelper.ListRemove(QueueName, value);
        }

    }

    public class RedisCommonQueueRepository<T> : RedisQueueRepository<T> where T : QueueEntity
    {
        public RedisCommonQueueRepository()
            : base(KC.Framework.Base.GlobalConfig.GetDecryptQueueConnectionString())
        {
        }
        public RedisCommonQueueRepository(string queueConnectionString)
            : base(queueConnectionString)
        {
        }
        public RedisCommonQueueRepository(Tenant tenant)
            : base(tenant)
        {

        }
    }
}
