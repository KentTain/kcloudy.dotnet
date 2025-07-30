using System.Threading;

namespace KC.Job.Core
{
    public abstract class AbstractJobManager
    {
        protected string InstanceName;
        protected string HostName;
        protected string WorkerRoleId;
        protected ConfigurationMonitor ConfigMonitor;
        protected AbstractJobCollection JobCollection;

        protected IThreadService ThreadService;
        protected AbstractJobManager(string instanceName, string hostName, string workerRoleId, IThreadService threadStatusds)
        {
            this.InstanceName = instanceName;
            this.HostName = hostName;
            this.WorkerRoleId = workerRoleId;
            this.ThreadService = threadStatusds;
            this.ConfigMonitor = new ConfigurationMonitor(instanceName, hostName, workerRoleId, this.ThreadService);

            //SetJobCollection();
        }

        public void OnStart()
        {
            SetJobCollection();

            this.ConfigMonitor.RoleConfigReceived += (sender, e) => { this.JobCollection.Load(this.ConfigMonitor.RoleConfig); };
            this.ConfigMonitor.RoleConfigUpdated += (sender, e) => { this.JobCollection.Set(this.ConfigMonitor.RoleConfig); };
            this.ConfigMonitor.Start();
        }

        public void Run()
        {
            while (!this.JobCollection.IsLoaded)
            {
                Thread.Sleep(1000);
            }

            this.JobCollection.Run();
        }

        public void OnStop()
        {
            if (this.JobCollection.IsLoaded)
            {
                this.JobCollection.Stop();
            }
        }

        protected abstract void SetJobCollection();
    }
}
