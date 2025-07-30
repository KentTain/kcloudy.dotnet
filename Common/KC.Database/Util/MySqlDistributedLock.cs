using KC.Framework.Tenant;
using KC.Framework.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KC.Database.Util
{
    /// <summary>
    /// MySql的分布式锁，使用数据库的行级锁--表：dbo.sys_DatabaseDistributedLock
    /// </summary>
    public class MySqlDistributedLock : DistributedLockAbstract
    {
        public string DefaultDbo = "dbo";
        public string LockTableName = "sys_DatabaseDistributedLock";

        public string connectionString = KC.Framework.Base.GlobalConfig.GetDecryptMySqlConnectionString();
        public static DatabaseType dbType = DatabaseType.MySql;

        public MySqlDistributedLock()
        {
            using (var connection = GetDbConnection())
            {
                connection.Open();
                //创建锁表及插入行
                var createTableSql = GetCreateLockTableSql();
                ExecuteCommand(connection, createTableSql);
            }
        }
        
        /// <summary>
        ///  MySql的分布式锁
        /// </summary>
        /// <param name="key">锁的key</param>
        /// <param name="acquireTimeout">获取Key的超时时间</param>
        /// <param name="lockTimeOut">锁的过期时间</param>
        /// <param name="action"></param>
        public override void DoDistributedLock(string key, TimeSpan acquireTimeout, TimeSpan lockTimeOut, Action action)
        {
            key = DistributedLockPro + key;
            var value = Guid.NewGuid().ToString();
            var connection = GetDbConnection();
            try
            {
                connection.Open();
                //创建锁表及插入行
                var insertDataSql = InsertLockKey(key, value);
                ExecuteCommand(connection, insertDataSql);

                var transaction = connection.BeginTransaction();
                var command = GetAcquireCommand(connection, transaction, key, (int)acquireTimeout.TotalMilliseconds);

                var end = DateTime.UtcNow.Ticks + acquireTimeout.Ticks;
                while (DateTime.UtcNow.Ticks < end)
                {
                    var result = command.ExecuteScalar();
                    if (result != null && !string.IsNullOrEmpty(result.ToString()))
                    {
                        try
                        {
                            action();
                        }
                        finally
                        {
                            transaction.Commit();
                        }

                        return;
                    }

                    try
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    catch (Exception e)
                    {
                        LogUtil.LogException(e);
                        System.Threading.Thread.CurrentThread.Interrupt();
                    }
                }
            }
            finally
            {
                connection.Dispose();
            }
        }
        
        /// <summary>
        ///  MySql的分布式锁
        /// </summary>
        /// <param name="key">锁的key</param>
        /// <param name="acquireTimeout">获取Key的超时时间</param>
        /// <param name="lockTimeOut">锁的过期时间</param>
        /// <param name="action"></param>
        public override async Task DoDistributedLockAsync(string key, TimeSpan acquireTimeout, TimeSpan lockTimeOut, Action action)
        {
            key = DistributedLockPro + key;
            var value = Guid.NewGuid().ToString();
            var connection = GetDbConnection();
            try
            {
                connection.Open();
                //创建锁表及插入行
                var insertDataSql = InsertLockKey(key, value);
                ExecuteCommand(connection, insertDataSql);

                var transaction = connection.BeginTransaction();
                var command = GetAcquireCommand(connection, transaction, key, (int)acquireTimeout.TotalMilliseconds);

                var end = DateTime.UtcNow.Ticks + acquireTimeout.Ticks;
                while (DateTime.UtcNow.Ticks < end)
                {
                    var result = await command.ExecuteScalarAsync(new CancellationToken());
                    if (result != null && !string.IsNullOrEmpty(result.ToString()))
                    {
                        try
                        {
                            action();
                        }
                        finally
                        {
                            transaction.Commit();
                        }

                        return;
                    }

                    try
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    catch (Exception e)
                    {
                        LogUtil.LogException(e);
                        System.Threading.Thread.CurrentThread.Interrupt();
                    }
                }
            }
            finally
            {
                connection.Dispose();
            }
        }


        #region 私有方法：数据库访问
        private string GetCreateLockTableSql()
        {
            var sql = string.Empty;
            switch (dbType)
            {
                case DatabaseType.MySql:
                    sql = string.Format(@"
                        CREATE TABLE IF NOT EXISTS `{0}.{1}` (
                          `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键',
                          `LockKey` varchar(64) NOT NULL DEFAULT '' COMMENT '锁定的方法名',
                          `Desc` varchar(1024) NOT NULL DEFAULT '备注信息',
                          `UpdateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '保存数据时自动生成',
                          PRIMARY KEY (`id`),
                          UNIQUE KEY `IX_{1}_LockKey` (`LockKey`) USING BTREE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE utf8_general_ci COMMENT='锁定中的方法';",
                        DefaultDbo, LockTableName);
                    break;
                default:
                    sql = string.Format(@"
                    IF (NOT EXISTS (SELECT * 
                     FROM INFORMATION_SCHEMA.TABLES 
                     WHERE TABLE_SCHEMA = '{0}' 
                     AND  TABLE_NAME = '{1}'))
                        BEGIN
                             CREATE TABLE [{0}].[{1}](
	                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                            [LockKey] [nvarchar](64) NULL,
	                            [Desc] [nvarchar](1024) NULL,
	                            [UpdateTime] [datetime] NULL,
                             CONSTRAINT [IX_{1}_LockKey] UNIQUE ([LockKey]),
                             CONSTRAINT [PK_{1}] PRIMARY KEY CLUSTERED,
                            (
	                            [Id] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY]
                        END",
                    DefaultDbo, LockTableName);
                    break;
            }

            return sql;
        }
        private string InsertLockKey(string locakKey, string value)
        {
            var sql = string.Empty;
            switch (dbType)
            {
                case DatabaseType.MySql:
                    sql = string.Format(
                        @"INSERT INTO `{0}.{1}` (`LockKey`,`Desc`,`UpdateTime`)
                            VALUES ('{2}','{3}', UTC_TIMESTAMP())
                            ON DUPLICATE KEY UPDATE  LockKey=VALUES(LockKey)", 
                        DefaultDbo, LockTableName, locakKey, value );
                    break;
                default:
                    sql = string.Format(
                        @"IF NOT EXISTS (SELECT * FROM [{0}].[{1}] WHERE [LockKey] = '{2}')
                          BEGIN
                            INSERT INTO [{0}].[{1}] ([LockKey],[Desc],[UpdateTime])
                            VALUES ('{2}','{3}', getutcdate())
                          END", 
                        DefaultDbo, LockTableName, locakKey, value);
                    break;
            }

            return sql;
        }
        private string GetLockRowSql(string locakKey)
        {
            var sql = string.Empty;
            switch (dbType)
            {
                case DatabaseType.MySql:
                    sql = string.Format(@"select `Desc` from `{0}.{1}` where `LockKey`='{2}' for update", DefaultDbo, LockTableName, locakKey);
                    break;
                default:
                    sql = string.Format(@"select [Desc] from [{0}].[{1}] WITH (ROWLOCK,XLOCK) where [LockKey]=N'{2}'", DefaultDbo, LockTableName, locakKey);
                    break;
            }

            return sql;
        }

        private IDbConnection GetDbConnection()
        {
            switch (dbType)
            {
                case DatabaseType.MySql:
                    return new MySqlConnector.MySqlConnection(connectionString);
                default:
                    return new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            }
        }
        private IDbCommand GetAcquireCommand(IDbConnection connection, IDbTransaction transaction, string lockName, int timeoutMillis)
        {
            var command = connection.CreateCommand();
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = connection;
            command.Transaction = transaction;

            command.CommandText = GetLockRowSql(lockName);
            command.CommandType = CommandType.Text;
            command.CommandTimeout = timeoutMillis >= 0
                  // command timeout is in seconds. We always wait at least the lock timeout plus a buffer 
                  ? (timeoutMillis / 1000) + 30
                  // otherwise timeout is infinite so we use the infinite timeout of 0
                  // (see https://msdn.microsoft.com/en-us/library/Microsoft.Data.SqlClient.sqlcommand.commandtimeout%28v=vs.110%29.aspx)
                  : 0;

            return command;
        }

        private void ExecuteCommand(IDbConnection connection, string sql)
        {
            using (var command = GetExecuteCommand(connection, sql))
            {
                command.ExecuteNonQuery();
            }
        }
        private IDbCommand GetExecuteCommand(IDbConnection connection, string sql)
        {
            var command = connection.CreateCommand();
            command.Connection = connection;
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            return command;
        }
        #endregion
    }
}
