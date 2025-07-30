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
    public class PermissionConverter : ITypeConverter<Permission, PermissionDTO>
    {
        public PermissionDTO Convert(Permission source, PermissionDTO destination, ResolutionContext context)
        {
            var menu = source;
            if (menu == null)
            {
                return null;
            }

            var result = new PermissionDTO()
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
                //Children = ComMapper.MapperUtil.MapToPermissionDTOs(menu.ChildNodes.Where(m => !m.IsDeleted).OrderBy(m => m.Index).ToList()),
                Children = context.Mapper.Map<List<PermissionDTO>>(menu.ChildNodes),
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
            if (menu.PermissionRoles.Any())
            {
                result.Role = Mapp(menu.PermissionRoles.Select(m => m.Role).ToList());
            }
            return result;
        }
        public List<RoleDTO> Mapp(List<Role> role)
        {
            return role.Select(aspNetRole => new RoleDTO()
            {
                RoleId = aspNetRole.Id,
                RoleName = aspNetRole.Name,
                DisplayName = aspNetRole.DisplayName,
                Description = aspNetRole.Description,
            }).ToList();
        }
    }
    public class PermissionDTOConverter : ITypeConverter<PermissionDTO, Permission>
    {
        public Permission Convert(PermissionDTO source, Permission destination, ResolutionContext context)
        {
            var menu = source;
            if (menu == null)
            {
                return null;
            }

            var result = new Permission()
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
                //ChildNodes = ComMapper.MapperUtil.MapToPermissions(menu.Children),
                ChildNodes = context.Mapper.Map<List<Permission>>(menu.Children),
                ApplicationId = menu.ApplicationId,
                Description = menu.Description,
                DefaultRoleId = menu.DefaultRoleId,
                AuthorityId = menu.AuthorityId,
                IsDeleted = menu.IsDeleted,
                CreatedBy = menu.CreatedBy,
                CreatedName = menu.CreatedName,
                CreatedDate = menu.CreatedDate,
                ModifiedBy = menu.ModifiedBy,
                ModifiedDate = menu.ModifiedDate,
                ModifiedName = menu.ModifiedName,
            };
            return result;
        }
        public List<RoleDTO> Mapp(List<Role> role)
        {
            return role.Select(aspNetRole => new RoleDTO()
            {
                RoleId = aspNetRole.Id,
                RoleName = aspNetRole.Name,
                DisplayName = aspNetRole.DisplayName,
                Description = aspNetRole.Description,
            }).ToList();
        }
    }
}

