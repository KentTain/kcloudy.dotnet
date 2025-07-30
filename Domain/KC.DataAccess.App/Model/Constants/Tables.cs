using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.App.Constants
{
    public sealed class Tables
    {
        private const string Prx = "app_";

        public const string Application = Prx + "Application";
        public const string ApplicationLog = Prx + "ApplicationLog";
        public const string AppSetting = Prx + "AppSetting";
        public const string AppSettingProperty = Prx + "AppSettingProperty";

        public const string DevTemplate = Prx + "DevTemplate";

        public const string AppGit = Prx + "AppGit";
        public const string AppGitBranch = Prx + "AppGitBranch";
        public const string AppGitUser = Prx + "AppGitUser";


        public const string RelationDefinition = Prx + "RelationDefinition";
        public const string RelationDefDetail = Prx + "RelationDefDetail";

        public const string ApplicationModule = Prx + "ApplicationModule";
        public const string ApplicationBusiness = Prx + "ApplicationBusiness";

        public const string AppApiPushLog = Prx + "AppApiPushLog";
        public const string AppTargetApiSetting = Prx + "AppTargetApiSetting";
        public const string AppApiPushSetting = Prx + "AppApiPushSetting";
        public const string PushMapping = Prx + "PushMapping";
        public const string SecurityParameter = Prx + "SecurityParameter";

    }
}
