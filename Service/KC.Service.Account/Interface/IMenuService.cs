using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KC.Service.DTO.Account;
using KC.Service.DTO;

namespace KC.Service.Account
{
    public interface IMenuService
    {
        MenuNodeDTO GetDetailMenuById(int id);
        MenuNodeDTO GetMenuById(int id);
        List<MenuNodeDTO> FindMenuNodesByIds(List<int> menuIds);
        List<MenuNodeDTO> FindMenuNodesByRoleIds(List<string> roleIds);
        Task<List<MenuNodeSimpleDTO>> FindAllSimpleMenuTreesAsync();
        Task<List<MenuNodeSimpleDTO>> FindMenuTreesByNameAsync(string menuName);
        Task<bool> ExistMenuNameAsync(int pid, string name);
        bool RemoveMenuById(int id, string operatorId, string operatorName);
        bool SaveMenu(MenuNodeDTO data, string operatorId, string operatorName);
        Task<bool> SaveMenusAsync(List<MenuNodeDTO> models, Guid appId);
        bool UpdateRoleInMenu(int id, List<string> addList, string operatorId, string operatorName);
    }
}