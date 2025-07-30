using KC.Component.Repository.File;
using KC.Job.Core;
using KC.Job.IRepository;
using KC.Model.Job.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Component.Repository
{
    public class ThreadFileService : StorageServiceBase, IThreadService
    {
        protected IThreadConfigRepository ThreadConfigInfoRepository { get; set; }
        protected IThreadStatusRepository ThreadStatusInfoRepository { get; set; }

        public ThreadFileService()
        {
            ThreadConfigInfoRepository = new DefaultThreadConfigRepository();
            ThreadStatusInfoRepository = new DefaultThreadStatusRepository();
        }

        #region Thread

        public ThreadConfigInfo GetWorkerRoleConfigToRun(string workerRoleId, string hostName)
        {
            return ThreadConfigInfoRepository.GetWorkerRoleConfigToRun(workerRoleId, hostName);
        }

        public void DeleteWorkerRoleConfigInfo(string rowKey)
        {
            ThreadConfigInfoRepository.DeleteWorkerRoleConfigInfo(rowKey);
        }

        public void DeleteUselessWorkerRoleConfig()
        {
            ThreadConfigInfoRepository.DeleteUselessWorkerRoleConfig();
        }

        public void DeleteAllWorkerRoleConfig()
        {
            ThreadConfigInfoRepository.DeleteAllWorkerRoleConfig();
        }

        public List<ThreadStatusInfo> GetThreadStatus()
        {
            return ThreadStatusInfoRepository.GetThreadStatus();
        }

        public void InitializeWorkerRoleThreadStatus(string workerRoleId, string threadname)
        {
            ThreadStatusInfoRepository.InitializeWorkerRoleThreadStatus(workerRoleId, threadname);
        }

        public void UpdateWorkerRoleDate(string workerRoleId)
        {
            ThreadStatusInfoRepository.UpdateWorkerRoleDate(workerRoleId);
        }

        public void UpdateWorkerRoleThreadStatus(string workerRoleId, string threadname, int successCount, int failCount)
        {
            ThreadStatusInfoRepository.UpdateWorkerRoleThreadStatus(workerRoleId, threadname, successCount, failCount);
        }

        public void ResetAllWorkerRoleThreadStatus()
        {
            ThreadStatusInfoRepository.ResetAllWorkerRoleThreadStatus();
        }

        #endregion
    }
}
