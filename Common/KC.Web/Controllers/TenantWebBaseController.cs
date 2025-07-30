using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Threading.Tasks;

using KC.Framework.Base;
using KC.Framework.Tenant;

namespace KC.Web.Controllers
{
    public abstract class TenantWebBaseController : WebBaseController
    {
        protected TenantWebBaseController(
            Tenant tenant, 
            IServiceProvider serviceProvider,
            ILogger logger)
            : base(serviceProvider, logger)
        {
            Tenant = tenant;
        }

        #region Tenant Info

        private string _tenantName;
        /// <summary>
        /// 租户编码：UR2015060600001
        /// </summary>
        public string TenantName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this._tenantName))
                    return this._tenantName.ToLower();

                return null;
            }

            set
            {
                this._tenantName = value;
                ViewBag.TenantName = value;
            }
        }

        private string _tenantNickName;
        /// <summary>
        ///存的是NickName或者TenantName,看域名
        /// </summary>
        public string TenantNickName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this._tenantNickName))
                    return this._tenantNickName.ToLower();
                return null;
            }

            set
            {
                this._tenantNickName = value;
                ViewBag.TenantNickName = value;
            }
        }

        private string _tenantDisplayName;
        /// <summary>
        /// 租户的中文名称
        /// </summary>
        public string TenantDisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this._tenantDisplayName))
                    return this._tenantDisplayName.ToLower();

                return null;
            }

            set
            {
                this._tenantDisplayName = value;
                ViewBag.TenantDisplayName = value;
            }
        }

        private Tenant _tenant;
        public Tenant Tenant
        {
            get
            {
                if (this._tenant != null)
                    return this._tenant;

                return null;
            }

            set
            {
                this._tenant = value;
                this.TenantName = value?.TenantName;
                this.TenantNickName = value?.NickName;
                this.TenantDisplayName = value?.TenantDisplayName;

                ViewBag.Tenant = value;
            }
        }

        /// <summary>
        /// 是否为Dba管理系统
        /// </summary>
        public bool IsTenantAdmin
        {
            get
            {
                return TenantName.Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase);
            }
        }
        /// <summary>
        /// 是否为当前租户的内部用户
        /// </summary>
        public bool IsTenantUser
        {
            get { return TenantName.Equals(CurrentUserTenantName, StringComparison.OrdinalIgnoreCase); }
        }
        /// <summary>
        /// 是否为企业用户
        /// </summary>
        public bool IsEnterprise
        {
            get
            {
                return (Tenant.TenantType & TenantType.Enterprise) == TenantType.Enterprise;
            }
        }


        #endregion Tenant Info

        #region Upload
        
        /// <summary>
        /// UEdit config
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Upload(string parm)
        {
            if ("config".Equals(parm))
            {
                var domainName = GetDocumentApiUrl(Tenant.TenantName);
                var configContent = GetUEditorConfig();
                configContent = configContent.Replace("{ApiDomain}", domainName).Replace("{imageMaxSize}", (GlobalConfig.UploadConfig.ImageMaxSize * 1024 * 1024).ToString()).Replace("{fileMaxSize}", (GlobalConfig.UploadConfig.FileMaxSize * 1024 * 1024).ToString());
                return Content(configContent.Replace("\r\n", ""));
                //return Content(SerializeHelper.ToJson(JObject.Parse(configContent)));
            }

            return Json(new
            {
                success = false,
                message = "请选择上传文件",
                //ueditor所需参数
                state = "FALSE",
                error = "请选择上传文件",
                url = string.Empty,
                id = string.Empty,
                title = string.Empty,
                original = string.Empty
            });
        }
        /// <summary>
        /// 获取文档接口地址：http://[tenantName].docapi.kcloudy.com/api/ </br>
        ///     本地测试接口地址：http://[tenantName].localhost:2006/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        private string GetDocumentApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.DocWebDomain))
                return null;

            return GlobalConfig.DocWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("doc.", "docapi.").Replace(":2005", ":2006").Replace(":2015", ":2016") + "api/";
        }
        private string GetUEditorConfig()
        {
            var url = GlobalConfig.ResWebDomain + "js/ueditor/config.json";

            try
            {
                string result = Common.HttpHelper.HttpWebRequestHelper.DoGet(url, null);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("从【" + url + "】获取UEditor的配置文件出错，错误消息为：" + ex.Message);
                var configPath = ServerPath + "/wwwroot/js/ueditor/config.json";
                try
                {
                    return System.IO.File.ReadAllText(configPath);
                }
                catch (Exception ex1)
                {
                    Logger.LogError("从【" + configPath + "】获取UEditor的配置文件出错，错误消息为：" + ex1.Message);
                    return string.Empty;
                }
            }
        }

        #endregion Upload

    }
}