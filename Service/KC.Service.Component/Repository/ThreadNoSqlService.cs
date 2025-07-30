using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Component.Base;
using KC.Model.Job.Table;
using KC.Component.Repository.Table;
using KC.Component.IRepository;
using KC.Component.Repository;
using KC.Job.Core;

namespace KC.Service.Component
{

    public class ThreadNoSqlService : StorageServiceBase, IThreadService
    {
        private const string ServiceName = "KC.Service.Component.ThreadNoSqlService";
        protected ThreadConfigInfoDataContext ThreadConfigInfoRepository { get; set; }
        protected ThreadStatusInfoDataContext ThreadStatusInfoRepository { get; set; }

        public ThreadNoSqlService()
        {
            var storageType = KC.Framework.Base.GlobalConfig.BlobStorage.ToLower();
            switch (storageType)
            {
                case "azure":
                    {
                        ThreadConfigInfoRepository = new ThreadConfigInfoDataContext();
                        ThreadStatusInfoRepository = new ThreadStatusInfoDataContext();
                    }
                    break;
                case "cmb":
                    {
                        ThreadConfigInfoRepository = new ThreadConfigInfoDataContext();
                        ThreadStatusInfoRepository = new ThreadStatusInfoDataContext();
                    }
                    break;
                default:
                    {
                        ThreadConfigInfoRepository = new ThreadConfigInfoDataContext();
                        ThreadStatusInfoRepository = new ThreadStatusInfoDataContext();
                    }
                    break;
            }
        }

        public ThreadNoSqlService(Tenant tenant)
        {
            Tenant = tenant;
            var nosqlType = tenant.NoSqlType;
            switch (nosqlType)
            {
                case NoSqlType.AzureTable:
                    ThreadConfigInfoRepository = new ThreadConfigInfoDataContext(tenant);
                    ThreadStatusInfoRepository = new ThreadStatusInfoDataContext(tenant);
                    break;
                default:
                    ThreadConfigInfoRepository = new ThreadConfigInfoDataContext(tenant);
                    ThreadStatusInfoRepository = new ThreadStatusInfoDataContext(tenant);
                    break;
            }
        }

        #region Thread

        public ThreadConfigInfo GetWorkerRoleConfigToRun(string workerRoleId, string hostName)
        {
            try
            {
                return ThreadConfigInfoRepository.GetWorkerRoleConfigToRun(workerRoleId, hostName);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return null;
            }
        }

        public void DeleteWorkerRoleConfigInfo(string rowKey)
        {
            try
            {
                ThreadConfigInfoRepository.DeleteWorkerRoleConfigInfo(rowKey);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
            }
        }

        public void DeleteUselessWorkerRoleConfig()
        {
            try
            {
                ThreadConfigInfoRepository.DeleteUselessWorkerRoleConfig();
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
            }
        }

        public void DeleteAllWorkerRoleConfig()
        {
            try
            {
                ThreadConfigInfoRepository.DeleteAllWorkerRoleConfig();
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
            }
        }

        public List<ThreadStatusInfo> GetThreadStatus()
        {
            try
            {
                return ThreadStatusInfoRepository.GetThreadStatus();
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return null;
            }
        }

        public void InitializeWorkerRoleThreadStatus(string workerRoleId, string threadname)
        {
            try
            {
                ThreadStatusInfoRepository.InitializeWorkerRoleThreadStatus(workerRoleId, threadname);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
            }
        }

        public void UpdateWorkerRoleDate(string workerRoleId)
        {
            try
            {
                ThreadStatusInfoRepository.UpdateWorkerRoleDate(workerRoleId);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
            }
        }

        public void UpdateWorkerRoleThreadStatus(string workerRoleId, string threadname, int successCount, int failCount)
        {
            try
            {
                ThreadStatusInfoRepository.UpdateWorkerRoleThreadStatus(workerRoleId, threadname, successCount, failCount);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
            }
        }

        public void ResetAllWorkerRoleThreadStatus()
        {
            try
            {
                ThreadStatusInfoRepository.ResetAllWorkerRoleThreadStatus();
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
            }
        }

        #endregion

    }

}
