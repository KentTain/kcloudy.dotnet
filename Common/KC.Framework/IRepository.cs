using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using KC.Framework.Base;

namespace KC.Framework
{
    public interface IRepository<T> where T : EntityBase
    {
        /// <summary>
        /// 查询出所有数据
        /// </summary>
        /// <returns></returns>
        IList<T> FindAll();
        /// <summary>
        /// 根据条件（不排序），筛选出的数据
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        IList<T> FindAll(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 根据条件排序出的数据
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        IList<T> FindAll<K>(Expression<Func<T, K>> keySelector, bool ascending = true);
        /// <summary>
        /// 根据条件及排序，筛选出的数据
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        IList<T> FindAll<K>(Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true);
        
        /// <summary>
        ///     插入实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否成功 </returns>
        bool Add(T entity, bool isSave = true);
        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否成功 </returns>
        bool Modify(T entity, bool isSave = true);
        /// <summary>
        ///     删除实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否删除成功 </returns>
        bool Remove(T entity, bool isSave = true);
        /// <summary>
        ///     删除实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        int Remove(IEnumerable<T> entities, bool isSave = true);
        /// <summary>
        ///     删除所有记录
        /// </summary>
        /// <returns> 是否删除成功 </returns>
        bool RemoveAll();
    }
}
