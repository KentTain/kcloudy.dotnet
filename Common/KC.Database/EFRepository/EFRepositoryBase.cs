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

namespace KC.Database.EFRepository
{
    /// <summary>
    ///     EntityFramework仓储操作基类
    /// </summary>
    /// <typeparam name="T">动态实体类型</typeparam>
    public abstract class EFRepositoryBase<T> : IDbRepository<T> where T : EntityBase
    {
        protected EFRepositoryBase(EFUnitOfWorkContextBase unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        #region 属性

        /// <summary>
        ///     获取 仓储上下文的实例
        /// </summary>
        //[Import]
        public IUnitOfWork<DbContext> UnitOfWork { get; private set; }

        /// <summary>
        ///     获取或设置 EntityFramework的数据仓储上下文
        /// </summary>
        protected IUnitOfWorkContext<DbContext> EFContext
        {
            get
            {
                if (UnitOfWork is IUnitOfWorkContext<DbContext>)
                {
                    return UnitOfWork as IUnitOfWorkContext<DbContext>;
                }
                throw new DataAccessException(string.Format("数据仓储上下文对象类型不正确，应为IRepositoryContext，实际为 {0}", UnitOfWork.GetType().Name));
            }
        }

        /// <summary>
        ///     获取 当前实体的查询数据集
        /// </summary>
        public virtual IQueryable<T> Entities
        {
            get { return EFContext.Set<T>(); }
        }

        #endregion 属性

        #region 公共方法

        #region 是否存在
        public virtual bool ExistByFilter(Expression<Func<T, bool>> predicate)
        {
            return EFContext.Set<T>().AsNoTracking().Any(predicate);
        }
        public virtual async Task<bool> ExistByFilterAsync(Expression<Func<T, bool>> predicate)
        {
            return await EFContext.Set<T>().AsNoTracking().AnyAsync(predicate);
        }
        #endregion

        #region Search

        public virtual T GetById(params object[] keyValues)
        {
            var result = EFContext.Set<T>().Find(keyValues);
            //EFContext.Context.Detach(result);
            return result;
        }
        public async virtual Task<T> GetByIdAsync(params object[] keyValues)
        {
            var result = await EFContext.Set<T>().FindAsync(keyValues);
            //EFContext.Context.Detach(result);
            return result;
        }
        /// <summary>
        /// 根据条件获取第一条数据
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public virtual T GetByFilter(Expression<Func<T, bool>> predicate)
        {
            return EFContext.Set<T>().AsNoTracking().FirstOrDefault(predicate);
        }
        public async virtual Task<T> GetByFilterAsync(Expression<Func<T, bool>> predicate)
        {
            return await EFContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate);
        }
        /// <summary>
        /// 根据条件获取第一条数据
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        public virtual T GetByFilter<K>(Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return this.FindAll<K>(predicate, keySelector, ascending).FirstOrDefault();
        }
        public virtual async Task<T> GetByFilterAsync<K>(Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            var list = await this.FindAllAsync<K>(predicate, keySelector, ascending);
            return list.FirstOrDefault();
        }


        /// <summary>
        /// 查询出所有数据
        /// </summary>
        /// <returns></returns>
        public virtual IList<T> FindAll()
        {
            return EFContext.Set<T>().AsNoTracking().ToList();
        }
        public async virtual Task<IList<T>> FindAllAsync()
        {
            return await EFContext.Set<T>().AsNoTracking().ToListAsync();
        }
        /// <summary>
        /// 根据条件排序出的数据
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        public virtual IList<T> FindAll<K>(Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? EFContext.Set<T>().OrderBy(keySelector).AsNoTracking().ToList()
                : EFContext.Set<T>().OrderByDescending(keySelector).AsNoTracking().ToList();
        }
        public async virtual Task<IList<T>> FindAllAsync<K>(Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? await EFContext.Set<T>().OrderBy(keySelector).AsNoTracking().ToListAsync()
                : await EFContext.Set<T>().OrderByDescending(keySelector).AsNoTracking().ToListAsync();
        }
        /// <summary>
        /// 根据条件（不排序），筛选出的数据
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public virtual IList<T> FindAll(Expression<Func<T, bool>> predicate)
        {
            return EFContext.Set<T>().Where(predicate).AsNoTracking().ToList();
        }
        public async virtual Task<IList<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await EFContext.Set<T>().Where(predicate).AsNoTracking().ToListAsync();
        }
        /// <summary>
        /// 根据条件及排序，筛选出的数据
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        public virtual IList<T> FindAll<K>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, K>> keySelector,
            bool ascending = true)
        {
            return @ascending
                ? EFContext.Set<T>().Where(predicate).OrderBy(keySelector).AsNoTracking().ToList()
                : EFContext.Set<T>().Where(predicate).OrderByDescending(keySelector).AsNoTracking().ToList();
        }
        public async virtual Task<IList<T>> FindAllAsync<K>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, K>> keySelector,
            bool ascending = true)
        {
            return @ascending
                ? await EFContext.Set<T>().Where(predicate).OrderBy(keySelector).AsNoTracking().ToListAsync()
                : await EFContext.Set<T>().Where(predicate).OrderByDescending(keySelector).AsNoTracking().ToListAsync();
        }
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
        public virtual IList<T> FindPagenatedList<K>(
            int pageIndex, int pageSize, Expression<Func<T, bool>> predicate,
            Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? EFContext.Set<T>().Where(predicate).OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList()
                : EFContext.Set<T>().Where(predicate).OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
        }
        public async virtual Task<IList<T>> FindPagenatedListAsync<K>(
            int pageIndex, int pageSize, Expression<Func<T, bool>> predicate,
            Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? await EFContext.Set<T>().Where(predicate).OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync()
                : await EFContext.Set<T>().Where(predicate).OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
        }
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
        public virtual IList<T> FindPagenatedList<K>(
            int pageIndex, int pageSize, Expression<Func<T, bool>> predicate,
           string sortProperty, bool ascending = true)
        {
            var q = EFContext.Set<T>().Where(predicate).AsNoTracking();
            return q.SingleOrderBy(sortProperty, ascending).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        public async virtual Task<IList<T>> FindPagenatedListAsync<K>(
            int pageIndex, int pageSize, Expression<Func<T, bool>> predicate,
           string sortProperty, bool ascending = true)
        {
            var q = EFContext.Set<T>().Where(predicate).AsNoTracking();
            return await q.SingleOrderBy(sortProperty, ascending).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }
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
        public virtual Tuple<int, IList<T>> FindPagenatedListWithCount<K>(
            int pageIndex, int pageSize, Expression<Func<T, bool>> predicate,
            string sortProperty, bool ascending = true)
        {
            var q = EFContext.Set<T>().Where(predicate).AsNoTracking();
            int recordCount = q.Count();
            if (recordCount == 0)
                return new Tuple<int, IList<T>>(0, new List<T>());

            var data = q.SingleOrderBy(sortProperty, ascending).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new Tuple<int, IList<T>>(recordCount, data);
        }
        public async virtual Task<Tuple<int, IList<T>>> FindPagenatedListWithCountAsync<K>(
            int pageIndex, int pageSize, Expression<Func<T, bool>> predicate,
            string sortProperty, bool ascending = true)
        {
            var q = EFContext.Set<T>().Where(predicate).AsNoTracking();
            int recordCount = q.Count();
            if (recordCount == 0)
                return new Tuple<int, IList<T>>(0, new List<T>());

            var data = await q.SingleOrderBy(sortProperty, ascending).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<int, IList<T>>(recordCount, data);
        }
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
        public virtual Tuple<int, IList<T>> FindPagenatedListWithCount<K>(
            int pageIndex, int pageSize, Expression<Func<T, bool>> predicate,
            Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            var databaseItems = EFContext.Set<T>().Where(predicate).AsNoTracking();
            int recordCount = databaseItems.Count();
            if (recordCount == 0)
                return new Tuple<int, IList<T>>(0, new List<T>());

            return @ascending
                ? new Tuple<int, IList<T>>(recordCount, databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                : new Tuple<int, IList<T>>(recordCount, databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }
        public async virtual Task<Tuple<int, IList<T>>> FindPagenatedListWithCountAsync<K>(
            int pageIndex, int pageSize, Expression<Func<T, bool>> predicate,
            Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            var databaseItems = EFContext.Set<T>().Where(predicate).AsNoTracking();
            int recordCount = databaseItems.Count();
            if (recordCount == 0)
                return new Tuple<int, IList<T>>(0, new List<T>());

            return @ascending
                ? new Tuple<int, IList<T>>(recordCount, await databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync())
                : new Tuple<int, IList<T>>(recordCount, await databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync());
        }

        /// <summary>
        /// 树结构分页查询(无数据总数，无排序)
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="including"></param>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        public virtual IList<T> FindPagenatedTreeList<K>(
            int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including,
            Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? EFContext.Set<T>().Include(including).Where(predicate).OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList()
                : EFContext.Set<T>().Include(including).Where(predicate).OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
        }
        public async virtual Task<IList<T>> FindPagenatedTreeListAsync<K>(
            int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including,
            Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? await EFContext.Set<T>().Include(including).Where(predicate).OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync()
                : await EFContext.Set<T>().Include(including).Where(predicate).OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// 数结构分页查询(无数据总数，有排序)
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="including"></param>
        /// <param name="predicate">条件</param>
        /// <param name="sortProperty">排序字段名称</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        public virtual IList<T> FindPagenatedTreeList<K>(
            int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including,
            Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true)
        {
            var q = EFContext.Set<T>().Include(including).Where(predicate).AsNoTracking();
            return q.SingleOrderBy(sortProperty, ascending).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        public async virtual Task<IList<T>> FindPagenatedTreeListAsync<K>(
            int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including,
            Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true)
        {
            var q = EFContext.Set<T>().Include(including).Where(predicate).AsNoTracking();
            return await q.SingleOrderBy(sortProperty, ascending).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        /// <summary>
        /// 数结构分页查询(有数据总数，无排序)
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="including"></param>
        /// <param name="predicate">条件</param>
        /// <param name="keySelector">排序字段</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        public virtual Tuple<int, IList<T>> FindPagenatedTreeListWithCount<K>(
            int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including,
            Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            var databaseItems = EFContext.Set<T>().Include(including).Where(predicate).AsNoTracking();
            int recordCount = databaseItems.Count();
            if (recordCount == 0)
                return new Tuple<int, IList<T>>(0, new List<T>());

            return @ascending
                ? new Tuple<int, IList<T>>(recordCount, databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                : new Tuple<int, IList<T>>(recordCount, databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }
        public async virtual Task<Tuple<int, IList<T>>> FindPagenatedTreeListWithCountAsync<K>(
            int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including,
            Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            var databaseItems = EFContext.Set<T>().Include(including).Where(predicate).AsNoTracking();
            int recordCount = databaseItems.Count();
            if (recordCount == 0)
                return new Tuple<int, IList<T>>(0, new List<T>());

            return @ascending
                ? new Tuple<int, IList<T>>(recordCount, await databaseItems.OrderBy(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync())
                : new Tuple<int, IList<T>>(recordCount, await databaseItems.OrderByDescending(keySelector).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync());
        }
        /// <summary>
        /// 树结构分页查询(有数据总数，有排序)
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="including"></param>
        /// <param name="predicate">条件</param>
        /// <param name="sortProperty">排序字段名称</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        public virtual Tuple<int, IList<T>> FindPagenatedTreeListWithCount<K>(
            int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including,
            Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true)
        {
            var q = EFContext.Set<T>().Include(including).Where(predicate).AsNoTracking();
            var recordCount = q.Count();
            if (recordCount == 0)
                return new Tuple<int, IList<T>>(0, new List<T>());

            var data = q.SingleOrderBy(sortProperty, ascending).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new Tuple<int, IList<T>>(recordCount, data);
        }
        public async virtual Task<Tuple<int, IList<T>>> FindPagenatedTreeListWithCountAsync<K>(
            int pageIndex, int pageSize, Expression<Func<T, IEnumerable<IEnumerable<IList<T>>>>> including,
            Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true)
        {
            var q = EFContext.Set<T>().Include(including).Where(predicate).AsNoTracking();
            var recordCount = q.Count();
            if (recordCount == 0)
                return new Tuple<int, IList<T>>(0, new List<T>());

            var data = await q.SingleOrderBy(sortProperty, ascending).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<int, IList<T>>(recordCount, data);
        }
        #endregion

        #region 新增实体对象

        /// <summary>
        ///     插入实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual bool Add(T entity, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            EFContext.RegisterNew(entity);
            return isSave ? EFContext.Commit() > 0 : false;
        }
        /// <summary>
        ///     插入实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public async virtual Task<bool> AddAsync(T entity, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            await EFContext.Context.Set<T>().AddAsync(entity);
            return isSave ? await EFContext.CommitAsync() > 0 : false;
        }

        /// <summary>
        ///     批量插入实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual int Add(IEnumerable<T> entities, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entities, "entities");
            EFContext.RegisterNew(entities);
            return isSave ? EFContext.Commit() : 0;
        }
        /// <summary>
        ///     批量插入实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public async virtual Task<bool> AddAsync(IEnumerable<T> entities, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            foreach (var entity in entities)
            {
                await EFContext.Context.Set<T>().AddAsync(entity);
            }
            return isSave ? await EFContext.CommitAsync() > 0 : false;
        }

        /// <summary>
        ///     批量插入实体记录集合（海量数据插入）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        public virtual void BulkInsert(IEnumerable<T> entities)
        {
            EFContext.Context.BulkInsert<T>(entities);
        }

        /// <summary>
        ///     批量插入实体记录集合（海量数据插入）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        public async virtual Task BulkInsertAsync(IEnumerable<T> entities)
        {
            await EFContext.Context.BulkInsertAsync<T>(entities);
        }
        #endregion 新增实体对象

        #region 物理删除

        /// <summary>
        ///     删除指定编号的记录
        /// </summary>
        /// <param name="id"> 实体记录编号 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual bool RemoveById(object id, bool isSave = true)
        {
            //PublicHelper.CheckArgument(id, "id");
            T entity = EFContext.Set<T>().Find(id);
            return entity != null ? Remove(entity, isSave) : false;
        }
        /// <summary>
        ///     删除指定编号的记录
        /// </summary>
        /// <param name="id"> 实体记录编号 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public async virtual Task<bool> RemoveByIdAsync(object id, bool isSave = true)
        {
            //PublicHelper.CheckArgument(id, "id");
            T entity = await EFContext.Set<T>().FindAsync(id);
            return entity != null ? await RemoveAsync(entity, isSave) : false;
        }

        /// <summary>
        ///     删除实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual bool Remove(T entity, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            EFContext.RegisterDeleted(entity);
            return isSave && EFContext.Commit() > 0;
        }
        /// <summary>
        ///     删除实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public async virtual Task<bool> RemoveAsync(T entity, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            EFContext.RegisterDeleted(entity);
            return isSave ? await EFContext.CommitAsync() > 0 : false;
        }
        /// <summary>
        ///     删除实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual int Remove(IEnumerable<T> entities, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entities, "entities");
            EFContext.RegisterDeleted(entities);
            return isSave ? EFContext.Commit() : 0;
        }
        /// <summary>
        ///     删除实体记录集合
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public async virtual Task<int> RemoveAsync(IEnumerable<T> entities, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entities, "entities");
            EFContext.RegisterDeleted(entities);
            return isSave ? await EFContext.CommitAsync() : 0;
        }
        /// <summary>
        ///     删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual int Remove(Expression<Func<T, bool>> predicate, bool isSave = true)
        {
            //PublicHelper.CheckArgument(predicate, "predicate");
            var entities = EFContext.Set<T>().Where(predicate).ToList();
            return entities.Count > 0 ? Remove(entities, isSave) : 0;
        }
        /// <summary>
        ///     删除所有符合特定表达式的数据
        /// </summary>
        /// <param name="predicate"> 查询条件谓语表达式 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public async virtual Task<int> RemoveAsync(Expression<Func<T, bool>> predicate, bool isSave = true)
        {
            //PublicHelper.CheckArgument(predicate, "predicate");
            var entities = await EFContext.Set<T>().Where(predicate).ToListAsync();
            return entities.Count > 0 ? await RemoveAsync(entities, isSave) : 0;
        }

        /// <summary>
        ///     批量插入实体记录集合（海量数据插入）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        public virtual void BulkDelete(IEnumerable<T> entities)
        {
            EFContext.Context.BulkDelete<T>(entities);
        }

        /// <summary>
        ///     批量插入实体记录集合（海量数据插入）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        public async virtual Task BulkDeleteAsync(IEnumerable<T> entities)
        {
            await EFContext.Context.BulkDeleteAsync<T>(entities);
        }

        public virtual bool RemoveAll()
        {
            var entities = EFContext.Set<T>().ToList();
            if (entities.Count > 0)
                EFContext.Context.BulkDelete<T>(entities);
            return true;
        }
        public async virtual Task<bool> RemoveAllAsync()
        {
            var entities = await EFContext.Set<T>().ToListAsync();
            if (entities.Count > 0)
                await EFContext.Context.BulkDeleteAsync<T>(entities);

            return true;
        }
        #endregion 物理删除

        #region 逻辑删除：IsDeleted设为：true

        public virtual bool SoftRemoveById(object id, bool isSave = true)
        {
            //PublicHelper.CheckArgument(id, "id");
            T entity = EFContext.Set<T>().Find(id);
            return entity != null ? SoftRemove(entity, isSave) : false;
        }

        public virtual bool SoftRemove(T entity, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            if (!(entity is Entity))
            {
                throw new ArgumentException("T must be an Entity type that is derived from the Entity class in the namespace KC.Database.");
            }

            var aEntity = entity as Entity;
            aEntity.IsDeleted = true;

            EFContext.RegisterSoftDeleted(entity);
            return isSave ? EFContext.Commit() > 0 : false;
        }
        public async virtual Task<bool> SoftRemoveAsync(T entity, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            if (!(entity is Entity))
            {
                throw new ArgumentException("T must be an Entity type that is derived from the Entity class in the namespace KC.Database.");
            }

            var aEntity = entity as Entity;
            aEntity.IsDeleted = true;

            EFContext.RegisterSoftDeleted(entity);
            return isSave ? await EFContext.CommitAsync() > 0 : false;
        }
        public virtual int SoftRemove(IEnumerable<T> entities, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entities, "entities");
            foreach (T entity in entities)
            {
                if (!(entity is Entity))
                {
                    throw new ArgumentException("T must be an Entity type that is derived from the Entity class in the namespace KC.Database.");
                }

                var aEntity = entity as Entity;
                aEntity.IsDeleted = true;

                EFContext.RegisterSoftDeleted(entity);
            }

            return isSave ? EFContext.Commit() : 0;
        }
        public async virtual Task<int> SoftRemoveAsync(IEnumerable<T> entities, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entities, "entities");
            foreach (T entity in entities)
            {
                if (!(entity is Entity))
                {
                    throw new ArgumentException("T must be an Entity type that is derived from the Entity class in the namespace KC.Database.");
                }

                var aEntity = entity as Entity;
                aEntity.IsDeleted = true;

                EFContext.RegisterSoftDeleted(entity);
            }

            return isSave ? await EFContext.CommitAsync() : 0;
        }
        public virtual int SoftRemove(Expression<Func<T, bool>> predicate, bool isSave = true)
        {
            //PublicHelper.CheckArgument(predicate, "predicate");
            var entities = EFContext.Set<T>().Where(predicate).ToList();
            return entities.Count > 0 ? SoftRemove(entities, isSave) : 0;
        }
        public async virtual Task<int> SoftRemoveAsync(Expression<Func<T, bool>> predicate, bool isSave = true)
        {
            //PublicHelper.CheckArgument(predicate, "predicate");
            var entities = await EFContext.Set<T>().Where(predicate).ToListAsync();
            return entities.Count > 0 ? await SoftRemoveAsync(entities, isSave) : 0;
        }
        #endregion 逻辑删除

        #region 更新实体对象

        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual bool Modify(T entity, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            EFContext.RegisterModified(entity);
            return isSave && EFContext.Commit() > 0;
        }
        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public async virtual Task<bool> ModifyAsync(T entity, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            EFContext.RegisterModified(entity);
            return isSave && await EFContext.CommitAsync() > 0;
        }
        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="properties">需部分更新的属性值名称</param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual bool Modify(T entity, string[] properties, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            EFContext.RegisterModified(entity, properties);
            return isSave && EFContext.Commit() > 0;
        }
        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entity"> 实体对象 </param>
        /// <param name="properties">需部分更新的属性值名称</param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public async virtual Task<bool> ModifyAsync(T entity, string[] properties, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            EFContext.RegisterModified(entity, properties);
            return isSave && await EFContext.CommitAsync() > 0;
        }

        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entities"> 实体对象 </param>
        /// <param name="properties">需部分更新的属性值名称</param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public virtual int Modify(IEnumerable<T> entities, string[] properties = null, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            EFContext.RegisterModified(entities, properties);
            return isSave ? EFContext.Commit() : 0;
        }

        /// <summary>
        ///     更新实体记录
        /// </summary>
        /// <param name="entities"> 实体对象 </param>
        /// <param name="properties">需部分更新的属性值名称</param>
        /// <param name="isSave"> 是否执行保存 </param>
        /// <returns> 操作影响的行数 </returns>
        public async virtual Task<int> ModifyAsync(IEnumerable<T> entities, string[] properties = null, bool isSave = true)
        {
            //PublicHelper.CheckArgument(entity, "entity");
            EFContext.RegisterModified(entities, properties);
            return isSave ? await EFContext.CommitAsync() : 0;
        }

        /// <summary>
        ///     批量插入实体记录集合（海量数据插入）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        public virtual void BulkUpdate(IEnumerable<T> entities)
        {
            EFContext.Context.BulkUpdate<T>(entities);
        }

        /// <summary>
        ///     批量插入实体记录集合（海量数据插入）
        /// </summary>
        /// <param name="entities"> 实体记录集合 </param>
        public async virtual Task BulkUpdateAsync(IEnumerable<T> entities)
        {
            await EFContext.Context.BulkUpdateAsync<T>(entities);
        }
        #endregion 更新实体对象

        #region 获取总记录数
        /// <summary>
        /// 获取总记录数
        /// </summary>
        /// <returns></returns>
        public virtual int GetRecordCount()
        {
            return EFContext.Set<T>().AsNoTracking().Count();
        }
        public async virtual Task<int> GetRecordCountAsync()
        {
            return await EFContext.Set<T>().AsNoTracking().CountAsync();
        }

        /// <summary>
        /// 获取总记录数
        /// </summary>
        /// <returns></returns>
        public virtual int GetRecordCount(Expression<Func<T, bool>> predicate)
        {
            return EFContext.Set<T>().AsNoTracking().Count(predicate);
        }
        public async virtual Task<int> GetRecordCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await EFContext.Set<T>().AsNoTracking().CountAsync(predicate);
        }
        #endregion

        #region 执行数据库脚本

        public bool ExecuteSql(string query, params object[] parameters)
        {
            using (var transaction = EFContext.Context.Database.BeginTransaction())
            {
                try
                {
                    EFContext.Context.Database.ExecuteSqlRaw(query, parameters);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Framework.Util.LogUtil.LogFatal(string.Format("-------ExecuteSql is failed. \r\nMessage: {0}. \r\nStackTrace: {1}",
                        ex.InnerException != null ? ex.InnerException.Message : ex.Message,
                        ex.InnerException != null ? ex.InnerException.StackTrace : ex.StackTrace));
                    return false;
                }
            }
        }
        public async Task<bool> ExecuteSqlAsync(string query, params object[] parameters)
        {
            using (var transaction = EFContext.Context.Database.BeginTransaction())
            {
                try
                {
                    await EFContext.Context.Database.ExecuteSqlRawAsync(query, parameters);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Framework.Util.LogUtil.LogFatal(string.Format("-------ExecuteSql is failed. \r\nMessage: {0}. \r\nStackTrace: {1}",
                        ex.InnerException != null ? ex.InnerException.Message : ex.Message,
                        ex.InnerException != null ? ex.InnerException.StackTrace : ex.StackTrace));
                    return false;
                }
            }
        }
        #endregion
        #endregion 公共方法
    }
}