using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.CodeGenerate.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string Application = "Application";

        public const string ApplicationSetting = "ApplicationSetting";
    }

    public sealed class ActionName : ActionNameBase
    {
        public sealed class Application
        {
            public const string ApplicationList = "ApplicationList";
            public const string LoadApplicationList = "LoadApplicationList";
            public const string GetApplicationForm = "GetApplicationForm";
            public const string SaveApplication = "SaveApplication";
            public const string RemoveApplication = "RemoveApplication";

            public const string LoadModuleList = "LoadModuleList";
            public const string GetModuleForm = "GetModuleForm";
            public const string SaveModule = "SaveModule";
            public const string RemoveModule = "RemoveModule";

            public const string LoadBusinessList = "LoadBusinessList";
            public const string GetBusinessForm = "GetBusinessForm";
            public const string SaveBusiness = "SaveBusiness";
            public const string RemoveBusiness = "RemoveBusiness";


            public const string GetApplicationSelectList = "GetApplicationSelectList";
        }

        public sealed class ApplicationSetting
        {
            public const string LoadPushSettingList = "LoadPushSettingList";
            public const string GetPushSettingForm = "GetPushSettingForm";
            public const string SavePushSetting = "SavePushSetting";
            public const string RemovePushSetting = "RemovePushSetting";

            public const string LoadTargetSettingList = "LoadTargetSettingList";
            public const string EditTargetSetting = "EditTargetSetting";
            public const string SaveTargetSetting = "SaveTargetSetting";
            public const string RemoveTargetSetting = "RemoveTargetSetting";
        }


    }
}