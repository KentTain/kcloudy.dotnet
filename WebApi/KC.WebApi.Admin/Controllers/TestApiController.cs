using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KC.WebApi.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestApiController : Controller
    {

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
        [Route("RemoveCacheByKey")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public JsonResult RemoveCacheByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return Json(new { success = true, message = "缓存键值为空，无法清除缓存。" }); 

            Service.CacheUtil.RemoveCache(key);

            return Json(new { success = true, message = "清除缓存成功" });
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

        private string _serverPath;
        protected string ServerPath
        {
            get
            {
                return Directory.GetCurrentDirectory().Replace("\\", "/");

                //var exePath = Path.GetDirectoryName(System.Reflection
                //   .Assembly.GetExecutingAssembly().CodeBase);
                //Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
                //_serverPath = appPathMatcher.Match(exePath).Value;
                //return _serverPath;
            }
            set { _serverPath = value; }
        }
    }
}
