using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Service.DTO.Account;
using KC.Service.DTO;

namespace KC.Service.Account
{
    public interface IPermissionService
    {
        List<PermissionDTO> FindPermissionsByRoleIds(List<string> roleIds);
        List<PermissionDTO> FindPermissionsByRoleNames(List<string> roleNames);
        List<PermissionSimpleDTO> FindAllPermissions();
        Task<List<PermissionSimpleDTO>> FindAllSimplePermissionTreesAsync();

        Task<List<PermissionSimpleDTO>> FindRootPermissionsByNameAsync(string name);
        PaginatedBaseDTO<PermissionDTO> FindPaginatedPermissions(string key, string value, int pageIndex, int pageSize, string sidx, bool isAscent);

        PermissionDTO GetPermissionByAppIDAndName(Guid? appId, string controllerName, string actionName);
        PermissionDTO GetDetailPermissionById(int id);

        Task<bool> SavePermissionsAsync(List<PermissionDTO> models, Guid appId);
        bool UpdatePermission(PermissionDTO model, string operatorId, string operatorName);
        bool UpdateRoleInPermission(int id, List<string> addList, string operatorId, string operatorName);

        Task<bool> ExistPermissionNameAsync(int pid, string name);
        bool CreatePermission(PermissionDTO model, string operatorId, string operatorName);
        bool DeletePermission(int id, string operatorId, string operatorName);
        bool ExistsPermission(Guid appId, string controllerName, string actionName);
    }
}