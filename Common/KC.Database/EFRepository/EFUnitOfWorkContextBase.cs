using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using KC.Database.Extension;
using KC.Database.IRepository;
using KC.Framework.Exceptions;
using KC.Framework.Base;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace KC.Database.EFRepository
{
    /// <summary>  
    ///     单元操作实现  
    /// </summary>  
    public abstract class EFUnitOfWorkContextBase : IUnitOfWorkContext<DbContext>
    {
        /// <summary>  
        /// 获取 当前使用的数据访问上下文对象  
        /// </summary>  
        public abstract DbContext Context { get; }

        /// <summary>  
        ///     获取 当前单元操作是否已被提交  
        /// </summary>  
        public bool IsCommitted { get; private set; }

        /// <summary>  
        ///     提交当前单元操作的结果  
        /// </summary>  
        /// <returns></returns>  
        public int Commit()
        {
            if (IsCommitted)
            {
                return 0;
            }
            try
            {
                int result = Context.SaveChanges();
                IsCommitted = true;
                return result;
            }
            //catch (DbEntityValidationException validEx)
            //{
            //    var sberrors = new StringBuilder();
            //    foreach (var ve in validEx.EntityValidationErrors)
            //    {
            //        sberrors.Append(string.Format("提交数据实体{0}检验错误：", ve.Entry.Entity));
            //        foreach (var e in ve.ValidationErrors)
            //        {
            //            sberrors.Append(string.Format("数据校验错误Property：{0}，{1}。", e.PropertyName, e.ErrorMessage));
            //        }
            //    }

            //    throw PublicHelper.ThrowDataAccessException("提交数据验证时发生异常：" + sberrors, validEx);
            //}
            catch (DbUpdateConcurrencyException deConex)
            {
                // Update original values from the database
                //http://blogs.msdn.com/b/adonet/archive/2011/02/03/using-dbcontext-in-ef-feature-ctp5-part-9-optimistic-concurrency-patterns.aspx
                var entry = deConex.Entries.Single();
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                throw PublicHelper.ThrowDataAccessException("提交数据并发时发生异常：" + deConex.Message, deConex);
            }
            catch (DbUpdateException dbupEx)
            {
                if (dbupEx.InnerException != null && dbupEx.InnerException is SqlException)
                {
                    var sberrors = new StringBuilder();
                    var sqlEx = dbupEx.InnerException as SqlException;
                    foreach (SqlError err in sqlEx.Errors)
                    {
                        string msg = !string.IsNullOrEmpty(DataHelper.GetSqlExceptionMessage(sqlEx.Number))
                            ? DataHelper.GetSqlExceptionMessage(sqlEx.Number)
                            : err.Message;
                        sberrors.Append("SQL Error: " + err.Number + ", Message: " + msg + Environment.NewLine);
                    }

                    throw PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + sberrors, sqlEx);
                }

                throw PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + dbupEx.Message, dbupEx);
            }
            catch (SqlException sqlex)
            {
                var sberrors = new StringBuilder();
                foreach (SqlError err in sqlex.Errors)
                {
                    string msg = !string.IsNullOrEmpty(DataHelper.GetSqlExceptionMessage(sqlex.Number))
                        ? DataHelper.GetSqlExceptionMessage(sqlex.Number)
                        : err.Message;
                    sberrors.Append("SQL Error: " + err.Number + ", Message: " + msg + Environment.NewLine);
                }

                throw PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + sberrors.ToString(), sqlex);
            }
        }

        public async Task<int> CommitAsync()
        {
            if (IsCommitted)
            {
                return 0;
            }
            try
            {
                int result = await Context.SaveChangesAsync();
                IsCommitted = true;
                return result;
            }
            //catch (DbEntityValidationException validEx)
            //{
            //    var sberrors = new StringBuilder();
            //    foreach (var ve in validEx.EntityValidationErrors)
            //    {
            //        sberrors.Append(string.Format("提交数据实体{0}检验错误：", ve.Entry.Entity));
            //        foreach (var e in ve.ValidationErrors)
            //        {
            //            sberrors.Append(string.Format("数据校验错误Property：{0}，{1}。", e.PropertyName, e.ErrorMessage));
            //        }
            //    }

            //    throw PublicHelper.ThrowDataAccessException("提交数据验证时发生异常：" + sberrors, validEx);
            //}
            catch (DbUpdateConcurrencyException deConex)
            {
                // Update original values from the database
                //http://blogs.msdn.com/b/adonet/archive/2011/02/03/using-dbcontext-in-ef-feature-ctp5-part-9-optimistic-concurrency-patterns.aspx
                var entry = deConex.Entries.Single();
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                throw PublicHelper.ThrowDataAccessException("提交数据并发时发生异常：" + deConex.Message, deConex);
            }
            catch (DbUpdateException dbupEx)
            {
                if (dbupEx.InnerException != null && dbupEx.InnerException is SqlException)
                {
                    var sberrors = new StringBuilder();
                    var sqlEx = dbupEx.InnerException as SqlException;
                    foreach (SqlError err in sqlEx.Errors)
                    {
                        string msg = !string.IsNullOrEmpty(DataHelper.GetSqlExceptionMessage(sqlEx.Number))
                            ? DataHelper.GetSqlExceptionMessage(sqlEx.Number)
                            : err.Message;
                        sberrors.Append("SQL Error: " + err.Number + ", Message: " + msg + Environment.NewLine);
                    }

                    throw PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + sberrors, sqlEx);
                }

                throw PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + dbupEx.Message, dbupEx);
            }
            catch (SqlException sqlex)
            {
                var sberrors = new StringBuilder();
                foreach (SqlError err in sqlex.Errors)
                {
                    string msg = !string.IsNullOrEmpty(DataHelper.GetSqlExceptionMessage(sqlex.Number))
                        ? DataHelper.GetSqlExceptionMessage(sqlex.Number)
                        : err.Message;
                    sberrors.Append("SQL Error: " + err.Number + ", Message: " + msg + Environment.NewLine);
                }

                throw PublicHelper.ThrowDataAccessException("提交数据更新时发生异常：" + sberrors.ToString(), sqlex);
            }
        }

        /// <summary>  
        ///     把当前单元操作回滚成未提交状态  
        /// </summary>  
        public void Rollback()
        {
            IsCommitted = false;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                //if (!IsCommitted)
                //{
                //    Commit();
                //}
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>  
        ///   为指定的类型返回 System.Data.Entity.DbSet，这将允许对上下文中的给定实体执行 CRUD 操作。  
        /// </summary>  
        /// <typeparam name="TEntity"> 应为其返回一个集的实体类型。 </typeparam>  
        /// <returns> 给定实体类型的 System.Data.Entity.DbSet 实例。 </returns>  
        public DbSet<TEntity> Set<TEntity>() where TEntity : EntityBase
        {
            return Context.Set<TEntity>();
        }

        /// <summary>  
        ///     注册一个新的对象到仓储上下文中  
        /// </summary>  
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>  
        /// <param name="entity"> 要注册的对象 </param>  
        public void RegisterNew<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            //Context.Detach(entity);

            EntityState state = Context.Entry(entity).State;
            if (state == EntityState.Detached)
            {
                Context.Entry(entity).State = EntityState.Added;
            }
            IsCommitted = false;
        }

        /// <summary>  
        ///     批量注册多个新的对象到仓储上下文中  
        /// </summary>  
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>  
        /// <param name="entities"> 要注册的对象集合 </param>  
        public void RegisterNew<TEntity>(IEnumerable<TEntity> entities) where TEntity : EntityBase
        {
            try
            {
                Context.ChangeTracker.AutoDetectChangesEnabled = false;
                foreach (TEntity entity in entities)
                {
                    RegisterNew(entity);
                }
            }
            finally
            {
                Context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        /// <summary>  
        ///     注册一个更改的对象到仓储上下文中  
        /// </summary>  
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>  
        /// <param name="properties">需部分更新的属性值名称</param>
        /// <param name="entity"> 要注册的对象 </param>  
        public void RegisterModified<TEntity>(TEntity entity, string[] properties = null) where TEntity : EntityBase
        {
            Context.Detach<TEntity>(entity);

            if (Context.Entry(entity).State == EntityState.Detached)
            {
                Context.Set<TEntity>().Attach(entity);
            }

            if (properties != null)
            {
                Context.Entry(entity).State = EntityState.Unchanged;
                foreach (var property in properties)
                {
                    Context.Entry(entity).Property(property).IsModified = true;
                }
            }
            else
            {
                Context.Entry(entity).State = EntityState.Modified;
            }

            if (entity is Entity)
            {
                Context.Entry(entity).Property("CreatedBy").IsModified = false;
                Context.Entry(entity).Property("CreatedDate").IsModified = false;
                Context.Entry(entity).Property("CreatedName").IsModified = false;
            }

            IsCommitted = false;
        }

        /// <summary>  
        ///     注册多个更改的对象到仓储上下文中  
        /// </summary>  
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>  
        /// <param name="entities"> 要注册的对象集合 </param>  
        /// <param name="properties">需部分更新的属性值名称</param>
        public void RegisterModified<TEntity>(IEnumerable<TEntity> entities, string[] properties = null) where TEntity : EntityBase
        {
            try
            {
                Context.ChangeTracker.AutoDetectChangesEnabled = false;
                foreach (TEntity entity in entities)
                {
                    RegisterModified(entity, properties);
                }
            }
            finally
            {
                Context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        /// <summary>  
        ///   注册一个删除的对象到仓储上下文中  
        /// </summary>  
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>  
        /// <param name="entity"> 要注册的对象 </param>  
        public void RegisterDeleted<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            Context.Entry(entity).State = EntityState.Deleted;
            IsCommitted = false;
        }

        /// <summary>  
        ///   批量注册多个删除的对象到仓储上下文中  
        /// </summary>  
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>  
        /// <param name="entities"> 要注册的对象集合 </param>  
        public void RegisterDeleted<TEntity>(IEnumerable<TEntity> entities) where TEntity : EntityBase
        {
            try
            {
                Context.ChangeTracker.AutoDetectChangesEnabled = false;
                foreach (TEntity entity in entities)
                {
                    RegisterDeleted(entity);
                }
            }
            finally
            {
                Context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        /// <summary>  
        ///   注册一个删除的对象到仓储上下文中  
        /// </summary>  
        /// <typeparam name="TEntity"> 要注册的类型 </typeparam>  
        /// <param name="entity"> 要注册的对象 </param>  
        public void RegisterSoftDeleted<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            Context.Detach(entity);

            if (Context.Entry(entity).State == EntityState.Detached)
            {
                Context.Set<TEntity>().Attach(entity);
            }

            //Context.Entry(entity).State = EntityState.Modified;
            if (entity is Entity)
            {
                Context.Entry(entity).Property("IsDeleted").IsModified = true;
            }

            //if (entity is ErpEntityBase)
            //{
            //    Context.Entry(entity).Property("Stat").IsModified = true;
            //}

            IsCommitted = false;
        }
    }  
}
