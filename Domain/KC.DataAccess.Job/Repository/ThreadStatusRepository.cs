using System;
using System.Collections.Generic;
using System.Linq;
using KC.Framework.Tenant;
using KC.Model.Job.Table;
using KC.Job.IRepository;
using KC.Database.EFRepository;

namespace KC.DataAccess.Job.Repository
{
    public class ThreadStatusRepository : EFRepositoryBase<ThreadStatusInfo>, IThreadStatusRepository
    {
        public ThreadStatusRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public List<ThreadStatusInfo> GetThreadStatus()
        {
            return this.FindAll().ToList();
        }

        public void InitializeWorkerRoleThreadStatus(string workerRoleId, string threadname)
        {
            var threadStatus =
                this.FindAll(m => m.WorkerRoleId == workerRoleId && m.ThreadName == threadname).FirstOrDefault();
            if (threadStatus == null)
            {
                var newThreadStatus = new ThreadStatusInfo()
                {
                    Id = Guid.NewGuid().ToString(),
                    WorkerRoleId = workerRoleId,
                    ThreadName = threadname,
                };

                this.Add(newThreadStatus);
            }
        }

        public void UpdateWorkerRoleDate(string workerRoleId)
        {
            var threadStatus = this.FindAll(m => m.WorkerRoleId == workerRoleId);
            foreach (var threadStatusInfo in threadStatus)
            {
                threadStatusInfo.UpdateTime = DateTime.UtcNow;
                this.Modify(threadStatusInfo);
            }
        }

        public void UpdateWorkerRoleThreadStatus(string workerRoleId, string threadname, int successCount, int failCount)
        {
            var threadStatus =
                this.FindAll(m => m.WorkerRoleId == workerRoleId && m.ThreadName == threadname).FirstOrDefault();
            if (threadStatus != null)
            {
                threadStatus.Successes = successCount;
                threadStatus.Failures = failCount;

                this.Modify(threadStatus);
            }
        }

        public void ResetAllWorkerRoleThreadStatus()
        {
            var allThread = this.FindAll().ToList();
            foreach (var thread in allThread)
            {
                thread.UpdateTime = DateTime.UtcNow;
                thread.Successes = 0;
                thread.Failures = 0;

                this.Modify(thread);
            }
        }
    }
}
