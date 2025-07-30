using KC.Enums.Portal;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.DTO.Portal;
using KC.Service.Portal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace KC.Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RecommendInfoController : Portal.Controllers.PortalBaseController
    {
        protected IRecommendService _recommendService => ServiceProvider.GetService<IRecommendService>();
        
        public RecommendInfoController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<RecommendInfoController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 三级菜单：门户管理/门户推荐管理/推荐企业信息

        /// <summary>
        /// 三级菜单：门户管理/门户推荐管理/推荐企业信息
        /// </summary>
        [Web.Extension.MenuFilter("门户推荐管理", "推荐企业信息", "/Admin/RecommendInfo/RedCustomerList",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "B4874F5E-D933-4404-ACD4-93DB0D026C3F",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("推荐企业信息", "推荐企业信息", "/Admin/RecommendInfo/RedCustomerList", "B4874F5E-D933-4404-ACD4-93DB0D026C3F",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 21, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult RedCustomerList()
        {
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<RecommendStatus>();
            return View();
        }

        public IActionResult LoadRedCustomerList(int page, int rows, string name, RecommendStatus? status)
        {
            var result = _recommendService.FindPaginatedRecommendCustomers(page, rows, name, status);
            return Json(result);
        }

        [Web.Extension.PermissionFilter("推荐企业信息", "审核推荐企业申请", "/Admin/RecommendInfo/AuditRedCustomer", "0EF1FFBD-1D65-4F2B-AF78-5A9846315D00",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult AuditRedCustomer(int id, bool isApprove, string comment)
        {
            return GetServiceJsonResult(() =>
            {
                return _recommendService.AuditRedCustomerById(id, isApprove, comment, CurrentUserId, CurrentUserDisplayName);
            });
        }
        [Web.Extension.PermissionFilter("推荐企业信息", "下架推荐企业", "/Admin/RecommendInfo/TakeOffRedCustomer", "301FA9BF-E23D-4AF0-AFA2-4FAB68EF0E4E",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult TakeOffRedCustomer(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return _recommendService.TakeOffRedCustomerById(id, CurrentUserId, CurrentUserDisplayName);
            });
        }
        [Web.Extension.PermissionFilter("推荐企业信息", "删除推荐企业", "/Admin/RecommendInfo/RemoveRedCustomer", "DC0F94EC-3658-4E89-B44F-AB66B2A10DF6",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveRedCustomer(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return _recommendService.RemoveRedCustomerById(id, CurrentUserId, CurrentUserDisplayName);
            });
        }


        /// <summary>
        /// 三级菜单：门户管理/门户推荐管理/新增/编辑推荐企业
        /// </summary>
        [Web.Extension.MenuFilter("门户推荐管理", "新增/编辑推荐企业", "/Admin/RecommendInfo/GetRedCustomerForm",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "6F7FD3F4-CAD5-42FF-8705-3C732B62A535",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("新增/编辑推荐企业", "新增/编辑推荐企业", "/Admin/RecommendInfo/GetRedCustomerForm", "6F7FD3F4-CAD5-42FF-8705-3C732B62A535",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 22, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult GetRedCustomerForm(int id)
        {
            var model = _recommendService.GetRedCustomerById(id);
            if (model == null)
            {
                model = new RecommendCustomerDTO();
                model.IsEditMode = false;
            }
            else
            {
                model.IsEditMode = true;
            }

            ViewBag.CompanyTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CompanyType>((int)model.CompanyType);
            ViewBag.BusinessModels = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<Framework.Base.BusinessModel>((int)model.BusinessModel);

            return PartialView("RedCustomerForm", model);
        }

        [Web.Extension.PermissionFilter("新增/编辑推荐企业", "保存推荐企业", "/Admin/RecommendInfo/SaveRedCustomer", "A7C8FDF6-8EC7-4F92-93DF-7E8383774F73",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult SaveRedCustomer(RecommendCustomerDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                if (!model.IsEditMode)
                {
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedBy = CurrentUserId;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedBy = CurrentUserId;
                model.ModifiedDate = DateTime.UtcNow;

                return _recommendService.SaveRedCustomer(model);
            });
        }

        #endregion

        #region 三级菜单：门户管理/门户推荐管理/推荐商品信息

        /// <summary>
        /// 三级菜单：门户管理/门户推荐管理/推荐商品信息
        /// </summary>
        [Web.Extension.MenuFilter("门户推荐管理", "推荐商品信息", "/Admin/RecommendInfo/RedOfferingList",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "718F1F70-C159-4C80-A6B9-512596F3C8F5",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("推荐商品信息", "推荐商品信息", "/Admin/RecommendInfo/RedOfferingList", "718F1F70-C159-4C80-A6B9-512596F3C8F5",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 22, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult RedOfferingList()
        {
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<RecommendStatus>();
            return View();
        }

        public IActionResult LoadRedOfferingList(int page, int rows, string name, RecommendStatus? status)
        {
            var result = _recommendService.FindPaginatedRecommendOfferings(page, rows, name, status);
            return Json(result);
        }

        [Web.Extension.PermissionFilter("推荐商品信息", "审核推荐商品申请", "/Admin/RecommendInfo/AuditRedOffering", "314B8F26-F1D5-4EF7-805E-43843CC22C81",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult AuditRedOffering(int id, bool isApprove, string comment)
        {
            return GetServiceJsonResult(() =>
            {
                return _recommendService.AuditRedOfferingById(id, isApprove, comment, CurrentUserId, CurrentUserDisplayName);
            });
        }
        [Web.Extension.PermissionFilter("推荐商品信息", "下架推荐商品", "/Admin/RecommendInfo/TakeOffRedOffering", "BFEDE26C-3850-4DC2-A0D6-87301E4ADB92",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult TakeOffRedOffering(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return _recommendService.TakeOffRedOfferingById(id, CurrentUserId, CurrentUserDisplayName);
            });
        }
        [Web.Extension.PermissionFilter("推荐商品信息", "删除推荐商品", "/Admin/RecommendInfo/RemoveRedOffering", "D4FE2828-1B67-43A0-8B8D-5B50075827EA",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveRedOffering(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return _recommendService.RemoveRedOfferingById(id, CurrentUserId, CurrentUserDisplayName);
            });
        }
        /// <summary>
        /// 三级菜单：门户管理/门户推荐管理/新增/编辑推荐商品
        /// </summary>
        [Web.Extension.MenuFilter("门户推荐管理", "新增/编辑推荐商品", "/Admin/RecommendInfo/GetRedOfferingForm",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "97F3B450-DDD0-4D1F-8482-6A491BCBAF9D",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("新增/编辑推荐商品", "新增/编辑推荐商品", "/Admin/RecommendInfo/GetRedOfferingForm", "97F3B450-DDD0-4D1F-8482-6A491BCBAF9D",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 22, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult GetRedOfferingForm(int id)
        {
            var model = _recommendService.GetRedOfferingById(id);
            if (model == null)
            {
                model = new RecommendOfferingDTO();
                model.IsEditMode = false;
            }
            else
            {
                model.IsEditMode = true;
            }

            ViewBag.OfferingTypeList = KC.Web.Util.SelectItemsUtil.GetDictValueSelectItemsByTypeCode(ServiceProvider, "DCT2021010100001", model.OfferingTypeCode);
            //ViewBag.BusinessModels = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<Framework.Base.BusinessModel>((int)model.BusinessModel);

            return PartialView("RedOfferingForm", model);
        }

        [Web.Extension.PermissionFilter("新增/编辑推荐商品", "保存推荐商品", "/Admin/RecommendInfo/SaveRedOffering", "B6E24D0D-D7A3-4FCD-9D16-A7C13F700F57",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult SaveRedOffering(RecommendOfferingDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                if (!model.IsEditMode)
                {
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedBy = CurrentUserId;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedBy = CurrentUserId;
                model.ModifiedDate = DateTime.UtcNow;

                return _recommendService.SaveRedOffering(model);
            });
        }

        #endregion

        #region 三级菜单：门户管理/门户推荐管理/推荐采购需求

        /// <summary>
        /// 三级菜单：门户管理/门户推荐管理/推荐采购需求
        /// </summary>
        [Web.Extension.MenuFilter("门户推荐管理", "推荐采购需求", "/Admin/RecommendInfo/RedRequirementList",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "27C6F2E9-85FB-4B15-ABEF-DFC22FA2972F",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("推荐采购需求", "推荐采购需求", "/Admin/RecommendInfo/RedRequirementList", "27C6F2E9-85FB-4B15-ABEF-DFC22FA2972F",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 23, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult RedRequirementList()
        {
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<RecommendStatus>();
            return View();
        }

        public IActionResult LoadRedRequirementList(int page, int rows, string name, RecommendStatus? status)
        {
            var result = _recommendService.FindPaginatedRecommendRequirements(page, rows, name, status);
            return Json(result);
        }

        [Web.Extension.PermissionFilter("推荐采购需求", "审核推荐采购需求申请", "/Admin/RecommendInfo/AuditRedRequirement", "5E092E5A-50B9-4488-B39B-58DB0B6C37A1",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult AuditRedRequirement(int id, bool isApprove, string comment)
        {
            return GetServiceJsonResult(() =>
            {
                return _recommendService.AuditRedRequirementById(id, isApprove, comment, CurrentUserId, CurrentUserDisplayName);
            });
        }
        [Web.Extension.PermissionFilter("推荐采购需求", "下架推荐采购需求", "/Admin/RecommendInfo/TakeOffRedRequirement", "57DC4756-2E0E-4E20-B1E3-E9A67ED5E240",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult TakeOffRedRequirement(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return _recommendService.TaskOffRedRequirementById(id, CurrentUserId, CurrentUserDisplayName);
            });
        }

        [Web.Extension.PermissionFilter("推荐采购需求", "删除推荐采购需求", "/Admin/RecommendInfo/RemoveRedRequirement", "983DD57F-F886-43C9-9169-5CAA98A48777",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveRedRequirement(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return _recommendService.RemoveRedRequirementById(id, CurrentUserId, CurrentUserDisplayName);
            });
        }

        /// <summary>
        /// 三级菜单：门户管理/门户推荐管理/新增/编辑推荐采购需求
        /// </summary>
        [Web.Extension.MenuFilter("门户推荐管理", "新增/编辑推荐采购需求", "/Admin/RecommendInfo/GetRedRequirementForm",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "0639A723-9C9B-49FE-9CAF-BC6EB5B79656",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsExtPage = true, Level = 3)]
        [Web.Extension.PermissionFilter("新增/编辑推荐采购需求", "新增/编辑推荐采购需求", "/Admin/RecommendInfo/GetRedRequirementForm", "0639A723-9C9B-49FE-9CAF-BC6EB5B79656",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 22, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult GetRedRequirementForm(int id)
        {
            var model = _recommendService.GetRedRequirementById(id);
            if (model == null)
            {
                model = new RecommendRequirementDTO();
                model.IsEditMode = false;
            }
            else
            {
                model.IsEditMode = true;
            }

            ViewBag.RequirementTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<RequirementType>((int)model.RequirementType);
            ViewBag.ProvinceList = KC.Web.Util.SelectItemsUtil.GetProvinceSelectItems(ServiceProvider, model.ProvinceId);

            return PartialView("RedRequirementForm", model);
        }

        [Web.Extension.PermissionFilter("新增/编辑推荐采购需求", "保存推荐采购需求", "/Admin/RecommendInfo/SaveRedRequirement", "B5193678-9A6D-4E65-9D9C-7896AC19D58A",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult SaveRedRequirement(RecommendRequirementDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                if (!model.IsEditMode)
                {
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedBy = CurrentUserId;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedBy = CurrentUserId;
                model.ModifiedDate = DateTime.UtcNow;

                return _recommendService.SaveRedRequirement(model);
            });
        }



        #endregion
    }
}
