using KC.Framework.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KC.Database.Util
{
    /// <summary>
    /// Sql Server的分布式锁，使用：dbo.sp_getapplock、dbo.sp_releaseapplock
    /// </summary>
    public class SqlServerDistributedLock : DistributedLockAbstract
    {
        /// <summary>
        ///  SqlServer的分布式锁
        /// </summary>
        /// <param name="key">锁的key</param>
        /// <param name="acquireTimeout">获取Key的超时时间</param>
        /// <param name="lockTimeOut">锁的过期时间</param>
        /// <param name="action"></param>
        public override void DoDistributedLock(string key, TimeSpan acquireTimeout, TimeSpan lockTimeOut, Action action)
        {
            key = DistributedLockPro + key;
            var value = Guid.NewGuid().ToString();
            var connectionString = KC.Framework.Base.GlobalConfig.GetDecryptDatabaseConnectionString();
            var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            try
            {
                connection.Open();
                var end = DateTime.UtcNow.Ticks + acquireTimeout.Ticks;
                while (DateTime.UtcNow.Ticks < end)
                {
                    var result = ExecuteAcquireCommand(connection, key, (int)acquireTimeout.TotalMilliseconds, Mode.Exclusive);
                    if (result)
                    {
                        try
                        {
                            action();
                        }
                        finally
                        {
                            ExecuteReleaseCommand(connection, key);
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
        ///  SqlServer的分布式锁
        /// </summary>
        /// <param name="key">锁的key</param>
        /// <param name="acquireTimeout">获取Key的超时时间</param>
        /// <param name="lockTimeOut">锁的过期时间</param>
        /// <param name="action"></param>
        public override async Task DoDistributedLockAsync(string key, TimeSpan acquireTimeout, TimeSpan lockTimeOut, Action action)
        {
            key = DistributedLockPro + key;
            var value = Guid.NewGuid().ToString();
            var connectionString = KC.Framework.Base.GlobalConfig.GetDecryptDatabaseConnectionString();
            var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            try
            {
                connection.Open();
                var end = DateTime.UtcNow.Ticks + acquireTimeout.Ticks;
                while (DateTime.UtcNow.Ticks < end)
                {
                    if (await ExecuteAcquireCommandAsync(connection, key, (int)acquireTimeout.TotalMilliseconds, Mode.Exclusive, new CancellationToken()))
                    {
                        try
                        {
                            action();
                        }
                        finally
                        {
                            await ExecuteReleaseCommandAsync(connection, key, new CancellationToken());
                        }

                        return;
                    }

                    try
                    {
                        await Task.Delay(10);
                        //System.Threading.Thread.Sleep(10);
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
        private bool ExecuteAcquireCommand(IDbConnection connectionOrTransaction, string lockName, int timeoutMillis, Mode mode)
        {
            using (var command = CreateAcquireCommand(connectionOrTransaction, lockName, timeoutMillis, mode, out var returnValue))
            {
                command.ExecuteNonQuery();
                return ParseExitCode((int)returnValue.Value);
            }
        }
        private async Task<bool> ExecuteAcquireCommandAsync(IDbConnection connectionOrTransaction, string lockName, int timeoutMillis, Mode mode, CancellationToken cancellationToken)
        {
            using (var command = CreateAcquireCommand(connectionOrTransaction, lockName, timeoutMillis, mode, out var returnValue))
            {
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                return ParseExitCode((int)returnValue.Value);
            }
        }
        private void ExecuteReleaseCommand(IDbConnection connectionOrTransaction, string lockName)
        {
            using (var command = CreateReleaseCommand(connectionOrTransaction, lockName, out var returnValue))
            {
                command.ExecuteNonQuery();
                ParseExitCode((int)returnValue.Value);
            }
        }
        private async Task ExecuteReleaseCommandAsync(IDbConnection connectionOrTransaction, string lockName, CancellationToken cancellationToken)
        {
            using (var command = CreateReleaseCommand(connectionOrTransaction, lockName, out var returnValue))
            {
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false); ;
                ParseExitCode((int)returnValue.Value);
            }
        }

        private IDbCommand CreateAcquireCommand(IDbConnection connection, string lockName, int timeoutMillis, Mode mode, out IDbDataParameter returnValue)
        {
            var command = connection.CreateCommand();
            //command.Transaction = connection.Transaction;
            command.CommandText = "dbo.sp_getapplock";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = timeoutMillis >= 0
                  // command timeout is in seconds. We always wait at least the lock timeout plus a buffer 
                  ? (timeoutMillis / 1000) + 30
                  // otherwise timeout is infinite so we use the infinite timeout of 0
                  // (see https://msdn.microsoft.com/en-us/library/Microsoft.Data.SqlClient.sqlcommand.commandtimeout%28v=vs.110%29.aspx)
                  : 0;

            command.Parameters.Add(command.CreateParameter("Resource", lockName));
            command.Parameters.Add(command.CreateParameter("LockMode", GetModeString(mode)));
            command.Parameters.Add(command.CreateParameter("LockOwner", command.Transaction != null ? "Transaction" : "Session"));
            command.Parameters.Add(command.CreateParameter("LockTimeout", timeoutMillis));

            returnValue = command.CreateParameter();
            returnValue.Direction = ParameterDirection.ReturnValue;
            command.Parameters.Add(returnValue);

            return command;
        }
        private IDbCommand CreateReleaseCommand(IDbConnection connection, string lockName, out IDbDataParameter returnValue)
        {
            var command = connection.CreateCommand();
            //command.Transaction = connection.Transaction;
            command.CommandText = "dbo.sp_releaseapplock";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(command.CreateParameter("Resource", lockName));
            command.Parameters.Add(command.CreateParameter("LockOwner", command.Transaction != null ? "Transaction" : "Session"));

            returnValue = command.CreateParameter();
            returnValue.Direction = ParameterDirection.ReturnValue;
            command.Parameters.Add(returnValue);

            return command;
        }

        private bool ParseExitCode(int exitCode)
        {
            // sp_getapplock exit codes documented at
            // https://msdn.microsoft.com/en-us/library/ms189823.aspx

            switch (exitCode)
            {
                case 0:
                case 1:
                    return true;

                case -1:
                    return false;

                case -2: // canceled
                    throw new OperationCanceledException(GetErrorMessage(exitCode, "canceled"));
                case -3: // deadlock
                    throw new Exception(GetErrorMessage(exitCode, "deadlock"));
                case -999: // parameter / unknown
                    throw new ArgumentException(GetErrorMessage(exitCode, "parameter validation or other error"));

                default:
                    if (exitCode <= 0)
                        throw new InvalidOperationException(GetErrorMessage(exitCode, "unknown"));
                    return true; // unknown success code
            }
        }
        private string GetErrorMessage(int exitCode, string type) => $"The request for the distribute lock failed with exit code {exitCode} ({type})";
        private string GetModeString(Mode mode)
        {
            switch (mode)
            {
                case Mode.Shared: return "Shared";
                case Mode.Update: return "Update";
                case Mode.Exclusive: return "Exclusive";
                default: throw new ArgumentException(nameof(mode));
            }
        }

        private enum Mode
        {
            Shared,
            Update,
            Exclusive,
        }
        #endregion
    }
}
