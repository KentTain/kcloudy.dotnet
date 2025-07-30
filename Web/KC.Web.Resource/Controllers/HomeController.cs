using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KC.Web.Resource.Models;
using Microsoft.Extensions.Logging;
using KC.Common.FileHelper;

namespace KC.Web.Resource.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        public HomeController(
            ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        private string _cookieName = "Captcha";
        public IActionResult Captcha()
        {
            ViewData["Message"] = "Input the Captcha to verify the your request.";
            ViewBag.Captcha = _cookieName;
            return View();
        }

        public IActionResult ServiceAddress()
        {
            return View();
        }

        public IActionResult Demo()
        {
            return View();
        }

        /// <summary>
        /// 返回验证码图片
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCaptchaImage()
        {
            var model = await CaptchaFactory.Instance.CreateAsync();
            Response.Cookies.Append(_cookieName, model.Answer);
            return File(model.Image, model.ContentType);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        #region 辅助方法

        public IActionResult CheckHealth()
        {
            return Ok("OK");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
