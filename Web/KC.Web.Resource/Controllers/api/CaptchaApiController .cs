using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using KC.Common.FileHelper;
using KC.Framework.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace KC.Web.Resource.Controllers
{
    /// <summary>
    /// 获取验证码
    /// </summary>
    [Route("api/[controller]")]
    public class CaptchaApiController : Controller
    {
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
        /// 获取验证码，前端使用范例：
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetCaptcha")]
        public async Task<CaptchaInfo> GetCaptcha()
        {
            var model = await CaptchaFactory.Instance.CreateAsync();
            return model;
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
        ///     code = 101 : message = 验证失败，参数为空
        ///     code = 102 : message = 验证码失效，一个验证码只能调用一次接口
        ///     code = 103 : message = 验证码失效，验证码解密失败
        ///     code = 104 : message = 验证码失效，验证码解密失败
        ///     code = 105 : message = 验证失败，验证码不匹配
        /// </returns>
        [HttpPost, Route("VerifyCaptcha")]
        public async Task<VerifyResponse> Verify([FromBody] VerifyRequest model)
        {
            var response = await CaptchaFactory.Instance.VerifyAsync(model);

            return response;
        }
    }
}
