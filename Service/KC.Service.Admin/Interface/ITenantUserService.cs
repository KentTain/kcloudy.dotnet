using KC.Enums.App;
using KC.Framework.Tenant;
using KC.Service.DTO.Admin;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KC.Service.Admin
{
    public interface ITenantUserService
    {

        #region Tenant

        [Extension.CachingCallHandler()]
        Tenant GetTenantByName(string tenantName);
        [Extension.CachingCallHandler()]
        Task<Tenant> GetTenantByNameOrNickNameAsync(string tenantName);
        [Extension.CachingCallHandler()]
        Task<Tenant> GetTenantEndWithDomainNameAsync(string domainName);

        List<TenantSimpleDTO> FindTenantUserByDatabasePoolId(int databaseId);
        List<TenantSimpleDTO> FindTenantUserByStoragePoolId(int storagePoolId);
        List<TenantSimpleDTO> FindTenantUserByVodPoolId(int vodPoolId);
        List<TenantSimpleDTO> FindTenantUserByCodePoolId(int codePoolId);

        bool SetNickName(int tenantId, string nickName);

        #endregion

        #region TenantUsers

        bool ExistTenantName(string tenantName);

        List<TenantUserDTO> FindAllNeedInitDbTenantUsers();

        List<TenantSimpleDTO> FindInitalTenants();
        List<TenantSimpleDTO> FindTenantsByNames(List<string> tenantNames);
        List<TenantSimpleDTO> FindOpenAppTenantsByNames(Guid appId, List<string> tenantNames);
        List<TenantSimpleDTO> FindAllOpenAppTenants(Guid appId);

        PaginatedBaseDTO<TenantSimpleDTO> FindTenantUsersByFilter(int pageIndex, int pageSize, int? cloudType, int? version,
            string code, string contact, string userName);
        PaginatedBaseDTO<TenantSimpleDTO> FindTenantUsersByOpenAppId(int pageIndex, int pageSize, Guid applicationId,
            string tenantDisplayName, List<string> exceptTenants = null, bool isShowExceptTenants = false);
        PaginatedBaseDTO<TenantSimpleDTO> FindTenantUsers(int pageIndex, int pageSize,
            string tenantDisplayName, string providerTenantName = null, TenantType? tally = null);
        
        TenantUserDTO GetTenantUserById(int id);
        TenantUserDTO GetTenantUserByName(string tenantName);

        Task<bool> SaveTenantUser(TenantUserDTO model);
        bool UpdateTenantUserBasicInfo(TenantSimpleDTO model);
        bool SoftRemoveTenantUser(List<int> ids);
        bool SoftRemoveTenantUserByNames(List<string> tenantNames);

        bool AddTenantUserApplications(int tenantId, List<Guid> applicationIds);

        bool SaveTenantUserAuthInfo(TenantUserAuthenticationDTO model);
        #endregion

        #region Tenant's Application && Init Tenant's Database & Web Server

        TenantUserApplicationDTO GetDetailApplicationWithModelsByTenantIdAndAppId(int tenantId, Guid appId);
        TenantUserApplicationDTO GetTenantApplicationByFilter(int tenantId, Guid appId);
        List<TenantUserApplicationDTO> FindTenantApplicationsByTenantName(string tenantName);
        List<TenantUserApplicationDTO> FindTenantApplicationsByTenantId(int tenantId);

        List<Guid> FindTenantApplicationIdsByTenantId(int tenantId);
        string SetInitTenantsDbJobAppsStatus();

        bool GeneratDataBaseAndOpenWebServer(int tenantId, string appId);
        string RollBackTenantDataBase(int tenantId, string appId);

        List<string> UpgradeTenantDatabaseByIds(List<int> ids);
        List<string> UpgradeAllTenantDatabase();
        List<string> RollBackAllTenantDatabase();

        string UpgradeTenantDatabase(int tenantId, bool isUpdateStatus = true, bool isNewDatabase = true);
        string RollBackTenantDataBase(int tenantId);

        bool SendTenantAppOpenEmail(TenantUserDTO data);
        void AddOpenAppErrorLog(int tenantId, string tenantDisplayName, OpenServerType type, string error);

        #endregion

        #region Gitlab
        /// <summary>
        /// 创建租户管理员账号及租户分组，并返回管理员账号的token
        /// </summary>
        /// <param name="tenantName">租户编码</param>
        /// <param name="adminPassword">租户管理员密码</param>
        /// <returns></returns>
        Task<string> CreateTenantGitlabGroupAndUser(string tenantName, string adminPassword);
        #endregion

        #region Tenant's Sms & Email & CallCenter Service
        string OpenEmailService(int tenantId, string tenantName);
        string OpenSmsService(int tenantId, string tenantName);
        string OpenCallCenterService(int tenantId, string tenantName);
        #endregion

        #region 平台Tenant推送接口

        Task<bool> SaveSendTenantUsers(List<SendTenantUserDTO> models);

        Task<bool> SaveSendTenantUser(SendTenantUserDTO model);

        bool UpdateTenantUserInfo(SendTenantUserDTO model);

        bool SentUserVersion(SendTenantUserDTO model);
        #endregion

        #region TenantUserOpenAppErrorLog

        PaginatedBaseDTO<TenantUserOpenAppErrorLogDTO> FindOpenAppErrorLogsByFilter(int pageIndex, int pageSize,
            string tenantDisplayName, int? openServerType, string order);
        TenantUserOpenAppErrorLogDTO GetTenantErrorlogFormByProcessLogId(int id);

        bool UpdateTenantErrorLogStatus(int id);
        #endregion

        #region TenantUserOperationLog
        PaginatedBaseDTO<TenantUserOperationLogDTO> FindTenantUserOperationLogByFilter(int pageIndex, int pageSize, string searchTenantDisplayName, string searchTenantName, string order, int? operationLogType);

        bool AddTenantOperationLog(TenantUserOperationLogDTO model);
        #endregion

    }
}
