using KC.Database.EFRepository;
using KC.Database.Extension;
using KC.Enums.App;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Model.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace KC.DataAccess.Admin
{
    /// <summary>
    /// 数据库初始化操作类
    /// </summary>
    public class ComAdminDatabaseInitializer : MultiTenantSqlServerDatabaseInitializer<ComAdminContext>
    {
        public ComAdminDatabaseInitializer()
            : base()
        { }

        public override ComAdminContext Create(Tenant tenant)
        {
            var dbaTenant = TenantConstant.DbaTenantApiAccessInfo;

            var options = GetCachedDbContextOptions(dbaTenant.TenantName, dbaTenant.ConnectionString, dbaTenant.DatabaseType);
            return new ComAdminContext(options, null, null);
        }

        protected override string GetTargetMigration()
        {
            return KC.DataAccess.Admin.DataInitial.DBSqlInitializer.GetPreMigrationVersion();
        }

        public void SeedData(Tenant tenant)
        {
            using (var context = Create(null))
            {
                var connString = context.Database.GetDbConnection().ConnectionString;
                var builder = new SqlConnectionStringBuilder(connString);
                var server = builder.DataSource;
                var database = builder.InitialCatalog;
                var user = builder.UserID;
                var pwd = builder.Password;
                var hashPwd = EncryptPasswordUtil.EncryptPassword(pwd);

                #region DatabasePool
                var databasePool = new DatabasePool
                {
                    CloudType = CloudType.Aliyun,
                    DatabaseType = TenantConstant.DefaultDatabaseType,
                    DatabasePoolId = 1,
                    Server = server,
                    Database = database,
                    UserName = user,
                    TenantCount = 2,
                    CanEdit = false,
                    UserPasswordHash = hashPwd,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                };
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //context.DatabasePools.Add(databasePool);

                        //LogUtil.LogDebug(string.Format("---Database----server：{0}; Database: {1}; Password: {2}; PwshHash: {3}", server, database, pwd, hashPwd));
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_DatabasePool] ON ", TenantConstant.DbaTenantName));
                        context.AddOrUpdate(databasePool);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_DatabasePool] OFF ", TenantConstant.DbaTenantName));
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogFatal(string.Format("-------Admin Config insert Database Pool is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                    }
                }
                #endregion

                #region StoragePool

                var storagePool = new StoragePool { StoragePoolId = 1 };
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var connecttems = TenantConstant.GetStorageConnectionItems(false);
                        var endpont = connecttems.Item1;
                        var accessName = connecttems.Item2;
                        var accessKey = connecttems.Item3;

                        var storageEncryptKey = EncryptPasswordUtil.EncryptPassword(accessKey);
                        //LogUtil.LogDebug(string.Format("---Storage----Endpoint：{0}; AccountName: {1}; AccountKey: {2}; EncryptKey: {3}", endpont, accessName, accessKey, storageEncryptKey));
                        if (!string.IsNullOrEmpty(endpont))
                        {
                            storagePool = new StoragePool
                            {
                                CloudType = CloudType.Aliyun,
                                StorageType = TenantConstant.DefaultStorageType,
                                StoragePoolId = 1,
                                Endpoint = endpont,
                                AccessName = accessName,
                                TenantCount = 2,
                                CanEdit = false,
                                AccessKeyPasswordHash = storageEncryptKey,
                                IsDeleted = false,
                                CreatedBy = RoleConstants.AdminUserId,
                                CreatedName = RoleConstants.AdminUserName,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedBy = RoleConstants.AdminUserId,
                                ModifiedName = RoleConstants.AdminUserName,
                                ModifiedDate = DateTime.UtcNow,
                            };
                        }

                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_StoragePool] ON ", TenantConstant.DbaTenantName));
                        context.AddOrUpdate(storagePool);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_StoragePool] OFF ", TenantConstant.DbaTenantName));
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogFatal(string.Format("-------Admin Config insert Storage Pool is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                    }
                }
                #endregion

                #region QueuePool
                var queuePool = new QueuePool { QueuePoolId = 1 };
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var connecttems = TenantConstant.GetQueueConnectionItems(false);
                        var queueEndpont = connecttems.Item1;
                        var queueName = connecttems.Item2;
                        var queueKey = connecttems.Item3;

                        var storageEncryptKey = EncryptPasswordUtil.EncryptPassword(queueKey);
                        //LogUtil.LogDebug(string.Format("---Queue----Endpoint：{0}; AccountName: {1}; AccountKey: {2}; EncryptKey: {3}", queueEndpont, queueName, queueKey, storageEncryptKey));
                        if (!string.IsNullOrEmpty(queueEndpont))
                        {
                            queuePool = new QueuePool
                            {
                                CloudType = CloudType.Aliyun,
                                QueueType = TenantConstant.DefaultQueueType,
                                QueuePoolId = 1,
                                Endpoint = queueEndpont,
                                AccessName = queueName,
                                TenantCount = 2,
                                CanEdit = false,
                                AccessKeyPasswordHash = storageEncryptKey,
                                IsDeleted = false,
                                CreatedBy = RoleConstants.AdminUserId,
                                CreatedName = RoleConstants.AdminUserName,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedBy = RoleConstants.AdminUserId,
                                ModifiedName = RoleConstants.AdminUserName,
                                ModifiedDate = DateTime.UtcNow,
                            };
                        }

                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_QueuePool] ON ", TenantConstant.DbaTenantName));
                        context.AddOrUpdate(queuePool);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_QueuePool] OFF ", TenantConstant.DbaTenantName));
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogFatal(string.Format("-------Admin Config insert Queue Pool is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                    }
                }
                #endregion

                #region NoSqlPool

                var noSqlPool = new NoSqlPool { NoSqlPoolId = 1 };
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var connecttems = TenantConstant.GetNoSqlConnectionItems(false);
                        var noSqlEndpont = connecttems.Item1;
                        var noSqlName = connecttems.Item2;
                        var noSqlKey = connecttems.Item3;

                        var storageEncryptKey = EncryptPasswordUtil.EncryptPassword(noSqlKey);
                        //LogUtil.LogDebug(string.Format("---NoSql----Endpoint：{0}; AccountName: {1}; AccountKey: {2}; EncryptKey: {3}", noSqlEndpont, noSqlName, noSqlKey, storageEncryptKey));
                        if (!string.IsNullOrEmpty(noSqlEndpont))
                        {
                            noSqlPool = new NoSqlPool
                            {
                                CloudType = CloudType.Aliyun,
                                NoSqlType = TenantConstant.DefaultNoSqlType,
                                NoSqlPoolId = 1,
                                Endpoint = noSqlEndpont,
                                AccessName = noSqlName,
                                TenantCount = 2,
                                CanEdit = false,
                                AccessKeyPasswordHash = storageEncryptKey,
                                IsDeleted = false,
                                CreatedBy = RoleConstants.AdminUserId,
                                CreatedName = RoleConstants.AdminUserName,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedBy = RoleConstants.AdminUserId,
                                ModifiedName = RoleConstants.AdminUserName,
                                ModifiedDate = DateTime.UtcNow,
                            };
                        }

                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_NoSqlPool] ON ", TenantConstant.DbaTenantName));
                        context.AddOrUpdate(noSqlPool);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_NoSqlPool] OFF ", TenantConstant.DbaTenantName));
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogFatal(string.Format("-------Admin Config insert NoSql Pool is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                    }
                }
                #endregion

                #region ServiceBusPool

                var serviceBusPool = new ServiceBusPool { ServiceBusPoolId = 1 };
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var connecttems = TenantConstant.GetServiceBusConnectionItems(false);
                        var serviceBusEndpont = connecttems.Item1;
                        var serviceBusName = connecttems.Item2;
                        var serviceBusKey = connecttems.Item3;

                        var storageEncryptKey = EncryptPasswordUtil.EncryptPassword(serviceBusKey);
                        //LogUtil.LogDebug(string.Format("---ServiceBus----Endpoint：{0}; AccountName: {1}; AccountKey: {2}; EncryptKey: {3}", serviceBusEndpont, serviceBusName, serviceBusKey, storageEncryptKey));
                        if (!string.IsNullOrEmpty(serviceBusEndpont))
                        {
                            serviceBusPool = new ServiceBusPool
                            {
                                CloudType = CloudType.Aliyun,
                                ServiceBusType = TenantConstant.DefaultServiceBusType,
                                ServiceBusPoolId = 1,
                                Endpoint = serviceBusEndpont,
                                AccessName = serviceBusName,
                                TenantCount = 2,
                                CanEdit = false,
                                AccessKeyPasswordHash = storageEncryptKey,
                                IsDeleted = false,
                                CreatedBy = RoleConstants.AdminUserId,
                                CreatedName = RoleConstants.AdminUserName,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedBy = RoleConstants.AdminUserId,
                                ModifiedName = RoleConstants.AdminUserName,
                                ModifiedDate = DateTime.UtcNow,
                            };
                        }

                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_ServiceBusPool] ON ", TenantConstant.DbaTenantName));
                        context.AddOrUpdate(serviceBusPool);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_ServiceBusPool] OFF ", TenantConstant.DbaTenantName));
                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        LogUtil.LogFatal(string.Format("-------Admin Config insert ServiceBus Pool is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                    }
                }
                #endregion

                #region VodPool

                var vodPool = new VodPool { VodPoolId = 1 };
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var connecttems = TenantConstant.GetVodConnectionItems(false);
                        var endpont = connecttems.Item1;
                        var accessName = connecttems.Item2;
                        var accessKey = connecttems.Item3;

                        var encryptKey = EncryptPasswordUtil.EncryptPassword(accessKey);
                        //LogUtil.LogDebug(string.Format("---Vod----Endpoint：{0}; AccountName: {1}; AccountKey: {2}; EncryptKey: {3}", serviceBusEndpont, serviceBusName, serviceBusKey, storageEncryptKey));
                        if (!string.IsNullOrEmpty(endpont))
                        {
                            vodPool = new VodPool
                            {
                                CloudType = CloudType.Aliyun,
                                VodType = TenantConstant.DefaultVodType,
                                VodPoolId = 1,
                                Endpoint = endpont,
                                AccessName = accessName,
                                TenantCount = 2,
                                CanEdit = false,
                                AccessKeyPasswordHash = encryptKey,
                                IsDeleted = false,
                                CreatedBy = RoleConstants.AdminUserId,
                                CreatedName = RoleConstants.AdminUserName,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedBy = RoleConstants.AdminUserId,
                                ModifiedName = RoleConstants.AdminUserName,
                                ModifiedDate = DateTime.UtcNow,
                            };
                        }

                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_VodPool] ON ", TenantConstant.DbaTenantName));
                        context.AddOrUpdate(vodPool);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_VodPool] OFF ", TenantConstant.DbaTenantName));
                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        LogUtil.LogFatal(string.Format("-------Admin Config insert Vod Pool is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                    }
                }
                #endregion

                #region CodePool

                var codePool = new CodeRepositoryPool { CodePoolId = 1 };
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var connecttems = TenantConstant.GetCodeConnectionItems(false);
                        var endpont = connecttems.Item1;
                        var accessName = connecttems.Item2;
                        var accessKey = connecttems.Item3;

                        var encryptKey = EncryptPasswordUtil.EncryptPassword(accessKey);
                        //LogUtil.LogDebug(string.Format("---Code----Endpoint：{0}; AccountName: {1}; AccountKey: {2}; EncryptKey: {3}", serviceBusEndpont, serviceBusName, serviceBusKey, storageEncryptKey));
                        if (!string.IsNullOrEmpty(endpont))
                        {
                            codePool = new CodeRepositoryPool
                            {
                                CloudType = CloudType.Aliyun,
                                CodeType = TenantConstant.DefaultCodeType,
                                CodePoolId = 1,
                                Endpoint = endpont,
                                AccessName = accessName,
                                TenantCount = 2,
                                CanEdit = false,
                                AccessKeyPasswordHash = encryptKey,
                                IsDeleted = false,
                                CreatedBy = RoleConstants.AdminUserId,
                                CreatedName = RoleConstants.AdminUserName,
                                CreatedDate = DateTime.UtcNow,
                                ModifiedBy = RoleConstants.AdminUserId,
                                ModifiedName = RoleConstants.AdminUserName,
                                ModifiedDate = DateTime.UtcNow,
                            };
                        }

                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_CodeRepositoryPool] ON ", TenantConstant.DbaTenantName));
                        context.AddOrUpdate(codePool);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_CodeRepositoryPool] OFF ", TenantConstant.DbaTenantName));
                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        LogUtil.LogFatal(string.Format("-------Admin Config insert Code Pool is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                    }
                }
                #endregion

                #region cDba Tenant's Application
                var ssoWebDomain = GlobalConfig.SSOWebDomain?.TrimEnd('/');
                var adminWebDomain = GlobalConfig.AdminWebDomain?.TrimEnd('/');
                var blogWebDomain = GlobalConfig.BlogWebDomain?.TrimEnd('/');
                var codeWebDomain = GlobalConfig.CodeWebDomain?.TrimEnd('/');
                var cfgWebDomain = GlobalConfig.CfgWebDomain?.TrimEnd('/');
                var dicWebDomain = GlobalConfig.DicWebDomain?.TrimEnd('/');
                var appWebDomain = GlobalConfig.AppWebDomain?.TrimEnd('/');
                var msgWebDomain = GlobalConfig.MsgWebDomain?.TrimEnd('/');
                var accWebDomain = GlobalConfig.AccWebDomain?.TrimEnd('/');
                var econWebDomain = GlobalConfig.EconWebDomain?.TrimEnd('/');
                var docWebDomain = GlobalConfig.DocWebDomain?.TrimEnd('/');
                var hrWebDomain = GlobalConfig.HrWebDomain?.TrimEnd('/');
                var crmWebDomain = GlobalConfig.CrmWebDomain?.TrimEnd('/');
                var srmWebDomain = GlobalConfig.SrmWebDomain?.TrimEnd('/');
                var prdWebDomain = GlobalConfig.PrdWebDomain?.TrimEnd('/');
                var pmcWebDomain = GlobalConfig.PmcWebDomain?.TrimEnd('/');
                var portalWebDomain = GlobalConfig.PortalWebDomain?.TrimEnd('/');
                var somWebDomain = GlobalConfig.SomWebDomain?.TrimEnd('/');
                var pomWebDomain = GlobalConfig.PomWebDomain?.TrimEnd('/');
                var wmsWebDomain = GlobalConfig.WmsWebDomain?.TrimEnd('/');
                var jrWebDomain = GlobalConfig.JRWebDomain?.TrimEnd('/');
                var trainWebDomain = GlobalConfig.TrainWebDomain?.TrimEnd('/');
                var flowWebDomain = GlobalConfig.WorkflowWebDomain?.TrimEnd('/');
                var payWebDomain = GlobalConfig.PayWebDomain?.TrimEnd('/');
                var wxWebDomain = GlobalConfig.WXWebDomain?.TrimEnd('/');
                var apiWebDomain = GlobalConfig.ApiWebDomain?.TrimEnd('/') + "/api";

                var ssoWeb = new TenantUserApplication()
                {
                    Id = 1,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.SsoAppId,
                    ApplicationName = ApplicationConstant.SsoAppName,
                    WebSiteName = ApplicationConstant.SsoScope,
                    DomainName = ssoWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Account",
                };
                var adminWeb = new TenantUserApplication()
                {
                    Id = 2,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.AdminAppId,
                    ApplicationName = ApplicationConstant.AdminAppName,
                    WebSiteName = ApplicationConstant.AdminScope,
                    DomainName = adminWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Admin",
                };
                var blogWeb = new TenantUserApplication()
                {
                    Id = 3,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.BlogAppId,
                    ApplicationName = ApplicationConstant.BlogAppName,
                    WebSiteName = ApplicationConstant.BlogScope,
                    DomainName = blogWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Blog",
                };
                var cfgWeb = new TenantUserApplication()
                {
                    Id = 4,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.ConfigAppId,
                    ApplicationName = ApplicationConstant.ConfigAppName,
                    WebSiteName = ApplicationConstant.CfgScope,
                    DomainName = cfgWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Config",
                };
                var dicWeb = new TenantUserApplication()
                {
                    Id = 5,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.DictAppId,
                    ApplicationName = ApplicationConstant.DictAppName,
                    WebSiteName = ApplicationConstant.DicScope,
                    DomainName = dicWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Dict",
                };
                var appWeb = new TenantUserApplication()
                {
                    Id = 6,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.AppAppId,
                    ApplicationName = ApplicationConstant.AppAppName,
                    WebSiteName = ApplicationConstant.AppScope,
                    DomainName = appWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.App",
                };
                var msgWeb = new TenantUserApplication()
                {
                    Id = 7,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.MsgAppId,
                    ApplicationName = ApplicationConstant.MsgAppName,
                    WebSiteName = ApplicationConstant.MsgScope,
                    DomainName = msgWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Message",
                };
                var accWeb = new TenantUserApplication()
                {
                    Id = 8,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.AccAppId,
                    ApplicationName = ApplicationConstant.AccAppName,
                    WebSiteName = ApplicationConstant.AccScope,
                    DomainName = accWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Account",
                };
                var econWeb = new TenantUserApplication()
                {
                    Id = 9,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.EconAppId,
                    ApplicationName = ApplicationConstant.EconAppName,
                    WebSiteName = ApplicationConstant.EconScope,
                    DomainName = econWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Contract",
                };
                var docWeb = new TenantUserApplication()
                {
                    Id = 10,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.DocAppId,
                    ApplicationName = ApplicationConstant.DocAppName,
                    WebSiteName = ApplicationConstant.DocScope,
                    DomainName = docWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Doc",
                };
                var hrWeb = new TenantUserApplication()
                {
                    Id = 11,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.HrAppId,
                    ApplicationName = ApplicationConstant.HrAppName,
                    WebSiteName = ApplicationConstant.HrScope,
                    DomainName = hrWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.HumanResource",
                };
                var crmWeb = new TenantUserApplication()
                {
                    Id = 12,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.CrmAppId,
                    ApplicationName = ApplicationConstant.CrmAppName,
                    WebSiteName = ApplicationConstant.CrmScope,
                    DomainName = crmWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Customer",
                };
                var srmWeb = new TenantUserApplication()
                {
                    Id = 13,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.SrmAppId,
                    ApplicationName = ApplicationConstant.SrmAppName,
                    WebSiteName = ApplicationConstant.SrmScope,
                    DomainName = srmWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Supplier",
                };
                var prdWeb = new TenantUserApplication()
                {
                    Id = 14,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.PrdAppId,
                    ApplicationName = ApplicationConstant.PrdAppName,
                    WebSiteName = ApplicationConstant.PrdScope,
                    DomainName = prdWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Offering",
                };
                var pmcWeb = new TenantUserApplication()
                {
                    Id = 15,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.PmcAppId,
                    ApplicationName = ApplicationConstant.PmcAppName,
                    WebSiteName = ApplicationConstant.PmcScope,
                    DomainName = pmcWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Material",
                };
                var portalWeb = new TenantUserApplication()
                {
                    Id = 16,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.PortalAppId,
                    ApplicationName = ApplicationConstant.PortalAppName,
                    WebSiteName = ApplicationConstant.PortalScope,
                    DomainName = portalWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Portal",
                };
                var somWeb = new TenantUserApplication()
                {
                    Id = 17,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.SomAppId,
                    ApplicationName = ApplicationConstant.SomAppName,
                    WebSiteName = ApplicationConstant.SomScope,
                    DomainName = somWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Sales",
                };
                var pomWeb = new TenantUserApplication()
                {
                    Id = 18,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.PomAppId,
                    ApplicationName = ApplicationConstant.PomAppName,
                    WebSiteName = ApplicationConstant.PomScope,
                    DomainName = pomWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Purchase",
                };
                var wmsWeb = new TenantUserApplication()
                {
                    Id = 19,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.WmsAppId,
                    ApplicationName = ApplicationConstant.WmsAppName,
                    WebSiteName = ApplicationConstant.WmsScope,
                    DomainName = wmsWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Warehouse",
                };
                var jrWeb = new TenantUserApplication()
                {
                    Id = 20,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.JrAppId,
                    ApplicationName = ApplicationConstant.JrAppName,
                    WebSiteName = ApplicationConstant.JrScope,
                    DomainName = jrWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Finance",
                };
                var trainWeb = new TenantUserApplication()
                {
                    Id = 21,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.TrainAppId,
                    ApplicationName = ApplicationConstant.TrainAppName,
                    WebSiteName = ApplicationConstant.TrainScope,
                    DomainName = jrWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Training",
                };
                var flowWeb = new TenantUserApplication()
                {
                    Id = 22,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.WorkflowAppId,
                    ApplicationName = ApplicationConstant.WorkflowAppName,
                    WebSiteName = ApplicationConstant.WorkflowScope,
                    DomainName = flowWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Workflow",
                };
                var payWeb = new TenantUserApplication()
                {
                    Id = 23,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.PayAppId,
                    ApplicationName = ApplicationConstant.PayAppName,
                    WebSiteName = ApplicationConstant.PayScope,
                    DomainName = payWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Pay",
                };
                var wxWeb = new TenantUserApplication()
                {
                    Id = 24,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.WXAppId,
                    ApplicationName = ApplicationConstant.WXAppName,
                    WebSiteName = ApplicationConstant.WXScope,
                    DomainName = wxWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.Wechat",
                };
                var apiWeb = new TenantUserApplication()
                {
                    Id = 25,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.WebapiAppId,
                    ApplicationName = ApplicationConstant.WebapiAppName,
                    WebSiteName = ApplicationConstant.WebapiScope,
                    DomainName = apiWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess
                };
                var codeWeb = new TenantUserApplication()
                {
                    Id = 26,
                    TenantId = TenantConstant.DbaTenantId,
                    ApplicationId = ApplicationConstant.CodeAppId,
                    ApplicationName = ApplicationConstant.CodeAppName,
                    WebSiteName = ApplicationConstant.CodeScope,
                    DomainName = codeWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.DbaTenantName),
                    AppStatus = ApplicationStatus.OpenSuccess,
                    AssemblyName = "KC.DataAccess.CodeGenerate",
                };

                var dbaApps = new List<TenantUserApplication>()
                {
                    ssoWeb, adminWeb, blogWeb, codeWeb, cfgWeb, dicWeb, appWeb, msgWeb, accWeb,
                    econWeb, docWeb, hrWeb, crmWeb, srmWeb, prdWeb, pmcWeb, portalWeb,
                    somWeb, pomWeb, wmsWeb, jrWeb, trainWeb, flowWeb, payWeb, wxWeb, apiWeb
                };
                //var dbaAppModules = dbaApps.SelectMany(m => m.ApplicationModules).ToList();

                #endregion

                #region cTest Tenant's Application
                var tssoWeb = ObjectExtensions.DeepClone(ssoWeb);
                tssoWeb.Id = 101;
                tssoWeb.DomainName = ssoWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tssoWeb.ApplicationModules.ToList().ForEach(m => m.Id = 101);
                var tadminWeb = ObjectExtensions.DeepClone(adminWeb);
                tadminWeb.Id = 102;
                tadminWeb.DomainName = adminWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tadminWeb.ApplicationModules.ToList().ForEach(m => m.Id = 102);
                var tblogWeb = ObjectExtensions.DeepClone(blogWeb);
                tblogWeb.Id = 103;
                tblogWeb.DomainName = blogWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tblogWeb.ApplicationModules.ToList().ForEach(m => m.Id = 103);
                var tcfgWeb = ObjectExtensions.DeepClone(cfgWeb);
                tcfgWeb.Id = 104;
                tcfgWeb.DomainName = cfgWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tcfgWeb.ApplicationModules.ToList().ForEach(m => m.Id = 104);
                var tdicWeb = ObjectExtensions.DeepClone(dicWeb);
                tdicWeb.Id = 105;
                tdicWeb.DomainName = dicWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tdicWeb.ApplicationModules.ToList().ForEach(m => m.Id = 105);
                var tappWeb = ObjectExtensions.DeepClone(appWeb);
                tappWeb.Id = 106;
                tappWeb.DomainName = appWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tappWeb.ApplicationModules.ToList().ForEach(m => m.Id = 106);
                var tmsgWeb = ObjectExtensions.DeepClone(msgWeb);
                tmsgWeb.Id = 107;
                tmsgWeb.DomainName = msgWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tmsgWeb.ApplicationModules.ToList().ForEach(m => m.Id = 107);
                var taccWeb = ObjectExtensions.DeepClone(accWeb);
                taccWeb.Id = 108;
                taccWeb.DomainName = accWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //taccWeb.ApplicationModules.ToList().ForEach(m => m.Id = 108);
                var teconWeb = ObjectExtensions.DeepClone(econWeb);
                teconWeb.Id = 109;
                teconWeb.DomainName = econWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //teconWeb.ApplicationModules.ToList().ForEach(m => m.Id = 109);
                var tdocWeb = ObjectExtensions.DeepClone(docWeb);
                tdocWeb.Id = 110;
                tdocWeb.DomainName = docWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tdocWeb.ApplicationModules.ToList().ForEach(m => m.Id = 110);
                var thrWeb = ObjectExtensions.DeepClone(hrWeb);
                thrWeb.Id = 111;
                thrWeb.DomainName = hrWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //thrWeb.ApplicationModules.ToList().ForEach(m => m.Id = 111);
                var tcrmWeb = ObjectExtensions.DeepClone(crmWeb);
                tcrmWeb.Id = 112;
                tcrmWeb.DomainName = crmWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tcrmWeb.ApplicationModules.ToList().ForEach(m => m.Id = 112);
                var tsrmWeb = ObjectExtensions.DeepClone(srmWeb);
                tsrmWeb.Id = 113;
                tsrmWeb.DomainName = srmWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tsrmWeb.ApplicationModules.ToList().ForEach(m => m.Id = 113);
                var tprdWeb = ObjectExtensions.DeepClone(prdWeb);
                tprdWeb.Id = 114;
                tprdWeb.DomainName = prdWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tprdWeb.ApplicationModules.ToList().ForEach(m => m.Id = 114);
                var tpmcWeb = ObjectExtensions.DeepClone(pmcWeb);
                tpmcWeb.Id = 115;
                tpmcWeb.DomainName = pmcWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tpmcWeb.ApplicationModules.ToList().ForEach(m => m.Id = 115);
                var tportalWeb = ObjectExtensions.DeepClone(portalWeb);
                tportalWeb.Id = 116;
                tportalWeb.DomainName = portalWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tportalWeb.ApplicationModules.ToList().ForEach(m => m.Id = 116);
                var tsomWeb = ObjectExtensions.DeepClone(somWeb);
                tsomWeb.Id = 117;
                tsomWeb.DomainName = somWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tsomWeb.ApplicationModules.ToList().ForEach(m => m.Id = 117);
                var tpomWeb = ObjectExtensions.DeepClone(pomWeb);
                tpomWeb.Id = 118;
                tpomWeb.DomainName = pomWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tpomWeb.ApplicationModules.ToList().ForEach(m => m.Id = 118);
                var twmsWeb = ObjectExtensions.DeepClone(wmsWeb);
                twmsWeb.Id = 119;
                twmsWeb.DomainName = wmsWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //twmsWeb.ApplicationModules.ToList().ForEach(m => m.Id = 119);
                var tjrWeb = ObjectExtensions.DeepClone(jrWeb);
                tjrWeb.Id = 120;
                tjrWeb.DomainName = jrWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tjrWeb.ApplicationModules.ToList().ForEach(m => m.Id = 120);
                var ttrainWeb = ObjectExtensions.DeepClone(trainWeb);
                ttrainWeb.Id = 121;
                ttrainWeb.DomainName = trainWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //ttrainWeb.ApplicationModules.ToList().ForEach(m => m.Id = 121);
                var tflowWeb = ObjectExtensions.DeepClone(flowWeb);
                tflowWeb.Id = 122;
                tflowWeb.DomainName = flowWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tflowWeb.ApplicationModules.ToList().ForEach(m => m.Id = 122);
                var tpayWeb = ObjectExtensions.DeepClone(payWeb);
                tpayWeb.Id = 123;
                tpayWeb.DomainName = payWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tpayWeb.ApplicationModules.ToList().ForEach(m => m.Id = 122);
                var twxWeb = ObjectExtensions.DeepClone(wxWeb);
                twxWeb.Id = 124;
                twxWeb.DomainName = wxWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //twxWeb.ApplicationModules.ToList().ForEach(m => m.Id = 124);
                var tapiWeb = ObjectExtensions.DeepClone(apiWeb);
                tapiWeb.Id = 125;
                tapiWeb.DomainName = apiWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tapiWeb.ApplicationModules.ToList().ForEach(m => m.Id = 125);
                var tcodeWeb = ObjectExtensions.DeepClone(codeWeb);
                tcodeWeb.Id = 126;
                tcodeWeb.DomainName = codeWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.TestTenantName);
                //tcodeWeb.ApplicationModules.ToList().ForEach(m => m.Id = 126);

                var testApps = new List<TenantUserApplication>()
                {
                    tssoWeb, tadminWeb, tblogWeb, tcodeWeb, tcfgWeb, tdicWeb, tappWeb, tmsgWeb, taccWeb,
                    teconWeb, tdocWeb, thrWeb, tcrmWeb, tsrmWeb, tprdWeb, tpmcWeb, tportalWeb,
                    tsomWeb, tpomWeb, twmsWeb, tjrWeb, ttrainWeb, tflowWeb, tpayWeb, twxWeb, tapiWeb
                };
                //var testAppModules = testApps.SelectMany(m => m.ApplicationModules).ToList();
                #endregion

                #region cBuy Tenant's Application
                var bssoWeb = ObjectExtensions.DeepClone(ssoWeb);
                bssoWeb.Id = 201;
                bssoWeb.DomainName = ssoWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bssoWeb.ApplicationModules.ToList().ForEach(m => m.Id = 201);
                var badminWeb = ObjectExtensions.DeepClone(adminWeb);
                badminWeb.Id = 202;
                badminWeb.DomainName = adminWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //badminWeb.ApplicationModules.ToList().ForEach(m => m.Id = 202);
                var bblogWeb = ObjectExtensions.DeepClone(blogWeb);
                bblogWeb.Id = 203;
                bblogWeb.DomainName = blogWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bblogWeb.ApplicationModules.ToList().ForEach(m => m.Id = 203);
                var bcfgWeb = ObjectExtensions.DeepClone(cfgWeb);
                bcfgWeb.Id = 204;
                bcfgWeb.DomainName = cfgWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bcfgWeb.ApplicationModules.ToList().ForEach(m => m.Id = 204);
                var bdicWeb = ObjectExtensions.DeepClone(dicWeb);
                bdicWeb.Id = 205;
                bdicWeb.DomainName = dicWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bdicWeb.ApplicationModules.ToList().ForEach(m => m.Id = 205);
                var bappWeb = ObjectExtensions.DeepClone(appWeb);
                bappWeb.Id = 206;
                bappWeb.DomainName = appWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bappWeb.ApplicationModules.ToList().ForEach(m => m.Id = 206);
                var bmsgWeb = ObjectExtensions.DeepClone(msgWeb);
                bmsgWeb.Id = 207;
                bmsgWeb.DomainName = msgWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bmsgWeb.ApplicationModules.ToList().ForEach(m => m.Id = 207);
                var baccWeb = ObjectExtensions.DeepClone(accWeb);
                baccWeb.Id = 208;
                baccWeb.DomainName = accWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //baccWeb.ApplicationModules.ToList().ForEach(m => m.Id = 208);
                var beconWeb = ObjectExtensions.DeepClone(econWeb);
                beconWeb.Id = 209;
                beconWeb.DomainName = econWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //beconWeb.ApplicationModules.ToList().ForEach(m => m.Id = 209);
                var bdocWeb = ObjectExtensions.DeepClone(docWeb);
                bdocWeb.Id = 210;
                bdocWeb.DomainName = docWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bdocWeb.ApplicationModules.ToList().ForEach(m => m.Id = 210);
                var bhrWeb = ObjectExtensions.DeepClone(hrWeb);
                bhrWeb.Id = 211;
                bhrWeb.DomainName = hrWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bhrWeb.ApplicationModules.ToList().ForEach(m => m.Id = 211);
                var bcrmWeb = ObjectExtensions.DeepClone(crmWeb);
                bcrmWeb.Id = 212;
                bcrmWeb.DomainName = crmWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bcrmWeb.ApplicationModules.ToList().ForEach(m => m.Id = 212);
                var bsrmWeb = ObjectExtensions.DeepClone(srmWeb);
                bsrmWeb.Id = 213;
                bsrmWeb.DomainName = srmWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bsrmWeb.ApplicationModules.ToList().ForEach(m => m.Id = 213);
                var bprdWeb = ObjectExtensions.DeepClone(prdWeb);
                bprdWeb.Id = 214;
                bprdWeb.DomainName = prdWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bprdWeb.ApplicationModules.ToList().ForEach(m => m.Id = 214);
                var bpmcWeb = ObjectExtensions.DeepClone(pmcWeb);
                bpmcWeb.Id = 215;
                bpmcWeb.DomainName = pmcWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bpmcWeb.ApplicationModules.ToList().ForEach(m => m.Id = 215);
                var bportalWeb = ObjectExtensions.DeepClone(portalWeb);
                bportalWeb.Id = 216;
                bportalWeb.DomainName = portalWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bportalWeb.ApplicationModules.ToList().ForEach(m => m.Id = 216);
                var bsomWeb = ObjectExtensions.DeepClone(somWeb);
                bsomWeb.Id = 217;
                bsomWeb.DomainName = somWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bsomWeb.ApplicationModules.ToList().ForEach(m => m.Id = 217);
                var bpomWeb = ObjectExtensions.DeepClone(pomWeb);
                bpomWeb.Id = 218;
                bpomWeb.DomainName = pomWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bpomWeb.ApplicationModules.ToList().ForEach(m => m.Id = 218);
                var bwmsWeb = ObjectExtensions.DeepClone(wmsWeb);
                bwmsWeb.Id = 219;
                bwmsWeb.DomainName = wmsWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bwmsWeb.ApplicationModules.ToList().ForEach(m => m.Id = 219);
                var bjrWeb = ObjectExtensions.DeepClone(jrWeb);
                bjrWeb.Id = 220;
                bjrWeb.DomainName = jrWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bjrWeb.ApplicationModules.ToList().ForEach(m => m.Id = 220);
                var btrainWeb = ObjectExtensions.DeepClone(trainWeb);
                btrainWeb.Id = 221;
                btrainWeb.DomainName = trainWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //btrainWeb.ApplicationModules.ToList().ForEach(m => m.Id = 221);
                var bflowWeb = ObjectExtensions.DeepClone(flowWeb);
                bflowWeb.Id = 222;
                bflowWeb.DomainName = flowWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bflowWeb.ApplicationModules.ToList().ForEach(m => m.Id = 222);
                var bpayWeb = ObjectExtensions.DeepClone(payWeb);
                bpayWeb.Id = 223;
                bpayWeb.DomainName = payWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bpayWeb.ApplicationModules.ToList().ForEach(m => m.Id = 222);
                var bwxWeb = ObjectExtensions.DeepClone(wxWeb);
                bwxWeb.Id = 224;
                bwxWeb.DomainName = wxWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bwxWeb.ApplicationModules.ToList().ForEach(m => m.Id = 224);
                var bapiWeb = ObjectExtensions.DeepClone(apiWeb);
                bapiWeb.Id = 225;
                bapiWeb.DomainName = apiWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bapiWeb.ApplicationModules.ToList().ForEach(m => m.Id = 225);
                var bcodeWeb = ObjectExtensions.DeepClone(codeWeb);
                bcodeWeb.Id = 226;
                bcodeWeb.DomainName = codeWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.BuyTenantName);
                //bcodeWeb.ApplicationModules.ToList().ForEach(m => m.Id = 226);

                var buyApps = new List<TenantUserApplication>()
                {
                    bssoWeb, badminWeb, bblogWeb, bcodeWeb, bcfgWeb, bdicWeb, bappWeb, bmsgWeb, baccWeb,
                    beconWeb, bdocWeb, bhrWeb, bcrmWeb, bsrmWeb, bprdWeb, bpmcWeb, bportalWeb,
                    bsomWeb, bpomWeb, bwmsWeb, bjrWeb, btrainWeb, bflowWeb, bpayWeb, bwxWeb, bapiWeb
                };
                //var buyAppModules = buyApps.SelectMany(m => m.ApplicationModules).ToList();
                #endregion

                #region cSale Tenant's Application
                var sssoWeb = ObjectExtensions.DeepClone(ssoWeb);
                sssoWeb.Id = 301;
                sssoWeb.DomainName = ssoWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //sssoWeb.ApplicationModules.ToList().ForEach(m => m.Id = 301);
                var sadminWeb = ObjectExtensions.DeepClone(adminWeb);
                sadminWeb.Id = 302;
                sadminWeb.DomainName = adminWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //sadminWeb.ApplicationModules.ToList().ForEach(m => m.Id = 302);
                var sblogWeb = ObjectExtensions.DeepClone(blogWeb);
                sblogWeb.Id = 303;
                sblogWeb.DomainName = blogWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //sblogWeb.ApplicationModules.ToList().ForEach(m => m.Id = 303);
                var scfgWeb = ObjectExtensions.DeepClone(cfgWeb);
                scfgWeb.Id = 304;
                scfgWeb.DomainName = cfgWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //scfgWeb.ApplicationModules.ToList().ForEach(m => m.Id = 304);
                var sdicWeb = ObjectExtensions.DeepClone(dicWeb);
                sdicWeb.Id = 305;
                sdicWeb.DomainName = dicWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //sdicWeb.ApplicationModules.ToList().ForEach(m => m.Id = 305);
                var sappWeb = ObjectExtensions.DeepClone(appWeb);
                sappWeb.Id = 306;
                sappWeb.DomainName = appWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //sappWeb.ApplicationModules.ToList().ForEach(m => m.Id = 306);
                var smsgWeb = ObjectExtensions.DeepClone(msgWeb);
                smsgWeb.Id = 307;
                smsgWeb.DomainName = msgWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //smsgWeb.ApplicationModules.ToList().ForEach(m => m.Id = 307);
                var saccWeb = ObjectExtensions.DeepClone(accWeb);
                saccWeb.Id = 308;
                saccWeb.DomainName = accWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //saccWeb.ApplicationModules.ToList().ForEach(m => m.Id = 308);
                var seconWeb = ObjectExtensions.DeepClone(econWeb);
                seconWeb.Id = 309;
                seconWeb.DomainName = econWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //seconWeb.ApplicationModules.ToList().ForEach(m => m.Id = 309);
                var sdocWeb = ObjectExtensions.DeepClone(docWeb);
                sdocWeb.Id = 310;
                sdocWeb.DomainName = docWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //sdocWeb.ApplicationModules.ToList().ForEach(m => m.Id = 310);
                var shrWeb = ObjectExtensions.DeepClone(hrWeb);
                shrWeb.Id = 311;
                shrWeb.DomainName = hrWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //shrWeb.ApplicationModules.ToList().ForEach(m => m.Id = 311);
                var scrmWeb = ObjectExtensions.DeepClone(crmWeb);
                scrmWeb.Id = 312;
                scrmWeb.DomainName = crmWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //scrmWeb.ApplicationModules.ToList().ForEach(m => m.Id = 312);
                var ssrmWeb = ObjectExtensions.DeepClone(srmWeb);
                ssrmWeb.Id = 313;
                ssrmWeb.DomainName = srmWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //ssrmWeb.ApplicationModules.ToList().ForEach(m => m.Id = 313);
                var sprdWeb = ObjectExtensions.DeepClone(prdWeb);
                sprdWeb.Id = 314;
                sprdWeb.DomainName = prdWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //sprdWeb.ApplicationModules.ToList().ForEach(m => m.Id = 314);
                var spmcWeb = ObjectExtensions.DeepClone(pmcWeb);
                spmcWeb.Id = 315;
                spmcWeb.DomainName = pmcWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //spmcWeb.ApplicationModules.ToList().ForEach(m => m.Id = 315);
                var sportalWeb = ObjectExtensions.DeepClone(portalWeb);
                sportalWeb.Id = 316;
                sportalWeb.DomainName = portalWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //sportalWeb.ApplicationModules.ToList().ForEach(m => m.Id = 316);
                var ssomWeb = ObjectExtensions.DeepClone(somWeb);
                ssomWeb.Id = 317;
                ssomWeb.DomainName = somWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //ssomWeb.ApplicationModules.ToList().ForEach(m => m.Id = 317);
                var spomWeb = ObjectExtensions.DeepClone(pomWeb);
                spomWeb.Id = 318;
                spomWeb.DomainName = pomWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //spomWeb.ApplicationModules.ToList().ForEach(m => m.Id = 318);
                var swmsWeb = ObjectExtensions.DeepClone(wmsWeb);
                swmsWeb.Id = 319;
                swmsWeb.DomainName = wmsWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //swmsWeb.ApplicationModules.ToList().ForEach(m => m.Id = 319);
                var sjrWeb = ObjectExtensions.DeepClone(jrWeb);
                sjrWeb.Id = 320;
                sjrWeb.DomainName = jrWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //sjrWeb.ApplicationModules.ToList().ForEach(m => m.Id = 320);
                var strainWeb = ObjectExtensions.DeepClone(trainWeb);
                strainWeb.Id = 321;
                strainWeb.DomainName = trainWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //strainWeb.ApplicationModules.ToList().ForEach(m => m.Id = 321);
                var sflowWeb = ObjectExtensions.DeepClone(flowWeb);
                sflowWeb.Id = 322;
                sflowWeb.DomainName = flowWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //sflowWeb.ApplicationModules.ToList().ForEach(m => m.Id = 322);
                var spayWeb = ObjectExtensions.DeepClone(payWeb);
                spayWeb.Id = 323;
                spayWeb.DomainName = payWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //spayWeb.ApplicationModules.ToList().ForEach(m => m.Id = 322);
                var swxWeb = ObjectExtensions.DeepClone(wxWeb);
                swxWeb.Id = 324;
                swxWeb.DomainName = wxWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //swxWeb.ApplicationModules.ToList().ForEach(m => m.Id = 324);
                var sapiWeb = ObjectExtensions.DeepClone(apiWeb);
                sapiWeb.Id = 325;
                sapiWeb.DomainName = apiWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //sapiWeb.ApplicationModules.ToList().ForEach(m => m.Id = 325);
                var scodeWeb = ObjectExtensions.DeepClone(codeWeb);
                scodeWeb.Id = 326;
                scodeWeb.DomainName = codeWebDomain?.Replace(TenantConstant.SubDomain, TenantConstant.SaleTenantName);
                //scodeWeb.ApplicationModules.ToList().ForEach(m => m.Id = 326);

                var saleApps = new List<TenantUserApplication>()
                {
                    sssoWeb, sadminWeb, sblogWeb, scodeWeb, scfgWeb, sdicWeb, sappWeb, smsgWeb, saccWeb,
                    seconWeb, sdocWeb, shrWeb, scrmWeb, ssrmWeb, sprdWeb, spmcWeb, sportalWeb,
                    ssomWeb, spomWeb, swmsWeb, sjrWeb, strainWeb, sflowWeb, spayWeb, swxWeb, sapiWeb
                };
                //var saleAppModules = saleApps.SelectMany(m => m.ApplicationModules).ToList();
                #endregion

                #region Tenant's Setting
                #region cDba Tenant's Setting
                var cdbaTenantSetting = new List<TenantUserSetting>()
                {
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.DbaTenantId,
                        PropertyAttributeId = 1,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_EmailSetting,
                        DisplayName = "是否开通邮件服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_EmailLimit,
                        CanEdit = true,
                        Index = 1
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.DbaTenantId,
                        PropertyAttributeId = 2,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_SmsSetting,
                        DisplayName = "是否开通短信服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_SmsLimit,
                        CanEdit = true,
                        Index = 2
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.DbaTenantId,
                        PropertyAttributeId = 3,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_PaymentSetting,
                        DisplayName = "是否开通支付服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_PaymentLimit,
                        CanEdit = true,
                        Index = 3
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.DbaTenantId,
                        PropertyAttributeId = 4,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_IDSetting,
                        DisplayName = "是否开通身份验证",
                        Value = "true",
                        Ext1 = TenantConstant.Default_IDLimit,
                        CanEdit = true,
                        Index = 4
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.DbaTenantId,
                        PropertyAttributeId = 5,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_CallSetting,
                        DisplayName = "是否开通电话服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_CallLimit,
                        CanEdit = true,
                        Index = 5
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.DbaTenantId,
                        PropertyAttributeId = 6,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_LogisticsSetting,
                        DisplayName = "是否开通物流服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_LogisticsLimit,
                        CanEdit = true,
                        Index = 6
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.DbaTenantId,
                        PropertyAttributeId = 7,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_WeixinSetting,
                        DisplayName = "是否开通微信服务",
                        Value = "true",
                        CanEdit = true,
                        Index = 7
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.DbaTenantId,
                        PropertyAttributeId = 8,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_ContractSetting,
                        DisplayName = "是否开通合同服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_ContractLimit,
                        CanEdit = true,
                        Index = 8
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.DbaTenantId,
                        PropertyAttributeId = 9,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_OwnDomainSetting,
                        DisplayName = "是否开通自有域名",
                        Value = "true",
                        CanEdit = true,
                        Index = 9
                    }
                };
                #endregion

                #region cTest Tenant's Setting
                var ctestTenantSetting = new List<TenantUserSetting>()
                {
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.TestTenantId,
                        PropertyAttributeId = 11,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_EmailSetting,
                        DisplayName = "是否开通邮件服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_EmailLimit,
                        CanEdit = true,
                        Index = 1
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.TestTenantId,
                        PropertyAttributeId = 12,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_SmsSetting,
                        DisplayName = "是否开通短信服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_SmsLimit,
                        CanEdit = true,
                        Index = 2
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.TestTenantId,
                        PropertyAttributeId = 13,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_PaymentSetting,
                        DisplayName = "是否开通支付服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_PaymentLimit,
                        CanEdit = true,
                        Index = 3
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.TestTenantId,
                        PropertyAttributeId = 14,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_IDSetting,
                        DisplayName = "是否开通身份验证",
                        Value = "true",
                        Ext1 = TenantConstant.Default_IDLimit,
                        CanEdit = true,
                        Index = 4
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.TestTenantId,
                        PropertyAttributeId = 15,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_CallSetting,
                        DisplayName = "是否开通电话服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_CallLimit,
                        CanEdit = true,
                        Index = 5
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.TestTenantId,
                        PropertyAttributeId = 16,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_LogisticsSetting,
                        DisplayName = "是否开通物流服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_LogisticsLimit,
                        CanEdit = true,
                        Index = 6
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.TestTenantId,
                        PropertyAttributeId = 17,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_WeixinSetting,
                        DisplayName = "是否开通微信服务",
                        Value = "true",
                        CanEdit = true,
                        Index = 7
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.TestTenantId,
                        PropertyAttributeId = 18,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_ContractSetting,
                        DisplayName = "是否开通合同服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_ContractLimit,
                        CanEdit = true,
                        Index = 8
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.TestTenantId,
                        PropertyAttributeId = 19,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_OwnDomainSetting,
                        DisplayName = "是否开通自有域名",
                        Value = "true",
                        CanEdit = true,
                        Index = 9
                    }
                };
                #endregion

                #region cBuy Tenant's Setting
                var cbuyTenantSetting = new List<TenantUserSetting>()
                {
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.BuyTenantId,
                        PropertyAttributeId = 21,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_EmailSetting,
                        DisplayName = "是否开通邮件服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_EmailLimit,
                        CanEdit = true,
                        Index = 1
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.BuyTenantId,
                        PropertyAttributeId = 22,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_SmsSetting,
                        DisplayName = "是否开通短信服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_SmsLimit,
                        CanEdit = true,
                        Index = 2
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.BuyTenantId,
                        PropertyAttributeId = 23,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_PaymentSetting,
                        DisplayName = "是否开通支付服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_PaymentLimit,
                        CanEdit = true,
                        Index = 3
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.BuyTenantId,
                        PropertyAttributeId = 24,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_IDSetting,
                        DisplayName = "是否开通身份验证",
                        Value = "true",
                        Ext1 = TenantConstant.Default_IDLimit,
                        CanEdit = true,
                        Index = 4
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.BuyTenantId,
                        PropertyAttributeId = 25,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_CallSetting,
                        DisplayName = "是否开通电话服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_CallLimit,
                        CanEdit = true,
                        Index = 5
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.BuyTenantId,
                        PropertyAttributeId = 26,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_LogisticsSetting,
                        DisplayName = "是否开通物流服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_LogisticsLimit,
                        CanEdit = true,
                        Index = 6
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.BuyTenantId,
                        PropertyAttributeId = 27,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_WeixinSetting,
                        DisplayName = "是否开通微信服务",
                        Value = "true",
                        CanEdit = true,
                        Index = 7
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.BuyTenantId,
                        PropertyAttributeId = 28,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_ContractSetting,
                        DisplayName = "是否开通合同服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_ContractLimit,
                        CanEdit = true,
                        Index = 8
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.BuyTenantId,
                        PropertyAttributeId = 29,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_OwnDomainSetting,
                        DisplayName = "是否开通自有域名",
                        Value = "true",
                        CanEdit = true,
                        Index = 9
                    }
                };
                #endregion

                #region cSale Tenant's Setting
                var csaleTenantSetting = new List<TenantUserSetting>()
                {
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.SaleTenantId,
                        PropertyAttributeId = 31,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_EmailSetting,
                        DisplayName = "是否开通邮件服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_EmailLimit,
                        CanEdit = true,
                        Index = 1
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.SaleTenantId,
                        PropertyAttributeId = 32,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_SmsSetting,
                        DisplayName = "是否开通短信服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_SmsLimit,
                        CanEdit = true,
                        Index = 2
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.SaleTenantId,
                        PropertyAttributeId = 33,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_PaymentSetting,
                        DisplayName = "是否开通支付服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_PaymentLimit,
                        CanEdit = true,
                        Index = 3
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.SaleTenantId,
                        PropertyAttributeId = 34,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_IDSetting,
                        DisplayName = "是否开通身份验证",
                        Value = "true",
                        Ext1 = TenantConstant.Default_IDLimit,
                        CanEdit = true,
                        Index = 4
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.SaleTenantId,
                        PropertyAttributeId = 35,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_CallSetting,
                        DisplayName = "是否开通电话服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_CallLimit,
                        CanEdit = true,
                        Index = 5
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.SaleTenantId,
                        PropertyAttributeId = 36,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_LogisticsSetting,
                        DisplayName = "是否开通物流服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_LogisticsLimit,
                        CanEdit = true,
                        Index = 6
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.SaleTenantId,
                        PropertyAttributeId = 37,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_WeixinSetting,
                        DisplayName = "是否开通微信服务",
                        Value = "true",
                        CanEdit = true,
                        Index = 7
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.SaleTenantId,
                        PropertyAttributeId = 38,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_ContractSetting,
                        DisplayName = "是否开通合同服务",
                        Value = "true",
                        Ext1 = TenantConstant.Default_ContractLimit,
                        CanEdit = true,
                        Index = 8
                    },
                    new TenantUserSetting()
                    {
                        TenantId = TenantConstant.SaleTenantId,
                        PropertyAttributeId = 39,
                        DataType = AttributeDataType.Bool,
                        Name = TenantConstant.PropertyName_OwnDomainSetting,
                        DisplayName = "是否开通自有域名",
                        Value = "true",
                        CanEdit = true,
                        Index = 9
                    }
                };
                #endregion

                var settins = new List<TenantUserSetting>();
                settins.AddRange(cdbaTenantSetting);
                settins.AddRange(ctestTenantSetting);
                settins.AddRange(cbuyTenantSetting);
                settins.AddRange(csaleTenantSetting);
                #endregion

                #region Tenant
                using (var transaction = context.Database.BeginTransaction())
                {
                    var tDba = TenantConstant.DbaTenantApiAccessInfo;
                    var tenantDba = new TenantUser
                    {
                        CloudType = tDba.CloudType,
                        TenantType = tDba.TenantType,
                        Version = tDba.Version,
                        TenantId = tDba.TenantId,
                        TenantName = tDba.TenantName,
                        TenantDisplayName = tDba.TenantDisplayName,
                        TenantSignature = tDba.TenantSignature,
                        PrivateEncryptKey = tDba.PrivateEncryptKey,
                        DatabasePoolId = databasePool.DatabasePoolId,
                        DatabaseType = databasePool.DatabaseType,
                        Server = tDba.Server,
                        Database = tDba.Database,
                        DatabasePasswordHash = tDba.DatabasePasswordHash,
                        StoragePoolId = storagePool.StoragePoolId,
                        StorageType = storagePool.StorageType,
                        StorageEndpoint = tDba.StorageEndpoint,
                        StorageAccessName = tDba.StorageAccessName,
                        StorageAccessKeyPasswordHash = tDba.StorageAccessKeyPasswordHash,
                        QueuePoolId = queuePool.QueuePoolId,
                        QueueType = queuePool.QueueType,
                        QueueEndpoint = tDba.QueueEndpoint,
                        QueueAccessName = tDba.QueueAccessName,
                        QueueAccessKeyPasswordHash = tDba.QueueAccessKeyPasswordHash,
                        NoSqlPoolId = noSqlPool.NoSqlPoolId,
                        NoSqlType = noSqlPool.NoSqlType,
                        NoSqlEndpoint = tDba.NoSqlEndpoint,
                        NoSqlAccessName = tDba.NoSqlAccessName,
                        NoSqlAccessKeyPasswordHash = tDba.NoSqlAccessKeyPasswordHash,
                        ServiceBusPoolId = serviceBusPool.ServiceBusPoolId,
                        ServiceBusType = serviceBusPool.ServiceBusType,
                        ServiceBusEndpoint = tDba.ServiceBusEndpoint,
                        ServiceBusAccessName = tDba.ServiceBusAccessName,
                        ServiceBusAccessKeyPasswordHash = tDba.ServiceBusAccessKeyPasswordHash,
                        VodPoolId = vodPool.VodPoolId,
                        VodType = vodPool.VodType,
                        VodEndpoint = tDba.VodEndpoint,
                        VodAccessName = tDba.VodAccessName,
                        VodAccessKeyPasswordHash = tDba.VodAccessKeyPasswordHash,
                        CodePoolId = codePool.CodePoolId,
                        CodeType = codePool.CodeType,
                        CodeEndpoint = tDba.CodeEndpoint,
                        CodeAccessName = tDba.CodeAccessName,
                        CodeAccessKeyPasswordHash = tDba.CodeAccessKeyPasswordHash,
                        ContactName = tDba.ContactName,
                        ContactEmail = tDba.ContactEmail,
                        ContactPhone = tDba.ContactPhone,
                        CanEdit = false,
                        IsDeleted = false,
                        CreatedBy = RoleConstants.AdminUserId,
                        CreatedName = RoleConstants.AdminUserName,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedBy = RoleConstants.AdminUserId,
                        ModifiedName = RoleConstants.AdminUserName,
                        ModifiedDate = DateTime.UtcNow,
                        TenantSettings = cdbaTenantSetting,
                        Applications = dbaApps
                    };

                    var tTest = TenantConstant.TestTenantApiAccessInfo;
                    var tenantDev = new TenantUser
                    {
                        CloudType = tTest.CloudType,
                        TenantType = tTest.TenantType,
                        Version = tTest.Version,
                        TenantId = tTest.TenantId,
                        TenantName = tTest.TenantName,
                        TenantDisplayName = tTest.TenantDisplayName,
                        TenantSignature = tTest.TenantSignature,
                        PrivateEncryptKey = tTest.PrivateEncryptKey,
                        DatabasePoolId = databasePool.DatabasePoolId,
                        DatabaseType = databasePool.DatabaseType,
                        Server = tTest.Server,
                        Database = tTest.Database,
                        DatabasePasswordHash = tTest.DatabasePasswordHash,
                        StoragePoolId = storagePool.StoragePoolId,
                        StorageType = storagePool.StorageType,
                        StorageEndpoint = tTest.StorageEndpoint,
                        StorageAccessName = tTest.StorageAccessName,
                        StorageAccessKeyPasswordHash = tTest.StorageAccessKeyPasswordHash,
                        QueuePoolId = queuePool.QueuePoolId,
                        QueueType = queuePool.QueueType,
                        QueueEndpoint = tTest.QueueEndpoint,
                        QueueAccessName = tTest.QueueAccessName,
                        QueueAccessKeyPasswordHash = tTest.QueueAccessKeyPasswordHash,
                        NoSqlPoolId = noSqlPool.NoSqlPoolId,
                        NoSqlType = noSqlPool.NoSqlType,
                        NoSqlEndpoint = tTest.NoSqlEndpoint,
                        NoSqlAccessName = tTest.NoSqlAccessName,
                        NoSqlAccessKeyPasswordHash = tTest.NoSqlAccessKeyPasswordHash,
                        ServiceBusPoolId = serviceBusPool.ServiceBusPoolId,
                        ServiceBusType = serviceBusPool.ServiceBusType,
                        ServiceBusEndpoint = tTest.ServiceBusEndpoint,
                        ServiceBusAccessName = tTest.ServiceBusAccessName,
                        ServiceBusAccessKeyPasswordHash = tTest.ServiceBusAccessKeyPasswordHash,
                        VodPoolId = vodPool.VodPoolId,
                        VodType = vodPool.VodType,
                        VodEndpoint = tTest.VodEndpoint,
                        VodAccessName = tTest.VodAccessName,
                        VodAccessKeyPasswordHash = tTest.VodAccessKeyPasswordHash,
                        CodePoolId = codePool.CodePoolId,
                        CodeType = codePool.CodeType,
                        CodeEndpoint = tTest.CodeEndpoint,
                        CodeAccessName = tTest.CodeAccessName,
                        CodeAccessKeyPasswordHash = tTest.CodeAccessKeyPasswordHash,
                        ContactName = tTest.ContactName,
                        ContactEmail = tTest.ContactEmail,
                        ContactPhone = tTest.ContactPhone,
                        CanEdit = false,
                        IsDeleted = false,
                        CreatedBy = RoleConstants.AdminUserId,
                        CreatedName = RoleConstants.AdminUserName,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedBy = RoleConstants.AdminUserId,
                        ModifiedName = RoleConstants.AdminUserName,
                        ModifiedDate = DateTime.UtcNow,
                        TenantSettings = ctestTenantSetting,
                        Applications = testApps
                    };

                    var tBuy = TenantConstant.BuyTenantApiAccessInfo;
                    var tenantBuy = new TenantUser
                    {
                        CloudType = tBuy.CloudType,
                        TenantType = tBuy.TenantType,
                        Version = tBuy.Version,
                        TenantId = tBuy.TenantId,
                        TenantName = tBuy.TenantName,
                        TenantDisplayName = tBuy.TenantDisplayName,
                        TenantSignature = tBuy.TenantSignature,
                        PrivateEncryptKey = tBuy.PrivateEncryptKey,
                        DatabasePoolId = databasePool.DatabasePoolId,
                        DatabaseType = databasePool.DatabaseType,
                        Server = tBuy.Server,
                        Database = tBuy.Database,
                        DatabasePasswordHash = tBuy.DatabasePasswordHash,
                        StoragePoolId = storagePool.StoragePoolId,
                        StorageType = storagePool.StorageType,
                        StorageEndpoint = tBuy.StorageEndpoint,
                        StorageAccessName = tBuy.StorageAccessName,
                        StorageAccessKeyPasswordHash = tBuy.StorageAccessKeyPasswordHash,
                        QueuePoolId = queuePool.QueuePoolId,
                        QueueType = queuePool.QueueType,
                        QueueEndpoint = tBuy.QueueEndpoint,
                        QueueAccessName = tBuy.QueueAccessName,
                        QueueAccessKeyPasswordHash = tBuy.QueueAccessKeyPasswordHash,
                        NoSqlPoolId = noSqlPool.NoSqlPoolId,
                        NoSqlType = noSqlPool.NoSqlType,
                        NoSqlEndpoint = tBuy.NoSqlEndpoint,
                        NoSqlAccessName = tBuy.NoSqlAccessName,
                        NoSqlAccessKeyPasswordHash = tBuy.NoSqlAccessKeyPasswordHash,
                        ServiceBusPoolId = serviceBusPool.ServiceBusPoolId,
                        ServiceBusType = serviceBusPool.ServiceBusType,
                        ServiceBusEndpoint = tBuy.ServiceBusEndpoint,
                        ServiceBusAccessName = tBuy.ServiceBusAccessName,
                        ServiceBusAccessKeyPasswordHash = tBuy.ServiceBusAccessKeyPasswordHash,
                        VodPoolId = vodPool.VodPoolId,
                        VodType = vodPool.VodType,
                        VodEndpoint = tBuy.VodEndpoint,
                        VodAccessName = tBuy.VodAccessName,
                        VodAccessKeyPasswordHash = tBuy.VodAccessKeyPasswordHash,
                        CodePoolId = codePool.CodePoolId,
                        CodeType = codePool.CodeType,
                        CodeEndpoint = tBuy.CodeEndpoint,
                        CodeAccessName = tBuy.CodeAccessName,
                        CodeAccessKeyPasswordHash = tBuy.CodeAccessKeyPasswordHash,
                        ContactName = tBuy.ContactName,
                        ContactEmail = tBuy.ContactEmail,
                        ContactPhone = tBuy.ContactPhone,
                        CanEdit = false,
                        IsDeleted = false,
                        CreatedBy = RoleConstants.AdminUserId,
                        CreatedName = RoleConstants.AdminUserName,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedBy = RoleConstants.AdminUserId,
                        ModifiedName = RoleConstants.AdminUserName,
                        ModifiedDate = DateTime.UtcNow,
                        TenantSettings = cbuyTenantSetting,
                        Applications = buyApps
                    };

                    var tSale = TenantConstant.SaleTenantApiAccessInfo;
                    var tenantSale = new TenantUser
                    {
                        CloudType = tSale.CloudType,
                        TenantType = tSale.TenantType,
                        Version = tSale.Version,
                        TenantId = tSale.TenantId,
                        TenantName = tSale.TenantName,
                        TenantDisplayName = tSale.TenantDisplayName,
                        TenantSignature = tSale.TenantSignature,
                        PrivateEncryptKey = tSale.PrivateEncryptKey,
                        DatabasePoolId = databasePool.DatabasePoolId,
                        DatabaseType = databasePool.DatabaseType,
                        Server = tSale.Server,
                        Database = tSale.Database,
                        DatabasePasswordHash = tSale.DatabasePasswordHash,
                        StoragePoolId = storagePool.StoragePoolId,
                        StorageType = storagePool.StorageType,
                        StorageEndpoint = tSale.StorageEndpoint,
                        StorageAccessName = tSale.StorageAccessName,
                        StorageAccessKeyPasswordHash = tSale.StorageAccessKeyPasswordHash,
                        QueuePoolId = queuePool.QueuePoolId,
                        QueueType = queuePool.QueueType,
                        QueueEndpoint = tSale.QueueEndpoint,
                        QueueAccessName = tSale.QueueAccessName,
                        QueueAccessKeyPasswordHash = tSale.QueueAccessKeyPasswordHash,
                        NoSqlPoolId = noSqlPool.NoSqlPoolId,
                        NoSqlType = noSqlPool.NoSqlType,
                        NoSqlEndpoint = tSale.NoSqlEndpoint,
                        NoSqlAccessName = tSale.NoSqlAccessName,
                        NoSqlAccessKeyPasswordHash = tSale.NoSqlAccessKeyPasswordHash,
                        ServiceBusPoolId = serviceBusPool.ServiceBusPoolId,
                        ServiceBusType = serviceBusPool.ServiceBusType,
                        ServiceBusEndpoint = tSale.ServiceBusEndpoint,
                        ServiceBusAccessName = tSale.ServiceBusAccessName,
                        ServiceBusAccessKeyPasswordHash = tSale.ServiceBusAccessKeyPasswordHash,
                        VodPoolId = vodPool.VodPoolId,
                        VodType = vodPool.VodType,
                        VodEndpoint = tSale.VodEndpoint,
                        VodAccessName = tSale.VodAccessName,
                        VodAccessKeyPasswordHash = tSale.VodAccessKeyPasswordHash,
                        CodePoolId = codePool.CodePoolId,
                        CodeType = codePool.CodeType,
                        CodeEndpoint = tSale.CodeEndpoint,
                        CodeAccessName = tSale.CodeAccessName,
                        CodeAccessKeyPasswordHash = tSale.CodeAccessKeyPasswordHash,
                        ContactName = tSale.ContactName,
                        ContactEmail = tSale.ContactEmail,
                        ContactPhone = tSale.ContactPhone,
                        CanEdit = false,
                        IsDeleted = false,
                        CreatedBy = RoleConstants.AdminUserId,
                        CreatedName = RoleConstants.AdminUserName,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedBy = RoleConstants.AdminUserId,
                        ModifiedName = RoleConstants.AdminUserName,
                        ModifiedDate = DateTime.UtcNow,
                        TenantSettings = csaleTenantSetting,
                        Applications = saleApps
                    };

                    try
                    {
                        // 在添加之前复制到新列表
                        var settinsToAdd = settins.ToList();
                        var dbaAppsToAdd = dbaApps.ToList();
                        var testAppsToAdd = testApps.ToList();
                        var buyAppsToAdd = buyApps.ToList();
                        var saleAppsToAdd = saleApps.ToList();

                        //context.TenantUsers.Add(tenantDba);
                        //context.TenantUsers.Add(tenantDev);
                        //context.TenantUsers.Add(tenantBuy);
                        //context.TenantUsers.Add(tenantSale);

                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_TenantUser] ON ", TenantConstant.DbaTenantName));
                        context.AddOrUpdate(tenantDba);
                        context.AddOrUpdate(tenantDev);
                        context.AddOrUpdate(tenantBuy);
                        context.AddOrUpdate(tenantSale);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_TenantUser] OFF ", TenantConstant.DbaTenantName));

                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_TenantUserSetting] ON ", TenantConstant.DbaTenantName));
                        context.AddOrUpdates(settinsToAdd);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_TenantUserSetting] OFF ", TenantConstant.DbaTenantName));

                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_TenantUserApplication] ON ", TenantConstant.DbaTenantName));
                        context.AddOrUpdates(dbaAppsToAdd);
                        context.AddOrUpdates(testAppsToAdd);
                        context.AddOrUpdates(buyAppsToAdd);
                        context.AddOrUpdates(saleAppsToAdd);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_TenantUserApplication] OFF ", TenantConstant.DbaTenantName));

                        //context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_TenantUserAppModule] ON ", TenantConstant.DbaTenantName));
                        //context.AddOrUpdates(dbaAppModules);
                        //context.AddOrUpdates(testAppModules);
                        //context.AddOrUpdates(buyAppModules);
                        //context.AddOrUpdates(saleAppModules);
                        //context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[tenant_TenantUserAppModule] OFF ", TenantConstant.DbaTenantName));


                        //DbLoginUserUtil.CreateTenantDbLoginUser(tenantDba, connString);//创建数据库登录用户
                        //DbLoginUserUtil.CreateTenantDbLoginUser(tenantDev, connString);//创建数据库登录用户
                        //DbLoginUserUtil.CreateTenantDbLoginUser(tenantBuy, connString);//创建数据库登录用户
                        //DbLoginUserUtil.CreateTenantDbLoginUser(tenantSale, connString);//创建数据库登录用户
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogFatal(string.Format("-------Admin Config insert Tenant Pool is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                    }
                }
                #endregion
            }
        }
    }
}
