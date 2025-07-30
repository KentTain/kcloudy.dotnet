using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using KC.IdentityModel;
using KC.IdentityServer4.Events;
using KC.IdentityServer4.Extensions;
using KC.IdentityServer4.Services;
using KC.IdentityServer4.Stores;
using KC.Common.FileHelper;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Account;
using KC.Service;
using KC.Service.Account;
using KC.Service.Base;
using KC.Service.Constants;
using KC.Service.DTO.Account;
using KC.Service.WebApiService.Business;
using KC.Web.Base;
using KC.Web.Constants;
using KC.Web.SSO.AccountViewModels;
using KC.Web.SSO.Models;
using KC.Web.SSO.Models.AccountViewModels;
using KC.Web.SSO.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace KC.Web.SSO.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController : Controller
    {
        #region Services
        private IAccountService AccountService => _serviceProvider.GetService<IAccountService>(); 
        private ISysManageService SysManageService => _serviceProvider.GetService<ISysManageService>();

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;

        private readonly IEventService _events;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IClientStore _clientStore;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        private readonly Tenant _tenant;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IIdentityServerInteractionService interaction,
            IEmailSender emailSender,
            IClientStore clientStore,
            IEventService events,
            IAuthenticationSchemeProvider schemeProvider,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AccountController> logger,
            Tenant tenant = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _emailSender = emailSender;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _schemeProvider = schemeProvider;
            _events = events;
            _clientStore = clientStore;
            _httpContextAccessor = httpContextAccessor;

            _tenant = tenant;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        #region Login

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.isAuth = this.User.IsAuthenticated();
            ViewBag.userDisplayName = CurrentUserDisplayName;
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);
            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return await ExternalLogin(vm.ExternalLoginScheme, returnUrl);
            }

            var requestDomain = Request.Scheme + "://" + Request.Host;
            var tenantLoginDomain = Web.Constants.OpenIdConnectConstants.GetAuthUrlByConfig(vm.TenantName);
            if (!requestDomain.Equals(tenantLoginDomain, StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(tenantLoginDomain + "/Account/Login?returnUrl=" + System.Net.WebUtility.UrlEncode(returnUrl) );
            }

            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserName))
            {
                ModelState.AddModelError("UserName", "请输入用户名");
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "请输入密码");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberLogin, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        try
                        {
                            //设置用户登陆日志
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var userAgent = _httpContextAccessor?.HttpContext.Request.Headers["User-Agent"];
                                var ipAddress = _httpContextAccessor?.HttpContext.Connection.RemoteIpAddress.ToString();
                                //配置Nginx后，设置nginx配置文件中的location：proxy_set_header X-Real-IP $remote addr;
                                if (ipAddress.IsNullOrEmpty())
                                    ipAddress = _httpContextAccessor?.HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();

                                //根据用户邮箱或者用户名及手机，获取用户信息
                                User user;
                                if (model.UserName.Contains("@"))
                                {
                                    user = await _userManager.FindByEmailAsync(model.UserName);
                                }
                                else
                                {
                                    user = await _userManager.FindByNameAsync(model.UserName);
                                }
                                var log = new UserLoginLogDTO()
                                {
                                    OperatorId = user.Id,
                                    Operator = user.DisplayName,
                                    Remark = string.Format("用户【IP：{0}】使用浏览器【{1}】登录系统", ipAddress, userAgent),
                                    IPAddress = ipAddress,
                                    BrowserInfo = userAgent
                                };
                                //备注：在使用IdentityDbContext上下文保存其他自定义对象时，不能直接使用Repository进行保存
                                //需要使用继承IdentityDbContext的类ComAccountContext进行对象操作，见方法：AddUserLoginLogAsync
                                var sysManageService = scope.ServiceProvider.GetService<ISysManageService>();
                                await sysManageService.AddUserLoginLogAsync(log);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("记录用户登陆日志出错，错误消息：" + ex.Message + "，错误详情：" + ex.StackTrace);
                        }

                        // make sure the returnUrl is still valid, and if so redirect back to authorize endpoint or a local page
                        // the IsLocalUrl check is only necessary if you want to support additional local pages, otherwise IsValidReturnUrl is more strict
                        if (_interaction.IsValidReturnUrl(model.ReturnUrl) 
                            || Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        _logger.LogInformation(string.Format("---User logged in. ReturnUrl: {0}/t/n RedirectUrl: {1}", 
                            model.ReturnUrl, model.RedirectUrl));
                        //if (!string.IsNullOrEmpty(model.RedirectUrl))
                        //    return Redirect(model.RedirectUrl);

                        return Redirect(model.ReturnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(nameof(LoginWith2fa), new { model.ReturnUrl, model.RememberLogin });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToAction(nameof(Lockout));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(new EventId(1), ex, ex.Message);

                    var vm = new LoginViewModel()
                    {
                        TenantName = _tenant?.TenantName,
                        ReturnUrl = model.ReturnUrl,
                        RedirectUrl = model.RedirectUrl,
                    };
                    return View(vm);
                }
                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel
            {
                TenantName = _tenant?.TenantName,
                ReturnUrl = returnUrl,
                RememberMe = rememberMe
            };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;
            var model = new LoginWithRecoveryCodeViewModel()
            {
                TenantName = _tenant?.TenantName,
                ReturnUrl = returnUrl,
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        #endregion

        #region Logout

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            // delete local authentication cookie
            await _signInManager.SignOutAsync();

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AllowAnonymous]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        #endregion

        #region Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewBag.isAuth = this.User.IsAuthenticated();
            ViewBag.userDisplayName = CurrentUserDisplayName;
            var model = new RegisterViewModel()
            {
                UserType = Framework.Base.UserType.Company,
                TenantName = _tenant?.TenantName,
                ReturnUrl = returnUrl,
            };
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                var errinfo = new StringBuilder();
                //邮箱为必填项
                model.Email = !string.IsNullOrWhiteSpace(model.Email) ? model.Email : model.PhoneNumber + "@139.com";

                #region 验证短信验证码
                var phone = model.PhoneNumber;
                var code = model.Code;
                var sessionCode = ConnectionKeyConstant.PHONE_CODE_RERGISTER + phone;
                var content = CacheUtil.GetCache<SmsContent>(sessionCode);
                var currentDate = DateTime.UtcNow;
                if (content == null)
                    throw new ArgumentException("未发送相关手机的验证码，请重新发送", "Code");
                if (currentDate > content.ExpiredDateTime)
                {
                    CacheUtil.RemoveCache(sessionCode);
                    throw new ArgumentException("手机的验证码已经过期，请重新发送", "Code");
                }

                if (!content.PhoneNumber.Equals(phone, StringComparison.OrdinalIgnoreCase)
                    || !content.PhoneCode.Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    CacheUtil.RemoveCache(sessionCode);
                    throw new ArgumentException("手机验证码不正确，请重新获取手机验证码", "Code");
                }
                #endregion

                ModelState.Remove("DisplayName");
                ModelState.Remove("TenantName");
                if (ModelState.IsValid)
                {
                    var data = new UserRegisterDTO
                    {
                        UserType = model.UserType,
                        UserName = model.PhoneNumber,
                        DisplayName = model.DisplayName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Password = model.Password,
                        CompanyName = model.TenantName,
                        Recommended = model.Recommended
                    };

                    var result = await AccountService.UserRegister(data);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");
                        await _signInManager.PasswordSignInAsync(data.UserName, data.Password, isPersistent: false, false);
                        _logger.LogInformation("User sign in a new account with password.");
                        return true;
                    }

                    foreach (var error in result.Errors)
                    {
                        errinfo.AppendFormat("{0}\\n", error.Description);
                    }
                }
                else
                {
                    foreach (var s in ModelState.Values)
                    {
                        foreach (var p in s.Errors)
                        {
                            errinfo.AppendFormat("{0}\\n", p.ErrorMessage);
                        }
                    }
                }

                if (!errinfo.IsNullOrEmpty())
                    throw new Exception(errinfo.ToString());

                // If we got this far, something failed, redisplay form
                return true;
            });
        }
        #endregion

        #region Verfiy
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerfiyPhone(string phone)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var accountService = scope.ServiceProvider.GetService<IAccountService>();
                    return await accountService.ExistUserPhoneAsync(phone);
                }
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerfiyEmail(string email)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var accountService = scope.ServiceProvider.GetService<IAccountService>();
                    return await accountService.ExistUserEmailAsync(email);
                }
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerfiyPhoneCode(string phone, string code)
        {
            return GetServiceJsonResult(() =>
            {
                var sessionCode = ConnectionKeyConstant.PHONE_CODE_RERGISTER + phone;
                var content = CacheUtil.GetCache<SmsContent>(sessionCode);
                var currentDate = DateTime.UtcNow;
                if (content == null)
                    throw new ArgumentException("未发送相关手机的验证码，请重新发送");
                if (currentDate > content.ExpiredDateTime)
                {
                    CacheUtil.RemoveCache(sessionCode);
                    throw new ArgumentException("手机的验证码已经过期，请重新发送");
                }

                if (!content.PhoneNumber.Equals(phone, StringComparison.OrdinalIgnoreCase)
                    || !content.PhoneCode.Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    CacheUtil.RemoveCache(sessionCode);
                    throw new ArgumentException("手机验证码不正确，请重新获取手机验证码");
                }

                return true;
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateVerfiyPhoneCode([FromBody] VerifyRequest model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (model.Phone.IsNullOrEmpty())
                    throw new ArgumentNullException("phone", "手机号不能为空");
                if (model.Answer.IsNullOrEmpty())
                    throw new ArgumentNullException("answer", "图片验证码不能为空");
                if (model.Captcha.IsNullOrEmpty())
                    throw new ArgumentNullException("captcha", "图片验证码加密信息不能为空");

                //VerifyResponse
                var response = await CaptchaFactory.Instance.VerifyAsync(model);
                if (response != null && response.Code != 100)
                    throw new ArgumentException("图片验证码错误，错误消息：" + response.Message);

                if (!checkSendIP())
                    throw new ArgumentException("同一IP发送的手机号一天不能超过10个");

                var random = new Random();
                var code = random.Next(100000, 999999).ToString();
                if (!checkSendPhoneCode(model.Phone, code))
                    throw new ArgumentException("同一手机号2分钟之内不能重复发送短息");

                _logger.LogDebug(string.Format("----Phone：{0} generater the code：{1}", model.Phone, code));
                return sendSmsCode(model.Phone, code);
            });
        }

        [AllowAnonymous]
        public IActionResult ExistOrgName(string name, int? id)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var sysManageService = scope.ServiceProvider.GetService<ISysManageService>();
                var tname = sysManageService.ExistOrganizationName(id.Value, name);
                return Json(tname);
            }
        }

        private bool sendSmsCode(string phone, string code)
        {
#if DEBUG
            _logger.LogInformation(string.Format("-----Phone：{0} generater the code：{1}", phone, code));
            return true;
#endif
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var configService = scope.ServiceProvider.GetService<IConfigApiService>();
                    var config = configService.GetTenantSmsConfig(_tenant);
                    var profile = DefaultProfile.GetProfile(config.SmsUrl, config.UserAccount, config.Password);
                    var client = new DefaultAcsClient(profile);
                    var request = new CommonRequest();
                    request.Method = MethodType.POST;
                    request.Domain = "dysmsapi.aliyuncs.com";
                    request.Version = "2017-05-25";
                    request.Action = "SendSms";
                    // request.Protocol = ProtocolType.HTTP;

                    request.AddQueryParameters("SignName", config.Signature);
                    request.AddQueryParameters("PhoneNumbers", phone);
                    request.AddQueryParameters("TemplateCode", "SMS_224970011");
                    request.AddQueryParameters("TemplateParam", "{\"code\":\"" + code + "\"}");
                    request.AddQueryParameters("OutId", "");
                    CommonResponse response = client.GetCommonResponse(request);
                    var content = System.Text.Encoding.Default.GetString(response.HttpResponse.Content);
                    _logger.LogDebug(content);
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 同一手机号，二分钟内不能重复发短信
        /// </summary>
        /// <param name="phone">用户手机号</param>
        /// <returns></returns>
        private bool checkSendPhoneCode(string phone, string code)
        {
            var sessionCode = ConnectionKeyConstant.PHONE_CODE_RERGISTER + phone;
            var currentDate = DateTime.UtcNow;
            var phoneCode = CacheUtil.GetCache<SmsContent>(sessionCode);
            if (phoneCode == null)
            {
                var expiredDate = DateTime.UtcNow.AddMinutes(2);
                var content = new SmsContent();
                content.PhoneNumber = phone;
                content.PhoneCode = code;
                content.ExpiredDateTime = expiredDate;

                CacheUtil.SetCache(sessionCode, content);
                return true;
            }

            if (currentDate > phoneCode.ExpiredDateTime)//有效期内，不用重复发送
                return true;
            return false;
        }

        /// <summary>
        /// 同一个IP一天只能发送10次的注册短信
        /// </summary>
        /// <returns></returns>
        private bool checkSendIP()
        {
            var userIP = Request.HttpContext.Connection.RemoteIpAddress;
            var sessionIP = ConnectionKeyConstant.PHONE_CODE_RERGISTER + userIP;
            int? sendCount = CacheUtil.GetCache<int>(sessionIP);
            _logger.LogDebug(string.Format("---IP: {0} send count: {1}", sessionIP, sendCount));
            if (sendCount == null)
            {
                CacheUtil.SetCache(sessionIP, 10);
                return true;
            }

            if (sendCount > 10)
            {//有效期内，不用重复发送
                _logger.LogError(string.Format("用户IP【0】发送次数超过10次", sessionIP));
                return false;
            }

            CacheUtil.SetCache(sessionIP, sendCount - 1);
            return true;
        }
        #endregion

        #region Provision
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Provision()
        {
            ViewBag.isAuth = this.User.IsAuthenticated();
            ViewBag.userDisplayName = CurrentUserDisplayName;
            return View();
        }
        #endregion

        #region External
        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null)
        {
            if (AccountOptions.WindowsAuthenticationSchemeName == provider)
            {
                // windows authentication needs special handling
                return await ProcessWindowsLoginAsync(returnUrl);
            }
            else
            {
                // start challenge and roundtrip the return URL and 
                var props = new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("ExternalLoginCallback"),
                    Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", provider },
                    }
                };
                return Challenge(props, provider);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        #endregion

        #region Password
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string returnUrl = null)
        {
            ViewBag.isAuth = this.User.IsAuthenticated();
            ViewBag.userDisplayName = CurrentUserDisplayName;
            var model = new ForgotPasswordViewModel()
            {
                TenantName = _tenant?.TenantName,
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                ViewData["ReturnUrl"] = model.ReturnUrl;
                if (!ModelState.IsValid)
                {
                    var errorMsgs = ModelState
                        .Where(m => m.Value.Errors.Any())
                        .SelectMany(x => x.Value.Errors);
                    var errs = errorMsgs
                        .Select(e => e.ErrorMessage)
                        .ToCommaSeparatedString();
                    throw new ArgumentException(errs);
                }

                var user = await _userManager.FindByNameAsync(model.PhoneNumber);
                if (user == null)
                    throw new ArgumentNullException("phoneNumber", string.Format("未找到手机号为：{0}的用户", model.PhoneNumber));

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
                if (!result.Succeeded)
                {
                    var errMsgs = result.Errors.Select(m => m.Description);
                    var errs = errMsgs.ToCommaSeparatedString();
                    throw new ArgumentException(errs);
                }

                return true;
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            ViewBag.isAuth = this.User.IsAuthenticated();
            ViewBag.userDisplayName = CurrentUserDisplayName;
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel
            {
                TenantName = _tenant?.TenantName,
                Code = code
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            ViewBag.isAuth = this.User.IsAuthenticated();
            ViewBag.userDisplayName = CurrentUserDisplayName;
            return View();
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
            if (!User.Identity.IsAuthenticated)
                return Json(new { success = false, message = "用户未登陆，不能修改密码." });

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "数据有误，请重新输入." });

            var user = await _userManager.FindByIdAsync(CurrentUserId);
            if (user == null)
                return Json(new { success = false, message = "未找到当前登录用，请重试." });

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _userManager.UpdateAsync(user);
            }

            return result.Succeeded
                ? Json(new { success = true, message = "密码修改成功." })
                : Json(
                    new
                    {
                        success = false,
                        message = string.Join("<br/>", result.Errors.ToList())
                    });
        }
        /// <summary>
        /// 主页修改邮箱及手机号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ChangeMailPhone(ChangeEmailPhoneViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { success = false, message = "用户未登陆，不能修改邮箱及手机号." });

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "数据有误，请重新输入." });

            var user = await _userManager.FindByIdAsync(CurrentUserId);
            if (user == null)
                return Json(new { success = false, message = "未找到当前登录用，请重试." });

            user.Email = model.Email;
            user.PhoneNumber = model.Phone;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? Json(new { success = true, message = "邮箱及手机号修改成功." })
                : Json(new { success = false, message = "邮箱及手机号修改失败，请重试." });
        }
        #endregion

        #region 退出：SignOut

        public async Task SignOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(Web.Constants.OpenIdConnectConstants.AuthScheme);
                await HttpContext.SignOutAsync(Web.Constants.OpenIdConnectConstants.ChallengeScheme);

                RemoveCurrentUserCache();
            }
        }

        protected void RemoveCurrentUserCache()
        {
            var lowerTenant = TenantConstant.DbaTenantName.ToLower();
            var userCacheKey = lowerTenant + "-" + CacheKeyConstants.Prefix.CurrentUserId + CurrentUserId;
            var menuCacheKey = lowerTenant + "-CurrentUserMenus-" + CurrentUserId;
            var permissionCacheKey = lowerTenant + "-CurrentUserPermissions-" + CurrentUserId;

            CacheUtil.RemoveCache(userCacheKey);
            CacheUtil.RemoveCache(menuCacheKey);
            CacheUtil.RemoveCache(permissionCacheKey);
        }
        #endregion

        [HttpGet]
        public IActionResult AccessDenied()
        {
            ViewBag.isAuth = this.User.IsAuthenticated();
            ViewBag.userDisplayName = CurrentUserDisplayName;
            return View();
        }

        #region Helpers
        /// <summary>
        /// 获取ServiceResult<T>对象的JsonResult
        /// </summary>
        /// <typeparam name="T">所需返回的对象</typeparam>
        /// <param name="func">Lamdba表达式</param>
        /// <returns>ServiceResult<T></returns>
        protected JsonResult GetServiceJsonResult<T>(Func<T> func)
        {
            var result = ServiceWrapper.Invoke(
                "ControllerBase",
                func.Method.Name,
                func,
                _logger);
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
                "ControllerBase",
                func.Method.Name,
                func,
                _logger);
            return Json(result);
        }

        private ClaimsIdentity _identity;
        protected ClaimsIdentity Identity
        {
            get
            {
                if (_identity != null)
                    return _identity;

                if (User != null && User.Identity.IsAuthenticated)
                {
                    _identity = User.Identity as ClaimsIdentity;
                }

                return _identity;
            }
        }

        /// <summary>
        /// 登录用户Id
        /// </summary>
        private string CurrentUserId
        {
            get
            {
                var userId = string.Empty;
                if (this.User != null
                    && this.User.Identity.IsAuthenticated)
                {
                    if (this.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                    {
                        userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    }
                    else if (this.User.FindFirst(KC.IdentityModel.JwtClaimTypes.Subject) != null)
                    {
                        userId = this.User.FindFirst(KC.IdentityModel.JwtClaimTypes.Subject).Value;
                    }
                }
                return userId;
            }
        }
        /// <summary>
        /// 登录用户手机
        /// </summary>
        protected string CurrentUserPhone
        {
            get
            {
                if (Identity != null && Identity.FindFirst(ClaimTypes.MobilePhone) != null)
                {
                    return Identity.FindFirst(ClaimTypes.MobilePhone).Value;
                }
                else if (Identity != null && Identity.FindFirst(JwtClaimTypes.PhoneNumber) != null)
                {
                    return Identity.FindFirst(JwtClaimTypes.PhoneNumber).Value;
                }
                else if (Identity != null && Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_Phone) != null)
                {
                    return Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_Phone).Value;
                }
                return string.Empty;
            }
        }

        protected string CurrentUserDisplayName
        {
            get
            {
                var result = string.Empty;
                if (Identity != null && Identity.FindFirst(ClaimTypes.GivenName) != null)
                {
                    result = Identity.FindFirst(ClaimTypes.GivenName).Value;
                }
                else if (Identity != null && Identity.FindFirst(JwtClaimTypes.GivenName) != null)
                {
                    result = Identity.FindFirst(JwtClaimTypes.GivenName).Value;
                }
                else if (Identity != null && Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_DisplayName) != null)
                {
                    result = Identity.FindFirst(OpenIdConnectConstants.ClaimTypes_DisplayName).Value;
                }

                return result.IsNullOrEmpty() ? CurrentUserPhone.ToHidePhone() : result;
            }
        }
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var redirectUri = string.Empty;
            var tenantName = string.Empty;
            if (string.IsNullOrEmpty(returnUrl))
                return null;

            var authUrl = returnUrl;
            if (returnUrl.StartsWith("http://") || returnUrl.StartsWith("https://"))
            {
                var uri = new Uri(returnUrl);
                if (uri == null || string.IsNullOrEmpty(uri.PathAndQuery))
                    return null;

                authUrl = uri.PathAndQuery;
            }

            var context = await _interaction.GetAuthorizationContextAsync(authUrl);
            tenantName = context?.Tenant;
            redirectUri = context?.RedirectUri;
            if (redirectUri != null && redirectUri.Equals("/Account/Logout"))
            {
                redirectUri = "/";
            }
            if (authUrl != null && authUrl.Equals("/Account/Logout"))
            {
                authUrl = "/";
            }
            _logger.LogDebug(string.Format("---Login parser returnUrl and get tenantName: {0}，redirectUri：{1}，returnUrl：{2}" 
                , tenantName, redirectUri,authUrl));

            if (context?.IdP != null)
            {
                // this is meant to short circuit the UI and only trigger the one external IdP
                return new LoginViewModel
                {
                    EnableLocalLogin = false,
                    ReturnUrl = authUrl,
                    UserName = context?.LoginHint,
                    //TenantName = context.Tenant,
                    TenantName = _tenant?.TenantName,
                    TenantDisplayName = _tenant?.TenantDisplayName,
                    RedirectUrl = redirectUri,
                    ExternalProviders = new ExternalProvider[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
                };
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();
            var providers = schemes
                .Where(x => x.DisplayName != null ||
                            (x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                )
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = authUrl,
                UserName = context?.LoginHint,
                //IsRedirectToTenantLogin = !(_tenant.TenantName.Equals(tenantName, StringComparison.OrdinalIgnoreCase)),
                TenantName = !string.IsNullOrEmpty(tenantName) ? tenantName : _tenant?.TenantName,
                TenantDisplayName = _tenant?.TenantDisplayName,
                RedirectUrl = redirectUri,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.UserName = model.UserName;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != KC.IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        private async Task<IActionResult> ProcessWindowsLoginAsync(string returnUrl)
        {
            // see if windows auth has already been requested and succeeded
            var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
            if (result?.Principal is WindowsPrincipal wp)
            {
                // we will issue the external cookie and then redirect the
                // user back to the external callback, in essence, tresting windows
                // auth the same as any other external authentication mechanism
                var props = new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("ExternalLoginCallback"),
                    Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", AccountOptions.WindowsAuthenticationSchemeName },
                    }
                };

                var id = new ClaimsIdentity(AccountOptions.WindowsAuthenticationSchemeName);
                id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
                id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

                // add the groups as claims -- be careful if the number of groups is too large
                if (AccountOptions.IncludeWindowsGroups)
                {
                    var wi = wp.Identity as WindowsIdentity;
                    var groups = wi.Groups.Translate(typeof(NTAccount));
                    var roles = groups.Select(x => new Claim(JwtClaimTypes.Role, x.Value));
                    id.AddClaims(roles);
                }

                await HttpContext.SignInAsync(
                    IdentityConstants.ExternalScheme,
                    new ClaimsPrincipal(id),
                    props);
                return Redirect(props.RedirectUri);
            }
            else
            {
                // trigger windows auth
                // since windows auth don't support the redirect uri,
                // this URL is re-triggered when we call challenge
                return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
            }
        }

        private async Task<(User user, string provider, string providerUserId, IEnumerable<Claim> claims)>
            FindUserFromExternalProviderAsync(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            var user = await _userManager.FindByLoginAsync(provider, providerUserId);

            return (user, provider, providerUserId, claims);
        }

        private async Task<User> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            // user's display name
            var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            if (name != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, name));
            }
            else
            {
                var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            // email
            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
               claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Email, email));
            }

            var user = new User
            {
                UserName = Guid.NewGuid().ToString(),
            };
            var identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            if (filtered.Any())
            {
                identityResult = await _userManager.AddClaimsAsync(user, filtered);
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
            }

            identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            return user;
        }

        private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                if (localSignInProps.ExpiresUtc == null)
                    localSignInProps.ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(Service.Constants.TimeOutConstants.CookieTimeOut));

                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion

        #region SecurityStamp
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        private static string NewSecurityStamp()
        {
            byte[] bytes = new byte[20];
            _rng.GetBytes(bytes);
            return Base32.ToBase32(bytes);
        }
        internal static class Base32
        {
            private static readonly string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

            public static string ToBase32(byte[] input)
            {
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(input));
                }

                StringBuilder sb = new StringBuilder();
                for (int offset = 0; offset < input.Length;)
                {
                    byte a, b, c, d, e, f, g, h;
                    int numCharsToOutput = GetNextGroup(input, ref offset, out a, out b, out c, out d, out e, out f, out g, out h);

                    sb.Append((numCharsToOutput >= 1) ? _base32Chars[a] : '=');
                    sb.Append((numCharsToOutput >= 2) ? _base32Chars[b] : '=');
                    sb.Append((numCharsToOutput >= 3) ? _base32Chars[c] : '=');
                    sb.Append((numCharsToOutput >= 4) ? _base32Chars[d] : '=');
                    sb.Append((numCharsToOutput >= 5) ? _base32Chars[e] : '=');
                    sb.Append((numCharsToOutput >= 6) ? _base32Chars[f] : '=');
                    sb.Append((numCharsToOutput >= 7) ? _base32Chars[g] : '=');
                    sb.Append((numCharsToOutput >= 8) ? _base32Chars[h] : '=');
                }

                return sb.ToString();
            }

            public static byte[] FromBase32(string input)
            {
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(input));
                }
                input = input.TrimEnd('=').ToUpperInvariant();
                if (input.Length == 0)
                {
                    return new byte[0];
                }

                var output = new byte[input.Length * 5 / 8];
                var bitIndex = 0;
                var inputIndex = 0;
                var outputBits = 0;
                var outputIndex = 0;
                while (outputIndex < output.Length)
                {
                    var byteIndex = _base32Chars.IndexOf(input[inputIndex]);
                    if (byteIndex < 0)
                    {
                        throw new FormatException();
                    }

                    var bits = Math.Min(5 - bitIndex, 8 - outputBits);
                    output[outputIndex] <<= bits;
                    output[outputIndex] |= (byte)(byteIndex >> (5 - (bitIndex + bits)));

                    bitIndex += bits;
                    if (bitIndex >= 5)
                    {
                        inputIndex++;
                        bitIndex = 0;
                    }

                    outputBits += bits;
                    if (outputBits >= 8)
                    {
                        outputIndex++;
                        outputBits = 0;
                    }
                }
                return output;
            }

            // returns the number of bytes that were output
            private static int GetNextGroup(byte[] input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
            {
                uint b1, b2, b3, b4, b5;

                int retVal;
                switch (offset - input.Length)
                {
                    case 1: retVal = 2; break;
                    case 2: retVal = 4; break;
                    case 3: retVal = 5; break;
                    case 4: retVal = 7; break;
                    default: retVal = 8; break;
                }

                b1 = (offset < input.Length) ? input[offset++] : 0U;
                b2 = (offset < input.Length) ? input[offset++] : 0U;
                b3 = (offset < input.Length) ? input[offset++] : 0U;
                b4 = (offset < input.Length) ? input[offset++] : 0U;
                b5 = (offset < input.Length) ? input[offset++] : 0U;

                a = (byte)(b1 >> 3);
                b = (byte)(((b1 & 0x07) << 2) | (b2 >> 6));
                c = (byte)((b2 >> 1) & 0x1f);
                d = (byte)(((b2 & 0x01) << 4) | (b3 >> 4));
                e = (byte)(((b3 & 0x0f) << 1) | (b4 >> 7));
                f = (byte)((b4 >> 2) & 0x1f);
                g = (byte)(((b4 & 0x3) << 3) | (b5 >> 5));
                h = (byte)(b5 & 0x1f);

                return retVal;
            }
        }
        #endregion
    }
}