using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Service.EFService;
using KC.Framework.Tenant;
using KC.Job.Core;
using KC.Model.Job.Table;
using KC.Job.IRepository;

namespace KC.Service.Job
{

    public class ThreadDbService : EFServiceBase, IThreadService
    {
        private readonly IThreadConfigRepository _threadConfigInfoRepository;
        private readonly IThreadStatusRepository _threadStatusInfoRepository;
        public ThreadDbService(
            Tenant tenant,

            IThreadConfigRepository threadConfigInfoRepository,
            IThreadStatusRepository threadStatusInfoRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<ThreadDbService> logger)
            : base(tenant, clientFactory, logger)
        {
            _threadConfigInfoRepository = threadConfigInfoRepository;
            _threadStatusInfoRepository = threadStatusInfoRepository;
        }

        #region Thread

        public ThreadConfigInfo GetWorkerRoleConfigToRun(string workerRoleId, string hostName)
        {
            return _threadConfigInfoRepository.GetWorkerRoleConfigToRun(workerRoleId, hostName);
        }

        public void DeleteWorkerRoleConfigInfo(string rowKey)
        {
            _threadConfigInfoRepository.DeleteWorkerRoleConfigInfo(rowKey);
        }

        public void DeleteUselessWorkerRoleConfig()
        {
            _threadConfigInfoRepository.DeleteUselessWorkerRoleConfig();
        }

        public void DeleteAllWorkerRoleConfig()
        {
            _threadConfigInfoRepository.DeleteAllWorkerRoleConfig();
        }

        public List<ThreadStatusInfo> GetThreadStatus()
        {
            return _threadStatusInfoRepository.GetThreadStatus();
        }

        public void InitializeWorkerRoleThreadStatus(string workerRoleId, string threadname)
        {
            _threadStatusInfoRepository.InitializeWorkerRoleThreadStatus(workerRoleId, threadname);
        }

        public void UpdateWorkerRoleDate(string workerRoleId)
        {
            _threadStatusInfoRepository.UpdateWorkerRoleDate(workerRoleId);
        }

        public void UpdateWorkerRoleThreadStatus(string workerRoleId, string threadname, int successCount, int failCount)
        {
            _threadStatusInfoRepository.UpdateWorkerRoleThreadStatus(workerRoleId, threadname, successCount, failCount);
        }

        public void ResetAllWorkerRoleThreadStatus()
        {
            _threadStatusInfoRepository.ResetAllWorkerRoleThreadStatus();
        }

        #endregion
    }
}
