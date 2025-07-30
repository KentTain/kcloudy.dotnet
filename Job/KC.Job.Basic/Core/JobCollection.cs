using KC.Job.Core;
using KC.Service.Component;
using KC.Job.Basic.Jobs;
using System;
using KC.Component.IRepository;

namespace KC.Job.Basic
{
    public class JobCollection : AbstractJobCollection
    {
        private IServiceProvider _serviceProvider;
        public JobCollection(string hostName, string workerRoleId, IThreadService threadStatusDataStore, IServiceProvider serviceProvider)
            : base(hostName, workerRoleId, threadStatusDataStore)
        {
            _serviceProvider = serviceProvider;
        }

        #region Jobs

        private SampleJob _sampleJob = null;
        private SendSmsJob _sendSmsJob = null;
        private SendEmailJob _sendEmailJob = null;
        
        protected override void LoadJobs()
        {
            //this.ThreadStatusUpdater = new ThreadStatusUpdater(WorkerRoleId);

            //TODO: 根据Tenant的不同创建不同的线程
            this._sampleJob = new SampleJob(ThreadStatusUpdater, WorkerRoleId, _serviceProvider);
            this._sendSmsJob = new SendSmsJob(ThreadStatusUpdater, WorkerRoleId, _serviceProvider);
            this._sendEmailJob = new SendEmailJob(ThreadStatusUpdater, WorkerRoleId, _serviceProvider);

            base.AllJobs.Add(() => base.RoleConfig.EnableConversionMonitor, this._sampleJob);
            base.AllJobs.Add(() => base.RoleConfig.EnableConversionMonitor, this._sendSmsJob);
            base.AllJobs.Add(() => base.RoleConfig.EnableConversionMonitor, this._sendEmailJob);
            
        }

        protected override void KillTimeoutJobs()
        {
            //if (base.RoleConfig.EnableConversionService && _initDatabaseJob != null)
            //{
            //    // abort thread if timeout
            //    if (_initDatabaseJob.ThreadRunningDuration.TotalMinutes > RoleConfig.ConversionTimeoutInMinutes)
            //    {
            //        _initDatabaseJob.AbortJobThread();
            //    }
            //}
        }
        #endregion
    }
}
