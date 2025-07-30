using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using KC.Service.DTO.Config;
using KC.Framework.Base;
using KC.Service.Base;
using KC.Service.Constants;

namespace KC.Service.AutoMapper.Converter
{
    public class SmsConfigConverter : ITypeConverter<ConfigEntity, SmsConfig>
    {
        public SmsConfig Convert(ConfigEntity source, SmsConfig destination, ResolutionContext context)
        {
            var config = source;
            if (config == null)
            {
                throw new ArgumentException("The sms config can't be null.");
            }

            if (config.ConfigType != ConfigType.SmsConfig)
            {
                throw new ArgumentException("The config isn't a sms configuration.");
            }

            //return new SmsConfig(config.ConfigXml);

            return ConvertSmsConfig(config);
        }

        protected SmsConfig ConvertSmsConfig(ConfigEntity configEntity)
        {
            var config = new SmsConfig()
            {
                ConfigId = configEntity.ConfigId,
                ConfigName = configEntity.ConfigName,
                ConfigType = configEntity.ConfigType,
                ConfigDescription = configEntity.ConfigDescription
            };

            var attributes = configEntity.ConfigAttributes.ToList();

            var properties = typeof(SmsConfig).GetProperties();
            attributes.ForEach(k =>
            {
                var property = properties.FirstOrDefault(m => m.Name.Equals(k.Name, StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    if (k.Name.Equals("Type"))
                    {
                        config.Type = (SmsType)Enum.Parse(typeof(SmsType), k.Value);
                    }
                    else
                    {
                        property.SetValue(config, System.Convert.ChangeType(k.Value, property.PropertyType));
                    }

                }
            });

            return config;

            //var providerName = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.ProviderName));
            //if (providerName == null || string.IsNullOrWhiteSpace(providerName.Value))
            //    return null;
            //config.ProviderName = providerName.Value;

            //var smsUrl = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.SmsUrl));
            //if (smsUrl == null || string.IsNullOrWhiteSpace(smsUrl.Value))
            //    return null;
            //config.SmsUrl = smsUrl.Value;

            //var account = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.UserAccount));
            //if (account == null || string.IsNullOrWhiteSpace(account.Value))
            //    return null;
            //config.UserAccount = account.Value;

            //var password = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.Password));
            //if (password == null || string.IsNullOrWhiteSpace(password.Value))
            //    return null;
            //config.Password = password.Value;

            //var type = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.Type));
            //if (type != null)
            //{
            //    var t = 0;
            //    if (int.TryParse(type.Value, out t))
            //    {
            //        config.Type = (SmsType)t;
            //    }
            //}

            //var signature = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.Signature));
            //if (signature != null)
            //    config.Signature = signature.Value;
            //var userId = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.UserId));
            //if (userId != null)
            //    config.UserId = userId.Value;

            //return config;
        }

    }
    public class ConfigEntityToSmsConfigConverter : ITypeConverter<SmsConfig, ConfigEntity>
    {
        public ConfigEntity Convert(SmsConfig source, ConfigEntity destination, ResolutionContext context)
        {
            var cfgObjet = source;
            if (cfgObjet == null)
            {
                throw new ArgumentException("The sms config is not null.");
            }

            if (cfgObjet.ConfigType != ConfigType.SmsConfig)
            {
                throw new ArgumentException("The config is not sms configuration.");
            }

            var result = new ConfigEntity()
            {
                ConfigType = ConfigType.SmsConfig,
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
                            : "平台名称，如：CL（创蓝短信平台）",
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
                    Name = SmsConstants.Type,
                    Value = cfgObjet.Type != 0 ? cfgObjet.Type.ToString() : "类型，如：1（营销短信）",
                    DisplayName = "类型",
                    Description = "用途：0：通知短信；1：营销短信；2：语音短信",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.Int,
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
                    Name = SmsConstants.UserId,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.UserId) ? cfgObjet.UserId : "UserId，如：123456",
                    DisplayName = "用户ID",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = SmsConstants.UserAccount,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.UserAccount) ? cfgObjet.UserAccount : "用户名，如：XXXXXX",
                    DisplayName = "用户名",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = SmsConstants.Password,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Password) ? cfgObjet.Password : "密码，如：XXXXXX",
                    DisplayName = "密码",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = SmsConstants.Signature,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Signature) ? cfgObjet.Signature : "",
                    DisplayName = "签名设置",
                    Description = "企业签名设置",
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


    public class SmsConfigDTOConverter : ITypeConverter<ConfigEntityDTO, SmsConfig>
    {
        public SmsConfig Convert(ConfigEntityDTO source, SmsConfig destination, ResolutionContext context)
        {
            var config = source;
            if (config == null)
            {
                throw new ArgumentException("The sms config can't be null.");
            }

            if (config.ConfigType != ConfigType.SmsConfig)
            {
                throw new ArgumentException("The config isn't a sms configuration.");
            }

            //return new SmsConfig(config.ConfigXml);

            return ConvertSmsConfig(config);
        }

        protected SmsConfig ConvertSmsConfig(ConfigEntityDTO configEntity)
        {
            var config = new SmsConfig()
            {
                ConfigId = configEntity.ConfigId,
                ConfigName = configEntity.ConfigName,
                ConfigType = configEntity.ConfigType,
                ConfigDescription = configEntity.ConfigDescription
            };

            var attributes = configEntity.ConfigAttributes.ToList();

            var properties = typeof(SmsConfig).GetProperties();
            attributes.ForEach(k =>
            {
                var property = properties.FirstOrDefault(m => m.Name.Equals(k.Name, StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    if (k.Name.Equals("Type"))
                    {
                        config.Type = (SmsType)Enum.Parse(typeof(SmsType), k.Value);
                    }
                    else
                    {
                        property.SetValue(config, System.Convert.ChangeType(k.Value, property.PropertyType));
                    }

                }
            });

            return config;

            //var providerName = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.ProviderName));
            //if (providerName == null || string.IsNullOrWhiteSpace(providerName.Value))
            //    return null;
            //config.ProviderName = providerName.Value;

            //var smsUrl = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.SmsUrl));
            //if (smsUrl == null || string.IsNullOrWhiteSpace(smsUrl.Value))
            //    return null;
            //config.SmsUrl = smsUrl.Value;

            //var account = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.UserAccount));
            //if (account == null || string.IsNullOrWhiteSpace(account.Value))
            //    return null;
            //config.UserAccount = account.Value;

            //var password = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.Password));
            //if (password == null || string.IsNullOrWhiteSpace(password.Value))
            //    return null;
            //config.Password = password.Value;

            //var type = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.Type));
            //if (type != null)
            //{
            //    var t = 0;
            //    if (int.TryParse(type.Value, out t))
            //    {
            //        config.Type = (SmsType)t;
            //    }
            //}

            //var signature = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.Signature));
            //if (signature != null)
            //    config.Signature = signature.Value;
            //var userId = attributes.FirstOrDefault(m => m.Name.Equals(SmsConstants.UserId));
            //if (userId != null)
            //    config.UserId = userId.Value;

            //return config;
        }

    }
    public class ConfigEntityDTOToSmsConfigConverter : ITypeConverter<SmsConfig, ConfigEntityDTO>
    {
        public ConfigEntityDTO Convert(SmsConfig source, ConfigEntityDTO destination, ResolutionContext context)
        {
            var cfgObjet = source;
            if (cfgObjet == null)
            {
                throw new ArgumentException("The sms config is not null.");
            }

            if (cfgObjet.ConfigType != ConfigType.SmsConfig)
            {
                throw new ArgumentException("The config is not sms configuration.");
            }

            var result = new ConfigEntityDTO()
            {
                ConfigType = ConfigType.SmsConfig,
                ConfigId = cfgObjet.ConfigId,
                ConfigName = cfgObjet.ConfigName,
                ConfigDescription = cfgObjet.ConfigDescription,
                ConfigXml = cfgObjet.GetConfigObjectXml()
            };

            #region Attributes
            var attributes = new List<ConfigAttributeDTO>
            {
                new ConfigAttributeDTO()
                {
                    Name = SmsConstants.ProviderName,
                    Value =
                        !string.IsNullOrWhiteSpace(cfgObjet.ProviderName)
                            ? cfgObjet.ProviderName
                            : "平台名称，如：CL（创蓝短信平台）",
                    DisplayName = "平台名称",
                    Description = "需要结合程序中实现的Class名称使用。",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.String,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = SmsConstants.Type,
                    Value = cfgObjet.Type != 0 ? cfgObjet.Type.ToString() : "类型，如：1（营销短信）",
                    DisplayName = "类型",
                    Description = "用途：0：通知短信；1：营销短信；2：语音短信",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.Int,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = SmsConstants.SmsUrl,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.SmsUrl) ? cfgObjet.SmsUrl : "发送地址，如：http://111.111.111.111:8888/sms.aspx",
                    DisplayName = "发送地址",
                    Description = "发送地址，此项必须，只需要修改值",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.String,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = SmsConstants.UserId,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.UserId) ? cfgObjet.UserId : "UserId，如：123456",
                    DisplayName = "用户ID",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.String,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = SmsConstants.UserAccount,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.UserAccount) ? cfgObjet.UserAccount : "用户名，如：XXXXXX",
                    DisplayName = "用户名",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.String,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = SmsConstants.Password,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Password) ? cfgObjet.Password : "密码，如：XXXXXX",
                    DisplayName = "密码",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.String,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = SmsConstants.Signature,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Signature) ? cfgObjet.Signature : "",
                    DisplayName = "签名设置",
                    Description = "企业签名设置",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.String,
                    IsProviderAttr = false,
                    IsDeleted = false,
                }
            };

            result.ConfigAttributes = attributes;
            #endregion

            return result;
        }
    }
}
