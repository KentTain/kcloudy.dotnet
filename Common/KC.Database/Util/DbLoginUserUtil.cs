using KC.Framework.Base;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Framework.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace KC.Database.Util
{
    public static class DbLoginUserUtil
    {
        private const string UserPreString = "";

        /// <summary>
        /// TenantRole
        /// </summary>
        public static string DefaultTenantRole = "TenantRole";
        /// <summary>
        /// 注意，只能用于所有的Tenant都是在该Sa登录用户下的connectString去生成的数据库的
        /// </summary>
        /// <param name="tennats"></param>
        /// <param name="connectString">SA用户的连接字符串</param>
        /// <returns></returns>
        public static bool CreateTenantDbloginUserList(List<Tenant> tennats, string connectString)
        {
            bool isSuccess = true;

            foreach (var tenant in tennats)
            {
                var result = CreateTenantDbLoginUser(tenant, connectString);
                if (!result)
                    isSuccess = false;
            }

            return isSuccess;
        }
        /// <summary>
        /// 创建tenant的数据库登录用户及赋予相应的权限
        /// </summary>
        /// <param name="tenant">租户的TenantName、Database、Version、TenantPasswordHash、PrivateEncryptKey</param>
        /// <param name="connectString">SA用户的连接字符串</param>
        /// <returns></returns>
        public static bool CreateTenantDbLoginUser(Tenant tenant, string connectString)
        {
            var isProfessional = tenant.Version == TenantVersion.Professional;

            var tenantName = tenant.TenantName;
            var tenantDatabase = tenant.Database;
            var userName = UserPreString + tenantName;
            var tenantPwd = EncryptPasswordUtil.DecryptPassword(tenant.DatabasePasswordHash,
                !string.IsNullOrEmpty(tenant.PrivateEncryptKey) ? tenant.PrivateEncryptKey : TenantConstant.DefaultPrivateEncryptKey);

            if (string.IsNullOrWhiteSpace(connectString))
                connectString = GlobalConfig.GetDecryptDatabaseConnectionString();

            //创建登录名，在AzureSql下，必须在master下创建
            string masterDbConStr = ChangeDatabaseConnectionString(connectString, tenant.DatabaseType == DatabaseType.MySql ? "Sys" : "master");
            string tenantDbConStr = ChangeDatabaseConnectionString(connectString, tenantDatabase);
            var masterTenantConnection = new KC.Database.Util.TenantConnection()
            {
                TenantName = tenant.TenantName,
                DatabaseType = tenant.DatabaseType,
                ConnectionString = masterDbConStr,
            };
            var tenantOwnConnection = new KC.Database.Util.TenantConnection()
            {
                TenantName = tenant.TenantName,
                DatabaseType = tenant.DatabaseType,
                ConnectionString = tenantDbConStr,
            };

            //LogUtil.LogDebug(string.Format("{0}：创建登录账户\r\n master dbcon=[{1}] /r/n tenant dbcon=[{2}] /r/n tenant conn=[3]", tenantName, masterDbConStr, tenantDbConStr));

            //直接使用以下语句创建数据库（其他：CREATE USER、CREATE Login）：
            //Sql Azure 安全限制，见：https://azure.microsoft.com/zh-cn/documentation/articles/sql-database-security-guidelines/
            //  IF EXISTS (SELECT [name]
            //      FROM   [sys].[databases]
            //      WHERE  [name] = N'database_name')
            //         CREATE DATABASE [database_name];
            //sql Azure 会报错：The CREATE DATABASE statement must be the only statement in the batch

            #region 创建数据库
            var sbSql = new StringBuilder();
            try
            {
                //创建数据库，在AzureSql下，必须在master下创建
                if (!IsExistDatabase(masterDbConStr, tenantDatabase, tenant.DatabaseType))
                {
                    sbSql.Clear();
                    //sbSql.AppendLine(@"    GRANT VIEW any DATABASE TO PUBLIC ");
                    sbSql.AppendFormat(@"   CREATE database {0}", tenantDatabase);
                    LogUtil.LogDebug(string.Format("{0}：创建数据库（Database）--Sql：\r\n{1}", tenantName, sbSql));
                    new KC.Database.Util.DBHelper(masterTenantConnection).ExecuteNonQuery(CommandType.Text, sbSql.ToString());
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("创建数据库出错，错误消息：{0}. 脚本：{1}", ex.Message, sbSql), ex.StackTrace);
                return false;
            }
            #endregion

            #region 创建登录账户
            try
            {
                //创建登录账户，在AzureSql下，必须在master下创建
                if (!IsExistLoginUser(masterDbConStr, tenantName, tenant.DatabaseType))
                {
                    sbSql.Clear();
                    switch (tenant.DatabaseType)
                    {
                        case DatabaseType.MySql://MySql无数据库登录用户
                            break;
                        default:
                            sbSql.AppendLine(string.Format(@"CREATE LOGIN [{0}] with password=N'{1}'", tenantName, tenantPwd));
                            LogUtil.LogDebug(string.Format("{0}：创建登录账户（Login）--Sql：\r\n{1}", tenantName, sbSql));
                            new KC.Database.Util.DBHelper(masterTenantConnection).ExecuteNonQuery(CommandType.Text, sbSql.ToString());
                            break;
                    }
                }
                //租户的数据库登录账户无法登录时，需重新修改登录用户的密码：现有只支持Sql Server
                else if (tenant.DatabaseType == DatabaseType.SqlServer
                    && !string.IsNullOrWhiteSpace(tenant.ConnectionString)
                    && !CanConnectDatabase(tenant.ConnectionString, tenant.DatabaseType))
                {
                    var success1 = DeleteTenantDbLoginUser(tenant, connectString);
                    if (success1)
                    {
                        sbSql.Clear();
                        switch (tenant.DatabaseType)
                        {
                            case DatabaseType.MySql://MySql无数据库登录用户
                                break;
                            default:
                                sbSql.AppendLine(string.Format(@"CREATE LOGIN [{0}] with password=N'{1}'", tenantName, tenantPwd));
                                LogUtil.LogDebug(string.Format("{0}：创建登录账户（Login）--Sql：\r\n{1}", tenantName, sbSql));
                                new KC.Database.Util.DBHelper(masterTenantConnection).ExecuteNonQuery(CommandType.Text, sbSql.ToString());
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("创建登录账户(Login)--[Tenant]:{0}的登录用户出错，错误消息：{1}. 脚本：{2}", tenantName, ex.Message, sbSql), ex.StackTrace);
                return false;
            }
            #endregion

            #region 创建用户及给用户赋权限
            try
            {
                //创建Schema
                sbSql.Clear();
                switch (tenant.DatabaseType)
                {
                    case DatabaseType.MySql://MySql无Schema定义（Database即为Schema）
                        break;
                    default:
                        sbSql.AppendLine(@"IF NOT EXISTS (");
                        sbSql.AppendLine(@"  SELECT name FROM sys.schemas ");
                        sbSql.AppendLine(string.Format(@"    WHERE name = N'{0}' ) ", tenantName));
                        sbSql.AppendLine(@"BEGIN ");
                        //sbSql.AppendLine(string.Format(@"  EXEC N'CREATE SCHEMA [{0}] AUTHORIZATION [dbo]'", tenantName));
                        sbSql.AppendLine(string.Format(@"  EXEC (N'CREATE SCHEMA [{0}]')", tenantName));
                        sbSql.AppendLine(@"END");

                        LogUtil.LogDebug(string.Format("{0}：创建Schema--Sql：\r\n{1}", tenantName, sbSql));
                        new KC.Database.Util.DBHelper(tenantOwnConnection).ExecuteNonQuery(true, CommandType.Text, sbSql.ToString());
                        break;
                }

                if (!IsExistUser(tenantDbConStr, userName, tenant.DatabaseType))
                {
                    sbSql.Clear();
                    switch (tenant.DatabaseType)
                    {
                        case DatabaseType.MySql:
                            sbSql.AppendLine(string.Format(@"CREATE USER '{0}'@'%' IDENTIFIED BY '{1}';", userName, tenantPwd));
                            sbSql.AppendLine(string.Format(@"GRANT all privileges on `{0}`.* to '{1}'@'%';", tenantDatabase, userName));
                            //LogUtil.LogDebug(string.Format("{0}：用户授权（User:{1}）--Sql：\r\n{2}", tenantName, userName, sbSql));
                            new KC.Database.Util.DBHelper(masterTenantConnection).ExecuteNonQuery(true, CommandType.Text, sbSql.ToString());

                            sbSql.Clear();
                            sbSql.AppendLine(string.Format(@"FLUSH privileges"));
                            new KC.Database.Util.DBHelper(masterTenantConnection).ExecuteNonQuery(CommandType.Text, sbSql.ToString());
                            break;
                        default:
                            sbSql.AppendLine(string.Format(@"CREATE USER [{0}] FOR LOGIN [{1}] WITH DEFAULT_SCHEMA=[{1}]", userName, tenantName));
                            LogUtil.LogDebug(string.Format("{0}：创建数据库用户--Sql：\r\n{1}", tenantName, sbSql));
                            new KC.Database.Util.DBHelper(tenantOwnConnection).ExecuteNonQuery(CommandType.Text, sbSql.ToString());

                            sbSql.Clear();
                            sbSql.AppendLine(string.Format(@"GRANT ALTER, DELETE, EXECUTE, INSERT, REFERENCES, SELECT, UPDATE ON SCHEMA::[{0}] TO [{1}]", tenantName, userName));
                            sbSql.AppendLine(string.Format(@"EXEC sp_addrolemember N'db_ddladmin', N'{0}'", userName));
                            LogUtil.LogDebug(string.Format("{0}：用户授权（User）--Sql：\r\n{1}", tenantName, sbSql));
                            new KC.Database.Util.DBHelper(tenantOwnConnection).ExecuteNonQuery(true, CommandType.Text, sbSql.ToString());
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("创建用户角色及给角色赋权限--[Tenant]:{0}的登录用户出错，错误消息：{1}. 脚本：{2}", tenantName, ex.Message, sbSql), ex.StackTrace);
                return false;
            }
            #endregion

            #region 限制登录账户（Login）的访问权限
            try
            {
                if (isProfessional && IsExistLoginUser(masterDbConStr, tenantName, tenant.DatabaseType))
                {
                    //sbSql.Clear();
                    //sbSql.AppendLine(string.Format("    DENY VIEW any DATABASE TO PUBLIC "));
                    //sbSql.AppendLine(string.Format("    ALTER AUTHORIZATION ON DATABASE::[{0}] TO [{1}] ", tenantDatabase, tenantName));
                    //LogUtil.LogDebug(string.Format("{0}：限制登录账户（Login）的访问权限--Sql：\r\n{1}", tenantName, sbSql));
                    //SqlDatabaseHelper.ExecuteNonQuery(masterDbConStr, sbSql.ToString(), null);
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("创建登录账户(Login)--[Tenant]:{0}的登录用户出错，错误消息：{1}. 脚本：{2}", tenantName, ex.Message, sbSql), ex.StackTrace);
                return false;
            }
            #endregion

            return true;
        }
        /// <summary>
        /// 删除租户的数据库登录用户
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="connectString">SA用户的连接字符串</param>
        /// <returns></returns>
        public static bool DeleteTenantDbLoginUser(Tenant tenant, string connectString)
        {
            var tenantName = tenant.TenantName;
            var tenantDatabase = tenant.Database;
            var userName = UserPreString + tenantName;

            if (string.IsNullOrWhiteSpace(connectString))
                connectString = GlobalConfig.GetDecryptDatabaseConnectionString();

            //创建登录名，在AzureSql下，必须在master下创建
            string masterDbConStr = ChangeDatabaseConnectionString(connectString, tenant.DatabaseType == DatabaseType.MySql ? "Sys" : "master");
            string tenantDbConStr = ChangeDatabaseConnectionString(connectString, tenantDatabase);
            var masterTenantConnection = new KC.Database.Util.TenantConnection()
            {
                TenantName = tenant.TenantName,
                DatabaseType = tenant.DatabaseType,
                ConnectionString = masterDbConStr,
            };
            var tenantOwnConnection = new KC.Database.Util.TenantConnection()
            {
                TenantName = tenant.TenantName,
                DatabaseType = tenant.DatabaseType,
                ConnectionString = tenantDbConStr,
            };

            try
            {
                var sbSql = new StringBuilder();
                //删除数据库Schema的用户
                if (IsExistUser(tenantDbConStr, userName, tenant.DatabaseType))
                {
                    sbSql.Clear();
                    switch (tenant.DatabaseType)
                    {
                        case DatabaseType.MySql:
                            sbSql.AppendLine(string.Format(@"DROP USER '{0}'@'%'", tenantName));
                            //LogUtil.LogDebug(string.Format("{0}：删除数据库用户（User）--Sql：\r\n{1}", tenantName, sbSql));
                            new KC.Database.Util.DBHelper(masterTenantConnection).ExecuteNonQuery(true, CommandType.Text, sbSql.ToString());
                            break;
                        default:
                            sbSql.AppendLine(string.Format(@"   ALTER AUTHORIZATION ON SCHEMA::[{0}] TO [dbo]", tenantName));
                            //sbSql.AppendLine(string.Format(@"   EXEC sp_droprolemember N'{0}', N'{1}' ", DefaultTenantRole, userName));
                            sbSql.AppendLine(string.Format(@"   DROP USER {0}", userName));
                            //LogUtil.LogDebug(string.Format("{0}：删除数据库用户（User）--Sql：\r\n{1}", tenantName, sbSql));
                            new KC.Database.Util.DBHelper(tenantOwnConnection).ExecuteNonQuery(true, CommandType.Text, sbSql.ToString());
                            break;
                    }

                }

                //删除Tenant角色
                //sbSql.Clear();
                //sbSql.AppendLine(@"IF NOT EXISTS (");
                //sbSql.AppendLine(@"  SELECT members.[name] ");
                //sbSql.AppendLine(@"  FROM sys.database_role_members AS rolemembers ");
                //sbSql.AppendLine(@"     JOIN sys.database_principals AS roles ON roles.[principal_id] = rolemembers.[role_principal_id] ");
                //sbSql.AppendLine(@"     JOIN sys.database_principals AS members ON members.[principal_id] = rolemembers.[member_principal_id] ");
                //sbSql.AppendLine(string.Format(@"    WHERE roles.[Name]=N'{0}' ) ", DefaultTenantRole));
                //sbSql.AppendLine(@"BEGIN");
                //sbSql.AppendLine(string.Format(@"   DROP ROLE [{0}] ", DefaultTenantRole));
                //sbSql.AppendLine(@"END");
                //LogUtil.LogDebug(string.Format("{0}：删除Tenant角色（Role）--Sql：\r\n{1}", tenantName, sbSql));
                //SqlDatabaseHelper.TranExecuteNonQuery(tenantDbConStr, sbSql.ToString(), null);

                //删除登录用户(Login)
                if (tenant.DatabaseType == DatabaseType.SqlServer
                    && IsExistLoginUser(masterDbConStr, tenantName, tenant.DatabaseType))
                {
                    sbSql.Clear();
                    sbSql.AppendLine(string.Format(@"   DROP LOGIN [{0}]", tenantName));
                    //LogUtil.LogDebug(string.Format("{0}：删除登录账户（Login）--Sql：\r\n{1}", tenantName, sbSql));
                    new KC.Database.Util.DBHelper(masterTenantConnection).ExecuteNonQuery(CommandType.Text, sbSql.ToString());
                }

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("删除Login及User--[Tenant]:{0}的登录用户出错，错误消息：{1}", tenantName, ex.Message), ex.StackTrace);
                return false;
            }
        }

        private static bool IsExistDatabase(string masterDbConStr, string tenantDatabase, DatabaseType type)
        {
            var masterTenantConnection = new KC.Database.Util.TenantConnection()
            {
                DatabaseType = type,
                ConnectionString = masterDbConStr,
            };
            var sbSql = new StringBuilder();
            switch (type)
            {
                case DatabaseType.MySql:
                    //sbSql.AppendFormat(
                    //    @"SELECT (CASE WHEN count(*)> 0 THEN 1 ELSE 0 END) as Num FROM information_schema.SCHEMATA where SCHEMA_NAME=N'{0}'",
                    //    tenantDatabase);
                    sbSql.AppendFormat(
                        @"SELECT EXISTS (SELECT 1 FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = '{0}') AS Num;",
                        tenantDatabase);
                    break;
                default:
                    sbSql.AppendFormat(
                        @"IF db_id(N'{0}') IS NOT NULL SELECT 1 ELSE SELECT Count(*) FROM sys.databases WHERE [name]=N'{0}'",
                        tenantDatabase);
                    break;
            }
            var result = new KC.Database.Util.DBHelper(masterTenantConnection).ExecuteScalar(CommandType.Text, sbSql.ToString());
            bool isExistDatabase = int.Parse(result.ToString()) > 0;
            return isExistDatabase;
        }
        private static bool IsExistLoginUser(string masterDbConStr, string tenantName, DatabaseType type)
        {
            var masterTenantConnection = new KC.Database.Util.TenantConnection()
            {
                DatabaseType = type,
                ConnectionString = masterDbConStr,
            };
            var sbSql = new StringBuilder();
            switch (type)
            {
                case DatabaseType.MySql://MySql无数据库登录用户
                    return true;
                default:
                    sbSql.AppendFormat(
                        @"IF NOT EXISTS (SELECT [Name] FROM sys.sql_logins WHERE [Name]=N'{0}') SELECT 0 ELSE SELECT 1",
                        tenantName);
                    break;
            }
            var result = new KC.Database.Util.DBHelper(masterTenantConnection).ExecuteScalar(CommandType.Text, sbSql.ToString());
            bool isExistUser = int.Parse(result.ToString()) > 0;
            return isExistUser;
        }
        private static bool IsExistUser(string masterDbConStr, string userName, DatabaseType type)
        {
            var masterTenantConnection = new KC.Database.Util.TenantConnection()
            {
                DatabaseType = type,
                ConnectionString = masterDbConStr,
            };
            var sbSql = new StringBuilder();
            switch (type)
            {
                case DatabaseType.MySql:
                    //sbSql.AppendFormat(
                    //    @"SELECT (CASE WHEN count(*)> 0 THEN 1 ELSE 0 END) as Num FROM mysql.user where User='{0}'",
                    //    userName);
                    sbSql.AppendFormat(
                        @"SELECT EXISTS (SELECT 1 FROM mysql.user WHERE User = '{0}') AS Num;",
                        userName);
                    break;
                default:
                    sbSql.AppendFormat(
                        @"IF NOT EXISTS (SELECT [Name] FROM sys.database_principals WHERE [Name]=N'{0}') SELECT 0 ELSE SELECT 1",
                        userName);
                    break;
            }
            var result = new KC.Database.Util.DBHelper(masterTenantConnection).ExecuteScalar(CommandType.Text, sbSql.ToString());
            bool isExistUser = int.Parse(result.ToString()) > 0;
            return isExistUser;
        }
        public static bool CanConnectDatabase(string connectionStr, DatabaseType type)
        {
            try
            {
                var masterTenantConnection = new KC.Database.Util.TenantConnection()
                {
                    DatabaseType = type,
                    ConnectionString = connectionStr,
                };

                return new KC.Database.Util.DBHelper(masterTenantConnection).CanConnectDatabase();
            }
            catch (Exception ex)
            {
                var errorMsg = string.Format(
                    "连接数据库[Connection String：{0}]出错; " + Environment.NewLine +
                    "错误消息：{1}; " + Environment.NewLine +
                    "错误详情：{2}",
                    connectionStr, ex.Message, ex.StackTrace);
                return false;
            }
        }

        private static string ChangeDatabaseConnectionString(string connectString, string replaceDatabseName)
        {
            return new Regex("(?<=Database=)(.*?)(?=;User ID)", RegexOptions.IgnoreCase).Replace(connectString, replaceDatabseName);
        }
    }
}
