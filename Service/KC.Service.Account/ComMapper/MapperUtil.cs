using KC.Framework.Extension;
using KC.Model.Account;
using KC.Service.DTO.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Account.ComMapper
{
    public static class MapperUtil
    {
        public static MenuNode MapToMenu(MenuNodeDTO menu)
        {
            if (menu == null)
                return null;
            return new MenuNode()
            {
                IsDeleted = menu.IsDeleted,
                CreatedBy = menu.CreatedBy,
                CreatedName = menu.CreatedName,
                CreatedDate = menu.CreatedDate,
                ModifiedBy = menu.ModifiedBy,
                ModifiedDate = menu.ModifiedDate,
                ModifiedName = menu.ModifiedName,

                Id = menu.Id,
                Name = menu.Text,
                ParentId = menu.ParentId,
                TreeCode = menu.TreeCode,
                Leaf = menu.Leaf,
                Level = menu.Level,
                Index = menu.Index,

                AreaName = menu.AreaName,
                ActionName = menu.ActionName,
                ControllerName = menu.ControllerName,
                ChildNodes = MapToMenus(menu.Children),

                SmallIcon = menu.SmallIcon,
                BigIcon = menu.BigIcon,
                IsExtPage = menu.IsExtPage,
                ApplicationName = menu.ApplicationName,
                ApplicationId = menu.ApplicationId,
                Description = menu.Description,
                TenantType = menu.TenantType,
                Version = menu.Version,
                DefaultRoleId = menu.DefaultRoleId,
                AuthorityId = menu.AuthorityId
            };
        }
        public static List<MenuNode> MapToMenus(List<MenuNodeDTO> menu)
        {
            return menu.Select(m => MapToMenu(m)).ToList();
        }

        public static MenuNodeDTO MapToMenuDTO(MenuNode menu)
        {
            if (menu == null)
                return null;
            return new MenuNodeDTO()
            {
                IsDeleted = menu.IsDeleted,
                CreatedBy = menu.CreatedBy,
                CreatedName = menu.CreatedName,
                CreatedDate = menu.CreatedDate,
                ModifiedBy = menu.ModifiedBy,
                ModifiedDate = menu.ModifiedDate,
                ModifiedName = menu.ModifiedName,

                Id = menu.Id,
                Text = menu.Name,
                ParentId = menu.ParentId,
                TreeCode = menu.TreeCode,
                Leaf = menu.Leaf,
                Level = menu.Level,
                Index = menu.Index,

                AreaName = menu.AreaName,
                ActionName = menu.ActionName,
                ControllerName = menu.ControllerName,
                Children = MapToMenuDTOs(menu.ChildNodes),

                SmallIcon = menu.SmallIcon,
                BigIcon = menu.BigIcon,
                IsExtPage = menu.IsExtPage,
                ApplicationName = menu.ApplicationName,
                ApplicationId = menu.ApplicationId,
                Description = menu.Description,
                TenantType = menu.TenantType,
                Version = menu.Version,
                DefaultRoleId = menu.DefaultRoleId,
                AuthorityId = menu.AuthorityId
            };
        }
        public static List<MenuNodeDTO> MapToMenuDTOs(List<MenuNode> menu)
        {
            return menu.Select(m => MapToMenuDTO(m)).ToList();
        }

        public static MenuNodeSimpleDTO MapToMenuSimpleDTO(MenuNode menu)
        {
            if (menu == null)
                return null;
            return new MenuNodeSimpleDTO()
            {
                Id = menu.Id,
                Text = menu.Name,
                ParentId = menu.ParentId,
                TreeCode = menu.TreeCode,
                Leaf = menu.Leaf,
                Level = menu.Level,
                Index = menu.Index,

                AreaName = menu.AreaName,
                ActionName = menu.ActionName,
                ControllerName = menu.ControllerName,
                Children = MapToMenuSimpleDTOs(menu.ChildNodes),

                SmallIcon = menu.SmallIcon,
                BigIcon = menu.BigIcon,
                IsExtPage = menu.IsExtPage,
                ApplicationId = menu.ApplicationId,
                Description = menu.Description,
                TenantType = menu.TenantType,
                Version = menu.Version,
                DefaultRoleId = menu.DefaultRoleId,
                AuthorityId = menu.AuthorityId
            };
        }
        public static List<MenuNodeSimpleDTO> MapToMenuSimpleDTOs(List<MenuNode> menu)
        {
            return menu.Select(m => MapToMenuSimpleDTO(m)).ToList();
        }


        public static Permission MapToPermission(PermissionDTO menu)
        {
            if (menu == null)
                return null;
            return new Permission()
            {
                Id = menu.Id,
                Name = menu.Text,
                AreaName = menu.AreaName,
                ActionName = menu.ActionName,
                ControllerName = menu.ControllerName,
                Parameters = menu.Parameters,
                Index = menu.Index,
                Leaf = menu.Leaf,
                Level = menu.Level,
                ParentId = menu.ParentId,
                ResultType = menu.ResultType,
                ApplicationName = menu.ApplicationName,
                ChildNodes = MapToPermissions(menu.Children.Where(m => !m.IsDeleted).OrderBy(m => m.Index).ToList()),
                ApplicationId = menu.ApplicationId,
                Description = menu.Description,
                DefaultRoleId = menu.DefaultRoleId,
                IsDeleted = menu.IsDeleted,
                CreatedBy = menu.CreatedBy,
                CreatedName = menu.CreatedName,
                CreatedDate = menu.CreatedDate,
                ModifiedBy = menu.ModifiedBy,
                ModifiedDate = menu.ModifiedDate,
                ModifiedName = menu.ModifiedName,
                AuthorityId = menu.AuthorityId
            };
        }
        public static List<Permission> MapToPermissions(List<PermissionDTO> menu)
        {
            return menu.Select(m => MapToPermission(m)).ToList();
        }

        public static PermissionDTO MapToPermissionDTO(Permission menu)
        {
            if (menu == null)
                return null;
            return new PermissionDTO()
            {
                Id = menu.Id,
                Text = menu.Name,
                AreaName = menu.AreaName,
                ActionName = menu.ActionName,
                ControllerName = menu.ControllerName,
                Parameters = menu.Parameters,
                Index = menu.Index,
                Leaf = menu.Leaf,
                Level = menu.Level,
                ParentId = menu.ParentId,
                ResultType = menu.ResultType,
                ApplicationName = menu.ApplicationName,
                Children = MapToPermissionDTOs(menu.ChildNodes.Where(m => !m.IsDeleted).OrderBy(m => m.Index).ToList()),
                ApplicationId = menu.ApplicationId,
                Description = menu.Description,
                DefaultRoleId = menu.DefaultRoleId,
                IsDeleted = menu.IsDeleted,
                CreatedBy = menu.CreatedBy,
                CreatedName = menu.CreatedName,
                CreatedDate = menu.CreatedDate,
                ModifiedBy = menu.ModifiedBy,
                ModifiedDate = menu.ModifiedDate,
                ModifiedName = menu.ModifiedName,
                AuthorityId = menu.AuthorityId
            }; 
        }
        public static List<PermissionDTO> MapToPermissionDTOs(List<Permission> menu)
        {
            return menu.Select(m => MapToPermissionDTO(m)).ToList();
        }

        public static PermissionSimpleDTO MapToPermissionSimpleDTO(Permission menu)
        {
            if (menu == null)
                return null;
            return new PermissionSimpleDTO()
            {
                Id = menu.Id,
                Text = menu.Name,
                AreaName = menu.AreaName,
                ActionName = menu.ActionName,
                ControllerName = menu.ControllerName,
                Parameters = menu.Parameters,
                Index = menu.Index,
                Leaf = menu.Leaf,
                Level = menu.Level,
                ParentId = menu.ParentId,
                ResultType = menu.ResultType,
                ApplicationName = menu.ApplicationName,
                Children = MapToPermissionSimpleDTOs(menu.ChildNodes.Where(m => !m.IsDeleted).OrderBy(m => m.Index).ToList()),
                ApplicationId = menu.ApplicationId,
                Description = menu.Description,
                DefaultRoleId = menu.DefaultRoleId,
                AuthorityId = menu.AuthorityId
            };
        }
        public static List<PermissionSimpleDTO> MapToPermissionSimpleDTOs(List<Permission> menu)
        {
            return menu.Select(m => MapToPermissionSimpleDTO(m)).ToList();
        }


        public static UserDTO MapToUserDTO(User user)
        {
            if (user == null)
                return null;
            return new UserDTO()
            {
                UserType = user.UserType,
                PositionLevel = user.PositionLevel,
                PositionLevelName = user.PositionLevel.ToDescription(),
                UserId = user.Id,
                UserName = user.UserName,
                MemberId = user.MemberId,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEnd?.DateTime,
                AccessFailedCount = user.AccessFailedCount,
                CreateDate = user.CreateDate,
                ContactQQ = user.ContactQQ,
                Telephone = user.Telephone,
                OpenId = user.OpenId,
                IsClient = user.IsClient,
                ReferenceId1 = user.ReferenceId1,
                ReferenceId2 = user.ReferenceId2,
                ReferenceId3 = user.ReferenceId3,
                Status = user.Status,
                IsModifyPassword = user.IsModifyPassword,
                RoleIds = user.UserRoles.Select(m => m.RoleId).ToList(),
                OrganizationIds = user.UserOrganizations.Select(m => m.OrganizationId).ToList(),
            };
        }
        public static List<UserDTO> MapToUserDTOs(List<User> menu)
        {
            return menu.Select(m => MapToUserDTO(m)).ToList();
        }

        public static UserSimpleDTO MapToUserSimpleDTO(User user)
        {
            if (user == null)
                return null;
            return new UserSimpleDTO()
            {
                UserType = user.UserType,
                PositionLevel = user.PositionLevel,
                PositionLevelName = user.PositionLevel.ToDescription(),
                UserId = user.Id,
                UserName = user.UserName,
                MemberId = user.MemberId,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEnd?.DateTime,
                AccessFailedCount = user.AccessFailedCount,
                CreateDate = user.CreateDate,
                ContactQQ = user.ContactQQ,
                Telephone = user.Telephone,
                OpenId = user.OpenId,
                IsClient = user.IsClient,
                ReferenceId1 = user.ReferenceId1,
                ReferenceId2 = user.ReferenceId2,
                ReferenceId3 = user.ReferenceId3,
                Status = user.Status,
                IsModifyPassword = user.IsModifyPassword,
            };
        }
        public static List<UserSimpleDTO> MapToUserSimpleDTOs(List<User> menu)
        {
            return menu.Select(m => MapToUserSimpleDTO(m)).ToList();
        }
        public static User MapSimpleToUser(UserSimpleDTO user)
        {
            if (user == null)
                return null;
            return new User()
            {
                UserType = user.UserType,
                PositionLevel = user.PositionLevel,
                Id = user.UserId,
                UserName = user.UserName,
                MemberId = user.MemberId,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount,
                CreateDate = user.CreateDate,
                ContactQQ = user.ContactQQ,
                Telephone = user.Telephone,
                OpenId = user.OpenId,
                IsClient = user.IsClient,
                ReferenceId1 = user.ReferenceId1,
                ReferenceId2 = user.ReferenceId2,
                ReferenceId3 = user.ReferenceId3,
                Status = user.Status,
                IsModifyPassword = user.IsModifyPassword,
            };
        }
        public static List<User> MapSimpleToUsers(List<UserSimpleDTO> menu)
        {
            return menu.Select(m => MapSimpleToUser(m)).ToList();
        }
    }
}
