using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.Admin.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string MainPage = "MainPage";
        public const string Tenant = "Tenant";
        public const string Application = "Application";

        public const string CacheManager = "CacheManager";
        public const string NLog = "NLog";
        public const string QueueErrorMessage = "QueueErrorMessage";
        public const string WorkOrder = "WorkOrder";

        public const string DatabasePool = "DatabasePool";
        public const string StoragePool = "StoragePool";
        public const string QueuePool = "QueuePool";
        public const string NoSqlPool = "NoSqlPool";
        public const string VodPool = "VodPool";
        public const string CodePool = "CodePool";
        public const string ServiceBusPool = "ServiceBusPool";

    }

    public sealed class ActionName : ActionNameBase
    {
        public sealed class QueueErrorMessage
        {
            public const string GetQueueErrorMessageJson = "GetQueueErrorMessageJson";
            public const string DeleteQueueErrorMessage = "DeleteQueueErrorMessage";
            public const string ResetQueue = "ResetQueue";
        }

        public sealed class NLog
        {
            public const string GetNLogJson = "GetNLogJson";
            public const string DeleteNLog = "DeleteNLog";


        }
        public sealed class Tenant
        {
            public const string TenantUserList = "TenantUserList";
            public const string LoadTenantUserList = "LoadTenantUserList";
            public const string ExistTenantName = "ExistTenantName";
            public const string TenantDetail = "TenantDetail";

            public const string GetTenantUserForm = "GetTenantUserForm";
            public const string SaveTenantUserForm = "SaveTenantUserForm";
            public const string RemoveTenantUser = "RemoveTenantUser";
            public const string RemoveTenantUserByName = "RemoveTenantUserByName";
            public const string InitAllTenantWebServer = "InitAllTenantWebServer";
            public const string RollBackAllTenantDatabase = "RollBackAllTenantDatabase";

            public const string SavaTenantUserApplications = "SavaTenantUserApplications";
            public const string TestTenantDBConnection = "TestTenantDBConnection";
            public const string TestTenantStorageConnection = "TestTenantStorageConnection";

            public const string LoadTenantAppList = "LoadTenantAppList";
            public const string GetApplicationServiceForm = "GetApplicationServiceForm";
            public const string OpenApplicationService = "OpenApplicationService";
            public const string RollBackDatabaseService = "RollBackDatabaseService";

            public const string ModifyPhoneAndEmailForm = "ModifyPhoneAndEmailForm";
            public const string ModifyTenantUserContactPhone = "ModifyTenantUserContactPhone";
            public const string ModifyTenantUserContactEmail = "ModifyTenantUserContactEmail";

            public const string CheckExistsApps = "CheckExistsApps";
            public const string GetTenantAppStatus = "GetTenantAppStatus";

            public const string GeneratePasswordHash = "GeneratePasswordHash";

            public const string OpenSmsService = "OpenSmsService";
            public const string OpenEmailService = "OpenEmailService";
            public const string OpenCallCenterService = "OpenCallCenterService";

            public const string UpgradeTenantDatabase = "UpgradeTenantDatabase";
            public const string RollBackTenantDataBase = "RollBackTenantDataBase";
            public const string ReSendOpenAppEmail = "ReSendOpenAppEmail";

            public const string SyncData = "SyncData";
            public const string SyncAllData = "SyncAllData";
            public const string SendOpenEmail = "SendOpenEmail";
            public const string SendAllOpenEmail = "SendAllOpenEmail";
            public const string SendOpenSms = "SendOpenSms";

        }
        public sealed class Application
        {
            public const string ApplicationList = "ApplicationList";
            public const string LoadApplicationList = "LoadApplicationList";
            public const string GetApplicationForm = "GetApplicationForm";
            public const string SaveApplicationForm = "SaveApplicationForm";
            public const string RemoveApplication = "RemoveApplication";

            public const string LoadModuleList = "LoadModuleList";
            public const string GetModuleForm = "GetModuleForm";
            public const string SaveModuleForm = "SaveModuleForm";
            public const string RemoveModule = "RemoveModule";

            public const string GetApplicationSelectList = "GetApplicationSelectList";
        }
        public sealed class CacheManager
        {
            public const string LoadCacheList = "LoadCacheList";
            public const string RemoveCache = "RemoveCache";
            public const string RemoveAllCache = "RemoveAllCache";
        }

        public sealed class DatabasePool
        {
            public const string DatabasePoolList = "DatabasePoolList";
            public const string LoadDatabasePoolList = "LoadDatabasePoolList";
            public const string LoadTenantUserListByDatabasePool = "LoadTenantUserListByDatabasePool";
            public const string GetDatabasePoolForm = "GetDatabasePoolForm";
            public const string SaveDatabasePoolForm = "SaveDatabasePoolForm";
            public const string RemoveDatabasePool = "RemoveDatabasePool";
            public const string TestDatabaseConnection = "TestDatabaseConnection";
        }

        public sealed class StoragePool
        {
            public const string StoragePoolList = "StoragePoolList";
            public const string LoadStoragePoolList = "LoadStoragePoolList";
            public const string LoadTenantUserListByStoragePool = "LoadTenantUserListByStoragePool";
            public const string GetStoragePoolForm = "GetStoragePoolForm";
            public const string SaveStoragePoolForm = "SaveStoragePoolForm";
            public const string RemoveStoragePool = "RemoveStoragePool";
            public const string TestStorageConnection = "TestStorageConnection";
        }

        public sealed class QueuePool
        {
            public const string QueuePoolList = "QueuePoolList";
            public const string LoadQueuePoolList = "LoadQueuePoolList";
            public const string GetQueuePoolForm = "GetQueuePoolForm";
            public const string SaveQueuePoolForm = "SaveQueuePoolForm";
            public const string RemoveQueuePool = "RemoveQueuePool";
            public const string TestQueueConnection = "TestQueueConnection";
        }

        public sealed class NoSqlPool
        {
            public const string NoSqlPoolList = "NoSqlPoolList";
            public const string LoadNoSqlPoolList = "LoadNoSqlPoolList";
            public const string GetNoSqlPoolForm = "GetNoSqlPoolForm";
            public const string SaveNoSqlPoolForm = "SaveNoSqlPoolForm";
            public const string RemoveNoSqlPool = "RemoveNoSqlPool";
            public const string TestNoSqlConnection = "TestNoSqlConnection";
        }

        public sealed class VodPool
        {
            public const string VodPoolList = "VodPoolList";
            public const string LoadVodPoolList = "LoadVodPoolList";
            public const string LoadTenantUserListByVodPool = "LoadTenantUserListByVodPool";
            public const string GetVodPoolForm = "GetVodPoolForm";
            public const string SaveVodPoolForm = "SaveVodPoolForm";
            public const string RemoveVodPool = "RemoveVodPool";
            public const string TestVodConnection = "TestVodConnection";
        }

        public sealed class CodePool
        {
            public const string CodeRepositoryPoolList = "CodeRepositoryPoolList";
            public const string LoadCodeRepositoryPoolList = "LoadCodeRepositoryPoolList";
            public const string LoadTenantUserListByCodePool = "LoadTenantUserListByCodePool";
            public const string GetCodeRepositoryPoolForm = "GetCodeRepositoryPoolForm";
            public const string SaveCodeRepositoryPoolForm = "SaveCodeRepositoryPoolForm";
            public const string RemoveCodeRepositoryPool = "RemoveCodeRepositoryPool";
            public const string TestCodeConnection = "TestCodeConnection";
        }

        public sealed class ServiceBusPool
        {
            public const string ServiceBusPoolList = "ServiceBusPoolList";
            public const string LoadServiceBusPoolList = "LoadServiceBusPoolList";
            public const string GetServiceBusPoolForm = "GetServiceBusPoolForm";
            public const string SaveserviceBusPoolForm = "SaveserviceBusPoolForm";
            public const string DeleteServiceBusPool = "DeleteServiceBusPool";
            public const string TestServiceBusPoolConnection = "TestServiceBusPoolConnection";
            
        }
    }
}