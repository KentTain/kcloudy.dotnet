using System.Collections.Generic;
using KC.Service.DTO.Config;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Base;
using System;
using KC.Service.Constants;
using AutoMapper;
using Microsoft.Extensions.Logging;
using KC.Framework.Extension;

namespace KC.Service.WebApiService.Business
{
    public interface IConfigApiService
    {
        /// <summary>
        /// 获取系统编码
        /// </summary>
        /// <param name="name">配置名称</param>
        /// <returns></returns>
        string GetSeedCodeByName(string name);
        /// <summary>
        /// 获取系统编码对象
        /// </summary>
        /// <param name="name">配置名称</param>
        /// <param name="step">步长</param>
        /// <returns></returns>
        SeedEntity GetSeedEntityByName(string name, int step = 1);

        /// <summary>
        /// 根据配置文件类型获取配置文件
        /// </summary>
        /// <param name="type">配置文件类型</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        List<ConfigEntityDTO> GetAllConfigsByType(ConfigType type);

        /// <summary>
        /// 根据配置文件Id获取配置文件
        /// </summary>
        /// <param name="configId">配置文件Id</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        ConfigEntityDTO GetConfigById(int configId);

        /// <summary>
        /// 根据配置文件类型获取配置文件
        /// </summary>
        /// <param name="type">配置文件类型</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        ConfigEntityDTO GetConfigByType(ConfigType type);

        /// <summary>
        /// 根据配置文件类型获取配置文件
        /// </summary>
        /// <param name="type">配置文件类型</param>
        /// <param name="sign">支付类型</param>
        /// <returns></returns>
        [Extension.CachingCallHandler()]
        ConfigEntityDTO GetConfigsByTypeAndSign(ConfigType type, int sign);

        /// <summary>
        /// 获取租户的邮件配置
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        EmailConfig GetTenantEmailConfig(Tenant tenant);
        void RemoveTenantEmailConfigCache(Tenant tenant);
        void RemoveDefaultEmailConfigCache();

        /// <summary>
        /// 获取短信配置
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        SmsConfig GetTenantSmsConfig(Tenant tenant);
        void RemoveTenantSmsConfigCache(Tenant tenant);
        void RemoveDefaultSmsConfigCache();

        /// <summary>
        /// 获取租户的呼叫中心配置
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        CallConfig GetTenantCallUncallConfig(Tenant tenant);
        void RemoveTenantCallConfigCache(Tenant tenant);
        void RemoveDefaultCallConfigCache();
    }

    public class ConfigApiService : IdSrvOAuth2ClientRequestBase, IConfigApiService
    {
        private const string _serviceName = "KC.Service.WebApiService.Business.ConfigApiService";
        private readonly IMapper Mapper;
        private IGlobalConfigApiService _globalConfigApiService;
        public ConfigApiService(
            Tenant tenant,
            IGlobalConfigApiService globalConfigApiService,
            IMapper mapper,
            System.Net.Http.IHttpClientFactory httpClient,
            ILogger<AccountApiService> logger)
            : base(tenant ?? TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {
            Mapper = mapper;
            _globalConfigApiService = globalConfigApiService;
        }

        /// <summary>
        /// 获取配置信息接口地址：http://[tenantName].cfg.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:1101/api/
        /// </summary>
        private string _configServiceUrl
        {
            get
            {
                if (string.IsNullOrEmpty(GlobalConfig.CfgWebDomain))
                    return null;

                return GlobalConfig.CfgWebDomain.Replace(TenantConstant.SubDomain, Tenant.TenantName) + "api/";
            }
        }

        #region 系统编码
        /// <summary>
        /// 获取系统编码
        /// </summary>
        /// <param name="name">配置名称</param>
        /// <param name="step">步长</param>
        /// <returns></returns>
        public string GetSeedCodeByName(string name)
        {
            ServiceResult<string> result = null;
            WebSendGet<ServiceResult<string>>(
                _serviceName + ".GetSeedCodeByName",
                _configServiceUrl + "ConfigApi/GetSeedCodeByName?name=" + name,
                ApplicationConstant.CfgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<string>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return StringExtensions.GenerateStringID();
        }

        /// <summary>
        /// 获取系统编码
        /// </summary>
        /// <param name="name">配置名称</param>
        /// <param name="step">步长</param>
        /// <returns></returns>
        public SeedEntity GetSeedEntityByName(string name, int step = 1)
        {
            ServiceResult<SeedEntity> result = null;
            WebSendGet<ServiceResult<SeedEntity>>(
                _serviceName + ".GetSeedEntityByName",
                _configServiceUrl + "ConfigApi/GetSeedEntityByName?name=" + name + "&step=" + step,
                ApplicationConstant.CfgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<SeedEntity>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }
        #endregion

        #region 获取配置数据
        /// <summary>
        /// 根据配置文件类型获取配置文件
        /// </summary>
        /// <param name="type">配置文件类型</param>
        /// <returns></returns>
        public List<ConfigEntityDTO> GetAllConfigsByType(ConfigType type)
        {
            ServiceResult<List<ConfigEntityDTO>> result = null;
            WebSendGet<ServiceResult<List<ConfigEntityDTO>>>(
                _serviceName + ".GetAllConfigsByType",
                _configServiceUrl + "ConfigApi/GetAllConfigsByType?type=" + type,
                ApplicationConstant.CfgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<ConfigEntityDTO>>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        /// <summary>
        /// 根据配置文件Id获取配置文件
        /// </summary>
        /// <param name="configId">配置文件Id</param>
        /// <returns></returns>
        public ConfigEntityDTO GetConfigById(int configId)
        {
            ServiceResult<ConfigEntityDTO> result = null;
            WebSendGet<ServiceResult<ConfigEntityDTO>>(
                _serviceName + ".GetConfigById",
                _configServiceUrl + "ConfigApi/GetConfigById?configId=" + configId,
                ApplicationConstant.CfgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<ConfigEntityDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }
        /// <summary>
        /// 根据配置文件类型获取配置文件
        /// </summary>
        /// <param name="type">配置文件类型</param>
        /// <returns></returns>
        public ConfigEntityDTO GetConfigByType(ConfigType type)
        {
            ServiceResult<ConfigEntityDTO> result = null;
            WebSendGet<ServiceResult<ConfigEntityDTO>>(
                _serviceName + ".GetConfigByType",
                _configServiceUrl + "ConfigApi/GetConfigByType?type=" + (int)type,
                ApplicationConstant.CfgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<ConfigEntityDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }

        public ConfigEntityDTO GetConfigsByTypeAndSign(ConfigType type, int sign)
        {
            ServiceResult<ConfigEntityDTO> result = null;
            WebSendGet<ServiceResult<ConfigEntityDTO>>(
                _serviceName + ".GetConfigsByTypeAndName",
                _configServiceUrl + "ConfigApi/GetConfigsByTypeAndSign?type=" + (int)type + "&sign=" + sign,
                ApplicationConstant.CfgScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<ConfigEntityDTO>(ServiceResultType.Error, httpStatusCode, errorMessage);
                },
                true);

            if (result.success && result.Result != null)
            {
                return result.Result;
            }

            return null;
        }
        #endregion

        #region 邮件配置

        public EmailConfig GetTenantEmailConfig(Tenant tenant)
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-EmailConfig";
            var emailConfig = CacheUtil.GetCache<EmailConfig>(cacheKey);
            if (emailConfig != null) return emailConfig;

            try
            {
                var dbconfigs = GetConfigByType(ConfigType.EmailConfig);
                if (dbconfigs != null)
                {
                    emailConfig = Mapper.Map<EmailConfig>(dbconfigs);
                    if (emailConfig != null)
                    {
                        emailConfig.TenantName = tenant.TenantName;
                        CacheUtil.SetCache(cacheKey, emailConfig, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                        return emailConfig;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("获取租户【{0}-{1}】邮件配置出错，错误消息：{2}，，已使用系统dba的默认配置", tenant.TenantName, tenant.TenantDisplayName, ex.Message));
            }

            return _globalConfigApiService.GetDefaultEmailConfig();
        }
        public void RemoveTenantEmailConfigCache(Tenant tenant)
        {
            var allCacheKeys = GetAllTenantEmailCacheKeys(tenant);
            allCacheKeys.ForEach(CacheUtil.RemoveCache);
        }
        public void RemoveDefaultEmailConfigCache()
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "DefaultEmailConfig";
            CacheUtil.RemoveCache(cacheKey);
        }

        private List<string> GetAllTenantEmailCacheKeys(Tenant tenant)
        {
            return new List<string>()
            {
                CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-EmailConfig",
        };
        }
        #endregion

        #region 短信配置
        
        /// <summary>
        /// 获取短信配置
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        public SmsConfig GetTenantSmsConfig(Tenant tenant)
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-SmsConfig";
            var smsConfig = CacheUtil.GetCache<SmsConfig>(cacheKey);
            if (smsConfig != null) return smsConfig;

            try
            {
                var dbConfigs = GetConfigByType(ConfigType.SmsConfig);
                if (dbConfigs != null)
                {
                    smsConfig = Mapper.Map<SmsConfig>(dbConfigs);
                    smsConfig.TenantName = tenant.TenantName;
                    CacheUtil.SetCache(cacheKey, smsConfig, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                    return smsConfig;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("获取租户【{0}-{1}】短信配置出错，错误消息：{2}，已使用系统dba的默认配置", tenant.TenantName, tenant.TenantDisplayName, ex.Message));
            }

            return _globalConfigApiService.GetDefaultSmsConfig();
        }

        public void RemoveTenantSmsConfigCache(Tenant tenant)
        {
            var allCacheKeys = GetAllTenantSmsCacheKeys(tenant);
            allCacheKeys.ForEach(CacheUtil.RemoveCache);
        }
        public void RemoveDefaultSmsConfigCache()
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "DefaultSmsConfig";
            CacheUtil.RemoveCache(cacheKey);
        }

        private static List<string> GetAllTenantSmsCacheKeys(Tenant tenant)
        {
            return new List<string>()
            {
                CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-SmsConfig",
            };
        }

        #endregion

        #region 呼叫中心配置

        public const int HuaweiConfigId = 10;//华为短信配置
        public const int SHJConfigId = 11;//深海捷短信配置
        public const int UncallConfigId = 12;//长鑫盛通短信配置
        public CallConfig GetTenantCallHuaweiConfig(Tenant tenant)
        {
            return GetTenantCallConfig(tenant, HuaweiConfigId);
        }
        public CallConfig GetTenantCallSHJConfig(Tenant tenant)
        {
            return GetTenantCallConfig(tenant, SHJConfigId);
        }
        public CallConfig GetTenantCallUncallConfig(Tenant tenant)
        {
            return GetTenantCallConfig(tenant, UncallConfigId);
        }
        public CallConfig GetTenantCallGeneralConfig(Tenant tenant)
        {
            return GetDefaultTenantCallConfig(tenant);
        }

        public void RemoveTenantCallConfigCache(Tenant tenant)
        {
            var allCacheKeys = GetAllTenantCallCacheKeys(tenant);
            allCacheKeys.ForEach(CacheUtil.RemoveCache);
        }
        public void RemoveDefaultCallConfigCache()
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "DefaultCallConfig";
            CacheUtil.RemoveCache(cacheKey);
        }

        private List<string> GetAllTenantCallCacheKeys(Tenant tenant)
        {
            return new List<string>()
            {
                CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-CallCofnig-" + HuaweiConfigId,
                CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-CallCofnig-" + SHJConfigId,
                CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-CallCofnig-" + UncallConfigId
            };
        }

        private CallConfig GetTenantCallConfig(Tenant tenant, int configId)
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-CallCofnig-" + configId;
            var callConfig = CacheUtil.GetCache<CallConfig>(cacheKey);
            if (callConfig != null) return callConfig;

            try
            {
                var dbConfigs = GetConfigById(configId);
                if (dbConfigs != null)
                {
                    callConfig = Mapper.Map<CallConfig>(dbConfigs);
                    callConfig.TenantName = tenant.TenantName;
                    CacheUtil.SetCache(cacheKey, callConfig, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                    return callConfig;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("获取租户【{0}-{1}】呼叫中心配置出错，错误消息：{2}，已使用系统dba的默认配置", tenant.TenantName, tenant.TenantDisplayName, ex.Message));
            }

            return GetDefaultCallConfigById(configId);
        }
        private CallConfig GetDefaultTenantCallConfig(Tenant tenant)
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-DefaultCallConfig-" + tenant.TenantName;
            var callConfig = CacheUtil.GetCache<CallConfig>(cacheKey);
            if (callConfig != null) return callConfig;

            try
            {
                var dbConfigs = GetConfigByType(Framework.Base.ConfigType.CallConfig);
                if (dbConfigs != null)
                {
                    callConfig = Mapper.Map<CallConfig>(dbConfigs);
                    if (callConfig != null)
                    {
                        callConfig.TenantName = tenant.TenantName;
                        CacheUtil.SetCache(cacheKey, callConfig, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                        return callConfig;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("获取租户【{0}-{1}】邮件配置出错，已使用系统dba的默认配置", tenant.TenantName, tenant.TenantDisplayName), ex);
            }
            return _globalConfigApiService.GetDefaultCallConfig();
        }
        private CallConfig GetDefaultCallConfigById(int configId)
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "DefaultCallConfig-" + configId;
            var callConfig = CacheUtil.GetCache<CallConfig>(cacheKey);
            if (callConfig == null)
            {
                //var dbaTenant = TenantUtil.GetDbaTenantUser();
                var dbaTenant = TenantConstant.DbaTenantApiAccessInfo;
                var dbConfigs = GetConfigById(configId);
                if (dbConfigs != null)
                {
                    callConfig = Mapper.Map<CallConfig>(dbConfigs);
                    callConfig.TenantName = dbaTenant.TenantName;
                    CacheUtil.SetCache(cacheKey, callConfig, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                }
                else
                {
                    return _globalConfigApiService.GetDefaultCallConfig();
                }
            }

            return callConfig;
        }
        
        #endregion
    }

}
