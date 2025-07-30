using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Model.Job.Table;
using KC.Component.Repository.Table;
using KC.Component.Repository;
using KC.Job.IRepository;
using AutoMapper;
using KC.Framework.Base;
using KC.Database.EFRepository;
using KC.DataAccess.Job.Repository;

namespace KC.Job.Services
{
    public interface IJobService
    {
        int GetCurrentVersion(string databaseName);
        void UpdateDatabaseStatus(string databaseName, DatabaseStatus status);
        void UpdateDatabaseVersion(string databaseName, int versionNumber);
        bool IsUpgrading(string databaseName);

        #region QueueErrorMessage
        bool AddQueueErrorMessage(QueueErrorMessage model);
        List<QueueErrorMessage> GetAllQueueErrorMessages();
        QueueErrorMessage GetQueueErrorMessage(string rowKey);
        bool RemoveByRowKey(string rowKey);
        PaginatedBase<QueueErrorMessage> FindPagenatedQueueErrorList(int pageIndex,
            int pageSize, string queueName, string sort, string order);
        bool ClearAllQueueErrorMessages();
        #endregion

    }

    public class JobService : StorageServiceBase, IJobService
    {
        private const string ServiceName = "KC.Job.Services.JobService";
        protected IDatabaseVersionInfoRepository DatabaseVersionInfoRepository { get; set; }
        protected IQueueErrorMessageRepository QueueErrorMessageRepository { get; set; }

        public JobService(IMapper mapper, Tenant tenant, EFUnitOfWorkContextBase unitOfWork)
        {
            Tenant = tenant;
            var noSqlType = tenant.NoSqlType;
            switch (noSqlType)
            {
                case NoSqlType.AzureTable:
                    DatabaseVersionInfoRepository = new DatabaseVersionInfoDataContext(tenant);
                    QueueErrorMessageRepository = new QueueErrorMessageDataContext(mapper, tenant);
                    break;
                default:
                    DatabaseVersionInfoRepository = new DatabaseVersionInfoRepository(unitOfWork);
                    QueueErrorMessageRepository = new QueueErrorMessageRepository(unitOfWork);
                    break;
            }
        }

        #region DatabaseVersionInfo

        public int GetCurrentVersion(string databaseName)
        {
            try
            {
                return DatabaseVersionInfoRepository.GetCurrentVersion(databaseName);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return 0;
            }
        }

        public void UpdateDatabaseStatus(string databaseName, DatabaseStatus status)
        {
            try
            {
                DatabaseVersionInfoRepository.UpdateDatabaseStatus(databaseName, status);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
            }
        }

        public void UpdateDatabaseVersion(string databaseName, int versionNumber)
        {
            try
            {
                DatabaseVersionInfoRepository.UpdateDatabaseVersion(databaseName, versionNumber);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
            }
        }

        public bool IsUpgrading(string databaseName)
        {
            try
            {
                return DatabaseVersionInfoRepository.IsUpgrading(databaseName);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        #endregion

        #region QueueErrorMessage

        public bool AddQueueErrorMessage(QueueErrorMessage model)
        {
            try
            {
                return QueueErrorMessageRepository.Add(model);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        public List<QueueErrorMessage> GetAllQueueErrorMessages()
        {
            try
            {
                return QueueErrorMessageRepository.FindAll().ToList();
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return null;
            }
        }

        public bool RemoveByRowKey(string rowKey)
        {
            try
            {
                return QueueErrorMessageRepository.RemoveByRowKey(rowKey);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }
        public PaginatedBase<QueueErrorMessage> FindPagenatedQueueErrorList(int pageIndex, int pageSize, string queueName, string sort, string order)
        {
            return QueueErrorMessageRepository.FindPagenatedListWithCount(pageIndex, pageSize, queueName, sort, order);;
        }

        public QueueErrorMessage GetQueueErrorMessage(string rowKey)
        {
            try
            {
                return QueueErrorMessageRepository.FindByRowKey(rowKey);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return null;
            }
        }

        public bool ClearAllQueueErrorMessages()
        {
            try
            {
                return QueueErrorMessageRepository.RemoveAll();
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ServiceName + " throw exception: " + ex.Message, ex.StackTrace);
                return false;
            }
        }

        #endregion

    }

}
