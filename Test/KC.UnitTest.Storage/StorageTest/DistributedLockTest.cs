using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KC.Component.Util;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.UnitTest.Storage;
using Xunit;
using Microsoft.Extensions.Logging;
using KC.UnitTest;
using KC.Framework.Base;

namespace KC.UnitTest.Storage.Component
{
    public class DistributedLockTest : StorageTestBase
    {
        private int n = 500;//库存：500
        private int icount = 50; //线程数
        //private string exceptResult = Enumerable.Range(0 , icount).Select(i => n - i - 1).ToCommaSeparatedInt();
        private ILogger _logger;
        public DistributedLockTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(DistributedLockTest));
        }

        [Xunit.Fact]
        public void NoDistributedLock_SecKill_Test()
        {
            n = 500;
            var exceptResult = Enumerable.Range(0, icount).Select(i => n - i - 1).ToCommaSeparatedInt();
            Parallel.For(0, icount, i =>
            {
                SecKill("NoDistributedLock_SecKill_Test", SecList.NoKillSet);
            });

            _logger.LogInformation(SecList.NoKillSet.ToCommaSeparatedInt());
            Assert.NotEqual(exceptResult, SecList.NoKillSet.ToCommaSeparatedInt());
        }

        #region Redis 分布式锁

        [Xunit.Fact]
        public void Redis_DistributedLock_SecKill_Test()
        {
            n = 500;//库存：500
            var exceptResult = Enumerable.Range(0, icount).Select(i => n - i - 1).ToCommaSeparatedInt();
            Parallel.For(0, icount, i =>
            {
                new RedisDistributedLock().DoDistributedLock(
                    "Redis_DistributedLock_SecKill_Test",
                    () => { SecKill("Redis_DistributedLock_SecKill_Test", SecList.RedisSecKillSet); });
            });

            _logger.LogInformation(SecList.RedisSecKillSet.ToCommaSeparatedInt());
            Assert.Equal(exceptResult, SecList.RedisSecKillSet.ToCommaSeparatedInt());
        }

        [Xunit.Fact]
        public async Task Redis_DistributedLockAsync_SecKill_Test()
        {
            n = 500;
            var exceptResult = Enumerable.Range(0, icount).Select(i => n - i - 1).ToCommaSeparatedInt();

            //var tasks = Enumerable.Range(0, icount).Select(DoDistributedLockAsync).ToArray();
            var tasks = Enumerable.Range(0, icount).Select(async i =>
            {
                await new RedisDistributedLock().DoDistributedLockAsync(
                    "Redis_DistributedLockAsync_SecKill_Test",
                    () => { SecKill("Redis_DistributedLockAsync_SecKill_Test", SecList.RedisSecKillAsyncSet); });
            }).ToArray();
            await Task.WhenAll(tasks);

            //多线程方法（Parallel.For）不支持异步方式（async/await)：https://stackoverflow.com/questions/37566848/await-parallel-foreach
            //Parallel.For(0, icount, async i =>
            //{
            //    await new RedisDistributedLock().DoDistributedLockAsync(
            //        "Redis_DistributedLockAsync_SecKill_Test",
            //        () => { SecKill("Redis_DistributedLockAsync_SecKill_Test", SecList.RedisSecKillAsyncSet); });
            //});

            _logger.LogInformation(SecList.RedisSecKillAsyncSet.ToCommaSeparatedInt());
            Assert.Equal(exceptResult, SecList.RedisSecKillAsyncSet.ToCommaSeparatedInt());
        }
        private async Task DoDistributedLockAsync(int fileUrl)
        {
            await new RedisDistributedLock().DoDistributedLockAsync(
                    "Redis_DistributedLockAsync_SecKill_Test",
                    () => { SecKill("Redis_DistributedLockAsync_SecKill_Test", SecList.RedisSecKillAsyncSet); });
        }

        #endregion

        /// <summary>
        /// 秒杀
        /// </summary>
        /// <param name="name"></param>
        public void SecKill(string name, List<int> left)
        {
            try
            {
                _logger.LogInformation(string.Format("Thread[{0}--{1}]获得了锁，剩余库存：{2}",
                    name,
                    Thread.CurrentThread.ManagedThreadId.ToString(), //异步下抛出异常
                    (--n).ToString()
                    ));
                left.Add(n);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }
    }


    public static class SecList
    {
        static SecList()
        {
            NoKillSet = new List<int>();
            RedisSecKillSet = new List<int>();
            RedisSecKillAsyncSet = new List<int>();
            SqlServerSecKillSet = new List<int>();
            SqlServerSecKillAsyncSet = new List<int>();
            MySqlSecKillSet = new List<int>();
            MySqlSecKillAsyncSet = new List<int>();
        }
        public static List<int> NoKillSet { get; set; }
        public static List<int> RedisSecKillSet { get; set; }
        public static List<int> RedisSecKillAsyncSet { get; set; }
        public static List<int> SqlServerSecKillSet { get; set; }
        public static List<int> SqlServerSecKillAsyncSet { get; set; }
        public static List<int> MySqlSecKillSet { get; set; }
        public static List<int> MySqlSecKillAsyncSet { get; set; }
    }
}
