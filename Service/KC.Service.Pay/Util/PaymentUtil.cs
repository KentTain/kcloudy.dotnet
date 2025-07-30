using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO.Config;
using KC.Enums.Pay;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Service.Constants;
using KC.Service.WebApiService.Business;
using KC.Service.AutoMapper;

namespace KC.Service.Util
{
    public  static class PaymentUtil
    {
        
        public static ConfigEntityDTO GetTenantConfigEntityDTO(Tenant tenant, int configId)
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-PaymentCofnig-" + configId;
            var ConfigEntityDTO = CacheUtil.GetCache<ConfigEntityDTO>(cacheKey);
            if (ConfigEntityDTO != null) return ConfigEntityDTO;

            try
            {
                var config = AutoMapperConfiguration.Configure();
                var mapper = config.CreateMapper();
                ConfigEntityDTO = new ConfigApiService(tenant, null, mapper, null, null).GetConfigById(configId);
                if (ConfigEntityDTO != null)
                {
                    CacheUtil.SetCache(cacheKey, ConfigEntityDTO, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                    return ConfigEntityDTO;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }

            return GetDefaultConfigEntityDTOById(configId);
        }

        /// <summary>
        /// 根据id获取配置文件
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="configId">ID</param>
        /// <returns></returns>
        public static ConfigEntityDTO GetConfigsById(Tenant tenant, int configId)
        {
            var config = AutoMapperConfiguration.Configure();
            var mapper = config.CreateMapper();
            return new ConfigApiService(tenant, null, mapper, null, null).GetConfigById(configId);
        }


        /// <summary>
        /// 根据Type和Name获取配置文件
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ConfigEntityDTO GetConfigsByTypeAndSign(Tenant tenant, int sign)
        {
            var config = AutoMapperConfiguration.Configure();
            var mapper = config.CreateMapper();
            var configEntityDTO = new ConfigApiService(tenant, null, mapper, null, null).GetConfigsByTypeAndSign(Framework.Base.ConfigType.PaymentMethod, sign);
            return configEntityDTO;
        }

        private static ConfigEntityDTO GetDefaultTenantConfigEntityDTO(Tenant tenant)
        {
            var config = AutoMapperConfiguration.Configure();
            var mapper = config.CreateMapper();
            return new ConfigApiService(tenant, null, mapper, null, null).GetConfigByType(Framework.Base.ConfigType.PaymentMethod);

        }

        private static ConfigEntityDTO GetDefaultConfigEntityDTOById(int configId)
        {
            var config = AutoMapperConfiguration.Configure();
            var mapper = config.CreateMapper();
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "DefaultConfigEntityDTO-" + configId;
            var ConfigEntityDTO = CacheUtil.GetCache<ConfigEntityDTO>(cacheKey);
            if (ConfigEntityDTO == null)
            {
                //var dbaTenant = TenantUtil.GetDbaTenantUser();
                var dbaTenant = TenantConstant.DbaTenantApiAccessInfo;
                ConfigEntityDTO = new ConfigApiService(dbaTenant, null, mapper, null, null).GetConfigById(configId);
                if (ConfigEntityDTO != null)
                {
                    CacheUtil.SetCache(cacheKey, ConfigEntityDTO, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                }
                else
                {
                    return GetDefaultConfigEntityDTO();
                }
            }

            return ConfigEntityDTO;
        }
        public static ConfigEntityDTO GetDefaultConfigEntityDTO()
        {
            var config = AutoMapperConfiguration.Configure();
            var mapper = config.CreateMapper();
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "DefaultConfigEntityDTO";
            var ConfigEntityDTO = CacheUtil.GetCache<ConfigEntityDTO>(cacheKey);
            if (ConfigEntityDTO == null)
            {
                //var dbaTenant = TenantUtil.GetDbaTenantUser();
                var dbaTenant = TenantConstant.DbaTenantApiAccessInfo;
                ConfigEntityDTO = new ConfigApiService(dbaTenant, null, mapper, null, null).GetConfigByType(Framework.Base.ConfigType.PaymentMethod);
                if (ConfigEntityDTO != null)
                {
                        CacheUtil.SetCache(cacheKey, ConfigEntityDTO, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                }
            }

            return ConfigEntityDTO;
        }

        public static void RemoveTenantConfigEntityDTOCache(Tenant tenant)
        {
            var allCacheKeys = GetAllTenantPaymentCacheKeys(tenant);
            allCacheKeys.ForEach(CacheUtil.RemoveCache);
        }
        public static void RemoveDefaultConfigEntityDTOCache()
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "DefaultConfigEntityDTO";
            CacheUtil.RemoveCache(cacheKey);
        }

        private static List<string> GetAllTenantPaymentCacheKeys(Tenant tenant)
        {
            return new List<string>()
            {
                CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-PaymentCofnig-" + (int)ThirdPartyType.UnionpayConfigSign,
                CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-PaymentCofnig-" + (int)ThirdPartyType.FuiouConfigSign,
                CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-PaymentCofnig-" + (int)ThirdPartyType.CMBCBSConfigSign,
                CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-PaymentCofnig-" + (int)ThirdPartyType.AllinpayConfigSign,
                CacheKeyConstants.Prefix.ConfigName + "-" + tenant.TenantName + "-PaymentCofnig-" + (int)ThirdPartyType.HeliPayConfigSign
            };
        }
    }
}
