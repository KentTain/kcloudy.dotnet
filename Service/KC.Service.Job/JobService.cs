using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using KC.Framework.Util;
using KC.Framework.Tenant;
using KC.Model.Job.Table;
using KC.Job.IRepository;
using AutoMapper;
using KC.Framework.Base;
using KC.Database.EFRepository;
using KC.DataAccess.Job.Repository;
using KC.Service.EFService;

namespace KC.Service.Job
{
    public interface IJobDbService
    {

        #region QueueErrorMessage
        bool RemoveByRowKey(string rowKey);
        PaginatedBase<QueueErrorMessage> FindPagenatedQueueErrorList(int pageIndex,
            int pageSize, string queueName, string sort, string order);
        #endregion

    }

    public class JobDbService : EFServiceBase, IJobDbService
    {
        private const string ServiceName = "KC.Job.Services.JobService";
        protected IDatabaseVersionInfoRepository DatabaseVersionInfoRepository { get; set; }
        protected IQueueErrorMessageRepository QueueErrorMessageRepository { get; set; }

        public JobDbService(
            IMapper mapper, 
            Tenant tenant, 

            IDatabaseVersionInfoRepository databaseVersionInfoRepository,
            IQueueErrorMessageRepository queueErrorMessageRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<JobDbService> logger)
            : base(tenant, clientFactory, logger)
        {
            DatabaseVersionInfoRepository = databaseVersionInfoRepository;
            QueueErrorMessageRepository = queueErrorMessageRepository;
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
            return QueueErrorMessageRepository.FindPagenatedListWithCount(pageIndex, pageSize, queueName, sort, order); ;
        }

    }

}
