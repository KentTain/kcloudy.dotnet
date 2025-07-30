using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Service.DTO.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using KC.Framework.Exceptions;
using KC.Framework.Base;
using KC.Service.Util;

namespace KC.Web.Account.Controllers
{
    /// <summary>
    /// 三级级菜单：系统管理/组织管理/组织架构管理
    /// </summary>
    /// <returns></returns>
    [Web.Extension.MenuFilter("组织管理", "组织架构管理", "/Organization/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-sitemap", AuthorityId = "4DB22A38-1759-40B2-9926-44FACFA59E68",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 3)]
    public class OrganizationController : AccountBaseController
    {
        public OrganizationController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<OrganizationController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("组织架构管理", "组织架构管理", "/Organization/Index", "4DB22A38-1759-40B2-9926-44FACFA59E68",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            ViewBag.meUserId = CurrentUserId;
            ViewBag.BusinessTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<BusinessType>();
            ViewBag.PositionLevels = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<PositionLevel>(null, null, "请选择岗位类型");
            ViewBag.StatusList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<WorkflowBusStatus>(null, null, "请选择状态");
            return View();
        }

        //[Web.Extension.PermissionFilter("组织架构管理", "加载组织架构列表数据", "/Organization/LoadOrganizationList",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        [AllowAnonymous]
        public IActionResult LoadOrganizationTree(string name, int excludeId, int selectedId, bool hasAll = false, bool hasRoot = true, int maxLevel = 3)
        {
            var result = new List<OrganizationDTO>();
            if (hasAll)
            {
                result.Add(new OrganizationDTO()
                {
                    Id = 0,
                    //TenantType = TenantType.CommercialFactoring,
                    Text = "所有用户",
                    Children = null,
                    Level = 1
                });
                result.Add(new OrganizationDTO()
                {
                    Id = -1,
                    //TenantType = TenantType.CommercialFactoring,
                    Text = "未归属组织的用户",
                    Children = null,
                    Level = 1
                });
            }

            // 系统管理员及人事经理的角色
            if (IsSystemAdmin || CurrentUserRoleIds.Any(m => m == RoleConstants.HrManagerRoleId))
            {
                var data = SysManageService.FindOrgTreesByName(name);
                result.AddRange(data);
            }
            else
            {
                var data = SysManageService.FindOrgTreesByIds(CurrentUserOrgIds, name);
                result.AddRange(data);
            }

            if (hasRoot)
            {
                var rootMenu = new OrganizationDTO() { Text = "顶级组织", Children = result, Level = 0 };
                var org = TreeNodeUtil.GetNeedLevelTreeNodeDTO(rootMenu, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
                return Json(new List<OrganizationDTO> { org });
            }

            var orgList = TreeNodeUtil.LoadNeedLevelTreeNodeDTO(result, maxLevel, new List<int>() { excludeId }, new List<int>() { selectedId });
            return Json(orgList);
        }

        [AllowAnonymous]
        public PartialViewResult GetOrganizationForm(int id = 0, int? parentId = null, string parentName = "顶级组织")
        {
            ViewBag.appid = CurrentApplicationId;
            var model = new OrganizationDTO();
            model.IsEditMode = false;
            model.ParentId = parentId;
            model.ParentName = parentName;
            if (id != 0)
            {
                model = SysManageService.GetOrganizationById(id);
                model.IsEditMode = true;
                model.ParentName = model.ParentName == null ? parentName : model.ParentName;
                ViewBag.BusinessTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<BusinessType>((int)model.BusinessType);
            }
            else
            {
                ViewBag.BusinessTypes = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<BusinessType>();
            }

            return PartialView("_organizationForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("组织架构管理", "保存组织架构", "/Organization/SaveOrganizationForm", "0EBCF1F7-3E2A-4A0E-AF90-36689ABB0AA3",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SaveOrganizationForm(OrganizationDTO model)
        {
            if (model.ParentId == 0)
                model.ParentId = null;
            ModelState.Remove("OrganizationID");
            ModelState.Remove("Status");
            ModelState.Remove("Leaf");
            ModelState.Remove("IsDeleted");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            return GetServiceJsonResult(() =>
            {
                model.Level = 1;
                if (model.ParentId.HasValue && model.ParentId > 0)
                {
                    var parentOrganization = SysManageService.GetOrganizationById(model.ParentId.Value);
                    if (null == parentOrganization)
                    {
                        throw new ArgumentException(string.Format("父级ID:{0} 未找到！", model.ParentId));
                    }
                    if (parentOrganization.Level >= 4)
                    {
                        throw new ArgumentException(string.Format("父级:{0} 不能作为父级！", model.Text));
                    }

                    model.Level = parentOrganization.Level + 1;
                }
                model.CreatedDate = DateTime.UtcNow;
                model.ModifiedDate = DateTime.UtcNow;
                return SysManageService.SaveOrganization(model, CurrentUserId);
            });

        }

        [Web.Extension.PermissionFilter("组织架构管理", "删除组织架构", "/Organization/RemoveOrganization", "93F6DC06-6426-41DF-9DB8-C0FA0758FBB7",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveOrganization(int id)
        {
            return GetServiceJsonResult(() =>
            {
                var organization = SysManageService.GetOrganizationById(id);
                if (null == organization)
                {
                    throw new ArgumentException(string.Format("未找到菜单ID:{0}", id));
                }
                if (organization.Children.Any())
                {
                    throw new ArgumentException(string.Format("菜单ID:{0}存在子菜单，无法删除，请先删除子菜单！", id));
                }
                var organization2 = SysManageService.GetOrganizationsWithUsersByOrgId(id);
                if (organization2.Users.Count > 0)
                {
                    throw new BusinessPromptException(string.Format("存在下属员工，无法删除，请先移除下属员工！"));
                }
                return SysManageService.RemoveOrganization(id);
            });
        }

        [AllowAnonymous]
        public JsonResult ExistOrganizationName(int id, string name, string orginalName, bool isEditMode)
        {
            if (isEditMode && name.Equals(orginalName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(false);
            }

            var tname = SysManageService.ExistOrganizationName(id, name);
            return Json(tname);
        }

    }
}