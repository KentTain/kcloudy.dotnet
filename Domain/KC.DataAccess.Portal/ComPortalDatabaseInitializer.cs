using KC.Database.EFRepository;
using KC.Database.Extension;
using KC.Enums.Portal;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Model.Portal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace KC.DataAccess.Portal
{
    /// <summary>
    /// 数据库初始化操作类
    /// </summary>
    public class ComPortalDatabaseInitializer : MultiTenantSqlServerDatabaseInitializer<ComPortalContext>
    {
        public ComPortalDatabaseInitializer()
            : base()
        { }

        public override ComPortalContext Create(Tenant tenant)
        {
            if (tenant == null)
                throw new System.ArgumentNullException("Tenant is null", "tenant");
            if (string.IsNullOrEmpty(tenant.TenantName))
                throw new System.ArgumentException("tenantName is null or empty", "tenantName");
            if (string.IsNullOrEmpty(tenant.ConnectionString))
                throw new System.ArgumentException("connectionString is null or empty", "connectionString");

            var options = GetCachedDbContextOptions(tenant.TenantName, tenant.ConnectionString, tenant.DatabaseType);
            return new ComPortalContext(options, tenant);
        }

        protected override string GetTargetMigration()
        {
            return DataInitial.DBSqlInitializer.GetPreMigrationVersion();
        }

        public void SeedData(Tenant tenant)
        {
            using (var context = Create(tenant))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        #region 首页模板
                        var pageId_1 = Guid.Parse("D1176C38-F5A4-4AFF-AAC7-9306A910426E");

                        //卡片式：左右结构
                        var columnId_1_1 = Guid.Parse("7F19B73D-755A-45E0-A11A-1E27865C4194");
                        var item_1_1_1 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_1,
                            Id = Guid.Parse("EC39AE82-0635-411C-A77F-8BCAEEA35D48"),
                            Title = "数字化咨询",
                            SubTitle = "基于国际主流的方法理论体系及丰富的团队经验，为企业提供数字化规划等咨询服务。",
                            IsImage = false,
                            ImageOrIConCls = "fa fa-university",
                            IsShow = true,
                            CanEdit = false,
                            Index = 1,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var item_1_1_2 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_1,
                            Id = Guid.Parse("DD2F45E2-FFA5-4A72-8C82-D5C77CEE2312"),
                            Title = "软件及服务",
                            SubTitle = "基于鑫亚科技SaaS一体化开放平台，为用户提供标准化软件产品、定制化开发服务。",
                            IsImage = false,
                            ImageOrIConCls = "fa fa-meetup",
                            IsShow = true,
                            CanEdit = false,
                            Index = 2,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var item_1_1_3 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_1,
                            Id = Guid.Parse("75517BEA-5B3F-4BCC-8C8A-1A8F943AAEB5"),
                            Title = "整体解决方案",
                            SubTitle = "通过软硬件集成与定制化开发，为企业用户提供数字化专项整体解决方案实施服务。",
                            IsImage = false,
                            ImageOrIConCls = "fa fa-soundcloud",
                            IsShow = true,
                            CanEdit = false,
                            Index = 3,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var column_1_1 = new WebSiteColumn()
                        {
                            WebSitePageId = pageId_1,
                            Id = columnId_1_1,
                            Title = "",
                            SubTitle = "",
                            Type = WebSiteColumnType.Card,
                            ItemType = WebSiteItemType.LeftRight,
                            RowCount = 1,
                            ColumnCount = 3,
                            Index = 1,
                            IsShow = true,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                            WebSiteItems = new List<WebSiteItem>() { item_1_1_1, item_1_1_2 , item_1_1_3 }
                        };
                        //图片式
                        var columnId_1_2 = Guid.Parse("83A3EBBF-7DE5-4D57-9023-780F6155B064");
                        var item_1_2_1 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_2,
                            Id = Guid.Parse("03A88C0E-F32B-43CB-8CFC-9E5FCFD0B151"),
                            Title = "",
                            SubTitle = "",
                            IsImage = true,
                            ImageOrIConCls = "/images/demo/home/home-2.png",
                            IsShow = true,
                            CanEdit = false,
                            Index = 1,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var column_1_2 = new WebSiteColumn()
                        {
                            WebSitePageId = pageId_1,
                            Id = columnId_1_2,
                            Title = "软件产品与服务",
                            SubTitle = "鑫亚科技PaaS/SaaS一体化柔性平台，基于云原生、零代码、微服务、容器、移动应用等新一代IT技术，<br> 为用户提供标准化软件产品、定制化开发服务、零代码搭建平台。",
                            Type = WebSiteColumnType.Image,
                            ItemType = WebSiteItemType.TopBottom,
                            RowCount = 1,
                            ColumnCount = 1,
                            Index = 2,
                            IsShow = true,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                            WebSiteItems = new List<WebSiteItem>() { item_1_2_1 }
                        };
                        //卡片式：上下结构
                        var columnId_1_3 = Guid.Parse("EA50CC3F-E8A9-498E-8636-2DB3BF37E6C2");
                        var item_1_3_1 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_3,
                            Id = Guid.Parse("5A136940-AB59-45C9-80FF-A3152CA319E0"),
                            Title = "标准化产品<br /><span>Standardization</span>",
                            SubTitle = "鑫亚科技云平台已数字化封装30余项标准化业务应用，涵盖权限验证、流程管理、文档管理、配置采购、项目管理、行政后勤、财务管理等领域。",
                            IsImage = false,
                            ImageOrIConCls = "fa fa-gg-circle",
                            IsShow = true,
                            CanEdit = false,
                            Index = 1,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var item_1_3_2 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_3,
                            Id = Guid.Parse("20A3C061-3F3E-44A5-8CCD-B5BD967BB815"),
                            Title = "定制化服务<br /><span>Customization</span>",
                            SubTitle = "鑫亚科技Saas一体化平台，具备敏捷开发、便捷拓展的特点，围绕企业个性化需求，同时为企业用户提供私有化系统定制开发服务。",
                            IsImage = false,
                            ImageOrIConCls = "fa fa-tachometer",
                            IsShow = true,
                            CanEdit = false,
                            Index = 2,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var item_1_3_3 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_3,
                            Id = Guid.Parse("8098435B-7482-4A72-8D61-14D307A5E0D1"),
                            Title = "本地化部署<br /><span>Localization</span>",
                            SubTitle = "鑫亚科技为用户提供云端及本地化部署服务，无需具备IT技能，可根据业务用户需求轻松创建个性化、轻量化应用系统，实现随时随地数字化、移动化管理。",
                            IsImage = false,
                            ImageOrIConCls = "fa fa-life-ring",
                            IsShow = true,
                            CanEdit = false,
                            Index = 2,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var column_1_3 = new WebSiteColumn()
                        {
                            WebSitePageId = pageId_1,
                            Id = columnId_1_3,
                            Title = "三大类用户服务",
                            SubTitle = "基于鑫亚科技柔性云平台，为用户提供标准化产品、定制化服务和零代码搭建。",
                            Type = WebSiteColumnType.Card,
                            ItemType = WebSiteItemType.TopBottom,
                            RowCount = 1,
                            ColumnCount = 3,
                            Index = 3,
                            IsShow = true,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                            WebSiteItems = new List<WebSiteItem>() { item_1_3_1, item_1_3_2, item_1_3_3 }
                        };
                        //产品式：上下结构
                        var columnId_1_4 = Guid.Parse("AB9751A0-EEB5-4361-9C61-1358206459C3");
                        var item_1_4_1 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_4,
                            Id = Guid.Parse("9991DD6C-BD19-4445-B7C4-636FD5354853"),
                            Title = "产品名称",
                            SubTitle = "价格：面议",
                            IsImage = true,
                            ImageOrIConCls = "/images/image_add_small.png",
                            IsShow = true,
                            CanEdit = false,
                            Index = 1,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var item_1_4_2 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_4,
                            Id = Guid.Parse("2781A649-9D8E-45F4-8077-583815EF19FA"),
                            Title = "产品名称",
                            SubTitle = "价格：面议",
                            IsImage = true,
                            ImageOrIConCls = "/images/image_add_small.png",
                            IsShow = true,
                            CanEdit = false,
                            Index = 2,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var item_1_4_3 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_4,
                            Id = Guid.Parse("F4CF5BE1-8CB0-4C91-AF50-386E3F71CDD4"),
                            Title = "产品名称",
                            SubTitle = "价格：面议",
                            IsImage = true,
                            ImageOrIConCls = "/images/image_add_small.png",
                            IsShow = true,
                            CanEdit = false,
                            Index = 3,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var item_1_4_4 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_4,
                            Id = Guid.Parse("ECFCC649-E395-4346-8D49-2ABAEAD72852"),
                            Title = "产品名称",
                            SubTitle = "价格：面议",
                            IsImage = true,
                            ImageOrIConCls = "/images/image_add_small.png",
                            IsShow = true,
                            CanEdit = false,
                            Index = 4,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var item_1_4_5 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_4,
                            Id = Guid.Parse("4297DA8E-7669-4AD5-9F86-11B6FFEA9BC3"),
                            Title = "产品名称",
                            SubTitle = "价格：面议",
                            IsImage = true,
                            ImageOrIConCls = "/images/image_add_small.png",
                            IsShow = true,
                            CanEdit = false,
                            Index = 5,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var item_1_4_6 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_4,
                            Id = Guid.Parse("6979348B-1A3D-4CF4-AFEF-10D0B1DF41F8"),
                            Title = "产品名称",
                            SubTitle = "价格：面议",
                            IsImage = true,
                            ImageOrIConCls = "/images/image_add_small.png",
                            IsShow = true,
                            CanEdit = false,
                            Index = 6,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var column_1_4 = new WebSiteColumn()
                        {
                            WebSitePageId = pageId_1,
                            Id = columnId_1_4,
                            Title = "产品展示",
                            SubTitle = "基于鑫亚科技柔性云平台，为用户提供标准化产品、定制化服务和零代码搭建。",
                            Type = WebSiteColumnType.Product,
                            ItemType = WebSiteItemType.TopBottom,
                            RowCount = 2,
                            ColumnCount = 6,
                            Index = 4,
                            IsShow = true,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                            WebSiteItems = new List<WebSiteItem>() { item_1_4_1, item_1_4_2, item_1_4_3, item_1_4_4, item_1_4_5, item_1_4_6 }
                        };
                        //轮播式：左右结构
                        var columnId_1_5 = Guid.Parse("83BF5E61-74E1-4A3E-AD75-8C13C839711D");
                        var item_1_5_1 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_5,
                            Id = Guid.Parse("F43E8221-73AD-4615-ABDD-13476BCBC8A6"),
                            Title = "",
                            SubTitle = "",
                            IsImage = true,
                            ImageOrIConCls = "/images/image_add_small.png",
                            IsShow = true,
                            CanEdit = false,
                            Index = 1,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var item_1_5_2 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_5,
                            Id = Guid.Parse("0E1B4DD1-BDF2-4A56-92E7-AD773054D284"),
                            Title = "",
                            SubTitle = "",
                            IsImage = true,
                            ImageOrIConCls = "/images/image_add_small.png",
                            IsShow = true,
                            CanEdit = false,
                            Index = 2,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var item_1_5_3 = new WebSiteItem()
                        {
                            WebSiteColumnId = columnId_1_5,
                            Id = Guid.Parse("C0720988-8702-4197-9B61-303C2F8CE486"),
                            Title = "",
                            SubTitle = "",
                            IsImage = true,
                            ImageOrIConCls = "/images/image_add_small.png",
                            IsShow = true,
                            CanEdit = false,
                            Index = 3,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                        };
                        var column_1_5 = new WebSiteColumn()
                        {
                            WebSitePageId = pageId_1,
                            Id = columnId_1_5,
                            Title = "合作企业",
                            SubTitle = "FINANCIAL INSTITUTION",
                            Type = WebSiteColumnType.Slide,
                            ItemType = WebSiteItemType.LeftRight,
                            RowCount = 1,
                            ColumnCount = 6,
                            Index = 5,
                            IsShow = true,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                            WebSiteItems = new List<WebSiteItem>() { item_1_5_1, item_1_5_2, item_1_5_3 }
                        };

                        var page_1 = new WebSitePage
                        {
                            Id = pageId_1,
                            Name = "首页",
                            SkinCode = "skn2021010100001",
                            SkinName = "企业门户网设计",
                            Type = WebSitePageType.System,
                            Status = WorkflowBusStatus.Approved,
                            IsEnable = true,
                            MainColor = "#2277DD",
                            SecondaryColor = "#33DDFF",
                            Link = "/Home/Index",
                            LinkIsOpenNewPage = false,
                            UseMainSlide = false,
                            MainSlide = null,
                            CanEdit = true,
                            Index = 1,
                            CreatedBy = RoleConstants.AdminUserId,
                            CreatedName = RoleConstants.AdminUserName,
                            ModifiedBy = RoleConstants.AdminUserId,
                            ModifiedName = RoleConstants.AdminUserName,
                            WebSiteColumns = new List<WebSiteColumn>() { column_1_1, column_1_2, column_1_3, column_1_4 }
                        };
                        //首页
                        context.AddOrUpdate(page_1);
                        //卡片式：左右结构
                        context.AddOrUpdate(column_1_1);
                        context.AddOrUpdate(item_1_1_1);
                        context.AddOrUpdate(item_1_1_2);
                        context.AddOrUpdate(item_1_1_3);
                        //图片式
                        context.AddOrUpdate(column_1_2);
                        context.AddOrUpdate(item_1_2_1);
                        //卡片式：上下结构
                        context.AddOrUpdate(column_1_3);
                        context.AddOrUpdate(item_1_3_1);
                        context.AddOrUpdate(item_1_3_2);
                        context.AddOrUpdate(item_1_3_3);
                        //产品式：上下结构
                        context.AddOrUpdate(column_1_4);
                        context.AddOrUpdate(item_1_4_1);
                        context.AddOrUpdate(item_1_4_2);
                        context.AddOrUpdate(item_1_4_3);
                        context.AddOrUpdate(item_1_4_4);
                        context.AddOrUpdate(item_1_4_5);
                        context.AddOrUpdate(item_1_4_6);
                        //轮播式：左右结构
                        context.AddOrUpdate(column_1_5);
                        context.AddOrUpdate(item_1_5_1);
                        context.AddOrUpdate(item_1_5_2);
                        context.AddOrUpdate(item_1_5_3);
                        #endregion

                        #region 产品列表
                        var pageId_2 = Guid.Parse("26D440F6-C284-446A-9722-AF0DEB9B8194");
                        var page_2 = new WebSitePage
                        {
                            Id = pageId_2,
                            Name = "产品信息",
                            SkinCode = "skn2021010100001",
                            SkinName = "企业门户网设计",
                            Type = WebSitePageType.Link,
                            Status = WorkflowBusStatus.Approved,
                            IsEnable = true,
                            Link = "/Home/ProductList",
                            LinkIsOpenNewPage = false,
                            MainColor = "#2277DD",
                            SecondaryColor = "#33DDFF",
                            UseMainSlide = false,
                            CanEdit = false,
                            Index = 2,
                        };
                        //产品列表
                        context.AddOrUpdate(page_2);
                        #endregion

                        #region 采购列表
                        var pageId_3 = Guid.Parse("B6FA61B6-EF23-4B93-A0C1-4AF0561EBFDE");
                        var page_3 = new WebSitePage
                        {
                            Id = pageId_3,
                            Name = "采购信息",
                            SkinCode = "skn2021010100001",
                            SkinName = "企业门户网设计",
                            Type = WebSitePageType.Link,
                            Status = WorkflowBusStatus.Approved,
                            IsEnable = true,
                            Link = "/Home/RequirementList",
                            LinkIsOpenNewPage = false,
                            MainColor = "#2277DD",
                            SecondaryColor = "#33DDFF",
                            UseMainSlide = false,
                            CanEdit = false,
                            Index = 3,
                        };
                        //采购列表
                        context.AddOrUpdate(page_3);
                        #endregion

                        #region 企业动态
                        var pageId_4 = Guid.Parse("D0E732DC-DB69-4C47-81FE-0AA6CC6C8F80");
                        var page_4 = new WebSitePage
                        {
                            Id = pageId_4,
                            Name = "公司动态",
                            SkinCode = "skn2021010100001",
                            SkinName = "企业门户网设计",
                            Type = WebSitePageType.Link,
                            Status = WorkflowBusStatus.Approved,
                            IsEnable = true,
                            Link = "/Home/NewsList",
                            LinkIsOpenNewPage = false,
                            MainColor = "#2277DD",
                            SecondaryColor = "#33DDFF",
                            UseMainSlide = false,
                            CanEdit = false,
                            Index = 4,
                        };
                        //企业信息
                        context.AddOrUpdate(page_4);
                        #endregion

                        #region 企业信息
                        var pageId_5 = Guid.Parse("586EBD8A-6D31-49AD-8AC3-9F0674F405C2");
                        var page_5 = new WebSitePage
                        {
                            Id = pageId_5,
                            Name = "公司信息",
                            SkinCode = "skn2021010100001",
                            SkinName = "企业门户网设计",
                            Type = WebSitePageType.Link,
                            Status = WorkflowBusStatus.Approved,
                            IsEnable = true,
                            Link = "/Home/CompanyInfo",
                            LinkIsOpenNewPage = false,
                            MainColor = "#2277DD",
                            SecondaryColor = "#33DDFF",
                            UseMainSlide = false,
                            CanEdit = false,
                            Index = 5,
                        };
                        //企业信息
                        context.AddOrUpdate(page_5);
                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogFatal(string.Format("-------insert Tenant WebSitePage is failed. \r\nMessage: {0}. \r\nStackTrace: {1}", ex.Message, ex.StackTrace));
                    }
                }
            }
        }
    }
}
