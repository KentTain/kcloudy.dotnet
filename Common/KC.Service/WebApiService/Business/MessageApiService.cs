using System.Collections.Generic;
using KC.Common;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.DTO.Account;
using KC.Service.DTO.Message;

namespace KC.Service.WebApiService.Business
{
    public class MessageApiService : IdSrvOAuth2ClientRequestBase, IMessageApiService
    {
        private const string _serviceName = "KC.Service.WebApiService.Business.MessageApiService";

        public MessageApiService(
            Tenant tenant, 
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<MessageApiService> logger)
            : base(tenant, httpClient, logger)
        {
        }

        /// <summary>
        /// 获取文档接口地址：http://[tenantName].msg.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:1107/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        private string _messageServiceUrl
        {
            get
            {
                if (string.IsNullOrEmpty(GlobalConfig.MsgWebDomain))
                    return null;

                return GlobalConfig.MsgWebDomain.Replace(TenantConstant.SubDomain, Tenant.TenantName) + "api/";
            }
        }


        public MessageClassDTO GetMessageClassByCode(string code)
        {
            ServiceResult<MessageClassDTO> result = null;
            WebSendGet<ServiceResult<MessageClassDTO>>(
                _serviceName + ".GetMessageClassByCode",
                _messageServiceUrl + "MessageApi/GetMessageClassByCode?code=" + code,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<MessageClassDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }
        public MessageClassDTO GetMessageClassByName(string name)
        {
            ServiceResult<MessageClassDTO> result = null;
            WebSendGet<ServiceResult<MessageClassDTO>>(
                _serviceName + ".GetMessageClassByName",
                _messageServiceUrl + "MessageApi/GetMessageClassByName?name=" + name,
                ApplicationConstant.MsgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<MessageClassDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        public bool AddMessageLog(MessageLogDTO data)
        {
            var postData = SerializeHelper.ToJson(data);
            ServiceResult<bool> result = null;
            WebSendPost<ServiceResult<bool>>(
                _serviceName + ".AddMessageLog",
                _messageServiceUrl + "MessageApi/AddMessageLog",
                ApplicationConstant.MsgScope,
                postData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return false;
        }

        public bool AddRemindMessages(List<MemberRemindMessageDTO> data)
        {
            var postData = SerializeHelper.ToJson(data);
            ServiceResult<bool> result = null;
            WebSendPost<ServiceResult<bool>>(
                _serviceName + ".AddRemindMessages",
                _messageServiceUrl + "MessageApi/AddRemindMessages",
                ApplicationConstant.MsgScope,
                postData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success)
            {
                return result.Result;
            }

            return false;
        }

    }
}
