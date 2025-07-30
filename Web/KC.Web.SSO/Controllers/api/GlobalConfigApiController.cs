using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Service;
using KC.Service.Util;
using KC.Service.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace KC.Web.SSO.Controllers
{
    /// <summary>
    /// 获取系统的全局设置
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class GlobalConfigApiController : Controller
    {
        private const string cacheKey = "KC.Web.SSO.Controllers.GlobalConfigApiController.GetData";
        /// <summary>
        /// 接口是否有效
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// 获取全局配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetData")]
        public GlobalConfigData GetData()
        {
           return new GlobalConfigData()
           {
               //EncryptKey = GlobalConfig.EncryptKey,
               BlobStorage = GlobalConfig.BlobStorage,
               AdminEmails = GlobalConfig.AdminEmails,
               TempFilePath = GlobalConfig.TempFilePath,
               Applications = GlobalConfig.Applications,

               SSOWebDomain = GlobalConfig.SSOWebDomain,
               AdminWebDomain = GlobalConfig.AdminWebDomain,
               BlogWebDomain = GlobalConfig.BlogWebDomain,

               CfgWebDomain = GlobalConfig.CfgWebDomain,
               DicWebDomain = GlobalConfig.DicWebDomain,
               AppWebDomain = GlobalConfig.AppWebDomain,
               MsgWebDomain = GlobalConfig.MsgWebDomain,

               AccWebDomain = GlobalConfig.AccWebDomain,
               EconWebDomain = GlobalConfig.EconWebDomain,
               DocWebDomain = GlobalConfig.DocWebDomain,

               CrmWebDomain = GlobalConfig.CrmWebDomain,
               SrmWebDomain = GlobalConfig.SrmWebDomain,
               PrdWebDomain = GlobalConfig.PrdWebDomain,
               PmcWebDomain = GlobalConfig.PmcWebDomain,

               PortalWebDomain = GlobalConfig.PortalWebDomain,
               SomWebDomain = GlobalConfig.SomWebDomain,
               PomWebDomain = GlobalConfig.PomWebDomain,
               WmsWebDomain = GlobalConfig.WmsWebDomain,

               JRWebDomain = GlobalConfig.JRWebDomain,
               TrainWebDomain = GlobalConfig.TrainWebDomain,

               FlowWebDomain = GlobalConfig.WorkflowWebDomain,
               PayWebDomain = GlobalConfig.PayWebDomain,

               WXWebDomain = GlobalConfig.WXWebDomain,
               ApiWebDomain = GlobalConfig.ApiWebDomain,
               ResWebDomain = GlobalConfig.ResWebDomain,

               ApiServerUrl = GlobalConfig.ApiServerUrl,

               UploadConfig = GlobalConfig.UploadConfig,
           };
        }
    }
}
