using KC.Service.Customer;
using KC.Web.Customer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KC.Enums.CRM;
using KC.Service.DTO.Customer;
using System.Text;
using KC.Framework.Base;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Mvc;
using KC.Model.Component.Queue;
using KC.Service.Util;
using KC.Service.CallCenter;
using KC.Framework.Extension;
using KC.Web.Customer.Models;
using KC.Service.WebApiService.Business;
using KC.Web.Util;

namespace KC.Portal.CRM.Controllers
{
    public class NotificationController : CustomerBaseController
    {
        protected KC.Service.Component.IStorageQueueService StorageQueueService
        {
            get
            {
                //TODO: Storage with TenantName
                return ServiceProvider.GetService<KC.Service.Component.IStorageQueueService>();
            }
        }

        protected IConfigApiService ConfigApiService => ServiceProvider.GetService<IConfigApiService>();
        protected ICustomerService CustomerService => ServiceProvider.GetService<ICustomerService>();
        protected ICustomerSeaService CustomerSeaService => ServiceProvider.GetService<ICustomerSeaService>();
        public NotificationController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<NotificationController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        public ActionResult Index()
        {
            ViewBag.CustomerTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CompanyType>();
            ViewBag.ClientTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<ClientType>();
            ViewBag.ClientSourceList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CustomerSource>();
            ViewBag.CurrentUserMobile = CurrentUserPhone;
            return View();
        }


        /// <summary>
        /// 查询联系人
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="customerType">客户类型</param>
        /// <param name="viewName">通知方式(Email：邮件；Sms：短信)</param>
        /// <param name="searchKey">查询关键字</param>
        /// <param name="searchValue">查询关键字的值</param>
        /// <returns></returns>
        public ActionResult LoadCustomerContactInfos(int page, int rows, int? customerType, string viewName,
            string searchKey, string searchValue)
        {
            searchValue = !string.IsNullOrEmpty(searchValue) ? searchValue.Trim() : searchValue;
            var result = CustomerService.GetPaginatedCustomerContactInfos(page, rows, CurrentUserId,CurrentUserRoleIds, (CompanyType?)customerType, viewName, searchKey,
                searchValue);
            return Json(result);
        }

        /// <summary>
        /// 查询联系人的Form
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public ActionResult GetSelectContactForm(string viewName)
        {
            ViewBag.CustomerTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CompanyType>();
            ViewBag.ViewName = viewName;
            return PartialView("_SelectContactForm");
        }

        /// <summary>
        ///邮件
        /// </summary>
        /// <param name="customerContactId">当前联系人</param>
        /// <returns></returns>
        public async Task<IActionResult> EmailAsync(int customerContactId = 0)
        {
            var CurrentContact = customerContactId;
            var CustomerTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CompanyType>();
            var ProcessLogTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<ProcessLogType>();
            var data =
                CustomerService.GetCustomerCompanyByCustomerContact(customerContactId).FirstOrDefault()??
                    new CustomerContactInfoDTO();

            var result = await ViewEngineUtil.RenderToStringAsync(ServiceProvider, "Notification/Email", CustomerContactInfoViewModel.FromDto(data, CurrentContact, CustomerTypeList, ProcessLogTypeList));
            return Content(result);
        }

        /// <summary>
        ///短信
        /// </summary>
        /// <param name="customerContactId"> 当前联系人Id</param>
        /// <returns></returns>
        public async Task<ActionResult> SmsAsync(int customerContactId = 0)
        {
            var CurrentContact = customerContactId;
            var CustomerTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CompanyType>();
            var ProcessLogTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<ProcessLogType>();
            var data = CustomerService.GetCustomerCompanyByCustomerContact(customerContactId).FirstOrDefault() ??
                        new CustomerContactInfoDTO();

            var result = await ViewEngineUtil.RenderToStringAsync(ServiceProvider, "Notification/Sms", CustomerContactInfoViewModel.FromDto(data, CurrentContact, CustomerTypeList, ProcessLogTypeList));
            return Content(result);
        }

        /// <summary>
        /// 话务
        /// </summary>
        /// <returns></returns>
        public ActionResult Call()
        {
            ViewBag.CurrentUserTelphone = CurrentUser.UserPhone;
            ViewBag.CustomerTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CompanyType>();
            ViewBag.CallStateList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CallState>();
            return View();
        }

        /// <summary>
        /// 跟踪日志
        /// </summary>
        /// <param name="customerContactId"></param>
        /// <returns></returns>
        public async Task<ActionResult> CustomerTracingLogListAsync(int customerContactId = 0)
        {
            var CurrentContact = customerContactId;
            var CustomerTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CompanyType>();
            var ProcessLogTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<ProcessLogType>();
            var TracingTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<TracingType>();
            var data = CustomerService.GetCustomerCompanyByCustomerContact(customerContactId).FirstOrDefault() ??
                        new CustomerContactInfoDTO();

            var result = await ViewEngineUtil.RenderToStringAsync(ServiceProvider, "Notification/CustomerTracingLogList", CustomerContactInfoViewModel.FromDto(data, CurrentContact, CustomerTypeList, ProcessLogTypeList, TracingTypeList));

            return Content(result);
        }

        /// <summary>
        /// 发送通知
        /// created by clf on 2017-10-26
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ccList"></param>
        /// <param name="viewName"></param>
        /// <param name="subject"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SendNotifications(List<CustomerTracingLogDTO> model, List<string> ccList, string viewName,
            string subject, string contents)
        {
            return base.GetServiceJsonResult(() =>
            {
                bool flag = false;

                switch (viewName)
                {
                    case "Email":
                        flag = SendEmail(model, ccList, subject, contents);
                        break;
                    case "SMS":
                        flag = SendSMS(model, contents);
                        break;
                }
                return flag;
            });
        }

        /// <summary>
        /// 发送邮件
        /// created by clf on 2017-10-26
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ccList"></param>
        /// <param name="subject"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool SendEmail(List<CustomerTracingLogDTO> model, List<string> ccList,
            string subject, string contents)
        {
            foreach (var m in model)
            {
                EmailInfo email = new EmailInfo
                {
                    UserId = base.CurrentUserId,
                    Tenant = base.TenantName,
                    EmailTitle = subject,
                    EmailBody =contents.Replace("[name]", m.ContactName),
                    SendTo =new List<string>{m.SendTo},
                    CcTo = ccList
                };
                var config = ConfigApiService.GetTenantEmailConfig(Tenant);
                Task.Factory.StartNew(() =>
                {
                    EmailUtil.Send(config, email.EmailTitle, email.EmailBody,
                        email.SendTo, email.CcTo);
                });
                //插入跟踪日志
                InsertTrackingLog(m, ccList, "Email", subject, contents,true,model.Count()>1);
            }
            return true;
        }

        /// <summary>
        /// 发送短信
        /// created by clf on 2017-10-30
        /// </summary>
        /// <param name="model"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool SendSMS(List<CustomerTracingLogDTO> model, string contents)
        {
            var isSucceed = true;
            foreach (var m in model)
            {
                long phone;
                if (long.TryParse(m.SendTo, out phone))
                {
                    var smsInfo = new SmsInfo
                    {
                        Tenant = base.TenantName,
                        Type = SmsType.Notice,
                        Phone = new List<long> {phone},
                        SmsContent = contents.Replace("[name]", m.ContactName)
                    };
                    isSucceed = StorageQueueService.InsertSmsQueue(smsInfo);
                    InsertTrackingLog(m, null, "SMS", null, contents, isSucceed,model.Count()>1);
                    if (!isSucceed) break;
                }
            }
            //插入跟踪日志
            return isSucceed;
        }

        private bool InsertTrackingLog(CustomerTracingLogDTO model, List<string> ccList,
            string viewName, string title, string contents, bool isSucceed = true, bool isMass = false)
        {
            CustomerTracingLogDTO log = new CustomerTracingLogDTO
            {
                CustomerId = model.CustomerId,
                CustomerName = model.CustomerName,
                CustomerContactId = model.CustomerContactId,
                ContactName = model.ContactName,
                OperateDate = DateTime.Now,
                Operator = CurrentUserName,
                OperatorId = CurrentUserId,
                SendTo = model.SendTo,
                Type = isSucceed ? ProcessLogType.Success : ProcessLogType.Failure,
                Title = title
            };
            switch (viewName)
            {
                case "Email":
                    log.From = CurrentUserEmail;
                    log.CcTo = ccList != null ? string.Join(",", ccList.ToArray()) : null;
                    log.Remark = contents.Replace("[name]", model.ContactName);
                    log.TracingType = isMass ? TracingType.MassEmailNotify : TracingType.EmailNotify;
                    break;
                case "SMS":
                    log.From = CurrentUserPhone;
                    log.Remark = contents.Replace("[name]", model.ContactName);
                    log.TracingType = isMass ? TracingType.MassSmsNotify : TracingType.SmsNotify;
                    break;
                default:
                    log.TracingType = TracingType.Manual;
                    log.Remark = model.Remark;
                    break;
            }
            return CustomerService.SaveCustomerTracingLogDTO(new List<CustomerTracingLogDTO> {log});
        }

        

        /// <summary>
        /// 查询客户跟踪日志
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="currentContactId">当前联系人Id</param>
        /// <param name="customerType">客户类型</param>
        /// <param name="searchKey">searchKey</param>
        /// <param name="searchValue">searchValue</param>
        /// <param name="startTime">开始日期</param>
        /// <param name="endTime">结束日期</param>
        /// <param name="type">跟踪类型（日志类型）</param>
        /// <param name="processLogType">跟踪状态</param>
        /// <param name="callState">通话状态</param>
        /// <returns></returns>
        public JsonResult GetCustomerTracingLogList(string searchKey, string searchValue, string startTime, int? currentContactId,
            string endTime, CompanyType? customerType, TracingType? type, ProcessLogType? processLogType,
            CallState? callState, int page = 1, int rows = 10)
        {
            searchValue = !string.IsNullOrEmpty(searchValue) ? searchValue.Trim() : searchValue;
            var data = CustomerService.GetCustomerTraceInfoReport(page, rows, CurrentUserId,CurrentUserRoleIds,currentContactId,searchKey, searchValue, startTime, endTime,
                customerType, type, processLogType, callState);
            return Json(data);
        }

        public ActionResult GetCustomerTracingLogForm(int id=0) 
        {
            CustomerTracingLogDTO model=new CustomerTracingLogDTO();
            if (id != 0)
            {
                model = CustomerService.GetActivityLogById(id);
            }
            ViewBag.TracingTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<TracingType>();
            return PartialView("_TracingLogForm",model);
        }

        /// <summary>
        ///  根据联系人获取联系人所属单位
        /// </summary>
        /// <param name="customerContactId">联系人Id</param>
        /// <returns></returns>
        public JsonResult GetCustomerCompanyByCustomerContact(int customerContactId)
        {
            var result =CustomerService.GetCustomerCompanyByCustomerContact(customerContactId);
            return Json(result);
        }

       /// <summary>
        /// 根据客户分类获取联系人
       /// </summary>
       /// <param name="customerType">客户分类Id</param>
       /// <returns></returns>
        public JsonResult GetCustomerContactsByCustomerType(CompanyType customerType)
       {
           var result = CustomerService.GetCustomerContactsByCompanyType(customerType);
           return Json(result);
        }

        /// <summary>
        /// 新增跟踪日志
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SaveCustomerTracingLog(CustomerTracingLogDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                if (model.ProcessLogId == 0)
                {
                    return InsertTrackingLog(model, null, null, null, null);
                }
                var data = CustomerService.GetActivityLogById(model.ProcessLogId);
                data.CustomerId = model.CustomerId;
                data.CustomerName = model.CustomerName;
                data.CustomerContactId = model.CustomerContactId;
                data.ContactName = model.ContactName;
                data.Remark = model.Remark;
                return CustomerService.SaveCustomerTracingLogDTO(data);
            });
        }

        /// <summary>
        /// 删除跟踪日志
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public JsonResult RomoveCustomerTracingLog(List<int> ids)
        {
            return base.GetServiceJsonResult(() =>
            {
                return CustomerService.RomoveCustomerTracingLogDTO(ids);
            });
        }

        /// <summary>
        /// 去电（来电）弹屏显示客户信息Form
        /// </summary>
        /// <returns></returns>
        public ActionResult PopCustomerForm(int customerContactId)
        {
            CustomerContactInfoDTO model =
                CustomerService.GetCustomerCompanyByCustomerContact(customerContactId).FirstOrDefault();
            if (model != null)
            {
                ViewBag.CustomerTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CompanyType>((int) model.CompanyType);
                ViewBag.ClientTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ClientType>((int) model.ClientType);
            }
            return PartialView("_PopCustomerForm", model ?? new CustomerContactInfoDTO());
        }

        /// <summary>
        /// 去电(来电保存客户信息)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="referenceId">当前通话会话Id</param>
        /// <param name="callRemark">当前通话备注</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SavePopCustomerInfo(CustomerContactInfoDTO model, string referenceId,string callRemark)
        {
            return GetServiceJsonResult(() => CustomerService.SavePopCustomerInfo(model, referenceId,callRemark));
        }

        #region 拨号

        public JsonResult CallCustomer(int customerContactId, int customerId, string customerName, string contactName,
            string customerPhoneNumber, CallType phoneType = CallType.Telephone)
        {
            if (string.IsNullOrWhiteSpace(customerPhoneNumber))
                return base.ThrowErrorJsonMessage(false, "客户未设置联系方式，请补充客户的联系方式后重试！");

            if (phoneType == CallType.Telephone && string.IsNullOrEmpty(CurrentUserTelephone))
                return base.ThrowErrorJsonMessage(false, "当前登录用户未设置座机及其分机号，请补充个人联系方式！");

            if (phoneType == CallType.Mobile && string.IsNullOrEmpty(CurrentUserPhone))
                return base.ThrowErrorJsonMessage(false, "当前登录用户未设置手机号，请补充个人联系方式！");

            var busyMsg = CustomerService.IsNotBusy(CurrentUserTelephone);
            if (!string.IsNullOrWhiteSpace(busyMsg))
                return base.ThrowErrorJsonMessage(false, string.Format("呼叫失败（错误消息：{0}），请重试！", busyMsg));

            var result = string.Empty;
            string referenceId=string.Empty;
            switch (phoneType)
            {
                case CallType.Telephone:
                    result = CustomerService.RunCallContact(customerContactId, CurrentUserId, CurrentUserDisplayName,
                        CurrentUserTelephone,
                        customerId, customerName, contactName, customerPhoneNumber, phoneType, out referenceId);
                    break;
                case CallType.Mobile:
                    result = CustomerService.RunCallContact(customerContactId, CurrentUserId, CurrentUserDisplayName,
                        CurrentUserPhone,
                        customerId, customerName, contactName, customerPhoneNumber, phoneType, out referenceId);
                    break;
            }

            var isSuccess = string.IsNullOrWhiteSpace(result);
            return base.ThrowErrorJsonMessage(isSuccess,
                isSuccess ? referenceId : string.Format("呼叫失败（错误消息：{0}），请重试！", result));
        }

        /// <summary>
        /// 中断
        /// </summary>
        /// <param name="customerPhoneNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public JsonResult StopCallCustomer(string customerPhoneNumber, string message)
        {
            return base.GetServiceJsonResult(() => CustomerService.StopCallContact(customerPhoneNumber, message));
        }

        #endregion
    }
}