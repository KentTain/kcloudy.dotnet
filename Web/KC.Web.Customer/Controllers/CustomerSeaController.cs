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
using KC.Framework.Base;

namespace KC.Web.Customer.Controllers
{
    public class CustomerSeaController : CustomerBaseController
    {
        protected ICustomerSeaService CustomerSeaService => ServiceProvider.GetService<ICustomerSeaService>();
        public CustomerSeaController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<AccountController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            //当前操作人是否可以从公海中捕捞客户
            ViewBag.CanPickCustomerFromSeas = CustomerSeaService.CanTransferCutomerToSeas(CurrentUserId);
            ViewBag.CustomerTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CompanyType>();
            ViewBag.CustomerSeasTimeSpanList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<CustomerSeasTimeSpan>();
            return View();
        }

        #region 公海客户

        /// <summary>
        /// 获取公海客户列表
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="searchValue"></param>
        /// <param name="customerType"></param>
        /// <param name="customerSeasTimeSpan"></param>
        /// <param name="operatorName"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task<JsonResult> LoadCustomerSeasList(string searchKey, string searchValue, CompanyType? customerType,
            CustomerSeasTimeSpan? customerSeasTimeSpan, string operatorName, int page = 0, int rows = 10)
        {
            searchValue = !string.IsNullOrEmpty(searchValue) ? searchValue.Trim() : searchValue;
            var result = await CustomerSeaService.GetPaginatedCustomerSeasByFilterAsync(page, rows, searchKey, searchValue,
                customerType, CurrentUserId, operatorName, (int?)customerSeasTimeSpan);
            return Json(result);
        }

        public JsonResult GetAllCustomerExtInfoProviers(string searchKey = "", string searchValue = "")
        {
            searchValue = !string.IsNullOrEmpty(searchValue) ? searchValue.Trim() : searchValue;
            var list = CustomerSeaService.GetAllCustomerExtInfoProviers(searchKey, searchValue);
            return Json(list);
        }

        /// <summary>
        /// 转移客户到公海
        /// </summary>
        /// <param name="customerIds"></param>
        /// <returns></returns>
        public JsonResult TransferCutomerToSeas(List<int> customerIds)
        {
            return
                GetServiceJsonResult(
                    () => CustomerSeaService.TransferCutomerToSeas(customerIds, CurrentUserId, CurrentUserDisplayName));
        }

        /// <summary>
        /// 从公海中捕捞客户
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="customerIds"></param>
        /// <returns></returns>
        public JsonResult PickCustomerFromSeas(List<int> ids, List<int> customerIds)
        {
            return
                GetServiceJsonResult(
                    () => CustomerSeaService.PickCustomerFromSeas(ids, customerIds, CurrentUserId, CurrentUserDisplayName));
        }

        #endregion

        #region 客户扩展信息模板
        public ActionResult CustomerExtInfoProviderList()
        {
            return View();
        }

        public JsonResult LoadCustomerExtInfoProviderList(string searchKey = "", string searchValue = "")
        {
            var res = CustomerSeaService.GetAllCustomerExtInfoProviers(searchKey, searchValue);
            return Json(res);
        }

        public PartialViewResult CustomerExtInfoProviderForm(int propertyAttributeId)
        {
            var model = new CustomerExtInfoProviderDTO();
            if (propertyAttributeId != 0)
            {
                model = CustomerSeaService.GetCustomerExtInfoProvidersById(propertyAttributeId);
                ViewBag.DataType = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<AttributeDataType>((int)model.DataType);
            }
            else
            {
                ViewBag.DataType = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<AttributeDataType>();
            }
            return PartialView("_customerExtInfoProviderForm", model);
        }

        public JsonResult SaveCustomerExtInfoProviderForm(CustomerExtInfoProviderDTO model)
        {
            return base.GetServiceJsonResult(() =>
            {
                return CustomerSeaService.SaveCustomerExtInfoProvider(model);
            });
        }

        public JsonResult RemoveCustomerExtInfoProvider(int propertyAttributeId)
        {
            return base.GetServiceJsonResult(() =>
            {
                return CustomerSeaService.SoftRemoveCustomerExtInfoProvider(propertyAttributeId);
            });
        }
        #endregion
    }
}
