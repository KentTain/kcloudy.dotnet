using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KC.Framework.Util;

namespace KC.Database.Util
{
    public class SqlDatabaseHelper
    {
        public static void ExecuteBatchTemplate(string connectString, List<string> sqlStatements, Dictionary<string, string> replacements, bool goDelimiter)
        {
            if (goDelimiter)
            {
                StringBuilder sqlBuilder = new StringBuilder();
                foreach (string line in sqlStatements)
                {
                    string sqlLine = line;
                    if (line.Trim().ToUpper() == "GO")
                    {
                        string sql = sqlBuilder.ToString();
                        if (replacements != null)
                        {
                            foreach (var replacement in replacements.Keys)
                            {
                                sql = sql.Replace(replacement, replacements[replacement]);
                            }
                        }
                        ExecuteNonQuery(connectString, sql, null);
                        sqlBuilder.Clear();
                    }
                    else
                    {
                        sqlBuilder.AppendLine(sqlLine);
                    }
                }
            }
            else
            {
                foreach (var line in sqlStatements)
                {
                    var statement = line;
                    ExecuteNonQuery(connectString, statement, null);
                }
            }
        }

        public static int ExecuteNonQuery(string connectString, string sql, IEnumerable<SqlParameter> parameters, bool avoidEventTrigger = false, int retryNumber = 0, bool logStatement = false, string logTitle = null)
        {
            if (retryNumber == 0 || string.IsNullOrEmpty(logTitle))
                logTitle = "ExecuteNonQuery - " + new StackFrame(1).GetMethod().Name;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                int affectRows;
                using (SqlConnection connection = new SqlConnection(connectString))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = PrepareCommand(connection, sql, parameters))
                        {
                            try
                            {
                                affectRows = command.ExecuteNonQuery();
                            }
                            finally
                            {
                                command.Parameters.Clear();
                            }
                        }
                    }
                    finally
                    {
                        connection.Close();
                        stopwatch.Stop();
                    }
                }

                if (logStatement)
                {
                    string logMessage = GetSqlStatementFromSqlCommand(sql, parameters);
                    LogUtil.LogDebug(logTitle, logMessage, stopwatch.ElapsedMilliseconds);
                }

                return affectRows;
            }
            catch (Exception ex)
            {
                string sqlScript = GetSqlStatementFromSqlCommand(sql, parameters);
                string logMessage = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlScript;
                //LogUtil.LogError(logTitle, logMessage, stopwatch.ElapsedMilliseconds);

                if (IsTransient(ex) && retryNumber < 3)
                {
                    Thread.Sleep(100);
                    //LogUtil.LogError(logTitle, "Retry statement: " + (retryNumber + 1));
                    int affectRows = ExecuteNonQuery(connectString, sql, parameters, avoidEventTrigger, retryNumber + 1, logStatement, logTitle);

                    return affectRows;
                }
                else
                {
                    throw new Exception(sql, ex);
                }
            }
        }

        public static int TranExecuteNonQuery(string connectString, string sql, IEnumerable<SqlParameter> parameters, bool avoidEventTrigger = false, int retryNumber = 0, bool logStatement = false, string logTitle = null)
        {
            if (retryNumber == 0 || string.IsNullOrEmpty(logTitle))
                logTitle = "ExecuteNonQuery - " + new StackFrame(1).GetMethod().Name;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                int affectRows;
                using (SqlConnection connection = new SqlConnection(connectString))
                {
                    connection.Open();
                    using (SqlTransaction sqlTransaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand command = PrepareCommand(connection, sql, parameters, sqlTransaction))
                            {
                                try
                                {
                                    affectRows = command.ExecuteNonQuery();
                                    sqlTransaction.Commit();
                                }
                                finally
                                {
                                    command.Parameters.Clear();
                                }
                            }
                        }
                        finally
                        {
                            connection.Close();
                            stopwatch.Stop();
                        }
                    }
                }

                if (logStatement)
                {
                    string logMessage = GetSqlStatementFromSqlCommand(sql, parameters);
                    LogUtil.LogDebug(logTitle, logMessage, stopwatch.ElapsedMilliseconds);
                }

                return affectRows;
            }
            catch (Exception ex)
            {
                string sqlScript = GetSqlStatementFromSqlCommand(sql, parameters);
                string logMessage = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlScript;
                //LogUtil.LogError(logTitle, logMessage, stopwatch.ElapsedMilliseconds);

                if (IsTransient(ex) && retryNumber < 3)
                {
                    Thread.Sleep(100);
                    //LogUtil.LogError(logTitle, "Retry statement: " + (retryNumber + 1));
                    int affectRows = ExecuteNonQuery(connectString, sql, parameters, avoidEventTrigger, retryNumber + 1, logStatement, logTitle);

                    return affectRows;
                }
                else
                {
                    throw new Exception("Error accessing database - " + ex.Message + sql, ex);
                }
            }
        }

        public static object ExecuteScalar(string connectString, string sql, IEnumerable<SqlParameter> parameters, int retryNumber = 0, bool logStatement = false, string logTitle = null)
        {
            if (retryNumber == 0 || string.IsNullOrEmpty(logTitle))
                logTitle = "ExecuteScalar - " + new StackFrame(1).GetMethod().Name;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                object result;
                using (SqlConnection connection = new SqlConnection(connectString))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = PrepareCommand(connection, sql, parameters))
                        {
                            try
                            {
                                result = command.ExecuteScalar();
                            }
                            finally
                            {
                                command.Parameters.Clear();
                            }
                        }
                    }
                    finally
                    {
                        connection.Close();
                        stopwatch.Stop();
                    }
                }

                if (logStatement)
                {
                    string logMessage = GetSqlStatementFromSqlCommand(sql, parameters);
                    LogUtil.LogDebug(logTitle, logMessage, stopwatch.ElapsedMilliseconds);
                }

                return result;
            }
            catch (Exception ex)
            {
                string sqlScript = GetSqlStatementFromSqlCommand(sql, parameters);
                string logMessage = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlScript;
                //LogUtil.LogError(logTitle, logMessage, stopwatch.ElapsedMilliseconds);

                if (IsTransient(ex) && retryNumber < 3)
                {
                    Thread.Sleep(100);
                    //LogUtil.LogError(logTitle, "Retry statement: " + (retryNumber + 1));
                    object result = ExecuteScalar(connectString, sql, parameters, retryNumber + 1, logStatement, logTitle);
                    return result;
                }
                else
                {
                    throw new Exception("Error accessing database - " + ex.Message + sql, ex);
                }
            }
        }

        public static SqlDataReader ExecuteReader(string connectString, string sql, IEnumerable<SqlParameter> parameters, int retryNumber = 0, bool logStatement = false, string logTitle = null)
        {
            if (retryNumber == 0 || string.IsNullOrEmpty(logTitle))
                logTitle = "ExecuteReader - " + new StackFrame(1).GetMethod().Name;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                SqlDataReader reader = null;
                SqlConnection connection = new SqlConnection(connectString);
                connection.Open();
                try
                {
                    using (SqlCommand command = PrepareCommand(connection, sql, parameters))
                    {
                        try
                        {
                            reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                        }
                        finally
                        {
                            command.Parameters.Clear();
                        }
                    }
                }
                finally
                {
                    stopwatch.Stop();
                }

                if (logStatement)
                {
                    string logMessage = GetSqlStatementFromSqlCommand(sql, parameters);
                    LogUtil.LogDebug(logTitle, logMessage, stopwatch.ElapsedMilliseconds);
                }

                return reader;
            }
            catch (Exception ex)
            {
                string sqlScript = GetSqlStatementFromSqlCommand(sql, parameters);
                string logMessage = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlScript;
                //LogUtil.LogError(logTitle, logMessage, stopwatch.ElapsedMilliseconds);

                if (IsTransient(ex) && retryNumber < 3)
                {
                    Thread.Sleep(1);

                    //LogUtil.LogError(logTitle, "Retry statement: " + (retryNumber + 1));
                    return ExecuteReader(connectString, sql, parameters, retryNumber + 1, logStatement, logTitle);
                }
                else
                {
                    throw new Exception("Error accessing database - " + ex.Message + ", Detail:" + ex.StackTrace + sql, ex);
                }
            }
        }

        public static DataSet ExecuteDataSet(string connectString, string tableName, string sql, IEnumerable<SqlParameter> parameters, bool fillSchema = false, int executeTimeout = -1, int retryNumber = 0, bool logStatement = false, string logTitle = null)
        {
            if (retryNumber == 0 || string.IsNullOrEmpty(logTitle))
                logTitle = "ExecuteDataSet - " + new StackFrame(1).GetMethod().Name;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                DataSet dataSet = null;

                using (SqlConnection connection = new SqlConnection(connectString))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = PrepareCommand(connection, sql, parameters))
                        {
                            try
                            {
                                if (executeTimeout > 0)
                                {
                                    command.CommandTimeout = executeTimeout;
                                }

                                dataSet = new DataSet();
                                using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(command))
                                {
                                    if (fillSchema)
                                    {
                                        sqlAdapter.FillSchema(dataSet, SchemaType.Source, tableName);
                                    }
                                    sqlAdapter.Fill(dataSet);
                                }

                            }
                            finally
                            {
                                command.Parameters.Clear();
                            }
                        }
                    }
                    finally
                    {
                        connection.Close();
                        stopwatch.Stop();
                    }
                }

                if (logStatement)
                {
                    string logMessage = GetSqlStatementFromSqlCommand(sql, parameters);
                    LogUtil.LogDebug(logTitle, logMessage, stopwatch.ElapsedMilliseconds);
                }

                return dataSet;
            }
            catch (Exception ex)
            {
                string sqlScript = GetSqlStatementFromSqlCommand(sql, parameters);
                string logMessage = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlScript;
                //LogUtil.LogError(logTitle, logMessage, stopwatch.ElapsedMilliseconds);

                if (IsTransient(ex) && retryNumber < 3)
                {
                    Thread.Sleep(100);

                    //LogUtil.LogError(logTitle, "Retry statement: " + (retryNumber + 1));
                    return ExecuteDataSet(connectString, tableName, sql, parameters, fillSchema, executeTimeout, retryNumber + 1, logStatement, logTitle);
                }

                throw new Exception("Error accessing database - " + ex.Message + sql, ex);
            }
        }

        public static string GetSqlStatementFromSqlCommand(string sql, IEnumerable<SqlParameter> parameters)
        {
            StringBuilder sqlBuidler = new StringBuilder(sql);
            if (parameters != null)
            {
                // Replace from the last param to the first to avoid issue like "@id1, @id2, ..., @id11, @id12..."
                parameters = parameters.OrderByDescending(x => x.ParameterName);
                foreach (SqlParameter parameter in parameters)
                {
                    sqlBuidler = sqlBuidler.Replace(parameter.ParameterName, GetParameterValueForSQL(parameter));
                }
            }
            return sqlBuidler.ToString();
        }

        private static SqlCommand PrepareCommand(SqlConnection connection, string sql, IEnumerable<SqlParameter> parameters, SqlTransaction transaction = null)
        {
            if (sql == null || sql.Length == 0)
                throw new ArgumentNullException("sql", "sql cannot be empty or null.");

            // If we were provided a transaction, assign it
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");


            SqlCommand command = transaction == null ? new SqlCommand(sql, connection) : new SqlCommand(sql, connection, transaction);
            // Attach the command parameters if they are provided
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    if (parameter != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                            (parameter.Value == null))
                        {
                            parameter.Value = DBNull.Value;
                        }
                        command.Parameters.Add(parameter);
                    }
                }
            }
            return command;
        }

        private static string GetParameterValueForSQL(SqlParameter parameter)
        {
            if (parameter.Value == null || parameter.Value == DBNull.Value)
                return "NULL";

            switch (parameter.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.Time:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    return "'" + parameter.Value.ToString().Replace("'", "''") + "'";
                case SqlDbType.Bit:
                    return ((bool)parameter.Value) ? "1" : "0";
                default:
                    return parameter.Value.ToString().Replace("'", "''");
            }
        }

        private static void AssignParameterValues(SqlParameter[] parameters, object[] parameterValues)
        {
            if ((parameters == null) || (parameterValues == null))
            {
                // Do nothing if we get no data
                return;
            }

            // We must have the same number of values as we pave parameters to put them in
            if (parameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            // Iterate through the SqlParameters, assigning the values from the corresponding position in the 
            // value array
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameterValues[i] == null)
                {
                    parameters[i].Value = DBNull.Value;
                }
                else
                {
                    parameters[i].Value = parameterValues[i];
                }
            }
        }

        /// <summary>
        /// Determines whether the specified exception represents a transient failure that can be compensated by a retry.
        /// </summary>
        /// <param name="ex">The exception object to be verified.</param>
        /// <returns>true if the specified exception is considered as transient; otherwise, false.</returns>
        private static bool IsTransient(Exception ex)
        {
            if (ex != null)
            {
                SqlException sqlException;
                if ((sqlException = ex as SqlException) != null)
                {
                    string errorMessage = string.Empty;
                    bool isTransientError = false;

                    // Enumerate through all errors found in the exception.
                    foreach (SqlError err in sqlException.Errors)
                    {
                        errorMessage += "SQL Error: " + err.Number + ", Message: " + err.Message + Environment.NewLine;
                        switch (err.Number)
                        {
                            // SQL Error Code: 40501
                            // The service is currently busy. Retry the request after 10 seconds. Code: (reason code to be decoded).
                            case 40501:

                            // SQL Error Code: 10928
                            // Resource ID: %d. The %s limit for the database is %d and has been reached.
                            case 10928:

                            // SQL Error Code: 10929
                            // Resource ID: %d. The %s minimum guarantee is %d, maximum limit is %d and the current usage for the database is %d. 
                            // However, the server is currently too busy to support requests greater than %d for this database.
                            case 10929:

                            // SQL Error Code: 10053
                            // A transport-level error has occurred when receiving results from the server.
                            // An established connection was aborted by the software in your host machine.
                            case 10053:

                            // SQL Error Code: 10054
                            // A transport-level error has occurred when sending the request to the server. 
                            // (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)
                            case 10054:

                            // SQL Error Code: 10060
                            // A network-related or instance-specific error occurred while establishing a connection to SQL Server. 
                            // The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server 
                            // is configured to allow remote connections. (provider: TCP Provider, error: 0 - A connection attempt failed 
                            // because the connected party did not properly respond after a period of time, or established connection failed 
                            // because connected host has failed to respond.)"}
                            case 10060:

                            // SQL Error Code: 40197
                            // The service has encountered an error processing your request. Please try again.
                            case 40197:

                            // SQL Error Code: 40540
                            // The service has encountered an error processing your request. Please try again.
                            case 40540:

                            // SQL Error Code: 40613
                            // Database XXXX on server YYYY is not currently available. Please retry the connection later. If the problem persists, contact customer 
                            // support, and provide them the session tracing ID of ZZZZZ.
                            case 40613:

                            // SQL Error Code: 40143
                            // The service has encountered an error processing your request. Please try again.
                            case 40143:

                            // SQL Error Code: 233
                            // The client was unable to establish a connection because of an error during connection initialization process before login. 
                            // Possible causes include the following: the client tried to connect to an unsupported version of SQL Server; the server was too busy 
                            // to accept new connections; or there was a resource limitation (insufficient memory or maximum allowed connections) on the server. 
                            // (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)
                            case 233:

                            // SQL Error Code: 64
                            // A connection was successfully established with the server, but then an error occurred during the login process. 
                            // (provider: TCP Provider, error: 0 - The specified network name is no longer available.) 
                            case 64:

                            //A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found 
                            //or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections.
                            case 2:

                            /// Timeout expired. The timeout period elapsed prior to completion of the operation or the server is not responding. (Microsoft SQL Server, Error: -2).
                            case -2:

                            // DBNETLIB Error Code: 20
                            // The instance of SQL Server you attempted to connect to does not support encryption.
                            case 20:
                                isTransientError = true;
                                break;
                        }
                    }

                    //LogUtil.LogError("SQL Exception Details:", errorMessage);

                    return isTransientError;
                }
                else if (ex is TimeoutException)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
