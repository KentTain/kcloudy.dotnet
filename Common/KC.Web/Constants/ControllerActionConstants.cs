using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Web.Constants
{
    public class ControllerNameBase
    {
        public const string Home = "Home";
        public const string Account = "Account";

        public static List<string> DbaOwnControllers = new List<string>() { "tenant", "cachemanagement", "queueerrormessage" }; 
    }

    public class ActionNameBase
    {
        public const string Index = "Index";
        public const string Detail = "Detail";
        public const string Download = "Download";
        public const string ClearAllCache = "ClearAllCache";
        
        public const string UploadFileToTemp = "UploadFileToTemp";
        public const string ChunkCheck = "ChunkCheck";
        public const string ChunksMerge = "ChunksMerge";

        public const string LoadMenus = "LoadMenus";
        public const string LoadNewsBulletins = "LoadNewsBulletins";
        public const string LoadMessages = "LoadMessages";
        public const string ReadMessage = "ReadMessage";
        public const string LoadWorkflowTasks = "LoadWorkflowTasks";
        public const string ExecuteTask = "ExecuteTask";
        public const string GetChartData = "GetChartData";

        public const string GetMenuIdByUrl = "GetMenuIdByUrl";

        public const string ChangePassword = "ChangePassword";
        public const string ChangeMailPhone = "ChangeMailPhone";

        public const string SelectLocationPartial = "_SelectLocationPartial";
        public const string LoadProvinceList = "LoadProvinceList";
        public const string LoadCityList = "LoadCityList";
        public const string LoadCitiesByProvinceId = "LoadCitiesByProvinceId";

        public const string SelectIconPartial = "_SelectIconPartial";
        public const string SelectUserPartial = "_SelectUserPartial";
        public const string GetRootOrganizationsWithUsers = "GetRootOrganizationsWithUsers";
        public const string GetOrgsWithUsersByRoleIdsAndOrgids = "GetOrgsWithUsersByRoleIdsAndOrgids";

        public const string SelectOrganizationPartial = "_SelectOrganizationPartial";
        public const string GetRootOrganizations = "GetRootOrganizations";

        public const string SelectIndustryClassficationPartial = "_SelectIndustryClassficationPartial";
        public const string GetRootIndustryClassfications = "GetRootIndustryClassfications";

        public const string InitTenantIISHostWithWebName = "InitTenantIISHostWithWebName";

       
        public sealed class Home
        {
            public const string GetHomeContentJson = "GetHomeContentJson";
            public const string RemoveHomeContent = "RemoveHomeContent";
            public const string SaveHomeContentForm = "SaveHomeContentForm";
            public const string GetHomeContentForm = "GetHomeContentForm";
            public const string GetCount = "GetCount";
            
            public const string GetMessage = "GetMessage";
            public const string CheckChangePwd = "CheckChangePwd";
            public const string DeleteMessages="DeleteMessages";
            public const string DeleteOneMessage = "DeleteOneMessage";
            public const string RemoveOneMessageToRead = "RemoveOneMessageToRead";
            public const string RemoveMessagesToRead = "RemoveMessagesToRead";
        }
    }
}
