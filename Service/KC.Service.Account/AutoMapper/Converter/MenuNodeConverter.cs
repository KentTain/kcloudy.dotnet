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
    public class MenuNodeConverter : ITypeConverter<MenuNode, MenuNodeDTO>
    {
        public MenuNodeDTO Convert(MenuNode source, MenuNodeDTO destination, ResolutionContext context)
        {
            var menu = source;
            if (menu == null)
            {
                return null;
            }

            var result = new MenuNodeDTO()
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
                SmallIcon = menu.SmallIcon,
                BigIcon = menu.BigIcon,
                URL = menu.URL,
                IsExtPage = menu.IsExtPage,
                //Children = ComMapper.MapperUtil.MapToMenuDTOs(menu.ChildNodes.Where(m => !m.IsDeleted).OrderBy(m => m.Index).ToList()),
                Children = context.Mapper.Map<List<MenuNodeDTO>>(menu.ChildNodes),
                ApplicationId = menu.ApplicationId,
                ApplicationName = menu.ApplicationName,
                Description = menu.Description,
                TenantType = menu.TenantType,
                Version = menu.Version,
                DefaultRoleId = menu.DefaultRoleId,
                AuthorityId = menu.AuthorityId
            };

            if (menu.MenuRoles.Any())
            {
                result.Role = Mapp(menu.MenuRoles.Select(m => m.Role).ToList());
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
    public class MenuNodeDTOConverter : ITypeConverter<MenuNodeDTO, MenuNode>
    {
        public MenuNode Convert(MenuNodeDTO source, MenuNode destination, ResolutionContext context)
        {
            var menu = source;
            if (menu == null)
            {
                return null;
            }

            var result = new MenuNode()
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
                //ChildNodes = ComMapper.MapperUtil.MapToMenus(menu.Children),
                ChildNodes = context.Mapper.Map<List<MenuNode>>(menu.Children),

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

