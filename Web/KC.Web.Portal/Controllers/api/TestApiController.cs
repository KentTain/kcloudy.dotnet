using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KC.Web.Portal.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TestApiController : Web.Controllers.WebApiBaseController
    {
        public TestApiController(
            Tenant tenant, IServiceProvider serviceProvider,
            ILogger<TestApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 获取当前Tenant编码及名称
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTenant")]
        public IEnumerable<string> GetTenant()
        {
            return new string[] { Tenant.TenantName, Tenant.TenantDisplayName };
        }

        /// <summary>
        /// 获取当前环境类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetEnv")]
        [AllowAnonymous]
        public ActionResult<string> GetEnv()
        {
            return @KC.Framework.Base.GlobalConfig.SystemType.ToString();
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("DownLoadLogFile")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult DownLoadLogFile(string name)
        {
            var url = ServerPath + "/logs/" + name + ".log";
            return File(url, "text/plain");
        }
    }
}
