using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Framework.Util
{
    interface IDistributedLock
    {
        void DoDistributedLock(string key, Action action);
        void DoDistributedLock(string key, TimeSpan acquireTimeout, Action action);
        void DoDistributedLock(string key, TimeSpan acquireTimeout, TimeSpan lockTimeOut, Action action);

        Task DoDistributedLockAsync(string key, Action action);
        Task DoDistributedLockAsync(string key, TimeSpan acquireTimeout, Action action);
        Task DoDistributedLockAsync(string key, TimeSpan acquireTimeout, TimeSpan lockTimeOut, Action action);
    }

    public abstract class DistributedLockAbstract : IDistributedLock
    {
        public TimeSpan DefaultAcuireTimeOut = new TimeSpan(0, 10, 0);
        public TimeSpan DefaultLockTimeOut = new TimeSpan(0, 1, 0);
        public static string DistributedLockPro = "D_Lock_";

        public void DoDistributedLock(string key, Action action)
        {
            DoDistributedLock(key, DefaultAcuireTimeOut, DefaultLockTimeOut, action);
        }
        public void DoDistributedLock(string key, TimeSpan acquireTimeout, Action action)
        {
            DoDistributedLock(key, acquireTimeout, DefaultLockTimeOut, action);
        }
        public abstract void  DoDistributedLock(string key, TimeSpan acquireTimeout, TimeSpan lockTimeOut, Action action);

        public async Task DoDistributedLockAsync(string key, Action action)
        {
            await DoDistributedLockAsync(key, DefaultAcuireTimeOut, DefaultLockTimeOut, action);
        }
        public async Task DoDistributedLockAsync(string key, TimeSpan acquireTimeout, Action action)
        {
            await DoDistributedLockAsync(key, acquireTimeout, DefaultLockTimeOut, action);
        }
        public abstract Task DoDistributedLockAsync(string key, TimeSpan acquireTimeout, TimeSpan lockTimeOut, Action action);
    }
}
