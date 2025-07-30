using AutoMapper;
using KC.Service.DTO.Dict;
using KC.DataAccess.Admin;
using KC.DataAccess.Admin.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Database.Util;
using KC.Enums.App;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Model.Admin;
using KC.Model.Component.Queue;
using KC.Service.Constants;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Admin;
using KC.Service.DTO;
using KC.Service.Component;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using KC.GitLabApiClient;
using KC.GitLabApiClient.Models.Groups.Requests;
using KC.GitLabApiClient.Models.Users.Requests;
using KC.GitLabApiClient.Models.Users.Responses;
using KC.Framework.Exceptions;

namespace KC.Service.Admin
{
    public class TenantUserService : ITenantUserService
    {
        private readonly IMapper _mapper;

        #region Repository
        private EFUnitOfWorkContextBase _unitOfContext => _serviceProvider.GetService<ComAdminUnitOfWorkContext>();
        private IDbRepository<DatabasePool> _databasePoolRepository => _serviceProvider.GetService<IDbRepository<DatabasePool>>();
        private IDbRepository<StoragePool> _storagePoolRepository => _serviceProvider.GetService<IDbRepository<StoragePool>>();
        private IDbRepository<QueuePool> _queuePoolRepository => _serviceProvider.GetService<IDbRepository<QueuePool>>();
        private IDbRepository<NoSqlPool> _noSqlPoolRepository => _serviceProvider.GetService<IDbRepository<NoSqlPool>>();
        private IDbRepository<VodPool> _vodPoolRepository => _serviceProvider.GetService<IDbRepository<VodPool>>();
        private IDbRepository<CodeRepositoryPool> _codePoolRepository => _serviceProvider.GetService<IDbRepository<CodeRepositoryPool>>();

        private IDbRepository<TenantUserOperationLog> _tenantUserOperationLogRepository => _serviceProvider.GetService<IDbRepository<TenantUserOperationLog>>();

        private ITenantUserRepository _tenantUserRepository => _serviceProvider.GetService<ITenantUserRepository>();
        private ITenantUserApplicationRepository _tenantApplicationRepository => _serviceProvider.GetService<ITenantUserApplicationRepository>();
        private ITenantUserOpenAppErrorLogRepository _tenantUserOpenAppErrorLogRepository => _serviceProvider.GetService<ITenantUserOpenAppErrorLogRepository>();
        private ITenantUserOperationLogRepository _tenantUserOperationNewLogRepository => _serviceProvider.GetService<ITenantUserOperationLogRepository>();

        private IDbRepository<TenantUserAuthentication> _tenantAuthRepository => _serviceProvider.GetService<IDbRepository<TenantUserAuthentication>>();
        #endregion

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        public TenantUserService(
            IMapper mapper,
            IHttpClientFactory clientFactory,
            IServiceProvider serviceProvider,
            ILogger<TenantUserService> logger)
        {
            _mapper = mapper;
            //AdminUnitOfWorkContext = new ComAdminUnitOfWorkContext();
            _httpClientFactory = clientFactory;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        #region Tenant

        public Tenant GetTenantByName(string tenantName)
        {
            var result = _tenantUserRepository.GetDetailTenantByName(tenantName);
            AppendTenantWithApplicationInfos(result);
            return result;
        }

        public async Task<Tenant> GetTenantByNameOrNickNameAsync(string tenantName)
        {
            var result = await _tenantUserRepository.GetTenantByNameOrNickNameAsync(tenantName);
            AppendTenantWithApplicationInfos(result);
            return result;
        }

        public async Task<Tenant> GetTenantEndWithDomainNameAsync(string domainName)
        {
            var result = await _tenantUserRepository.GetTenantEndWithDomainName(domainName);
            AppendTenantWithApplicationInfos(result);
            return result;
        }

        public List<TenantSimpleDTO> FindTenantUserByDatabasePoolId(int databaseId)
        {
            var model = _tenantUserRepository.FindAll(m => m.DatabasePoolId == databaseId);
            return _mapper.Map<List<TenantSimpleDTO>>(model);
        }
        public List<TenantSimpleDTO> FindTenantUserByStoragePoolId(int storagePoolId)
        {
            var model = _tenantUserRepository.FindAll(m => m.StoragePoolId == storagePoolId);
            return _mapper.Map<List<TenantSimpleDTO>>(model);
        }
        public List<TenantSimpleDTO> FindTenantUserByVodPoolId(int vodPoolId)
        {
            var model = _tenantUserRepository.FindAll(m => m.VodPoolId == vodPoolId);
            return _mapper.Map<List<TenantSimpleDTO>>(model);
        }
        public List<TenantSimpleDTO> FindTenantUserByCodePoolId(int codePoolId)
        {
            var model = _tenantUserRepository.FindAll(m => m.CodePoolId == codePoolId);
            return _mapper.Map<List<TenantSimpleDTO>>(model);
        }

        public bool SetNickName(int tenantId, string nickName)
        {
            var tenant = _tenantUserRepository.GetById(tenantId);
            if (tenant != null)
            {
                tenant.NickName = nickName;
                tenant.NickNameLastModifyDate = DateTime.UtcNow;
                return _tenantUserRepository.Modify(tenant);
            }

            return true;
        }

        private void ClearAboutTenantCache(string tenantName)
        {
            #region Tenant Api 缓存
            var apiTenantName = TenantConstant.DbaTenantApiAccessInfo.TenantName;
            var apiMethod1 = typeof(ITenantUserApiService).GetMethod("GetTenantByName");
            var cacheKey1 = Util.CacheKeyUtil.CacheKeyGenerator(apiTenantName, apiMethod1, new object[] { tenantName });
            if (!string.IsNullOrEmpty(cacheKey1))
                Service.CacheUtil.RemoveCache(cacheKey1);

            var apiMethod2 = typeof(ITenantUserApiService).GetMethod("GetTenantEndWithDomainName");
            var cacheKey2 = Util.CacheKeyUtil.CacheKeyGenerator(apiTenantName, apiMethod2, new object[] { tenantName });
            if (!string.IsNullOrEmpty(cacheKey2))
                Service.CacheUtil.RemoveCache(cacheKey2);
            #endregion

            #region Tenant Service 缓存
            var serviceTenantName = TenantConstant.DbaTenantApiAccessInfo.TenantName;
            var serviceMethod1 = typeof(ITenantUserService).GetMethod("GetTenantByName");
            var cacheKey3 = Util.CacheKeyUtil.CacheKeyGenerator(serviceTenantName, serviceMethod1, new object[] { tenantName });
            if (!string.IsNullOrEmpty(cacheKey3))
                Service.CacheUtil.RemoveCache(cacheKey3);

            var serviceMethod2 = typeof(ITenantUserService).GetMethod("GetTenantByNameOrNickNameAsync");
            var cacheKey4 = Util.CacheKeyUtil.CacheKeyGenerator(serviceTenantName, serviceMethod2, new object[] { tenantName });
            if (!string.IsNullOrEmpty(cacheKey4))
                Service.CacheUtil.RemoveCache(cacheKey4);

            var serviceMethod3 = typeof(ITenantUserService).GetMethod("GetTenantEndWithDomainNameAsync");
            var cacheKey5 = Util.CacheKeyUtil.CacheKeyGenerator(serviceTenantName, serviceMethod3, new object[] { tenantName });
            if (!string.IsNullOrEmpty(cacheKey5))
                Service.CacheUtil.RemoveCache(cacheKey5);
            #endregion

        }
        #endregion

        #region TenantUser

        public bool ExistTenantName(string tenantName)
        {
            return _tenantUserRepository.ExistTenantName(tenantName);
        }

        public List<TenantUserDTO> FindAllNeedInitDbTenantUsers()
        {
            Expression<Func<TenantUser, bool>> predicate =
                m => !m.IsDeleted /*&& m.Applications.Any(a => a.AppStatus == ApplicationStatus.GeneratDataBase)*/;
            var data = _tenantUserRepository.FindAll(predicate, k => k.CanEdit, false).ToList();
            return _mapper.Map<List<TenantUserDTO>>(data);
        }

        public List<TenantSimpleDTO> FindInitalTenants()
        {
            var data = _tenantUserRepository.FindAll().ToList();
            foreach (var tenant in data)
            {
                AppendTenantWithApplicationInfos(tenant);
            }

            return _mapper.Map<List<TenantSimpleDTO>>(data);
        }
        public List<TenantSimpleDTO> FindTenantsByNames(List<string> tenantNames)
        {
            var data = _tenantUserRepository.FindAllDetailTenantUsersByNames(tenantNames);
            return _mapper.Map<List<TenantSimpleDTO>>(data);
        }
        public List<TenantSimpleDTO> FindOpenAppTenantsByNames(Guid appId, List<string> tenantNames)
        {
            var data = _tenantUserRepository.FindOpenAppTenantsByNames(appId, tenantNames).ToList();
            return _mapper.Map<List<TenantSimpleDTO>>(data);
        }
        public List<TenantSimpleDTO> FindAllOpenAppTenants(Guid appId)
        {
            var data = _tenantUserRepository.FindAllTenantUsersByTally(appId, null).ToList();
            return _mapper.Map<List<TenantSimpleDTO>>(data);
        }
        private void AppendTenantWithApplicationInfos(TenantUser tenant)
        {
            if (tenant == null) return;

            tenant.Hostnames = tenant.Applications
                .Where(m => !m.DomainName.IsNullOrEmpty())
                .Select(m => m.DomainName.Replace(TenantConstant.SubDomain, tenant.TenantName)
                .ToLower()).ToArray();
            foreach (var app in tenant.Applications)
            {
                if (!tenant.Scopes.ContainsKey(app.WebSiteName))
                    tenant.Scopes.Add(app.WebSiteName, app.ApplicationName);
            }
        }

        public PaginatedBaseDTO<TenantSimpleDTO> FindTenantUsersByFilter(int pageIndex, int pageSize, int? cloudType, int? version,
            string tenantCode, string contact, string tenantName)
        {

            Expression<Func<TenantUser, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(tenantCode))
            {
                predicate = predicate.And(m => m.TenantName.Contains(tenantCode));
            }
            if (!string.IsNullOrWhiteSpace(contact))
            {
                predicate = predicate.And(m => m.ContactName.Contains(contact));
            }
            if (!string.IsNullOrWhiteSpace(tenantName))
            {
                predicate = predicate.And(m => m.TenantDisplayName.Contains(tenantName));
            }
            if (cloudType.HasValue)
            {
                predicate = predicate.And(m => m.CloudType == (CloudType)cloudType);
            }
            if (version.HasValue)
            {
                predicate = predicate.And(m => m.Version == (Framework.Tenant.TenantVersion)version);
            }
            var data = _tenantUserRepository.FindPagenatedListWithCount<TenantUser>(
                pageIndex,
                pageSize,
                predicate, "TenantId", false);

            var total = data.Item1;
            var rows = _mapper.Map<List<TenantSimpleDTO>>(data.Item2);
            return new PaginatedBaseDTO<TenantSimpleDTO>(pageIndex, pageSize, total, rows);
        }
        public PaginatedBaseDTO<TenantSimpleDTO> FindTenantUsersByOpenAppId(int pageIndex, int pageSize,
            Guid applicationId, string tenantDisplayName, List<string> exceptTenants = null, bool isShowExceptTenants = false)
        {
            var data = _tenantUserRepository.FindPagenatedTenantsByApplicationId(
                pageIndex,
                pageSize,
                applicationId,
                tenantDisplayName,
                exceptTenants,
                isShowExceptTenants);

            var total = data.Item1;
            var rows = _mapper.Map<List<TenantSimpleDTO>>(data.Item2);
            return new PaginatedBaseDTO<TenantSimpleDTO>(pageIndex, pageSize, total, rows);
        }
        public PaginatedBaseDTO<TenantSimpleDTO> FindTenantUsers(int pageIndex, int pageSize,
            string tenantDisplayName, string providerTenantName = null, TenantType? tally = null)
        {
            Expression<Func<TenantUser, bool>> predicate =
                o => !o.IsDeleted &&
                        o.Applications.Any(c => c.ApplicationId == ApplicationConstant.JrAppId);
            if (!string.IsNullOrWhiteSpace(tenantDisplayName))
            {
                predicate = predicate.And(m => m.TenantDisplayName.Contains(tenantDisplayName));
            }
            if (!string.IsNullOrWhiteSpace(providerTenantName))
            {
                predicate = predicate.And(m => m.TenantName != providerTenantName);
            }
            if (tally != null)
            {
                predicate = predicate.And(m => m.TenantType == tally);
            }
            var data = _tenantUserRepository.FindPagenatedListWithCount<TenantUser>(
                pageIndex,
                pageSize,
                predicate, "TenantId", false);
            var total = data.Item1;
            var rows = _mapper.Map<List<TenantSimpleDTO>>(data.Item2);
            return new PaginatedBaseDTO<TenantSimpleDTO>(pageIndex, pageSize, total, rows);
        }

        public TenantUserDTO GetTenantUserById(int id)
        {
            var data = _tenantUserRepository.GetById(id);
            return _mapper.Map<TenantUserDTO>(data);
        }
        public TenantUserDTO GetTenantUserByName(string tenantName)
        {
            var model = _tenantUserRepository.GetDetailTenantByName(tenantName);
            var data = _mapper.Map<TenantUserDTO>(model);
            if (data != null)
                data.Applications = _mapper.Map<List<TenantUserApplicationDTO>>(model.Applications);
            return data;
        }


        public async Task<bool> SaveTenantUser(TenantUserDTO model)
        {
            var data = _mapper.Map<TenantUser>(model);
            if (string.IsNullOrEmpty(data.PrivateEncryptKey))
            {
                data.PrivateEncryptKey = EncryptPasswordUtil.GetRandomString();
            }

            //独立数据库
            var isCreated = model.TenantId == 0;
            var isProfessional = model.Version == Framework.Tenant.TenantVersion.Professional;

            #region 租户资源分配
            var database = await _databasePoolRepository.GetByIdAsync(model.DatabasePoolId);
            var pwd = EncryptPasswordUtil.GetRandomString();//生成数据库Tenant用户的登录密码
            if (null != database)
            {
                data.Server = database.Server;
                if (!data.TenantName.Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase))
                    data.Database = isProfessional ? model.TenantName : database.Database;
                var connestring = database.ConnectionString;
                if (string.IsNullOrEmpty(data.DatabasePasswordHash))
                {
                    data.DatabasePasswordHash = EncryptPasswordUtil.EncryptPassword(pwd, data.PrivateEncryptKey);
                }
                else
                {
                    pwd = EncryptPasswordUtil.DecryptPassword(data.DatabasePasswordHash, data.PrivateEncryptKey);
                }

                //更新资源消耗的次数：TenantCount
                if (isCreated)
                {
                    if (!isProfessional)
                    {
                        database.TenantCount++;
                        await _databasePoolRepository.ModifyAsync(database, new[] { "TenantCount" }, false);
                    }
                    else
                    {
                        var newDatabase = new DatabasePool()
                        {
                            CloudType = database.CloudType,
                            DatabaseType = database.DatabaseType,
                            Server = database.Server,
                            Database = model.TenantName,
                            UserName = database.UserName,
                            UserPasswordHash = database.UserPasswordHash,
                            TenantCount = 1,
                            CanEdit = database.CanEdit,
                        };
                        //_databasePoolRepository.Add(newDatabase, false);

                        data.DatabasePoolId = 0;
                        data.OwnDatabase = newDatabase;

                        connestring = newDatabase.ConnectionString;
                    }
                    //创建租户的数据库及数据库登录用户
                    var createDataBase = DbLoginUserUtil.CreateTenantDbLoginUser(data, connestring);
                    _logger.LogInformation(string.Format("创建Tenant【{0}】的数据库【{1}--{2}】登录用户是否成功？{3}",
                        model.TenantName, data.Server, data.Database, createDataBase));
                }
            }

            var storage = await _storagePoolRepository.GetByIdAsync(model.StoragePoolId);
            var storageDeString = string.Empty;
            if (null != storage)
            {
                data.StorageEndpoint = storage.Endpoint;
                data.StorageAccessName = storage.AccessName;

                storageDeString = EncryptPasswordUtil.DecryptPassword(storage.AccessKeyPasswordHash);
                data.StorageAccessKeyPasswordHash = EncryptPasswordUtil.EncryptPassword(storageDeString, data.PrivateEncryptKey);
                //更新资源消耗的次数：TenantCount
                if (isCreated)
                {
                    if (!isProfessional)
                    {
                        storage.TenantCount++;
                        await _storagePoolRepository.ModifyAsync(storage, new[] { "TenantCount" }, false);
                    }
                }
            }

            var queue = await _queuePoolRepository.GetByIdAsync(model.QueuePoolId);
            if (null != queue)
            {
                data.QueueEndpoint = queue.Endpoint;
                data.QueueAccessName = queue.AccessName;

                var deString = EncryptPasswordUtil.DecryptPassword(queue.AccessKeyPasswordHash);
                data.QueueAccessKeyPasswordHash = EncryptPasswordUtil.EncryptPassword(deString, data.PrivateEncryptKey);
                //更新资源消耗的次数：TenantCount
                if (isCreated)
                {
                    if (!isProfessional)
                    {
                        queue.TenantCount++;
                        await _queuePoolRepository.ModifyAsync(queue, new[] { "TenantCount" }, false);
                    }
                }
            }

            var nosql = await _noSqlPoolRepository.GetByIdAsync(model.NoSqlPoolId);
            if (null != nosql)
            {
                data.NoSqlEndpoint = nosql.Endpoint;
                data.NoSqlAccessName = nosql.AccessName;

                var deString = EncryptPasswordUtil.DecryptPassword(nosql.AccessKeyPasswordHash);
                data.NoSqlAccessKeyPasswordHash = EncryptPasswordUtil.EncryptPassword(deString, data.PrivateEncryptKey);
                //更新资源消耗的次数：TenantCount
                if (!isProfessional)
                {
                    if (isCreated)
                    {
                        nosql.TenantCount++;
                        await _noSqlPoolRepository.ModifyAsync(nosql, new[] { "TenantCount" }, false);
                    }
                }
            }

            var vod = await _vodPoolRepository.GetByIdAsync(model.VodPoolId);
            if (null != vod)
            {
                data.VodEndpoint = vod.Endpoint;
                data.VodAccessName = vod.AccessName;

                var deString = EncryptPasswordUtil.DecryptPassword(vod.AccessKeyPasswordHash);
                data.NoSqlAccessKeyPasswordHash = EncryptPasswordUtil.EncryptPassword(deString, data.PrivateEncryptKey);
                //更新资源消耗的次数：TenantCount
                if (!isProfessional)
                {
                    if (isCreated)
                    {
                        vod.TenantCount++;
                        await _vodPoolRepository.ModifyAsync(vod, new[] { "TenantCount" }, false);
                    }
                }
            }

            var code = await _codePoolRepository.GetByIdAsync(model.CodePoolId);
            if (null != code)
            {
                data.CodeEndpoint = code.Endpoint;
                data.CodeAccessName = code.AccessName;

                var deString = EncryptPasswordUtil.DecryptPassword(code.AccessKeyPasswordHash);
                data.NoSqlAccessKeyPasswordHash = EncryptPasswordUtil.EncryptPassword(deString, data.PrivateEncryptKey);
                //更新资源消耗的次数：TenantCount
                if (!isProfessional)
                {
                    if (isCreated)
                    {
                        code.TenantCount++;
                        await _codePoolRepository.ModifyAsync(code, new[] { "TenantCount" }, false);
                    }
                }
            }

            _logger.LogDebug(string.Format("SaveTenantUser------Tenant[{0}] Database[id={1}, Server={2}] Password: {3}; " +
                                           "Stroage[id={4}, Endpoint={5}] SecurityKey: {6}; " +
                                           "Queue[id={7}, Endpoint={8}]; NoSql[id={9}, Endpoint={10}];" +
                                           "Vod[id={11}, Endpoint={12}]; Code[id={13}, Endpoint={14}];",
                model.TenantName, database != null ? database.DatabasePoolId : 1,
                database != null ? database.Server : string.Empty, pwd,
                storage != null ? storage.StoragePoolId : 1, storage != null ? storage.Endpoint : string.Empty, storageDeString,
                queue != null ? queue.QueuePoolId : 1, queue != null ? queue.Endpoint : string.Empty,
                nosql != null ? nosql.NoSqlPoolId : 1, nosql != null ? nosql.Endpoint : string.Empty,
                vod != null ? vod.VodPoolId : 1, vod != null ? vod.Endpoint : string.Empty,
                code != null ? code.CodePoolId : 1, code != null ? code.Endpoint : string.Empty));
            #endregion

            if (isCreated)
            {
                await _tenantUserRepository.AddAsync(data, false);
                var success = await _unitOfContext.CommitAsync() > 0;
                if (success)
                {
                    var defaultSendTo = string.IsNullOrWhiteSpace(GlobalConfig.AdminEmails)
                        ? "tianchangjun@outlook.com"
                        : GlobalConfig.AdminEmails;
                    var sendTo = !string.IsNullOrEmpty(data.ContactEmail)
                        ? data.ContactEmail
                        : defaultSendTo;
                    var emailTitle = string.IsNullOrEmpty(data.ContactEmail)
                        ? "[租户开通消息]租户通知邮箱为空，请手动发送该邮件"
                        : "[系统消息]租户开通";
                    var adminUserNewPwd = EncryptPasswordUtil.GetRandomString();
                    var emailBody = GetTenantOpenAppsEmailBody(data, adminUserNewPwd);

#if DEBUG //业务逻辑同：KC.WorkerRole.Jobs.OpenTenantAppsJob
                    var watch = new Stopwatch();
                    watch.Start();
                    var message = UpgradeTenantDatabase(data.TenantId);
                    watch.Stop();
                    _logger.LogInformation(
                        string.Format("SaveTenantUser----Thread Id={0} end to initialize the tenant: [{1}--{2}] Database by time: {3}. ",
                            Thread.CurrentThread.ManagedThreadId, data.TenantName, data.TenantDisplayName,
                            watch.ElapsedMilliseconds));

                    #region 修改租户的系统管理员的初始密码

                    //修改租户的系统管理员所默认密码：123456
                    var tenant = GetTenantByName(data.TenantName);
                    var changeResult = new AccountApiService(tenant, _httpClientFactory, null).ChangeAdminRawInfo(
                        adminUserNewPwd, data.ContactEmail, data.ContactPhone).Result;
                    var changePwsSuccess = changeResult.success && changeResult.Result;
                    if (!changePwsSuccess)
                    {
                        AddOpenAppErrorLog(data.TenantId, data.TenantDisplayName,
                            OpenServerType.ChangeAdminUserRawInfo,
                            string.Format(
                                "修改管理员初始密码【{0}】为【{1}】失败，但是开通邮件已经发送，不影响租户的使用，该租户的系统管理员将使用初始密码【{0}】进行登录，接口返回消息：{2}", "123456",
                                adminUserNewPwd, changeResult.message));
                    }
                    #endregion

                    #region 创建用户的Gitlab分组及管理员账号
                    var gitlabTenantAdminPwd = changePwsSuccess ? adminUserNewPwd : "123456";
                    var resultMessage = await CreateTenantGitlabGroupAndUser(data.TenantName, gitlabTenantAdminPwd);
                    if (!resultMessage.IsNullOrEmpty())
                    {
                        AddOpenAppErrorLog(data.TenantId, data.TenantDisplayName,
                            OpenServerType.OpenGitlab,
                            string.Format(
                                "创建租户的Gitlab分组及管理员账号【密码：{0}】失败，接口返回消息：{1}",
                                gitlabTenantAdminPwd, resultMessage));
                    }
                    #endregion

                    if (string.IsNullOrEmpty(message))
                    {
                        _logger.LogInformation(
                                string.Format(
                                    "SaveTenantUser----end to open the tenant: [{0}--{1}] apps. changePwsSuccess={2}. createGitlabAccount={3}.",
                                    data.TenantName, data.TenantDisplayName, changePwsSuccess, resultMessage.IsNullOrEmpty()));
                        return true;
                    }
                    else
                    {
                        //_tenantService.UpgradeTenantDatabase方法中已经添加了日志记录：AddOpenAppErrorLog
                        //_logger.LogError(message);
                        //EmailUtil.SendAdministratorMail(string.Format("开通租户（{0}--{1}）的数据库失败消息", data.TenantId, data.TenantDisplayName), message);
                        return true;
                    }
#else
                    var openTenantApplications = new TenantApplications()
                    {
                        TenantId = data.TenantId,
                        TenantName = data.TenantName,
                        TenantDisplayName = data.TenantDisplayName,
                        Email = sendTo,
                        EmailTitle = emailTitle,
                        EmailContent = emailBody,
                        AdminNewPassword = adminUserNewPwd,
                        AdminEmail = data.ContactEmail,
                        AdminPhone = data.ContactPhone,
                        AppIds = data.Applications.Select(m => m.Id).ToList()
                    };
                    return new StorageQueueService().InsertTenantApplicationsQueue(openTenantApplications);
#endif

                }

                return true;
            }
            else
            {
                #region 修改Tenant信息
                var modifiedFields = new List<string>()
                {
                    "TenantDisplayName",
                    "CloudType",
                    "PrivateEncryptKey",
                    "Server",
                    "Database",
                    "Endpoint",
                    "AccessName",
                    "AccessKeyPasswordHash",
                    "QueueEndpoint",
                    "QueueAccessName",
                    "QueueAccessKeyPasswordHash",
                    "NoSqlEndpoint",
                    "NoSqlAccessName",
                    "NoSqlAccessKeyPasswordHash",
                    "ContactName",
                    "ContactEmail",
                    "ContactPhone",
                    "PasswordExpiredTime",
                    "DatabasePoolId",
                    "StoragePoolId",
                    "QueuePoolId",
                    "NoSqlPoolId",
                    "TenantType",
                    "Version",
                    "Bank"
                };
                var success = _tenantUserRepository.Modify(data, modifiedFields.ToArray());
                #endregion

                if (success)
                {
                    ClearAboutTenantCache(data.TenantName.ToLower());
                }
                return success;
            }
        }
        public bool SoftRemoveTenantUser(List<int> ids)
        {
            return _tenantUserRepository.SoftRemove(m => ids.Contains(m.TenantId), true) > 0;
        }

        public bool SoftRemoveTenantUserByNames(List<string> tenantNames)
        {
            return _tenantUserRepository.SoftRemove(m => tenantNames.Contains(m.TenantName), true) > 0;
        }
        public bool AddTenantUserApplications(int tenantId, List<Guid> applicationIds)
        {
            //using (var scope = ServiceProvider.CreateScope())
            {
                //var _unitOfContext = scope.ServiceProvider.GetRequiredService<ComAdminUnitOfWorkContext>();

                var dbaTenant = TenantConstant.DbaTenantApiAccessInfo;
                //var applicationRepository = new AspNetApplicationRespository(dbaTenant);
                //var sysApplication = applicationRepository.GetDetailApplicationsByIds(applicationIds);

                //给Tenant开通应用及其模块（AspNetApplication）
                var tenant = _tenantUserRepository.GetById(tenantId);
                //applicationRepository = new AspNetApplicationRespository(tenant);
                //sysApplication.ForEach(m => m.DomainName = m.DomainName.Replace(TenantConstant.SubDomain, tenant.TenantName));
                //applicationRepository.Add(sysApplication);

                //记录每个Tenant的开通应用的列表（给SuperAdmin使用）
                //var tenantApp = new TenantUserApplicationMapper().Map(sysApplication).ToList();
                var tenantApp = new List<TenantUserApplication>();
                tenantApp.ForEach(m =>
                {
                    m.TenantId = tenantId;
                    //_tenantApplicationRepository.Add(m, false);
                    var log = new TenantUserOperationLog
                    {
                        ApplicationId = m.ApplicationId,
                        ApplicationName = m.ApplicationName,
                        DomainName = m.DomainName,
                        AppStatus = ApplicationStatus.Initial,
                        TenantName = tenant.TenantName,
                        TenantDisplayName = tenant.TenantDisplayName,
                        LogType = OperationLogType.OpenApplication,
                        Remark = string.Format("初始化应用状态操作，将应用（{0}）状态设置为：{1}。",
                                m.ApplicationName, ApplicationStatus.Initial.ToDescription()),
                        TenantUserApplication = m,
                    };
                    _tenantUserOperationLogRepository.Add(log, false);
                });

                return _unitOfContext.Commit() > 0;
            }
        }

        public bool UpdateTenantUserBasicInfo(TenantSimpleDTO model)
        {
            if (string.IsNullOrEmpty(model.TenantName))
                throw new ArgumentNullException("TenantName", "租户代码为空，无法查询到租户信息");

            var tenantUser = _tenantUserRepository.GetByFilter(m => m.TenantName.Equals(model.TenantName));
            tenantUser.TenantDisplayName = string.IsNullOrEmpty(model.TenantDisplayName) ? tenantUser.TenantDisplayName : model.TenantDisplayName;
            tenantUser.ContactName = string.IsNullOrEmpty(model.ContactName) ? tenantUser.ContactName : model.ContactName;
            tenantUser.ContactEmail = string.IsNullOrEmpty(model.ContactEmail) ? tenantUser.ContactEmail : model.ContactEmail;
            tenantUser.ContactPhone = string.IsNullOrEmpty(model.ContactPhone) ? tenantUser.ContactPhone : model.ContactPhone;
            tenantUser.IndustryId = string.IsNullOrEmpty(model.IndustryId) ? tenantUser.IndustryId : model.IndustryId;
            tenantUser.IndustryName = string.IsNullOrEmpty(model.IndustryName) ? tenantUser.IndustryName : model.IndustryName;
            tenantUser.BusinessModel = model.BusinessModel == BusinessModel.ProductionFoundry ? tenantUser.BusinessModel : model.BusinessModel;

            var success = _tenantUserRepository.Modify(tenantUser, new[] { "TenantDisplayName", "ContactName", "ContactPhone", "ContactEmail", "BusinessModel", "IndustryId", "IndustryName" });

            if (success)
            {
                ClearAboutTenantCache(model.TenantName);
            }
            return success;
        }

        public bool SaveTenantUserAuthInfo(TenantUserAuthenticationDTO model)
        {
            if (string.IsNullOrEmpty(model.CompanyCode))
                throw new ArgumentNullException("CompanyCode", "租户代码为空，无法查询到租户信息");

            var tenantUser = _tenantAuthRepository.GetByFilter(m => m.TenantName.Equals(model.CompanyCode));
            if (tenantUser != null)
            {
                tenantUser.ZipCode = string.IsNullOrEmpty(model.ZipCode) ? tenantUser.ZipCode : model.ZipCode;
                tenantUser.ProvinceId = model.ProvinceId.HasValue ? model.ProvinceId.Value : tenantUser.ProvinceId;
                tenantUser.ProvinceName = string.IsNullOrEmpty(model.ProvinceName) ? tenantUser.ProvinceName : model.ProvinceName;
                tenantUser.CityId = model.CityId.HasValue ? model.CityId.Value : tenantUser.CityId;
                tenantUser.CityName = string.IsNullOrEmpty(model.CityName) ? tenantUser.CityName : model.CityName;
                tenantUser.DistrictId = model.DistrictId.HasValue ? model.DistrictId.Value : tenantUser.DistrictId;
                tenantUser.DistrictName = string.IsNullOrEmpty(model.DistrictName) ? tenantUser.DistrictName : model.DistrictName;
                tenantUser.CompanyAddress = string.IsNullOrEmpty(model.CompanyAddress) ? tenantUser.CompanyAddress : model.CompanyAddress;
                tenantUser.BulidDate = model.BulidDate.HasValue ? model.BulidDate.Value : tenantUser.BulidDate;
                tenantUser.BusinessDateLimit = model.BusinessDateLimit.HasValue ? model.BusinessDateLimit.Value : tenantUser.BusinessDateLimit;
                tenantUser.RegisteredCapital = model.RegisteredCapital != 0 ? model.RegisteredCapital : tenantUser.RegisteredCapital;
                tenantUser.ScopeOfBusiness = string.IsNullOrEmpty(model.ScopeOfBusiness) ? tenantUser.ScopeOfBusiness : model.ScopeOfBusiness;
                tenantUser.UnifiedSocialCreditCode = string.IsNullOrEmpty(model.UnifiedSocialCreditCode) ? tenantUser.UnifiedSocialCreditCode : model.UnifiedSocialCreditCode;
                tenantUser.UnifiedSocialCreditCodeScannPhoto = string.IsNullOrEmpty(model.UnifiedSocialCreditCodeScannPhoto) ? tenantUser.UnifiedSocialCreditCodeScannPhoto : model.UnifiedSocialCreditCodeScannPhoto;

                tenantUser.LegalPerson = string.IsNullOrEmpty(model.LegalPerson) ? tenantUser.LegalPerson : model.LegalPerson;
                tenantUser.LegalPersonScannPhoto = string.IsNullOrEmpty(model.LegalPersonScannPhoto) ? tenantUser.LegalPersonScannPhoto : model.LegalPersonScannPhoto;
                tenantUser.LegalPersonIdentityCardNumber = string.IsNullOrEmpty(model.LegalPersonIdentityCardNumber) ? tenantUser.LegalPersonIdentityCardNumber : model.LegalPersonIdentityCardNumber;
                tenantUser.LegalPersonIdentityCardPhoto = string.IsNullOrEmpty(model.LegalPersonIdentityCardPhoto) ? tenantUser.LegalPersonIdentityCardPhoto : model.LegalPersonIdentityCardPhoto;
                tenantUser.LegalPersonIdentityCardPhotoOtherSide = string.IsNullOrEmpty(model.LegalPersonIdentityCardPhotoOtherSide) ? tenantUser.LegalPersonIdentityCardPhotoOtherSide : model.LegalPersonIdentityCardPhotoOtherSide;
                tenantUser.LegalPersonCertificateExpiryDate = model.LegalPersonCertificateExpiryDate.HasValue ? model.LegalPersonCertificateExpiryDate.Value : tenantUser.LegalPersonCertificateExpiryDate;

                tenantUser.LetterOfAuthorityScannPhoto = string.IsNullOrEmpty(model.LetterOfAuthorityScannPhoto) ? tenantUser.LetterOfAuthorityScannPhoto : model.LetterOfAuthorityScannPhoto;
                tenantUser.EnterpriseQualificationPhoto = string.IsNullOrEmpty(model.EnterpriseQualificationPhoto) ? tenantUser.EnterpriseQualificationPhoto : model.EnterpriseQualificationPhoto;
                tenantUser.AuditComment = string.IsNullOrEmpty(model.AuditComment) ? tenantUser.AuditComment : model.AuditComment;

                return _tenantAuthRepository.Modify(tenantUser, new[] { "ZipCode", "ProvinceId", "ProvinceName", "CityId", "CityName", "DistrictId", "DistrictName", "CompanyAddress", "BulidDate", "BusinessDateLimit", "IsLongTerm", "RegisteredCapital", "ScopeOfBusiness", "UnifiedSocialCreditCode", "UnifiedSocialCreditCodeScannPhoto", "LegalPerson", "LegalPersonScannPhoto", "LegalPersonIdentityCardNumber", "LegalPersonIdentityCardPhoto", "LegalPersonIdentityCardPhotoOtherSide", "LegalPersonCertificateExpiryDate", "LetterOfAuthorityScannPhoto", "EnterpriseQualificationPhoto", "AuditComment" });
            }

            var data = _mapper.Map<TenantUserAuthentication>(model);
            return _tenantAuthRepository.Add(data);
        }

        #endregion

        #region Tenant's Application && Init Tenant's Database & Web Server

        public TenantUserApplicationDTO GetDetailApplicationWithModelsByTenantIdAndAppId(int tenantId, Guid appId)
        {
            Expression<Func<TenantUserApplication, bool>> predicate = m => m.TenantId == tenantId && m.ApplicationId == appId;
            var model = _tenantApplicationRepository.GetWithModulesByFilter(predicate);
            return _mapper.Map<TenantUserApplicationDTO>(model);
        }

        public TenantUserApplicationDTO GetTenantApplicationByFilter(int tenantId, Guid appId)
        {
            Expression<Func<TenantUserApplication, bool>> predicate = m => m.TenantId == tenantId && m.ApplicationId == appId;
            var model = _tenantApplicationRepository.GetTenantApplicationByFilter(predicate);
            return _mapper.Map<TenantUserApplicationDTO>(model);
        }

        public List<TenantUserApplicationDTO> FindTenantApplicationsByTenantName(string tenantName)
        {
            Expression<Func<TenantUserApplication, bool>> predicate
                = m => m.TenantUser != null &&
                       m.TenantUser.TenantName.Equals(tenantName, StringComparison.OrdinalIgnoreCase);

            var model = _tenantApplicationRepository.GetTenantApplicationsByFilter(predicate, m => m.ApplicationName);
            return _mapper.Map<List<TenantUserApplicationDTO>>(model);
        }

        public List<TenantUserApplicationDTO> FindTenantApplicationsByTenantId(int tenantId)
        {
            var model = _tenantApplicationRepository.FindAll(m => m.TenantId == tenantId);
            return _mapper.Map<List<TenantUserApplicationDTO>>(model);
        }

        public List<Guid> FindTenantApplicationIdsByTenantId(int tenantId)
        {
            return _tenantApplicationRepository.FindAll(m => m.TenantId == tenantId).Select(m => m.ApplicationId).ToList();
        }

        /// <summary>
        /// 更新所有租户的应用状态为：ApplicationStatus.GeneratDataBase </br>
        /// 以便:KC.WorkerRole.Jobs.InitTenantDatabaseJob开始初始化数据库
        /// </summary>
        /// <returns></returns>
        public string SetInitTenantsDbJobAppsStatus()
        {
            var error = string.Empty;
            var count = 0;
            while (count <= 3)//失败后再执行三次
            {
                try
                {
                    var sql = string.Format("UPDATE [dba].[tenant_TenantUserApplication] set [AppStatus]={0}",
                        (int)ApplicationStatus.GeneratDataBase);

                    var success = _tenantApplicationRepository.ExecuteSql(sql);
                    if (success) return string.Empty;

                    error = string.Format("初始化所有租户的应用状态为：{0} 失败，执行脚本失败：{1}",
                        ApplicationStatus.GeneratDataBase.ToDescription(), sql);
                    count++;
                }
                catch (Exception ex)
                {
                    var message = string.Format("初始化所有租户的应用状态为：{0} 失败，失败消息：{1}",
                        ApplicationStatus.GeneratDataBase.ToDescription(), ex.Message);
                    _logger.LogError(message, ex.StackTrace);
                    count++;
                    error = message;
                }
            }

            return error;
        }

        #region Init Tenant's Database & Web Server

        public bool GeneratDataBaseAndOpenWebServer(int tenantId, string appId)
        {
            var tenant = _tenantUserRepository.GetDetailTenantById(tenantId);
            if (null == tenant)
                return false;
            var app =
                tenant.Applications.FirstOrDefault(
                    m => m.ApplicationId.ToString().Equals(appId, StringComparison.OrdinalIgnoreCase));
            if (app == null)
                return true;

            if (tenant.OwnDatabase == null)
            {
                var message = string.Format("租户[{0}--{1}]，未设置所归属的数据服务器（OwnDatabase）。",
                            tenant.TenantName, tenant.TenantDisplayName);
                _logger.LogError(message);
                return false;
            }

            //必须获取每个Tenant所归属的OwnDatabase对象，以便获取Sa的登录用户权限进行创建Tenant的数据库LoginUser用户
            var connestring = KC.Framework.Base.GlobalConfig.GetDecryptDatabaseConnectionString();
            if (tenant.OwnDatabase != null
                && !string.IsNullOrWhiteSpace(tenant.OwnDatabase.ConnectionString))
            {
                connestring = tenant.OwnDatabase.ConnectionString;
            }
            var createDataBase = DbLoginUserUtil.CreateTenantDbLoginUser(tenant, connestring);
            if (!createDataBase)
            {
                var errormsg = string.Format("租户[{0}--{1}]，创建所属数据库的登录用户出错。", tenant.TenantName, tenant.TenantDisplayName);
                AddOpenAppErrorLog(tenantId, tenant.TenantDisplayName, OpenServerType.CreateDbUser, errormsg);
                //EmailUtil.SendAdministratorMail(
                //    string.Format("租户[{0}--{1}]，创建数据库登录用户出错", tenant.TenantName, tenant.TenantDisplayName), errormsg);
                return false;
            }

            string msg;
            var errors = new List<string>();
            switch (app.AppStatus)
            {
                case ApplicationStatus.Initial:
                    msg = DatabaseRollBack(app.AssemblyName, tenant);
                    if (!string.IsNullOrEmpty(msg))
                        errors.Add(msg);
                    //var modules = app.ApplicationModules.ToList();
                    //foreach (var module in modules)
                    //{
                    //    msg = DatabaseInitialize(module.AssemblyName, tenant);
                    //    if (!string.IsNullOrEmpty(msg))
                    //        errors.Add(msg);
                    //}

                    if (!errors.Any())
                    {
                        //2017-09-27 v2.0.0.6 版本以后，使用windwos server 2016的泛域名解析，不再需要添加Web站点的IIS的HostHeader了。
                        var modifyAppStatus = ModifyTenantUserApplication(tenant.TenantName, tenant.TenantDisplayName, app, ApplicationStatus.OpenSuccess);
                        //if (modifyAppStatus)
                        //{
                        //    //OpenWebServer
                        //    msg = OpenWebServer(tenant, app);
                        //    if (!string.IsNullOrEmpty(msg))
                        //    {
                        //        errors.Add(msg);
                        //        AddOpenAppErrorLog(tenantId, tenant.TenantDisplayName, OpenServerType.OpenWebServer, msg);
                        //    }
                        //}
                    }
                    else
                    {
                        AddOpenAppErrorLog(tenantId, tenant.TenantDisplayName, OpenServerType.OpenDatabase,
                            string.Join(". " + Environment.NewLine, errors));
                    }

                    break;
                case ApplicationStatus.GeneratDataBase:
                //2017-09-27 v2.0.0.6 版本以后，使用windwos server 2016的泛域名解析，不再需要添加Web站点的IIS的HostHeader了。     
                //OpenWebServer
                //msg = OpenWebServer(tenant, app);
                //if (!string.IsNullOrEmpty(msg))
                //{
                //    errors.Add(msg);
                //    AddOpenAppErrorLog(tenantId, tenant.TenantDisplayName, OpenServerType.OpenWebServer, msg);
                //}
                //break;
                case ApplicationStatus.OpenWebServer:
                    var modifyAppStatus1 = ModifyTenantUserApplication(tenant.TenantName, tenant.TenantDisplayName, app, ApplicationStatus.OpenSuccess);
                    if (!modifyAppStatus1)
                    {
                        msg = string.Format("更改租户[{0}--{1}]应用[{2}]的为状态（ApplicationStatus.OpenSuccess）失败。", tenant.TenantName, tenant.TenantDisplayName, app.ApplicationName);
                        errors.Add(msg);
                        AddOpenAppErrorLog(tenantId, tenant.TenantDisplayName, OpenServerType.UpdateStatus, msg);
                    }
                    break;
            }

            if (errors.Any())
            {
                string message = string.Join(". " + Environment.NewLine, errors);
                //EmailUtil.SendAdministratorMail(
                //    string.Format("租户[{0}--{1}]开通应用失败", tenant.TenantName, tenant.TenantDisplayName), message);
            }

            return true;
        }
        public string RollBackTenantDataBase(int tenantId, string appId)
        {
            var tenant = _tenantUserRepository.GetDetailTenantById(tenantId);
            if (null == tenant)
                return string.Empty;
            var app =
                tenant.Applications.FirstOrDefault(
                    m => m.ApplicationId.ToString().Equals(appId, StringComparison.OrdinalIgnoreCase));
            if (app == null)
                return string.Empty;

            string msg;
            var errors = new List<string>();
            switch (app.AppStatus)
            {
                case ApplicationStatus.GeneratDataBase:
                    msg = DatabaseRollBack(app.AssemblyName, tenant);
                    if (!string.IsNullOrEmpty(msg))
                        errors.Add(msg);
                    //var modules = app.ApplicationModules.ToList();
                    //foreach (var module in modules)
                    //{
                    //    msg = DatabaseRollBack(module.AssemblyName, tenant);
                    //    if (!string.IsNullOrEmpty(msg))
                    //        errors.Add(msg);
                    //}
                    break;
            }

            if (errors.Any())
            {
                string message = string.Join(". " + Environment.NewLine, errors);
                //EmailUtil.SendAdministratorMail(
                //    string.Format("Tenant(({0}--{1}))开通应用. ", tenant.TenantName, tenant.TenantDisplayName), message);

                return message;
            }

            return string.Empty;
        }

        public List<string> UpgradeTenantDatabaseByIds(List<int> ids)
        {
            var result = new List<string>();
            var userTenants = _tenantUserRepository.FindOpenAppsTenantUsersByIds(ids).ToList();
            foreach (var tenant in userTenants.ToList())
            {
                var message = UpgradeTenantDatabase(tenant, true, false);
                if (!string.IsNullOrEmpty(message))
                    result.Add(message);
            }

            return result;
        }
        public List<string> UpgradeAllTenantDatabase()
        {
            var result = new List<string>();
            var userTenants = _tenantUserRepository.FindAllDetailTenantUsers();
            foreach (var tenant in userTenants.Where(m => m.TenantId >= 0).ToList())
            {
                var message = UpgradeTenantDatabase(tenant, false, false);
                if (!string.IsNullOrEmpty(message))
                    result.Add(message);
            }

            return result;
        }
        public List<string> RollBackAllTenantDatabase()
        {
            var result = new List<string>();
            var userTenants = _tenantUserRepository.FindAllDetailTenantUsers();
            foreach (var tenant in userTenants.ToList())
            {
                var message = RollBackTenantDataBase(tenant);
                if (!string.IsNullOrEmpty(message))
                    result.Add(message);
            }
            return result;
        }

        public string UpgradeTenantDatabase(int tenantId, bool isUpdateStatus = true, bool isNewDatabase = true)
        {
            var tenant = _tenantUserRepository.GetDetailTenantById(tenantId);
            if (null == tenant)
                return string.Empty;

            return UpgradeTenantDatabase(tenant, isUpdateStatus, isNewDatabase);
        }
        public string RollBackTenantDataBase(int tenantId)
        {
            var tenant = _tenantUserRepository.GetDetailTenantById(tenantId);
            if (null == tenant)
                return string.Empty;

            return RollBackTenantDataBase(tenant);
        }
        /// <summary>
        /// 更新租户数据库，传入参数tenant必须包含以下子对象：OwnDatabase、Applications（及其子对象：ApplicationModules）
        /// </summary>
        /// <param name="tenant">
        ///     租户对象，包含以下子对象：OwnDatabase、Applications（及其子对象：ApplicationModules）
        /// </param>
        /// <param name="isUpdateStatus">是否更新应用状态（ApplicationStatus）</param>
        /// <param name="isNewDatabase">是否为第一次初始化数据库</param>
        /// <returns></returns>
        private string UpgradeTenantDatabase(TenantUser tenant, bool isUpdateStatus = true, bool isNewDatabase = true)
        {
            try
            {
                var connestring = KC.Framework.Base.GlobalConfig.GetDecryptDatabaseConnectionString();
                if (tenant.OwnDatabase == null)
                {
                    var message = string.Format("租户[{0}--{1}]，未设置所归属的数据服务器（OwnDatabase）。",
                        tenant.TenantName, tenant.TenantDisplayName);
                    _logger.LogError(message);
                    return message;
                }

                //必须获取每个Tenant所归属的OwnDatabase对象，以便获取Sa的登录用户权限进行创建Tenant的数据库LoginUser用户
                if (tenant.OwnDatabase != null
                    && !string.IsNullOrWhiteSpace(tenant.OwnDatabase.ConnectionString))
                {
                    connestring = tenant.OwnDatabase.ConnectionString;
                }
                if (isNewDatabase)
                {
                    var createDataBase = DbLoginUserUtil.CreateTenantDbLoginUser(tenant, connestring);
                    if (!createDataBase)
                    {
                        var errormsg = string.Format("租户[{0}--{1}]，创建所属数据库的登录用户出错。", tenant.TenantName, tenant.TenantDisplayName);
                        AddOpenAppErrorLog(tenant.TenantId, tenant.TenantDisplayName, OpenServerType.CreateDbUser, errormsg);

                        return errormsg;
                    }
                }

                //_logger.LogDebug(
                //    string.Format(
                //        "UpgradeTenantDatabase----Tenant[{0}] start to initilize Applications[{1}] with Modules: " + Environment.NewLine +
                //        "{2}.",
                //        tenant.TenantName, tenant.Applications.Select(m => m.ApplicationName).ToCommaSeparatedString(),
                //        (tenant.Applications.Any()
                //            ? tenant.Applications.SelectMany(m => m.ApplicationModules)
                //                .Select(m => m.AssemblyName)
                //                .ToCommaSeparatedString()
                //            : string.Empty)));
                var sbError = new StringBuilder();
                foreach (var app in tenant.Applications.ToList())
                {
                    //var modules = app.ApplicationModules.ToList();
                    //if (!modules.Any())
                    //{
                    //    //应用无任何模块（比如：WebApi），只需更改应用状态为：ApplicationStatus.OpenSuccess，无需写日志
                    //    if (app.AppStatus != ApplicationStatus.OpenSuccess)
                    //        ModifyTenantUserApplication(tenant.TenantName, tenant.TenantDisplayName, app, ApplicationStatus.OpenSuccess, false);
                    //    continue;
                    //}

                    //开始更新数据库前，设置应用状态为：ApplicationStatus.GeneratDataBase
                    if (isUpdateStatus && app.AppStatus != ApplicationStatus.GeneratDataBase)
                        ModifyTenantUserApplication(tenant.TenantName, tenant.TenantDisplayName, app, ApplicationStatus.GeneratDataBase, false);
                    var sbMsg = new StringBuilder();
                    //更新数据库时，先判断程序集是否有更新，无更新的话继续，有更新的话更新数据库
                    if (!isNewDatabase && !IsNeedUpdateDatabase(app.AssemblyName))
                    {
                        continue;
                    }

                    var msg = DatabaseInitialize(app.AssemblyName, tenant);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        sbMsg.AppendLine(msg);
                    }
                    //foreach (var module in modules.ToList())
                    //{
                    //    //更新数据库时，先判断程序集是否有更新，无更新的话继续，有更新的话更新数据库
                    //    if (!isNewDatabase && !IsNeedUpdateDatabase(module.AssemblyName))
                    //    {
                    //        continue;
                    //    }

                    //    var msg = DatabaseInitialize(module.AssemblyName, tenant);
                    //    if (!string.IsNullOrEmpty(msg))
                    //    {
                    //        sbMsg.AppendLine(msg);
                    //    }
                    //}

                    if (sbMsg.Length == 0)
                    {
                        if (isUpdateStatus)
                        {
                            //2017-09-27 v2.0.0.6 版本以后，使用windwos server 2016的泛域名解析，不再需要添加Web站点的IIS的HostHeader了。
                            ModifyTenantUserApplication(tenant.TenantName, tenant.TenantDisplayName, app, ApplicationStatus.OpenSuccess);
                        }
                    }
                    else
                    {
                        sbError.AppendLine(sbMsg.ToString());
                    }
                }

                var errorMsg = sbError.ToString();
                if (!string.IsNullOrEmpty(errorMsg))
                    AddOpenAppErrorLog(tenant.TenantId, tenant.TenantDisplayName, OpenServerType.OpenDatabase, errorMsg);

                return errorMsg;
            }
            catch (Exception ex)
            {
                var message =
                    string.Format(
                        "Thread Id=" + Thread.CurrentThread.ManagedThreadId +
                        ": 租户[{0}--{1}]，UpgradeTenantDatabase出错, 错误消息：{2}.",
                        tenant.TenantName, tenant.TenantDisplayName, ex.Message);
                AddOpenAppErrorLog(tenant.TenantId, tenant.TenantDisplayName, OpenServerType.OpenDatabase, message);
                _logger.LogError(message + " Starck Tracke: " + ex.StackTrace);
                return message;
            }
        }
        private string RollBackTenantDataBase(TenantUser tenant, bool isUpdateStatus = true)
        {
            try
            {
                var connestring = KC.Framework.Base.GlobalConfig.GetDecryptDatabaseConnectionString();
                if (tenant.OwnDatabase == null)
                {
                    var message = string.Format("租户[{0}--{1}]，未设置所归属的数据服务器（OwnDatabase）。",
                        tenant.TenantName, tenant.TenantDisplayName);
                    _logger.LogError(message);

                    return message;
                }

                //必须获取每个Tenant所归属的OwnDatabase对象，以便获取Sa的登录用户权限进行创建Tenant的数据库LoginUser用户
                if (tenant.OwnDatabase != null
                    && !string.IsNullOrWhiteSpace(tenant.OwnDatabase.ConnectionString))
                {
                    connestring = tenant.OwnDatabase.ConnectionString;
                }
                var createDataBase = DbLoginUserUtil.CreateTenantDbLoginUser(tenant, connestring);
                if (!createDataBase)
                {
                    var errormsg = string.Format("租户[{0}--{1}]，创建所属数据库的登录用户出错。", tenant.TenantName,
                        tenant.TenantDisplayName);
                    AddOpenAppErrorLog(tenant.TenantId, tenant.TenantDisplayName, OpenServerType.CreateDbUser,
                        errormsg);

                    return errormsg;
                }

                var sbError = new StringBuilder();
                foreach (var app in tenant.Applications.ToList())
                {
                    var msg = DatabaseRollBack(app.AssemblyName, tenant);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        sbError.AppendLine(msg);
                    }
                    //var modules = app.ApplicationModules.ToList();
                    //if (!modules.Any())
                    //    continue;

                    //foreach (var module in modules.ToList())
                    //{
                    //    var msg = DatabaseRollBack(module.AssemblyName, tenant);
                    //    if (!string.IsNullOrEmpty(msg))
                    //    {
                    //        sbError.AppendLine(msg);
                    //    }
                    //}
                }

                var errorMsg = sbError.ToString();
                if (!string.IsNullOrEmpty(errorMsg))
                    AddOpenAppErrorLog(tenant.TenantId, tenant.TenantDisplayName, OpenServerType.RollbackDatabase, errorMsg);

                return errorMsg;
            }
            catch (Exception ex)
            {
                var message = string.Format("租户[{0}--{1}]，RollBackTenantDataBase出错, 错误消息：{2}.",
                    tenant.TenantName, tenant.TenantDisplayName, ex.Message);
                AddOpenAppErrorLog(tenant.TenantId, tenant.TenantDisplayName, OpenServerType.RollbackDatabase, message);

                return message;
            }
        }

        private string DatabaseInitialize(string assemblyName, Tenant tenant)
        {
            try
            {
                _logger.LogDebug(string.Format("-----开始初始化tenant（{0}--{1}）的数据库（{2}）",
                                tenant.TenantName, tenant.TenantDisplayName, assemblyName));
                var preMigration = string.Empty;
                var latestMigration = string.Empty;
                switch (assemblyName)
                {
                    //TODO: generate the database depandent by assambly name
                    //case "KC.DataAccess.Account":
                    //    new KC.DataAccess.Account.ComAccountDatabaseInitializer().Initialize(tenant);
                    //    break;
                    //case "KC.DataAccess.Customer":
                    //    new KC.DataAccess.Customer.ComCustomerDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.Offering":
                    //    new KC.DataAccess.Offering.ComOfferingDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.Shop":
                    //    new KC.DataAccess.Shop.ComShopDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.Order":
                    //    new KC.DataAccess.Order.ComOrderDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.FinancingOffering":
                    //    new KC.DataAccess.FinancingOffering.ComFinancingOfferingDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.FinancingOrder":
                    //    new KC.DataAccess.FinancingOrder.ComFinancingOrderDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.FinancingCredit":
                    //    new KC.DataAccess.FinancingCredit.ComFinancingCreditDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.CreditLevel":
                    //    new KC.DataAccess.CreditLevel.ComCreditDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.Erp":
                    //    new KC.DataAccess.Erp.ComErpDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.Scm":
                    //    new KC.DataAccess.Scm.ComScmDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.Finance":
                    //    new KC.DataAccess.Finance.ComFmsDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.Arap":
                    //    new KC.DataAccess.Arap.ComArapDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.Casier":
                    //    new KC.DataAccess.Cashier.ComCashierDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.WorkFlow":
                    //    new KC.DataAccess.WorkFlow.ComWorkFlowDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.Payment":
                    //    new KC.DataAccess.Payment.ComPaymentDatabaseInitializer(tenant).Initialize();
                    //    break;

                }

                return string.Empty;
            }
            catch (Exception e)
            {
                string errorMsg = string.Format("更新租户（{0}--{1}）的数据库（{2}）失败, 错误消息：{3}.",
                    tenant.TenantName, tenant.TenantDisplayName, assemblyName, e.Message);
                _logger.LogError(errorMsg, e.StackTrace);
                return errorMsg;
            }
        }
        private bool IsNeedUpdateDatabase(string assemblyName)
        {
            switch (assemblyName)
            {
                //TODO: generate the database depandent by assambly name
                //case "KC.DataAccess.Account":
                //    return KC.DataAccess.Account.DataInitial.DBSqlInitializer.IsNeedUpdataDatabase();
                //break;
                //case "KC.DataAccess.Customer":
                //    return KC.DataAccess.Customer.DataInitial.DBSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.Offering":
                //    return KC.DataAccess.Offering.DataInitial.DBSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.Shop":
                //    return KC.DataAccess.Shop.DataInitial.DBSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.Order":
                //    return KC.DataAccess.Order.DataInitial.DBSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.FinancingOffering":
                //    return KC.DataAccess.FinancingOffering.DataInitial.DBSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.FinancingOrder":
                //    return KC.DataAccess.FinancingOrder.DataInitial.DBSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.FinancingCredit":
                //    return KC.DataAccess.FinancingCredit.DataInitial.DBSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.CreditLevel":
                //    return KC.DataAccess.CreditLevel.DataInitial.DBSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.Erp":
                //    return KC.DataAccess.Erp.DataInitial.DbSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.Scm":
                //    return KC.DataAccess.Scm.DataInitial.DbSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.Finance":
                //    return KC.DataAccess.Finance.DataInitial.DbSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.Arap":
                //    return KC.DataAccess.Arap.DataInitial.DbSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.Casier":
                //    return KC.DataAccess.Cashier.DataInitial.DbSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.WorkFlow":
                //    return KC.DataAccess.WorkFlow.DataInitial.DbSqlInitializer.IsNeedUpdataDatabase();
                //    break;
                //case "KC.DataAccess.Payment":
                //    return KC.DataAccess.Payment.DataInitial.DBSqlInitializer.IsNeedUpdataDatabase();
                //    break;
            }

            return false;
        }
        private string DatabaseRollBack(string assemblyName, Tenant tenant)
        {
            _logger.LogDebug(string.Format("-----开始回滚tenant（{0}--{1}）的数据库（{2}）",
                tenant.TenantName, tenant.TenantDisplayName, assemblyName));
            try
            {
                switch (assemblyName)
                {
                    //TODO: generate the database depandent by assambly name
                    //case "KC.DataAccess.Account":
                    //    new KC.DataAccess.Account.ComAccountDatabaseInitializer().RollBack(tenant);
                    //    break;
                    //case "KC.DataAccess.Customer":
                    //    new KC.DataAccess.Customer.ComCustomerDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.Offering":
                    //    new KC.DataAccess.Offering.ComOfferingDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.Shop":
                    //    new KC.DataAccess.Shop.ComShopDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.Order":
                    //    new KC.DataAccess.Order.ComOrderDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.FinancingOffering":
                    //    new KC.DataAccess.FinancingOffering.ComFinancingOfferingDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.FinancingOrder":
                    //    new KC.DataAccess.FinancingOrder.ComFinancingOrderDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.CreditLevel":
                    //    new KC.DataAccess.CreditLevel.ComCreditDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.Erp":
                    //    new KC.DataAccess.Erp.ComErpDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.Finance":
                    //    new KC.DataAccess.Finance.ComFmsDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.Scm":
                    //    new KC.DataAccess.Scm.ComScmDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.Arap":
                    //    new KC.DataAccess.Arap.ComArapDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.Casier":
                    //    new KC.DataAccess.Cashier.ComCashierDatabaseInitializer(tenant).Initialize();
                    //    break;
                    //case "KC.DataAccess.WorkFlow":
                    //    new KC.DataAccess.WorkFlow.ComWorkFlowDatabaseInitializer(tenant).RollBack();
                    //    break;
                    //case "KC.DataAccess.Payment":
                    //    new KC.DataAccess.Payment.ComPaymentDatabaseInitializer(tenant).RollBack();
                    //    break;
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                string errorMsg = string.Format("回滚租户（{0}--{1}）的数据库（{2}）失败, 错误消息为：{3}.",
                    tenant.TenantName, tenant.TenantDisplayName, assemblyName, e.Message);
                _logger.LogError(errorMsg, e.StackTrace);

                return errorMsg;
            }
        }

        public bool SendTenantAppOpenEmail(TenantUserDTO data)
        {
            var tenant = _mapper.Map<TenantUser>(data);

            var emailTitle = string.IsNullOrEmpty(data.ContactEmail)
                ? "[租户开通消息]租户通知邮箱为空，请手动发送该邮件"
                : "[系统消息]租户开通";

            var emailBody = GetTenantOpenAppsEmailBody(tenant, null);

            var testInboxs = GlobalConfig.AdminEmails;
            var defaultSendTo = string.IsNullOrWhiteSpace(testInboxs)
                ? new List<string>() { "tianchangjun@cfwin.com", "rayxuan@cfwin.com" }
                : testInboxs.ArrayFromCommaDelimitedStrings().ToList();
            var mail = new KC.Model.Component.Queue.EmailInfo
            {
                UserId = RoleConstants.AdminUserId,
                Tenant = data.TenantName,
                EmailTitle = emailTitle,
                EmailBody = emailBody,
                SendTo = !string.IsNullOrWhiteSpace(data.ContactEmail)
                    ? new List<string>() { data.ContactEmail }
                    : defaultSendTo
            };

            var isSucceed = new StorageQueueService().InsertEmailQueue(mail);
            //var isSucceed = (new TenantStorageQueueService(data)).InserEmailQueue(mail);
            //_logger.LogInformation((isSucceed ? "插入Email队列成功，邮件内容：" : "插入Email队列失败，邮件内容：") + emailBody);
            if (!isSucceed)
            {
                AddOpenAppErrorLog(data.TenantId, data.TenantDisplayName, OpenServerType.SendOpenEmail,
                    "插入Email队列失败，邮件内容：" + emailBody);
            }
            else
            {
                _logger.LogInformation(string.Format("开通邮件已成功发送给租户【{0}】邮箱：{1}", data.TenantDisplayName, mail.SendTo.First()));
            }
            return isSucceed;
        }

        private string GetTenantOpenAppsEmailBody(TenantUser data, string adminUserNewPwd)
        {
            //独立数据库
            var isProfessional = data.Version == Framework.Tenant.TenantVersion.Professional;
            //标准版
            var isStandard = data.Version == Framework.Tenant.TenantVersion.Standard;

            //企业和金融机构地址不同
            var ssourl = GlobalConfig.SSOWebDomain.Replace(TenantConstant.SubDomain, data.TenantName).ToLower();

            var tenantUserType =
                data.TenantType == TenantType.Enterprise
                ? TenantUserType.Company
                : TenantUserType.Institution;
            if (tenantUserType == TenantUserType.Company)
                ssourl = GlobalConfig.PortalWebDomain.Replace(TenantConstant.SubDomain, data.TenantName).ToLower();
            if (tenantUserType == TenantUserType.Institution)
                ssourl = GlobalConfig.JRWebDomain.Replace(TenantConstant.SubDomain, data.TenantName).ToLower();

            var dbaurl = GlobalConfig.SSOWebDomain.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName).ToLower();
            var imageHeader = dbaurl + "/content/images/v3.0/image002.jpg";
            var imageFooter = dbaurl + "/content/images/v3.0/image003.jpg";

            var pwd = EncryptPasswordUtil.DecryptPassword(data.DatabasePasswordHash,
                !string.IsNullOrEmpty(data.PrivateEncryptKey) ? data.PrivateEncryptKey : null);
            var storageDeString = EncryptPasswordUtil.DecryptPassword(data.StorageAccessKeyPasswordHash,
                !string.IsNullOrEmpty(data.PrivateEncryptKey) ? data.PrivateEncryptKey : null);

            var apps = new StringBuilder();
            if (data.Applications.Any())
            {
                data.Applications.ToList().ForEach(
                    a =>
                        apps.Append(
                            string.Format(
                                @"<tr><td><p>{0}</p></td><td><p  style='color: #c6d4df;'>{1}</p></td><td><a  style='color: #c6d4df;' href='http://{2}'>{2}</a></td></tr>",
                                a.ApplicationName, a.ApplicationId, a.DomainName.ToLower())));
            }

            var adminPwd = !string.IsNullOrEmpty(adminUserNewPwd)
                ? adminUserNewPwd
                : "对不起，您的登录密码已被系统加密，请使用登录密码进行登录或找回密码功能进行找回";

            #region 邮件

            var emailBody = isProfessional
                ? string.Format(
                    @"<!doctype html><html lang='en'><head><meta charset='UTF-8'><title></title><style>*,html,body{{margin:0;padding:0}}.divdiv{{width:638px;position:relative;margin:0 auto;background:#17212E;padding:20px;padding-top:0}}table{{width:100%;background:#454d5b;text-align:center;margin:0 auto;margin-top:50px;margin-bottom:50px;border-collapse:collapse}}.p_white{{color:#fff}}table tr td:first-child{{text-align:left;width:33%}}table tr td{{text-align:left;padding:9px;color:#fff;min-width:136px}}table tr th{{color:#fff;padding:13px;background:#4b5761;text-align:left}}table tr td p{{word-break:break-all;color:inherit}}.app_tb tr td:first-child{{text-align:left}}tr{{border:1px solid #636363}}.tips tr{{border:none}}.tips tr td:first-child{{text-align:left}}a{{text-decoration:none;color:#fff;word-break:break-all}}.guanggao_contain{{width:100%;position:relative;float:left;margin-top:30px;margin-bottom:30px}}.guanggao_logo{{float:left;margin-left:20px;display:block;position:relative;background-position:35px 14px;color:#afafaf}}.guanggao_logo_p{{float:left;margin-left:20px;display:block;position:relative;background-position:35px 14px;color:#afafaf;margin-bottom:10px;clear:both;width:100%}}.tips{{min-height:50px;background:0 0;font-size:12px}}.guanggao_logo:hover{{color:#fff}}.title p{{ font-size: 16px; color: #c6d4df; font-family: Arial, Helvetica, sans-serif; font-weight: bold;margin: 8px;
}}</style></head><body><div class='divdiv'><div class='guanggao_contain'><a href='http://www.cfwin.com/' target='_blank' class='guanggao_logo'>
<img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIwAAAAyCAYAAACOADM7AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyJpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNiAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6OEM5OUYyMUZBMUE0MTFFN0FFNThCQjEyNDI1MjA0MEIiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6OEM5OUYyMjBBMUE0MTFFN0FFNThCQjEyNDI1MjA0MEIiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDo4Qzk5RjIxREExQTQxMUU3QUU1OEJCMTI0MjUyMDQwQiIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDo4Qzk5RjIxRUExQTQxMUU3QUU1OEJCMTI0MjUyMDQwQiIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PsljtMkAAAmlSURBVHja7F0JbBVVFH21AopsQt2QAIpGwCCCgAtFqiyWXRIxuIJiFBWVusQ1EbegEgQUgygqSBCMCBQjAmqBihs7FgWUTSQIgtACLmyt9+Tfl3/7mPl/5s/0/+nv3OSkb/b335x37333vjfNOPRNo8EqeTK/Vscdu8ydZWVlqgrL04SnCBmEkYQXglzZDCJMMt/WNUSYxQEgTC/CQEIrQh3CcMLcFLR/R8JSY18OYUlQCXNyFevNZxFm8EuRUj9F9Wlpsa9FSJhgCEjxNeHCANVpucW+ZUFuxKpEmDcMsvxFmE4oIfyaojqtIdxEuJd9mAmEVSFhUi/NCDeL7e/Yj9kfgLrNYFQKOamKEKa3KJcyefarUELCxNAw0gxsC199+pok2PbOhH6EtoRGhEzCAcIGQiHhY8Ju47rGokM0FPuPEpoa5+Je+whdeKit5X32cawki3Cr4ax+G+N3DCCcy+W/Ce9wuY8g9GbCp4LkfcT18G8OE+oR7mSteRGhOmEv4UfCLG6L0gp7GQGPw3QivE64NM59QYKJhGfECy4m1HVYr3EqEou5m++jpT9hjs01QwiTxPaXhG4xNDle6um8DVL05fIc7gyQfML1XMbf2eIeuLYr16++A0d6Q1UzSY8RFjsgC6QaYRhhBeECD8+cb2x3iXFuL2P7akJtm3NbC7JAFiZQt1tYe8SLGaG9vjK0atoT5gHCq0b95rIJ6KAiwa1cwnhW71qaEC7z8NzthPVSI9qcV91Cm1RnDWAlXeIQ04mMFk77dDZxlzOGEjaJcxty+6XEh4F998tsHXNwDnrja2J7LzeOacqgchdww+SzZhnA+7Q5y+Ty88IfWEm4y7jXHuNltuDyxYQzCX8a52cTalnUvY9hRrTkiPIW4+U6lRrcFn05LKAM/wlD8yXcfpAbCXnGb0sKYZqQ31GcRO3ysqhXqU0DSfmdezuc4bVif5Eo7xPlQ2zn7WQBN7SWay3iJL1tru3JTrrsYJlMXi/aRcvtMdqihDVzoTDTOWzG0tYkNWNTo2VyHLJo+csgixdBL/03jlnqYUMA5KraGediu45ByEQERPg8zjlIfew0tHXSTdITNJL6z6fnTSZtFSsG0sPYfisFpP2PSZNr43+cT2jO5ePcqzeKzgdneLkN4TCaK0iwXh86PK9IOLwNUkGYx3183uI4QTMZAzmoUpdX+VwQphmbux3C7Gj5lv2RQuGnwFyNsPFfvmGTmIgsd0F4Laem+yjpDMM3OZ6iephmo6vNcFoPj2eJfRilnSNGTtk+mCOlAhKdDhph6ony4RTWY6PxgrSWqGlojHn8d46F86s4BHCaTw5vaUgYa+dVOpCplPkWcRSMmE7hMlIRq4U2XGYxipLk2u2jYx4SRsQotMBxaxIQwjTiOE9Pw7zI4bOMv3TjuEmOYb7KQsL4K4UWcYdUSQGPauRop2cM8yL9GJih6wz/Zb5KAwkaYZDA2yu281QF5UQcyEFVPvs8TGi8UnViPugXwk9i+znWMoo1y8KQMP4LHF2ZFkDCDlHWWnGuQ44pX/gXfg6vtVwiyssMf8vKLMmk6SqjI4SE8VFAGBnW78QxiFyL+iLP8wphioqkEObxSKaihtfxzMssB8Sr1BLECVTQMpgfgjC3nnDUnBsdi+AwUegfNlWIeWSKazuzr/GZT3VZy8882yFhMGraSjjP2L8wXQgT1OkNaPSrVCQyKgUvrruKTC7qYJAFc3T7+0gW7XsssBj6x4q6mtlqZPu/SycNk8zYgJuw+HY2R0jT388jjgyL8xADmUoYU0F+ArTJIGO7NA5hHhbbmMx0LF0IkxGESjhcKovplsi+nqOic0MQkd3s4NpsFZ2Jt8vlELcuay4tmNW3Lo7Wvk207So2o3aCYGBj0Ul0crIxH9OC5OMRB/WV90Oea2lVJUwooQ8TSmiSQgmU+K29Tw7NQSihSQolJEwowZCKjPS2V5FI7aY4w1C3gq82YWYeZskvqoB6I9OsPwuCGI9VzgjTHbK4jDSG3zMDkRPLFUPtwHwCJMOBD7MojnOMG2AagjlZ+kFCG8IXhEdU+fxQLAHJBsQ4jqwx5tlidvyoGOdhOermBNpELlFFtnysxTmTVTSYhzTAtgSeg3iS3XxprDK4g8tYWGeXWkAcakLQNEwO/0X0cqvYj4bSmWUEigYb1+ngUUsmjlPCoHeP4fIn6sTo8FIVDUaZy2gxh/YmLm9zSZi2XOfLxb5WKrrW2eq3QZCqwEK3n1VkioMbgabU363BCgk9LRXphHGV3SQtYtYP4t47WhxDzger/obw9rsqEq5HEnEm4YME6/cs4SUX548QhHEr3dkMZIl9nVX5T4VoaS7K+JICZuq/55Iwf7CG1ITJdmnaFiVbuyTiw9RlWC06byA0wxwPdVov1PEfLu+1U1zr1u6/zJAmabwDkzRQJT6jf5cHTVIUdA0TT/xaA7ObTUmhh3v0Y2cx6ILs+0NCSztJUjZVkZl/+azV0oIwXldKymkLyFjvcHANHMW1lThcUKScLa05RaVwcryfhJHrh//x8b5XKGfTFmpWQpLsN0zSYXbczVHpMfG+vmesr0yE+dtGvepjBzzWaw+rXO0MOpV84ftUBskTJkkxeQarE7+cha9NlLATrgXbE4NOmHrihVrZVsgWj3Vq4HJkZCVP8ov4zeH5tTlWZI6AclX51ZjKYjiPT50Vc0cZlUBdS/gZxQZxzmCnuJlo2yUc5hhrEcYIHGGgJtsJxlvFTyCbPNaptop+8+1Ni/thGNqeHePxxjEE/R4VQ+zfXLRDU+F/TbHoCGaPXyM6EXAwwd8LbWIVPUVEG8tWjhrD/DKPA4IKJ8w+dkK7quik7Css7Gwb/rvSx/odMnoe5Iiw68Ux/Ci3voTusZgSqv8HwEc2vkKOigY0Czy+wAPsxJrkrsbkzRTOLgKEWClxpXI2+y4lhGnAvWCFeGH3qMhUwDrcs+Bw6nU7K3ysXy6TU8oFwmcy1XItj8/Db5jG7YIg3Eib86Dd5rI2vJfNWCIfis7gNjSvBSkach1aCMLA2UUk+iwmVNLFyfATZJjNLwr29ioe5sIETeUekM1/jzJh6vn0g/AZ1esN6Dmv6yyO5Xlsi7dFJxoaY5j7O9dNv9xEP0CYxfdqpaIpkI7iOBboIeVwAz9nJ3eU+wyfMnmC5GMM1CAUlEWkmHAl729NKOH90wijuPw1YXhZeRkT5xkmGpf5I61dPneouPZdB+dnEn4Q13R0+TxgA2EmYbVoqxe5PJYwiXCcMIj3DefrruHtzgk80xP+F2AAonbUk1/DbMYAAAAASUVORK5CYII='></a></div>
<div class='title'><p>主题：【产融协作服务已开通】</p><p>尊敬的 {1}：您好！</p><p>您已成功开通财富共赢产融协作服务，以下是您开通服务的相关信息，请妥善保管：</p> </div>
<table><tr><td><p>公司名称:</p></td><td><p>{1}</p></td></tr>
<tr><td><p>租户代码:</p></td><td><p>{0}</p></td></tr>
<tr><td><p>联系人:</p></td><td><p>{3}</p></td></tr>
<tr><td><p>联系人电话:</p></td><td><p>{4}</p></td></tr>
<tr><td><p>联系人邮箱:</p></td><td><p style='color: #c6d4df;'>{5}</p></td></tr>
<tr><td><p>登录地址:</p></td><td><p><a  style='color: #c6d4df;' href='{2}'>{2}（请选择【产融协作员工】登录）</a></p></td></tr>
<tr><td><p>管理员账户:</p></td><td><p  style='color: #c6d4df;'>admin@cfwin.com</p></td></tr>
<tr><td><p>初始密码:</p></td><td><p>{15}</p></td></tr>
<tr><td><p>加密密钥:</p></td><td><p>{6}</p></td></tr>
<tr><td><p>WebApi授权密钥</p></td><td><p>{7}</p></td></tr></table>
<table><tr><td><p>数据库服务器:</p></td><td><p>{8}</p></td></tr>
<tr><td><p>数据库实例:</p></td><td><p>{9}</p></td></tr>
<tr><td><p>数据库密码:</p></td><td><p>{10}</p></td></tr>
<tr><td><p>存储服务器:</p></td><td><p>{11}</p></td></tr>
<tr><td><p>存储服务器Key:</p></td><td><p>{12}</p></td></tr>
<tr><td><p>存储服务器SecuryKey:</p></td><td><p>{13}</p></td></tr></table>
<div style='padding-top:12px' colspan='2'><span style='font-size: 17px; color: #c6d4df; font-family: Arial, Helvetica, sans-serif; font-weight: bold;'><p>您此次开通的系统地址:</p></span></div>
<table class='app_tb'><tr><th><p>系统名称</p></th><th><p>系统ID</p></th><th><p>系统地址</p></th></tr>{14}</table>
<table class='tips'><tr><td>
<h3>说明：</h3>
            <p>管理员账号：管理员账号可登陆所有系统，拥有最大权限，建议专人使用。管理员账号用于管理组织架构、人员及权限添加、流程设置等。</p>
            <p>初始密码：管理员账号的初始密码，登录后请及时修改；密码一经修改，所有系统同步；密码需同时包含字母和数字。</p>
            <p>登录不成功：请联系在线客服或拨打客服热线400-788-8586（工作日8:30-18:00），感谢您对财富共赢的支持。</p>
            <p>--------------------------------------此邮件由系统自动发送--------------------------------</p>
</td></tr><tr><td><br></td></tr></table>
<div class='clear'></div><div class='tips'><p class='guanggao_logo_p'>友情链接</p><a href='http://www.cfwin.com/' target='_blank' class='guanggao_logo'>财富共赢金融超市</a> <a href='http://www.starlu.com/' target='_blank' class='guanggao_logo'>大陆之星赊销商城</a> <a href='http://www.cfwinclub.com/' target='_blank' class='guanggao_logo'>财富共赢俱乐部</a></div></div></body></html>",
                    data.TenantName, data.TenantDisplayName, ssourl,
                    data.ContactName, data.ContactPhone, data.ContactEmail,
                    data.PrivateEncryptKey, data.TenantSignature,
                    data.Server, data.Database, pwd,
                    data.StorageEndpoint, data.StorageAccessName, storageDeString, apps, adminPwd)
                : GetTenantOpenAppsEmailBodyStandard(data, ssourl, adminPwd);

            #endregion

            return emailBody;
        }

        /// <summary>
        /// 开通通用版本发送邮件的模版
        /// </summary>
        /// <param name="data">租户信息</param>
        /// <param name="url">登录系统地址</param>
        /// <param name="adminUserNewPwd">登录系统密码</param>
        /// <returns></returns>
        private string GetTenantOpenAppsEmailBodyStandard(TenantUser data, string url, string adminUserNewPwd)
        {
            return string.Format(@"<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Email</title>
    <style>
        .bgimage1 {{width: 211px;height: 117px;margin-left: 50px;background-image: url(headerImageUrl);}}
        .bgimage2 {{ width: 690px;height: 298px;background-image: url(footerImageUrl); }}
    </style>
</head>
<body>
    <div><table id='tblContent' width='85%' border='0' cellpadding='0' style='width: 85%'>	<tbody>	<tr>		<td style='width: 99.5%; padding: .75pt'>			<div align='center' style='text-align: center'>				<table width='1000' border='0' cellspacing='0' cellpadding='0' style='width: 600pt'>				<tbody>				<tr>					<td style='width: 100%; padding: 0'>					</td>				</tr>				<tr>					<td style='width: 100%; padding: 0; border: 6pt solid #CCC'>						<table width='100%' border='0' cellpadding='0' style='width: 100%'>						<tbody>						<tr>							<td style='width: 50%; padding: 0'>								<div style='margin: 0'>									<font face='宋体' size='3'>																		<div class='bgimage1'></div>																		</font>								</div>							</td>							<td style='padding: 0'>								<div style='display: inline-block; padding: 0 18pt 0 0; border-style: none solid none none; border-right-width: 6pt; border-right-color: #FCD105'>									<div align='right' style='margin: 0'>										<font face='宋体' size='3'><span style='font-size: 12pt'><font face='宋体' size='2'><span style='font-size: 9pt'><b>系统开通信息</b></span></font><font face='Verdana,sans-serif' size='2'><span style='font-size: 9pt' lang='en-US'>&nbsp;</span></font></span></font>									</div>								</div>							</td>						</tr>						</tbody>						</table>						<div style='margin: 0 0 12pt 0'>							<font face='宋体' size='3'><span style='font-size: 12pt'><font face='Verdana,sans-serif' size='2'><span style='font-size: 9pt' lang='en-US'>&nbsp;</span></font></span></font>						</div>						<div>							<div style='margin: 12pt 24pt'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='Verdana,sans-serif' size='2'><span style='font-size: 9pt' lang='en-US'><b><br>								</b></span></font><font face='Verdana,sans-serif' size='2'><span style='font-size: 9pt' lang='en-US'><b><br>								</b></span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'><b>尊敬的</b></span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'><b>&nbsp;</b></span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'><b>{1}</b></span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'><b>,</b></span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'></span></font><span style='font-size: 10pt' lang='en-US'><br>								</span></span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'><br>								</span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>感谢贵司注册为财富共赢俱乐部会员，贵司的「财富共赢产融协作系统」通用版</span></font><font face='微软雅黑,sans-serif' size='2' color='#1F497D'><span style='font-size: 10pt'></span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>已开通。</span></font>							</div>							<div style='margin: 12pt 24pt'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>&nbsp;</span></font></span></font>							</div>						</div>						<div style='display: inline-block; padding: 0 0 0 18pt; border-style: none none none solid; border-left-width: 6pt; border-left-color: #FCD105'>							<div style='margin: 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif'><b>系统信息内容</b></font><font face='微软雅黑,sans-serif'></font></span></font>							</div>						</div>						<div style='margin: 0'>							<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>&nbsp;</span></font></span></font>						</div>						<table width='90%' border='0' cellpadding='0' style='width: 90%; margin-left: 24pt'>						<tbody>						<tr>							<td style='padding: 0'>								<table width='100%' border='0' cellpadding='0' style='width: 100%; margin-left: 24pt'>								<tbody>								<tr>									<td style='width: 30%; padding: .75pt'>										<div style='margin: 0'>											<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>会员编号：</span></font></span></font>										</div>									</td>									<td style='padding: .75pt'>										<div style='margin: 0'>											<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'><b>{0}</b></span></font></span></font>										</div>									</td>								</tr>								<tr>									<td style='width: 30%; padding: .75pt'>										<div style='margin: 0'>											<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>会员名称：</span></font></span></font>										</div>									</td>									<td style='padding: .75pt'>										<div style='margin: 0'>											<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>{1}</span></font></span></font>										</div>									</td>								</tr>								<tr height='20' style='height: 12.5pt'>									<td style='width: 30%; height: 12.5pt; padding: .75pt'>										<div style='margin: 0'>											<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>系统登陆地址：</span></font></span></font>										</div>									</td>									<td style='height: 12.5pt; padding: .75pt'>										<div style='margin: 0'>											<font face='宋体' size='3'><span style='font-size: 12pt'><a href='{2}' target='_blank' rel='noopener noreferrer'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>{2}</span></font></a></span></font>										</div>									</td>								</tr>								</tbody>								</table>								<table width='100%' border='0' cellpadding='0' style='width: 100%; margin-left: 24pt'>								<tbody>								<tr>									<td style='width: 30%; padding: .75pt'>										<div style='margin: 0'>											<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>管理员账户：</span></font></span></font>										</div>									</td>									<td style='padding: .75pt'>										<div style='margin: 0'>											<font face='宋体' size='3'><span style='font-size: 12pt'><a href='mailto:admin@cfwin.com' target='_blank' rel='noopener noreferrer'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>admin@cfwin.com</span></font></a></span></font>										</div>									</td>								</tr>								<tr>									<td style='width: 30%; padding: .75pt'>										<div style='margin: 0'>											<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>管理员初始密码：</span></font></span></font>										</div>									</td>									<td style='padding: .75pt'>										<div style='margin: 0'>											<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>{9}</span></font></span></font>										</div>									</td>								</tr>								</tbody>								</table>								<div style='margin: 0'>									<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 9pt' lang='en-US'><br>									</span></font><a href='{2}' target='_blank' rel='noopener noreferrer'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 9pt' lang='en-US'><span lang='en-US'>点击此处跳转网页登录「系统」</span></span></font></a><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 9pt' lang='en-US'></span></font></span></font>								</div>								<div style='margin: 0'>									<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 9pt' lang='en-US'><br>									</span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 9pt'>请妥善保管贵司的系统信息内容，如在登录或使用系统期间出现任何问题，请联络我们。</span></font></span></font>								</div>							</td>						</tr>						</tbody>						</table>						<div style='margin: 0'>							<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>&nbsp;</span></font></span></font>						</div>						<div style='margin: 0'>							<font face='宋体' size='3'><span style='font-size: 12pt'><font face='Calibri,sans-serif' size='2'><span style='font-size: 10.5pt' lang='en-US'>&nbsp;</span></font></span></font>						</div>						<div style='display: inline-block; padding: 0 0 0 18pt; border-style: none none none solid; border-left-width: 6pt; border-left-color: #FCD105'>							<div style='margin: 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif'><b>注意事项</b></font><font face='微软雅黑,sans-serif'></font></span></font>							</div>						</div>						<div style='width: 90%; margin-left: 24pt'>							<div style='margin: 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'><b>&nbsp;</b></span></font></span></font>							</div>							<div style='margin: 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='Calibri,sans-serif' size='2'><span style='font-size: 10.5pt' lang='en-US'><b>&nbsp;</b></span></font></span></font>							</div>							<div style='margin: 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'><b>管理员账户</b></span></font></span></font>							</div>						</div>						<div style='margin-left: 24pt'>							<div style='margin: 0 0 12pt 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>管理员账号可用于管理组织架构、人员及权限添加、流程设置等，并且拥有</span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'><b><i>最大权限</i></b></span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>，建议专人使用。</span></font></span></font>							</div>						</div>						<div style='margin-left: 24pt'>							<div style='margin: 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'><b>管理员初始密码</b></span></font></span></font>							</div>						</div>						<div style='margin-left: 24pt'>							<div style='margin: 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>首次登录管理员账户时，系统会强制要求修改新密码，新密码须为英文字母与数字组合。</span></font></span></font>							</div>						</div>						<div style='margin: 0'>							<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>&nbsp;</span></font></span></font>						</div>						<div style='display: inline-block; padding: 0 0 0 18pt; border-style: none none none solid; border-left-width: 6pt; border-left-color: white'>							<div style='margin: 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10.5pt'><b>「财富共赢产融协作系统」客户服务中心</b></span></font></span></font>							</div>							<div style='margin: 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='Calibri,sans-serif'><span lang='en-US'>&nbsp;</span></font></span></font>							</div>							<table width='100%' border='0' cellpadding='0' style='width: 100%; margin-left: 24pt'>							<tbody>							<tr>								<td style='width: 30%; padding: .75pt'>									<div style='margin: 0'>										<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>客服电话：</span></font></span></font>									</div>								</td>								<td style='padding: .75pt'>									<div style='margin: 0'>										<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>400-788-8586</span></font></span></font>									</div>								</td>							</tr>							<tr>								<td style='width: 30%; padding: .75pt'>									<div style='margin: 0'>										<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>客服</span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>QQ</span></font><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>：</span></font></span></font>									</div>								</td>								<td style='padding: .75pt'>									<div style='margin: 0'>										<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>4007888586</span></font></span></font>									</div>								</td>							</tr>							<tr>								<td style='width: 30%; padding: .75pt'>									<div style='margin: 0'>										<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>客服邮箱：</span></font></span></font>									</div>								</td>								<td style='padding: .75pt'>									<div style='margin: 0'>										<font face='宋体' size='3'><span style='font-size: 12pt'><a href='mailto:support@cfwin.com' target='_blank' rel='noopener noreferrer'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>support@cfwin.com</span></font></a></span></font>									</div>								</td>							</tr>							<tr>								<td style='width: 30%; padding: .75pt'>									<div style='margin: 0'>										<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt'>网页</span></font></span></font>									</div>								</td>								<td style='padding: .75pt'>									<div style='margin: 0'>										<font face='宋体' size='3'><span style='font-size: 12pt'><a href='http://www.cfwinclub.com' target='_blank' rel='noopener noreferrer'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>http://www.cfwinclub.com</span></font></a></span></font>									</div>								</td>							</tr>							</tbody>							</table>							<div style='margin: 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='Calibri,sans-serif' size='2'><span style='font-size: 10.5pt' lang='en-US'>&nbsp;</span></font></span></font>							</div>						</div>						<div>							<table width='100%' border='0' cellpadding='0' style='width: 100%; margin-left: 24pt'>							</table>							<div style='margin: 0'>								<font face='宋体' size='3'><span style='font-size: 12pt'><font face='微软雅黑,sans-serif' size='2'><span style='font-size: 10pt' lang='en-US'>&nbsp;</span></font></span></font>							</div>							<table width='90%' border='0' cellpadding='0' style='width: 90%; margin-left: 24pt'>							<tbody>							<tr>								<td style='padding: 0'>									<div style='margin: 0'>										<font face='宋体' size='3'>										<div class='bgimage2'>										</div>										</font>									</div>								</td>							</tr>							<tr>								<td style='padding:0'>									<div style='margin:0'>										<font face='宋体' size='3'><span style='font-size:12pt'><a href='https://www.starlu.com/Discount/209' target='_blank' rel='noopener noreferrer'><font face='Verdana,sans-serif' size='2'><span style='font-size:8.5pt' lang='en-US'><font face='宋体'><span lang='en-US'>名企季度采购</span></font></span></font></a></span></font>									</div>								</td>							</tr>							</tbody>							</table>							<div>								<div align='center' style='text-align:center;margin:0'>									<font face='宋体' size='3'><span style='font-size:12pt'><font face='Verdana,sans-serif' size='2'><span style='font-size:9pt' lang='en-US'>&nbsp;</span></font></span></font>								</div>							</div>							<div style='padding:0;border-style:solid none none none;border-top-width:4.5pt;border-top-color:#FDBC15'>								<div align='center' style='text-align:center;background-color:#CCC;margin:0'>									<font face='宋体' size='3'><span style='font-size:12pt;background-color:#CCC'><font face='Verdana,sans-serif' size='2'><span style='font-size:9pt' lang='en-US'><br>									</span></font><a href='http://www.cfwinclub.com/' target='_blank' rel='noopener noreferrer'><font face='Verdana,sans-serif' size='2'><span style='font-size:9pt' lang='en-US'>?<font face='宋体'><span lang='en-US'>「财富共赢」</span></font></span></font></a><font face='Verdana,sans-serif' size='2'><span style='font-size:9pt' lang='en-US'>&nbsp;I</span></font><font size='2'><span style='font-size:9pt'>财富共赢集团（深圳）有限公司</span></font></span></font>								</div>							</div>						</div>					</td>				</tr>				</tbody>				</table>			</div>		</td>	</tr>	</tbody>	</table>	<div style='margin:0'>		<font face='宋体' size='3'><span style='font-size:12pt'><font size='2'><span style='font-size:8.5pt'>此邮件由系统自动发送，请勿回复</span></font></span></font>	</div></div>
    <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAwAAAAFCAYAAABxeg0vAAAAFElEQVQImWP8DwQMJAAmUhQPUg0AzCEEBgkwS/wAAAAASUVORK5CYII=' />
</body>
</html>", data.TenantName, data.TenantDisplayName, url, data.ContactName, data.ContactPhone, data.ContactEmail, data.PrivateEncryptKey,
data.TenantSignature, "", adminUserNewPwd);
        }

        public void AddOpenAppErrorLog(int tenantId, string tenantDisplayName, OpenServerType type, string error)
        {
            try
            {
                _tenantUserOpenAppErrorLogRepository.Add(new TenantUserOpenAppErrorLog()
                {
                    TenantId = tenantId,
                    TenantDisplayName = tenantDisplayName,
                    OperatorId = RoleConstants.AdminUserId,
                    OperateDate = DateTime.UtcNow,
                    Operator = "系统",
                    OpenServerType = type,
                    Remark = error
                });
            }
            catch
            {
            }
        }

        private bool ModifyTenantUserApplication(string tenantName, string tenantDisplayName, TenantUserApplication app, ApplicationStatus appStatus, bool isAddLog = true)
        {
            try
            {
                var edtApp = _tenantApplicationRepository.GetById(app.Id);
                if (edtApp.AppStatus != appStatus)
                {
                    //using (var scope = ServiceProvider.CreateScope())
                    {
                        //var _unitOfContext = scope.ServiceProvider.GetRequiredService<ComAdminUnitOfWorkContext>();

                        var oldStatus = edtApp.AppStatus;
                        edtApp.AppStatus = appStatus;
                        _tenantApplicationRepository.ModifyAsync(edtApp, new[] { "AppStatus" }, false);

                        if (isAddLog)
                        {
                            var log = new TenantUserOperationLog
                            {
                                ApplicationId = app.ApplicationId,
                                ApplicationName = app.ApplicationName,
                                DomainName = app.DomainName,
                                AppStatus = appStatus,
                                TenantName = tenantName,
                                TenantDisplayName = tenantDisplayName,
                                LogType = OperationLogType.ChangeStatus,
                                Remark = string.Format("更新应用状态操作，将应用（{0}）状态（{1}）更改为：{2}。",
                                        app.ApplicationName, oldStatus.ToDescription(), appStatus.ToDescription()),
                                TenantUserApplicationId = app.Id,
                            };
                            _tenantUserOperationLogRepository.Add(log, false);
                        }

                        return _unitOfContext.Commit() > 0;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                string errorMsg =
                    string.Format(
                        "Thread Id=" + Thread.CurrentThread.ManagedThreadId +
                        ": 开通Tenant（{0}）的应用（Id:{1}--{2}）的Web服务器（{2}）成功，但更改该用户的应用开通状态失败。<br/>" + Environment.NewLine +
                        "请管理员更改数据库（表：TenantUserApplication）中应用（Id：{1}）状态（字段：AppStatus）为值为3。<br/>" + Environment.NewLine +
                        "错误消息：{3}",
                        tenantName, app.Id, app.ApplicationName, app.WebSiteName, e.Message);
                _logger.LogError(errorMsg, e.StackTrace);

                return false;
            }

        }

        #endregion

        #endregion

        #region Gitlab

        private static string TENANT_GROUP_NAME = "tenant";
        /// <summary>
        /// 获取配置的Gitlab域名，例如：http://gitlab.kcloudy.com
        /// </summary>
        /// <returns>Gitlab域名</returns>
        /// <exception cref="BusinessException"></exception>
        private string GetGitlabHostUrl()
        {
            string codeConnectString = GlobalConfig.GetDecryptCodeConnectionString();
            if (string.IsNullOrEmpty(codeConnectString))
                throw new BusinessException("未配置平台租户管理员的Gitlab相关连接字符串.");
            return StringExtensions.GetValueFromConnectionString(codeConnectString, ConnectionKeyConstant.CodeEndpoint);
        }
        /// <summary>
        /// 获取访问Gitlab api的客户端
        /// </summary>
        /// <returns>Gitlab api的客户端</returns>
        private GitLabClient GetAdminGitlabClient()
        {
            var codeConnectString = GlobalConfig.GetDecryptCodeConnectionString();
            if (string.IsNullOrEmpty(codeConnectString))
                throw new BusinessException("未配置平台租户管理员的Gitlab相关连接字符串.");
            var gitHostUrl = StringExtensions.GetValueFromConnectionString(codeConnectString, ConnectionKeyConstant.CodeEndpoint);
            if (string.IsNullOrEmpty(gitHostUrl))
                throw new BusinessException("未配置平台Gitlab的访问地址.");
            var admin_token = StringExtensions.GetValueFromConnectionString(codeConnectString, ConnectionKeyConstant.AccessKey);
            if (string.IsNullOrEmpty(admin_token))
                throw new BusinessException("未配置平台租户管理员的Gitlab相关Token.");

            return new GitLabClient(gitHostUrl, admin_token);
        }
        public async Task<string> CreateTenantGitlabGroupAndUser(string tenantName, string adminPassword)
        {
            var tenant = await _tenantUserRepository.GetTenantByNameOrNickNameAsync(tenantName);
            if (null == tenant)
                return $"未找到租户【{tenantName}】的信息。";

            try
            {
                var tenant_user_name = tenantName;
                var tenant_password = adminPassword;
                var tenant_group_name = tenantName;
                var tenant_token_name = tenantName + "-token";
                var tenant_token_expiredDate = DateTime.UtcNow.AddDays(365 * 3);

                var gitLabAdminApi = GetAdminGitlabClient();

                //获取Gitlab根分组
                int parentId;
                var adminGroup = await gitLabAdminApi.Groups.GetAsync(TENANT_GROUP_NAME);
                if (null == adminGroup)
                {
                    var newAdminGroup = new CreateGroupRequest(TENANT_GROUP_NAME, TENANT_GROUP_NAME)
                    {
                        Visibility = GroupsVisibility.Private,
                        RequestAccessEnabled = true
                    };
                    adminGroup = await gitLabAdminApi.Groups.CreateAsync(newAdminGroup);
                    _logger.LogInformation("----Create a admin group: " + adminGroup.Name);
                }
                parentId = adminGroup.Id;

                //创建租户管理员
                var tenantAdminUser = await gitLabAdminApi.Users.GetAsync(tenant_user_name);
                if (null == tenantAdminUser)
                {
                    var userRequest = new CreateUserRequest(tenant_user_name, tenant_user_name, tenant_user_name + "@126.com")
                    {
                        Admin = false,
                        Password = tenant_password,
                        ProjectsLimit = 20,
                        SkipConfirmation = true,
                        CanCreateGroup = true,
                        ResetPassword = false,
                        ForceRandomPassword = false
                    };
                    tenantAdminUser = await gitLabAdminApi.Users.CreateAsync(userRequest);
                    _logger.LogInformation($"----1. 租户【{tenantName}】 创建Gitlab的管理员账号: {tenantAdminUser.Name}");
                }

                //创建取租户管理员token
                var token = string.Empty;
                var userTokens = await gitLabAdminApi.Users.GetAllImpersonationTokensAsync(tenantAdminUser, ImpersonationState.ACTIVE);
                if (null == userTokens || !userTokens.Any(m => m.Name.Equals(tenant_token_name) && !m.Revoked))
                {
                    //https://docs.gitlab.com/ee/api/users.html#get-an-impersonation-token-of-a-user
                    var scopes = new List<Scope> {
                        Scope.API,
                        Scope.READ_USER,
                        Scope.READ_REPOSITORY,
                        Scope.WRITE_REPOSITORY,
                        //Scope.SUDO   //超级用户权限，除Owner外不允许授予此权限
                    };
                    var tokenRequest = new CreateImpersonationTokenRequest(tenantAdminUser, tenant_token_name, tenant_token_expiredDate, scopes);
                    var userToken = await gitLabAdminApi.Users.CreateImpersonationTokenAsync(tenantAdminUser, tokenRequest);
                    token = userToken.Token;
                    _logger.LogInformation($"----2. 租户【{tenantName}】 创建Gitlab租户管理员Token: {token}");
                }

                //在Gitlab根分组下创建租户分组
                var tenantGroupId = 0;
                var subGroups = await gitLabAdminApi.Groups.GetSubgroupsAsync(adminGroup);
                if (null == subGroups || !subGroups.Any(m => m.Name.Equals(tenant_group_name)))
                {
                    var groupRequest = new CreateGroupRequest(tenant_group_name, tenant_group_name)
                    {
                        ParentId = parentId,
                        Visibility = GroupsVisibility.Private,
                        RequestAccessEnabled = true
                    };
                    var tenantGroup = await gitLabAdminApi.Groups.CreateAsync(groupRequest);
                    tenantGroupId = tenantGroup.Id;

                    //将租户管理员用户加入gitlab的租户分组中
                    var member = new AddGroupMemberRequest(KC.GitLabApiClient.Models.AccessLevel.Owner, tenantAdminUser.Id);
                    await gitLabAdminApi.Groups.AddMemberAsync(tenantGroup, member);
                    _logger.LogInformation($"----3. 租户【{tenantName}】 创建Gitlab的租户分组: {tenantGroup.FullName}");
                }

                //更新租户代码仓库的Token
                var codeEncryptString = EncryptPasswordUtil.EncryptPassword(token, tenant.PrivateEncryptKey);
                //tenant.CodeEndpoint = gitHostUrl;
                tenant.CodeAccessName = tenant_token_name;
                tenant.CodeAccessKeyPasswordHash = codeEncryptString;

                var success = await _tenantUserRepository.ModifyAsync(tenant, new string[] { "CodeEndpoint", "CodeAccessName", "CodeAccessKeyPasswordHash" });
                if (success)
                    return "";

                return $"保存租户【{tenantName}】的代码仓库访问数据出错，请管理员手动保存，数据格式如下：" +
                    $"CodeEndpoint：{tenant.CodeEndpoint}，CodeAccessName：{tenant_token_name}，CodeAccessKeyPasswordHash：{codeEncryptString}";
            }
            catch (GitLabException e)
            {
                return $"租户【{tenantName}】调用Gitlab的接口出错，错误代码：{e.HttpStatusCode}，错误消息：{e.Message}";
            }
            catch (Exception e)
            {
                return $"创建租户【{tenantName}】的Gitlab分组及管理员账号出错，错误消息：{e.Message}";
            }

        }
        #endregion

        #region Tenant's Sms & Email & CallCenter Service

        public string OpenEmailService(int tenantId, string tenantName)
        {
            var tenant = _tenantUserRepository.GetDetailTenantById(tenantId);
            if (null == tenant)
                return "未找到该租户（TenantId=" + tenantId + "；TenantName=" + tenantName + "）的信息！";

            //tenant.EnableEmail = true;
            var sucess = _tenantUserRepository.Modify(tenant, new[] { "EnableEmail" });
            if (sucess)
                return string.Empty;

            return string.Format(
                "该租户（TenantId={0}；TenantName={1}）开通邮件服务已经{2}",
                tenantId,
                tenantName,
                "成功，但未更新邮件服务的状态，请管理员手动更新表dba.TenantUser字段EnableEmail的值为1！");
        }

        public string OpenSmsService(int tenantId, string tenantName)
        {
            var tenant = _tenantUserRepository.GetDetailTenantById(tenantId);
            if (null == tenant)
                return "未找到该租户（TenantId=" + tenantId + "；TenantName=" + tenantName + "）的信息！";

            //tenant.EnableSms = true;
            var sucess = _tenantUserRepository.Modify(tenant, new[] { "EnableSms" });
            if (sucess)
                return string.Empty;

            return string.Format(
                "该租户（TenantId={0}；TenantName={1}）开通短信服务已经{2}",
                tenantId,
                tenantName,
                "成功，但未更新短信服务的状态，请管理员手动更新表dba.TenantUser字段EnableSms的值为1！");
        }

        public string OpenCallCenterService(int tenantId, string tenantName)
        {
            var tenant = _tenantUserRepository.GetDetailTenantById(tenantId);
            if (null == tenant)
                return "未找到该租户（TenantId=" + tenantId + "；TenantName=" + tenantName + "）的信息！";

            //tenant.EnableCallCenter = true;
            var sucess = _tenantUserRepository.Modify(tenant, new[] { "EnableCallCenter" });
            if (sucess)
                return string.Empty;

            return string.Format(
                "该租户（TenantId={0}；TenantName={1}）开通呼叫中心服务已经{2}",
                tenantId,
                tenantName,
                "成功，但未更新呼叫中心服务的状态，请管理员手动更新表dba.TenantUser字段EnableCallCenter的值为1！");
        }

        #endregion

        #region 平台Tenant推送接口
        public async Task<bool> SaveSendTenantUsers(List<SendTenantUserDTO> models)
        {
            int successcount = 0;
            int errorcount = 0;
            foreach (var model in models)
            {
                if (await SaveSendTenantUser(model))
                {
                    successcount++;
                }
                else
                {
                    errorcount++;
                }
            }
            _logger.LogInformation("推送成功条数：" + successcount + "----失败条数：" + errorcount);
            return successcount > 0 ? true : false;
        }

        public async Task<bool> SaveSendTenantUser(SendTenantUserDTO model)
        {
            var existsTenant = _tenantUserRepository.GetDetailTenantByName(model.MemberId);
            if (existsTenant == null)
            {
                #region 新增

                #region 获取租户使用的资源对象

                var cloudType = model.CloudType;
                var selectedDb =
                    _databasePoolRepository.FindAll(
                        m =>
                            m.CloudType == cloudType && m.DatabasePoolId != 1 && m.Database.StartsWith("Com") &&
                            m.TenantCount <= ApplicationConstant.DatabaseLimit,
                        m => m.TenantCount, true)
                        .FirstOrDefault();
                var selectedStorage =
                    _storagePoolRepository.FindAll(
                        m => m.CloudType == cloudType && m.TenantCount <= ApplicationConstant.StorageLimit,
                        m => m.TenantCount, true)
                        .FirstOrDefault();
                var selectedQueue =
                    _queuePoolRepository.FindAll(
                        m => m.CloudType == cloudType && m.TenantCount <= ApplicationConstant.StorageLimit,
                        m => m.TenantCount, true)
                        .FirstOrDefault();
                var selectedNosql =
                    _noSqlPoolRepository.FindAll(
                        m => m.CloudType == cloudType && m.TenantCount <= ApplicationConstant.StorageLimit,
                        m => m.TenantCount, true)
                        .FirstOrDefault();

                var result = new TenantUserDTO()
                {
                    CloudType = cloudType,
                    PrivateEncryptKey = EncryptPasswordUtil.GetRandomString(),
                    TenantDisplayName = model.DisplayName,
                    TenantName = model.MemberId,
                    TenantSignature = model.Signature,
                    DatabasePoolId = selectedDb != null ? selectedDb.DatabasePoolId : 1,
                    StoragePoolId = selectedStorage != null ? selectedStorage.StoragePoolId : 1,
                    QueuePoolId = selectedQueue != null ? selectedQueue.QueuePoolId : 1,
                    NoSqlPoolId = selectedNosql != null ? selectedNosql.NoSqlPoolId : 1,
                    CanEdit = false,
                    ReferenceId = model.UserId,
                    ContactPhone = model.ContactPhone,
                    ContactEmail = model.ContactEmail,
                    ContactName = model.ContactName,
                    Version = ((int)model.Version) == 0 ? Framework.Tenant.TenantVersion.Standard : model.Version,
                    NickName = model.NickName,
                };

                #endregion

                #region 租户的类型

                if (model.IsEnterprise)
                {
                    result.TenantType = TenantType.Enterprise;
                }
                else
                {
                    result.TenantType = TenantType.Institution;
                }

                #endregion

                #region 租户开通的应用
                if (model.OwnApplications.Any())
                {
                    var cacheKey = "com-devdb-ApplicationWithModules-for-SaveSendTenantUser";
                    var applications = Service.CacheUtil.GetCache<List<TenantUserApplication>>(cacheKey);
                    if (applications == null || !applications.Any())
                    {
                        applications = _tenantApplicationRepository.FindWithModulesByFilter(m => m.TenantId == 2); //DevDB
                        if (applications.Any())
                        {
                            Service.CacheUtil.SetCache(cacheKey, result, TimeSpan.FromMinutes(TimeOutConstants.DefaultCacheTimeOut));
                        }
                    }

                    if (!model.OwnApplications.Any(m => m.ApplicationId.Equals(ApplicationConstant.SsoAppId.ToString(), StringComparison.OrdinalIgnoreCase)))
                    {
                        model.OwnApplications.Add(new SendApplcationDTO()
                        {
                            ApplicationId = ApplicationConstant.SsoAppId.ToString(),
                            ApplicationName = ApplicationConstant.SsoAppName,
                            ApplicationDomain =
                                GlobalConfig.SSOWebDomain.ToLower()
                                    .Replace(TenantConstant.SubDomain.ToLower(), model.MemberId)
                        });
                    }

                    model.OwnApplications.ForEach((Action<SendApplcationDTO>)(m =>
                    {
                        Guid appId;
                        if (!Guid.TryParse(m.ApplicationId, out appId)) return;

                        if (result.Applications.All(a => a.ApplicationId != appId))
                        {
                            if (applications.Any(a => a.ApplicationId.Equals(appId)))
                            {
                                var app = applications.FirstOrDefault(a => a.ApplicationId.Equals(appId));
                                app.Id = 0;
                                app.DomainName = app.DomainName.ToLower().Replace(TenantConstant.TestTenantName.ToLower(), model.MemberId);
                                app.AppStatus = ApplicationStatus.Initial;
                                try
                                {
                                    var version = (TenantVersion)((int)result.Version);
                                    app.Version = version;
                                }
                                catch
                                {
                                    app.Version = TenantVersion.Standard;
                                }

                                var appDto = _mapper.Map<TenantUserApplicationDTO>(app);
                                result.Applications.Add(appDto);
                            }
                        }
                    }));
                }
                #endregion

                #endregion

                return await SaveTenantUser(result);
            }
            else if (model.OwnApplications.Any())
            {
                var existsAppIds = existsTenant.Applications.Select(m => m.ApplicationId);
                var allAppIds = model.OwnApplications.Select(m => new Guid(m.ApplicationId));
                var openAppIds = allAppIds.Except(existsAppIds).ToList();

                if (openAppIds.Any())
                {
                    return AddTenantUserApplications(existsTenant.TenantId, openAppIds);
                }
            }

            return true;
        }

        public bool UpdateTenantUserInfo(SendTenantUserDTO model)
        {
            TenantUser data = _tenantUserRepository.GetDetailTenantByName(model.MemberId);
            if (!string.IsNullOrEmpty(model.ContactEmail))
                data.ContactEmail = model.ContactEmail;
            if (!string.IsNullOrEmpty(model.ContactPhone))
                data.ContactPhone = model.ContactPhone;
            var result = _tenantUserRepository.Modify(data, new[] { "ContactEmail", "ContactPhone" });
            return result;
        }

        /// <summary>
        /// 会员中心支付成功后更改版本为专业版
        /// </summary>
        /// <param name="memberIds"></param>
        /// <returns></returns>
        public bool SentUserVersion(SendTenantUserDTO model)
        {
            TenantUser data = _tenantUserRepository.GetDetailTenantByName(model.MemberId);
            data.Version = model.Version;
            var success = _tenantUserRepository.Modify(data, new[] { "Version" });
            _logger.LogInformation(string.Format("推送租户（{0}-{1}）更新到专业版是否成功：{2}",
                data.TenantName, data.TenantDisplayName, success));
            return success;
        }

        #endregion

        #region TenantUserOpenAppErrorLog

        public PaginatedBaseDTO<TenantUserOpenAppErrorLogDTO> FindOpenAppErrorLogsByFilter(int pageIndex, int pageSize, string tenantDisplayName, int? openServerType, string order)
        {
            Expression<Func<TenantUserOpenAppErrorLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(tenantDisplayName))
            {
                predicate = predicate.And(m => m.TenantDisplayName.Contains(tenantDisplayName));
            }
            if (openServerType != null)
            {
                predicate = predicate.And(m => (int)m.OpenServerType == openServerType);
            }

            var data = _tenantUserOpenAppErrorLogRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.OperateDate, order.Equals("asc"));

            var total = data.Item1;
            var rows = _mapper.Map<List<TenantUserOpenAppErrorLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<TenantUserOpenAppErrorLogDTO>(pageIndex, pageSize, total, rows);
        }
        public TenantUserOpenAppErrorLogDTO GetTenantErrorlogFormByProcessLogId(int id)
        {
            var data = _tenantUserOpenAppErrorLogRepository.GetTenantApplicationByFilter(m => m.ProcessLogId == id);
            return _mapper.Map<TenantUserOpenAppErrorLogDTO>(data);
        }

        public bool UpdateTenantErrorLogStatus(int id)
        {
            var model = new TenantUserOpenAppErrorLog
            {
                ProcessLogId = id,
                Type = ProcessLogType.Success,
            };

            return _tenantUserOpenAppErrorLogRepository.Modify(model, new[] { "Type" });
        }

        #endregion

        #region TenantUserOperationLog
        public PaginatedBaseDTO<TenantUserOperationLogDTO> FindTenantUserOperationLogByFilter(int pageIndex, int pageSize, string searchTenantDisplayName, string searchTenantName, string order, int? operationLogType)
        {
            Expression<Func<TenantUserOperationLog, bool>> precidate = m => true;
            if (!string.IsNullOrWhiteSpace(searchTenantDisplayName))
            {
                precidate = precidate.And(m => m.TenantDisplayName.Contains(searchTenantDisplayName));
            }
            if (!string.IsNullOrWhiteSpace(searchTenantName))
            {
                precidate = precidate.And(m => m.TenantName.Contains(searchTenantName));
            }
            if (operationLogType != null)
            {
                precidate = precidate.And(m => (int)m.LogType == operationLogType);
            }
            var model = _tenantUserOperationNewLogRepository.FindPagenatedListWithCount(pageIndex, pageSize, precidate, m => m.OperateDate, order.Equals("asc"));
            var total = model.Item1;
            var rows = _mapper.Map<List<TenantUserOperationLogDTO>>(model.Item2);
            return new PaginatedBaseDTO<TenantUserOperationLogDTO>(pageIndex, pageSize, total, rows);
        }

        public bool AddTenantOperationLog(TenantUserOperationLogDTO model)
        {
            var data = _mapper.Map<TenantUserOperationLog>(model);
            return _tenantUserOperationLogRepository.Add(data);
        }


        #endregion

    }

}
