using KC.Database.IRepository;
using KC.Framework.Exceptions;
using KC.Framework.Extension;
using KC.Framework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KC.Database.Extension;

namespace KC.Database.EFRepository
{
    /// <summary>
    ///     EntityFramework仓储操作基类
    /// </summary>
    /// <typeparam name="T">动态实体类型</typeparam>
    public abstract class EFTreeRepositoryBase<K> : EFRepositoryBase<K>, IDbTreeRepository<K> where K : TreeNode<K>
    {
        protected EFTreeRepositoryBase(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        #region Tree entity

        #region Exist
        /// <summary>
        ///     根据条件判断名称是否存在（同一Level下的）
        /// </summary>
        /// <param name="id"> 树Id，id=0时，则是新增记录； id>0时，则是编辑数据</param>
        /// <param name="parentId"> 父节点Id</param>
        /// <param name="name"> 需要判断的节点名称</param>
        /// <returns> 是否存在 </returns>
        public bool ExistByTreeName(int id, int? parentId, string name)
        {
            Expression<Func<K, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            //编辑时，需要排除自身（可以和自己的名称一样）
            if (id != 0)
            {
                predicate = predicate.And(m => !m.Id.Equals(id));
            }
            //同级别的名称进行比较
            if (parentId.HasValue)
            {
                predicate = predicate.And(m => m.ParentId == parentId.Value);
            }
            else
            {
                predicate = predicate.And(m => m.ParentId == null);
            }

            return base.ExistByFilter(predicate);
        }
        /// <summary>
        ///     根据条件判断是否存在（异步）
        /// </summary>
        /// <param name="id"> 树Id，id=0时，则是新增记录； id>0时，则是编辑数据</param>
        /// <param name="parentId"> 父节点Id</param>
        /// <param name="name"> 需要判断的节点名称</param>
        /// <returns> 是否存在 </returns>
        public Task<bool> ExistByTreeNameAsync(int id, int? parentId, string name)
        {
            Expression<Func<K, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            //编辑时，需要排除自身（可以和自己的名称一样）
            if (id != 0)
            {
                predicate = predicate.And(m => !m.Id.Equals(id));
            }
            //同级别的名称进行比较
            if (parentId.HasValue)
            {
                predicate = predicate.And(m => m.ParentId == parentId.Value);
            }
            else
            {
                predicate = predicate.And(m => m.ParentId == null);
            }

            return base.ExistByFilterAsync(predicate);
        }
        #endregion

        /// <summary>
        /// 获取所有的以Tree方式组装的列表
        /// </summary>
        /// <typeparam name="K">TreeNode<K></typeparam>
        /// <returns>
        /// 返回结果，形如：
        /// --node  --> ParentId == null
        ///     --node
        ///         --node
        ///         --node
        ///             --node
        ///     --node
        ///         --node
        /// --node  --> ParentId == null
        ///     --node
        ///         --node
        /// </returns>
        public virtual List<K> FindAllTreeNodeWithNestChild()
        {
            Expression<Func<K, bool>> predicate = m => !m.IsDeleted;
            //https://stackoverflow.com/questions/17195047/entity-framework-self-referencing-entity-query-results-are-flat-and-hiearchical/17221404#17221404
            var subset = new List<K>();
            var query = EFContext.Set<K>().Where(predicate);
            foreach (var disp in query.OrderBy(x => x.ParentId).ToArray())
            {
                if (!disp.ParentId.HasValue)
                    subset.Add(disp);
                else
                    break;
            }

            return subset;
        }
        /// <summary>
        /// 获取所有的以Tree方式组装的列表
        /// </summary>
        /// <typeparam name="K">TreeNode<K></typeparam>
        /// <returns>
        /// 返回结果，形如：
        /// --node  --> ParentId == null
        ///     --node
        ///         --node
        ///         --node
        ///             --node
        ///     --node
        ///         --node
        /// --node  --> ParentId == null
        ///     --node
        ///         --node
        /// </returns>
        public virtual async Task<List<K>> FindAllTreeNodeWithNestChildAsync()
        {
            Expression<Func<K, bool>> predicate = m => !m.IsDeleted;
            //https://stackoverflow.com/questions/17195047/entity-framework-self-referencing-entity-query-results-are-flat-and-hiearchical/17221404#17221404
            var subset = new List<K>();
            var query = EFContext.Set<K>().Where(predicate);
            foreach (var disp in await query.OrderBy(x => x.ParentId).ToArrayAsync())
            {
                if (!disp.ParentId.HasValue)
                    subset.Add(disp);
                else
                    break;
            }

            return subset;
        }

        ///// <summary>
        ///// 根据条件，获取所有的以Tree方式组装的列表
        ///// </summary>
        ///// <typeparam name="K">TreeNode<K></typeparam>
        ///// <returns>
        ///// 返回结果，形如：
        ///// --node  --> ParentId == null
        /////     --node
        /////         --node
        /////         --node
        /////             --node
        /////     --node
        /////         --node
        ///// --node  --> ParentId == null
        /////     --node
        /////         --node
        ///// </returns>
        //public virtual List<K> GetAllTreeNodeWithNestChildByFilter(Expression<Func<K, bool>> predicate) 
        //{
        //    //https://stackoverflow.com/questions/17195047/entity-framework-self-referencing-entity-query-results-are-flat-and-hiearchical/17221404#17221404
        //    var subset = new List<K>();
        //    var query = EFContext.Set<K>().Where(predicate);
        //    foreach (var disp in query.OrderBy(x => x.ParentId).ToArray())
        //    {
        //        if (!disp.ParentId.HasValue)
        //            subset.Add(disp);
        //        else
        //            break;
        //    }
        //    return subset;
        //}
        ///// <summary>
        ///// 获取所有的以Tree方式组装的列表
        ///// </summary>
        ///// <typeparam name="K">TreeNode<K></typeparam>
        ///// <returns>
        ///// 返回结果，形如：
        ///// --node  --> ParentId == null
        /////     --node
        /////         --node
        /////         --node
        /////             --node
        /////     --node
        /////         --node
        ///// --node  --> ParentId == null
        /////     --node
        /////         --node
        ///// </returns>
        //public virtual async Task<List<K>> GetAllTreeNodeWithNestChildByFilterAsync(Expression<Func<K, bool>> predicate)
        //{
        //    //https://stackoverflow.com/questions/17195047/entity-framework-self-referencing-entity-query-results-are-flat-and-hiearchical/17221404#17221404
        //    var subset = new List<K>();
        //    var query = EFContext.Set<K>().Where(predicate);
        //    foreach (var disp in await query.OrderBy(x => x.ParentId).ToArrayAsync())
        //    {
        //        if (!disp.ParentId.HasValue)
        //            subset.Add(disp);
        //        else
        //            break;
        //    }

        //    return subset;
        //}

        /// <summary>
        /// 获取节点及其子节点下的所有数据（包括子节点下的子节点），
        /// </summary>
        /// <typeparam name="K">TreeNode<K></typeparam>
        /// <param name="id">节点Id</param>
        /// <returns>
        /// 返回结果，形如：
        /// --node  --> Id == id
        ///     --node
        ///         --node
        ///         --node
        ///             --node
        ///     --node
        ///         --node
        /// </returns>
        public virtual K GetTreeNodeWithNestChildById(int id)
        {
            var result = EFContext.Set<K>()
                .AsNoTracking()
                .FirstOrDefault(o => !o.IsDeleted && o.Id == id);

            if (result == null)
                return result;

            var child = new List<K>();
            var nestChild = FindNodeListContainNestChildByParentId(id);
            foreach (var parentNode in nestChild.Where(m => m.ParentId == id).OrderBy(m => m.Index))
            {
                NestTreeNodeWithChild(parentNode, nestChild);
                child.Add(parentNode);
            }

            result.ChildNodes.AddRange(child);

            return result;
        }
        /// <summary>
        /// 获取节点及其子节点下的所有数据（包括子节点下的子节点），
        /// </summary>
        /// <typeparam name="K">TreeNode<K></typeparam>
        /// <param name="id">节点Id</param>
        /// <returns>
        /// 返回结果，形如：
        /// --node  --> Id == id
        ///     --node
        ///         --node
        ///         --node
        ///             --node
        ///     --node
        ///         --node
        /// </returns>
        public virtual async Task<K> GetTreeNodeWithNestChildByIdAsync(int id)
        {
            var result = await EFContext.Set<K>()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => !o.IsDeleted && o.Id == id);

            if (result == null)
                return result;

            var child = new List<K>();
            var nestChild = await FindNodeListContainNestChildByParentIdAsync(id);
            foreach (var parentNode in nestChild.Where(m => m.ParentId == id).OrderBy(m => m.Index))
            {
                NestTreeNodeWithChild(parentNode, nestChild);
                child.Add(parentNode);
            }

            result.ChildNodes.AddRange(child);

            return result;
        }

        /// <summary>
        /// 根据Id，获取包含其父节点（包含父节点上的父节点）及其子节点（子节点的子节点）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="id">根据节点Id，通过TreeCode字段（1-1001-10001）</param>
        /// <returns>
        /// 返回结果形如：
        /// --node  
        ///     --node
        ///         --node --> Id == id （TreeCode：1-1001-10001）
        ///             --node
        ///             --node
        /// </returns>
        public virtual List<K> FindTreeNodesWithNestParentAndChildById(int id)
        {
            var entity = EFContext.Set<K>()
                .AsNoTracking()
                .FirstOrDefault(m => !m.IsDeleted && m.Id == id);
            if (entity == null || (entity != null && string.IsNullOrEmpty(entity.TreeCode)))
                return new List<K>();

            return FindTreeNodesWithNestParentAndChildByTreeCode(entity.TreeCode);
        }
        /// <summary>
        /// 获取包含其父节点（包含父节点上的父节点）及其子节点（子节点的子节点）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="id">根据节点Id，通过TreeCode字段（1-1001-10001）</param>
        /// <returns>
        /// 返回结果形如：
        /// --node  
        ///     --node
        ///         --node --> Id == id （TreeCode：1-1001-10001）
        ///             --node
        ///             --node
        /// </returns>
        public virtual async Task<List<K>> FindTreeNodesWithNestParentAndChildByIdAsync(int id)
        {
            var entity = await EFContext.Set<K>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => !m.IsDeleted && m.Id == id);
            if (entity == null || (entity != null && string.IsNullOrEmpty(entity.TreeCode)))
                return new List<K>();

            return await FindTreeNodesWithNestParentAndChildByTreeCodeAsync(entity.TreeCode);
        }

        /// <summary>
        /// 根据TreeCode，获取包含其父节点（包含父节点上的父节点）及其子节点（子节点的子节点）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="treeCode">根据TreeCode字段（1-1001-10001）</param>
        /// <returns>
        /// 返回结果形如：
        /// --node  
        ///     --node
        ///         --node --> Id == id （TreeCode：1-1001-10001）
        ///             --node
        ///             --node
        /// </returns>
        public virtual List<K> FindTreeNodesWithNestParentAndChildByTreeCode(string treeCode)
        {
            var result = new List<K>();

            var ids = treeCode.ArrayFromCommaDelimitedIntegersBySplitChar('-');
            var entities = EFContext.Context.Set<K>()
                .Where(m => !m.IsDeleted && ids.Contains(m.Id))
                .AsNoTracking()
                .ToList();

            var child = FindNodeListContainNestChildByParentId(ids.LastOrDefault());
            entities.AddRange(child);
            foreach (var parentNode in entities.Where(m => m.ParentId == null).OrderBy(m => m.Index))
            {
                NestTreeNodeWithChild(parentNode, entities);
                result.Add(parentNode);
            }

            return result;
        }
        /// <summary>
        /// 获取包含其父节点（包含父节点上的父节点）及其子节点（子节点的子节点）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="treeCode">根据TreeCode字段（1-1001-10001）</param>
        /// <returns>
        /// 返回结果形如：
        /// --node  
        ///     --node
        ///         --node --> Id == id （TreeCode：1-1001-10001）
        ///             --node
        ///             --node
        /// </returns>
        public virtual async Task<List<K>> FindTreeNodesWithNestParentAndChildByTreeCodeAsync(string treeCode)
        {
            var result = new List<K>();

            var ids = treeCode.ArrayFromCommaDelimitedIntegersBySplitChar('-');
            var entities = await EFContext.Context.Set<K>()
                .Where(m => !m.IsDeleted && ids.Contains(m.Id))
                .AsNoTracking()
                .ToListAsync();

            var child = await FindNodeListContainNestChildByParentIdAsync(ids.LastOrDefault());
            entities.AddRange(child);
            foreach (var parentNode in entities.Where(m => m.ParentId == null).OrderBy(m => m.Index))
            {
                NestTreeNodeWithChild(parentNode, entities);
                result.Add(parentNode);
            }

            return result;
        }

        /// <summary>
        /// 根据条件，获取包含其父节点（包含父节点上的父节点）及其子节点（子节点的子节点）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="predicate">搜索条件</param>
        /// <returns>
        /// 返回结果形如：
        /// --node  
        ///     --node
        ///         --node --> filter （TreeCode：1-1001-10001）
        ///             --node
        ///             --node
        ///     --node --> filter （TreeCode：1-1002）
        ///         --node
        ///             --node
        /// </returns>
        public virtual List<K> FindTreeNodesWithNestParentAndChildByFilter(Expression<Func<K, bool>> predicate)
        {
            var nodes = EFContext.Set<K>().Where(predicate).AsNoTracking().ToList();
            if (nodes == null || !nodes.Any())
                return new List<K>();

            var allIds = new HashSet<int>();
            var lastIds = new HashSet<int>();
            nodes.ForEach(m => {
                var newIds = m.TreeCode.ArrayFromCommaDelimitedIntegersBySplitChar('-').ToList();
                lastIds.Add(newIds.LastOrDefault());
                newIds.ForEach(n => allIds.Add(n));
            });

            var entities = EFContext.Context.Set<K>()
                .Where(m => !m.IsDeleted && allIds.Contains(m.Id))
                .AsNoTracking()
                .ToList();

            var result = new List<K>();
            foreach (var id in lastIds)
            {
                var child = FindNodeListContainNestChildByParentId(id);
                foreach (var children in child)
                {
                    if (!entities.Any(m => m.Id == children.Id))
                    {
                        entities.Add(children);
                    }
                }
            }

            foreach (var parentNode in entities.Where(m => m.ParentId == null).OrderBy(m => m.Index))
            {
                NestTreeNodeWithChild(parentNode, entities);
                result.Add(parentNode);
            }

            return result;
        }
        /// <summary>
        /// 根据条件，获取包含其父节点（包含父节点上的父节点）及其子节点（子节点的子节点）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="predicate">搜索条件</param>
        /// <returns>
        /// 返回结果形如：
        /// --node  
        ///     --node
        ///         --node --> filter （TreeCode：1-1001-10001）
        ///             --node
        ///             --node
        ///     --node --> filter （TreeCode：1-1002）
        ///         --node
        ///             --node
        /// </returns>
        public virtual async Task<List<K>> FindTreeNodesWithNestParentAndChildByFilterAsync(Expression<Func<K, bool>> predicate)
        {
            var nodes = await EFContext.Set<K>().Where(predicate).AsNoTracking().ToListAsync();
            if (nodes == null || !nodes.Any())
                return new List<K>();

            var allIds = new HashSet<int>();
            var lastIds = new HashSet<int>();
            nodes.ForEach(m => {
                var newIds = m.TreeCode.ArrayFromCommaDelimitedIntegersBySplitChar('-').ToList();
                lastIds.Add(newIds.LastOrDefault());
                newIds.ForEach(n => allIds.Add(n));
            });

            var entities = await EFContext.Context.Set<K>()
                .Where(m => !m.IsDeleted && allIds.Contains(m.Id))
                .AsNoTracking()
                .ToListAsync();

            var result = new List<K>();
            foreach (var id in lastIds)
            {
                var child = await FindNodeListContainNestChildByParentIdAsync(id);
                foreach (var children in child)
                {
                    if (!entities.Any(m => m.Id == children.Id))
                    {
                        entities.Add(children);
                    }
                }
            }

            foreach (var parentNode in entities.Where(m => m.ParentId == null).OrderBy(m => m.Index))
            {
                NestTreeNodeWithChild(parentNode, entities);
                result.Add(parentNode);
            }

            return result;
        }

        /// <summary>
        /// 更新字段的TreeCode、Level、Leaf等字段值，TreeCode值格式（例如：1-1001-10005）
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public virtual bool UpdateExtendFields()
        {
            Expression<Func<K, bool>> predicate = m => true;
            return UpdateExtendFieldsByFilter(predicate);
        }
        /// <summary>
        /// 更新字段的TreeCode、Level、Leaf等字段值，TreeCode值格式（例如：1-1001-10005）
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateExtendFieldsAsync()
        {
            Expression<Func<K, bool>> predicate = m => true;
            return await UpdateExtendFieldsByFilterAsync(predicate);
        }

        /// <summary>
        /// 更新字段的TreeCode、Level、Leaf等字段值，TreeCode值格式（例如：1-1001-10005）
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateExtendFieldsByFilterAsync(Expression<Func<K, bool>> predicate)
        {
            var entities = await EFContext.Set<K>().Where(predicate).ToListAsync();
            foreach (var parentNode in entities.Where(m => m.ParentId == null))
            {
                parentNode.Level = 1;
                parentNode.TreeCode = parentNode.Id.ToString();

                SetTreeCodeWithNestChild(parentNode, entities);
            }
            return await UnitOfWork.CommitAsync() > 0;
        }
        /// <summary>
        /// 更新字段的TreeCode、Level、Leaf等字段值，TreeCode值格式（例如：1-1001-10005）
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public virtual bool UpdateExtendFieldsByFilter(Expression<Func<K, bool>> predicate)
        {
            var entities = EFContext.Set<K>().Where(predicate).ToList();
            foreach (var parentNode in entities.Where(m => m.ParentId == null))
            {
                parentNode.Level = 1;
                parentNode.TreeCode = parentNode.Id.ToString();

                SetTreeCodeWithNestChild(parentNode, entities);
            }

            return UnitOfWork.Commit() > 0;
        }

        /// <summary>
        /// 根据父节点Id获取其下级所有的列表数据（非树结构）
        /// </summary>
        /// <typeparam name="K">TreeNode<K></typeparam>
        /// <param name="id">父节点Id</param>
        /// <returns>
        /// 返回结果，形如：
        /// --node  --> ParentId == parentId
        ///     --node
        ///         --node
        ///         --node
        ///             --node
        ///     --node
        ///         --node
        /// --node  --> ParentId == parentId
        ///     --node
        ///         --node
        ///     --node
        /// </returns>
        public virtual List<K> FindNodeListContainNestChildByParentId(int? parentId)
        {
            var result = EFContext.Set<K>()
                .Where(o => !o.IsDeleted && o.ParentId == parentId)
                .AsNoTracking()
                .ToList();

            var ids = result.OrderBy(m => m.Index).Select(m => m.Id).ToList();
            result.AddRange(FindNodeListContainNestChildByParentIds(ids));
            return result;
        }
        /// <summary>
        /// 根据父节点Id获取其下级所有的列表数据（非树结构）
        /// </summary>
        /// <typeparam name="K">TreeNode<K></typeparam>
        /// <param name="id">父节点Id</param>
        /// <returns>
        /// 返回结果，形如：
        /// --node  --> ParentId == parentId
        ///     --node
        ///         --node
        ///         --node
        ///             --node
        ///     --node
        ///         --node
        /// --node  --> ParentId == parentId
        ///     --node
        ///         --node
        ///     --node
        /// </returns>
        public virtual async Task<List<K>> FindNodeListContainNestChildByParentIdAsync(int? parentId)
        {
            var result = await EFContext.Set<K>()
                .Where(o => !o.IsDeleted && o.ParentId == parentId)
                .AsNoTracking()
                .ToListAsync();

            var ids = result.OrderBy(m => m.Index).Select(m => m.Id).ToList();
            result.AddRange(FindNodeListContainNestChildByParentIds(ids));
            return result;
        }
        /// <summary>
        /// 根据父节点Id列表获取其下级所有的列表数据（非树结构）
        /// </summary>
        /// <typeparam name="K">TreeNode<K></typeparam>
        /// <param name="id">父节点Id</param>
        /// <returns>
        /// 返回结果，形如：
        /// --node  --> ParentId == parentId
        ///     --node
        ///         --node
        ///         --node
        ///             --node
        ///     --node
        ///         --node
        /// --node  --> ParentId == parentId
        ///     --node
        ///         --node
        ///     --node
        /// </returns>
        public virtual List<K> FindNodeListContainNestChildByParentIds(List<int> parentIds)
        {
            if (parentIds == null || !parentIds.Any()) return new List<K>();

            var result = EFContext.Set<K>()
                .Where(o => !o.IsDeleted
                        && o.ParentId != null
                        && parentIds.Contains(o.ParentId.Value))
                .AsNoTracking()
                .ToList();

            var pIds = result.OrderBy(m => m.Index).Select(m => m.Id).ToList();
            if (pIds == null || !pIds.Any()) return result;

            result.AddRange(FindNodeListContainNestChildByParentIds(pIds));
            return result;
        }
        /// <summary>
        /// 根据父节点Id获取其下级所有的列表数据（非树结构）
        /// </summary>
        /// <typeparam name="K">TreeNode<K></typeparam>
        /// <param name="id">父节点Id</param>
        /// <returns>
        /// 返回结果，形如：
        /// --node  --> ParentId == parentId
        ///     --node
        ///         --node
        ///         --node
        ///             --node
        ///     --node
        ///         --node
        /// --node  --> ParentId == parentId
        ///     --node
        ///         --node
        ///     --node
        /// </returns>
        public virtual async Task<List<K>> GetNodeListContainNestChildByParentIdsAsync(List<int> parentIds)
        {
            if (parentIds == null || !parentIds.Any()) return new List<K>();

            var result = await EFContext.Set<K>()
                .Where(o => !o.IsDeleted
                        && o.ParentId != null
                        && parentIds.Contains(o.ParentId.Value))
                .AsNoTracking()
                .ToListAsync();

            var pIds = result.OrderBy(m => m.Index).Select(m => m.Id).ToList();
            if (pIds == null || !pIds.Any()) return result;

            result.AddRange(FindNodeListContainNestChildByParentIds(pIds));
            return result;
        }

        /// <summary>
        /// 根据筛选条件，获取符合条件的子节点及其子节点的所有数据（包括子节点下的子节点）
        /// </summary>
        /// <typeparam name="K">TreeNode<K></typeparam>
        /// <param name="predicate">筛选条件</param>
        /// <returns>
        /// 返回结果，形如：
        /// --node  --> filter
        ///     --node
        ///         --node
        ///         --node
        ///             --node
        ///     --node
        ///         --node
        /// --node (不包含)
        ///     --node --> filter
        ///         --node
        ///     --node (不包含)
        /// </returns>
        public virtual List<K> FindNodeListContainNestChildByFilter(Expression<Func<K, bool>> predicate)
        {
            var result = EFContext.Set<K>()
                .Where(predicate)
                .AsNoTracking()
                .ToList();

            var ids = result.OrderBy(m => m.Index).Select(m => m.Id).ToList();
            result.AddRange(FindNodeListContainNestChildByParentIds(ids));
            return result;
        }

        protected void NestTreeNodeWithChild(K parent, List<K> entities)
        {
            var child = entities.Where(m => m.ParentId.Equals(parent.Id)).ToList();
            parent.ChildNodes = child;
            foreach (var children in child.OrderBy(m => m.Index))
            {
                children.ParentNode = parent;
                NestTreeNodeWithChild(children, entities);
            }
        }
        private void SetTreeCodeWithNestChild(K parent, List<K> entities)
        {
            var child = entities.Where(m => m.ParentId.Equals(parent.Id));
            parent.Leaf = !child.Any();
            foreach (var children in child)
            {
                children.Level = parent.Level + 1;
                children.TreeCode = parent.TreeCode + DatabaseExtensions.TreeCodeSplitIdWithChar + children.Id;

                SetTreeCodeWithNestChild(children, entities);
            }
        }

        #endregion
    }
}