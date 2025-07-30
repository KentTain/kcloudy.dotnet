using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Base;

namespace KC.Service.WebApiService.Business
{
    public class TestComApiService : IdSrvOAuth2ClientRequestBase, ITestComApiService
    {
        private const string _serviceName = "KC.Service.WebApiService.Business.TestComApiService";

        public TestComApiService(
            Tenant tenant, 
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<TestComApiService> logger)
            : base(tenant ?? TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {
        }

        private string _accoutServerUrl
        {
            get
            {
                if (string.IsNullOrEmpty(GlobalConfig.AccWebDomain))
                    return null;

                return GlobalConfig.AccWebDomain.Replace(TenantConstant.SubDomain, Tenant.TenantName) + "api/";
            }
        }

        public ServiceResult<bool> Get()
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                _serviceName + ".Get",
                _accoutServerUrl + "Get",
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

        public async Task<ServiceResult<bool>> GetAsync()
        {
            ServiceResult<bool> result = null;
            await WebSendGetAsync<ServiceResult<bool>>(
                _serviceName + ".GetAsync",
                _accoutServerUrl + "TestApi",
                ApplicationConstant.AccScope,
                DefaultContentType, 
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

        public string GetTestHtml(string title)
        {
            string result = null;
            WebSendGet<string>(
                _serviceName + ".GetTestHtml",
                _accoutServerUrl + "TestApi/GetTestHtml?title=" + title,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = errorMessage;
                },
                false);

            return result;
        }
    }
}
