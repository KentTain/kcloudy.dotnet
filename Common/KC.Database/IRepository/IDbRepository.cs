using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Framework;
using KC.Framework.Base;
using Microsoft.EntityFrameworkCore;

namespace KC.Database.IRepository
{
    public interface IDbRepository<T> : IRepository<T> where T : EntityBase
    {
        #region 属性
        /// <summary>
        ///     获取 当前实体的查询数据集
        /// </summary>
        IQueryable<T> Entities { get; }

        IUnitOfWork<DbContext> UnitOfWork { get; }
        #endregion

        #region Exist
        /// <summary>
        ///     根据条件判断是否存在
        /// </summary>
        /// <param name="predicate"> 条件 </param>
        /// <returns> 是否存在 </returns>
        bool ExistByFilter(Expression<Func<T, bool>> predicate);
        /// <summary>
        ///     根据条件判断是否存在（异步）
        /// </summary>
        /// <param name="predicate"> 条件 </param>
        /// <returns> 是否存在 </returns>
        Task<bool> ExistByFilterAsync(Expression<Func<T, bool>> predicate);
        #endregion

        #region Search
        /// <summary>
        /// 根据主键Id获取第一条数据
        /// </summary>
        /// <param name="keyValues">主键Id</param>
        /// <returns></returns>
        T GetById(params object[] keyValues);
        /// <summary>
        /// 根据主键Id获取第一条数据（异步）
        /// </summary>
        /// <param name="keyValues">主键Id</param>
        /// <returns></returns>
        Task<T> GetByIdAsync(params object[] keyValues);
        /// <summary>
        /// 根据条件获取第一条数据
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        T GetByFilter(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 根据条件获取第一条数据（异步）
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        Task<T> GetByFilterAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 根据条件获取第一条数据
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        T GetByFilter<K>(Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true);
        /// <summary>
        /// 根据条件获取第一条数据（异步）
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        Task<T> GetByFilterAsync<K>(Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true);

        /// <summary>
        /// 查询出所有数据（异步）
        /// </summary>
        /// <returns></returns>
        Task<IList<T>> FindAllAsync();
        /// <summary>
        /// 根据条件排序出的数据（异步）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        Task<IList<T>> FindAllAsync<K>(Expression<Func<T, K>> keySelector, bool ascending = true);
        /// <summary>
        /// 根据条件（不排序），筛选出的数据（异步）
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        Task<IList<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 根据条件及排序，筛选出的数据（异步）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        Task<IList<T>> FindAllAsync<K>(Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true);

        /// <summary>
        /// 带条件及排序的分页算法（不含数据记录数）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        IList<T> FindPagenatedList<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true);
        /// <summary>
        /// 带条件及排序的分页算法（不含数据记录数）（异步）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        IList<T> FindPagenatedList<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true);
        /// <summary>
        /// 带条件及排序的分页算法（不含数据总记录数）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="predicate">条件</param>
        /// <param name="sortProperty">排序字段名称</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        Task<IList<T>> FindPagenatedListAsync<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true);
        /// <summary>
        /// 带条件及排序的分页算法（不含数据总记录数）（异步）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="predicate">条件</param>
        /// <param name="sortProperty">排序字段名称</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        Task<IList<T>> FindPagenatedListAsync<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true);
        /// <summary>
        /// 带条件及排序的分页算法（含数据记录数）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="predicate">条件</param>
        /// <param name="sortProperty">排序字段名称</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        Tuple<int, IList<T>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true);
        /// <summary>
        /// 带条件及排序的分页算法（含数据记录数）（异步）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="predicate">条件</param>
        /// <param name="sortProperty">排序字段名称</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        Tuple<int, IList<T>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true);
        /// <summary>
        /// 带条件及排序的分页算法（含数据记录数）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        Task<Tuple<int, IList<T>>> FindPagenatedListWithCountAsync<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true);
        /// <summary>
        /// 带条件及排序的分页算法（含数据记录数）（异步）
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        Task<Tuple<int, IList<T>>> FindPagenatedListWithCountAsync<K>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true);
        #endregion

        #region Add
        /// <summary>
        ///     插入实体记录（异步）
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        Task<bool> AddAsync(T entity, bool isSave = true);

        /// <summary>
        ///     批量插入实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        int Add(IEnumerable<T> entities, bool isSave = true);
        /// <summary>
        ///     批量插入实体记录集合（异步）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        Task<bool> AddAsync(IEnumerable<T> entities, bool isSave = true);
        
        /// <summary>
        ///     批量插入实体记录集合（海量数据插入）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        void BulkInsert(IEnumerable<T> entities);

        /// <summary>
        ///     批量插入实体记录集合（海量数据插入）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        Task BulkInsertAsync(IEnumerable<T> entities);
        #endregion

        #region Modify
        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否成功 </returns>
        Task<bool> ModifyAsync(T entity, bool isSave = true);
        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="properties"> 更新字段名称的列表 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否成功 </returns>
        bool Modify(T entity, string[] properties, bool isSave = true);
        /// <summary>
        ///     更新实体记录（异步）
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="properties"> 更新字段名称的列表 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否成功 </returns>
        Task<bool> ModifyAsync(T entity, string[] properties, bool isSave = true);
        /// <summary>
        ///     更新实体列表
        /// </summary>
        /// <param name="entities"> 实体对象列表 </param>
        /// <param name="properties"> 更新字段名称的列表 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否成功 </returns>
        int Modify(IEnumerable<T> entities, string[] properties = null, bool isSave = true);
        /// <summary>
        ///     更新实体列表（异步）
        /// </summary>
        /// <param name="entities"> 实体对象列表 </param>
        /// <param name="properties"> 更新字段名称的列表 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否成功 </returns>
        Task<int> ModifyAsync(IEnumerable<T> entities, string[] properties = null, bool isSave = true);

        /// <summary>
        ///     批量更新实体记录集合（海量数据更新）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        void BulkUpdate(IEnumerable<T> entities);

        /// <summary>
        ///     批量更新实体记录集合（海量数据更新）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        Task BulkUpdateAsync(IEnumerable<T> entities);
        #endregion

        #region Remove & SoftRemove

        /// <summary>
        ///     删除指定编号的记录
        /// </summary>
        /// <param name="id"> 实体记录编号 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否删除成功 </returns>
        bool RemoveById(object id, bool isSave = true);
        /// <summary>
        ///     删除指定编号的记录（异步）
        /// </summary>
        /// <param name="id"> 实体记录编号 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否删除成功 </returns>
        Task<bool> RemoveByIdAsync(object id, bool isSave = true);
        /// <summary>
        ///     删除实体记录（异步）
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否删除成功 </returns>
        Task<bool> RemoveAsync(T entity, bool isSave = true);
        /// <summary>
        ///     删除实体记录集合（异步）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        Task<int> RemoveAsync(IEnumerable<T> entities, bool isSave = true);
        /// <summary>
        ///     删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        int Remove(Expression<Func<T, bool>> predicate, bool isSave = true);
        /// <summary>
        ///     删除所有符合特定表达式的数据（异步）
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        Task<int> RemoveAsync(Expression<Func<T, bool>> predicate, bool isSave = true);
        /// <summary>
        ///     删除所有记录（异步）
        /// </summary>
        /// <returns> 是否删除成功 </returns>
        Task<bool> RemoveAllAsync();
        /// <summary>
        ///     批量删除实体记录集合（海量数据更新）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        void BulkDelete(IEnumerable<T> entities);

        /// <summary>
        ///     批量删除实体记录集合（海量数据更新）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        Task BulkDeleteAsync(IEnumerable<T> entities);

        /// <summary>
        ///     删除指定编号的记录（逻辑删除）
        /// </summary>
        /// <param name="id"> 实体记录编号 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否删除成功 </returns>
        bool SoftRemoveById(object id, bool isSave = true);
        /// <summary>
        ///     删除实体对象（逻辑删除）
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否删除成功 </returns>
        bool SoftRemove(T entity, bool isSave = true);
        /// <summary>
        ///     删除实体对象（逻辑删除）（异步）
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 是否删除成功 </returns>
        Task<bool> SoftRemoveAsync(T entity, bool isSave = true);
        /// <summary>
        ///     根据条件，删除实体对象（逻辑删除）
        /// </summary>
        /// <param name="predicate"> 条件 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 删除的记录数 </returns>
        int SoftRemove(Expression<Func<T, bool>> predicate, bool isSave = true);
        /// <summary>
        ///     根据条件，删除实体对象（逻辑删除）（异步）
        /// </summary>
        /// <param name="predicate"> 条件 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 删除的记录数 </returns>
        Task<int> SoftRemoveAsync(Expression<Func<T, bool>> predicate, bool isSave = true);
        /// <summary>
        ///     删除实体对象列表（逻辑删除）
        /// </summary>
        /// <param name="entities"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 删除的记录数 </returns>
        int SoftRemove(IEnumerable<T> entities, bool isSave = true);
        /// <summary>
        ///     删除实体对象列表（逻辑删除）（异步）
        /// </summary>
        /// <param name="entities"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 删除的记录数 </returns>
        Task<int> SoftRemoveAsync(IEnumerable<T> entities, bool isSave = true);
        #endregion

        #region Count
        int GetRecordCount();
        Task<int> GetRecordCountAsync();

        int GetRecordCount(Expression<Func<T, bool>> predicate);
        Task<int> GetRecordCountAsync(Expression<Func<T, bool>> predicate);

        #endregion

        #region 执行数据库脚本
        bool ExecuteSql(string query, params object[] parameters);
        Task<bool> ExecuteSqlAsync(string query, params object[] parameters);
        #endregion
    }
}
