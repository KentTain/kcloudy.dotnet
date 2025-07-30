using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KC.Service.DTO.Pay;
using KC.Common.HttpHelper;
using KC.Common;
using Microsoft.AspNetCore.Mvc;
using KC.Service.Pay;
using System.Text;
using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Framework.Base;
using KC.Service.WebApiService.Business;

namespace KC.Web.Pay.Controllers
{
    public abstract class PaymentBaseController : Web.Controllers.TenantWebBaseController
    {
        protected ITenantUserApiService TenantStore => ServiceProvider.GetService<ITenantUserApiService>();
        protected Service.WebApiService.Business.IAccountApiService AccountApiService
        {
            get
            {
                return ServiceProvider.GetService<Service.WebApiService.Business.IAccountApiService>();
            }
        }

        /// <summary>
        /// 获取支付接口地址：http://[tenantName].payapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:8002/api/
        /// </summary>
        /// <returns></returns>
        protected string PayApiDomain;
        protected const string PlatformAccountName = "";
        protected const string PlatformAccountNum = "";
        
        protected IPaymentService PaymentService => ServiceProvider.GetService<IPaymentService>();

        public PaymentBaseController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger logger)
            : base(tenant, serviceProvider, logger)
        {
            PayApiDomain = GetPaymentApiUrl(TenantName);
        }

        #region 选小图标控件: Shared/_SelectUserPartial.cshtml
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.PartialViewResult _SelectIconPartial()
        {
            return PartialView();
        }

        #endregion 选小图标控件: Shared/_SelectUserPartial.cshtml

        #region 选人控件: Shared/_SelectUserPartial.cshtml
        /// <summary>
        /// 获取所有的部门信息及下属员工
        /// </summary>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.JsonResult GetRootOrganizationsWithUsers()
        {
            var res = AccountApiService.LoadOrgTreesWithUsers();
            return Json(res);
        }

        /// <summary>
        /// 获取相关部门以及角色信息及下属员工
        /// </summary>
        /// <param name="searchModel">筛选条件：部门Ids列表、角色Ids列表</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<Microsoft.AspNetCore.Mvc.JsonResult> GetOrgsWithUsersByRoleIdsAndOrgids(Service.DTO.Search.OrgTreesAndRolesWithUsersSearchDTO searchModel)
        {
            var orgs = await AccountApiService.LoadOrgTreesWithUsersByOrgIds(searchModel);
            var roles = await AccountApiService.LoadRolesWithUsersByRoleIds(searchModel);
            return this.Json(new { orgInfos = orgs, roleInfos = roles });
        }

        #endregion 选人控件: Shared/_SelectUserPartial.cshtml

        /// <summary>
        /// 获取登录用户接口地址：http://[tenantName].payapi.kcloudy.com/api/
        ///     本地测试接口地址：http://[tenantName].localhost:8002/api/
        /// </summary>
        /// <param name="tenantName">租户代码</param>
        /// <returns></returns>
        protected string GetPaymentApiUrl(string tenantName)
        {
            if (string.IsNullOrEmpty(GlobalConfig.PayWebDomain))
                return null;

            return GlobalConfig.PayWebDomain.Replace(TenantConstant.SubDomain, tenantName)
                .Replace("pay.", "payapi.").Replace(":8001", ":8002") + "api/";
        }

        /// <summary>
        /// 绑定的银行卡信息(包含余额信息)
        /// </summary>
        /// <returns></returns>
        protected PaymentBankAccountDTO GetPaymentBankAccount(string tenantName = null, ThirdPartyType type = ThirdPartyType.CPCNConfigSign)
        {
            return PaymentService.GetPaymentBankAccountByMemberId(tenantName, type);
        }

        /// <summary>
        /// 获取银行卡信息
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        protected List<PaymentBankAccountDTO> GetPaymentAccounts(string memberId)
        {
            return PaymentService.GetPaymentAccounts(memberId);
        }

        /// <summary>
        /// 获取开通的结算体系
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        protected List<string> GetPaymentAccountNames(string memberId)
        {
            var accounts = PaymentService.GetPaymentAccounts(memberId);
            if (accounts == null || !accounts.Any())
                return new List<string>();
            return accounts.Select(m => m.PaymentType.ToDescription()).ToList();
        }
        /// <summary>
        /// 是否开通支付
        /// </summary>
        /// <returns></returns>
        public JsonResult IsOpenPaymentPhone()
        {
            var result = PaymentService.IsOpenPayment(TenantName, ThirdPartyType.CPCNConfigSign);
            return ThrowErrorJsonMessage(result, "");
        }

        /// <summary>
        /// 是否进行安全设置(设置支付和绑定支付手机号码)
        /// </summary>
        /// <returns></returns>
        public bool IsOpenSecuritySetting()
        {
            return PaymentService.IsOpenPayment(TenantName, ThirdPartyType.CPCNConfigSign);
        }

        /// <summary>
        /// 是否开通支付和绑定银行卡，Item1:开通支付，Item2:绑定银行卡
        /// </summary>
        /// <returns></returns>
        public Tuple<bool, bool> CheckOpenPaymentAndBindBank(string tenantName = null)
        {
            var paymentBankAccount = GetPaymentBankAccount(tenantName);
            if (paymentBankAccount == null)
                return new Tuple<bool, bool>(false, false);
            if (string.IsNullOrEmpty(paymentBankAccount.BindBankAccount))
            {
                return new Tuple<bool, bool>(true, false);
            }
            return new Tuple<bool, bool>(true, true);
        }


        /// <summary>
        /// 是否绑定银行卡
        /// </summary>
        /// <returns></returns>
        public bool IsBindingBankAccount()
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

        public BankAccountDTO GetBindBankAccount()
        {
            return PaymentService.GetBindBankAccount(base.TenantName);
        }

        public ActionResult Payment(bool isAdminPortal, int payType, int orderAmount, string billNo, string orderTime,
            string goodsName = null, string remark = "")
        {
            var paymentReturnModel = PaymentService.Payment(isAdminPortal, CurrentUserId, payType, orderAmount, billNo, orderTime, goodsName, remark);
            if (!paymentReturnModel.Success)
            {
                ViewBag.ErrorMessage = "配置信息错误:" + paymentReturnModel.ErrorMessage;
                return View("Payment");
            }
            //Response.ContentEncoding = Encoding.UTF8; // 指定输出编码
            //Response.Write(paymentReturnModel.HtmlStr);
            ViewBag.Message = paymentReturnModel.HtmlStr;
            return View("Payment", paymentReturnModel.HtmlStr);
        }

        public IActionResult PaymentPickup(Object response, int sign)
        {
            var paymentReturnModel = PaymentService.PaymentFrontUrl(CurrentUserId, response, sign);
            if (!paymentReturnModel.Success)
            {
                ViewBag.Message = paymentReturnModel.ErrorMessage;
                return View("PaymentPickup");
            }
            ViewBag.Message = paymentReturnModel.HtmlStr;
            return View("PaymentPickup");
        }

        public IActionResult PaymentReceive(Object response, int sign)
        {
            var paymentReturnModel = PaymentService.PaymentBackUrl(CurrentUserId, response, sign);
            if (!paymentReturnModel.Success)
            {
                ViewBag.Message = paymentReturnModel.ErrorMessage;
                return View("PaymentReceive");
            }
            ViewBag.Message = paymentReturnModel.HtmlStr;
            return View("PaymentReceive");
        }

        /// <summary>
        /// 支付请求的返回方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramDTO"></param>
        /// <param name="postUrl"></param>
        /// <returns></returns>
        protected PaymentReturnModel PaymentReturn<T>(T paramDTO, string postUrl) where T : PayBaseParamDTO
        {
            PaymentReturnModel returnModel = new PaymentReturnModel();
            var isLegal = IsLegalData(paramDTO, paramDTO.EncryptString).Result;
            if (!isLegal)
            {
                returnModel.Success = false;
                returnModel.ErrorMessage = "数据校验有误！";
            }
            else
            {
                var postData = KC.Common.ToolsHelper.OtherUtilHelper.GetPostData(paramDTO);
                var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
                if (result.Item1)
                {
                    returnModel = SerializeHelper.FromJson<PaymentReturnModel>(result.Item2);
                }
                else
                {
                    returnModel.Success = false;
                    returnModel.ErrorMessage = "请求站点报错！";
                }
            }
            return returnModel;
        }

        /// <summary>
        /// 对加密的数据进行验证是否合法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="entryData"></param>
        /// <returns></returns>
        public async Task<bool> IsLegalData<T>(T model, string entryData) where T : PayBaseParamDTO
        {
            var memberId = model.MemberId;
            var encryptString = entryData;
            var timeStamp = ConvertDateTimeToInt(DateTime.UtcNow);
            //校验时间差绝对值是否大于一分钟
            if (System.Math.Abs(timeStamp - model.Timestamp) > 60000)
            {
                return false;
            }
            //根据memberId 获取TenantUser 的PricateEncryptKey
            var tenantUser = await TenantStore.GetTenantByName(memberId);
            var newEncryptString = KC.Common.ToolsHelper.OtherUtilHelper.GetStrByModel(model, tenantUser.PrivateEncryptKey);
            if (encryptString != newEncryptString)
            {
                return false;
            }
            return true;
        }

        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Utc);
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        public PaymentInfoDTO GetPaymentInfo(string memberId, ThirdPartyType configSign)
        {
            return PaymentService.GetPaymentInfo(memberId, configSign);
        }
    }
}