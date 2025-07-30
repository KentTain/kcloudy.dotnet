using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KC.Framework.Extension;
using KC.Model.Account;
using KC.Service.Extension;
using KC.Service.DTO.Account;

namespace KC.Service.Account.AutoMapper.Converter
{
    public class RoleConverter : ITypeConverter<RoleDTO, Role>
    {
        public Role Convert(RoleDTO source, Role destination, ResolutionContext context)
        {
            var roleDto = source;
            if (roleDto == null)
            {
                return null;
            }
            var result = new Role()
            {
                IsSystemRole = roleDto.IsSystemRole,
                Id = roleDto.RoleId,
                Name = roleDto.RoleName,
                BusinessType = roleDto.BusinessType,
                DisplayName = roleDto.DisplayName,
                Description = roleDto.Description,
                IsDeleted = roleDto.IsDeleted,
                CreatedBy = roleDto.CreatedBy,
                CreatedName = roleDto.CreatedName,
                CreatedDate = roleDto.CreatedDate,
                ModifiedBy = roleDto.ModifiedBy,
                ModifiedDate = roleDto.ModifiedDate,
                ModifiedName = roleDto.ModifiedName,
            };
            //if (roleDto.MenuNodes != null)
            //    result.MenuNodes = roleDto.MenuNodes.MapTo<MenuNode>();
            //if (roleDto.Permissions != null)
            //    result.Permissions = roleDto.Permissions.MapTo<Permission>();
            return result;
        }
    }

    public class RoleDTOConverter : ITypeConverter<Role, RoleDTO>
    {
        public RoleDTO Convert(Role source, RoleDTO destination, ResolutionContext context)
        {
            var role = source;
            if (role == null)
            {
                return null;
            }

            var result = new RoleDTO()
            {
                IsSystemRole = role.IsSystemRole,
                RoleId = role.Id,
                RoleName = role.Name,
                BusinessType = role.BusinessType,
                DisplayName = role.DisplayName,
                Description = role.Description,
                IsDeleted = role.IsDeleted,
                CreatedBy = role.CreatedBy,
                CreatedName = role.CreatedName,
                CreatedDate = role.CreatedDate,
                ModifiedBy = role.ModifiedBy,
                ModifiedDate = role.ModifiedDate,
                ModifiedName = role.ModifiedName,
            };
            if (role.RoleUsers != null)
                role.RoleUsers.ToList().ForEach(u => result.UserIds.Add(u.UserId));
            if (role.RoleUsers != null)
                result.Users = ComMapper.MapperUtil.MapToUserDTOs(role.RoleUsers.Select(m => m.User).ToList());
            if (role.RoleMenuNodes != null)
                result.MenuNodes = ComMapper.MapperUtil.MapToMenuDTOs(role.RoleMenuNodes.Select(m => m.MenuNode).Where(m => !m.IsDeleted).ToList());
            if (role.RolePermissions != null)
                result.Permissions = ComMapper.MapperUtil.MapToPermissionDTOs(role.RolePermissions.Select(m => m.Permission).Where(m => !m.IsDeleted).ToList());

            return result;
        }
    }
}

