using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using KC.Framework.Base;
using KC.Service.Base;
using KC.Service.Constants;

namespace KC.Service.AutoMapper.Converter
{
    public class CallConfigConverter : ITypeConverter<ConfigEntity, CallConfig>
    {
        public CallConfig Convert(ConfigEntity source, CallConfig destination, ResolutionContext context)
        {
            var config = source;
            if (config == null)
            {
                throw new ArgumentException("The sms config can't be null.");
            }

            if (config.ConfigType != ConfigType.CallConfig)
            {
                throw new ArgumentException("The config isn't a call configuration.");
            }

            //return new CallConfig(config.ConfigXml);

            return ConvertCallConfig(config);
        }

        protected CallConfig ConvertCallConfig(ConfigEntity configEntity)
        {
            var config = new CallConfig()
            {
                ConfigId = configEntity.ConfigId,
                ConfigName = configEntity.ConfigName,
                ConfigType = configEntity.ConfigType,
                ConfigDescription = configEntity.ConfigDescription
            };

            var attributes = configEntity.ConfigAttributes.ToList();

            var properties = typeof(CallConfig).GetProperties();
            attributes.ForEach(k =>
            {
                var property = properties.FirstOrDefault(m => m.Name.Equals(k.Name, StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    property.SetValue(config, System.Convert.ChangeType(k.Value, property.PropertyType));
                }
            });

            return config;
        }

    }
    public class ConfigEntityToCallConfigConverter : ITypeConverter<CallConfig, ConfigEntity>
    {
        public ConfigEntity Convert(CallConfig source, ConfigEntity destination, ResolutionContext context)
        {
            var cfgObjet = source;
            if (cfgObjet == null)
            {
                throw new ArgumentException("The sms config is not null.");
            }

            if (cfgObjet.ConfigType != ConfigType.CallConfig)
            {
                throw new ArgumentException("The config is not sms configuration.");
            }

            var result = new ConfigEntity()
            {
                ConfigType = ConfigType.CallConfig,
                ConfigId = cfgObjet.ConfigId,
                ConfigName = cfgObjet.ConfigName,
                ConfigDescription = cfgObjet.ConfigDescription,
                ConfigXml = cfgObjet.GetConfigObjectXml()
            };

            #region Attributes
            var attributes = new List<ConfigAttribute>
            {
                new ConfigAttribute()
                {
                    Name = SmsConstants.ProviderName,
                    Value =
                        !string.IsNullOrWhiteSpace(cfgObjet.ProviderName)
                            ? cfgObjet.ProviderName
                            : "平台名称，如：UNCALL（长鑫盛通）",
                    DisplayName = "平台名称",
                    Description = "需要结合程序中实现的Class名称使用。",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = SmsConstants.SmsUrl,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.SmsUrl) ? cfgObjet.SmsUrl : "发送地址，如：http://111.111.111.111:8888/sms.aspx",
                    DisplayName = "发送地址",
                    Description = "发送地址，此项必须，只需要修改值",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = "Value1",
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Value1) ? cfgObjet.Value1 : "语音文件下载地址，如：uncall_api/downloadFile.php?f_path=",
                    DisplayName = "语音文件下载地址，此项必须，只需要修改值",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = "Value2",
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Value2) ? cfgObjet.Value2 : "拨打手机前加拨的数字，如：9",
                    DisplayName = "拨打手机前加拨",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = "Value3",
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Value3) ? cfgObjet.Value3 : "开通呼叫中心服务所属省市，如：广东深圳",
                    DisplayName = "开通呼叫中心服务所属省市",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                }
            };

            result.ConfigAttributes = attributes;
            #endregion

            return result;
        }
    }
}
