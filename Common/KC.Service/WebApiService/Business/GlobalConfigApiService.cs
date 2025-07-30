using AutoMapper;
using KC.Service.DTO.Config;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Base;
using KC.Service.Constants;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace KC.Service.WebApiService.Business
{
    public interface IGlobalConfigApiService
    {
        /// <summary>
        /// 获取全局配置
        /// </summary>
        /// <returns></returns>
        GlobalConfigData GetGlobalConfigData();

        /// <summary>
        /// 获取默认的邮件配置
        /// </summary>
        /// <returns></returns>
        EmailConfig GetDefaultEmailConfig();

        /// <summary>
        /// 获取默认的短信中心配置
        /// </summary>
        /// <returns></returns>
        SmsConfig GetDefaultSmsConfig();

        /// <summary>
        /// 获取默认的呼叫中心配置
        /// </summary>
        /// <returns></returns>
        CallConfig GetDefaultCallConfig();
    }

    public class GlobalConfigApiService : IdSrvOAuth2ClientRequestBase, IGlobalConfigApiService
    {
        private const string _serviceName = "KC.Service.WebApiService.Business.GlobalConfigApiService";

        public GlobalConfigApiService(IHttpClientFactory clientFactory)
            : base(TenantConstant.DbaTenantApiAccessInfo, clientFactory, null)
        {
        }

        /// <summary>
        /// 获取全局配置
        /// </summary>
        /// <returns></returns>
        public GlobalConfigData GetGlobalConfigData()
        {
            GlobalConfigData result = null;
            WebSendGet<GlobalConfigData>(
                _serviceName + ".GetGlobalConfigData",
                GlobalConfig.ResWebDomain + "api/GlobalConfigApi/GetData",
                ApplicationConstant.SsoScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    Logger?.LogError(errorMessage);
                },
                false);

            return result;
        }
        
        /// <summary>
        /// 获取默认的邮件配置
        /// </summary>
        /// <returns></returns>
        public EmailConfig GetDefaultEmailConfig()
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "DefaultEmailConfig";
            var emailConfig = CacheUtil.GetCache<EmailConfig>(cacheKey);
            if (emailConfig == null)
            {
                //var dbaTenant = TenantUtil.GetDbaTenantUser();
                var dbaTenant = TenantConstant.DbaTenantApiAccessInfo;
                var dbconfigs = GetConfigByType(ConfigType.EmailConfig);
                if (dbconfigs != null)
                {
                    var config = KC.Service.AutoMapper.AutoMapperConfiguration.Configure();
                    var Mapper = config.CreateMapper();
                    emailConfig = Mapper.Map<EmailConfig>(dbconfigs);
                    if (emailConfig != null)
                    {
                        emailConfig.TenantName = dbaTenant.TenantName;
                        CacheUtil.SetCache(cacheKey, emailConfig, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                    }
                }
            }

            return emailConfig;
        }

        /// <summary>
        /// 获取默认的短信中心配置
        /// </summary>
        /// <returns></returns>
        public SmsConfig GetDefaultSmsConfig()
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "DefaultSmsConfig";
            var smsConfig = CacheUtil.GetCache<SmsConfig>(cacheKey);
            if (smsConfig == null)
            {
                //var dbaTenant = TenantUtil.GetDbaTenantUser();
                var dbaTenant = TenantConstant.DbaTenantApiAccessInfo;
                var dbConfigs = GetConfigByType(Framework.Base.ConfigType.SmsConfig);
                if (dbConfigs != null)
                {
                    var config = KC.Service.AutoMapper.AutoMapperConfiguration.Configure();
                    var Mapper = config.CreateMapper();
                    smsConfig = Mapper.Map<SmsConfig>(dbConfigs);
                    if (smsConfig != null)
                    {
                        smsConfig.TenantName = dbaTenant.TenantName;
                        CacheUtil.SetCache(cacheKey, smsConfig, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                    }
                }
            }

            return smsConfig;
        }

        /// <summary>
        /// 获取默认的呼叫中心配置
        /// </summary>
        /// <returns></returns>
        public CallConfig GetDefaultCallConfig()
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "DefaultCallConfig";
            var callConfig = CacheUtil.GetCache<CallConfig>(cacheKey);
            if (callConfig == null)
            {
                //var dbaTenant = TenantUtil.GetDbaTenantUser();
                var dbaTenant = TenantConstant.DbaTenantApiAccessInfo;
                var dbConfigs = GetConfigByType(Framework.Base.ConfigType.CallConfig);
                if (dbConfigs != null)
                {
                    var config = KC.Service.AutoMapper.AutoMapperConfiguration.Configure();
                    var Mapper = config.CreateMapper();
                    callConfig = Mapper.Map<CallConfig>(dbConfigs);
                    if (callConfig != null)
                    {
                        callConfig.TenantName = dbaTenant.TenantName;
                        CacheUtil.SetCache(cacheKey, callConfig, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                    }
                }
            }

            return callConfig;
        }

        /// <summary>
        /// 根据配置文件类型获取配置文件
        /// </summary>
        /// <param name="type">配置文件类型</param>
        /// <returns></returns>
        private ConfigEntityDTO GetConfigByType(ConfigType type)
        {
            ServiceResult<ConfigEntityDTO> result = null;
            WebSendGet<ServiceResult<ConfigEntityDTO>>(
                _serviceName + ".GetConfigByType",
                SsoApiServerUrl + "ConfigApi/GetConfigByType?type=" + (int)type,
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
    }
}
