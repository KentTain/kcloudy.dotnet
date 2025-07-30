using AutoMapper;
using KC.Common;
using KC.Service.DTO.Contract;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Contract;
using KC.Service;
using KC.Service.Constants;
using KC.Service.WebApiService;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KC.Enums.Contract;

namespace KC.Service.Contract.WebApiService.ThridParty
{
    public interface IElectronicSignApiService
    {
        #region E签宝个人账户
        /// <summary>
        /// 创建电子签章个人账号
        /// </summary>
        /// <param name="personInfo">E签宝个人账户信息</param>
        /// <returns></returns>
        Task<ElectronicSignResponse> CreatePersonAccount(ElectronicPersonDTO personInfo);
        /// <summary>
        /// 修改电子签章个人账号
        /// </summary>
        /// <param name="personInfo">E签宝个人账户信息</param>
        /// <returns></returns>
        Task<ElectronicSignResponse> EditPersonAccount(ElectronicPersonDTO personInfo);
        /// <summary>
        /// 注销电子签章个人账号
        /// </summary>
        /// <param name="accountId">E签宝用户Id</param>
        /// <returns></returns>
        Task<ElectronicSignResponse> RemovePersonAccount(string accountId);
        /// <summary>
        /// 创建个人电子签章
        /// </summary>
        /// <param name="accountId">E签宝个人账户Id</param>
        /// <returns></returns>
        Task<ElectronicSignResponse> CreatePersonSeal(string accountId);
        /// <summary>
        /// 注销个人电子签章
        /// </summary>
        /// <param name="accountId">E签宝个人账户Id</param>
        /// <param name="sealId">E签宝印章Id</param>
        /// <returns></returns>
        Task<ElectronicSignResponse> RemovePersonSeal(string accountId, string sealId);
        #endregion

        #region E签宝机构账户
        /// <summary>
        /// 创建电子签章机构账号
        /// </summary>
        /// <param name="orgInfo">E签宝机构信息</param>
        /// <returns></returns>
        Task<ElectronicSignResponse> CreateOrganizationAccount(ElectronicOrganizationDTO orgInfo);
        /// <summary>
        /// 修改电子签章机构账号
        /// </summary>
        /// <param name="orgInfo">E签宝机构信息</param>
        /// <returns></returns>
        Task<ElectronicSignResponse> EditOrganizationAccount(ElectronicOrganizationDTO orgInfo);
        /// <summary>
        /// 注销电子签章机构账号
        /// </summary>
        /// <param name="orgId">E签宝机构账户Id</param>
        /// <returns></returns>
        Task<ElectronicSignResponse> RemoveOrganizationAccount(string orgId);
        /// <summary>
        /// 创建机构电子签章
        /// </summary>
        /// <param name="orgId">E签宝机账户Id</param>
        /// <param name="qtext">横向文：机构全称</param>
        /// <param name="orgId">下弦文：机构代码</param>
        /// <returns></returns>
        Task<ElectronicSignResponse> CreateOrganizationSeal(string orgId, string qtext, string htext);
        /// <summary>
        /// 注销机构电子签章
        /// </summary>
        /// <param name="orgId">E签宝机构账户Id</param>
        /// <param name="sealId">E签宝印章Id</param>
        /// <returns></returns>
        Task<ElectronicSignResponse> RemoveOrganizationSeal(string orgId, string sealId);
        #endregion

        ServiceResult<int> ConfirmCurrencySign(ContractGroupDTO model, bool isStatu = false);
        ServiceResult<bool> FindContractByNo(string contractNo, string id);
        ServiceResult<bool> RemoveCurrencySignService(string id);
        ServiceResult<Tuple<int, string, byte[]>> AddSignatureToPdf(byte[] buffer0, string FileName, string posX, string posY, string key = "", int signType = 3, string code = "", string posPage = "0", string xCoordinate = "0", string tenantName = "", bool personal = false);
    }

    public class ElectronicSignApiService : OAuth2ClientRequestBase, IElectronicSignApiService
    {
        private const string ServiceName = "KC.Service.WebApiService.ElectronicSignApiService";
        private readonly IMapper _mapper;

        public Tenant Tenant { get; set; }

        public ElectronicSignApiService(
            Tenant tenant,
            IMapper mapper,
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<ElectronicSignApiService> logger)
            : base(httpClient, logger)
        {
            _mapper = mapper;
            Tenant = tenant;
        }

        private const string grantType = "client_credentials";
        private const string clientId = "4438771934";
        private const string clientSecret = "fa3c091d439791a450c6591467716217";
        private const string esignApiDomain = "https://openapi.esign.cn";
        protected override OAuth2ClientInfo GetOAuth2ClientInfo()
        {
            string tenantName = Tenant.TenantName;
            string credential = TenantConstant.Sha256(clientSecret);
            string tokenEndpoint = esignApiDomain + "/v1/oauth2/access_token";

            return new OAuth2ClientInfo(tenantName, clientId, clientSecret, tokenEndpoint, grantType);
        }

        /// <summary>
        /// 获取accesstoken
        /// </summary>
        /// <param name="clientScope"></param>
        /// <returns></returns>
        protected override async Task<string> GetAccessTokenAsync(string clientScope)
        {
            var clientInfo = GetOAuth2ClientInfo();
            var tenantName = clientInfo.TenantName;
            var tokenEndpoint = clientInfo.TokenEndpoint;
            var clientId = clientInfo.ClientId;
            var clientSecret = clientInfo.ClientSecret;

            HttpClient client = httpClientFactory != null 
                ? httpClientFactory.CreateClient() 
                : new HttpClient();

            var accessTokenCacheKey = CacheKeyConstants.Prefix.TenantAccessToken + "esign-" + clientId;
            var accessToken = Service.CacheUtil.GetCache<string>(accessTokenCacheKey);
            if (string.IsNullOrEmpty(accessToken))
            {
                var tokenUrl = tokenEndpoint + string.Format("?appId={0}&secret={1}&grantType=client_credentials", clientId, clientSecret);
                var responseString = await client.GetStringAsync(tokenUrl);
                var result = SerializeHelper.FromJson<ElectronicSignResponse>(responseString);
                if (result.code == 0 && result.data != null)
                {
                    accessToken = result.data.token;
                    var expiredTimeSpan = result.data.expiresIn - 100;
                    Service.CacheUtil.SetCache(accessTokenCacheKey, accessToken, TimeSpan.FromSeconds(expiredTimeSpan));
                }
                
                Logger?.LogDebug(Framework.Base.GlobalConfig.CurrentApplication?.AppName + "|" + ServiceName+ " get accessToken: " + accessToken);
            }

            return accessToken;
        }
        /// <summary>
        /// 设置访问E签宝接口所需的Token数据
        /// </summary>
        /// <returns></returns>
        private async Task<Dictionary<string, string>> GetHeaders()
        {
            var clientInfo = GetOAuth2ClientInfo();
            var tenantName = clientInfo.TenantName;
            var tokenEndpoint = clientInfo.TokenEndpoint;
            var clientId = clientInfo.ClientId;
            var clientSecret = clientInfo.ClientSecret;
            var accessToken = await GetAccessTokenAsync(null);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Tsign-Open-App-Id", clientId);
            headers.Add("X-Tsign-Open-Token", accessToken);
            return headers;
        }

        #region E签宝个人账户
        /// <summary>
        /// 创建电子签章个人账号
        /// </summary>
        /// <param name="personInfo">E签宝个人账户信息</param>
        /// <returns></returns>
        public async Task<ElectronicSignResponse> CreatePersonAccount(ElectronicPersonDTO personInfo)
        {
            if (personInfo == null)
                return new ElectronicSignResponse(404, "传入的参数：orgInfo 为空，无法创建电子签章机构账号");

            var headers = await GetHeaders();
            var postData = string.Format("{{'thirdPartyUserId':'{0}','name':'{1}','email':'{2}','mobile':'{3}','idNumber':'{4}','idType':'CRED_PSN_CH_IDCARD'}}", personInfo.UserId, personInfo.UserName, personInfo.Email, personInfo.Mobile, personInfo.IdentityNumber);
            
            ElectronicSignResponse result = null;
            await WebSendPostAsync<ElectronicSignResponse>(
                ServiceName + ".CreatePersonAccount",
                esignApiDomain + "/v1/accounts/createByThirdPartyUserId",
                ApplicationConstant.AccScope,
                postData,
                headers,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ElectronicSignResponse((int)httpStatusCode, errorMessage);
                },
                false);

            return result;
        }

        /// <summary>
        /// 修改电子签章个人账号
        /// </summary>
        /// <param name="personInfo">E签宝个人账户信息</param>
        /// <returns></returns>
        public async Task<ElectronicSignResponse> EditPersonAccount(ElectronicPersonDTO personInfo)
        {
            if (personInfo == null)
                return new ElectronicSignResponse(404, "传入的参数：orgInfo 为空，无法创建电子签章机构账号");

            var postData = string.Format("{{'accountId':'{0}','name':'{1}','email':'{2}','mobile':'{3}','idNumber':'{4}','idType':'CRED_PSN_CH_IDCARD'}}", personInfo.AccountId, personInfo.UserName, personInfo.Email, personInfo.Mobile, personInfo.IdentityNumber);

            var headers = await GetHeaders();
            ElectronicSignResponse result = null;
            await WebSendPutAsync<ElectronicSignResponse>(
                ServiceName + ".EditPersonAccount",
                esignApiDomain + "/v1/accounts/" + personInfo.AccountId,
                ApplicationConstant.AccScope,
                postData,
                headers,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ElectronicSignResponse((int)httpStatusCode, errorMessage);
                },
                false);

            return result;
        }

        /// <summary>
        /// 注销电子签章个人账号
        /// </summary>
        /// <param name="accountId">E签宝个人账户Id</param>
        /// <returns></returns>
        public async Task<ElectronicSignResponse> RemovePersonAccount(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                return new ElectronicSignResponse(404, "传入的参数：accountId 为空，无法注销个人账户");

            var headers = await GetHeaders();
            ElectronicSignResponse result = null;
            await WebSendDeleteAsync<ElectronicSignResponse>(
                ServiceName + ".RemovePersonAccount",
                esignApiDomain + "/v1/accounts/" + accountId,
                ApplicationConstant.AccScope,
                DefaultContentType,
                headers,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ElectronicSignResponse((int)httpStatusCode, errorMessage);
                },
                false);

            return result;
        }

        /// <summary>
        /// 创建个人电子签章
        /// </summary>
        /// <param name="accountId">E签宝个人账户Id</param>
        /// <returns></returns>
        public async Task<ElectronicSignResponse> CreatePersonSeal(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                return new ElectronicSignResponse(404, "传入的参数：accountId 为空，无法创建个人电子签章");

            var headers = await GetHeaders();
            var postData = string.Format("{{'alias':'红色四方形印章','type':'SQUARE','color':'RED','height':'95','width':'95'}}");

            ElectronicSignResponse result = null;
            await WebSendPostAsync<ElectronicSignResponse>(
                ServiceName + ".CreatePersonAccount",
                esignApiDomain + "/v1/accounts/" + accountId + "/seals/personaltemplate",
                ApplicationConstant.AccScope,
                postData,
                headers,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ElectronicSignResponse((int)httpStatusCode, errorMessage);
                },
                false);

            return result;
        }

        /// <summary>
        /// 注销个人电子签章
        /// </summary>
        /// <param name="accountId">E签宝个人账户Id</param>
        /// <param name="sealId">E签宝印章Id</param>
        /// <returns></returns>
        public async Task<ElectronicSignResponse> RemovePersonSeal(string accountId, string sealId)
        {
            if (string.IsNullOrEmpty(accountId))
                return new ElectronicSignResponse(404, "传入的参数：accountId 为空，无法注销个人电子签章");
            if (string.IsNullOrEmpty(sealId))
                return new ElectronicSignResponse(404, "传入的参数：sealId 为空，无法注销个人电子签章");

            var headers = await GetHeaders();
            ElectronicSignResponse result = null;
            await WebSendDeleteAsync<ElectronicSignResponse>(
                ServiceName + ".RemovePersonSeal",
                esignApiDomain + "/v1/accounts/" + accountId + "/seals/" + sealId,
                ApplicationConstant.AccScope,
                DefaultContentType,
                headers,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ElectronicSignResponse((int)httpStatusCode, errorMessage);
                },
                false);

            return result;
        }
        #endregion

        #region E签宝机构账户
        /// <summary>
        /// 创建电子签章机构账号
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="name">用户名</param>
        /// <param name="email">用户邮箱</param>
        /// <param name="mobile">用户手机号</param>
        /// <param name="idNumber">用户身份证</param>
        /// <returns></returns>
        public async Task<ElectronicSignResponse> CreateOrganizationAccount(ElectronicOrganizationDTO orgInfo)
        {
            if (orgInfo == null)
                return new ElectronicSignResponse(404, "传入的参数：orgInfo 为空，无法创建电子签章机构账号");
            if (orgInfo.ElectronicPerson == null || string.IsNullOrEmpty(orgInfo.ElectronicPerson.AccountId))
                return new ElectronicSignResponse(404, "未找到该企业相关的E签宝个人注册信息");

            var person = orgInfo.ElectronicPerson;
            var headers = await GetHeaders();
            var postData = string.Format("{{'thirdPartyUserId':'{0}','name':'{1}','idType':'{2}','idNumber':'{3}','creator':'{4}','orgLegalName':'{5}','orgLegalIdNumber':'{6}'}}", orgInfo.TenantName, orgInfo.Name, orgInfo.RegType == RegType.OrgCode ? "CRED_ORG_CODE" : "CRED_ORG_USCC", orgInfo.OrgNumber,  person.AccountId, orgInfo.OrgLegalName, orgInfo.OrgLegalIdNumber);

            ElectronicSignResponse result = null;
            await WebSendPostAsync<ElectronicSignResponse>(
                ServiceName + ".CreatePersonAccount",
                esignApiDomain + "/v1/organizations/createByThirdPartyUserId",
                ApplicationConstant.AccScope,
                postData,
                headers,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ElectronicSignResponse((int)httpStatusCode, errorMessage);
                },
                false);

            return result;
        }

        /// <summary>
        /// 修改电子签章机构账号
        /// </summary>
        /// <param name="orgInfo">E签宝机构信息</param>
        /// <returns></returns>
        public async Task<ElectronicSignResponse> EditOrganizationAccount(ElectronicOrganizationDTO orgInfo)
        {
            if (orgInfo == null)
                return new ElectronicSignResponse(404, "传入的参数：orgInfo 为空，无法创建电子签章机构账号");
            if (string.IsNullOrEmpty(orgInfo.OrgId))
                return new ElectronicSignResponse(404, "未找到该企业相关的E签宝机构注册信息");
            if (orgInfo.ElectronicPerson == null || string.IsNullOrEmpty(orgInfo.ElectronicPerson.AccountId))
                return new ElectronicSignResponse(404, "未找到该企业相关的E签宝个人注册信息");

            var person = orgInfo.ElectronicPerson;
            var headers = await GetHeaders();
            var postData = string.Format("{{'orgId':'{0}','name':'{1}','idType':'{2}','idNumber':'{3}','creator':'{4}','orgLegalName':'{5}','orgLegalIdNumber':'{6}'}}", orgInfo.OrgId, orgInfo.Name, orgInfo.RegType == RegType.OrgCode ? "CRED_ORG_CODE" : "CRED_ORG_USCC", orgInfo.OrgNumber, person.AccountId, orgInfo.OrgLegalName, orgInfo.OrgLegalIdNumber);

            ElectronicSignResponse result = null;
            await WebSendPutAsync<ElectronicSignResponse>(
                ServiceName + ".EditOrganizationAccount",
                esignApiDomain + "/v1/organizations/" + orgInfo.OrgId,
                ApplicationConstant.AccScope,
                postData,
                headers,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ElectronicSignResponse((int)httpStatusCode, errorMessage);
                },
                false);

            return result;
        }

        /// <summary>
        /// 注销电子签章机构账号
        /// </summary>
        /// <param name="orgId">E签宝机构账户Id</param>
        /// <returns></returns>
        public async Task<ElectronicSignResponse> RemoveOrganizationAccount(string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
                return new ElectronicSignResponse(404, "传入的参数：orgId 为空，无法注销个人账户");

            var headers = await GetHeaders();
            ElectronicSignResponse result = null;
            await WebSendDeleteAsync<ElectronicSignResponse>(
                ServiceName + ".RemoveOrganizationAccount",
                esignApiDomain + "/v1/organizations/" + orgId,
                ApplicationConstant.AccScope,
                DefaultContentType,
                headers,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ElectronicSignResponse((int)httpStatusCode, errorMessage);
                },
                false);

            return result;
        }

        /// <summary>
        /// 创建机构电子签章
        /// </summary>
        /// <param name="orgId">E签宝机账户Id</param>
        /// <param name="qtext">横向文：机构全称</param>
        /// <param name="orgId">下弦文：机构代码</param>
        /// <returns></returns>
        public async Task<ElectronicSignResponse> CreateOrganizationSeal(string orgId, string qtext, string htext)
        {
            if (string.IsNullOrEmpty(orgId))
                return new ElectronicSignResponse(404, "传入的参数：accountId 为空，无法创建个人电子签章");

            var headers = await GetHeaders();
            var postData = string.Format("{{'alias':'企业星型印章','type':'TEMPLATE_ROUND','color':'RED','central':'STAR','height':'159','width':'159', 'qtext':'{0}','htext':'{1}'}}", qtext, htext);

            ElectronicSignResponse result = null;
            await WebSendPostAsync<ElectronicSignResponse>(
                ServiceName + ".CreatePersonAccount",
                esignApiDomain + "/v1/accounts/" + orgId + "/seals/personaltemplate",
                ApplicationConstant.AccScope,
                postData,
                headers,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ElectronicSignResponse((int)httpStatusCode, errorMessage);
                },
                false);

            return result;
        }

        /// <summary>
        /// 注销机构电子签章
        /// </summary>
        /// <param name="orgId">E签宝机构账户Id</param>
        /// <param name="sealId">E签宝印章Id</param>
        /// <returns></returns>
        public async Task<ElectronicSignResponse> RemoveOrganizationSeal(string orgId, string sealId)
        {
            if (string.IsNullOrEmpty(orgId))
                return new ElectronicSignResponse(404, "传入的参数：accountId 为空，无法注销个人电子签章");
            if (string.IsNullOrEmpty(sealId))
                return new ElectronicSignResponse(404, "传入的参数：sealId 为空，无法注销个人电子签章");

            var headers = await GetHeaders();
            ElectronicSignResponse result = null;
            await WebSendDeleteAsync<ElectronicSignResponse>(
                ServiceName + ".RemovePersonSeal",
                esignApiDomain + "/v1/organizations/" + orgId + "/seals/" + sealId,
                ApplicationConstant.AccScope,
                DefaultContentType,
                headers,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ElectronicSignResponse((int)httpStatusCode, errorMessage);
                },
                false);

            return result;
        }
        #endregion

        public ServiceResult<int> ConfirmCurrencySign(ContractGroupDTO model, bool isStatu = false)
        {
            var contractGroup = _mapper.Map<ContractGroup>(model);
            var postData = SerializeHelper.ToJson(contractGroup);
            ServiceResult<int> result = null;
            WebSendGet<ServiceResult<int>>(
                ServiceName + ".Get",
                esignApiDomain + "Get",
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<int>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            return result;
        }

        public ServiceResult<bool> FindContractByNo(string contractNo, string id)
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".ConfirmCurrencySign",
                esignApiDomain + "CurrencySign_New/FindContractByNo?contractNo=" + contractNo + "&id=" + id,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, errorMessage);
                },
                true);
            return result;
        }

        public ServiceResult<bool> RemoveCurrencySignService(string id)
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".RemoveCurrencySignService",
                esignApiDomain + "CurrencySign_New/RemoveCurrencySignService?referenceId=" + id,
                ApplicationConstant.AccScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, errorMessage);
                },
                true);
            return result;
        }

        public ServiceResult<Tuple<int, string, byte[]>> AddSignatureToPdf(byte[] buffer0, string FileName, string posX, string posY, string key = "", int signType = 3, string code = "", string posPage = "0", string xCoordinate = "0", string tenantName = "", bool personal = false)
        {
            var postData = SerializeHelper.ToJson(buffer0);
            ServiceResult<Tuple<int, string, byte[]>> result = null;
            WebSendPost<ServiceResult<Tuple<int, string, byte[]>>>(
                ServiceName + ".AddSignatureToPdf",
                esignApiDomain + "CurrencySign_New/AddSignatureToPdf?FileName=" + FileName + "&tenantName=" + tenantName + "&posX=" + posX + "&posY=" + posY + "&key=" + key + "&signType=" + signType + "&code=" + code + "&posPage=" + posPage + "&xCoordinate=" + xCoordinate + "&personal=" + personal,
                ApplicationConstant.AccScope,
                postData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<Tuple<int, string, byte[]>>(ServiceResultType.Error, errorMessage);
                },
                true);
            return result;
        }

    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class ElectronicSignResponse
    {
        public ElectronicSignResponse() { }
        public ElectronicSignResponse(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
        [DataMember]
        public int code { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public ElectronicSignData data { get; set; }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class ElectronicSignData
    {
        [DataMember]
        public string accountId { get; set; }

        [DataMember]
        public string orgId { get; set; }

        #region 印章数据
        [DataMember]
        public string sealId { get; set; }
        [DataMember]
        public string fileKey { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public string width { get; set; }
        [DataMember]
        public string height { get; set; }
        #endregion

        #region token数据
        [DataMember]
        public string token { get; set; }
        [DataMember]
        public string refreshToken { get; set; }
        [DataMember]
        public long expiresIn { get; set; }
        #endregion
    }
}
