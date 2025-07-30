using System;
using System.Collections.Generic;
using System.Threading;
using KC.Framework.Util;
using KC.Model.Job.Table;

namespace KC.Job.Core
{
    public abstract class AbstractJobCollection
    {
        #region Properties
        protected readonly string HostName;
        protected readonly string WorkerRoleId;
        protected ThreadConfigInfo RoleConfig;

        protected ThreadStatusUpdater ThreadStatusUpdater;

        /// <summary> ThreadStatusUpdater is not inside</summary>
        protected Dictionary<Func<bool>, JobBase> AllJobs = new Dictionary<Func<bool>, JobBase>();

        public bool IsLoaded { get; private set; }
        public bool IsConversionLoaded { get; private set; }
        public bool IsStopping { get; private set; }

        #endregion

        protected AbstractJobCollection(string hostName, string workerRoleId, IThreadService threadStatusDataStore)
        {
            this.IsLoaded = false;
            this.IsConversionLoaded = false;
            this.IsStopping = false;

            this.HostName = hostName;
            this.WorkerRoleId = workerRoleId;
            this.ThreadStatusUpdater = new ThreadStatusUpdater(WorkerRoleId, threadStatusDataStore);
        }

        #region public Methods
        public void Load(ThreadConfigInfo workerRoleConfig)
        {
            if (workerRoleConfig != null)
                this.RoleConfig = workerRoleConfig;

            if (this.RoleConfig != null)
            {
                if (!this.IsLoaded)
                {
                    this.IsLoaded = true;

                    this.LoadJobs();
                }
            }
        }

        public void Set(ThreadConfigInfo config)
        {
            this.RoleConfig = config;
            this.RefreshRoleConfig();
        }

        public void Run()
        {
            while (!this.IsStopping)
            {
                if (this.IsLoaded && this.RoleConfig.IsEnabled)
                {
                    try
                    {
                        foreach (var jobEntry in this.AllJobs)
                        {
                            JobBase job = jobEntry.Value;
                            if (jobEntry.Key() && job != null && !job.IsThreadRunning)
                            {
                                job.RunOneJobUseThread();
                            }
                        }

                        // Note: ThreadStatusUpdater is not inside _allJobs
                        if (this.ThreadStatusUpdater != null && !this.ThreadStatusUpdater.IsThreadRunning)
                        {
                            this.ThreadStatusUpdater.RunOneJobUseThread();
                        }

                        this.KillTimeoutJobs();
                    }
                    catch (Exception ex)
                    {
                        //ErrorEmailHelper.SendErrorEmail(this._hostName, "WorkerRole", "Run", ex);
                        LogUtil.LogException(ex);
                    }
                }

                Thread.Sleep(this.RoleConfig.ConversionCheckInterval);
            }
        }

        public void Stop()
        {
            this.IsStopping = true;
            while (this.HasRunningJob())
            {
                Thread.Sleep(10);
            }
        }

        #endregion

        #region Jobs

        private bool HasRunningJob()
        {
            foreach (var jobEntry in AllJobs)
            {
                JobBase job = jobEntry.Value;
                if (job != null && job.IsThreadRunning)
                {
                    return true;
                }
            }
            return false;
        }

        private void RefreshRoleConfig()
        {

        }

        private void RefreshConversion()
        {
            this.ThreadStatusUpdater.SetConversionAuthToken();
        }

        #endregion

        protected abstract void LoadJobs();
        protected abstract void KillTimeoutJobs();
    }
}
