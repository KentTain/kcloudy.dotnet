using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace KC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class WebApiBaseController : Controller
    {
        protected string ServiceName = "KC.Web.Controllers.WebApiBaseController";
        protected readonly Tenant Tenant;
        protected readonly IServiceProvider ServiceProvider;
        protected readonly ILogger Logger;

        public WebApiBaseController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger logger)
        {
            Tenant = tenant;
            ServiceProvider = serviceProvider;
            Logger = logger;
        }

        #region 文件地址
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

        private string _tempDir;
        //上传使用
        protected string TempDir
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_tempDir))
                {
                    _tempDir = ServerPath + "/TempDir";
                    if (!Directory.Exists(_tempDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(_tempDir);
                        }
                        catch (SecurityException)
                        {

                        }
                    }
                }
                return _tempDir;
            }
            set { _tempDir = value; }
        }
        #endregion

        #region Get JsonResult
        /// <summary>
        /// 返回boolean类型的快捷方式
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="message">失败后的消息</param>
        /// <returns></returns>
        protected JsonResult ThrowErrorJsonMessage(bool success, string message)
        {
            return Json(new { success = success, message = message });
        }

        /// <summary>
        /// 获取ServiceResult<T>对象的JsonResult
        /// </summary>
        /// <typeparam name="T">所需返回的对象</typeparam>
        /// <param name="func">Lamdba表达式</param>
        /// <returns>ServiceResult<T></returns>
        protected JsonResult GetServiceJsonResult<T>(Func<T> func)
        {
            var result = ServiceWrapper.Invoke(
                ServiceName,
                func.Method.Name,
                func,
                Logger);
            return Json(result);
        }

        /// <summary>
        /// 获取ServiceResult<T>对象的JsonResult（异步方法）
        /// </summary>
        /// <typeparam name="T">所需返回的对象</typeparam>
        /// <param name="func">Lamdba表达式</param>
        /// <returns>ServiceResult<T></returns>
        protected async Task<JsonResult> GetServiceJsonResultAsync<T>(Func<Task<T>> func)
        {
            var result = await ServiceWrapper.InvokeAsync<T>(
                ServiceName,
                func.Method.Name,
                func,
                Logger);
            return Json(result);
        }
        /// <summary>
        /// 获取ServiceResult<T>对象
        /// </summary>
        /// <typeparam name="T">所需返回的对象</typeparam>
        /// <param name="func">Lamdba表达式</param>
        /// <returns>ServiceResult<T></returns>
        protected ServiceResult<T> GetServiceResult<T>(Func<T> func)
        {
            return ServiceWrapper.Invoke(
                ServiceName,
                func.Method.Name,
                func,
                Logger);
        }
        /// <summary>
        /// 获取ServiceResult<T>对象（异步方法）
        /// </summary>
        /// <typeparam name="T">所需返回的对象</typeparam>
        /// <param name="func">Lamdba表达式</param>
        /// <returns>ServiceResult<T></returns>
        protected async Task<ServiceResult<T>> GetServiceResultAsync<T>(Func<Task<T>> func)
        {
            var result = await ServiceWrapper.InvokeAsync<T>(
                ServiceName,
                func.Method.Name,
                func,
                Logger);
            return result;
        }

        #endregion

        protected string GetAllModelErrorMessages()
        {
            var errors = new StringBuilder();
            ModelState.Remove("Id");
            ModelState.Remove("IsEditMode");
            ModelState.Remove("TreeCode");
            ModelState.Remove("Leaf");
            ModelState.Remove("Level");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedName");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedBy");
            ModelState.Remove("ModifiedName");
            ModelState.Remove("ModifiedDate");
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var error = string.Empty;
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        foreach (var errorInfo in state.Errors)
                        {
                            var message = !errorInfo.ErrorMessage.IsNullOrEmpty()
                                ? errorInfo.ErrorMessage
                                : errorInfo.Exception != null
                                    ? errorInfo.Exception.ToString()
                                    : string.Empty;

                            if (!message.IsNullOrEmpty())
                                error += message + "；";
                        }
                    }

                    if (!error.IsNullOrEmpty())
                        errors.Append(string.Format("对象属性【{0}】，验证错误：{1}</br>", key, error));
                }
            }

            return errors.ToString();
        }
    }
}
