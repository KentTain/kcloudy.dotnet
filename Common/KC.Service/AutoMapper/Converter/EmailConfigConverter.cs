using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KC.Service.DTO.Config;
using KC.Framework.Base;
using KC.Service.Base;
using KC.Service.Constants;

namespace KC.Service.AutoMapper.Converter
{
    public class EmailConfigConverter : ITypeConverter<ConfigEntity, EmailConfig>
    {
        public EmailConfig Convert(ConfigEntity source, EmailConfig destination, ResolutionContext context)
        {
            var config = source;
            if (config == null)
            {
                throw new ArgumentException("The email config is null.");
            }

            if (config.ConfigType != ConfigType.EmailConfig)
            {
                throw new ArgumentException("The config is not email configuration.");
            }

            //return new EmailConfig(config.ConfigXml);

            return ConvertEmailConfig(config);
        }

        protected EmailConfig ConvertEmailConfig(ConfigEntity configEntity)
        {
            var config = new EmailConfig
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

            var server = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.Server));
            if (server != null && !string.IsNullOrWhiteSpace(server.Value))
                config.Server = server.Value;

            var emailPort = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.Port));
            if (emailPort != null && !string.IsNullOrWhiteSpace(emailPort.Value))
            {
                var port = 587;
                int.TryParse(emailPort.Value, out port);
                config.Port = port;
            }

            var account = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.Account));
            if (account != null && !string.IsNullOrWhiteSpace(account.Value))
                config.Account = account.Value;

            var password = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.Password));
            if (password != null && !string.IsNullOrWhiteSpace(password.Value))
                config.Password = password.Value;

            var enablessl = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.EnableSsl));
            if (enablessl != null && !string.IsNullOrWhiteSpace(enablessl.Value))
            {
                var t = true;
                bool.TryParse(enablessl.Value, out t);
                config.EnableSsl = t;
            }

            var enablePwdCheck = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.EnablePwdCheck));
            if (enablePwdCheck != null && !string.IsNullOrWhiteSpace(enablePwdCheck.Value))
            {
                var t = true;
                bool.TryParse(enablePwdCheck.Value, out t);
                config.EnablePwdCheck = t;
            }

            var enableMail = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.EnableMail));
            if (enableMail != null && !string.IsNullOrWhiteSpace(enableMail.Value))
            {
                var t = true;
                bool.TryParse(enableMail.Value, out t);
                config.EnableMail = t;
            }

            var sign = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.CompanySign));
            if (sign != null && !string.IsNullOrWhiteSpace(sign.Value))
                config.CompanySign = sign.Value;

            var effectMinute = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.EffectiveMinuteCount));
            if (effectMinute != null && !string.IsNullOrWhiteSpace(effectMinute.Value))
            {
                var minute = 30;
                int.TryParse(effectMinute.Value, out minute);
                config.EffectiveMinuteCount = minute;
            }

            return config;
        }
    }

    public class ConfigEntityToMailConfigConverter : ITypeConverter<EmailConfig, ConfigEntity>
    {
        public ConfigEntity Convert(EmailConfig source, ConfigEntity destination, ResolutionContext context)
        {
            var cfgObjet = source;
            if (cfgObjet == null)
            {
                throw new ArgumentException("The email config is null.");
            }

            if (cfgObjet.ConfigType != ConfigType.EmailConfig)
            {
                throw new ArgumentException("The config is not email configuration.");
            }

            var result = new ConfigEntity()
            {
                ConfigType = ConfigType.EmailConfig,
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
                    Name = EmailConstants.Server,
                    Value =
                        !string.IsNullOrWhiteSpace(cfgObjet.Server)
                            ? cfgObjet.Server
                            : "Smtp服务器地址，如：smtp.partner.outlook.cn",
                    DisplayName = "Smtp服务器地址",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = EmailConstants.Port,
                    Value = cfgObjet.Port != 0 ? cfgObjet.Port.ToString() : "端口号，如：587",
                    DisplayName = "端口号",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.Int,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = EmailConstants.Account,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Account) ? cfgObjet.Account : "邮箱用户名，如：XXXX@cfwin.com",
                    DisplayName = "用户名",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = EmailConstants.Password,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Password) ? cfgObjet.Password : "邮箱用户密码，如：XXXXXX",
                    DisplayName = "用户密码",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.String,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = EmailConstants.EnableSsl,
                    Value = cfgObjet.EnableSsl ? "true" : "false",
                    DisplayName = "是否使用SSL",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.Bool,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = EmailConstants.EnablePwdCheck,
                    Value = cfgObjet.EnablePwdCheck ? "true" : "false",
                    DisplayName = "是否检查密码",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.Bool,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = EmailConstants.EnableMail,
                    Value = cfgObjet.EnableMail ? "true" : "false",
                    DisplayName = "是否启用配置",
                    Description = "为true时，使用配置的邮件服务器，为false时（测试环境），发送至测试邮箱（TestInBoxs）",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.Bool,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = EmailConstants.EffectiveMinuteCount,
                    Value = cfgObjet.EffectiveMinuteCount != 0 ? cfgObjet.EffectiveMinuteCount.ToString() : "30",
                    DisplayName = "邮件确认链接有效时间",
                    Description = "电子邮件确认链接有效时间(分钟)",
                    CanEdit = true,
                    ConfigEntity = result,
                    DataType = AttributeDataType.Int,
                    IsRequire = true,
                    IsDeleted = false,
                },
                new ConfigAttribute()
                {
                    Name = EmailConstants.CompanySign,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.CompanySign) ? cfgObjet.CompanySign : "",
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

    public class EmailConfigDTOConverter : ITypeConverter<ConfigEntityDTO, EmailConfig>
    {
        public EmailConfig Convert(ConfigEntityDTO source, EmailConfig destination, ResolutionContext context)
        {
            var config = source;
            if (config == null)
            {
                throw new ArgumentException("The email config is null.");
            }

            if (config.ConfigType != ConfigType.EmailConfig)
            {
                throw new ArgumentException("The config is not email configuration.");
            }

            //return new EmailConfig(config.ConfigXml);

            return ConvertEmailConfig(config);
        }

        protected EmailConfig ConvertEmailConfig(ConfigEntityDTO configEntity)
        {
            var config = new EmailConfig
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

            var server = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.Server));
            if (server != null && !string.IsNullOrWhiteSpace(server.Value))
                config.Server = server.Value;

            var emailPort = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.Port));
            if (emailPort != null && !string.IsNullOrWhiteSpace(emailPort.Value))
            {
                var port = 587;
                int.TryParse(emailPort.Value, out port);
                config.Port = port;
            }

            var account = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.Account));
            if (account != null && !string.IsNullOrWhiteSpace(account.Value))
                config.Account = account.Value;

            var password = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.Password));
            if (password != null && !string.IsNullOrWhiteSpace(password.Value))
                config.Password = password.Value;

            var enablessl = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.EnableSsl));
            if (enablessl != null && !string.IsNullOrWhiteSpace(enablessl.Value))
            {
                var t = true;
                bool.TryParse(enablessl.Value, out t);
                config.EnableSsl = t;
            }

            var enablePwdCheck = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.EnablePwdCheck));
            if (enablePwdCheck != null && !string.IsNullOrWhiteSpace(enablePwdCheck.Value))
            {
                var t = true;
                bool.TryParse(enablePwdCheck.Value, out t);
                config.EnablePwdCheck = t;
            }

            var enableMail = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.EnableMail));
            if (enableMail != null && !string.IsNullOrWhiteSpace(enableMail.Value))
            {
                var t = true;
                bool.TryParse(enableMail.Value, out t);
                config.EnableMail = t;
            }

            var sign = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.CompanySign));
            if (sign != null && !string.IsNullOrWhiteSpace(sign.Value))
                config.CompanySign = sign.Value;

            var effectMinute = attributes.FirstOrDefault(m => m.Name.Equals(EmailConstants.EffectiveMinuteCount));
            if (effectMinute != null && !string.IsNullOrWhiteSpace(effectMinute.Value))
            {
                var minute = 30;
                int.TryParse(effectMinute.Value, out minute);
                config.EffectiveMinuteCount = minute;
            }

            return config;
        }
    }

    public class ConfigEntityDTOToMailConfigConverter : ITypeConverter<EmailConfig, ConfigEntityDTO>
    {
        public ConfigEntityDTO Convert(EmailConfig source, ConfigEntityDTO destination, ResolutionContext context)
        {
            var cfgObjet = source;
            if (cfgObjet == null)
            {
                throw new ArgumentException("The email config is null.");
            }

            if (cfgObjet.ConfigType != ConfigType.EmailConfig)
            {
                throw new ArgumentException("The config is not email configuration.");
            }

            var result = new ConfigEntityDTO()
            {
                ConfigType = ConfigType.EmailConfig,
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
                    Name = EmailConstants.Server,
                    Value =
                        !string.IsNullOrWhiteSpace(cfgObjet.Server)
                            ? cfgObjet.Server
                            : "Smtp服务器地址，如：smtp.partner.outlook.cn",
                    DisplayName = "Smtp服务器地址",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.String,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = EmailConstants.Port,
                    Value = cfgObjet.Port != 0 ? cfgObjet.Port.ToString() : "端口号，如：587",
                    DisplayName = "端口号",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.Int,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = EmailConstants.Account,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Account) ? cfgObjet.Account : "邮箱用户名，如：XXXX@cfwin.com",
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
                    Name = EmailConstants.Password,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.Password) ? cfgObjet.Password : "邮箱用户密码，如：XXXXXX",
                    DisplayName = "用户密码",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.String,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = EmailConstants.EnableSsl,
                    Value = cfgObjet.EnableSsl ? "true" : "false",
                    DisplayName = "是否使用SSL",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.Bool,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = EmailConstants.EnablePwdCheck,
                    Value = cfgObjet.EnablePwdCheck ? "true" : "false",
                    DisplayName = "是否检查密码",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.Bool,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = EmailConstants.EnableMail,
                    Value = cfgObjet.EnableMail ? "true" : "false",
                    DisplayName = "是否启用配置",
                    Description = "为true时，使用配置的邮件服务器，为false时（测试环境），发送至测试邮箱（TestInBoxs）",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.Bool,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = EmailConstants.EffectiveMinuteCount,
                    Value = cfgObjet.EffectiveMinuteCount != 0 ? cfgObjet.EffectiveMinuteCount.ToString() : "30",
                    DisplayName = "邮件确认链接有效时间",
                    Description = "电子邮件确认链接有效时间(分钟)",
                    CanEdit = true,
                    ConfigId = result.ConfigId,
                    ConfigName = result.ConfigName,
                    DataType = AttributeDataType.Int,
                    IsProviderAttr = false,
                    IsDeleted = false,
                },
                new ConfigAttributeDTO()
                {
                    Name = EmailConstants.CompanySign,
                    Value = !string.IsNullOrWhiteSpace(cfgObjet.CompanySign) ? cfgObjet.CompanySign : "",
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
