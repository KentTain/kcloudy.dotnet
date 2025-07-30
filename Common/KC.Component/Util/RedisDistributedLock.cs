using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Component.Util
{
    /// <summary>
    /// Redis实现的分布式锁
    /// </summary>
    public class RedisDistributedLock : DistributedLockAbstract
    {
        private string _serviceName = "KC.Component.Util.RedisDistributedLock";

        /// <summary>
        ///  Redis的分布式锁
        /// </summary>
        /// <param name="key">锁的key</param>
        /// <param name="acquireTimeout">获取Key的超时时间</param>
        /// <param name="lockTimeOut">锁的过期时间</param>
        /// <param name="action"></param>
        public override void DoDistributedLock(string key, TimeSpan acquireTimeout, TimeSpan lockTimeOut, Action action)
        {
            key = DistributedLockPro + key;
            var value = Guid.NewGuid().ToString();

            var connString = GlobalConfig.GetDecryptRedisConnectionString();
            if (string.IsNullOrEmpty(connString))
                throw new ArgumentNullException("connString", _serviceName + ": Redis connection string is Empty.");
            var redisConn = RedisHelper.GetRedisConnectionString(connString);
            if (string.IsNullOrEmpty(redisConn))
                throw new ArgumentNullException("connString", _serviceName + ": Redis connection string is Empty.");

            var readWriteHosts = new List<string>() { redisConn };
            var end = DateTime.UtcNow.Ticks + acquireTimeout.Ticks;
            var redisHelper = new RedisHelper(10, readWriteHosts);
            while (DateTime.UtcNow.Ticks < end)
            {
                if (redisHelper.TryGetDistributedLock(key, value, lockTimeOut))
                {
                    try
                    {
                        action();
                    }
                    finally
                    {
                        redisHelper.ReleaseDistributedLock(key, value);
                    }

                    return;
                }

                try
                {
                    System.Threading.Thread.Sleep(20);
                }
                catch (Exception e)
                {
                    LogUtil.LogException(e);
                    System.Threading.Thread.CurrentThread.Interrupt();
                }
            }
        }

        /// <summary>
        ///  Redis的分布式锁
        /// </summary>
        /// <param name="key">锁的key</param>
        /// <param name="acquireTimeout">获取Key的超时时间</param>
        /// <param name="lockTimeOut">锁的过期时间</param>
        /// <param name="action"></param>
        public override async Task DoDistributedLockAsync(string key, TimeSpan acquireTimeout, TimeSpan lockTimeOut, Action action)
        {
            key = DistributedLockPro + key;
            var value = Guid.NewGuid().ToString();

            var connString = GlobalConfig.GetDecryptRedisConnectionString();
            if (string.IsNullOrEmpty(connString))
                throw new ArgumentNullException("connString", _serviceName + ": Redis connection string is Empty.");
            var redisConn = RedisHelper.GetRedisConnectionString(connString);
            if (string.IsNullOrEmpty(redisConn))
                throw new ArgumentNullException("connString", _serviceName + ": Redis connection string is Empty.");

            var readWriteHosts = new List<string>() { redisConn };
            var end = DateTime.UtcNow.Ticks + acquireTimeout.Ticks;
            var redisHelper = new RedisHelper(10, readWriteHosts);
            while (DateTime.UtcNow.Ticks < end)
            {
                if (await redisHelper.TryGetDistributedLockAsync(key, value, lockTimeOut))
                {
                    try
                    {
                        action();
                    }
                    finally
                    {
                        await redisHelper.ReleaseDistributedLockAsync(key, value);
                    }

                    return;
                }

                try
                {
                    await Task.Delay(20);
                    //System.Threading.Thread.Sleep(10);
                }
                catch (Exception e)
                {
                    LogUtil.LogException(e);
                    System.Threading.Thread.CurrentThread.Interrupt();
                }
            }
        }
    }
}
