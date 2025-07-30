using KC.Component.IRepository;
using KC.Job.Core;
using KC.Service.Component;
using System;


namespace KC.Job.Basic
{
    public class JobManager : AbstractJobManager
    {
        private IServiceProvider _serviceProvider;
        public JobManager(string instanceName, string hostName, string workerRoleId, IServiceProvider serviceProvider, IThreadService threadStatusds)
            : base(instanceName, hostName, workerRoleId, threadStatusds)
        {
            _serviceProvider = serviceProvider;
        }

        protected override void SetJobCollection()
        {
            JobCollection = new JobCollection(base.HostName, base.WorkerRoleId, base.ThreadService, this._serviceProvider);
        }
    }
}
