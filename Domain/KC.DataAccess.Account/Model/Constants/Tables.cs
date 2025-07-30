using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Account.Constants
{
    public sealed class Tables
    {
        private const string Prx = "sys_";

        public const string Role = Prx + "Role";
        public const string User = Prx + "User";
        public const string UsersInRoles = Prx + "UsersInRoles";

        public const string RoleClaim = Prx + "RoleClaim";
        public const string UserLogin = Prx + "UserLogin";
        public const string UserClaim = Prx + "UserClaim";
        public const string UserToken = Prx + "UserToken";


        public const string Organization = Prx + "Organization";
        public const string UsersInOrganizations = Prx + "UsersInOrganizations";

        public const string MenuNode = Prx + "MenuNode";
        public const string MenuNodesInRoles = Prx + "MenuNodesInRoles";

        public const string Permission = Prx + "Permission";
        public const string PermissionsInRoles = Prx + "PermissionsInRoles";

        public const string UserSetting = Prx + "UserSetting";
        public const string UserSettingProperty = Prx + "UserSettingProperty";

        public const string SystemSetting = Prx + "SystemSetting";
        public const string SystemSettingProperty = Prx + "SystemSettingProperty";


        public const string UserTracingLog = Prx + "UserTracingLog";
        public const string UserLoginLog = Prx + "UserLoginLog";
    }
}
