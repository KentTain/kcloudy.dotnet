using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Tenant;
using KC.Service.Constants;
using KC.Service.DTO;
using KC.Service.DTO.Message;
using KC.Service.Enums.Message;
using KC.Service.WebApiService.Business;

namespace KC.Service.Portal.WebApiService.Business
{
    public interface INewsBulletinApiService
    {
        /// <summary>
        /// 获取最新的新闻公告：缓存12小时
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler(Constants.TimeOutConstants.CacheTimeOut)]
        Task<List<NewsBulletinDTO>> LoadLatestNewsBulletins(NewsBulletinType? type);

        Task<PaginatedBaseDTO<NewsBulletinDTO>> LoadPaginatedNewsBulletins(int pageIndex, int pageSize, string title, NewsBulletinType? type);

        Task<List<NewsBulletinDTO>> GetNewsBulletinById(int id);
    }

    public class NewsBulletinApiService : IdSrvOAuth2ClientRequestBase, INewsBulletinApiService
    {
        private const string ServiceName = "KC.Service.Portal.WebApiService.Business.NewsBulletinApiService";

        public NewsBulletinApiService(
            Tenant tenant, 
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<NewsBulletinApiService> logger)
            : base(tenant, httpClient, logger)
        {
        }


        /// <summary>
        /// 获取最新的新闻公告
        /// </summary>
        /// <returns></returns>
        public async Task<List<NewsBulletinDTO>> LoadLatestNewsBulletins(NewsBulletinType? type)
        {
            ServiceResult<List<NewsBulletinDTO>> result = null;
            var t = type.HasValue ? ((int)type.Value).ToString() : string.Empty;
            await WebSendGetAsync<ServiceResult<List<NewsBulletinDTO>>>(
                ServiceName + ".LoadLatestNewsBulletins",
                MessageApiUrl + "NewsBulletinApi/LoadLatestNewsBulletins?type=" + t,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<NewsBulletinDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                }, true);
            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return new List<NewsBulletinDTO>();
        }

        /// <summary>
        /// 获取最新的新闻公告
        /// </summary>
        /// <returns></returns>
        public async Task<PaginatedBaseDTO<NewsBulletinDTO>> LoadPaginatedNewsBulletins(int pageIndex, int pageSize, string title, NewsBulletinType? type)
        {
            ServiceResult<PaginatedBaseDTO<NewsBulletinDTO>> result = null;
            var t = type.HasValue ? ((int)type.Value).ToString() : string.Empty;
            await WebSendGetAsync<ServiceResult<PaginatedBaseDTO<NewsBulletinDTO>>>(
                ServiceName + ".LoadLatestNewsBulletins",
                MessageApiUrl + "NewsBulletinApi/LoadPaginatedNewsBulletins?pageIndex=" + pageIndex + "&pageSize=" + pageSize + "&title=" + title + "&type=" + t,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<NewsBulletinDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                }, true);
            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return new PaginatedBaseDTO<NewsBulletinDTO>();
        }

        /// <summary>
        /// 获取最新的新闻公告
        /// </summary>
        /// <returns></returns>
        public async Task<List<NewsBulletinDTO>> GetNewsBulletinById(int id)
        {
            ServiceResult<List<NewsBulletinDTO>> result = null;
            await WebSendGetAsync<ServiceResult<List<NewsBulletinDTO>>>(
                ServiceName + ".GetNewsBulletinById",
                MessageApiUrl + "NewsBulletinApi/GetNewsBulletinById?id=" + id,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<NewsBulletinDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                }, true);
            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return new List<NewsBulletinDTO>();
        }
    }
}
