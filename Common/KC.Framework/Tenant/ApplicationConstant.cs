using KC.Framework.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Framework.Tenant
{
    public class ApplicationConstant
    {
        #region 应用相关的常量

        public const string SsoAppName = "统一认证系统";
        public const string SsoCode = "sso";
        public const string SsoScope = "ssoapi";
        public const string SsoAppSId = "e98d7c1c-f6d8-4370-822a-c0a4377e0a59";
        public static readonly Guid SsoAppId = new Guid(SsoAppSId);

        public const string AdminAppName = "租户管理";
        public const string AdminCode = "admin";
        public const string AdminScope = "adminapi";
        public const string AdminAppSId = "98E6825F-7702-4A83-B194-A25442A25D7A";
        public static readonly Guid AdminAppId = new Guid(AdminAppSId);

        public const string BlogAppName = "博客管理";
        public const string BlogCode = "blog";
        public const string BlogScope = "blogapi";
        public const string BlogAppSId = "D0E0D0F1-9A7E-4869-8295-DA9BBC072BAB";
        public static readonly Guid BlogAppId = new Guid(BlogAppSId);

        public const string CodeAppName = "代码生成管理";
        public const string CodeCode = "code";
        public const string CodeScope = "codeapi";
        public const string CodeAppSId = "8473A5DD-6CED-4D18-81E0-A64988B72A05";
        public static readonly Guid CodeAppId = new Guid(CodeAppSId);

        public const string AppAppName = "应用管理";
        public const string AppCode = "app";
        public const string AppScope = "appapi";
        public const string AppAppSId = "1F301943-268F-4940-8BFB-900E0E6E0D35";
        public static readonly Guid AppAppId = new Guid(AppAppSId);

        public const string ConfigAppName = "配置管理";
        public const string CfgCode = "cfg";
        public const string CfgScope = "cfgapi";
        public const string ConfigAppSId = "9158A492-C6AE-4C50-87A4-7ADB8BB8D36D";
        public static readonly Guid ConfigAppId = new Guid(ConfigAppSId);

        public const string DictAppName = "字典管理";
        public const string DicCode = "dic";
        public const string DicScope = "dicapi";
        public const string DictAppSId = "21E87C50-B014-40BD-ADD7-01C64513FD3A";
        public static readonly Guid DictAppId = new Guid(DictAppSId);

        public const string MsgAppName = "消息管理";
        public const string MsgCode = "msg";
        public const string MsgScope = "msgapi";
        public const string MsgAppSId = "8600D55B-5F73-41F8-9D06-0F65710D3D80";
        public static readonly Guid MsgAppId = new Guid(MsgAppSId);

        public const string AccAppName = "账户管理";
        public const string AccCode = "acc";
        public const string AccScope = "accapi";
        public const string AccAppSId = "45672506-DDB7-4D57-AD44-BD0AB136B556";
        public static readonly Guid AccAppId = new Guid(AccAppSId);

        public const string EconAppName = "合同管理";
        public const string EconCode = "econ";
        public const string EconScope = "econapi";
        public const string EconAppSId = "18CC3E21-1B74-4BCD-BC5F-FF5E1E4E6C57";
        public static readonly Guid EconAppId = new Guid(EconAppSId);

        public const string DocAppName = "文档管理";
        public const string DocCode = "doc";
        public const string DocScope = "docapi";
        public const string DocAppSId = "7B3A90EC-8311-4113-B010-88E046D4D036";
        public static readonly Guid DocAppId = new Guid(DocAppSId);

        public const string PrjAppName = "项目管理";
        public const string PrjCode = "prj";
        public const string PrjScope = "prjapi";
        public const string PrjAppSId = "6DEFB2F1-AF54-4E3B-B38F-B80DF10AEBD0";
        public static readonly Guid PrjAppId = new Guid(PrjAppSId);
        
        public const string MbrAppName = "会员管理";
        public const string MbrCode = "mbr";
        public const string MbrScope = "mbrapi";
        public const string MbrAppSId = "0868A6DC-B7B6-4EA9-A40E-CB3617C92512";
        public static readonly Guid MbrAppId = new Guid(MbrAppSId);

        public const string HrAppName = "人事管理";
        public const string HrCode = "hr";
        public const string HrScope = "hrapi";
        public const string HrAppSId = "D96AB61D-7556-4552-98D2-B3507FBF0E55";
        public static readonly Guid HrAppId = new Guid(HrAppSId);

        public const string CrmAppName = "客户管理";
        public const string CrmCode = "crm";
        public const string CrmScope = "crmapi";
        public const string CrmAppSId = "95e9e18f-0316-4c04-bedc-a8e321431c0a";
        public static readonly Guid CrmAppId = new Guid(CrmAppSId);

        public const string SrmAppName = "供应商管理";
        public const string SrmCode = "srm";
        public const string SrmScope = "srmapi";
        public const string SrmAppSId = "379DD8C7-B603-4A45-BD23-72BAABEE374A";
        public static readonly Guid SrmAppId = new Guid(SrmAppSId);

        public const string PrdAppName = "商品管理";
        public const string PrdCode = "prd";
        public const string PrdScope = "prdapi";
        public const string PrdAppSId = "211D74B3-A43F-4167-8BAD-924AF898D570";
        public static readonly Guid PrdAppId = new Guid(PrdAppSId);

        public const string PmcAppName = "物料管理";
        public const string PmcCode = "pmc";
        public const string PmcScope = "pmcapi";
        public const string PmcAppSId = "82D4AEBE-45E8-46DA-83CA-DA3DF071D96E";
        public static readonly Guid PmcAppId = new Guid(PmcAppSId);

        public const string PortalAppName = "门户网站";
        public const string PortalCode = "portal";
        public const string PortalScope = "portalapi";
        public const string PortalAppSId = "AD401D87-0F1C-46DE-AE3E-BE4EC1C57D2C";
        public static readonly Guid PortalAppId = new Guid(PortalAppSId);

        public const string SomAppName = "销售管理";
        public const string SomCode = "som";
        public const string SomScope = "somapi";
        public const string SomAppSId = "A328591D-A64D-4379-B70E-3BDD152848D1";
        public static readonly Guid SomAppId = new Guid(SomAppSId);
        
        public const string PomAppName = "采购管理";
        public const string PomCode = "pom";
        public const string PomScope = "pomapi";
        public const string PomAppSId = "8A6CD786-582E-44BD-8F5A-E45266E0ABF7";
        public static readonly Guid PomAppId = new Guid(PomAppSId);
        
        public const string WmsAppName = "仓储管理";
        public const string WmsCode = "wms";
        public const string WmsScope = "wmsapi";
        public const string WmsAppSId = "418841DB-3A2A-4C75-9F7A-C0A14F6EE756";
        public static readonly Guid WmsAppId = new Guid(WmsAppSId);

        public const string JrAppName = "融资管理";
        public const string JrCode = "jr";
        public const string JrScope = "jrapi";
        public const string JrAppSId = "5A79FD6A-7DD9-45E6-B35B-AB24D32E89BB";
        public static readonly Guid JrAppId = new Guid(JrAppSId);

        public const string TrainAppName = "培训管理";
        public const string TrainCode = "train";
        public const string TrainScope = "trainapi";
        public const string TrainAppSId = "6F0B2682-CCE6-4D7A-AF12-A609DE44EBA6";
        public static readonly Guid TrainAppId = new Guid(TrainAppSId);

        public const string ExamAppName = "考试管理";
        public const string ExamCode = "exam";
        public const string ExamScope = "examapi";
        public const string ExamAppSId = "E9E3A02D-B28C-4B53-A26B-69DD807C89C8";
        public static readonly Guid ExamAppId = new Guid(ExamAppSId);

        public const string WorkflowAppName = "工作流管理";
        public const string WorkflowCode = "flow";
        public const string WorkflowScope = "flowapi";
        public const string WorkflowAppSId = "D6036537-1BEB-4818-AF7B-3AF493E0C930";
        public static readonly Guid WorkflowAppId = new Guid(WorkflowAppSId);
        
        public const string PayAppName = "支付管理";
        public const string PayCode = "pay";
        public const string PayScope = "payapi";
        public const string PayAppSId = "9376560F-FFDE-416F-B9E8-966B7A79012A";
        public static readonly Guid PayAppId = new Guid(PayAppSId);

        public const string WXAppName = "微信管理";
        public const string WXCode = "wx";
        public const string WXScope = "wxapi";
        public const string WXAppSId = "26755248-3986-4C7C-9980-E353BAA85AC4";
        public static readonly Guid WXAppId = new Guid(WXAppSId);
        
        public const string ResapiAppName = "资源服务";
        public const string ResCode = "res";
        public const string ResScope = "resapi";
        public const string ResapiAppSId = "3C2ADF22-0979-4392-873F-33DBEA51234D";
        public static readonly Guid ResapiAppId = new Guid(ResapiAppSId);
        
        public const string WebapiAppName = "接口管理";
        public const string WebapiCode = "api";
        public const string WebapiScope = "webapi";
        public const string WebapiAppSId = "740b4c83-aa39-410f-a35d-6d427ac33311";
        public static readonly Guid WebapiAppId = new Guid(WebapiAppSId);
        
        //应用Scope
        public const string OpenIdScope = "openid";
        public const string ProfileScope = "profile";
        

        #endregion

        public static List<ApplicationInfo> GetAllApplications()
        {
            return new List<ApplicationInfo>()
            {
                new ApplicationInfo(SsoAppId, SsoCode, SsoAppName, GlobalConfig.SSOWebDomain, SsoScope, 0),
                new ApplicationInfo(AdminAppId, AdminCode, AdminAppName, GlobalConfig.AdminWebDomain, AdminScope, 28),
                new ApplicationInfo(BlogAppId, BlogCode, BlogAppName, GlobalConfig.BlogWebDomain, BlogScope, 13),
                new ApplicationInfo(CodeAppId, CodeCode, CodeAppName, GlobalConfig.CodeWebDomain, CodeScope, 27),

                new ApplicationInfo(AppAppId, AppCode, AppAppName, GlobalConfig.AppWebDomain, AppScope, 26),
                new ApplicationInfo(ConfigAppId, CfgCode, ConfigAppName, GlobalConfig.CfgWebDomain, CfgScope, 24),
                new ApplicationInfo(DictAppId, DicCode, DictAppName, GlobalConfig.DicWebDomain, DicScope, 25),
                new ApplicationInfo(MsgAppId, MsgCode, MsgAppName, GlobalConfig.MsgWebDomain, MsgScope, 23),

                new ApplicationInfo(AccAppId, AccCode, AccAppName, GlobalConfig.AccWebDomain, AccScope, 28),
                new ApplicationInfo(EconAppId, EconCode, EconAppName, GlobalConfig.EconWebDomain, EconScope, 18),
                new ApplicationInfo(DocAppId, DocCode, DocAppName, GlobalConfig.DocWebDomain, DocScope, 20),
                new ApplicationInfo(HrAppId, HrCode, HrAppName, GlobalConfig.HrWebDomain, HrScope, 11),

                new ApplicationInfo(CrmAppId, CrmCode, CrmAppName, GlobalConfig.CrmWebDomain, CrmScope, 2),
                new ApplicationInfo(SrmAppId, SrmCode, SrmAppName, GlobalConfig.SrmWebDomain, SrmScope, 3),
                new ApplicationInfo(PrdAppId, PrdCode, PrdAppName, GlobalConfig.PrdWebDomain, PrdScope, 4),
                new ApplicationInfo(PmcAppId, PmcCode, PmcAppName, GlobalConfig.PmcWebDomain, PmcScope, 5),

                new ApplicationInfo(PortalAppId, PortalCode, PortalAppName, GlobalConfig.PortalWebDomain, PortalScope, 1),
                new ApplicationInfo(SomAppId, SomCode, SomAppName, GlobalConfig.SomWebDomain, SomScope, 6),
                new ApplicationInfo(PomAppId, PomCode, PomAppName, GlobalConfig.PomWebDomain, PomScope, 7),
                new ApplicationInfo(WmsAppId, WmsCode, WmsAppName, GlobalConfig.WmsWebDomain, WmsScope, 9),

                new ApplicationInfo(JrAppId, JrCode, JrAppName, GlobalConfig.JRWebDomain, JrScope, 10),
                new ApplicationInfo(PrjAppId, PrjCode, PrjAppName, GlobalConfig.PrjWebDomain, PrjScope, 14),
                new ApplicationInfo(MbrAppId, MbrCode, MbrAppName, GlobalConfig.MbrWebDomain, MbrScope, 15),

                new ApplicationInfo(TrainAppId, TrainCode, TrainAppName, GlobalConfig.TrainWebDomain, TrainScope, 12),
                new ApplicationInfo(ExamAppId, ExamCode, ExamAppName, GlobalConfig.ExamWebDomain, ExamScope, 16),

                new ApplicationInfo(WorkflowAppId, WorkflowCode, WorkflowAppName, GlobalConfig.WorkflowWebDomain, WorkflowScope, 21),
                new ApplicationInfo(PayAppId, PayCode, PayAppName, GlobalConfig.PayWebDomain, PayScope, 22),

                new ApplicationInfo(WXAppId, WXCode, WXAppName, GlobalConfig.WXWebDomain, WXScope, 19),
                new ApplicationInfo(ResapiAppId, ResCode, ResapiAppName, GlobalConfig.ResWebDomain, ResScope, 29),
                new ApplicationInfo(WebapiAppId, WebapiCode, WebapiAppName, GlobalConfig.ApiWebDomain, WebapiScope, 30),
            };
        }

        public const string ClientAuthorityId = "5DE763F5-3E85-4A2A-8203-31D11FE9599D";
        public const string DefaultAuthorityId = "126AC4CF-84CF-410B-8989-A4EB8397EC3F";

        public const char DefaultAuthoritySplitChar = ':';

        /// <summary>
        /// 租户存储限制：1000
        /// </summary>
        public const int StorageLimit = 1000;

        /// <summary>
        /// 数据库实例限制：20
        /// </summary>
        public const int DatabaseLimit = 20;
    }
}
