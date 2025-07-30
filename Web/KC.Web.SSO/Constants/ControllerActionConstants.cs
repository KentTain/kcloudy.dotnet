using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.SSO.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        #region Account
        public const string Application = "Application";
        public const string Menu = "Menu";
        public const string Permission = "Permission";
        public const string Organization = "Organization";
        public const string User = "User";
        public const string Role = "Role";

        public const string ConfigManager = "ConfigManager";
        public const string SysSequence = "SysSequence";
        public const string Message = "Message";
        public const string Dictionary = "Dictionary";
        #endregion

    }

    public sealed class ActionName : ActionNameBase
    {
        #region Account
        public sealed class Account
        {
            public const string Login = "Login";
            public const string Logout = "Logout";
            public const string Register = "Register"; 
            public const string VerfiyPhone = "VerfiyPhone";
            public const string VerfiyPhoneCode = "VerfiyPhoneCode";
            public const string GenerateVerfiyPhoneCode = "GenerateVerfiyPhoneCode";
            public const string Provision = "Provision";

        }
        public sealed class Menu
        {
            public const string MenuList = "MenuList";
            public const string LoadMenuList = "LoadMenuList";
            public const string GetMenuForm = "GetMenuForm";
            public const string SaveMenu = "SaveMenu";
            public const string RemoveMenu = "RemoveMenu";
            public const string Add = "Add";
            public const string LoadMenuTree = "LoadMenuTree";

            public const string RoleInMenu = "RoleInMenu";
            public const string GetRoleInMenu = "GetRoleInMenu";
            public const string SubmitRoleInMenu = "SubmitRoleInMenu";
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

        public sealed class Permission
        {
            public const string Index = "Index";
            
            public const string GetApplication = "GetApplication";
            public const string GetAllPermissions = "GetAllPermissions";
            public const string RemovePermission = "RemovePermission";
            public const string SavePermission = "SavePermission";
            public const string PermissionForm = "PermissionForm";

            public const string LoadPermissionList = "LoadPermissionList";
            public const string LoadPermissionTrees = "LoadPermissionTrees";

            public const string RoleInPermission = "RoleInPermission";
            public const string GetRoleInPermission = "GetRoleInPermission";
            public const string SubmitRoleInPermission = "SubmitRoleInPermission";

            public const string GetPermissionTreeByPermissionGroupId = "GetPermissionTreeByPermissionGroupId";
            public const string IsPermissionGroupInRole = "IsPermissionGroupInRole";
            public const string SubmitPermissionGroupInRole = "SubmitPermissionGroupInRole";
            public const string GetPermissionInPermissionGroup = "GetPermissionInPermissionGroup";
            public const string GetAllPermissionInGroup = "GetAllPermissionInGroup";
        }

        public sealed class Role
        {
            public const string ExistRoleName = "ExistRoleName";
            public const string EditRole = "EditRole";
            public const string GetRoleListResult = "GetRoleListResult";
            public const string GetRoleForm = "GetRoleForm";
            public const string RomoveRole = "RomoveRole";
            public const string GetRoleData = "GetRoleData";
            public const string RoleDetail = "RoleDetail";

            public const string MenuInRole = "MenuInRole";
            public const string GetMenuInRole = "GetMenuInRole";
            public const string SubmitMenuInRole = "SubmitMenuInRole";

            public const string PermissionInRole = "PermissionInRole";
            public const string GetPermissionInRole = "GetPermissionInRole";
            public const string GetAllPermissionInRole = "GetAllPermissionInRole";
            public const string SubmitPermissionInRole = "SubmitPermissionInRole";

            public const string LoadUserLeftInRoseList = "LoadUserLeftInRoseList";
            public const string UserInRole = "UserInRole";
            public const string GetUserInRole = "GetUserInRole";
            public const string SubmitUserInRole = "SubmitUserInRole";

        }

        public sealed class Organization
        {
            public const string OrganizationList = "OrganizationList";
            public const string LoadOrganizationList = "LoadOrganizationList";
            public const string GetOrganizationForm = "GetOrganizationForm";
            public const string SaveOrganizationForm = "SaveOrganizationForm";
            public const string RemoveOrganization = "RemoveOrganization";
            public const string ExistOrganizationName = "ExistOrganizationName";

            public const string LoadOrganizationTree = "LoadOrganizationTree";
            public const string LoadOrganizationOneLevelTree = "LoadOrganizationOneLevelTree";

            public const string LoadOrgTree = "LoadOrgTree";

        }

        public sealed class User
        {
            public const string LoadUserList = "LoadUserList";
            public const string GetUserForm = "GetUserForm";
            public const string UserOrganizationTree = "UserOrganizationTree";

            public const string SaveUser = "SaveUser";
            public const string RemoveUser = "RemoveUser";

            public const string RemoveUsers = "RemoveUsers";

            public const string FreezeOrActivation = "FreezeOrActivation";
            public const string UserDetail = "UserDetail";

            public const string ExistUserName = "ExistUserName";
            public const string ExistUserEmail = "ExistUserEmail";
            public const string ExistUserPhone = "ExistUserPhone";

            public const string LoadRoleByIds = "LoadRoleByIds";
            public const string LoadUserListNotInRole = "LoadUserListNotInRole";

            public const string DownLoadExcelTemplate = "DownLoadExcelTemplate";
            public const string UploadExcelTemplate = "UploadExcelTemplate";

            public const string LoadUserTracingLogList = "LoadUserTracingLogList";

            public const string RoleInUser = "RoleInUser";
            public const string GetRoleInUser = "GetRoleInUser";
            public const string SubmitRoleInUser = "SubmitRoleInUser";
            public const string GetRoleInUserUserList = "GetRoleInUserUserList";
            public const string GetRoleInUserRoleList = "GetRoleInUserRoleList";


            public const string LoadUserApplicationList = "LoadUserApplicationList";
            public const string DeleteUserApplication = "DeleteUserApplication";
            public const string UpdateapplicationStatusagree = "UpdateapplicationStatusagree";
            public const string UpdateapplicationStatusnoagree = "UpdateapplicationStatusnoagree";

            public const string ModifyPhoneAndEmailForm = "ModifyPhoneAndEmailForm";
            public const string ModifyTenantUserContactPhone = "ModifyTenantUserContactPhone";
            public const string ModifyTenantUserContactEmail = "ModifyTenantUserContactEmail";
            public const string SaveUserList = "SaveUserList";
        }

        public sealed class ConfigManager
        {
            public const string GetConfigTypeList = "GetConfigTypeList";

            public const string LoadConfigList = "LoadConfigList";
            public const string GetConfigForm = "GetConfigForm";
            public const string SaveConfig = "SaveConfig";
            public const string RemoveConfig = "RemoveConfig";

            public const string LoadPropertyList = "LoadPropertyList";
            public const string GetPropertyForm = "GetPropertyForm";
            public const string SaveConfigAttribute = "SaveConfigAttribute";
            public const string RemoveConfigAttribute = "RemoveConfigAttribute";
        }

        public sealed class SysSequence
        {

            public const string LoadSysSequenceList = "LoadSysSequenceList";
            public const string GetSysSequenceForm = "GetSysSequenceForm";
            public const string SaveSysSequence = "SaveSysSequence";
            public const string RemoveSysSequence = "RemoveSysSequence";
        }

        public sealed class Message
        {
            public const string LoadMessageCategoryParentTree = "LoadMessageCategoryParentTree";
            public const string SaveMessageCategory = "SaveMessageCategory";
            public const string RemoveMessageCategory = "RemoveMessageCategory";
            public const string LoadMessageCategoryList = "LoadMessageCategoryList";
            public const string GetMessageCategoryForm = "GetMessageCategoryForm";
            public const string LoadMessageCategoryparentchildrenTree = "LoadMessageCategoryparentchildrenTree";

            public const string LoadMessageClassList = "LoadMessageClassList";
            public const string GetMessageClassForm = "GetMessageClassForm";
            public const string SaveMessageClassForm = "SaveMessageClassForm";
            public const string RemoveMessageClass = "RemoveMessageClass";

            public const string LoadMessageTemplateList = "LoadMessageTemplateList";
            public const string GetMessageTemplateForm = "GetMessageTemplateForm";
            public const string SaveMessageTemplateForm = "SaveMessageTemplateForm";
            public const string RemoveMessageTemplate = "RemoveMessageTemplate";
            
        }

        public sealed class Dictionary
        {
            public const string SaveDictType = "SaveDictType";
            public const string RemoveDictType = "RemoveDictType";
            public const string LoadDictTypeList = "LoadDictTypeList";
            public const string GetDictTypeForm = "GetDictTypeForm";

            public const string LoadDictValueList = "LoadDictValueList";
            public const string GetDictValueForm = "GetDictValueForm";
            public const string SaveDictValue = "SaveDictValue";
            public const string RemoveDictValue = "RemoveDictValue";
        }
        #endregion

    }
}