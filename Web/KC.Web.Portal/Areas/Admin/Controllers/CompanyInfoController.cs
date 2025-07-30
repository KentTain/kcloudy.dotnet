using KC.Enums.Portal;
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.DTO.Portal;
using KC.Service.Portal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KC.Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyInfoController : Portal.Controllers.PortalBaseController
    {
        private ICompanyInfoService _companyInfoService => ServiceProvider.GetService<ICompanyInfoService>();

        public CompanyInfoController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<CompanyInfoController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 三级菜单：门户管理/企业信息管理/企业基本信息
        [Web.Extension.MenuFilter("企业信息管理", "企业基本信息", "/Admin/CompanyInfo/CompanyBasicInfo",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-database", AuthorityId = "A0A0055F-D307-4E62-A8D2-4CCD44E47093",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("企业基本信息", "企业基本信息", "/Admin/CompanyInfo/CompanyBasicInfo", "A0A0055F-D307-4E62-A8D2-4CCD44E47093",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> CompanyBasicInfo()
        {
            var data = await _companyInfoService.GetCompanyInfoAsync();
            if (data == null)
            {
                data = new CompanyInfoDTO
                {
                    CompanyCode = TenantName,
                    CompanyName = TenantDisplayName,
                    IsEditMode = false,
                    CreatedBy = CurrentUserId,
                    CreatedName = CurrentUserDisplayName,
                    CreatedDate = DateTime.UtcNow,
                };

                var success = await _companyInfoService.SaveCompanyInfo(data, CurrentUserId, CurrentUserDisplayName);
                if (success)
                    data.IsEditMode = true;

                ViewBag.BusinessModels = IsEnterprise
                    ? KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<Framework.Base.BusinessModel>() { Framework.Base.BusinessModel.SupplyChainFinance, Framework.Base.BusinessModel.CommercialFactoring, Framework.Base.BusinessModel.FinanceLease, Framework.Base.BusinessModel.SmallLoan }) 
                    : KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum(null, new List<Framework.Base.BusinessModel>() { Framework.Base.BusinessModel.ProductionFoundry, Framework.Base.BusinessModel.DesignAndSales, Framework.Base.BusinessModel.ProductionAdnSales, Framework.Base.BusinessModel.DesignAndProductionAndSales, Framework.Base.BusinessModel.InformationService });
            }
            else
            {
                data.IsEditMode = true;
                ViewBag.BusinessModels = IsEnterprise
                    ? KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum((int)data.BusinessModel, new List<Framework.Base.BusinessModel>() { Framework.Base.BusinessModel.SupplyChainFinance, Framework.Base.BusinessModel.CommercialFactoring, Framework.Base.BusinessModel.FinanceLease, Framework.Base.BusinessModel.SmallLoan })
                    : KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum((int)data.BusinessModel, new List<Framework.Base.BusinessModel>() { Framework.Base.BusinessModel.ProductionFoundry, Framework.Base.BusinessModel.DesignAndSales, Framework.Base.BusinessModel.ProductionAdnSales, Framework.Base.BusinessModel.DesignAndProductionAndSales, Framework.Base.BusinessModel.InformationService });
            }

            return View(data);
        }

        [HttpPost]
        [Web.Extension.PermissionFilter("企业基本信息", "保存基本信息", "/Admin/CompanyInfo/SaveCompanyInfo", "8225119B-3AF4-47D4-8537-9D1C012CF5CC",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<JsonResult> SaveCompanyInfo(CompanyInfoDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                //model.BusinessModel = Request.Form["BusinessModel"];

                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                return await _companyInfoService.SaveCompanyInfo(model, CurrentUserId, CurrentUserDisplayName);
            });

        }

        #endregion

        #region 三级菜单：门户管理/企业信息管理/企业认证信息

        /// <summary>
        /// 三级菜单：门户管理/企业信息管理/企业认证信息
        /// </summary>
        [Web.Extension.MenuFilter("企业信息管理", "企业认证信息", "/Admin/CompanyInfo/Authentication",
            Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType,
            SmallIcon = "fa fa-file-code-o", AuthorityId = "D5177193-7148-4E75-B048-7FDDAF5880D3",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("企业认证信息", "企业认证信息", "/Admin/CompanyInfo/Authentication",
            "D5177193-7148-4E75-B048-7FDDAF5880D3", DefaultRoleId = RoleConstants.OperatorManagerRoleId,
            Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public async Task<IActionResult> Authentication()
        {
            var model = await _companyInfoService.GetCompanyAuthAsync();
            if (model == null)
            {
                model = new CompanyAuthenticationDTO
                {
                    CompanyCode = TenantName,
                    CompanyName = TenantDisplayName,
                    IsEditMode = false
                };
            }
            else
            {
                model.IsEditMode = true;
            }
            ViewBag.ProvinceList = KC.Web.Util.SelectItemsUtil.GetProvinceSelectItems(ServiceProvider, model.ProvinceId);

            return View(model);
        }


        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("企业认证信息", "保存会员认证信息", "/Admin/CompanyInfo/SaveComAuthentication",
            "AACCC04F-480F-4726-A32F-E3F4E5A7B5F4", DefaultRoleId = RoleConstants.OperatorManagerRoleId,
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<JsonResult> SaveComAuthentication(CompanyAuthenticationDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    var errors = GetModelErrors();
                    throw new ArgumentException(errors);
                }


                if (model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                return await _companyInfoService.SaveComAuthentication(model);
            });
        }

        #endregion

        #region 三级菜单：门户管理/企业信息管理/企业对外联系人
        /// <summary>
        /// 三级菜单：门户管理/企业信息管理/企业对外联系人
        /// </summary>
        [Web.Extension.MenuFilter("企业信息管理", "企业对外联系人", "/Admin/CompanyInfo/CompanyContactList",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-database", AuthorityId = "4FB4D065-C1F2-4FE2-BC6C-261BF77732A8",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("企业对外联系人", "企业对外联系人", "/Admin/CompanyInfo/CompanyContactList", "4FB4D065-C1F2-4FE2-BC6C-261BF77732A8",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult CompanyContactList()
        {
            ViewBag.SelectUserIds = _companyInfoService.LoadAllContactUserIds().ToCommaSeparatedString();
            ViewBag.BusinessTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll(new List<BusinessType>() { BusinessType.None });
            return View();
        }

        public async Task<IActionResult> LoadCompanyContactList(int page, int rows, string name, string phone, BusinessType? type)
        {
            var result = await _companyInfoService.GetPaginatedCompanyContactsByFilterAsync(page, rows, name, phone, type);

            return Json(result);
        }

        public async Task<PartialViewResult> GetCompanyContactForm(int id = 0)
        {
            var model = new CompanyContactDTO()
            {
                IsEditMode = false,
                CompanyCode = TenantName,
                CompanyName = TenantDisplayName,
            };
            if (id != 0)
            {
                model = await _companyInfoService.GetCompanyContactByIdAsync(id);
                model.IsEditMode = true;
            }
            ViewBag.BusinessTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum((int)model.BusinessType, new List<BusinessType>() { BusinessType.None });
            return PartialView("_companyContactForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("企业对外联系人", "保存企业联系人", "/Admin/CompanyInfo/SaveCompanyContact", "15BE64EF-5787-4A23-A353-1C6228048ADE",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> SaveCompanyContact(CompanyContactDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;

                return await _companyInfoService.SaveCompanyContactAsync(model);
            });
        }

        [HttpPost]
        public async Task<IActionResult> SaveCompanyContacts([FromBody]List<CompanyContactDTO> models)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                models.ForEach(model =>
                {
                    if (!model.IsEditMode)
                    {
                        model.CreatedBy = CurrentUserId;
                        model.CreatedName = CurrentUserDisplayName;
                        model.CreatedDate = DateTime.UtcNow;
                    }
                    model.ModifiedBy = CurrentUserId;
                    model.ModifiedName = CurrentUserDisplayName;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.CompanyCode = TenantName;
                    model.CompanyName = TenantDisplayName;
                });

                return await _companyInfoService.SaveCompanyContactsAsync(models);
            });
        }

        [Web.Extension.PermissionFilter("企业对外联系人", "删除企业联系人", "/Admin/CompanyInfo/RemoveCompanyContact", "B0CA0F0E-FC09-468D-978E-7C9BA47F1E35",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveCompanyContact(int id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await _companyInfoService.SoftRemoveCompanyContactAsync(id);
            });
        }
        #endregion

        #region 三级菜单：门户管理/企业信息管理/企业常用地址
        /// <summary>
        /// 三级菜单：门户管理/企业信息管理/企业常用地址
        /// </summary>
        [Web.Extension.MenuFilter("企业信息管理", "企业常用地址", "/Admin/CompanyInfo/CompanyAddressList",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-database", AuthorityId = "DA8FBC7C-4E87-4B87-B4EF-915967048036",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 4, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("企业常用地址", "企业常用地址", "/Admin/CompanyInfo/CompanyAddressList", "DA8FBC7C-4E87-4B87-B4EF-915967048036",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 4, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult CompanyAddressList()
        {
            ViewBag.AddressTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<AddressType>();
            return View();
        }

        public async Task<IActionResult> LoadCompanyAddressList(int page, int rows, string name, string bankNumber, AddressType? type)
        {
            var result = await _companyInfoService.GetPaginatedCompanyAddresssByFilterAsync(page, rows, name, bankNumber, type);

            return Json(result);
        }

        public async Task<PartialViewResult> GetCompanyAddressForm(int id = 0)
        {
            var model = new CompanyAddressDTO()
            {
                IsEditMode = false,
                CompanyCode = TenantName,
                CompanyName = TenantDisplayName,
            };
            if (id != 0)
            {
                model = await _companyInfoService.GetCompanyAddressByIdAsync(id);
                model.IsEditMode = true;
            }

            ViewBag.ProvinceList = KC.Web.Util.SelectItemsUtil.GetProvinceSelectItems(ServiceProvider, model.ProvinceId); 
            ViewBag.AddressTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<AddressType>((int)model.AddressType);
            return PartialView("_companyAddressForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("企业常用地址", "保存企业地址", "/Admin/CompanyInfo/SaveCompanyAddressForm", "ECE944E7-4805-4432-98DC-19B8F60B3B52",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> SaveCompanyAddressForm(CompanyAddressDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;
                model.CompanyCode = TenantName;
                model.CompanyName = TenantDisplayName;

                return await _companyInfoService.SaveCompanyAddressAsync(model);
            });
        }

        [Web.Extension.PermissionFilter("企业常用地址", "删除企业地址", "/Admin/CompanyInfo/RemoveCompanyAddress", "491EA0FC-1702-467D-A264-449737A65D13",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "491EA0FC-1702-467D-A264-449737A65D13")]
        public async Task<IActionResult> RemoveCompanyAddress(int id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await _companyInfoService.SoftRemoveCompanyAddressAsync(id, CurrentUserId, CurrentUserDisplayName);
            });
        }
        #endregion

        #region 三级菜单：门户管理/企业信息管理/企业银行账户
        /// <summary>
        /// 三级菜单：门户管理/企业信息管理/企业银行账户
        /// </summary>
        [Web.Extension.MenuFilter("企业信息管理", "企业银行账户", "/Admin/CompanyInfo/CompanyAccountList",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-database", AuthorityId = "D5A484BA-2708-4F55-9F5E-C6775BDD4150",
        DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 5, IsExtPage = false, Level = 3)]
        [Web.Extension.PermissionFilter("企业银行账户", "企业银行账户", "/Admin/CompanyInfo/CompanyAccountList", "D5A484BA-2708-4F55-9F5E-C6775BDD4150",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 5, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult CompanyAccountList()
        {
            ViewBag.BankAccountTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnumWithAll<BankAccountType>();
            return View();
        }

        
        public async Task<IActionResult> LoadCompanyAccountList(int page, int rows, string name, string bankNumber, BankAccountType? type)
        {
            var result = await _companyInfoService.GetPaginatedCompanyAccountsByFilterAsync(page, rows, name, bankNumber, type);

            return Json(result);
        }

        public async Task<PartialViewResult> GetCompanyAccountForm(int id = 0)
        {
            var model = new CompanyAccountDTO()
            {
                IsEditMode = false,
                CompanyCode = TenantName,
                CompanyName = TenantDisplayName,
            };
            if (id != 0)
            {
                model = await _companyInfoService.GetCompanyAccountByIdAsync(id);
                model.IsEditMode = true;
            }

            ViewBag.ProvinceList = KC.Web.Util.SelectItemsUtil.GetProvinceSelectItems(ServiceProvider, model.ProvinceId);
            ViewBag.BankAccountTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum((int)model.BankType, new List<BankAccountType>() { BankAccountType.Other });
            return PartialView("_companyAccountForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("企业银行账户", "保存企业银行账户", "/Admin/CompanyInfo/SaveCompanyAccountForm", "47D69147-7F33-4677-8038-34608CDC1516",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> SaveCompanyAccountForm(CompanyAccountDTO model)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                if (!model.IsEditMode)
                {
                    model.CreatedBy = CurrentUserId;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedBy = CurrentUserId;
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedDate = DateTime.UtcNow;
                model.CompanyCode = TenantName;
                model.CompanyName = TenantDisplayName;

                return await _companyInfoService.SaveCompanyAccountAsync(model, CurrentUserId, CurrentUserDisplayName);
            });
        }

        [Web.Extension.PermissionFilter("企业银行账户", "删除企业银行账户", "/Admin/CompanyInfo/RemoveCompanyAccount", "22B8D7F1-4B6E-4AF4-8F1A-743A6D079345",
            DefaultRoleId = RoleConstants.OperatorManagerRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public async Task<IActionResult> RemoveCompanyAccount(int id)
        {
            return await GetServiceJsonResultAsync(async () =>
            {
                return await _companyInfoService.SoftRemoveCompanyAccountAsync(id, CurrentUserId, CurrentUserDisplayName);
            });
        }
        #endregion
    }
}