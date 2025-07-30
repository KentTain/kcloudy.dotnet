using System.Net;
using KC.Framework.Util;

namespace KC.Job.Core
{
    public abstract class AbstractJobInitializer
    {
        protected string InstanceName;
        protected string HostName;
        protected string WorkerRoleId;
        protected AbstractJobInitializer()
        {
            this.HostName = Dns.GetHostName();
            this.InstanceName = KC.Framework.Base.GlobalConfig.CurrentApplication.AppName;
        }

        public static AbstractJobManager Initialize(AbstractJobInitializer jobInitializer, out bool canRun)
        {
            AbstractJobInitializer initializer = jobInitializer;
            initializer.Start();

            canRun = initializer.CanRun();
            return initializer.CreateJobManager();
        }

        protected abstract void Start();

        protected abstract bool CanRun();

        protected abstract AbstractJobManager CreateJobManager();
    }
}
