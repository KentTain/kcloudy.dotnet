using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using KC.Framework;
using KC.Framework.Util;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Component.Base;
using Azure.Data.Tables;
using Azure;
using KC.Framework.Base;

namespace KC.Component.NoSqlRepository
{
    //TODO: 增加MongDB、DocumentDB实现
    public abstract class AzureTableRepositoryBase<T> : IRepository<T>, IDisposable where T : AzureTableEntity, ITableEntity, new()
    {
        private TableServiceClient _serviceClient;

        private string _tableName;
        public Tenant Tenant { get; private set; }

        protected AzureTableRepositoryBase(string connectString, string tableName = null)
            : this(connectString, null, tableName)
        {

        }

        protected AzureTableRepositoryBase(Tenant tenant, string tableName = null)
            : this(tenant.GetDecryptNoSqlConnectionString(), tenant.TenantName, tableName)
        {
            Tenant = tenant;
        }

        private AzureTableRepositoryBase(string connectString, string tenantName, string tableName = null)
        {
            //_account = account;

            if (!string.IsNullOrWhiteSpace(tenantName)
                && !string.IsNullOrWhiteSpace(tableName))
            {
                _tableName = (tenantName + tableName).ToLower();
            }
            else if (!string.IsNullOrWhiteSpace(tenantName)
                      && string.IsNullOrWhiteSpace(tableName))
            {
                _tableName = (tenantName + typeof(T).Name).ToLower();
            }
            else if (string.IsNullOrWhiteSpace(tenantName)
                     && !string.IsNullOrWhiteSpace(tableName))
            {
                _tableName = tableName;
            }
            else
            {
                _tableName = ("com" + typeof(T).Name).ToLower();
            }

            //Table name rule: http://msdn.microsoft.com/en-us/library/azure/dd179338.aspx
            var regex = new Regex(@"^[A-Za-z][A-Za-z0-9]{2,62}$");
            if(!regex.IsMatch(_tableName))
                throw new ArgumentException(string.Format("the tableName {0} is not match the Azure name rule.", _tableName), "tenantName");

            //_tableClient = new CloudTableClient(_account.TableEndpoint.AbsoluteUri, _account.Credentials);
            _serviceClient = new TableServiceClient(connectString);
            _serviceClient.CreateTableIfNotExists(_tableName);
        }

        public virtual T FindByRowKey(string rowKey)
        {
            var _tableClient = _serviceClient.GetTableClient(_tableName);
            // Execute the retrieve operation.
            var retrievedResult = _tableClient.GetEntity<T>(_tableName, rowKey);
            return retrievedResult;
        }

        public virtual IList<T> FindAll(Expression<Func<T, bool>> predicate)
        {
            var _tableClient = _serviceClient.GetTableClient(_tableName);
            var iterator =  _tableClient.Query<T>(predicate).GetEnumerator();
            var result = new List<T>();
            while (true)
            {
                var hasValue = iterator.MoveNext();
                if (!hasValue)
                {
                    break;
                }
                result.Add(iterator.Current);
            }
            return result;
        }
        public virtual IList<T> FindAll<K>(Expression<Func<T, bool>> predicate, Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? FindAll().OrderBy(keySelector.Compile()).Where(predicate.Compile()).ToList()
                : FindAll().OrderByDescending(keySelector.Compile()).Where(predicate.Compile()).ToList();
        }

        public virtual IList<T> FindAll()
        {
            return FindAll(m => true); 
        }
        public virtual IList<T> FindAll<K>(Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            return @ascending
                ? FindAll().OrderBy(keySelector.Compile()).ToList()
                : FindAll().OrderByDescending(keySelector.Compile()).ToList();
        }

        public Tuple<int, IList<T>> FindPagenatedListWithCount(int pageIndex, int pageSize,
            Expression<Func<T, bool>> predicate, string sortProperty, bool ascending = true)
        {
            var all = FindAll();
            //var q = CreateQuery<T>(_tableName).Where(predicate);
            int recordCount = all.Where(predicate.Compile()).Count();
            var q = all.SingleOrderBy(sortProperty, ascending)
                .Where(predicate.Compile())
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new Tuple<int, IList<T>>(recordCount, q);
        }
        public Tuple<int, IList<T>> FindPagenatedListWithCount<K>(int pageIndex, int pageSize,
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, K>> keySelector, bool ascending = true)
        {
            var databaseItems = FindAll();
            int recordCount = databaseItems.Count();
            return @ascending 
                ? new Tuple<int, IList<T>>(recordCount, databaseItems.OrderBy(keySelector.Compile()).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()) 
                : new Tuple<int, IList<T>>(recordCount, databaseItems.OrderByDescending(keySelector.Compile()).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }

        public virtual bool Add(T entity, bool isSave = true)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Table:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        _tableName, MethodBase.GetCurrentMethod()));

            entity.PartitionKey = _tableName;
            entity.CreatedDate = DateTime.UtcNow;
            entity.ModifiedDate = DateTime.UtcNow;

            var _tableClient = _serviceClient.GetTableClient(_tableName);
            // Execute the insert operation.
            var result = _tableClient.AddEntity(entity);
            return result.Status == 200;
        }

        public virtual bool Modify(T entity, bool isSave = true)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Table:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        _tableName, MethodBase.GetCurrentMethod()));

            entity.ModifiedDate = DateTime.UtcNow;

            var _tableClient = _serviceClient.GetTableClient(_tableName);
            T retrievedResult = _tableClient.GetEntity<T>(entity.PartitionKey, entity.RowKey);
            if (retrievedResult != null)
            {
                var result = _tableClient.UpdateEntity<T>(entity, retrievedResult.ETag);
                return result.Status == 200;
            }
            return true;
        }

        public virtual bool RemoveByRowKey(string rowKey, bool isSave = true)
        {
            var _tableClient = _serviceClient.GetTableClient(_tableName);

            var iCount = 0;
            Pageable<T> queryResultsFilter = _tableClient.Query<T>(filter: $"RowKey eq '{rowKey}'");
            foreach (T qEntity in queryResultsFilter)
            {
                var result = _tableClient.DeleteEntity(qEntity.PartitionKey, qEntity.RowKey);
                if (result.Status == 200)
                    iCount++;
            }
            return iCount > 0;
        }
        public virtual bool Remove(T entity, bool isSave = true)
        {
            if (entity == null)
                throw new ArgumentException(
                    string.Format("The Table:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        _tableName, MethodBase.GetCurrentMethod()));

            var _tableClient = _serviceClient.GetTableClient(_tableName);
            // Execute the operation.
            var result = _tableClient.DeleteEntity(entity.PartitionKey, entity.RowKey);
            return result.Status == 200;
        }
        public virtual int Remove(IEnumerable<T> entities, bool isSave = true)
        {
            if (entities == null)
                throw new ArgumentException(
                    string.Format("The Table:{0} fails to execute Method:{1} with ArgumentException: entity is null",
                        _tableName, MethodBase.GetCurrentMethod()));
            
            int i = 0;
            try
            {
                foreach (var entity in entities)
                {
                    Remove(entity);
                    i++;
                }
                return i;
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                return i;
            }
        }
        public virtual bool RemoveAll()
        {
            var _tableClient = _serviceClient.GetTableClient(_tableName);
            _tableClient.Delete();
            return true;
        }

        ~AzureTableRepositoryBase()
        {
            this.Dispose();
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!this._disposed)
            {
                this._disposed = true;
            }
        }
    }

    public abstract class CommonTableServiceRepository<T> : AzureTableRepositoryBase<T> where T : AzureTableEntity, ITableEntity, new()
    {
        private static readonly string NoSqlConnectionString = KC.Framework.Base.GlobalConfig.NoSqlConnectionString;

        protected CommonTableServiceRepository()
            : base(NoSqlConnectionString)
        {

        }

        protected CommonTableServiceRepository(string tableName)
            : base(NoSqlConnectionString, tableName)
        {

        }

        protected CommonTableServiceRepository(Tenant tenant)
            : base(tenant)
        {

        }

        protected CommonTableServiceRepository(Tenant tenant, string tableName)
            : base(tenant, tableName)
        {

        }
    }
}