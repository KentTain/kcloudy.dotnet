using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Database.EFRepository;
using KC.Service.EFService;
using KC.DataAccess.App.Repository;
using KC.Service.DTO.App;
using KC.Service.DTO;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using KC.Service;
using KC.Model.App;
using KC.Framework.Extension;
using KC.Database.IRepository;
using KC.Service.WebApiService.Business;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.GitLabApiClient;
using KC.GitLabApiClient.Models.Projects.Responses;
using KC.GitLabApiClient.Models.Projects.Requests;
using System.Text.RegularExpressions;
using KC.Framework.Exceptions;

namespace KC.Service.App
{
    public interface IApplicationService : IEFService
    {
        #region 应用管理

        Task<List<ApplicationDTO>> FindAllApplications(string name);

        Task<PaginatedBaseDTO<ApplicationDTO>> FindPaginatedApplications(int pageIndex, int pageSize, string name, bool isDeleted);

        Task<ApplicationDTO> GetApplicationById(Guid id);
        Task<ApplicationDTO> GetApplicationWithSettingsById(Guid id);

        Task<bool> ExistAppCode(Guid? id, string code);
        Task<bool> ExistAppName(Guid? id, string name);
        Task<bool> SaveApplication(ApplicationDTO data);

        Task<bool> RemoveApplication(Guid id);
        Task<bool> RecoverApplication(Guid id);
        bool UpdateTenantWebDomain(List<OpenApplicatin> openApps);

        bool EnableWorkFlow(Guid id, bool type);
        //[CachingCallHandler(TimeOutConstants.AccessTokenTimeOut, Order = 0)]
        bool GetIsEnabledWorkFlow(Guid id);
        #endregion

        #region Gitlab

        Task<bool> CreateAppGitProject(string appCode);
        Task<bool> DeleteAppGitProject(string appCode);
        #endregion

        #region 应用日志
        PaginatedBaseDTO<ApplicationLogDTO> FindPaginatedApplicationLogs(int pageIndex, int pageSize, string name);
        #endregion
    }

    public class ApplicationService : EFServiceBase, IApplicationService
    {
        #region Context && Repository
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        private EFUnitOfWorkContextBase _unitOfContext;

        private IApplicationRepository _applicationRepository;
        private IDbRepository<ApplicationLog> _applicationLogRepository;
        private readonly IConfigApiService ConfigApiService;

        public ApplicationService(
            Tenant tenant,
            IConfigApiService configApiService,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            IApplicationRepository applicationRepository,
            IDbRepository<ApplicationLog> applicationLogRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<ApplicationService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfContext = unitOfContext;
            ConfigApiService = configApiService;

            _applicationRepository = applicationRepository;

            _applicationLogRepository = applicationLogRepository;
        }
        #endregion

        #region 应用管理

        public async Task<List<ApplicationDTO>> FindAllApplications(string name)
        {
            Expression<Func<Application, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.ApplicationName.Contains(name));
            }
            var data = await _applicationRepository.FindAllAsync(predicate);
            return _mapper.Map<List<ApplicationDTO>>(data);
        }

        public async Task<PaginatedBaseDTO<ApplicationDTO>> FindPaginatedApplications(int pageIndex, int pageSize, string name, bool isDeleted)
        {
            Expression<Func<Application, bool>> predicate = m => m.IsDeleted == isDeleted;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.ApplicationName.Contains(name));
            }
            //var dbaTenant = TenantUtil.GetDbaTenantUser();
            //var applicationRepository = new ApplicationRespository(dbaTenant);
            var data = await _applicationRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.CreatedDate, true);
            var total = data.Item1;
            var rows = _mapper.Map<List<ApplicationDTO>>(data.Item2);
            return new PaginatedBaseDTO<ApplicationDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<ApplicationDTO> GetApplicationById(Guid id)
        {
            var data = await _applicationRepository.GetByIdAsync(id);
            return _mapper.Map<ApplicationDTO>(data);
        }
        public async Task<ApplicationDTO> GetApplicationWithSettingsById(Guid id)
        {
            var data = await _applicationRepository.GetApplicationWithSettingsById(id);
            return _mapper.Map<ApplicationDTO>(data);
        }

        public async Task<bool> ExistAppCode(Guid? id, string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("ApplicationCode", "应用编码为空.");

            var existSysApp = GlobalConfig.Applications.Any(m => m.AppId == id.Value && m.AppCode.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (existSysApp)
                return true;

            Expression<Func<Application, bool>> predicate = m => !m.IsDeleted && m.ApplicationCode.Equals(code);
            if (id.HasValue)
                predicate = predicate.And(c => c.ApplicationId != id.Value);

            return await _applicationRepository.ExistByFilterAsync(predicate);
        }

        public async Task<bool> ExistAppName(Guid? id, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("ApplicationName", "应用名称为空.");

            var existSysApp = GlobalConfig.Applications.Any(m => m.AppId == id.Value && m.AppName.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existSysApp)
                return true;

            Expression<Func<Application, bool>> predicate = m => !m.IsDeleted && m.ApplicationName.Equals(name);
            if (id.HasValue)
                predicate = predicate.And(c => c.ApplicationId != id.Value);

            return await _applicationRepository.ExistByFilterAsync(predicate);
        }

        public async Task<bool> SaveApplication(ApplicationDTO data)
        {
            var existCode = await ExistAppCode(data.ApplicationId, data.ApplicationCode);
            if (existCode)
                throw new ArgumentNullException("ApplicationCode", "应用编码已存在.");

            var existName = await ExistAppName(data.ApplicationId, data.ApplicationCode);
            if (existName)
                throw new ArgumentNullException("ApplicationName", "应用名称已存在.");

            var model = _mapper.Map<Application>(data);
            if (model.ApplicationId == Guid.Empty)
            {
                model.ApplicationId = Guid.NewGuid();
                return await _applicationRepository.AddAsync(model);
            }
            else
            {
                return await _applicationRepository.ModifyAsync(model, true);
            }
        }


        public async Task<bool> RemoveApplication(Guid id)
        {
            var data = await _applicationRepository.GetByIdAsync(id);
            if (null == data)
                return true;

            data.IsDeleted = true;
            data.ApplicationCode = data.ApplicationCode + "_" + data.ApplicationId + "_deleted";
            return await _applicationRepository.ModifyAsync(data, new string[] { "IsDeleted", "ApplicationCode" });
        }
        public async Task<bool> RecoverApplication(Guid id)
        {
            var data = await _applicationRepository.GetByIdAsync(id);
            if (null == data)
                return true;

            data.IsDeleted = false;
            data.ApplicationCode = data.ApplicationCode.Split("_").FirstOrDefault();
            return await _applicationRepository.ModifyAsync(data, new string[] { "IsDeleted", "ApplicationCode" });
        }

        public bool UpdateTenantWebDomain(List<OpenApplicatin> openApps)
        {
            var appIds = openApps.Select(m => m.ApplicationId).ToList();
            var apps = _applicationRepository.FindAll(m => appIds.Contains(m.ApplicationId));
            foreach (var app in apps)
            {
                app.DomainName = openApps.First(m => m.ApplicationId == app.ApplicationId).DomainName;
                app.IsDeleted = !openApps.Any(m => m.ApplicationId == app.ApplicationId);
            }

            return _applicationRepository.Modify(apps, new[] { "DomainName", "IsDeleted" }) > 0;
        }


        public bool EnableWorkFlow(Guid id, bool type)
        {
            var result = _applicationRepository.GetById(id);
            if (result != null)
            {
                result.IsEnabledWorkFlow = type;
                string key = base.Tenant.TenantName + "-" + result.ApplicationId + "-IsEnabledWorkFlow";
                CacheUtil.RemoveCache(key);
                return _applicationRepository.Modify(result, new[] { "IsEnabledWorkFlow" }, true);
            }
            Logger.LogError("没找到应用编号");
            return false;
        }
        public bool GetIsEnabledWorkFlow(Guid id)
        {
            string key = base.Tenant.TenantName + "-" + id + "-IsEnabledWorkFlow";
            var result = CacheUtil.GetCache<ApplicationDTO>(key);
            if (result != null) return result.IsEnabledWorkFlow;
            var temp = _applicationRepository.GetById(id);
            if (temp != null)
            {
                CacheUtil.SetCache(key, _mapper.Map<ApplicationDTO>(temp));
                return temp.IsEnabledWorkFlow;
            }
            Logger.LogError("没找到应用编号");
            return false;
        }

        #endregion

        #region Gitlab

        private static string GIT_EXT = ".git";
        private static string DEFAULT_BRANCH = "main";
        private static string TENANT_GROUP_NAME = "tenant";
        /// <summary>
        /// 获取配置的Gitlab域名，例如：http://gitlab.kcloudy.com
        /// </summary>
        /// <returns>Gitlab域名</returns>
        /// <exception cref="BusinessException"></exception>
        private string GetGitlabHostUrl()
        {
            var codeConnectString = GlobalConfig.GetDecryptCodeConnectionString();
            if (string.IsNullOrEmpty(codeConnectString))
                throw new BusinessException("未配置平台租户管理员的Gitlab相关连接字符串.");
            return StringExtensions.GetValueFromConnectionString(codeConnectString, ConnectionKeyConstant.CodeEndpoint);
        }
        /// <summary>
        /// 获取项目在gitlab中的地址，例如：http://gitlab.kcloudy.com/tenant/cTest/cTest-appCode.git
        /// </summary>
        /// <param name="appCode">应用编码</param>
        /// <returns>项目在gitlab中的地址</returns>
        /// <exception cref="BusinessException"></exception>
        private string GetTenantProjectGitlabUrl(string appCode)
        {
            var gitlabHostUrl = GetGitlabHostUrl();
            return string.Format("{0}/{1}/{2}/{3}.{4}", gitlabHostUrl, TENANT_GROUP_NAME, Tenant.TenantName, Tenant.TenantName + "-" + appCode, GIT_EXT);
        }
        /// <summary>
        /// 获取项目在本地克隆的地址，例如：/temp/cTest-appCode
        /// </summary>
        /// <param name="appCode">应用编码</param>
        /// <returns></returns>
        private string GetTenantProjectLocalPath(string appCode)
        {
            var tempPath = GlobalConfig.TempFilePath;
            return string.Format("%s/%s", tempPath, Tenant.TenantName + "-" + appCode);
        }
        /// <summary>
        /// 获取访问Gitlab api的客户端
        /// </summary>
        /// <returns>Gitlab api的客户端</returns>
        private GitLabClient GetAdminGitlabClient()
        {
            var codeConnectString = GlobalConfig.GetDecryptCodeConnectionString();
            var gitHostUrl = StringExtensions.GetValueFromConnectionString(codeConnectString, ConnectionKeyConstant.CodeEndpoint);
            var admin_token = StringExtensions.GetValueFromConnectionString(codeConnectString, ConnectionKeyConstant.AccessKey);

            return new GitLabClient(gitHostUrl, admin_token);
        }
        /// <summary>
        /// 获取租户设置的访问Gitlab api的客户端
        /// </summary>
        /// <returns>Gitlab api的客户端</returns>
        private GitLabClient GetTenantGitlabClient()
        {
            var codeConnectString = Tenant.GetDecryptCodeConnectionString();
            var gitHostUrl = StringExtensions.GetValueFromConnectionString(codeConnectString, ConnectionKeyConstant.CodeEndpoint);
            var admin_token = StringExtensions.GetValueFromConnectionString(codeConnectString, ConnectionKeyConstant.AccessKey);

            return new GitLabClient(gitHostUrl, admin_token);
        }
        
        public async Task<bool> CreateAppGitProject(string appCode)
        {
            var tenant_group_name = Tenant.TenantName;
            var tenant_project_name = Tenant.TenantName + "-" + appCode;

            //使用新用户Token创建租户分组及项目
            var tenant_gitLabApi = GetTenantGitlabClient();

            //判断Gitlab下的租户分组是否存在
            var tenantGroupPath = TENANT_GROUP_NAME + "/" + tenant_group_name;
            var tenantGroup = await tenant_gitLabApi.Groups.GetAsync(tenantGroupPath);
            if (null == tenantGroup)
                throw new ComponentException($"未找到租户的Gitlab相关分组【{tenant_group_name}】的值");

            //创建gitlab项目
            var projectPath = tenantGroupPath + "/" + tenant_project_name;
            var searchProject = new Project() { PathWithNamespace = projectPath };
            Project tenantProject = null;
            try
            {
                tenantProject = await tenant_gitLabApi.Projects.GetAsync(searchProject);
            }
            catch (GitLabException e)
            {
                //未找到Gitlab项目，gitlab client api 会抛出404异常
                var status = e.HttpStatusCode;
                if (status != System.Net.HttpStatusCode.NotFound)
                    throw new ComponentException("创建gitlab项目失败", e);
            }
            if (null == tenantProject)
            {
                var projectRequest = CreateProjectRequest.FromName(tenant_project_name);
                projectRequest.NamespaceId = tenantGroup.Id;
                projectRequest.DefaultBranch = DEFAULT_BRANCH;
                projectRequest.Visibility = ProjectVisibilityLevel.Private;
                tenantProject = await tenant_gitLabApi.Projects.CreateAsync(projectRequest);
                Logger.LogInformation($"----租户【{Tenant.TenantName}】 根据应用【{appCode}】创建了新的Gitlab项目: {tenantProject.Name}");

                //将租户管理员用户加入gitlab的分组中
                //var memberRequest = new AddProjectMemberRequest(AccessLevel.Maintainer, tenantAdminUser);
                //await tenant_gitLabApi.Projects.AddMemberAsync(newProject, memberRequest);
            }

            return true;
        }

        public async Task<bool> DeleteAppGitProject(string appCode)
        {
            var tenant_group_name = Tenant.TenantName;
            var tenant_project_name = Tenant.TenantName + "-" + appCode;
            var gitUrl = "http://gitlab.kcloudy.com/demo/net/kcloudy.demotest.net.git";

            var gitHostUrl = StringExtensions.GetHostUrl(gitUrl);
            var gitLabAdminApi = new GitLabClient(gitHostUrl, "uEjxNXLsit9P5KyZySxy");

            //创建gitlab项目
            string projectPath = TENANT_GROUP_NAME + "/" + tenant_group_name + "/" + tenant_project_name;
            Project newProject = await gitLabAdminApi.Projects.GetAsync(projectPath);
            if (null == newProject)
            {
                return false;
            }

            await gitLabAdminApi.Projects.DeleteAsync(newProject);
            Logger.LogInformation("----Delete the project: " + newProject.Name);
            return true;
        }
        #endregion

        #region 应用日志
        public PaginatedBaseDTO<ApplicationLogDTO> FindPaginatedApplicationLogs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<ApplicationLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _applicationLogRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.OperateDate, false);

            var total = data.Item1;
            var rows = _mapper.Map<List<ApplicationLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<ApplicationLogDTO>(pageIndex, pageSize, total, rows);

        }
        #endregion
    }
}
