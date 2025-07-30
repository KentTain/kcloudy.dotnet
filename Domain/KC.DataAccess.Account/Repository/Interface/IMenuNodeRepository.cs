using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Model.Account;

namespace KC.DataAccess.Account.Repository
{
    public interface IMenuNodeRepository : Database.IRepository.IDbTreeRepository<MenuNode>
    {
        MenuNode GetDetailMenuById(int id);
        MenuNode GetMenuByFilter(Expression<Func<MenuNode, bool>> predicate);
        MenuNode GetMenuById(int id);
        IList<MenuNode> GetMenuNodesByIds(List<int> menuIds);
        IList<MenuNode> GetMenuNodesByRoleIds(List<string> roleIds);
        Task<bool> SaveMenusAsync(List<MenuNode> menuTrees, Guid appId);
        bool SaveRoleInMenu(int id, List<string> newIds, string operatorId, string operatorName);
        Task<bool> SetSystemAdminRoleDefaultMenus();
    }
}