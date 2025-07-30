namespace KC.Model.Customer.Constants
{
    public sealed class Tables
    {
        private const string Prx = "crm_";

        public const string CustomerInfo = Prx  + "CustomerInfo";
        public const string CustomerAuthentication = Prx + "CustomerAuthentication";
        public const string CustomerAccount = Prx + "CustomerAccount";
        public const string CustomerAddress = Prx + "CustomerAddress";
        public const string CustomerContact = Prx + "CustomerContact";

        public const string CustomerExtInfo = Prx + "CustomerExtInfo";
        public const string CustomerExtInfoProvider = Prx + "CustomerExtInfoProvider";

        public const string CustomerTracingLog = Prx + "CustomerTracingLog";
        public const string CustomerChangeLog = Prx + "CustomerChangeLog";

        public const string CustomerActivity = Prx + "CustomerActivity";
        public const string CustomerSendToTenantLog = Prx + "CustomerSendToTenantLog";

        public const string NotificationApplication = Prx + "NotificationApplication";

        public const string CustomerManager = Prx + "CustomerManager";

        public const string CustomerFieldCustom = Prx + "CustomerFieldCustom";
        public const string CustomerSeas = Prx + "CustomerSeas";
        public const string CustomerCallCharge = Prx + "CustomerCallCharge";
    }
}
