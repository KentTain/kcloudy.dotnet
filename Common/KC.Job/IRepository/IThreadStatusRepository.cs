using KC.Model.Job.Table;
using System.Collections.Generic;

namespace KC.Job.IRepository
{
    public interface IThreadStatusRepository
    {
        List<ThreadStatusInfo> GetThreadStatus();
        void InitializeWorkerRoleThreadStatus(string workerRoleId, string threadname);
        void ResetAllWorkerRoleThreadStatus();
        void UpdateWorkerRoleDate(string workerRoleId);
        void UpdateWorkerRoleThreadStatus(string workerRoleId, string threadname, int successCount, int failCount);
    }
}