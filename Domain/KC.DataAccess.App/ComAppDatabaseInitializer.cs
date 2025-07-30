using KC.Database.EFRepository;
using KC.Database.Extension;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Model.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KC.DataAccess.App
{
    /// <summary>
    /// 数据库初始化操作类
    /// </summary>
    public class ComAppDatabaseInitializer : MultiTenantSqlServerDatabaseInitializer<ComAppContext>
    {
        public ComAppDatabaseInitializer()
            : base()
        { }

        public override ComAppContext Create(Tenant tenant)
        {
            if (tenant == null)
                throw new System.ArgumentNullException("Tenant is null", "tenant");
            if (string.IsNullOrEmpty(tenant.TenantName))
                throw new System.ArgumentException("tenantName is null or empty", "tenantName");
            if (string.IsNullOrEmpty(tenant.ConnectionString))
                throw new System.ArgumentException("connectionString is null or empty", "connectionString");

            var options = GetCachedDbContextOptions(tenant.TenantName, tenant.ConnectionString, tenant.DatabaseType);
            return new ComAppContext(options, tenant);
        }

        protected override string GetTargetMigration()
        {
            return DataInitial.DBSqlInitializer.GetPreMigrationVersion();
        }

        public void SeedData(List<Tenant> tenants)
        {
            tenants.ForEach(m => SeedData(m));
        }

        public void SeedData(Tenant tenant)
        {
            #region Application
            var ssoWebDomain = GlobalConfig.SSOWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var adminWebDomain = GlobalConfig.AdminWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var blogWebDomain = GlobalConfig.BlogWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var codeWebDomain = GlobalConfig.CodeWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var cfgWebDomain = GlobalConfig.CfgWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var dicWebDomain = GlobalConfig.DicWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var appWebDomain = GlobalConfig.AppWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var msgWebDomain = GlobalConfig.MsgWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var accWebDomain = GlobalConfig.AccWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var econWebDomain = GlobalConfig.EconWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var docWebDomain = GlobalConfig.DocWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var hrWebDomain = GlobalConfig.HrWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var crmWebDomain = GlobalConfig.CrmWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var srmWebDomain = GlobalConfig.SrmWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var prdWebDomain = GlobalConfig.PrdWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var pmcWebDomain = GlobalConfig.PmcWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var portalWebDomain = GlobalConfig.PortalWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var somWebDomain = GlobalConfig.SomWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var pomWebDomain = GlobalConfig.PomWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var wmsWebDomain = GlobalConfig.WmsWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var jrWebDomain = GlobalConfig.JRWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var trainWebDomain = GlobalConfig.TrainWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var flowWebDomain = GlobalConfig.WorkflowWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var payWebDomain = GlobalConfig.PayWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var wxWebDomain = GlobalConfig.WXWebDomain?.TrimEnd('/').Replace("http://", "").Replace("https://", "");
            var apiWebDomain = GlobalConfig.ApiWebDomain?.Replace("http://", "").Replace("https://", "") + "api";

            var ssoWeb = new Application()
            {
                Index = 1,
                ApplicationId = ApplicationConstant.SsoAppId,
                ApplicationCode = ApplicationConstant.SsoCode,
                ApplicationName = ApplicationConstant.SsoAppName,
                WebSiteName = ApplicationConstant.SsoScope,
                SmallIcon = "fa fa-users",
                BigIcon = "fa fa-users fa-4",
                DomainName = ssoWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Account",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var adminWeb = new Application()
            {
                Index = 2,
                ApplicationId = ApplicationConstant.AdminAppId,
                ApplicationCode = ApplicationConstant.AdminCode,
                ApplicationName = ApplicationConstant.AdminAppName,
                WebSiteName = ApplicationConstant.AdminScope,
                SmallIcon = "fa fa-cogs",
                BigIcon = "fa fa-cogs fa-4",
                DomainName = adminWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Admin",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var blogWeb = new Application()
            {
                Index = 3,
                ApplicationId = ApplicationConstant.BlogAppId,
                ApplicationCode = ApplicationConstant.BlogCode,
                ApplicationName = ApplicationConstant.BlogAppName,
                WebSiteName = ApplicationConstant.BlogScope,
                SmallIcon = "fa fa-rss",
                BigIcon = "fa fa-rss fa-4",
                DomainName = blogWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Blog",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var codeWeb = new Application()
            {
                Index = 4,
                ApplicationId = ApplicationConstant.CodeAppId,
                ApplicationCode = ApplicationConstant.CodeCode,
                ApplicationName = ApplicationConstant.CodeAppName,
                WebSiteName = ApplicationConstant.CodeScope,
                SmallIcon = "fa fa-code",
                BigIcon = "fa fa-code fa-4",
                DomainName = codeWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.CodeGenerate",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var cfgWeb = new Application()
            {
                Index = 5,
                ApplicationId = ApplicationConstant.ConfigAppId,
                ApplicationCode = ApplicationConstant.CfgCode,
                ApplicationName = ApplicationConstant.ConfigAppName,
                WebSiteName = ApplicationConstant.CfgScope,
                SmallIcon = "fa fa-cog",
                BigIcon = "fa fa-cog fa-4",
                DomainName = cfgWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Config",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var dicWeb = new Application()
            {
                Index = 6,
                ApplicationId = ApplicationConstant.DictAppId,
                ApplicationCode = ApplicationConstant.DicCode,
                ApplicationName = ApplicationConstant.DictAppName,
                WebSiteName = ApplicationConstant.DicScope,
                SmallIcon = "fa fa-book",
                BigIcon = "fa fa-book fa-4",
                DomainName = dicWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Dict",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var appWeb = new Application()
            {
                Index = 7,
                ApplicationId = ApplicationConstant.AppAppId,
                ApplicationCode = ApplicationConstant.AppCode,
                ApplicationName = ApplicationConstant.AppAppName,
                WebSiteName = ApplicationConstant.AppScope,
                SmallIcon = "fa fa-mobile",
                BigIcon = "fa fa-mobile fa-4",
                DomainName = appWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.App",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var msgWeb = new Application()
            {
                Index = 8,
                ApplicationId = ApplicationConstant.MsgAppId,
                ApplicationCode = ApplicationConstant.MsgCode,
                ApplicationName = ApplicationConstant.MsgAppName,
                WebSiteName = ApplicationConstant.MsgScope,
                SmallIcon = "fa fa-envelope",
                BigIcon = "fa fa-envelope fa-4",
                DomainName = msgWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Message",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var accWeb = new Application()
            {
                Index = 9,
                ApplicationId = ApplicationConstant.AccAppId,
                ApplicationCode = ApplicationConstant.AccCode,
                ApplicationName = ApplicationConstant.AccAppName,
                WebSiteName = ApplicationConstant.AccScope,
                SmallIcon = "fa fa-users",
                BigIcon = "fa fa-users fa-4",
                DomainName = accWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Account",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var econWeb = new Application()
            {
                Index = 10,
                ApplicationId = ApplicationConstant.EconAppId,
                ApplicationCode = ApplicationConstant.EconCode,
                ApplicationName = ApplicationConstant.EconAppName,
                WebSiteName = ApplicationConstant.EconScope,
                SmallIcon = "fa fa-file-text",
                BigIcon = "fa fa-file-text fa-4",
                DomainName = econWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Contract",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var docWeb = new Application()
            {
                Index = 11,
                ApplicationId = ApplicationConstant.DocAppId,
                ApplicationCode = ApplicationConstant.DocCode,
                ApplicationName = ApplicationConstant.DocAppName,
                WebSiteName = ApplicationConstant.DocScope,
                SmallIcon = "fa fa-file-o",
                BigIcon = "fa fa-file-o fa-4",
                DomainName = docWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Doc",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var hrWeb = new Application()
            {
                Index = 12,
                ApplicationId = ApplicationConstant.HrAppId,
                ApplicationCode = ApplicationConstant.HrCode,
                ApplicationName = ApplicationConstant.HrAppName,
                WebSiteName = ApplicationConstant.HrScope,
                SmallIcon = "fa fa-male",
                BigIcon = "fa fa-male fa-4",
                DomainName = hrWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.HumanResource",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var crmWeb = new Application()
            {
                Index = 13,
                ApplicationId = ApplicationConstant.CrmAppId,
                ApplicationCode = ApplicationConstant.CrmCode,
                ApplicationName = ApplicationConstant.CrmAppName,
                WebSiteName = ApplicationConstant.CrmScope,
                SmallIcon = "fa fa-id-card",
                BigIcon = "fa fa-id-card fa-4",
                DomainName = crmWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Customer",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var srmWeb = new Application()
            {
                Index = 14,
                ApplicationId = ApplicationConstant.SrmAppId,
                ApplicationCode = ApplicationConstant.SrmCode,
                ApplicationName = ApplicationConstant.SrmAppName,
                WebSiteName = ApplicationConstant.SrmScope,
                SmallIcon = "fa fa-truck",
                BigIcon = "fa fa-truck fa-4",
                DomainName = srmWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Supplier",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var prdWeb = new Application()
            {
                Index = 15,
                ApplicationId = ApplicationConstant.PrdAppId,
                ApplicationCode = ApplicationConstant.PrdCode,
                ApplicationName = ApplicationConstant.PrdAppName,
                WebSiteName = ApplicationConstant.PrdScope,
                SmallIcon = "fa fa-archive",
                BigIcon = "fa fa-archive fa-4",
                DomainName = prdWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Offering",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var pmcWeb = new Application()
            {
                Index = 16,
                ApplicationId = ApplicationConstant.PmcAppId,
                ApplicationCode = ApplicationConstant.PmcCode,
                ApplicationName = ApplicationConstant.PmcAppName,
                WebSiteName = ApplicationConstant.PmcScope,
                SmallIcon = "fa fa-cubes",
                BigIcon = "fa fa-cubes fa-4",
                DomainName = pmcWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Material",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var portalWeb = new Application()
            {
                Index = 17,
                ApplicationId = ApplicationConstant.PortalAppId,
                ApplicationCode = ApplicationConstant.PortalCode,
                ApplicationName = ApplicationConstant.PortalAppName,
                WebSiteName = ApplicationConstant.PortalScope,
                SmallIcon = "fa fa-wordpress",
                BigIcon = "fa fa-wordpress fa-4",
                DomainName = portalWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Portal",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var somWeb = new Application()
            {
                Index = 18,
                ApplicationId = ApplicationConstant.SomAppId,
                ApplicationCode = ApplicationConstant.SomCode,
                ApplicationName = ApplicationConstant.SomAppName,
                WebSiteName = ApplicationConstant.SomScope,
                SmallIcon = "fa fa-area-chart",
                BigIcon = "fa fa-area-chart fa-4",
                DomainName = somWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Sales",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var pomWeb = new Application()
            {
                Index = 19,
                ApplicationId = ApplicationConstant.PomAppId,
                ApplicationCode = ApplicationConstant.PomCode,
                ApplicationName = ApplicationConstant.PomAppName,
                WebSiteName = ApplicationConstant.PomScope,
                SmallIcon = "fa fa-bar-chart",
                BigIcon = "fa fa-bar-chart fa-4",
                DomainName = pomWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Purchase",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var wmsWeb = new Application()
            {
                Index = 20,
                ApplicationId = ApplicationConstant.WmsAppId,
                ApplicationCode = ApplicationConstant.WmsCode,
                ApplicationName = ApplicationConstant.WmsAppName,
                WebSiteName = ApplicationConstant.WmsScope,
                SmallIcon = "fa fa-building",
                BigIcon = "fa fa-building fa-4",
                DomainName = wmsWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Warehouse",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var jrWeb = new Application()
            {
                Index = 21,
                ApplicationId = ApplicationConstant.JrAppId,
                ApplicationCode = ApplicationConstant.JrCode,
                ApplicationName = ApplicationConstant.JrAppName,
                WebSiteName = ApplicationConstant.JrScope,
                SmallIcon = "fa fa-money",
                BigIcon = "fa fa-money fa-4",
                DomainName = jrWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Finance",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var trainWeb = new Application()
            {
                Index = 22,
                ApplicationId = ApplicationConstant.TrainAppId,
                ApplicationCode = ApplicationConstant.TrainCode,
                ApplicationName = ApplicationConstant.TrainAppName,
                WebSiteName = ApplicationConstant.TrainScope,
                SmallIcon = "fa fa-tachometer",
                BigIcon = "fa fa-tachometer fa-4",
                DomainName = trainWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Training",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var flowWeb = new Application()
            {
                Index = 23,
                ApplicationId = ApplicationConstant.WorkflowAppId,
                ApplicationCode = ApplicationConstant.WorkflowCode,
                ApplicationName = ApplicationConstant.WorkflowAppName,
                WebSiteName = ApplicationConstant.WorkflowScope,
                SmallIcon = "fa fa-code-fork",
                BigIcon = "fa fa-code-fork fa-4",
                DomainName = flowWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Workflow",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var payWeb = new Application()
            {
                Index = 24,
                ApplicationId = ApplicationConstant.PayAppId,
                ApplicationCode = ApplicationConstant.PayCode,
                ApplicationName = ApplicationConstant.PayAppName,
                WebSiteName = ApplicationConstant.PayScope,
                SmallIcon = "fa fa-paypal",
                BigIcon = "fa fa-paypal fa-4",
                DomainName = payWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Pay",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var wxWeb = new Application()
            {
                Index = 25,
                ApplicationId = ApplicationConstant.WXAppId,
                ApplicationCode = ApplicationConstant.WXCode,
                ApplicationName = ApplicationConstant.WXAppName,
                WebSiteName = ApplicationConstant.WXScope,
                SmallIcon = "fa fa-weixin",
                BigIcon = "fa fa-weixin fa-4",
                DomainName = wxWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                AssemblyName = "KC.DataAccess.Wechat",
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };
            var apiWeb = new Application()
            {
                Index = 26,
                ApplicationId = ApplicationConstant.WebapiAppId,
                ApplicationCode = ApplicationConstant.WebapiCode,
                ApplicationName = ApplicationConstant.WebapiAppName,
                WebSiteName = ApplicationConstant.WebapiScope,
                SmallIcon = "fa fa-folder",
                BigIcon = "fa fa-folder fa-4",
                DomainName = apiWebDomain?.Replace(TenantConstant.SubDomain, tenant.TenantName),
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };

            var apps = new List<Application>()
                {
                    ssoWeb, adminWeb, blogWeb, codeWeb, cfgWeb, dicWeb, appWeb, msgWeb, accWeb,
                    econWeb, docWeb, hrWeb, crmWeb, srmWeb, prdWeb, pmcWeb, portalWeb,
                    somWeb, pomWeb, wmsWeb, jrWeb, trainWeb, flowWeb, payWeb, wxWeb, apiWeb
            };
            #endregion

            #region AppSetting
            var appSettingProps = new List<AppSettingProperty>()
            {
                new AppSettingProperty()
                {
                    AppSettingId = 1,
                    AppSettingCode = "ASC2020010100001",
                    PropertyAttributeId = 1,
                    Name = "DatabaseType",
                    DisplayName = "数据库类型",
                    DataType = AttributeDataType.List,
                    Value = "SqlServer",
                    Ext1 = "[{'id':'MySql','name':'MySql'},{'id':'SqlServer','name':'Sql Server'},{'id':'PostgreSql','name':'PostgreSql'}]",
                    CanEdit = false,
                    IsRequire = true,
                    IsDeleted = false,
                    Index = 1,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                },
                new AppSettingProperty()
                {
                    AppSettingId = 1,
                    AppSettingCode = "ASC2020010100001",
                    PropertyAttributeId = 2,
                    Name = "DevLanguage",
                    DisplayName = "开发语言",
                    DataType = AttributeDataType.List,
                    Value = "Java",
                    Ext1 = "[{'id':'Java','name':'Java'},{'id':'NetCore','name':'NetCore'}]",
                    CanEdit = false,
                    IsRequire = true,
                    IsDeleted = false,
                    Index = 2,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                },
                new AppSettingProperty()
                {
                    AppSettingId = 1,
                    AppSettingCode = "ASC2020010100001",
                    PropertyAttributeId = 3,
                    Name = "DevLanguageVersion",
                    DisplayName = "开发语言版本",
                    DataType = AttributeDataType.List,
                    Value = "Java1.8",
                    Ext1 = "[{'id':'Java1.8','name':'Java1.8','type':'Java'},{'id':'Java11','name':'Java11','type':'Java'},{'id':'NetCore5.0','name':'NetCore5.0','type':'NetCore'},{'id':'NetCore6.0','name':'NetCore6.0','type':'NetCore'}]",
                    CanEdit = false,
                    IsRequire = true,
                    IsDeleted = false,
                    Index = 3,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                },
                new AppSettingProperty()
                {
                    AppSettingId = 1,
                    AppSettingCode = "ASC2020010100001",
                    PropertyAttributeId = 4,
                    Name = "NameSpace",
                    DisplayName = "命名空间/包名",
                    DataType = AttributeDataType.String,
                    Value = "auto",
                    CanEdit = false,
                    IsRequire = true,
                    IsDeleted = false,
                    Index = 4,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                },
                new AppSettingProperty()
                {
                    AppSettingId = 1,
                    AppSettingCode = "ASC2020010100001",
                    PropertyAttributeId = 5,
                    Name = "BuildType",
                    DisplayName = "构建方式",
                    DataType = AttributeDataType.List,
                    Value = "Maven",
                    Ext1 = "[{'id':'Maven','name':'Maven','type':'Java'},{'id':'Gradle','name':'Gradle','type':'Java'},{'id':'Nuget','name':'Nuget','type':'NetCore'}]",
                    CanEdit = false,
                    IsRequire = true,
                    IsDeleted = false,
                    Index = 5,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                },
            };
            var appSetting = new AppSetting()
            {
                PropertyId = 1,
                Code = "ASC2020010100001",
                Name = "AppBackend",
                DisplayName = "工程配置信息",
                ApplicationId = ApplicationConstant.CodeAppId,
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
                PropertyAttributeList = appSettingProps,
            };

            var settins = new List<AppSetting>();
            settins.Add(appSetting);

            var settinProps = new List<AppSettingProperty>();
            settinProps.AddRange(appSettingProps);
            #endregion

            using (var context = Create(tenant))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[app_Application] ON ", tenant.TenantName));
                        context.AddOrUpdates(apps);
                        //context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[app_Application] OFF ", tenant.TenantName));

                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[app_AppSetting] ON ", tenant.TenantName));
                        context.AddOrUpdate(appSetting);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[app_AppSetting] OFF ", tenant.TenantName));

                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[app_AppSettingProperty] ON ", tenant.TenantName));
                        context.AddOrUpdates(settinProps);
                        context.Database.ExecuteSqlRaw(string.Format("SET IDENTITY_INSERT [{0}].[app_AppSettingProperty] OFF ", tenant.TenantName));

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogFatal(string.Format("-------Admin Config insert Tenant Pool is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                    }
                }
            }

        }
    }
}
