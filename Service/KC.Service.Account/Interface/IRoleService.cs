using KC.Service.EFService;
using KC.Service.DTO.Account;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO.Search;

namespace KC.Service.Account
{
    public interface IRoleService : IEFService
    {
        bool ExistsRole(string roleLoweredName);

        Task<List<MenuNodeSimpleDTO>> FindUserMenusByRoleIdsAsync(List<string> roleIds);
        Task<List<PermissionSimpleDTO>> FindUserPermissionsByRoleIdsAsync(List<string> roleIds);

        Task<List<RoleSimpleDTO>> FindAllSimpleRolesAsync();
        Task<List<RoleDTO>> FindRolesWithUsersByRoleIds(IEnumerable<string> roleIds, IEnumerable<string> exceptRoleIds);
        List<RoleDTO> FindRolesByIds(List<string> ids);
        List<RoleDTO> FindRolesWithUsersByUserId(string userId);

        PaginatedBaseDTO<RoleDTO> FindPagenatedRoleList(int pageIndex, int pageSize, string displayName, string currentUserId);

        RoleDTO GetDetailRoleByRoleId(string roleId);
        RoleDTO GetRoleWithUsersByRoleId(string roleId);

        bool CreateRole(RoleDTO role, string operatorId, string operatorName);
        bool RomoveRole(string roleId, string operatorId, string operatorName);
        bool UpdateRole(RoleDTO role, string operatorId, string operatorName);

        bool UpdatePermissionInRole(string roleId, List<int> newIds, string operatorId, string operatorName);
        bool UpdateMenuInRole(string roleId, List<int> newIds, string operatorId, string operatorName);
        bool UpdateUserInRole(string roleId, List<string> addList, List<string> delList, string operatorId, string operatorName);

    }
}
