using System;
using System.Collections.Generic;
using System.Linq;
using KC.Framework.Tenant;
using KC.Model.Job.Table;
using KC.Model.Component.Table;
using KC.Component.NoSqlRepository;
using KC.Job.IRepository;

namespace KC.Component.Repository.Table
{
    public class ThreadStatusInfoDataContext : CommonTableServiceRepository<ThreadStatusInfoTable>, IThreadStatusRepository
    {
        public ThreadStatusInfoDataContext()
            : base()
        {

        }
        public ThreadStatusInfoDataContext(Tenant tenant)
            : base(tenant)
        {

        }

        public List<ThreadStatusInfo> GetThreadStatus()
        {
            return base.FindAll().Select(m => m as ThreadStatusInfo).ToList();
        }

        public void InitializeWorkerRoleThreadStatus(string workerRoleId, string threadname)
        {
            var threadStatus =
                base.FindAll(m => m.WorkerRoleId == workerRoleId && m.ThreadName == threadname).FirstOrDefault();
            if (threadStatus == null)
            {
                var newThreadStatus = new ThreadStatusInfoTable()
                {
                    Id = Guid.NewGuid().ToString(),
                    WorkerRoleId = workerRoleId,
                    ThreadName = threadname,
                };

                base.Add(newThreadStatus);
            }
        }

        public void UpdateWorkerRoleDate(string workerRoleId)
        {
            var threadStatus = base.FindAll(m => m.WorkerRoleId == workerRoleId);
            foreach (var threadStatusInfo in threadStatus)
            {
                threadStatusInfo.UpdateTime = DateTime.UtcNow;
                base.Modify(threadStatusInfo);
            }
        }

        public void UpdateWorkerRoleThreadStatus(string workerRoleId, string threadname, int successCount, int failCount)
        {
            var threadStatus =
                base.FindAll(m => m.WorkerRoleId == workerRoleId && m.ThreadName == threadname).FirstOrDefault();
            if (threadStatus != null)
            {
                threadStatus.Successes = successCount;
                threadStatus.Failures = failCount;

                base.Modify(threadStatus);
            }
        }

        public void ResetAllWorkerRoleThreadStatus()
        {
            var allThread = base.FindAll().ToList();
            foreach (var thread in allThread)
            {
                thread.UpdateTime = DateTime.UtcNow;
                thread.Successes = 0;
                thread.Failures = 0;

                base.Modify(thread);
            }
        }
    }
}
