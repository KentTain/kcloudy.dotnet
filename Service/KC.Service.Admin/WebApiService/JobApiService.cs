using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Job.Table;
using KC.Service.Base;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.Configuration;

namespace KC.Service.Admin.WebApiService
{
    public interface IJobApiService
    {
        PaginatedBase<QueueErrorMessage> LoadPagenatedQueueErrors(int page, int rows, string queueName);
        ServiceResult<bool> RemoveByRowKey(string rowKey);
    }

    public class JobApiService : IdSrvOAuth2ClientRequestBase, IJobApiService
    {
        private const string ServiceName = "KC.Service.Admin.WebApiService.JobApiService";
        private readonly IConfiguration _configuration;
        public JobApiService(
            Tenant tenant,
            IConfiguration configuration,
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<JobApiService> logger)
            : base(tenant ?? TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {
            _configuration = configuration;
        }

        public PaginatedBase<QueueErrorMessage> LoadPagenatedQueueErrors(int page, int rows, string queueName)
        {
            var jobApiUrl = _configuration["AppSettings:JobWebApiUrl"]; 
            ServiceResult<PaginatedBase<QueueErrorMessage>> result = null;
            WebSendGet<ServiceResult<PaginatedBase<QueueErrorMessage>>>(
                ServiceName + ".LoadPagenatedQueueErrors",
                jobApiUrl + "LoadPagenatedQueueErrors?page=" + page + "&rows=" + rows + "&queueName=" + queueName,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBase<QueueErrorMessage>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        public ServiceResult<bool> RemoveByRowKey(string rowKey)
        {
            var jobApiUrl = _configuration["AppSettings:JobWebApiUrl"];
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".RemoveByRowKey",
                jobApiUrl + "RemoveByRowKey?rowKey=" + rowKey,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            return result;
        }

    }
}
