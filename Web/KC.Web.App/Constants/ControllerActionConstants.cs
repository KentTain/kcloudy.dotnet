using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.App.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string Application = "Application";

        public const string AppSetting = "AppSetting";
        public const string DevSetting = "DevSetting";
    }

    public sealed class ActionName : ActionNameBase
    {
        public sealed class Application
        {
            public const string ApplicationList = "ApplicationList";
            public const string LoadAllApplications = "LoadAllApplications";
            public const string LoadApplicationList = "LoadApplicationList";
            public const string GetApplicationForm = "GetApplicationForm";
            public const string ExistAppCode = "ExistAppCode";
            public const string ExistAppName = "ExistAppName";
            public const string SaveApplication = "SaveApplication";
            public const string RemoveApplication = "RemoveApplication";
            public const string enableWorkFlow = "enableWorkFlow";

            
            public const string AppSettingForm = "AppSettingForm";
            public const string SaveAppSetting = "SaveAppSetting";
            

            public const string AppRecycle = "AppRecycle";
            public const string LoadAppRecycleList = "LoadAppRecycleList";
            public const string RecoverApplication = "RecoverApplication";

            public const string LoadApplicationLogList = "LoadApplicationLogList";

            public const string GetApplicationSelectList = "GetApplicationSelectList";
        }

        public sealed class AppSetting
        {
            public const string LoadAllAppSettingList = "LoadAllAppSettingList";
            public const string LoadAppSettingList = "LoadAppSettingList";
            public const string GetAppSetDefForm = "GetAppSetDefForm";
            public const string ExistAppSetCode = "ExistAppSetCode";
            public const string ExistAppSetName = "ExistAppSetName";
            public const string SaveAppSetting = "SaveAppSetting";
            public const string RemoveAppSetting = "RemoveAppSetting";

            public const string LoadAppSettingPropertyList = "LoadAppSettingPropertyList";
            public const string GetAppSetPropDefForm = "GetAppSetPropDefForm";
            public const string ExistAppSetPropName = "ExistAppSetPropName";
            public const string SaveAppSettingProperty = "SaveAppSettingProperty";
            public const string RemoveAppSettingProperty = "RemoveAppSettingProperty";
        }


        public sealed class DevSetting
        {
            public const string AppGitList = "AppGitList";
            public const string LoadAllAppGitList = "LoadAllAppGitList";
            public const string LoadAppGitList = "LoadAppGitList";
            public const string AppGitDetail = "AppGitDetail";
            public const string SaveAppGit = "SaveAppGit";
            public const string EnableAppGit = "EnableAppGit";

            public const string AppGitBranchList = "AppGitBranchList";
            public const string LoadAllAppGitBranchList = "LoadAllAppGitBranchList";
            public const string LoadAppGitBranchList = "LoadAppGitBranchList";
            public const string GetAppGitBranchForm = "GetAppGitBranchForm";
            public const string SaveAppGitBranch = "SaveAppGitBranch";
            public const string RemoveAppGitBranch = "RemoveAppGitBranch";

            public const string AppGitUserList = "AppGitUserList";
            public const string LoadAllAppGitUserList = "LoadAllAppGitUserList";
            public const string LoadAppGitUserList = "LoadAppGitUserList";
            public const string GetAppGitUserForm = "GetAppGitUserForm";
            public const string SaveAppGitUser = "SaveAppGitUser";
            public const string RemoveAppGitUser = "RemoveAppGitUser";

            public const string AppTemplateList = "AppTemplateList";
            public const string LoadAppTemplateList = "LoadAppTemplateList";
            public const string LoadAllTemplateList = "LoadAllTemplateList";
            public const string GetAppTemplateForm = "GetAppTemplateForm";
            public const string ExistTemplateName = "ExistTemplateName";
            public const string SaveAppTemplate = "SaveAppTemplate";
            public const string RemoveAppTemplate = "RemoveAppTemplate";

        }
    }
}