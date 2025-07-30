using KC.Model.Job.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Job.Core
{
    public interface IThreadService
    {
        ThreadConfigInfo GetWorkerRoleConfigToRun(string workerRoleId, string hostName);
        void DeleteWorkerRoleConfigInfo(string rowKey);
        void DeleteUselessWorkerRoleConfig();
        void DeleteAllWorkerRoleConfig();


        List<ThreadStatusInfo> GetThreadStatus();
        void InitializeWorkerRoleThreadStatus(string workerRoleId, string threadname);
        void UpdateWorkerRoleDate(string workerRoleId);
        void UpdateWorkerRoleThreadStatus(string workerRoleId, string threadname, int successCount, int failCount);
        void ResetAllWorkerRoleThreadStatus();
    }
}
