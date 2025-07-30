using KC.Framework.Tenant;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace KC.Database.Util
{
    public class DatabaseFactory
    {
        public Tenant Tenant { get; private set; }
        public DatabaseFactory(Tenant tenant)
        {
            Tenant = tenant;
        }

        public int ExecuteNonQuery(string connectString, string sql, IEnumerable<DbParameter> parameters)
        {
            switch (Tenant.DatabaseType)
            {
                case DatabaseType.MySql:
                    var mySqlParms = parameters != null && parameters.Any(m => m is MySqlConnector.MySqlParameter)
                        ? parameters.Select(m => m as MySqlConnector.MySqlParameter)
                        : null;
                    return MySqlDatabaseHelper.ExecuteNonQuery(connectString, sql, mySqlParms);
                default:
                    var sqlParms = parameters != null && parameters.Any(m => m is SqlParameter)
                        ? parameters.Select(m => m as SqlParameter)
                        : null;
                    return SqlDatabaseHelper.ExecuteNonQuery(connectString, sql, sqlParms);
            }
        }

        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, null);
        }
        public int ExecuteNonQuery(string sql, IEnumerable<DbParameter> parameters)
        {
            switch (Tenant.DatabaseType)
            {
                case DatabaseType.MySql:
                    var mySqlParms = parameters != null && parameters.Any(m => m is MySqlConnector.MySqlParameter)
                        ? parameters.Select(m => m as MySqlConnector.MySqlParameter)
                        : null;
                    return MySqlDatabaseHelper.ExecuteNonQuery(Tenant.ConnectionString, sql, mySqlParms);
                default:
                    var sqlParms = parameters != null && parameters.Any(m => m is SqlParameter)
                        ? parameters.Select(m => m as SqlParameter)
                        : null;
                    return SqlDatabaseHelper.ExecuteNonQuery(Tenant.ConnectionString, sql, sqlParms);
            }
        }

        public int TranExecuteNonQuery(string sql)
        {
            return TranExecuteNonQuery(sql, null);
        }
        public int TranExecuteNonQuery(string sql, IEnumerable<DbParameter> parameters)
        {
            switch (Tenant.DatabaseType)
            {
                case DatabaseType.MySql:
                    var mySqlParms = parameters != null && parameters.Any(m => m is MySqlConnector.MySqlParameter)
                        ? parameters.Select(m => m as MySqlConnector.MySqlParameter)
                        : null;
                    return MySqlDatabaseHelper.TranExecuteNonQuery(Tenant.ConnectionString, sql, mySqlParms);
                default:
                    var sqlParms = parameters != null && parameters.Any(m => m is SqlParameter)
                        ? parameters.Select(m => m as SqlParameter)
                        : null;
                    return SqlDatabaseHelper.TranExecuteNonQuery(Tenant.ConnectionString, sql, sqlParms);
            }
        }

        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null);
        }
        public object ExecuteScalar(string sql, IEnumerable<DbParameter> parameters)
        {
            switch (Tenant.DatabaseType)
            {
                case DatabaseType.MySql:
                    var mySqlParms = parameters != null && parameters.Any(m => m is MySqlConnector.MySqlParameter)
                        ? parameters.Select(m => m as MySqlConnector.MySqlParameter)
                        : null;
                    return MySqlDatabaseHelper.ExecuteScalar(Tenant.ConnectionString, sql, mySqlParms);
                default:
                    var sqlParms = parameters != null && parameters.Any(m => m is SqlParameter)
                        ? parameters.Select(m => m as SqlParameter)
                        : null;
                    return SqlDatabaseHelper.ExecuteScalar(Tenant.ConnectionString, sql, sqlParms);
            }
        }

        public DbDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, null);
        }
        public DbDataReader ExecuteReader(string sql, IEnumerable<DbParameter> parameters)
        {
            switch (Tenant.DatabaseType)
            {
                case DatabaseType.MySql:
                    var mySqlParms = parameters != null && parameters.Any(m => m is MySqlConnector.MySqlParameter)
                        ? parameters.Select(m => m as MySqlConnector.MySqlParameter) 
                        : null;
                    return MySqlDatabaseHelper.ExecuteReader(Tenant.ConnectionString, sql, mySqlParms);
                default:
                    var sqlParms = parameters != null && parameters.Any(m => m is SqlParameter)
                        ? parameters.Select(m => m as SqlParameter)
                        : null;
                    return SqlDatabaseHelper.ExecuteReader(Tenant.ConnectionString, sql, sqlParms);
            }
        }

        public DataSet ExecuteDataSet(string tableName, string sql)
        {
            return ExecuteDataSet(tableName, sql, null);
        }
        public DataSet ExecuteDataSet(string tableName, string sql, IEnumerable<DbParameter> parameters)
        {
            switch (Tenant.DatabaseType)
            {
                case DatabaseType.MySql:
                    var mySqlParms = parameters != null && parameters.Any(m => m is MySqlConnector.MySqlParameter)
                        ? parameters.Select(m => m as MySqlConnector.MySqlParameter)
                        : null;
                    return MySqlDatabaseHelper.ExecuteDataSet(Tenant.ConnectionString, tableName, sql, mySqlParms);
                default:
                    var sqlParms = parameters != null && parameters.Any(m => m is SqlParameter)
                        ? parameters.Select(m => m as SqlParameter)
                        : null;
                    return SqlDatabaseHelper.ExecuteDataSet(Tenant.ConnectionString, tableName, sql, sqlParms);
            }
        }
    }
}
