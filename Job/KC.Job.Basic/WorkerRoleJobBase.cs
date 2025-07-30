using System;
using System.Threading;
using KC.Job.Core;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Job.Basic
{
    public abstract class WorkerRoleJobBase : JobBase
    {
        protected IServiceProvider ServiceProvider;

        protected WorkerRoleJobBase(ThreadStatusUpdater threadStatusUpdater, string workerRoleId, string threadName, int threadSleepMilliseconds, IServiceProvider serviceProvider)
            : base(threadStatusUpdater, workerRoleId, threadName, threadSleepMilliseconds)
        {
            ServiceProvider = serviceProvider;
        }

        protected WorkerRoleJobBase(ThreadStatusUpdater threadStatusUpdater, string workerRoleId, string threadName, int threadSleepMilliseconds, DateTime? beginRunDateTime, IServiceProvider serviceProvider)
            : base(threadStatusUpdater, workerRoleId, threadName, threadSleepMilliseconds, beginRunDateTime)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
