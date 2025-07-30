using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using KC.Framework.Tenant;
using KC.Service.Customer;
using Microsoft.Extensions.DependencyInjection;
using KC.Enums.CRM;
using KC.Service.DTO.Customer;
using System.Text;
using Microsoft.AspNetCore.Http;
using KC.Framework.Base;
using KC.Common;
using KC.Service.WebApiService.Business;
using KC.Service.Customer.WebApiService;

namespace KC.Web.Customer.Controllers
{
    public class CustomerInfoController : CustomerBaseController
    {
        protected ITenantSimpleApiService TenantStore => ServiceProvider.GetService<ITenantSimpleApiService>();
        protected ICustomerService CustomerService => ServiceProvider.GetService<ICustomerService>();
        protected ICustomerSeaService CustomerSeaService => ServiceProvider.GetService<ICustomerSeaService>();
        public CustomerInfoController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<AccountController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [Web.Extension.MenuFilter("我的客户", "我的客户", "/CustomerInfo/Index",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-user-circle", AuthorityId = "6DDD2C07-9224-4CDF-86D4-B3A072E5540B",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("我的客户", "客户信息维护", "/CustomerInfo/Index", "2FB7CF43-A9D3-4DA1-A0B6-D46DCFD68166",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "2FB7CF43-A9D3-4DA1-A0B6-D46DCFD68166")]
        public ActionResult Index(int initTabIndex = 0)
        {
            GetDropDownList();
            //客户信息页TabIndex
            ViewBag.InitTabIndex = initTabIndex;
            //当前操作人是否可以转移客户到公海
            ViewBag.CanTransforCustomer = CustomerSeaService.CanTransferCutomerToSeas(CurrentUserId);

            return View();
        }

        #region 客户信息维护

        #region Customer Info
        //[Web.Extension.PermissionFilter("我的客户", "加载客户列表", "/CustomerInfo/LoadCustomerInfoList", "984D5C3B-214F-4DC8-9E09-08D745B6BE58",
        //    DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "984D5C3B-214F-4DC8-9E09-08D745B6BE58")]
        public async Task<JsonResult> LoadCustomerInfoList(int page = 1, int rows = 10, string searchKey = "",
            string searchValue = "", int customerType = 100, int clientType = 100, int customerLevel = 100,
            int customerSource = 100, bool onlyShowClientSerivce = true, string customerManangeName = "",
            string startTime = "", string endTime = "", string businessScope = "",
            string area = "", bool onlyShowAssignedCustomer = true)
        {
            var typeList = new Dictionary<string, int>
            {
                {"customerType", customerType},
                {"clientType", clientType},
                {"customerLevel", customerLevel},
                {"customerSource", customerSource}
            };
            DateTime? createdStartTime = string.IsNullOrEmpty(startTime) ? null : (DateTime?)Convert.ToDateTime(startTime);
            DateTime? createdEndTime = string.IsNullOrEmpty(endTime)
                ? null
                : (DateTime?)Convert.ToDateTime(endTime).AddDays(1);
            searchValue = !string.IsNullOrEmpty(searchValue) ? searchValue.Trim() : searchValue;
            var result = await CustomerService.GetPaginatedCustomerInfosByFilterAsync(page, rows,
                searchKey, searchValue, typeList,
                CurrentUserRoleIds, CurrentUserId, onlyShowClientSerivce, customerManangeName,
                createdStartTime, createdEndTime, businessScope, area,
                onlyShowAssignedCustomer);

            return Json(result);
        }

        /// <summary>
        /// 显示客户信息的表单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tabIndex"></param>
        /// <returns></returns>
        public PartialViewResult GetCustomerInfoForm(int id, int tabIndex)
        {
            var model = new CustomerInfoDTO();
            if (id != 0)
            {
                model = CustomerService.GetCustomerInfoById(id);
            }


            ViewBag.ProvinceList = KC.Web.Util.SelectItemsUtil.GetProvinceSelectItems(ServiceProvider, null);
            ViewBag.TabIndex = tabIndex;
            GetDropDownList(model);
            return PartialView("_customerForm", model);
        }

        public JsonResult GetCustomerInfoById(int id)
        {
            var model = CustomerService.GetCustomerInfoById(id);
            return Json(model);
        }

        [Web.Extension.PermissionFilter("我的客户", "保存客户信息", "/CustomerInfo/SaveCustomerInfoForm", "09F580F0-C773-45E6-A5E8-776CB6BEF381",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "09F580F0-C773-45E6-A5E8-776CB6BEF381")]
        public JsonResult SaveCustomerInfoForm(CustomerInfoDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                var result = CustomerService.SaveCustomerInfo(model, CurrentUserDisplayName, CurrentUserId);
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return true;
                }
                return false;
            });
        }

        [Web.Extension.PermissionFilter("我的客户", "删除客户信息", "/CustomerInfo/SaveCustomerInfoForm", "8529088E-79E1-4727-BCA2-D52C1CE9DCD3",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "8529088E-79E1-4727-BCA2-D52C1CE9DCD3")]
        public JsonResult RemoveCustomerInfo(List<int> idList)
        {
            return GetServiceJsonResult(() => CustomerService.RemoveCustomerInfoByIds(idList, CurrentUserName));
        }

        #endregion

        #region download & upload Excel file
        [Web.Extension.PermissionFilter("我的客户", "下载客户导入模板", "/CustomerInfo/DownLoadExcelTemplate", "94208BD0-09EA-4A17-BF8E-5D9BE545F730",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 5, IsPage = false, ResultType = ResultType.ContentResult, AuthorityId = "94208BD0-09EA-4A17-BF8E-5D9BE545F730")]
        public ActionResult DownLoadExcelTemplate(CompanyType customerType)
        {
            var url = ServerPath + "Content/excels/CustomerTemplateOne.xlsx";
            if (customerType == CompanyType.Supplier || customerType == CompanyType.Institute)
            {
                url = ServerPath + "~/css/excels/CustomerTemplateTwo.xlsx";
            }
            return File(url, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// Excel导入
        /// </summary>
        /// <param name="file"></param>
        /// <param name="uploadType"></param>
        /// <param name="md5"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("我的客户", "导入客户模板数据", "/CustomerInfo/UploadExcelTemplate", "43E9C743-932E-45D1-9E0D-7E01E27B4F8E",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 6, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "43E9C743-932E-45D1-9E0D-7E01E27B4F8E")]
        public JsonResult UploadExcelTemplate()
        {
            return GetServiceJsonResult(() =>
            {
                var files = Request.Form.Files;
                if (files.Count == 0)
                    throw new Exception("请选择上传文件");

                string uploadType = Request.Form["uploadType"];
                string md5 = Request.Form["md5"];
                IFormFile file = files[0];

                int importToRowIndex = 0;
                StringBuilder businessExceptions = new StringBuilder();
                var result = CustomerService.ImportCustomerDataFromExcel(file.OpenReadStream(), CurrentUserId,
                    CurrentUserDisplayName, ref importToRowIndex, businessExceptions);
                return new { result, number = importToRowIndex, message = businessExceptions.ToString() };
            });
        }

        public FileResult UsersToExcel(string searchKey = "",
            string searchValue = "", int customerType = 100, int clientType = 100, int customerLevel = 100, int customerManangeName = 100,
            int customerSource = 100, bool onlyShowClientSerivce = true)
        {
            var typeList = new Dictionary<string, int>
            {
                {"customerType", customerType},
                {"clientType", clientType},
                {"customerLevel", customerLevel},
                {"customerSource", customerSource}
                 //{"customerManangeName", customerManangeName}
            };

            //如果用户不是客服经理的角色的话，仅仅显示自己的客户；如果用户既是客服有事客服经理，显示所有客户
            //if (!onlyShowClientSerivce && !CurrentUserRoleNames.Contains(RoleType.CustomerServiceManager))
            //    onlyShowClientSerivce = true;

            var result = CustomerService.SearchCustomerTable(searchKey, searchValue, typeList, CurrentUserRoleIds,
                CurrentUserId, onlyShowClientSerivce);
            if (result != null)
            {
                var oldColumn = new string[]
                {
                    "CustomerName", "CustomerType", "ClientType", "CustomerLevel", "ContactName", "ContactPhoneNumber",
                    "ContactEmail", "RecommandedUserName", "CustomerManangeName","OrganizationName"
                };
                var newColumn = new string[] { "客户名称", "客户类型", "服务类型", "客户等级", "联系人", "联系电话", "联系邮箱", "推荐人", "客户经理", "所属部门" };
                //DataTable ExportDt = this.ListToDataTable<AspNetUsers>(result);

                var fileName = "客户信息-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                //var stream = new NPOIExcelWriter().ExportToExcelStream(result, "Sheet1", "表头", oldColumn, newColumn);

                return File(fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 个人客户是否存在该手机号码
        /// </summary>
        /// <param name="customerId">客户Id</param>
        /// <param name="phoneNumber">客户手机号</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExistPersonalCustomerPhoneNumber(int customerId, string phoneNumber)
        {
            var result = CustomerService.ExistPersonalCustomerPhoneNumber(customerId, phoneNumber);
            return Json(result);
        }
        #endregion

        #endregion

        #region 客户详情
        [Web.Extension.PermissionFilter("客户详情", "客户详情", "/CustomerInfo/CustomerDetailInfo", "17BE6A9F-49EC-475B-B7B1-339ACB12E781",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "17BE6A9F-49EC-475B-B7B1-339ACB12E781")]
        [Web.Extension.MenuFilter("我的客户", "客户详情", "/CustomerInfo/CustomerDetailInfo",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-user", AuthorityId = "81A3E5AB-30B8-4A0A-B393-7E646E674BB1",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 2, IsExtPage = true, Level = 3)]
        public ActionResult CustomerDetailInfo(int customerId = 0, int customerType = 0, int isoperate = 0)
        {
            var model = CustomerService.GetCustomerInfoById(customerId);
            ViewBag.CustomerId = customerId;
            ViewBag.isoperate = isoperate;
            return View(model);
        }

        [Web.Extension.PermissionFilter("客户详情", "编辑客户简介", "/CustomerInfo/CustomerDetail", "C4E08917-67B5-415A-B3A7-CAAFE76B4D88",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "C4E08917-67B5-415A-B3A7-CAAFE76B4D88")]
        public JsonResult CustomerDetail(string customerId, string introduction)
        {
            return GetServiceJsonResult(() =>
            {
                return CustomerService.EditCustomer(customerId, introduction);
            });
        }

        #region Customer Contact    联系人列表

        [Web.Extension.PermissionFilter("客户联系人", "客户联系人", "/CustomerInfo/GetCustomerContact", "49C11A1E-A0E5-4FBE-A932-E6A40D034ED2",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "49C11A1E-A0E5-4FBE-A932-E6A40D034ED2")]
        [Web.Extension.MenuFilter("我的客户", "客户联系人", "/CustomerInfo/GetCustomerContact",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-user", AuthorityId = "AAC43DAB-1A8B-47FC-9106-E242518732E7",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 3, IsExtPage = true, Level = 3)]
        public ActionResult GetCustomerContact(int customerId = 0)
        {
            ViewBag.CustomerId = customerId;
            return View("CustomerContactList");
        }

        /// <summary>
        /// 查询客户联系人记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="searchKey">搜索关键字</param>
        /// <param name="searchValue">搜索关键字Value</param>
        /// <param name="customerId">客户Id</param>
        /// <returns></returns>
        //[Web.Extension.PermissionFilter("客户联系人", "加载客户联系人列表", "/CustomerInfo/GetCustomerContactByCustomerId", "C358343F-9E92-455E-BB68-D0FEEEB7DA12",
        //    DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "C358343F-9E92-455E-BB68-D0FEEEB7DA12")]
        public JsonResult GetCustomerContactByCustomerId(int page, int rows, string searchKey, string searchValue, int customerId = 0)
        {
            searchValue = !string.IsNullOrEmpty(searchValue) ? searchValue.Trim() : searchValue;
            var result = CustomerService.GetPaginatedCustomerContactsByCustomerId(page, rows, searchKey, searchValue, customerId);
            return Json(result);
        }

        public PartialViewResult CustomerContactForm(int customerId, int id = 0)
        {
            ViewBag.Id = id;
            //ViewBag.customerId = customerId;
            var model = new CustomerContactDTO();
            if (id != 0)
            {
                model = CustomerService.GetCustomerContactsById(id);
            }

            var customer = CustomerService.GetCustomerInfoById(customerId);
            if (customer != null)
            {
                ViewBag.CustomerType = (int)CustomerService.GetCustomerInfoById(customerId).CompanyType;
            }
            return PartialView("_customerContactForm", model);
        }
        [Web.Extension.PermissionFilter("客户联系人", "保存客户联系人", "/CustomerInfo/SaveCustomerContactForm", "B504DA27-ED87-4D39-BFCF-5D3B0F64D307",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "B504DA27-ED87-4D39-BFCF-5D3B0F64D307")]
        public JsonResult SaveCustomerContactForm(CustomerContactDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                var bl = model.Id > 0
                    ? CustomerService.EditCustomerContact(model)
                    : CustomerService.CreateCustomerContact(model);

                return bl;
            });
        }
        [Web.Extension.PermissionFilter("客户联系人", "删除客户联系人", "/CustomerInfo/RemoveCustomerContact", "B3922576-F341-4342-A290-8A39E9DC6242",
            DefaultRoleId = RoleConstants.PurchaseManagerRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "B3922576-F341-4342-A290-8A39E9DC6242")]
        public JsonResult RemoveCustomerContact(int id)
        {
            return GetServiceJsonResult(() =>
            {
                var result = CustomerService.RemoveCustomerContactSoft(id);
                //var result = CustomerService.RemoveCustomerContact(id);
                return result;
            });
        }

        #endregion

        #region Customer ExtInfo 客户扩展信息

        #region 客户扩展信息列表
        /// <summary>
        /// 客户扩展信息列表 + ActionResult GetCustomerExtraInfoList(int customerId)
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public ActionResult GetCustomerExtraInfoList(int customerId)
        {
            ViewBag.CustomerId = customerId;
            return View("CustomerExtraInfoList");
        }
        /// <summary>
        /// 读取客户扩展信息列表数据 + JsonResult LoadCustomerExtraInfoListByCustomerId(int customerId, int page = 1, int rows = 10)
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult LoadCustomerExtraInfoListByCustomerId(int customerId, int page = 1, int rows = 10)
        {
            var res = CustomerService.GetPaginatedCustomerExtInfoByCustomerId(page, rows, customerId);
            return Json(res);
        }
        #endregion

        #region 保存修改 & 移除 客户扩展信息列表
        /// <summary>
        /// 保存修改 + JsonResult SaveCustomerExtInfo()
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveAttribute([FromBody] string formData)
        {
            return base.GetServiceJsonResult(() =>
            {
                //string formData = Request.Form[0];
                var list = SerializeHelper.FromJson<List<CustomerExtInfoDTO>>(formData);
                if (list.Any())
                {
                    var a = Convert.ToInt32(list.FirstOrDefault().CustomerId);
                    list.ForEach(m => m.CustomerId = a);
                }

                return CustomerService.SaveCustomerExtInfoDtos(list);
            });
        }

        /// <summary>
        /// 移除 + JsonResult RemoveCustomerExtInfo(int propertyAttributeId)
        /// </summary>
        /// <param name="propertyAttributeId"></param>
        /// <returns></returns>
        public JsonResult RomoveAttr(int propertyAttributeId)
        {
            return base.GetServiceJsonResult(() =>
            {
                //硬删除
                //var res = CustomerService.RomoveCustomerExtInfoDto(propertyAttributeId);
                //软删除
                var res = CustomerService.RomoveCustomerExtInfoDtoSoft(propertyAttributeId);
                return res;
            });
        }
        #endregion

        #region 下拉列表 + JsonResult GenerateEditorData()
        /// <summary>
        /// 下拉列表 + JsonResult GenerateEditorData()
        /// </summary>
        /// <returns></returns>
        public JsonResult GenerateEditorData()
        {
            var res = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<AttributeDataType>();
            return Json(res);
        }
        #endregion

        #endregion

        #endregion

        #region 客户跟踪信息、变更日志

        public ActionResult CustomerInfoList()
        {
            GetDropDownList();
            return View();
        }

        #region Customer Change Log
        public ActionResult CustomerChangeLogList(int customerId)
        {
            ViewBag.customerId = customerId;
            return View();
        }

        /// <summary>
        /// 当前客户操作日志
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public JsonResult LoadCustomerChangeLogList(int customerId, string startTime, string endTime, int page = 1, int rows = 10, string searchKey = "", string searchValue = "")
        {
            searchValue = !string.IsNullOrEmpty(searchValue) ? searchValue.Trim() : searchValue;
            var result = CustomerService.GetCustomerChangeLogsByCustomerInfoId(page, rows, searchKey, searchValue, startTime, endTime, customerId);
            return Json(result);
        }

        public PartialViewResult CustomeChangeLogForm(int id = 0)
        {
            var model = new CustomerChangeLogDTO();
            if (id != 0)
            {
                model = CustomerService.GetCustomerChangeLogDTO(id);
            }
            return PartialView("_customeChangeLogForm", model);
        }

        public JsonResult EditCustomeChangeLog(CustomerChangeLogDTO modle)
        {
            return base.GetServiceJsonResult(() =>
            {
                return modle.Id > 0
                    ? CustomerService.EditCustomerChangeLogDTO(modle)
                    : CustomerService.AddCustomerChangeLogDTO(modle);
            });
        }

        public JsonResult RemoveCustomeChangeLog(int id = 0)
        {
            return base.GetServiceJsonResult(() =>
            {
                return CustomerService.RemoveCustomerChangeLogDTO(id);
            });
        }

        #endregion

        #region Customer Tracing Log

        public ActionResult CustomerTracingLogList(int customerId = 0, string customerName = null)
        {
            if (string.IsNullOrEmpty(customerName))
            {
                var customer = CustomerService.GetCustomerInfoById(customerId);
                if (customer != null)
                    customerName = customer.CustomerName;
            }

            ViewBag.CustomerId = customerId;
            ViewBag.CustomerName = customerName;
            ViewBag.TracingTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<TracingType>();
            return View();
        }

        public JsonResult GetActivityLogListByCustomerInfoId(int customerInfoId,
            string startTime, string endTime, int page = 1, int rows = 10, string searchKey = "",
            string searchValue = "", int tracingType = 100)
        {
            string customerName = string.Empty;
            string activityName = string.Empty;
            string operatoruser = string.Empty;
            if (!string.IsNullOrWhiteSpace(searchKey)
                && !string.IsNullOrWhiteSpace(searchValue))
            {
                searchValue = searchValue.Trim();
                switch (searchKey)
                {
                    case "customerName":
                        customerName = searchValue;
                        break;
                    case "activityName":
                        activityName = searchValue;
                        break;
                    case "operatoruser":
                        operatoruser = searchValue;
                        break;
                }
            }

            if (tracingType != 100)
            {
                var data = CustomerService.GetActivityLogListByCustomerInfoId(customerInfoId, page, rows, customerName,
                    activityName, operatoruser, startTime, endTime, (TracingType)tracingType);
                return Json(data);
            }
            else
            {
                var data = CustomerService.GetActivityLogListByCustomerInfoId(customerInfoId, page, rows, customerName,
                    activityName, operatoruser, startTime, endTime, null);
                return Json(data);
            }
        }

        public PartialViewResult CustomerTracingLogForm(int customerId, int id = 0, string customerName = null)
        {
            var model = new CustomerTracingLogDTO();
            if (id != 0)
            {
                model = CustomerService.GetActivityLogById(id);
            }
            else
            {
                model.CustomerId = customerId;
                model.CustomerName = customerName;
            }
            return PartialView("_customerTracingLogForm", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult SaveCustomerTracingLog(CustomerTracingLogDTO model)
        {
            return base.GetServiceJsonResult(() =>
            {
                //var customer = CustomerService.GetCustomerInfoById(model.CustomerId ?? 0);
                //if (customer == null)
                //    return base.ThrowErrorJsonMessage(bl, message);
                //model.CustomerId = customer.CustomerId;
                //model.CustomerName = customer.CustomerName;

                model.OperateDate = DateTime.UtcNow;
                model.Operator = CurrentUserDisplayName;
                model.OperatorId = CurrentUserId;
                model.Type = ProcessLogType.Success;
                model.TracingType = TracingType.Manual;

                return CustomerService.SaveCustomerTracingLogDTO(model);
            });
        }
        #endregion

        #endregion

        #region 客服分配

        public ActionResult AssignCustomerList()
        {
            GetDropDownList();
            return View();
        }

        /// <summary>
        /// 系统自动平均分配客服
        /// </summary>
        /// <returns></returns>
        public JsonResult ShareCustomerWithManager(List<string> selectedUsers)
        {
            return base.GetServiceJsonResult(
                () => CustomerService.ShareCustomerWithManager(selectedUsers, CurrentUserId));
        }

        /// <summary>
        /// 重新分配客户
        /// </summary>
        /// <param name="selectedUserIds"></param>
        /// <param name="customerIds"></param>
        /// <returns></returns>
        public JsonResult ReassignCustomerToManager(List<string> selectedUserIds, List<int> customerIds)
        {
            return base.GetServiceJsonResult(
                () => CustomerService.ReassignCustomerToOtherManager(selectedUserIds, customerIds, CurrentUserId));
        }

        /// <summary>
        /// 转移客户
        /// </summary>
        /// <param name="selectdUserId"></param>
        /// <param name="customerIds"></param>
        /// <returns></returns>
        public JsonResult TransferCustomerToOtherManager(string selectdUserId, List<int> customerIds)
        {
            return
                base.GetServiceJsonResult(
                    () => CustomerService.TransferCustomerToOtherManager(selectdUserId, customerIds, CurrentUserId));
        }

        #endregion

        #region 客户推送

        /// <summary>
        /// 编辑邮件/短信弹窗
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GetTenantSendingForm()
        {
            return PartialView("_sendingTenantForm");
        }

        public JsonResult LoadTenantUserList(int page, int rows, string searchKey = "", string searchValue = "")
        {
            string server = string.Empty;
            string database = string.Empty;
            string userName = string.Empty;
            if (!string.IsNullOrWhiteSpace(searchKey)
                && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey.ToLower())
                {
                    case "tenantname":
                        userName = searchValue.Trim();
                        break;
                }
            }

            var result = TenantStore.GetTenantUsersByOpenAppId(page, rows, ApplicationConstant.CrmAppId, userName, base.TenantName);

            return Json(result);
        }

        /// <summary>
        /// 推送客户
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="tenantNames"></param>
        /// <returns></returns>
        public JsonResult SendUserToTenant(string idList, string tenantNames)
        {
            return base.GetServiceJsonResult(() =>
            {
                var customerIds = new List<int>();
                if (!string.IsNullOrEmpty(idList))
                {
                    customerIds = idList.Split(',').ToList().ConvertAll(int.Parse);
                }
                var tenantNameList = new List<string>();
                if (!string.IsNullOrEmpty(tenantNames))
                {
                    var list = tenantNames.Split(',').Select(i => i);
                    tenantNameList = list.Where(m => !m.Equals(base.TenantName)).ToList();
                }

                var result = CustomerService.SendCustomersToTenant(customerIds, tenantNameList, CurrentUserId,
                    CurrentUserDisplayName);
                if (!string.IsNullOrWhiteSpace(result))
                    Logger.LogError(result);

                return string.IsNullOrWhiteSpace(result);
            });
        }

        #endregion

        #region 客服经理
        public async Task<JsonResult> GetCustomerManagersByCustomerId(int customerId)
        {
            var result = await CustomerService.GetCustomerManagersByCustomerIdAsync(CurrentUserId, customerId, CurrentUserRoleIds);
            return Json(result);
        }
        #endregion

        #region utils
        protected void GetDropDownList()
        {
            ViewBag.CustomerTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CompanyType>();
            ViewBag.ClientTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<ClientType>();
            ViewBag.ClientSourceList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CustomerSource>();
        }
        protected void GetDropDownList(CustomerInfoDTO model)
        {
            ViewBag.CustomerTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CompanyType>((int)model.CompanyType);
            ViewBag.ClientTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<ClientType>((int)model.ClientType);
            ViewBag.ClientSourceList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CustomerSource>((int)model.CustomerSource);
        }

        #endregion
    }
}
