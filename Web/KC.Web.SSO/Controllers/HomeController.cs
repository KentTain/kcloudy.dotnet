using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KC.Web.SSO.Models;
using Microsoft.AspNetCore.Identity;
using KC.Model.Account;
using Microsoft.Extensions.Logging;
using KC.Common.FileHelper;
using KC.IdentityServer4.Services;

namespace KC.Web.SSO.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;
        public HomeController(
            IIdentityServerInteractionService interaction,
            IServiceProvider serviceProvider,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger)
        {
            _interaction = interaction;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ServiceAddress()
        {
            return View();
        }

        public IActionResult Introduce()
        {
            return View();
        }

        public IActionResult Demo()
        {
            return View();
        }

        #region Captcha
        private const string CaptchaSessionKey = "kcloudy-captchacode";
        /// <summary>
        /// 获取Captcha图片，并将Code保存在Cookies中，Cookies的key为基类中的常量：CaptchaSessionKey<br/>
        /// 前端使用实例： <img src="/Home/GetCaptchaImage" /> 
        /// 后端以Post方式，验证方法如下：VerifyCaptcha（VerifyRequest model）
        /// 
        /// 使用范例：http://resource.kcloudy.com/Home/Captcha
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetCaptchaImage()
        {
            var model = await CaptchaFactory.Instance.CreateAsync();
            ViewBag.Captcha = CaptchaSessionKey;
            Response.Cookies.Append(CaptchaSessionKey, model.Answer);
            return File(model.Image, model.ContentType);
        }

        /// <summary>
        /// 验证码的校验，验证结果对象为：VerifyResponse
        /// </summary>
        /// <param name="model">
        /// VerifyRequest
        ///     Answer：答案
        ///     Captcha：Cookie中对应Captcha的值，即GetCaptchaImage时，返回前端Cookies中包含包含的Captcha的值，前端获取该Cookie值的代码：
        ///     function getCookie() {
        ///           var strCookie = document.cookie;
        ///           var arrCookie = strCookie.split("; ");
        ///           for (var i = 0; i < arrCookie.length; i++) {
        ///               var arr = arrCookie[i].split("=");
        ///               if (arr[0] === '@ViewBag.Captcha') return arr[1];
        ///           }
        ///          return "";
        ///      }
        /// </param>
        /// <returns>
        ///     VerifyResponse
        ///     code = 100 : message = 验证成功
        ///     code = 101 : message = 验证失败,参数为空
        ///     code = 102 : message = 验证码失效,一个验证码只能调用一次接口
        ///     code = 103 : message = 验证码失效,验证码解密失败
        ///     code = 104 : message = 验证码失效,验证码解密失败
        ///     code = 105 : message = 验证失败，验证码不匹配
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> VerifyCaptcha([FromBody] VerifyRequest model)
        {
            //VerifyResponse
            var response = await CaptchaFactory.Instance.VerifyAsync(model);

            return Json(response);
        }
        #endregion

        #region 辅助方法

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel() { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

            }

            return View("Error", vm);
        }

        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        #endregion
    }
}
