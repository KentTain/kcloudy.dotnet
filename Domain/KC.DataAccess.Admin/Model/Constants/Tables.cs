namespace KC.Model.Admin.Constants
{
    public sealed class Tables
    {
        private const string Prx = "tenant_";

        public const string DatabasePool = Prx + "DatabasePool";
        public const string StoragePool = Prx + "StoragePool";
        public const string QueuePool = Prx + "QueuePool";
        public const string NoSqlPool = Prx + "NoSqlPool";
        public const string VodPool = Prx + "VodPool";
        public const string ServiceBusPool = Prx + "ServiceBusPool";
        public const string CodeRepositoryPool = Prx + "CodeRepositoryPool";

        public const string DevTemplate = Prx + "DevTemplate";
        

        public const string TenantUser = Prx + "TenantUser";
        public const string TenantUserAuthentication = Prx + "TenantUserAuthentication";
        public const string TenantSetting = Prx + "TenantUserSetting";
        public const string TenantUserApplication = Prx + "TenantUserApplication";
        public const string TenantUserAppModule = Prx + "TenantUserAppModule";
        public const string TenantUserOperationLog = Prx + "TenantUserOperationLog";

        public const string TenantUserOffering = Prx + "TenantUserOffering";
        public const string TenantUserMaterial = Prx + "TenantUserMaterial";
        public const string TenantUserRequirement = Prx + "TenantUserRequirement";
        public const string RequirementForMaterial = Prx + "RequirementForMaterial";

        public const string TenantUserChargeSms = Prx + "TenantUserChargeSms";
        public const string TenantUserChargeStorage = Prx + "TenantUserChargeStorage";


        public const string TenantUserOpenAppErrorLog = Prx + "TenantUserOpenAppErrorLog";
        public const string TenantUserServiceApplication = Prx + "TenantUserServiceApplication";
        public const string TenantUserLoopTask = Prx + "TenantUserLoopTask";

    }
}
