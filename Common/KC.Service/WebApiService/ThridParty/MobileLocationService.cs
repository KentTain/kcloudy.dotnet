using KC.Framework.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.WebApiService.ThridParty
{
    public class MobileLocationService : OAuth2ClientRequestBase
    {
        protected const string ServiceName = "KC.Service.WebApiService.ThridParty.MobileLocationService";
        public MobileLocationService(
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<MobileLocationService> logger)
            : base(httpClient, logger)
        {
        }

        protected override OAuth2ClientInfo GetOAuth2ClientInfo()
        {
            throw new NotImplementedException();
        }

        private string apiKey = "2fcc2602712c2643b924fe080a67c782"; //http://apistore.baidu.com
        private string ApiServerUrl = @"http://apis.baidu.com/apistore/mobilenumber/mobilenumber";
        public ServiceResult<MobileLocationResult> GetMobileLocation(string mobilePhone)
        {
            var headers =new Dictionary<string, string>();
            headers.Add("apikey", apiKey);
            ServiceResult<MobileLocationResult> result = null;
            WebSendGet<MobileLocationResult>(
                ServiceName + ".GetMobileLocation",
                ApiServerUrl + "?phone=" + mobilePhone,
                ApplicationConstant.DicScope,
                DefaultContentType,
                headers,
                callback =>
                {
                    result = new ServiceResult<MobileLocationResult>(ServiceResultType.Success, null, callback);
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<MobileLocationResult>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                false);

            return result;
        }

        
    }

    public class MobileLocationResult
    {
        public int errNum { get; set; }
        /// <summary>
        /// success获取failure
        /// </summary>
        public string retMsg { get; set; }
        public MobileLocationData retData { get; set; }
    }

    public class MobileLocationData
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 手机号码前7位
        /// </summary>
        public string prefix { get; set; }
        /// <summary>
        /// 移动 
        /// </summary>
        public string supplier { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 152卡
        /// </summary>
        public string suit { get; set; }
    }
}
