using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using KC.Framework.Tenant;
using KC.Framework.Extension;
using KC.Web.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using KC.Service.Account;

namespace KC.Web.Account.Controllers
{
    public class AccountController : AccountBaseController
    {
        public AccountController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<AccountController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [Authorize]
        public IActionResult Sigin()
        {
            return Redirect("/");
        }

        #region 退出：SignOut

        public new async Task SignOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(Web.Constants.OpenIdConnectConstants.AuthScheme);
                await HttpContext.SignOutAsync(Web.Constants.OpenIdConnectConstants.ChallengeScheme);

                RemoveCurrentUserCache();
            }
        }

        #endregion

        #region 修改密码:ChangePassword  &  修改邮箱/手机：ChangeMailPhone
        /// <summary>
        /// 主页修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!base.User.Identity.IsAuthenticated)
                return Json(new { success = false, message = "用户未登陆，不能修改密码." });

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "数据有误,请重新输入." });

            using (var scope = ServiceProvider.CreateScope())
            {
                var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
                var result = await accountService.ChangePasswordAsync(CurrentUserId, model.OldPassword, model.NewPassword);

                return result.Succeeded
                    ? Json(new { success = true, message = "密码修改成功." })
                    : Json(
                        new
                        {
                            success = false,
                            message =
                                result.Errors.ToCommaSeparatedStringByFilter(m => m.Code + "：" + m.Description)
                        }) ;
            }
        }
        /// <summary>
        /// 主页修改邮箱及手机号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ChangeMailPhone(ChangeEmailPhoneViewModel model)
        {
            if (!base.User.Identity.IsAuthenticated)
                return Json(new { success = false, message = "用户未登陆，不能修改邮箱及手机号." });

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "数据有误,请重新输入." });

            using (var scope = ServiceProvider.CreateScope())
            {
                var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
                var result = await accountService.ChangeMailPhoneAsync(CurrentUserId, model.Email, model.Phone);

                return result.Succeeded
                    ? Json(new { success = true, message = "邮箱及手机号修改成功." })
                    : Json(new { success = false, message = result.Errors.ToCommaSeparatedStringByFilter(m => m.Code + "：" + m.Description) });
            }
        }
        #endregion
    }
}
