using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Util;
using ServiceStack.Redis;

namespace KC.Component.Util
{
    public class RedisHelper
    {
        private int DatabaseId { get; set; }
        private List<string> ReadWriteHosts { get; set; }

        public string CustomKey = "kcloudy-";

        private static readonly object Locker = new object();
        private static IRedisClientsManager _instance;

        /// <summary>
        /// 单例获取
        /// </summary>
        private static IRedisClientsManager GetManager(List<string> readWriteHosts)
        {
            RedisConfig.VerifyMasterConnections = false; //加上这一句，否则调用 _pooledRedisClientManager.GetClient()会报错
            if (_instance == null)
            {
                lock (Locker)
                {
                    if (_instance == null)
                    {
                        //配置连接池和读写分类
                        _instance = new RedisManagerPool(
                            readWriteHosts,
                            new RedisPoolConfig()
                            {
                                MaxPoolSize = 50       //读节点个数
                            });
                    }
                }
            }

            return _instance;
        }
        private IRedisClient _client
        {
            get
            {
                var manager = GetManager(ReadWriteHosts);
                var client = manager.GetClient();//获取连接
                client.Db = DatabaseId;
                return client;
            }
        }

        #region 构造函数

        public RedisHelper(int databaseId, List<string> readWriteHosts)
        {
            DatabaseId = databaseId;
            ReadWriteHosts = readWriteHosts;
        }

        #endregion 构造函数

        #region String

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public void StringSet(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);

            if (expiry.HasValue)
                Do(db => db.Set<string>(key, value, expiry.Value));
            else
                Do(db => db.Set<string>(key, value));
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public void StringSet(Dictionary<string, string> keyValues)
        {
            Do(db => { db.SetAll(keyValues); });
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool StringSet<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            if (typeof(T) == typeof(string))
            {
                if (expiry.HasValue)
                    return Do(db => db.Set(key, obj.ToString(), expiry.Value));
                else
                    return Do(db => db.Set(key, obj.ToString()));
            }
            //var objString = SerializeHelper.ToBinary(obj);
            //var objString = SerializeHelper.ToJson(obj);
            if (expiry.HasValue)
                return Do(db => db.Set(key, obj, expiry.Value));
            else
                return Do(db => db.Set(key, obj));
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.Get<string>(key));
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public List<string> StringGet(List<string> listKey)
        {
            List<string> newKeys = listKey.Select(AddSysCustomKey).ToList();
            return Do(db => db.GetValues(ConvertRedisKeys(newKeys)));
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T StringGet<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.Get<T>(key));
        }

        public object StringGet(Type type, string key)
        {
            key = AddSysCustomKey(key);
            var objString = Do(db => db.GetValue(key));
            if (type == typeof(string))
            {
                return objString.ToString();
            }

            //var result = SerializeHelper.FromBinary<T>(objString);
            //var result = SerializeHelper.FromProtobufBinary(type, objString);
            var result = SerializeHelper.FromJson(type, objString);
            return result;
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, int val = 1)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.IncrementValueBy(key, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double StringDecrement(string key, int val = 1)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.DecrementValueBy(key, val));
        }

        public bool StringRemove(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.Remove(key));
        }
        #endregion String

        #region List 列表

        public void ListAdd(string key, string value, TimeSpan? expiredSp)
        {
            key = AddSysCustomKey(key);
            Do(db => {
                db.AddItemToList(key, value);
                if (expiredSp.HasValue)
                    db.ExpireEntryIn(key, expiredSp.Value);
            });
        }

        /// <summary>
        /// 添加key/value
        /// </summary>     
        public void ListAdd<T>(string key, T value, TimeSpan? expiredSp)
        {
            key = AddSysCustomKey(key);
            if (typeof(T) == typeof(string))
            {
                Do(db => {
                    db.AddItemToList(key, value.ToString());
                    if (expiredSp.HasValue)
                        db.ExpireEntryIn(key, expiredSp.Value);
                });
                return;
            }
            //var objString = SerializeHelper.ToBinary(obj);
            var objString = SerializeHelper.ToJson(value);
            Do(db => {
                db.AddItemToList(key, objString);
                if (expiredSp.HasValue)
                    db.ExpireEntryIn(key, expiredSp.Value);
            });
        }

        /// <summary>
        /// 左侧入队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListLeftPush<T>(string key, T value, TimeSpan? expiredSp)
        {
            key = AddSysCustomKey(key);
            if (typeof(T) == typeof(string))
            {
                Do(db => {
                    db.PushItemToList(key, value.ToString());
                    if (expiredSp.HasValue)
                        db.ExpireEntryIn(key, expiredSp.Value);
                });
                return;
            }
            //var objString = SerializeHelper.ToBinary(obj);
            var objString = SerializeHelper.ToJson(value);
            Do(db => {
                db.PushItemToList(key, objString);
                if (expiredSp.HasValue)
                    db.ExpireEntryIn(key, expiredSp.Value);
            });
        }

        /// <summary>
        /// 右侧入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRightPush<T>(string key, T value, TimeSpan? expiredSp)
        {
            key = AddSysCustomKey(key);
            if (typeof(T) == typeof(string))
            {
                Do(db => {
                    db.PrependItemToList(key, value.ToString());
                    if (expiredSp.HasValue)
                        db.ExpireEntryIn(key, expiredSp.Value);
                });
                return;
            }
            //var objString = SerializeHelper.ToBinary(obj);
            var objString = SerializeHelper.ToJson(value);
            Do(db => {
                db.PrependItemToList(key, objString);
                if (expiredSp.HasValue)
                    db.ExpireEntryIn(key, expiredSp.Value);
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.GetListCount(key));
        }

        /// <summary>
        /// 获取key包含的所有数据集合
        /// </summary>  
        public List<T> ListGet<T>(string key)
        {
            key = AddSysCustomKey(key);
            if (typeof(T) == typeof(string))
            {
                Do(db => {
                    return db.GetAllItemsFromList(key);
                });
            }

            return Do(db => {
                var result = db.GetAllItemsFromList(key);
                return result.Select(m => SerializeHelper.FromJson<T>(m)).ToList();
            });
        }

        public string ListBlockingGet(string key, TimeSpan? expiredSp)
        {
            key = AddSysCustomKey(key);
            return Do(db => {
                var result = db.BlockingDequeueItemFromList(key, expiredSp);
                return result;
            });
        }

        /// <summary>
        /// 阻塞命令：从list中key的头部移除一个值，并返回移除的值，阻塞时间为sp
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        public T ListBlockingGet<T>(string key, TimeSpan? expiredSp)
        {
            key = AddSysCustomKey(key);

            return Do(db => {
                var result = db.BlockingDequeueItemFromList(key, expiredSp);
                return SerializeHelper.FromJson<T>(result);
            });
        }

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRemove<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            if (typeof(T) == typeof(string))
            {
                Do(db => db.RemoveItemFromList(key, value.ToString()));
                return;
            }
            //var objString = SerializeHelper.ToBinary(obj);
            var objString = SerializeHelper.ToJson(value);
            Do(db => db.RemoveItemFromList(key, objString));
        }

        public void ListRemoveAll(string key)
        {
            key = AddSysCustomKey(key);
            Do(db => db.RemoveAllFromList(key));
        }

        public void ListRemoveTop(string key)
        {
            Do(db => db.RemoveEndFromList(key));
        }

        #endregion

        #region SortedSet 有序集合
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SortedSetAdd(string key, string value, double? score)
        {
            key = AddSysCustomKey(key);
            if (score.HasValue)
                return Do(db => db.AddItemToSortedSet(key, value.ToString(), score.Value));
            else
                return Do(db => db.AddItemToSortedSet(key, value.ToString()));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SortedSetAdd<T>(string key, T value, double? score)
        {
            key = AddSysCustomKey(key);
            if (typeof(T) == typeof(string))
            {
                if (score.HasValue)
                    return Do(db => db.AddItemToSortedSet(key, value.ToString(), score.Value));
                else
                    return Do(db => db.AddItemToSortedSet(key, value.ToString()));
            }
            //var objString = SerializeHelper.ToBinary(obj);
            var objString = SerializeHelper.ToJson(value);
            if (score.HasValue)
                return Do(db => db.AddItemToSortedSet(key, objString, score.Value));
            else
                return Do(db => db.AddItemToSortedSet(key, objString));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> SortedSetGet(string key, bool asc = true)
        {
            key = AddSysCustomKey(key);
            return Do(db => {
                return asc
                    ? db.GetAllItemsFromSortedSet(key)
                    : db.GetAllItemsFromSortedSetDesc(key);
            });
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SortedSetGet<T>(string key, bool asc = true)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                var values = asc
                    ? db.GetAllItemsFromSortedSet(key).ToArray()
                    : db.GetAllItemsFromSortedSet(key).ToArray();
                return ConvetList<T>(values);
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            key = AddSysCustomKey(key);
            return Do(redis => redis.GetSortedSetCount(key));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SortedSetRemove<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            if (typeof(T) == typeof(string))
            {
                return Do(db => db.RemoveItemFromSortedSet(key, value.ToString()));
            }
            //var objString = SerializeHelper.ToBinary(obj);
            var objString = SerializeHelper.ToJson(value);
            return Do(db => db.RemoveItemFromSortedSet(key, objString));
        }

        public bool SortedSetRemove(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.RemoveRangeFromSortedSetBySearch(key) >= 0);
        }

        /// <summary>
        /// 判断key集合中是否存在value数据
        /// </summary>
        public bool SortedSetContains<T>(string key, T value)
        {
            key = AddSysCustomKey(key);
            if (typeof(T) == typeof(string))
            {
                return Do(db => db.SortedSetContainsItem(key, value.ToString()));
            }
            var objString = SerializeHelper.ToJson(value);
            return Do(db => db.SortedSetContainsItem(key, objString));
        }

        #endregion SortedSet 有序集合

        #region Hash

        #region 同步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExists(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.ContainsKey(key));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, T t)
        {
            key = AddSysCustomKey(key);
            if (typeof(T) == typeof(string))
            {
                return Do(db => { db.AddItemToSet(key, t.ToString()); return true; });
            }
            //var objString = SerializeHelper.ToBinary(obj);
            //var objString = SerializeHelper.ToProtobufBinary(t);
            var objString = SerializeHelper.ToJson(t);
            return Do(db => { db.AddItemToSet(key, objString); return true; });
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return Do(db => { db.RemoveItemFromSet(key, dataKey); return true; });
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public bool HashDelete(List<string> dataKeys)
        {
            var dataKeys1 = dataKeys.Select(m => AddSysCustomKey(m)).ToList();
            return Do(db => { db.RemoveAll(dataKeys1); return true; });
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                string value = db.GetRandomItemFromSet(key);
                return ConvertObj<T>(value);
            });
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db =>
            {
                string[] values = db.GetAllItemsFromSet(key).ToArray();
                return ConvetList<T>(values);
            });
        }

        #endregion 同步方法

        #endregion Hash

        #region key

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public bool KeyDelete(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.RemoveEntry(key));
        }

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        public void KeyDelete(List<string> keys)
        {
            List<string> newKeys = keys.Select(AddSysCustomKey).ToList();
            Do(db => db.RemoveAll(ConvertRedisKeys(newKeys)));
        }

        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.ContainsKey(key));
        }

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public void KeyRename(string key, string newKey)
        {
            key = AddSysCustomKey(key);
            Do(db => db.RenameKey(key, newKey));
        }

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeyExpire(string key, TimeSpan expiry)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.ExpireEntryIn(key, expiry));
        }

        #endregion key

        #region 发布订阅
        public void PublishStart(string pubChannel)
        {
            var manager = GetManager(ReadWriteHosts);
            //发布、订阅服务 IRedisPubSubServer
            var pubSubServer = new RedisPubSubServer(manager, pubChannel)
            {
                OnMessage = (channel, msg) =>
                {
                    LogUtil.LogInfo($"从频道：{channel}上接受到消息：{msg},时间：{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}");
                    LogUtil.LogInfo("___________________________________________________________________");
                },
                OnStart = () =>
                {
                    LogUtil.LogInfo("发布服务已启动");
                    LogUtil.LogInfo("___________________________________________________________________");
                },
                OnStop = () => { LogUtil.LogInfo("发布服务停止"); },
                OnUnSubscribe = channel => { LogUtil.LogInfo(channel); },
                OnError = e => { LogUtil.LogInfo(e.Message); },
                OnFailover = s => { LogUtil.LogInfo(s.GetStatus()); },
            };
            //接收消息
            pubSubServer.Start();
        }

        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pubChannel"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long Publish(string pubChannel, string value)
        {
            using (var publisher = _client)
            {
                pubChannel = AddSysCustomKey(pubChannel);
                return publisher.PublishMessage(pubChannel, value);
            }
        }
        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long Publish<T>(string pubChannel, T value)
        {
            using (var publisher = _client)
            {
                pubChannel = AddSysCustomKey(pubChannel);
                if (typeof(T) == typeof(string))
                {
                    return publisher.PublishMessage(pubChannel, value.ToString());
                }
                //var objString = SerializeHelper.ToBinary(obj);
                var objString = SerializeHelper.ToJson(value);
                return publisher.PublishMessage(pubChannel, objString);
            }
        }

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<string, string> handler)
        {
            using (var consumer = _client)
            {
                subChannel = AddSysCustomKey(subChannel);
                //创建订阅
                var sub = consumer.CreateSubscription();
                //接受到消息时
                sub.OnMessage = (channel, message) =>
                {
                    LogUtil.LogInfo($"从频道：{channel}上接受到消息：{message},时间：{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}");
                    LogUtil.LogInfo("订阅数：" + sub.SubscriptionCount);
                    LogUtil.LogInfo("___________________________________________________________________");
                    handler(channel, message);
                };
                //订阅频道时
                sub.OnSubscribe = (channel) =>
                {
                    LogUtil.LogInfo("订阅客户端：开始订阅" + channel);
                };
                //取消订阅频道时
                sub.OnUnSubscribe = (channel) =>
                {
                    LogUtil.LogInfo("订阅客户端：取消订阅" + channel);
                };
                //订阅频道时
                sub.SubscribeToChannels(subChannel);
            }
        }

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe<T>(string subChannel, Action<string, T> handler)
        {
            using (var consumer = _client)
            {
                subChannel = AddSysCustomKey(subChannel);
                //创建订阅
                var sub = consumer.CreateSubscription();
                //接受到消息时
                sub.OnMessage = (channel, message) =>
                {
                    LogUtil.LogInfo($"从频道：{channel}上接受到消息：{message},时间：{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")}");
                    LogUtil.LogInfo("订阅数：" + sub.SubscriptionCount);
                    LogUtil.LogInfo("___________________________________________________________________");
                    var result = ConvertObj<T>(message);
                    handler(channel, result);
                };
                //订阅频道时
                sub.OnSubscribe = (channel) =>
                {
                    LogUtil.LogInfo("订阅客户端：开始订阅" + channel);
                };
                //取消订阅频道时
                sub.OnUnSubscribe = (channel) =>
                {
                    LogUtil.LogInfo("订阅客户端：取消订阅" + channel);
                };
                //订阅频道时
                sub.SubscribeToChannels(subChannel);
            }
        }

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string subChannel)
        {
            using (var consumer = _client)
            {
                subChannel = AddSysCustomKey(subChannel);
                //创建订阅
                var sub = consumer.CreateSubscription();
                //取消订阅频道时
                sub.OnUnSubscribe = (channel) =>
                {
                    LogUtil.LogInfo("订阅客户端：取消订阅" + channel);
                };

                sub.UnSubscribeFromChannels(subChannel);
            }
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            using (var consumer = _client)
            {
                //创建订阅
                var sub = consumer.CreateSubscription();
                //取消订阅频道时
                sub.OnUnSubscribe = (channel) =>
                {
                    LogUtil.LogInfo("订阅客户端：取消订阅" + channel);
                };

                sub.UnSubscribeFromAllChannels();
            }
        }

        #endregion 发布订阅

        #region 分布式锁
        /// <summary>
        /// 尝试获取分布式锁
        /// </summary>
        /// <param name="key">分布式锁的Key</param>
        /// <param name="value">分布式锁的Value/param>
        /// <param name="getlockTimeOut">分布式锁的过期时间</param>
        /// <returns></returns>
        public bool TryGetDistributedLock(string key, string value, TimeSpan getlockTimeOut)
        {
            try
            {
                string lockKey = AddSysCustomKey(key);
                using (var client = _client as RedisNativeClient)
                {
                    int expireSeconds = (int)getlockTimeOut.TotalSeconds;
                    //循环获取取锁
                    if (client.SetNX(lockKey, new byte[] { 1 }) == 1)
                    {
                        //设置锁的过期时间
                        client.Expire(lockKey, expireSeconds);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }

        }

        /// <summary>
        /// 尝试释放分布式锁
        /// </summary>
        /// <param name="key">分布式锁的Key</param>
        /// <param name="value">分布式锁的Value</param>
        /// <returns></returns>
        public bool ReleaseDistributedLock(string key, string value)
        {
            try
            {
                string lockKey = AddSysCustomKey(key);
                using (var client = _client as RedisNativeClient)
                {
                    client.Del(lockKey);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return false;
            }

        }

        /// <summary>
        /// 尝试获取分布式锁
        /// </summary>
        /// <param name="key">分布式锁的Key</param>
        /// <param name="value">分布式锁的Value/param>
        /// <param name="getlockTimeOut">分布式锁的过期时间</param>
        /// <returns></returns>
        public async Task<bool> TryGetDistributedLockAsync(string key, string value, TimeSpan getlockTimeOut)
        {
            return await Task.Factory.StartNew(() => {
                return TryGetDistributedLock(key, value, getlockTimeOut);
            });
        }

        /// <summary>
        /// 尝试释放分布式锁
        /// </summary>
        /// <param name="key">分布式锁的Key</param>
        /// <param name="value">分布式锁的Value/param>
        /// <returns></returns>
        public async Task<bool> ReleaseDistributedLockAsync(string key, string value)
        {
            return await Task.Factory.StartNew(() => {
                return ReleaseDistributedLock(key, value);
            });
        }
        #endregion

        #region 其他
        /// <summary>
        /// 根据配置文件中的连接字符串，拼接好Redis连接字符串
        /// </summary>
        /// <param name="connectConfig">配置文件中的连接字符串</param>
        /// <returns></returns>
        public static string GetRedisConnectionString(string connectConfig)
        {
            if (string.IsNullOrEmpty(connectConfig))
                return string.Empty;

            var keyValues = connectConfig.KeyValuePairFromConnectionString();
            var endpoint = keyValues[ConnectionKeyConstant.RedisEndpoint];
            var topic = keyValues[ConnectionKeyConstant.AccessName];
            var key = keyValues[ConnectionKeyConstant.AccessKey];

            if (string.IsNullOrWhiteSpace(endpoint))
                return string.Empty;

            //ConnectionString = string.Format("{0},password={1},ssl=True,abortConnect=False,allowAdmin=true", endpoint, key);
            return key.IsNullOrEmpty()
                ? endpoint
                : key + "@" + endpoint;
        }

        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public IRedisClient GetClient()
        {
            return _client;
        }

        public List<string> GetAllKeys(string key)
        {
            using (var client = _client)
            {
                return client.GetAllKeys();
            }
        }

        public void RemoveAllCache()
        {
            using (var client = _client)
            {
                client.FlushAll();
            }
        }

        public void RemoveByKey(string key)
        {
            using (var client = _client)
            {
                client.Remove(key);
            }
        }

        public void RemoveByKeys(IEnumerable<string> keys)
        {
            using (var client = _client)
            {
                client.RemoveAll(keys);
            }
        }

        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="customKey"></param>
        public void SetSysCustomKey(string customKey)
        {
            CustomKey = customKey;
        }

        #endregion 其他

        #region 辅助方法

        private string AddSysCustomKey(string oldKey)
        {
            var prefixKey = CustomKey ?? string.Empty;
            return prefixKey + oldKey;
        }

        private T Do<T>(Func<IRedisClient, T> func)
        {
            return func(_client);
        }

        private void Do(Action<IRedisClient> func)
        {
            func(_client);
        }

        private T ConvertObj<T>(string value)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(Object)value;
            }
            return SerializeHelper.FromJson<T>(value);
        }

        private List<T> ConvetList<T>(string[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }

        private List<string> ConvertRedisKeys(List<string> redisKeys)
        {
            return redisKeys.Select(redisKey => redisKey).ToList();
        }

        #endregion 辅助方法
    }
}
