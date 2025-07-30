using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KC.Framework.Base;
using KC.Service.Base;
using KC.Service.Constants;

namespace KC.Service.AutoMapper.Converter
{
    public class WeixinConfigConverter : ITypeConverter<ConfigEntity, WeixinConfig>
    {
        public WeixinConfig Convert(ConfigEntity source, WeixinConfig destination, ResolutionContext context)
        {
            var config = source;
            if (config == null)
            {
                throw new ArgumentException("The email config is null.");
            }

            if (config.ConfigType != ConfigType.WeixinConfig)
            {
                throw new ArgumentException("The config is not Weixin configuration.");
            }

            return ConvertWeixinConfig(config);
        }

        protected WeixinConfig ConvertWeixinConfig(ConfigEntity configEntity)
        {
            var config = new WeixinConfig
            {
                ConfigId = configEntity.ConfigId,
                ConfigName = configEntity.ConfigName,
                ConfigType = configEntity.ConfigType,
                ConfigDescription = configEntity.ConfigDescription
            };

            var attributes = configEntity.ConfigAttributes.ToList();

            //var properties = typeof(SmsConfig).GetProperties();
            //attributes.ForEach(k =>
            //{
            //    var property = properties.FirstOrDefault(m => m.Name.Equals(k.Name, StringComparison.OrdinalIgnoreCase));
            //    if (property != null)
            //    {
            //        if (k.Name.Equals("Type"))
            //        {
            //            //config.Type = (SmsType)Enum.Parse(typeof(SmsType), k.Value);
            //        }
            //        else
            //        {
            //            property.SetValue(config, System.Convert.ChangeType(k.Value, property.PropertyType));
            //        }

            //    }
            //});

            //return config;

            var server = attributes.FirstOrDefault(m => m.Name.Equals(WeixinConstants.AppId));
            if (server != null && !string.IsNullOrWhiteSpace(server.Value))
                config.AppId = server.Value;

            var emailPort = attributes.FirstOrDefault(m => m.Name.Equals(WeixinConstants.AppSecret));
            if (emailPort != null && !string.IsNullOrWhiteSpace(emailPort.Value))
            {
                config.AppSecret = emailPort.Value;
            }

            var account = attributes.FirstOrDefault(m => m.Name.Equals(WeixinConstants.AppToken));
            if (account != null && !string.IsNullOrWhiteSpace(account.Value))
                config.AppToken = account.Value;

            var password = attributes.FirstOrDefault(m => m.Name.Equals(WeixinConstants.Value1));
            if (password != null && !string.IsNullOrWhiteSpace(password.Value))
                config.Value1 = password.Value;

            var enablessl = attributes.FirstOrDefault(m => m.Name.Equals(WeixinConstants.Value2));
            if (enablessl != null && !string.IsNullOrWhiteSpace(enablessl.Value))
            {
                config.Value1 = enablessl.Value;
            }

            return config;
        }
    }
    public class ConfigEntityToWeixinConfigConverter : ITypeConverter<WeixinConfig, ConfigEntity>
    {
        public ConfigEntity Convert(WeixinConfig source, ConfigEntity destination, ResolutionContext context)
        {
            var cfgObjet = source;
            if (cfgObjet == null)
            {
                throw new ArgumentException("The Weixin config is null.");
            }

            if (cfgObjet.ConfigType != ConfigType.WeixinConfig)
            {
                throw new ArgumentException("The config is not Weixin configuration.");
            }

            var result = new ConfigEntity()
            {
                ConfigType = ConfigType.WeixinConfig,
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
                    Name = WeixinConstants.AppId,
                    Value =
                        !string.IsNullOrWhiteSpace(cfgObjet.AppId)
                            ? cfgObjet.AppId
                            : "应用Id，如：wx535acd8472971861",
                    DisplayName = "应用Id",
                    Description = "设置企业微信公众中所申请的AppId",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = WeixinConstants.AppSecret,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.AppSecret) 
                        ? cfgObjet.AppSecret 
                        : "应用Secret，如：2ff204ecdd7526e68578d5590c89dd1c",
                    DisplayName = "应用Secret",
                    Description = "设置企业微信公众中所申请的Secret",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.Int,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = WeixinConstants.AppToken,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.AppToken) 
                        ? cfgObjet.AppToken 
                        : "应用Token，如：1231231234",
                    DisplayName = "应用Token",
                    Description = "设置企业微信公众中所申请的Token",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = WeixinConstants.Value1,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Value1) 
                        ? cfgObjet.Value1 
                        : "Value1，如：XXXXXX",
                    DisplayName = "Value1",
                    Description = "微信公众号的其他设置Value1",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = WeixinConstants.Value2,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Value2) 
                        ? cfgObjet.Value2 
                        : "Value2，如：XXXXXX",
                    DisplayName = "Value2",
                    Description = "微信公众号的其他设置Value2",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.Bool,
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
