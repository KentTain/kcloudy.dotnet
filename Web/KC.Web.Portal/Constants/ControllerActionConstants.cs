using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.Portal.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string AdvertisementManage = "AdManage";

        public const string CompanyInfo = "CompanyInfo";
        public const string PortalInfo = "PortalInfo";
        public const string RecommendInfo = "RecommendInfo";
        public const string PortalDecorator = "PortalDecorator";
    }

    public sealed class ActionName : ActionNameBase
    {
        public sealed class Home
        {
            public const string LoadProductData = "LoadProductData";
            public const string LoadCompanyData = "LoadCompanyData";
            public const string LoadNewsData = "LoadNewsData";
            public const string LoadRequirementData = "LoadRequirementData";
        }

        public sealed class CompanyInfo
        {
            public const string LoadNewsList = "LoadNewsList";
            public const string Index = "Index";

            public const string SaveCompanyInfo = "SaveCompanyInfo";
            public const string QueryCategoryPath = "QueryCategoryPath";

            public const string CompanyContactList = "CompanyContactList";
            public const string LoadCompanyContactList = "LoadCompanyContactList";
            public const string GetCompanyContactForm = "GetCompanyContactForm";
            public const string SaveCompanyContact = "SaveCompanyContact";
            public const string SaveCompanyContacts = "SaveCompanyContacts";
            public const string RemoveCompanyContact = "RemoveCompanyContact";

            public const string CompanyAddressList = "CompanyAddressList";
            public const string LoadCompanyAddressList = "LoadCompanyAddressList";
            public const string GetCompanyAddressForm = "GetCompanyAddressForm";
            public const string SaveCompanyAddressForm = "SaveCompanyAddressForm";
            public const string RemoveCompanyAddress = "RemoveCompanyAddress";

            public const string CompanyAccountList = "CompanyAccountList";
            public const string LoadCompanyAccountList = "LoadCompanyAccountList";
            public const string GetCompanyAccountForm = "GetCompanyAccountForm";
            public const string SaveCompanyAccountForm = "SaveCompanyAccountForm";
            public const string RemoveCompanyAccount = "RemoveCompanyAccount";

            public const string ExistCompanyName = "ExistCompanyName";
            public const string SaveComAuthentication = "SaveComAuthentication";

        }

        public sealed class PortalInfo
        {
            public const string ExistPortalName = "ExistPortalName";
            public const string SaveWebSiteInfo = "SaveWebSiteInfo";

            public const string ExistCategoryName = "ExistCategoryName";
            public const string LoadRecommendCategoryTree = "LoadRecommendCategoryTree";
            public const string RemoveRecommendCategory = "RemoveRecommendCategory";
            public const string GetRecommendCategory = "GetRecommendCategory";
            public const string SaveRecommendCategory = "SaveRecommendCategory";
            public const string FindRecommendInfos = "FindRecommendInfos";

            public const string LoadWebSiteLinks = "LoadWebSiteLinks";
            public const string GetWebSiteLinkForm = "GetWebSiteLinkForm";
            public const string SaveWebSiteLink = "SaveWebSiteLink";
            public const string DeleteWebSiteLink = "DeleteWebSiteLink";

            public const string SkinManage = "SkinManage";
            public const string AddSkin = "AddSkin";
            public const string QueryAdsToSaas = "QueryAdsToSaas";
        }

        public sealed class RecommendInfo
        {
            public const string RedCustomerList = "RedCustomerList";
            public const string LoadRedCustomerList = "LoadRedCustomerList";
            public const string GetRedCustomerForm = "GetRedCustomerForm";
            public const string SaveRedCustomer = "SaveRedCustomer";
            public const string RemoveRedCustomer = "RemoveRedCustomer";
            public const string AuditRedCustomer = "AuditRedCustomer";
            public const string TakeOffRedCustomer = "TakeOffRedCustomer";

            public const string RecommendCustomerLog = "RecommendCustomerLog";
            public const string LoadRecommendCustomerLogList = "LoadRecommendCustomerLogList";

            public const string RedOfferingList = "RedOfferingList";
            public const string LoadRedOfferingList = "LoadRedOfferingList";
            public const string GetRedOfferingForm = "GetRedOfferingForm";
            public const string SaveRedOffering = "SaveRedOffering";
            public const string RemoveRedOffering = "RemoveRedOffering";
            public const string AuditRedOffering = "AuditRedOffering";
            public const string TakeOffRedOffering = "TakeOffRedOffering";

            public const string RecommendOfferingLog = "RecommendOfferingLog";
            public const string LoadRecommendOfferingLogList = "LoadRecommendOfferingLogList";


            public const string RedRequirementList = "RedRequirementList";
            public const string LoadRedRequirementList = "LoadRedRequirementList";
            public const string GetRedRequirementForm = "GetRedRequirementForm";
            public const string SaveRedRequirement = "SaveRedRequirement";
            public const string RemoveRedRequirement = "RemoveRedRequirement";
            public const string AuditRedRequirement = "AuditRedRequirement";
            public const string TakeOffRedRequirement = "TakeOffRedRequirement";

            public const string RecommendRequirementLog = "RecommendRequirementLog";
            public const string LoadRecommendRequirementLogList = "LoadRecommendRequirementLogList";
        }

        public sealed class PortalDecorator
        {
            public const string SavePortalSkin = "SavePortalSkin";

            public const string PortalPageList = "PortalPageList";
            public const string LoadWebSitePageList = "LoadWebSitePageList";
            public const string LoadWebSiteColumnList = "LoadWebSiteColumnList";
            public const string GetWebSitePageForm = "GetWebSitePageForm";
            public const string SaveWebSitePage = "SaveWebSitePage";
            public const string ChangeWebSitePageStatus = "ChangeWebSitePageStatus";
            public const string RemoveWebSitePage = "RemoveWebSitePage";

            public const string ChangeWebSiteColumnStatus = "ChangeWebSiteColumnStatus";
            public const string RemoveWebSiteColumn = "RemoveWebSiteColumn";

            public const string LoadWebSiteItemList = "LoadWebSiteItemList";
            public const string RemoveWebSiteItem = "RemoveWebSiteItem";
        }
    }
}