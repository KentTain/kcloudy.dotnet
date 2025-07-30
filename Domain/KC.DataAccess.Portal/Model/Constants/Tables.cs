using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Portal.Constants
{
    public sealed class Tables
    {
        private const string Prx = "ptl_";

        public const string WebSiteInfo = Prx + "WebSiteInfo";
        public const string WebSiteLink = Prx + "WebSiteLink";
        public const string FavoriteInfo = Prx + "FavoriteInfo";

        public const string WebSitePage = Prx + "WebSitePage";
        public const string WebSiteColumn = Prx + "WebSiteColumn";
        public const string WebSiteItem = Prx + "WebSiteItem";
        public const string WebSitePageLog = Prx + "WebSitePageLog";

        public const string WebSiteTemplateColumn = Prx + "WebSiteTemplateColumn";
        public const string WebSiteTemplateItem = Prx + "WebSiteTemplateItem";

        public const string ArticleCategory = Prx + "ArticleCategory";
        public const string Article = Prx + "Article";

        public const string CompanyInfo = Prx + "CompanyInfo";
        public const string CompanyExtInfo = Prx + "CompanyExtInfo";
        public const string CompanyProcessLog = Prx + "CompanyProcessLog";
        public const string CompanyAddress = Prx + "CompanyAddress";
        public const string CompanyAccount = Prx + "CompanyAccount";
        public const string CompanyContact = Prx + "CompanyContact";

        public const string CompanyAuthentication = Prx + "CompanyAuthentication";
        public const string CompanyAuthenticationFailedRecord = Prx + "CompanyAuthenticationFailedRecord";

        public const string RecommendCategory = Prx + "RecommendCategory";

        public const string RecommendCustomer = Prx + "RecommendCustomer";
        public const string RecommendCustomerLog = Prx + "RecommendCustomerLog";
        

        public const string RecommendOffering = Prx + "RecommendOffering";
        public const string RecommendOfferingLog = Prx + "RecommendOfferingLog";

        public const string RecommendRequirement = Prx + "RecommendRequirement";
        public const string RecommendRequirementLog = Prx + "RecommendRequirementLog";
        public const string RequirementForMaterial = Prx + "RequirementForMaterial";
        public const string RecommendMaterial = Prx + "RecommendMaterial";

        

    }
}
