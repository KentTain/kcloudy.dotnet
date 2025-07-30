using AutoMapper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using KC.DataAccess.Account.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.Base;
using KC.Service.Constants;
using KC.Service.EFService;
using KC.Service.Util;
using KC.Service.DTO.Config;
using KC.Service.DTO;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using KC.Model.Config;

namespace KC.Service.Config
{
    public interface IConfigService : IEFService
    {
        #region Config

        List<ConfigEntityDTO> FindAllConfigsWithProperties();
        List<ConfigEntityDTO> FindAllConfigsWithPropertiesByType(ConfigType type);
        PaginatedBaseDTO<ConfigEntityDTO> FindConfigsByFilter(int pageIndex, int pageSize, string searchValue, int searchType, string sort, string order);

        ConfigEntityDTO GetConfigById(int configId);
        ConfigEntityDTO GetConfigByType(ConfigType type);

        bool SaveConfig(ConfigEntityDTO model);
        Task<bool> SaveConfigWithAttributesAsync(ConfigEntityDTO model);

        Task<bool> RemoveConfigByIdAsync(int id, string currentUserId, string currentUserName);

        bool SaveCBSConfig(ConfigEntityDTO model);
        bool SoftRemoveConfigEntityById(int configId);

        bool ActivePaymentConfigByType(ConfigType type, string paymentDomain);
        bool ActiveConfigration(int configId, ConfigStatus state);
        #endregion

        #region ConfigAttribute
        List<ConfigAttributeDTO> FindConfigAttributesByConfigId(int configId);

        ConfigAttributeDTO GetPropertyById(int propertyId);

        bool SoftRemoveConfigAttributeById(int propertyId);
        #endregion


        PaginatedBaseDTO<ConfigLogDTO> FindPaginatedConfigLogs(int pageIndex, int pageSize, string name);

        bool AddEmailConfig();
        bool AddSmsConfig();
        bool AddCallCenterConfig();

        EmailConfig GetEmailConfig();
        SmsConfig GetSmsConfig();
    }

    public class ConfigService : EFServiceBase, IConfigService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        private EFUnitOfWorkContextBase _unitOfContext;

        private IConfigEntityRepository _configEntityRepository;
        private IDbRepository<ConfigAttribute> _configAttributeRepository;
        private IDbRepository<ConfigLog> _configLogRepository;

        public ConfigService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            IConfigEntityRepository configEntityRepository,
            IDbRepository<ConfigAttribute> configAttributeRepository,
            IDbRepository<ConfigLog> configLogRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<ConfigService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfContext = unitOfContext;

            _configEntityRepository = configEntityRepository;
            _configAttributeRepository = configAttributeRepository;
            _configLogRepository = configLogRepository;
        }

        #region ConfigEntity

        public List<ConfigEntityDTO> FindAllConfigsWithProperties()
        {
            var data = _configEntityRepository.GetConfigsWithAttributesByFilter(c => !c.IsDeleted);
            return _mapper.Map<List<ConfigEntityDTO>>(data);
        }
        public List<ConfigEntityDTO> FindAllConfigsWithPropertiesByType(ConfigType type)
        {
            var data = _configEntityRepository.GetConfigsWithAttributesByFilter(c => !c.IsDeleted && type == c.ConfigType);
            return _mapper.Map<List<ConfigEntityDTO>>(data);
        }
        public PaginatedBaseDTO<ConfigEntityDTO> FindConfigsByFilter(int pageIndex, int pageSize, string searchValue, int searchType, string sort, string order)
        {
            Expression<Func<ConfigEntity, bool>> predicate = m => true && !m.IsDeleted && m.ConfigType != ConfigType.WeixinConfig;
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                predicate = predicate.And(m => m.ConfigName.Contains(searchValue));
            }
            if (searchType != 0)
            {
                var t = (ConfigType)searchType;
                predicate = predicate.And(m => m.ConfigType == t);
            }
            var data = _configEntityRepository.FindPagenatedListWithCount<ConfigEntity>(pageIndex, pageSize, predicate, sort, order.Equals("asc"));

            var total = data.Item1;
            var rows = _mapper.Map<List<ConfigEntityDTO>>(data.Item2.Where(m => m.ConfigType != ConfigType.PaymentMethod));
            return new PaginatedBaseDTO<ConfigEntityDTO>(pageIndex, pageSize, total, rows);
        }

        public ConfigEntityDTO GetConfigById(int configId)
        {
            var data = _configEntityRepository.GetConfigWithAttributesById(configId);
            return _mapper.Map<ConfigEntityDTO>(data);
        }
        public ConfigEntityDTO GetConfigByType(ConfigType type)
        {
            var data = _configEntityRepository.GetConfigsWithAttributesByFilter(c => c.ConfigType == type && !c.IsDeleted && c.State == ConfigStatus.Enable).FirstOrDefault();
            return _mapper.Map<ConfigEntityDTO>(data);
        }

        public bool SaveConfig(ConfigEntityDTO model)
        {
            var data = _mapper.Map<ConfigEntity>(model);
            if (data.ConfigId == 0)
            {
                return _configEntityRepository.Add(data);
            }
            else
            {
                return _configEntityRepository.Modify(data, true);
            }
        }
        public async Task<bool> SaveConfigWithAttributesAsync(ConfigEntityDTO model)
        {
            var data = _mapper.Map<ConfigEntity>(model);
            //新增表单定义（新增表单定义及相关表单数据）
            var addFields = new List<ConfigAttribute>();
            if (!model.IsEditMode)
            {
                //保存表单表单，只保存两级的Field，超出不再保存
                foreach (var field in data.ConfigAttributes)
                {
                    field.PropertyAttributeId = 0;
                    field.ConfigId = data.ConfigId;
                    field.ConfigEntity = data;
                    addFields.Add(field);
                }

                await _configAttributeRepository.AddAsync(addFields, false);
                await _configEntityRepository.AddAsync(data, false);

                //添加日志
                var log = new ConfigLog()
                {
                    ConfigId = data.ConfigId,
                    ConfigName = data.ConfigName,
                    OperatorId = model.CreatedBy,
                    Operator = model.CreatedName,
                    Remark = string.Format("新增配置数据: " + data.ConfigName)
                };
                await _configLogRepository.AddAsync(log, false);
            }
            //修改表单定义（保存新的表单属性数据，同时更新表单定义数据）
            else
            {
                _configAttributeRepository.Remove(m => m.ConfigId.Equals(data.ConfigId), false);
                //删除老的表单属性数据，插入新的表单属性数据
                if (data.ConfigAttributes.Any())
                {
                    //保存表单属性
                    foreach (var field in data.ConfigAttributes)
                    {
                        field.PropertyAttributeId = 0;
                        field.ConfigId = data.ConfigId;
                        field.ConfigEntity = data;
                        addFields.Add(field);
                    }
                }

                await _configAttributeRepository.AddAsync(addFields, false);
                await _configEntityRepository.ModifyAsync(data, new string[]
                     { "Name", "BusinessType", "Description",
                         "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);

                //添加日志
                var log = new ConfigLog()
                {
                    ConfigId = data.ConfigId,
                    ConfigName = data.ConfigName,
                    OperatorId = model.ModifiedBy,
                    Operator = model.ModifiedName,
                    Remark = string.Format("编辑配置数据: " + data.ConfigName)
                };
                await _configLogRepository.AddAsync(log, false);
            }

            return await _unitOfContext.CommitAsync() > 0;
        }

        public async Task<bool> RemoveConfigByIdAsync(int id, string currentUserId, string currentUserName)
        {
            var data = await _configEntityRepository.GetByIdAsync(id);
            if (data != null)
            {
                data.IsDeleted = true;
                data.ModifiedBy = currentUserId;
                data.ModifiedName = currentUserName;
                data.ModifiedDate = DateTime.UtcNow;
                await _configEntityRepository.ModifyAsync(data, new string[] { "IsDeleted",
                         "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);
            }

            var list = await _configAttributeRepository.FindAllAsync(m => m.ConfigId == id);
            if (list.Any())
            {
                list.ToList().ForEach(m => {
                    m.IsDeleted = true;
                    m.ModifiedBy = currentUserId;
                    m.ModifiedName = currentUserName;
                    m.ModifiedDate = DateTime.UtcNow;
                });
                await _configAttributeRepository.ModifyAsync(list, new string[] { "IsDeleted",
                         "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);
            }
            var log = new ConfigLog()
            {
                ConfigId = data.ConfigId,
                ConfigName = data.ConfigName,
                OperatorId = currentUserId,
                Operator = currentUserName,
                Remark = string.Format("删除配置数据: " + data.ConfigName)
            };
            await _configLogRepository.AddAsync(log, false);

            return _unitOfContext.Commit() > 0;
        }

        public bool SaveCBSConfig(ConfigEntityDTO model)
        {
            var data = _mapper.Map<ConfigEntity>(model);
            if (data.ConfigId == 0)
            {
                return _configEntityRepository.Add(data);
            }
            else
            {
                var configAttributes = data.ConfigAttributes;
                return _configAttributeRepository.Modify(configAttributes) > 0;
            }
        }
        public bool SoftRemoveConfigEntityById(int configId)
        {
            var data = _configEntityRepository.GetById(configId);
            data.IsDeleted = true;
            return _configEntityRepository.Modify(data, true);
        }
        public bool SetWeiXinConfig(WeixinConfig config)
        {
            config.AppId = config.AppId.Trim();
            config.AppSecret = config.AppSecret.Trim();
            config.AppToken = config.AppToken.Trim();
            config.EncodingAESKey = config.EncodingAESKey.Trim();
            var wxConfig = _configEntityRepository.GetConfigsWithAttributesByFilter(c => c.ConfigType == ConfigType.WeixinConfig && !c.IsDeleted).FirstOrDefault();
            var needValid = true;
            if (wxConfig != null && !wxConfig.ConfigAttributes.IsNullOrEmpty())
            {
                ConfigAttribute appIdAttr = null;
                ConfigAttribute appSecretAttr = null;
                appIdAttr = wxConfig.ConfigAttributes.FirstOrDefault(m => m.Name.Equals(WeixinConstants.AppId, StringComparison.CurrentCultureIgnoreCase));
                appSecretAttr = wxConfig.ConfigAttributes.FirstOrDefault(m => m.Name.Equals(WeixinConstants.AppSecret, StringComparison.CurrentCultureIgnoreCase));
                if (appIdAttr != null && appSecretAttr != null && config.AppId == appIdAttr.Value && config.AppSecret == appSecretAttr.Value)
                    needValid = false;
            }
            if (needValid)//AppId和AppSecret未改变，不去验证
                WeChatUtil.CheckAppValid(config.AppId, config.AppSecret);
            bool success = false;
            if (wxConfig == null)
            {
                wxConfig = new ConfigEntity()
                {
                    ConfigType = ConfigType.WeixinConfig,
                    ConfigName = "微信公众号",
                    ConfigDescription = "微信公众号",
                    ConfigAttributes = new List<ConfigAttribute>() {
                        new ConfigAttribute(){
                            DisplayName =WeixinConstants.AppId,
                            Description ="值来源：微信公众平台>>开发>>基本配置>>公众号开发信息>>AppId",
                            Name =WeixinConstants.AppId,
                            Value=config.AppId,
                            CanEdit = true,
                            DataType = AttributeDataType.String,
                            IsRequire = false,
                        },
                        new ConfigAttribute(){
                            DisplayName =WeixinConstants.AppSecret,
                            Description ="值来源：微信公众平台>>开发>>基本配置>>公众号开发信息>>AppSecret",
                            Name =WeixinConstants.AppSecret,
                            Value =config.AppSecret,
                            CanEdit = true,
                            DataType = AttributeDataType.String,
                            IsRequire = false,
                        },
                        new ConfigAttribute(){
                            DisplayName =WeixinConstants.AppToken,
                            Description ="值来源：微信公众平台>>开发>>基本配置>>服务器配置>>Token",
                            Name =WeixinConstants.AppToken,
                            Value =config.AppToken,
                            CanEdit = true,
                            DataType = AttributeDataType.String,
                            IsRequire = false,
                        },
                        new ConfigAttribute(){
                            DisplayName =WeixinConstants.EncodingAESKey,
                            Description ="值来源：微信公众平台>>开发>>基本配置>>服务器配置>>EncodingAESKey",
                            Name =WeixinConstants.EncodingAESKey,Value=config.EncodingAESKey,
                            CanEdit = true,
                            DataType = AttributeDataType.String,
                            IsRequire = false,
                        },
                        new ConfigAttribute(){
                            DisplayName ="消息加解密方式",
                            Description ="值来源：微信公众平台>>开发>>基本配置>>服务器配置>>消息加解密方式",
                            Name =WeixinConstants.WeiXinMessageMode,
                            Value =config.WeiXinMessageMode.ToString(),
                            CanEdit = true,
                            DataType = AttributeDataType.String,
                            IsRequire = false,
                        }
                    }
                };
                success = _configEntityRepository.Add(wxConfig);
            }
            else
            {
                //把原先的ConfigAttributes全删，再添加，不用去判断之前是否存在了
                if (!wxConfig.ConfigAttributes.IsNullOrEmpty())
                    _configAttributeRepository.RemoveById(wxConfig.ConfigAttributes.ToList(), false);
                var configAttributes = new List<ConfigAttribute>() {
                     new ConfigAttribute(){
                         ConfigId=wxConfig.ConfigId,
                         DisplayName =WeixinConstants.AppId,
                         Description ="值来源：微信公众平台>>开发>>基本配置>>公众号开发信息>>AppId",
                         Name =WeixinConstants.AppId,
                         Value =config.AppId,
                         CanEdit = true,
                         DataType = AttributeDataType.String,
                         IsRequire = false,
                     },
                    new ConfigAttribute(){
                        ConfigId =wxConfig.ConfigId,
                        DisplayName =WeixinConstants.AppSecret,
                        Description ="值来源：微信公众平台>>开发>>基本配置>>公众号开发信息>>AppSecret",
                        Name =WeixinConstants.AppSecret,
                        Value =config.AppSecret,
                        CanEdit = true,
                        DataType = AttributeDataType.String,
                        IsRequire = false,
                    },
                    new ConfigAttribute(){
                        ConfigId =wxConfig.ConfigId,
                        DisplayName =WeixinConstants.AppToken,
                        Description ="值来源：微信公众平台>>开发>>基本配置>>服务器配置>>Token",
                        Name =WeixinConstants.AppToken,
                        Value =config.AppToken,
                        CanEdit = true,
                        DataType = AttributeDataType.String,
                        IsRequire = false,
                    },
                    new ConfigAttribute(){
                        ConfigId =wxConfig.ConfigId,
                        DisplayName =WeixinConstants.EncodingAESKey,
                        Description ="值来源：微信公众平台>>开发>>基本配置>>服务器配置>>EncodingAESKey",
                        Name =WeixinConstants.EncodingAESKey,Value=config.EncodingAESKey,
                        CanEdit = true,
                        DataType = AttributeDataType.String,
                        IsRequire = false,
                    },
                    new ConfigAttribute(){
                        ConfigId =wxConfig.ConfigId,
                        DisplayName ="消息加解密方式",
                        Description ="值来源：微信公众平台>>开发>>基本配置>>服务器配置>>消息加解密方式",
                        Name =WeixinConstants.WeiXinMessageMode,
                        Value =config.WeiXinMessageMode.ToString(),
                        CanEdit = true,
                        DataType = AttributeDataType.String,
                        IsRequire = false,
                    }
                };
                success = _configAttributeRepository.Add(configAttributes) > 0;
            }
            if (success)
            {
                var access_token_key = CacheKeyConstants.WeiXinCacheKey.AccessTokenKey(Tenant.TenantName);
                var config_key = CacheKeyConstants.WeiXinCacheKey.ConfigKey(Tenant.TenantName);
                var js_ticket_key = CacheKeyConstants.WeiXinCacheKey.JsTicketKey(Tenant.TenantName);//这个删不删无所谓
                Service.CacheUtil.RemoveCache(config_key);
                if (needValid)//AppId和Appsecret改变才清AccessToken和Js_Ticket缓存
                {
                    Service.CacheUtil.RemoveCache(access_token_key);
                    Service.CacheUtil.RemoveCache(js_ticket_key);
                }
            }
            return success;
        }

        /// <summary>
        /// 用户自定义支付配置
        /// 查询平台的配置插入租户数据库
        /// 首先把配置的状态设置为-1
        /// 然后对需要修改配置属性值清空
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool ActivePaymentConfigByType(ConfigType type, string paymentDomain)
        {
            //获取平台支付的配置
            var configData = _configEntityRepository.FindAll(m => m.ConfigType == type);
            foreach (var configEntityDTO in configData)
            {
                ConfigEntity configEntity = new ConfigEntity();
                configEntity = _mapper.Map<ConfigEntity>(configEntityDTO);
                //改配置的状态
                configEntity.State = ConfigStatus.Draft;
                configEntity.ConfigId = 0;
                var ConfigAttributes = configEntity.ConfigAttributes;
                foreach (var ConfigAttribute in ConfigAttributes)
                {

                    //查找需要修改的配置属性值
                    if (ConfigAttribute.CanEdit)
                    {
                        //清空值
                        ConfigAttribute.Value = "";
                    }
                    //替换前端返回的URL的配置
                    if (ConfigAttribute.Name == "frontUrl" || ConfigAttribute.Name == "page_notify_url" || ConfigAttribute.Name == "pickupUrl")
                    {
                        var configValue = ConfigAttribute.Value;
                        //正则表达式
                        var regexStr = @"(http|https)?\:\/\/[\.\-\w]+\/";
                        //查出需要替换的字符串
                        string replaceStr = Regex.Match(configValue, regexStr).Value;
                        //替换
                        string value = configValue.Replace(replaceStr, paymentDomain);
                        ConfigAttribute.Value = value;
                    }

                    ConfigAttribute.PropertyAttributeId = 0;
                }
                _configEntityRepository.Add(configEntity, false);
            }
            return _unitOfContext.Commit() > 0; ;

        }

        public bool ActiveConfigration(int configId, ConfigStatus state)
        {
            var data = _configEntityRepository.GetById(configId);
            data.State = state;
            return _configEntityRepository.Modify(data, true);
        }

        #endregion

        #region ConfigAttribute

        public List<ConfigAttributeDTO> FindConfigAttributesByConfigId(int configId)
        {
            Expression<Func<ConfigAttribute, bool>> predicate = m => m.ConfigId.Equals(configId) && !m.IsDeleted;

            var data = _configAttributeRepository.FindAll(predicate, k => k.CreatedDate, false);
            return _mapper.Map<List<ConfigAttributeDTO>>(data);
        }

        public ConfigAttributeDTO GetPropertyById(int propertyId)
        {
            var data = _configAttributeRepository.GetById(propertyId);
            return _mapper.Map<ConfigAttributeDTO>(data);
        }

        public bool SoftRemoveConfigAttributeById(int propertyId)
        {
            var data = _configAttributeRepository.GetById(propertyId);
            data.IsDeleted = true;
            return _configAttributeRepository.Modify(data, new string[] { "IsDeleted" }, true);
        }

        #endregion

        #region 日志
        public PaginatedBaseDTO<ConfigLogDTO> FindPaginatedConfigLogs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<ConfigLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _configLogRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.OperateDate, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<ConfigLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<ConfigLogDTO>(pageIndex, pageSize, total, rows);

        }
        #endregion

        public bool AddEmailConfig()
        {
            return _configEntityRepository.AddEmailConfig();
        }

        public bool AddSmsConfig()
        {
            return _configEntityRepository.AddSmsConfig();
        }

        public bool AddCallCenterConfig()
        {
            return _configEntityRepository.AddCallCenterConfig();
        }

        public EmailConfig GetEmailConfig()
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "-EmailConfig";
            var emailConfig = CacheUtil.GetCache<EmailConfig>(cacheKey);
            if (emailConfig != null) return emailConfig;

            try
            {
                var dbconfigs = GetConfigByType(ConfigType.EmailConfig);
                if (dbconfigs != null)
                {
                    emailConfig = _mapper.Map<EmailConfig>(dbconfigs);
                    if (emailConfig != null)
                    {
                        CacheUtil.SetCache(cacheKey, emailConfig, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                        return emailConfig;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("获取租户邮件配置出错，错误消息：{0}", ex.Message));
            }

            return null;
        }

        public SmsConfig GetSmsConfig()
        {
            var cacheKey = CacheKeyConstants.Prefix.ConfigName + "-SmsCofnig";
            var smsConfig = CacheUtil.GetCache<SmsConfig>(cacheKey);
            if (smsConfig != null) return smsConfig;

            try
            {
                var dbConfigs = GetConfigByType(ConfigType.SmsConfig);
                if (dbConfigs != null)
                {
                    smsConfig = _mapper.Map<SmsConfig>(dbConfigs);
                    CacheUtil.SetCache(cacheKey, smsConfig, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                    return smsConfig;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("获取租户短信配置出错，错误消息：{0}", ex.Message));
            }

            return null;
        }
    }
}
