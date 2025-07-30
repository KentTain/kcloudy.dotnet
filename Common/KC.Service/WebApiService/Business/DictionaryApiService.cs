using System;
using System.Collections.Generic;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Constants;
using KC.Service.DTO.Dict;

namespace KC.Service.WebApiService.Business
{
    public interface IDictionaryApiService
    {
        /// <summary>
        /// 获取省份信息
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        List<ProvinceDTO> LoadAllProvinces();

        /// <summary>
        /// 获取城市信息
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        List<CityDTO> LoadAllCities();
        /// <summary>
        /// 根据省份Id，获取城市信息
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        List<CityDTO> LoadCitiesByProvinceId(int provinceID);

        /// <summary>
        /// 获取国民经济行业分类信息
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        List<IndustryClassficationDTO> LoadRootIndustryClassfications();

        /// <summary>
        /// 获取手机归属地信息
        /// </summary>
        /// <param name="mobilePhone">手机号码</param>
        /// <returns></returns>
        MobileLocationDTO GetMobileLocation(string mobilePhone);

        /// <summary>
        /// 根据字典类型编码，获取其下的所有字典列表数据
        /// </summary>
        /// <param name="code">字典类型编码，例如：DCT2020010100001</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        List<DictValueDTO> LoadAllDictValuesByTypeCode(string code);
    }

    public class DictionaryApiService : IdSrvOAuth2ClientRequestBase, IDictionaryApiService
    {
        private const string _serviceName = "KC.Service.WebApiService.Business.DictionaryApiService";

        public DictionaryApiService(
            Tenant tenant,
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<AccountApiService> logger)
            : base(tenant ?? TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {
        }

        /// <summary>
        /// 租户字典接口地址：http://[tenantName].dic.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:1103/api/
        /// </summary>
        private string _dictionaryServiceUrl
        {
            get
            {
                if (string.IsNullOrEmpty(GlobalConfig.DicWebDomain))
                    return null;

                return GlobalConfig.DicWebDomain.Replace(TenantConstant.SubDomain, Tenant.TenantName) + "api/";
            }
        }

        /// <summary>
        /// 获取省份信息
        /// </summary>
        /// <returns></returns>
        public List<ProvinceDTO> LoadAllProvinces()
        {
            //var cacheKey = "WebApiService.DictionaryApiService-LoadAllProvinces";
            //var cache = Service.CacheUtil.GetCache<List<ProvinceDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<ProvinceDTO>> result = null;
            WebSendGet<ServiceResult<List<ProvinceDTO>>>(
                _serviceName + ".LoadAllProvinces",
                _dictionaryServiceUrl + "DictionaryApi/LoadAllProvinces",
                ApplicationConstant.DicScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<ProvinceDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取城市信息
        /// </summary>
        /// <returns></returns>
        public List<CityDTO> LoadAllCities()
        {
            //var cacheKey = "WebApiService.DictionaryApiService-LoadAllCities";
            //var cache = Service.CacheUtil.GetCache<List<CityDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<CityDTO>> result = null;
            WebSendGet<ServiceResult<List<CityDTO>>>(
                _serviceName + ".LoadAllCities",
                _dictionaryServiceUrl + "DictionaryApi/LoadAllCities",
                ApplicationConstant.DicScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<CityDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取手机归属地信息
        /// </summary>
        /// <param name="mobilePhone">手机号码</param>
        /// <returns></returns>
        public List<CityDTO> LoadCitiesByProvinceId(int provinceID)
        {
            //var cacheKey = "WebApiService.DictionaryApiService-LoadCitiesByProvinceId-" + provinceID;
            //var cache = Service.CacheUtil.GetCache<List<CityDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<CityDTO>> result = null;
            WebSendGet<ServiceResult<List<CityDTO>>>(
                _serviceName + ".LoadCitiesByProvinceId",
                _dictionaryServiceUrl + "DictionaryApi/LoadCitiesByProvinceId?provinceID=" + provinceID,
                ApplicationConstant.DicScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<CityDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取国民经济行业分类信息
        /// </summary>
        /// <returns></returns>
        public List<IndustryClassficationDTO> LoadRootIndustryClassfications()
        {
            //var cacheKey = "WebApiService.DictionaryApiService-LoadRootIndustryClassfications";
            //var cache = Service.CacheUtil.GetCache<List<IndustryClassficationDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<IndustryClassficationDTO>> result = null;
            WebSendGet<ServiceResult<List<IndustryClassficationDTO>>>(
                _serviceName + ".LoadRootIndustryClassfications",
                _dictionaryServiceUrl + "DictionaryApi/LoadRootIndustryClassfications",
                ApplicationConstant.DicScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<IndustryClassficationDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 获取手机归属地信息
        /// </summary>
        /// <param name="mobilePhone">手机号码</param>
        /// <returns></returns>
        public MobileLocationDTO GetMobileLocation(string mobilePhone)
        {
            ServiceResult<MobileLocationDTO> result = null;
            WebSendGet<ServiceResult<MobileLocationDTO>>(
                _serviceName + ".GetMobileLocation",
                _dictionaryServiceUrl + "DictionaryApi/GetMobileLocation?mobilePhone=" + mobilePhone,
                ApplicationConstant.DicScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<MobileLocationDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 根据字典类型编码，获取其下的所有字典列表数据
        /// </summary>
        /// <param name="code">字典类型编码，例如：DCT2020010100001</param>
        /// <returns></returns>
        public List<DictValueDTO> LoadAllDictValuesByTypeCode(string code)
        {
            //var cacheKey = "WebApiService.DictionaryApiService-LoadAllDictValuesByTypeCode-" + code;
            //var cache = Service.CacheUtil.GetCache<List<DictValueDTO>>(cacheKey);
            //if (cache != null)
            //    return cache;

            ServiceResult<List<DictValueDTO>> result = null;
            WebSendGet<ServiceResult<List<DictValueDTO>>>(
                _serviceName + ".LoadAllDictValuesByTypeCode",
                _dictionaryServiceUrl + "DictionaryApi/LoadAllDictValuesByTypeCode?code=" + code,
                ApplicationConstant.DicScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<DictValueDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                //Service.CacheUtil.SetCache(cacheKey, result.Result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));

                return result.Result;
            }

            return null;
        }
    }
}
