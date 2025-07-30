using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Framework.Base;

namespace KC.Database.IRepository
{
    public interface IDbTreeRepository<K> : IDbRepository<K> where K : TreeNode<K>
    {
        bool ExistByTreeName(int id, int? parentId, string name);
        Task<bool> ExistByTreeNameAsync(int id, int? parentId, string name);

        List<K> FindAllTreeNodeWithNestChild();
        Task<List<K>> FindAllTreeNodeWithNestChildAsync();
        
        List<K> FindNodeListContainNestChildByFilter(Expression<Func<K, bool>> predicate);
        
        List<K> FindNodeListContainNestChildByParentId(int? parentId);
        Task<List<K>> FindNodeListContainNestChildByParentIdAsync(int? parentId);
        
        List<K> FindNodeListContainNestChildByParentIds(List<int> parentIds);
        
        List<K> FindTreeNodesWithNestParentAndChildByFilter(Expression<Func<K, bool>> predicate);
        Task<List<K>> FindTreeNodesWithNestParentAndChildByFilterAsync(Expression<Func<K, bool>> predicate);
        
        List<K> FindTreeNodesWithNestParentAndChildById(int id);
        Task<List<K>> FindTreeNodesWithNestParentAndChildByIdAsync(int id);
        
        List<K> FindTreeNodesWithNestParentAndChildByTreeCode(string treeCode);
        Task<List<K>> FindTreeNodesWithNestParentAndChildByTreeCodeAsync(string treeCode);

        K GetTreeNodeWithNestChildById(int id);
        Task<K> GetTreeNodeWithNestChildByIdAsync(int id);

        bool UpdateExtendFields();
        Task<bool> UpdateExtendFieldsAsync();
        bool UpdateExtendFieldsByFilter(Expression<Func<K, bool>> predicate);
        Task<bool> UpdateExtendFieldsByFilterAsync(Expression<Func<K, bool>> predicate);
    }

}
