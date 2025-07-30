using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KC.Service.DTO.Pay;
using KC.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using KC.Enums.Pay;
using Microsoft.AspNetCore.Authorization;
using KC.Framework.Extension;
using KC.Service.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;
using KC.Service.Pay.Constants;
using KC.Framework.Base;
using Microsoft.AspNetCore.Http;
using KC.Model.Component.Queue;
using KC.Service.Pay.WebApiService.Platform;
using KC.Service.Base;
using KC.Common.HttpHelper;

namespace KC.Web.Pay.Controllers
{
    public class BankAccountController : PaymentBaseController
    {
        protected KC.Service.Component.IStorageQueueService StorageQueueService
        {
            get
            {
                //TODO: Storage with TenantName
                return ServiceProvider.GetService<KC.Service.Component.IStorageQueueService>();
            }
        }
        protected IPaymentApiService PaymentApiService => ServiceProvider.GetService<IPaymentApiService>();

        public BankAccountController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<BankAccountController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 账户信息

        public ActionResult AccountInfo(ThirdPartyType paymentType)
        {
            var model = GetPaymentBankAccount(Tenant.TenantName, paymentType);
            return View(model);
        }

        #endregion

        #region 银行账户
        public ActionResult BankInfo(Guid appId)
        {
            ViewBag.AppId = appId;
            return View();
        }

        public JsonResult LoadBankInfo(string name = "")
        {
            var result = PaymentService.GetBankAccountByMemberId(Tenant.TenantName, name);
            return Json(result);
        }

        /// <summary>
        /// 获取单个银行卡数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBankForm(int id)
        {
            var selectBankInfo = new List<SelectListItem>();
            var selectProvince = new List<SelectListItem>();
            var selectCity = new List<SelectListItem>();
            var selectPayBank = new List<SelectListItem>();

            BankAccountDTO model = new BankAccountDTO();

            var result = PaymentApiService.GetSupplyBanksByBusiType("UDK").Result;

            if (result == null || result.Count == 1)
            {
                return base.ThrowErrorJsonMessage(false, "没有银行数据，请先去业务字典添加银行信息");
            }
            else
            {
                selectBankInfo = result.Select(m => new SelectListItem
                {
                    Text = m.BankName,
                    Value = m.UBankId.ToString()
                }).ToList();
            }
            ViewBag.Bankslist = new SelectList(selectBankInfo, "Value", "Text");

            var selectProvinceList = PaymentApiService.GetStrandardProvinceList().Result;

            selectProvince = selectProvinceList.Select(m => new SelectListItem
            {
                Text = m.ProvName,
                Value = m.ProvCode
            }).ToList();

            if (id != 0)
            {
                model = PaymentService.GetBankAccountById(id, Tenant.TenantName);
                GetCityList(model.ProvinceCode, null);
                var selected = selectCity.FirstOrDefault(m => m.Text.Equals(model.ProvinceCode));
                if (selected != null)
                    selected.Selected = true;
            }
            else
            {
                var authentication = PaymentApiService.GetUnitAuthenticationByMemberName(Tenant.TenantName).Result;
                //新增默认赋的值
                model.AccountType = 1;
                model.BankAccountType = "A";
                model.CrossMark = "2";
                model.CardType = "G";
                model.CardNumber = authentication == null ? "" : authentication.UnifiedSocialCreditCode;
                model.BankState = BankAccountState.UnAuthenticated;
                model.MemberId = Tenant.TenantName;
                model.AccountName = Tenant.TenantDisplayName;
            }
            ViewBag.Provicnce = new SelectList(selectProvince, "Value", "Text");
            return PartialView("_BankForm", model);
        }
        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <param name="provinceID"></param>
        /// <param name="selectValue"></param>
        /// <returns></returns>
        public JsonResult GetCityList(string provinceCode, string selectValue)
        {
            if (!string.IsNullOrEmpty(provinceCode))
            {
                var selecCitytList = new List<SelectListItem>();
                var data = PaymentApiService.GetStrandardCityList(provinceCode).Result;
                if (data != null)
                {
                    selecCitytList = data.Select(model => new SelectListItem
                    {
                        Text = model.CityName,
                        Value = model.CityCode
                    }).ToList();
                    if (string.IsNullOrEmpty(selectValue))
                    {
                        var aa = new SelectList(selecCitytList, "Value", "Text");
                        return Json(aa);
                    }
                    else
                    {
                        var bb = new SelectList(selecCitytList, "Value", "Text", selectValue);
                        return Json(bb);
                    }

                }
                return Json(data);
            }
            return Json(new { success = false, message = "请先选择省。" });


            // return base.LoadCitiesByProvinceId(provinceID);
        }

        public JsonResult GetPayBankList(string cityCode, string bankCode, string branchBankCode, string selectValue)
        {
            if (!string.IsNullOrEmpty(cityCode) && !string.IsNullOrEmpty(bankCode))
            {
                var selecPayBankList = new List<SelectListItem>();
                var data = PaymentApiService.GetPayBankList(cityCode, bankCode, branchBankCode, "").Result;
                if (data != null)
                {
                    selecPayBankList = data.Select(model => new SelectListItem
                    {
                        Text = model.CXUMC1,
                        Value = model.FQHHO2
                    }).ToList();
                    if (string.IsNullOrEmpty(selectValue))
                    {
                        var aa = new SelectList(selecPayBankList, "Value", "Text");
                        return Json(aa);
                    }
                    else
                    {
                        var bb = new SelectList(selecPayBankList, "Value", "Text", selectValue);
                        return Json(bb);
                    }

                }
                return Json(data);
            }
            return Json(new { success = false, message = "请先选择城市和银行" });
        }

        /// <summary>
        /// 检查银行卡是否已被注册
        /// </summary>
        /// <param name="bankNumber"></param>
        /// <returns></returns>
        public JsonResult CheckBankNumber(string bankNumber, int id)
        {
            //if (!Luhn(bankNumber))
            //    return ThrowErrorJsonMessage(false, "银行卡号格式不正确");
            return GetServiceJsonResult(() =>
            {
                var memberBank = PaymentService.GetMemberBankByBankNumber(bankNumber);
                if (memberBank.Count > 0)
                {
                    if (id > 0)
                    {
                        if (memberBank.FirstOrDefault(m => m.Id != id) != null)
                        {
                            return false;
                        }
                        return true;
                    }
                    return false;
                }
                return true;
            });
        }

        /// <summary>
        /// 根据字典名字值列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBankList()
        {
            var result = PaymentApiService.GetSupplyBanksByBusiType("UDK");
            return Json(result);
        }
        /// <summary>
        /// 获取银行卡类型
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBankAccountType()
        {
            List<BankType> list = new List<BankType>();
            var dic = EnumExtensions.GetEnumDictionary<BankAccountType>();
            foreach (var VARIABLE in dic)
            {
                BankType bankType = new BankType();
                bankType.TypeId = VARIABLE.Key;
                bankType.TypeName = VARIABLE.Value;
                list.Add(bankType);
            }
            return Json(list);
        }

        //public JsonResult GetCardType()
        //{
        //    var codeDTO = AccountApiService.GetCodeListByCodeType("CertificateType");
        //    return Json(codeDTO);
        //}
        /// <summary>
        /// 修改银行卡信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveBankForm(BankAccountDTO model)
        {
            if (string.IsNullOrEmpty(model.OpenBankCode))
            {
                return base.ThrowErrorJsonMessage(false, "开户网点为空！");
            }
            var payBankList = PaymentApiService.GetPayBankList(model.CityCode, model.BankId, model.OpenBankCode, "").Result;

            if (payBankList == null || payBankList.Count < 1)
            {
                return base.ThrowErrorJsonMessage(false, "开户网点错误！");
            }

            //不验证
            if (model.IsUnVerify)
            {
                model.BankState = BankAccountState.AuthenticateSuccess;
            }

            var result = PaymentService.SaveBankAccount(model);
            if (result)
            {
                return base.ThrowErrorJsonMessage(true, "保存数据成功");
            }

            return base.ThrowErrorJsonMessage(false, "保存数据失败，请重试。");
        }

        /// <summary>
        /// 认证申请
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public JsonResult AuthenApplication(int accountId)
        {
            return GetServiceJsonResult(() =>
            {
                BankAuthenticationApplicationDTO paramDTO = new BankAuthenticationApplicationDTO();
                paramDTO.MemberId = Tenant.TenantName;
                paramDTO.UserName = CurrentUserName;
                paramDTO.BankAccountId = accountId;
                paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
                paramDTO.EncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(paramDTO, Tenant.PrivateEncryptKey);
                string postUrl = PayApiDomain + CPCNMethodConstants.BankAuthenticationAppliction;
                var postData = KC.Common.ToolsHelper.OtherUtilHelper.GetPostData(paramDTO);
                var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
                return SerializeHelper.FromJson<JsonPaymentReturnModel>(result.Item2).Result;
            });
        }

        /// <summary>
        /// 认证校验
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public JsonResult BankAuthenthentication(int accountId, decimal amount)
        {
            return GetServiceJsonResult(() =>
            {
                BankAuthenticationDTO paramDTO = new BankAuthenticationDTO();
                paramDTO.MemberId = Tenant.TenantName;
                paramDTO.UserName = CurrentUserName;
                paramDTO.BankAccountId = accountId;
                paramDTO.Amount = amount * 100;//单位为分
                paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
                paramDTO.EncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(paramDTO, Tenant.PrivateEncryptKey);

                string postUrl = PayApiDomain + CPCNMethodConstants.BankAuthentication;
                var postData = KC.Common.ToolsHelper.OtherUtilHelper.GetPostData(paramDTO);
                var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
                return SerializeHelper.FromJson<JsonPaymentReturnModel>(result.Item2).Result;
            });
        }

        /// <summary>
        /// 绑定银行卡
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="state">1绑定 3取消绑定</param>
        /// <returns></returns>
        public JsonResult BindBankAccount(int accountId, int state)
        {
            return GetServiceJsonResult(() =>
            {
                if (state == 1 && IsBindingBankAccount())
                {
                    PaymentReturnModel returnModel = new PaymentReturnModel();
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "只能绑定一个银行账户，请先解绑原账户后再进行操作!";
                    return returnModel;
                }
                BindBankAccountDTO paramDTO = new BindBankAccountDTO();
                paramDTO.MemberId = Tenant.TenantName;
                paramDTO.UserName = CurrentUserName;
                //根据id获取BankAccount数据
                paramDTO.BankId = accountId;
                paramDTO.BindState = state;
                paramDTO.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
                paramDTO.EncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(paramDTO, Tenant.PrivateEncryptKey);
                string postUrl = PayApiDomain + CPCNMethodConstants.BindBankAccount;
                var postData = KC.Common.ToolsHelper.OtherUtilHelper.GetPostData(paramDTO);
                var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
                return SerializeHelper.FromJson<JsonPaymentReturnModel>(result.Item2).Result;
            });
        }

        /// <summary>
        /// 是否绑定银行卡
        /// </summary>
        /// <returns></returns>
        private bool IsBindingBankAccount()
        {
            var paymentBankAccount = GetPaymentBankAccount();
            if (paymentBankAccount == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(paymentBankAccount.BindBankAccount))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 绑定的银行卡信息(包含余额信息)
        /// </summary>
        /// <returns></returns>
        public PaymentBankAccountDTO GetPaymentBankAccount(string tenantName = null, ThirdPartyType type = ThirdPartyType.CPCNConfigSign)
        {
            return PaymentService.GetPaymentBankAccountByMemberId(tenantName, type);
        }

        /// <summary>
        /// 删除银行卡信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult RemoveBank(int id)
        {
            var result = PaymentService.RemoveBankById(id);
            if (result)
            {
                return base.ThrowErrorJsonMessage(true, "删除数据成功");
            }

            return base.ThrowErrorJsonMessage(false, "保存数据失败，请重试。");
        }


        public class BankType
        {
            public int TypeId { get; set; }
            public string TypeName { get; set; }
        }

        #endregion

        #region 密码管理

        public ActionResult UpdateBoHaiPassword()
        {
            return View();
        }

        /// <summary>
        /// 设置手机号
        /// </summary>
        /// <returns></returns>
        public ActionResult SetPhone(Guid appId)
        {
            ViewBag.AppId = appId;
            return View();
        }

        public ActionResult SetPassword(Guid appId)
        {
            ViewBag.AppId = appId;
            var paymentInfo = GetPaymentInfo(Tenant.TenantName, ThirdPartyType.CPCNConfigSign);
            ViewBag.phone = paymentInfo.Phone;
            return View();
        }

        public ActionResult UpdatePhone(Guid appId)
        {
            ViewBag.AppId = appId;
            var paymentInfo = GetPaymentInfo(Tenant.TenantName, ThirdPartyType.CPCNConfigSign);
            ViewBag.phone = paymentInfo.Phone;
            return View();
        }

        public ActionResult UpdatePassword(Guid appId)
        {
            ViewBag.AppId = appId;
            var paymentInfo = GetPaymentInfo(Tenant.TenantName, ThirdPartyType.CPCNConfigSign);
            ViewBag.phone = paymentInfo.Phone;
            return View();
        }

        public ActionResult PasswordManage(Guid appId)
        {
            ViewBag.AppId = appId;
            var paymentInfo = GetPaymentInfo(Tenant.TenantName, ThirdPartyType.CPCNConfigSign);
            ViewBag.phone = paymentInfo.Phone;

            return View();
        }

        [HttpPost]
        public JsonResult SubmitUpPhone(string oldmobile, string newmobile, string oldcode, string newcode)
        {
            var paymentInfo = GetPaymentInfo(Tenant.TenantName, ThirdPartyType.CPCNConfigSign);
            if (paymentInfo == null)
            {
                return Json(new { message = "交易手机号修改失败", success = false });
            }
            //校验输入的旧手机号是否一致
            if (oldmobile != paymentInfo.Phone)
            {
                return Json(new { message = "原验证手机号不正确", success = false });
            }

            #region 校验
            if (string.IsNullOrEmpty(oldmobile))
            {
                return Json(new { message = "原验证手机号不能为空", success = false });
            }
            var bl = System.Text.RegularExpressions.Regex.IsMatch(oldmobile, @"^[1]+[3,5,4,7,8]+\d{9}");
            if (HttpContext.Session.GetString(oldmobile) != null)
            {
                if (HttpContext.Session.GetString(oldmobile) != oldcode)
                {
                    return Json(new { message = "原验证手机验证码错误", success = false });
                }
            }
            else
            {
                return Json(new { message = "原验证手机验证码错误", success = false });
            }
            if (!bl)
            {
                return Json(new { message = "原验证手机号码格式不正确", success = false });
            }
            if (string.IsNullOrEmpty(newmobile))
            {
                return Json(new { message = "新验证手机号不能为空", success = false });
            }
            var bl1 = System.Text.RegularExpressions.Regex.IsMatch(newmobile, @"^[1]+[3,5,4,7,8]+\d{9}");
            if (HttpContext.Session.GetString(newmobile) != null)
            {
                if (HttpContext.Session.GetString(newmobile) != newcode)
                {
                    return Json(new { message = "新验证手机验证码错误", success = false });
                }
            }
            else
            {
                return Json(new { message = "新验证手机验证码错误", success = false });
            }
            if (!bl1)
            {
                return Json(new { message = "新验证手机号码格式不正确", success = false });
            }
            #endregion

            var isSuccess = PaymentService.SavePaymentPhoneById(paymentInfo.Id, newmobile);
            if (!isSuccess)
            {
                return Json(new { message = "交易手机号修改失败", success = false });
            }
            return Json(new { message = "操作成功", success = true });
        }

        [HttpPost]
        public JsonResult SubmitUpPassword(string phone, string tradePassword, string phoneCode)
        {
            var paymentInfo = GetPaymentInfo(Tenant.TenantName, ThirdPartyType.CPCNConfigSign);
            if (paymentInfo == null)
            {
                return Json(new { message = "修改交易密码失败", success = false });
            }
            if (phone != paymentInfo.Phone)
            {
                return Json(new { message = "验证手机号不正确", success = false });
            }

            #region 校验
            if (string.IsNullOrEmpty(phone))
            {
                return Json(new { message = "验证手机号不能为空", success = false });
            }
            var bl = System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[1]+[3,5,4,7,8]+\d{9}");
            if (HttpContext.Session.GetString(phone) != null)
            {
                if (HttpContext.Session.GetString(phone) != phoneCode)
                {
                    return Json(new { message = "验证手机验证码错误", success = false });
                }
            }
            else
            {
                return Json(new { message = "验证手机验证码错误", success = false });
            }
            if (!bl)
            {
                return Json(new { message = "验证手机号码格式不正确", success = false });
            }
            if (string.IsNullOrEmpty(tradePassword))
            {
                return Json(new { message = "新交易密码不能为空！", success = false });
            }
            if (tradePassword.Length < 6)
            {
                return Json(new { message = "请设置6位及以上的新交易密码！", success = false });
            }
            #endregion
            var encryPassword = KC.Common.ToolsHelper.OtherUtilHelper.GetEncryptionString(tradePassword, Tenant.PrivateEncryptKey);

            var isSuccess = PaymentService.SavePaymentPasswordById(paymentInfo.Id, encryPassword);
            if (!isSuccess)
            {
                return Json(new { message = "修改交易密码失败", success = false });
            }
            return Json(new { message = "操作成功", success = true });
        }
        [HttpPost]
        public JsonResult SubmitSetPhone(string mobile, string code)
        {
            #region 校验
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new { message = "手机号码不能为空", success = false });
            }
            var bl = System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^[1]+[3,5,4,7,8]+\d{9}");

            if (HttpContext.Session.GetString(mobile) != null)
            {
                if (HttpContext.Session.GetString(mobile) != code)
                {
                    return Json(new { message = "手机验证码错误", success = false });
                }
                else
                {
                    HttpContext.Session.SetString(mobile, "");
                }
            }
            else
            {
                return Json(new { message = "手机验证码错误", success = false });
            }

            if (!bl)
            {
                return Json(new { message = "手机号码格式不正确", success = false });
            }

            #endregion
            var paymentInfo = GetPaymentInfo(Tenant.TenantName, ThirdPartyType.CPCNConfigSign);
            if (paymentInfo == null)
            {
                return Json(new { message = "设置交易手机号失败", success = false });
            }
            var isSuccess = PaymentService.SavePaymentPhoneById(paymentInfo.Id, mobile);
            if (!isSuccess)
            {
                return Json(new { message = "设置交易手机号失败", success = false });
            }

            return Json(new { message = "操作成功", success = true });
        }

        /// <summary>
        /// 初次设置密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitSetPassword(string password)
        {

            if (string.IsNullOrEmpty(password))
            {
                return Json(new { message = "设置交易密码为空！", success = false });
            }
            if (password.Length < 6)
            {
                return Json(new { message = "密码设置低于6位！", success = false });
            }
            var encryPassword = KC.Common.ToolsHelper.OtherUtilHelper.GetEncryptionString(password, Tenant.PrivateEncryptKey);
            var paymentInfo = GetPaymentInfo(Tenant.TenantName, ThirdPartyType.CPCNConfigSign);

            var mobile = paymentInfo.Phone;
            if (paymentInfo == null)
            {
                return Json(new { message = "设置交易密码失败", success = false });
            }
            if (!string.IsNullOrEmpty(paymentInfo.TradePassword))
            {
                return Json(new { message = "设置交易密码失败", success = false });
            }
            var isSuccess = PaymentService.SavePaymentPasswordById(paymentInfo.Id, encryPassword);
            if (!isSuccess)
            {
                return Json(new { message = "设置交易密码失败", success = false });
            }
            return Json(new { message = "操作成功", success = true });
        }

        /// <summary>
        /// 修改渤海云账本密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitBoHaiPassword(string oldPassword, string password)
        {
            if (string.IsNullOrEmpty(oldPassword))
            {
                return Json(new { message = "请输入旧密码！", success = false });
            }

            if (string.IsNullOrEmpty(password))
            {
                return Json(new { message = "设置交易密码为空！", success = false });
            }
            if (password.Length < 6)
            {
                return Json(new { message = "密码设置低于6位！", success = false });
            }

            UpdatePasswordParamDTO model = new UpdatePasswordParamDTO();
            model.MemberId = Tenant.TenantName;
            model.UserName = CurrentUserName;
            model.Timestamp = ConvertDateTimeToInt(DateTime.UtcNow);
            model.EncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(model, Tenant.PrivateEncryptKey);
            model.Password = password;
            model.OldPassword = oldPassword;

            string postUrl = GlobalConfig.PayWebDomain + BoHaiMethodConstants.ControllerName + "/" + BoHaiMethodConstants.UpdatePassword;
            var postData = KC.Common.ToolsHelper.OtherUtilHelper.GetPostData(model);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
            var retModel = SerializeHelper.FromJson<JsonPaymentReturnModel>(result.Item2).Result;

            if (retModel.Success)
            {
                return Json(new { message = "修改成功！", success = true });
            }

            return Json(new { message = retModel.ErrorMessage, success = false });

        }


        private void SetSessionPhoneCode(string phone, string code)
        {
            var prefix = "CreateElectronic-phonecode-";
            var utcNow = DateTime.UtcNow;
            var content = new SmsContent();
            content.PhoneNumber = phone;
            content.ExpiredDateTime = utcNow.AddMinutes(TimeOutConstants.PhoneCodeTimeout);
            content.PhoneCode = code;
            HttpContext.Session.SetString(prefix + phone, SerializeHelper.ToJson(content));
        }
        private bool IsSendPhoneCode(string phone)
        {
            var prefix = "CreateElectronic-phonecode-";
            var utcNow = DateTime.UtcNow;
            var phoneCode = SerializeHelper.FromJson<SmsContent>(HttpContext.Session.GetString(prefix + phone));
            if (phoneCode == null)
                return true;
            //if (phoneCode.ExpiredDateTime > utcNow)//有效期内，不用重复发送
            //    return false;
            return true;
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GenerateVerfiyPhoneCode(string mobile, int type, bool isNewPhone = false)
        {
            string phone = mobile;
            if (string.IsNullOrEmpty(phone))
            {
                return Json(new { message = "手机号码不能为空", success = false });
            }
            var bl = phone.IsMobile();
            if (!bl)
            {
                return Json(new { message = "手机号码格式不正确", success = false });
            }
            var paymentInfo = GetPaymentInfo(Tenant.TenantName, ThirdPartyType.CPCNConfigSign);
            if (paymentInfo == null)
            {
                return Json(new { message = "手机号不存在，发送验证码失败！", success = false });
            }
            //不是新的手机号去校验
            if (!isNewPhone)
            {
                if (phone != paymentInfo.Phone)
                {
                    return Json(new { message = "手机号不正确！", success = false });
                }
            }
            if (!IsSendPhoneCode(phone))
                return Json(new { success = true, message = string.Empty });
            var ra = new Random();
            var code = ra.Next(100000, 1000000).ToString();
            HttpContext.Session.SetString(phone, code);
            SetSessionPhoneCode(phone, code);
            Logger.LogInformation("----Set Phone Code in Session: " + code + ",Phone:" + phone);
            string message = "";
            if (type == 1)
            {
                message = string.Format("您正在设置验证手机号，验证码为：{0}，请在5分钟之内完成操作。", code);
            }
            else if (type == 2)
            {
                message = string.Format("您正在设置交易密码，验证码为：{0}，请在5分钟之内完成操作。", code);
            }

            return Json(new { success = StorageQueueService.InsertSmsQueue(new SmsInfo() { Phone = new List<long> { long.Parse(phone) }, SmsContent = message, Type = SmsType.Notice, Tenant = TenantName }), message = string.Empty });
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[ValidateMvcCaptcha("VerificationCode")]
        public JsonResult GenerateVerfiyPhoneCodes(string mobile)
        {
            string phone = mobile;
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(phone))
                {
                    return Json(new { success = false, message = string.Empty });
                }
                return Json(new { success = false, message = "图形验证码不正确！" });
            }
            if (string.IsNullOrEmpty(phone))
            {
                return Json(new { message = "手机号码不能为空", success = false });
            }
            var bl = phone.IsMobile();
            if (!bl)
            {
                return Json(new { message = "手机号码格式不正确", success = false });
            }

            if (!IsSendPhoneCode(phone))
                return Json(new { success = true, message = string.Empty });
            var ra = new Random();
            var code = ra.Next(100000, 1000000).ToString();
            HttpContext.Session.SetString(phone, code);
            SetSessionPhoneCode(phone, code);

            var message = string.Format("您正在修改验证手机号，验证码为{0}，请在5分钟之内完成操作。", code);
            return Json(new { success = StorageQueueService.InsertSmsQueue(new SmsInfo() { Phone = new List<long> { long.Parse(phone) }, SmsContent = message, Type = SmsType.Notice, Tenant = TenantName }), message = string.Empty });
        }

        #endregion
    }
}