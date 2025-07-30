using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Component.Util;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Component.Queue;
using KC.Service.Component;
using KC.UnitTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace KC.UnitTest.Storage.Component
{

    public class RedisHelperTest : KC.UnitTest.Storage.StorageTestBase
    {
        private static IStorageQueueService storageQueueService;
        private static IStorageQueueService BuyStorageQueueService;
        private ILogger _logger;
        public RedisHelperTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(RedisHelperTest));
        }

        protected override void SetUp()
        {
            base.SetUp();

            storageQueueService = Services.BuildServiceProvider().GetService<IStorageQueueService>();//返回调用者

            InjectTenant(BuyTenant);
            Services.AddScoped<IStorageQueueService, StorageQueueService>();
            BuyStorageQueueService = Services.BuildServiceProvider().GetService<IStorageQueueService>();//返回调用者
        }


        [Xunit.Fact]
        public void Test_RedisHelper_String()
        {
            var redisHelper = GetRedisHelper(3);

            var cachedStringKey = "redis_string_key";
            var cachedStringValue = "测试Redis缓存";
            redisHelper.StringSet(cachedStringKey, cachedStringValue);
            var stringResult = redisHelper.StringGet(cachedStringKey);
            Assert.Equal(cachedStringValue, stringResult);
            redisHelper.StringRemove(cachedStringKey);

            var cachedObjKey = "redis_object_key";
            var cachedObjValue = new BlobInfo("1", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "测试Redis缓存", "txt");
            redisHelper.StringSet(cachedObjKey, cachedObjValue);
            var objResult = redisHelper.StringGet<BlobInfo>(cachedObjKey);
            Assert.Equal(cachedObjValue.Id, objResult.Id);
            Assert.Equal(cachedObjValue.FileType, objResult.FileType);
            Assert.Equal(cachedObjValue.FileName, objResult.FileName);
            redisHelper.StringRemove(cachedObjKey);

            var cachedIncrementKey = "redis_increment_key";
            var cachedIncrementValue = 2;
            redisHelper.StringIncrement(cachedIncrementKey, cachedIncrementValue);
            var incrResult = redisHelper.StringGet<double>(cachedIncrementKey);
            Assert.Equal(cachedIncrementValue, incrResult);
            redisHelper.StringRemove(cachedIncrementKey);

            var cachedListKey = "redis_list_key";
            var cachedListValue = new List<string>() { "1", "2", "3", "4" };
            redisHelper.StringSet(cachedListKey, cachedListValue);
            var listResult = redisHelper.StringGet<List<string>>(cachedListKey);
            Assert.Equal(cachedListValue, listResult);
            var listResult2 = redisHelper.StringGet(typeof(List<string>), cachedListKey);
            Assert.Equal(cachedListValue, listResult2);
            redisHelper.StringRemove(cachedListKey);

        }

        [Xunit.Fact]
        public void Test_RedisHelper_List()
        {
            var redisHelper = GetRedisHelper();
            var blockingExpired = TimeSpan.FromSeconds(1);

            #region list for string
            var cachedListKey = "redis_list_key_string";

            redisHelper.ListAdd(cachedListKey, "List-1", null);
            redisHelper.ListAdd(cachedListKey, "List-2", null);
            redisHelper.ListAdd(cachedListKey, "List-3", null);
            redisHelper.ListAdd(cachedListKey, "List-4", null);
            var incrResult1 = redisHelper.ListBlockingGet(cachedListKey, blockingExpired);
            Assert.Equal("List-4", incrResult1);
            incrResult1 = redisHelper.ListBlockingGet(cachedListKey, blockingExpired);
            Assert.Equal("List-3", incrResult1);
            incrResult1 = redisHelper.ListBlockingGet(cachedListKey, blockingExpired);
            Assert.Equal("List-2", incrResult1);
            incrResult1 = redisHelper.ListBlockingGet(cachedListKey, blockingExpired);
            Assert.Equal("List-1", incrResult1);

            redisHelper.ListRemoveAll(cachedListKey);
            #endregion

            #region list for object
            var cachedObjKey = "redis_list_key_object";

            redisHelper.ListAdd(cachedObjKey, new BlobInfo("1", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "1.测试Redis缓存", "txt"), null);
            redisHelper.ListAdd(cachedObjKey, new BlobInfo("2", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "2.测试Redis缓存", "txt"), null);
            redisHelper.ListAdd(cachedObjKey, new BlobInfo("3", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "3.测试Redis缓存", "txt"), null);
            redisHelper.ListAdd(cachedObjKey, new BlobInfo("4", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "4.测试Redis缓存", "txt"), null);
            var objResult1 = redisHelper.ListBlockingGet<BlobInfo>(cachedObjKey, blockingExpired);
            Assert.Equal("4", objResult1.Id);
            objResult1 = redisHelper.ListBlockingGet<BlobInfo>(cachedObjKey, blockingExpired);
            Assert.Equal("3", objResult1.Id);
            objResult1 = redisHelper.ListBlockingGet<BlobInfo>(cachedObjKey, blockingExpired);
            Assert.Equal("2", objResult1.Id);
            objResult1 = redisHelper.ListBlockingGet<BlobInfo>(cachedObjKey, blockingExpired);
            Assert.Equal("1", objResult1.Id);

            redisHelper.ListRemoveAll(cachedObjKey);
            #endregion
        }

        [Xunit.Fact]
        public void Test_RedisHelper_SortedSet()
        {
            var redisHelper = GetRedisHelper();

            #region set for string
            var cachedListKey = "redis_sortedset_key_string";

            redisHelper.SortedSetAdd(cachedListKey, "List-1", 1);
            redisHelper.SortedSetAdd(cachedListKey, "List-2", 2);
            redisHelper.SortedSetAdd(cachedListKey, "List-3", 3);
            redisHelper.SortedSetAdd(cachedListKey, "List-4", 4);
            var listResult = redisHelper.SortedSetGet(cachedListKey, true);
            for (var i = 1; i <= 4; i++)
            {
                Assert.Equal("List-" + i, listResult[i - 1]);
            }
            var listSortedResult = redisHelper.SortedSetGet(cachedListKey, false);
            for (var i = 4; i <= 0; i--)
            {
                Assert.Equal("List-" + i, listSortedResult[i - 1]);
            }

            redisHelper.SortedSetRemove(cachedListKey);
            #endregion

            #region set for object
            var cachedObjKey = "redis_sortedset_key_object";

            redisHelper.SortedSetAdd(cachedObjKey, new BlobInfo("1", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "1.测试Redis缓存", "txt"), 1);
            redisHelper.SortedSetAdd(cachedObjKey, new BlobInfo("2", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "2.测试Redis缓存", "txt"), 2);
            redisHelper.SortedSetAdd(cachedObjKey, new BlobInfo("3", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "3.测试Redis缓存", "txt"), 3);
            redisHelper.SortedSetAdd(cachedObjKey, new BlobInfo("4", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "4.测试Redis缓存", "txt"), 4);
            var listObjResult = redisHelper.SortedSetGet<BlobInfo>(cachedObjKey, true);
            for (var i = 1; i <= 4; i++)
            {
                Assert.Equal(i.ToString(), listObjResult[i - 1].Id);
            }
            var listObjSortedResult = redisHelper.SortedSetGet<BlobInfo>(cachedObjKey, false);
            for (var i = 4; i <= 0; i--)
            {
                Assert.Equal(i.ToString(), listObjSortedResult[i - 1].Id);
            }

            redisHelper.SortedSetRemove(cachedObjKey);
            #endregion
        }

        //[Xunit.Fact]
        [Xunit.Fact(Skip = "Redis订阅会阻塞线程，所以需要忽略不执行，需要测试时，单独执行该方法")]
        public void Test_RedisHelper_PubSub_String()
        {
            var redisHelper = GetRedisHelper(5);
            var cachedListKey = "redis_pubsub_key_string";

            //模拟：发送100条消息给服务
            Task.Run(() =>
            {
                var redisHelper = GetRedisHelper(5);
                for (int i = 1; i <= 10; i++)
                {
                    redisHelper.Publish(cachedListKey, string.Format("这是我发送的第{0}消息!", i));
                    System.Threading.Thread.Sleep(200);
                }

                //sub.UnSubscribeFromChannels(cachedListKey);
            });

            redisHelper.Subscribe(cachedListKey, (chanel, result) =>
            {
                _logger.LogDebug("---subscribe the chanel: " + chanel + ", get value: " + result);
            });
        }

        //[Xunit.Fact]
        [Xunit.Fact(Skip = "Redis订阅会阻塞线程，所以需要忽略不执行，需要测试时，单独执行该方法")]
        public void Test_RedisHelper_PubSub_Object()
        {
            var redisHelper = GetRedisHelper(5);
            var blockingExpired = TimeSpan.FromSeconds(1);

            #region Publish for object
            var cachedObjKey = "redis_pubsub_key_object";

            Task.Run(() =>
            {
                var redisHelper = GetRedisHelper(5);
                redisHelper.Publish(cachedObjKey, new BlobInfo("1", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "1.测试Redis缓存", "txt"));
                System.Threading.Thread.Sleep(200);
                redisHelper.Publish(cachedObjKey, new BlobInfo("1", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "1.测试Redis缓存", "txt"));
                System.Threading.Thread.Sleep(200);
                redisHelper.Publish(cachedObjKey, new BlobInfo("1", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "1.测试Redis缓存", "txt"));
                System.Threading.Thread.Sleep(200);
                redisHelper.Publish(cachedObjKey, new BlobInfo("1", Guid.NewGuid().ToString(), DateTime.UtcNow.ToString(), "1.测试Redis缓存", "txt"));
                System.Threading.Thread.Sleep(200);
                //sub.UnSubscribeFromChannels(cachedListKey);
            });
            #endregion

            #region Subscribe for object
            redisHelper.Subscribe<BlobInfo>(cachedObjKey, (chanel, result) =>
            {
                _logger.LogDebug("---subscribe the chanel: " + chanel + ", get value: " + result.FileName);
            });

            #endregion
        }

        private RedisHelper GetRedisHelper(int databaseId = 0)
        {
            var redisConnString = GlobalConfig.GetDecryptRedisConnectionString();
            var keyValues = redisConnString.KeyValuePairFromConnectionString();
            var endpoint = keyValues[ConnectionKeyConstant.RedisEndpoint];
            var user = keyValues[ConnectionKeyConstant.AccessName];
            var password = keyValues[ConnectionKeyConstant.AccessKey];
            var connstring = endpoint;
            if (!password.IsNullOrEmpty())
            {
                connstring = user + ":" + password + "@" + endpoint;
            }
            _logger.LogInformation(connstring);
            return new RedisHelper(databaseId, new List<string> { connstring });
        }
    }
}
