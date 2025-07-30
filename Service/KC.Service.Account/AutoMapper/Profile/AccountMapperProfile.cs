using System;
using System.Collections.Generic;
using System.Linq;
using KC.Framework.Extension;
using KC.Model.Account;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Account.AutoMapper.Converter;
using KC.Service.DTO.Account;
using KC.Service.DTO;

namespace KC.Service.Account.AutoMapper.Profile
{
    public class AccountMapperProfile : global::AutoMapper.Profile
    {
        public AccountMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();

            CreateMap<ProcessLogBase, ProcessLogBaseDTO>()
                .ForMember(target => target.TypeString, config => config.Ignore());
            CreateMap<ProcessLogBaseDTO, ProcessLogBase>();

            CreateMap<TreeNode<Organization>, TreeNodeDTO<OrganizationDTO>>()
                .IncludeBase<Entity, EntityDTO>()
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.@checked, config => config.Ignore());
            CreateMap<TreeNodeDTO<OrganizationDTO>, TreeNode<Organization>>()
                .IncludeBase<EntityDTO, Entity>()
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.Ignore())
                .ForMember(target => target.ParentNode, config => config.Ignore());

            CreateMap<TreeNode<Organization>, TreeNodeSimpleDTO<OrganizationSimpleDTO>>()
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.@checked, config => config.Ignore());
            CreateMap<TreeNodeSimpleDTO<OrganizationSimpleDTO>, TreeNode<Organization>>()
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.Ignore())
                .ForMember(target => target.ParentNode, config => config.Ignore())
                .ForMember(target => target.IsDeleted, config => config.Ignore())
                .ForMember(target => target.CreatedBy, config => config.Ignore())
                .ForMember(target => target.CreatedName, config => config.Ignore())
                .ForMember(target => target.CreatedDate, config => config.Ignore())
                .ForMember(target => target.ModifiedBy, config => config.Ignore())
                .ForMember(target => target.ModifiedName, config => config.Ignore())
                .ForMember(target => target.ModifiedDate, config => config.Ignore());

            #region 组织架构、角色
            CreateMap<Role, RoleDTO>()
                .ForMember(target => target.@checked, config => config.Ignore())
                .ConvertUsing<RoleDTOConverter>();
            CreateMap<RoleDTO, Role>()
                .ConvertUsing<RoleConverter>();

            CreateMap<Role, RoleSimpleDTO>()
                .ForMember(target => target.RoleId, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.RoleName, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.DisplayName, config => config.MapFrom(src => src.DisplayName))
                .ForMember(target => target.Users,
                    config =>
                        config.MapFrom(
                            src => src.RoleUsers != null
                                ? ComMapper.MapperUtil.MapToUserSimpleDTOs(src.RoleUsers.Select(m => m.User).Where(m => m.Status == WorkflowBusStatus.Approved).ToList())
                                : new List<UserSimpleDTO>()))
                .ForMember(target => target.MenuNodes,
                    config =>
                        config.MapFrom(
                            src => src.RoleMenuNodes != null
                                ? ComMapper.MapperUtil.MapToMenuSimpleDTOs(src.RoleMenuNodes.Select(m => m.MenuNode).Where(m => !m.IsDeleted).ToList())
                                : new List<MenuNodeSimpleDTO>()))
                .ForMember(target => target.Permissions,
                    config =>
                        config.MapFrom(
                            src => src.RolePermissions != null
                                ? ComMapper.MapperUtil.MapToPermissionSimpleDTOs(src.RolePermissions.Select(m => m.Permission).Where(m => !m.IsDeleted).ToList())
                                : new List<PermissionSimpleDTO>()))
                .ForMember(target => target.@checked, config => config.Ignore());

            CreateMap<Organization, OrganizationDTO>()
                .IncludeBase<TreeNode<Organization>, TreeNodeDTO<OrganizationDTO>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.ParentName, config => config.MapFrom(src => src.ParentNode.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.UserNamesInfo,
                    config =>
                        config.MapFrom(
                            src => src.Users != null
                                ? string.Join("; ", src.Users.Select(m => m.DisplayName))
                                : string.Empty))
                .ForMember(target => target.@checked, config => config.Ignore())
                .ForMember(target => target.Users, config => config.Ignore())
                .ForMember(target => target.IsEditMode, config => config.Ignore());

            CreateMap<OrganizationDTO, Organization>()
                .IncludeBase<TreeNodeDTO<OrganizationDTO>, TreeNode<Organization>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.ChildNodes, config => config.Ignore())
                .ForMember(target => target.ParentNode, config => config.Ignore())
                .ForMember(target => target.OrganizationUsers, config => config.Ignore())
                .ForMember(target => target.Users, config => config.Ignore());

            CreateMap<Organization, OrganizationSimpleDTO>()
                .IncludeBase<TreeNode<Organization>, TreeNodeSimpleDTO<OrganizationSimpleDTO>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Text, config => config.MapFrom(src => src.Name))
                .ForMember(target => target.ParentName, config => config.MapFrom(src => src.ParentNode.Name))
                .ForMember(target => target.Children, config => config.MapFrom(src => src.ChildNodes))
                .ForMember(target => target.Users,
                    config =>
                        config.MapFrom(
                            src => src.Users != null
                                ? ComMapper.MapperUtil.MapToUserSimpleDTOs(src.Users.ToList())
                                : new List<UserSimpleDTO>()))
                .ForMember(target => target.@checked, config => config.Ignore())
                .ForMember(target => target.Users, config => config.Ignore());

            CreateMap<OrganizationSimpleDTO, Organization>()
                .IncludeBase<TreeNodeSimpleDTO<OrganizationSimpleDTO>, TreeNode<Organization>>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.Name, config => config.MapFrom(src => src.Text))
                .ForMember(target => target.Users,
                    config =>
                        config.MapFrom(
                            src => src.Users != null
                                ? ComMapper.MapperUtil.MapSimpleToUsers(src.Users).ToList()
                                : new List<User>()))
                .ForMember(target => target.ChildNodes, config => config.Ignore())
                .ForMember(target => target.ParentNode, config => config.Ignore())
                .ForMember(target => target.OrganizationUsers, config => config.Ignore())
                .ForMember(target => target.Users, config => config.Ignore());
            #endregion

            #region 系统及用户设置
            CreateMap<SystemSettingProperty, SystemSettingPropertyDTO>()
                .ForMember(target => target.SystemSettingCode,
                    config =>
                        config.MapFrom(
                            src => src.SystemSetting != null
                                    ? src.SystemSetting.Code
                                    : string.Empty))
                .ForMember(target => target.SystemSettingName,
                    config =>
                        config.MapFrom(
                            src => src.SystemSetting != null
                                    ? src.SystemSetting.Name
                                    : string.Empty));
            CreateMap<SystemSettingPropertyDTO, SystemSettingProperty>()
                .ForMember(target => target.SystemSetting, config => config.Ignore());

            CreateMap<SystemSetting, SystemSettingDTO>();
            CreateMap<SystemSettingDTO, SystemSetting>();

            CreateMap<UserSettingProperty, UserSettingPropertyDTO>()
                .ForMember(target => target.UserSettingCode,
                    config =>
                        config.MapFrom(
                            src => src.UserSetting != null
                                    ? src.UserSetting.Code
                                    : string.Empty))
                .ForMember(target => target.UserSettingName,
                    config =>
                        config.MapFrom(
                            src => src.UserSetting != null
                                    ? src.UserSetting.Name
                                    : string.Empty))
                .ForMember(target => target.IsSystemSetting,
                    config =>
                        config.MapFrom(
                            src => src.UserSetting != null
                                    ? src.UserSetting.IsSystemSetting
                                    : false));
            CreateMap<UserSettingPropertyDTO, UserSettingProperty>()
                .ForMember(target => target.UserSetting, config => config.Ignore());

            CreateMap<UserSetting, UserSettingDTO>()
                .ForMember(target => target.UserCode,
                    config =>
                        config.MapFrom(
                            src => src.User != null
                                    ? src.User.MemberId
                                    : string.Empty))
                .ForMember(target => target.UserDisplayName,
                    config =>
                        config.MapFrom(
                            src => src.User != null
                                    ? src.User.DisplayName
                                    : string.Empty));
            CreateMap<UserSettingDTO, UserSetting>()
                .ForMember(target => target.User, config => config.Ignore());

            //将系统设置转化为用户个人设置
            CreateMap<SystemSettingProperty, UserSettingProperty>()
                .ForMember(target => target.SystemSettingPropertyAttrId, config => config.MapFrom(src => src.PropertyAttributeId))
                .ForMember(target => target.PropertyAttributeId, config => config.Ignore())
                .ForMember(target => target.UserSettingId, config => config.Ignore())
                .ForMember(target => target.UserSettingCode, config => config.Ignore())
                .ForMember(target => target.UserSetting, config => config.Ignore());

            CreateMap<SystemSetting, UserSetting>()
                .ForMember(target => target.SystemSettingId, config => config.MapFrom(src => src.PropertyId))
                .ForMember(target => target.SystemSettingCode, config => config.MapFrom(src => src.Code))
                .ForMember(target => target.IsSystemSetting, config => config.Ignore())
                .ForMember(target => target.PropertyId, config => config.Ignore())
                .ForMember(target => target.Code, config => config.Ignore())
                .ForMember(target => target.UserId, config => config.Ignore())
                .ForMember(target => target.User, config => config.Ignore());
            #endregion

            #region 用户
            CreateMap<UserTracingLog, UserTracingLogDTO>();
            CreateMap<UserTracingLogDTO, UserTracingLog>();

            CreateMap<UserLoginLog, UserLoginLogDTO>();
            CreateMap<UserLoginLogDTO, UserLoginLog>();

            CreateMap<User, UserDTO>()
                .ForMember(target => target.UserId, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.PositionLevelName,
                    config => config.MapFrom(src => src.PositionLevel.ToDescription()))
                .ForMember(target => target.Organizations, config => config.MapFrom(src => src.Organizations))
                .ForMember(target => target.OrganizationIds,
                    config =>
                        config.MapFrom(
                            src =>
                                src.UserOrganizations.Any()
                                    ? src.UserOrganizations.Select(m => m.OrganizationId).ToList()
                                    : default(List<int>)))
                //.ForMember(target => target.OrganizationName,
                //    config =>
                //        config.MapFrom(
                //            src =>
                //                src.UserOrganizations.Any()
                //                    ? src.UserOrganizations.Select(m => m.Organization).ToCommaSeparatedStringByFilter(m => m.Name)
                //                    : string.Empty))
                .ForMember(target => target.RoleIds,
                    config =>
                        config.MapFrom(
                            src =>
                                src.UserRoles.Any()
                                    ? src.UserRoles.Select(idntityRole => idntityRole.RoleId)
                                    : new List<string>()))
                .ForMember(target => target.RoleNames,
                    config =>
                        config.MapFrom(
                            src =>
                                src.UserRoles.Any()
                                    ? src.UserRoles.Select(m => m.Role.DisplayName).ToList()
                                    : new List<string>()))
                .ForMember(target => target.IsSystemAdmin,
                    config =>
                        config.MapFrom(
                            src => src.Id.Equals(RoleConstants.AdminUserId, StringComparison.OrdinalIgnoreCase)))
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.TenantName, config => config.Ignore())
                .ForMember(target => target.TenantNickName, config => config.Ignore())
                .ForMember(target => target.Password, config => config.Ignore())
                .ForMember(target => target.LockoutEndDateUtc, config => config.Ignore());
            CreateMap<UserDTO, User>()
                .ForMember(target => target.Id, config => config.MapFrom(src => src.UserId))
                .ForMember(target => target.UserRoles,
                    config =>
                        config.MapFrom(
                            src =>
                                src.RoleIds.Any()
                                    ? src.RoleIds.Select(roleId => new UsersInRoles() { RoleId = roleId, UserId = src.UserId })
                                    : new List<UsersInRoles>()))
                .ForMember(target => target.UserOrganizations,
                    config =>
                        config.MapFrom(
                            src =>
                                src.OrganizationIds.Any()
                                    ? src.OrganizationIds.Select(orgId => new UsersInOrganizations() { OrganizationId = orgId, UserId = src.UserId })
                                    : new List<UsersInOrganizations>()))
                .ForMember(target => target.NormalizedUserName, config => config.Ignore())
                .ForMember(target => target.NormalizedEmail, config => config.Ignore())
                .ForMember(target => target.ConcurrencyStamp, config => config.Ignore())
                .ForMember(target => target.LockoutEnd, config => config.Ignore())
                .ForMember(target => target.PasswordHash, config => config.Ignore())
                .ForMember(target => target.Roles, config => config.Ignore())
                .ForMember(target => target.Organizations, config => config.Ignore())
                .ForMember(target => target.Claims, config => config.Ignore())
                .ForMember(target => target.Recommended, config => config.Ignore())
                .ForMember(target => target.UserOrganizations, config => config.Ignore());

            CreateMap<User, UserSimpleDTO>()
                .ForMember(target => target.UserId, config => config.MapFrom(src => src.Id))
                .ForMember(target => target.PositionLevelName, config => config.MapFrom(src => src.PositionLevel.ToDescription()))
                .ForMember(target => target.UserOrgIds,
                    config =>
                        config.MapFrom(
                            src =>
                                src.UserOrganizations.Any()
                                    ? src.UserOrganizations.Select(m => m.OrganizationId).ToList()
                                    : default(List<int>)))
                .ForMember(target => target.UserOrgCodes,
                    config =>
                        config.MapFrom(
                            src =>
                                src.UserOrganizations.Any()
                                    ? src.Organizations.Select(m => m.OrganizationCode).ToList()
                                    : default(List<string>)))
                .ForMember(target => target.UserOrgTreeCodes,
                    config =>
                        config.MapFrom(
                            src =>
                                src.UserOrganizations.Any()
                                    ? src.Organizations.Select(m => m.TreeCode).ToList()
                                    : default(List<string>)))
                .ForMember(target => target.UserOrgNames,
                    config =>
                        config.MapFrom(
                            src =>
                                src.UserOrganizations.Any()
                                    ? src.Organizations.Select(m => m.Name).ToList()
                                    : default(List<string>)))
                .ForMember(target => target.UserRoleIds,
                    config =>
                        config.MapFrom(
                            src =>
                                src.UserRoles.Any()
                                    ? src.UserRoles.Select(m => m.RoleId).ToList()
                                    : default(List<string>)))
                .ForMember(target => target.UserRoleNames,
                    config =>
                        config.MapFrom(
                            src =>
                                src.UserRoles.Any()
                                    ? src.Roles.Select(m => m.DisplayName).ToList()
                                    : default(List<string>)))
                .ForMember(target => target.LockoutEndDateUtc, config => config.Ignore())
                .ForMember(target => target.TenantName, config => config.Ignore());
            #endregion
        }
    }
}
