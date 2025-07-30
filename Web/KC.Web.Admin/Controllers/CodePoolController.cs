using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Service.Admin;
using KC.Service.DTO.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Framework.Base;

namespace KC.Web.Admin.Controllers
{
    [Web.Extension.MenuFilter("资源管理", "代码仓库池管理", "/CodePool/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, 
        SmallIcon = "fa fa-archive", AuthorityId = "6DA3F466-0186-43C0-B804-0E12AD1CDB7D",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 3)]
    public class CodePoolController : AdminBaseController
    {
        protected ICodePoolService CodePoolService => ServiceProvider.GetService<ICodePoolService>();
        public CodePoolController(
            IServiceProvider serviceProvider,
            ILogger<CodePoolController> logger)
            : base(serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("代码仓库池管理", "代码仓库池管理", "/CodePool/Index",
            "6DA3F466-0186-43C0-B804-0E12AD1CDB7D", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 加载表单
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="searchKey"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public IActionResult LoadCodeRepositoryPoolList(int page, int rows, string selectname, string order = "desc")
        {
            string accessName = string.Empty;
            if (!string.IsNullOrWhiteSpace(selectname))
            {
                accessName = selectname;
            }

            var model = CodePoolService.FindCodeRepositoryPoolsByFilter(page, rows, accessName, order);
            return Json(model);
        }

        public IActionResult LoadTenantUserListByCodePool(int codePoolId, int CloudType)
        {
            var result = TenantService.FindTenantUserByCodePoolId(codePoolId);
            return Json(result);
        }
        /// <summary>
        ///  弹窗
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GetCodeRepositoryPoolForm(int id = 0)
        {
            var model = new CodeRepositoryPoolDTO()
            {
                //AccessKeyPasswordHash = EncryptPasswordUtil.EncryptPassword(EncryptPasswordUtil.GetRandomString()),
                IsEditMode = false
            };
            if (id != 0)
            {
                model = CodePoolService.GetCodeRepositoryPoolById(id);
                model.IsEditMode = true;
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>((int)model.CloudType);
                ViewBag.CodeTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CodeType>((int)model.CodeType);
            }
            else
            {
                ViewBag.CloudTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CloudType>();
                ViewBag.CodeTypeList = KC.Web.Util.SelectItemsUtil.GetSelectItemsByEnum<CodeType>();
            }
            return PartialView("_codePoolForm", model);
        }

        /// <summary>
        /// 添加/新增队列链接
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Web.Extension.PermissionFilter("代码仓库池管理", "保存代码仓库连接", "/CodePool/SaveCodeRepositoryPoolForm",
            "B850B854-4761-4990-A17B-0916375A4AD0", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult SaveCodeRepositoryPoolForm(CodeRepositoryPoolDTO model)
        {
            return GetServiceJsonResult(() =>
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
                return CodePoolService.SaveCodeRepositoryPool(model);
            });
        }

        /// <summary>
        /// 删除队列链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("代码仓库池管理", "删除代码仓库连接", "/CodePool/RemoveCodeRepositoryPool",
            "15DAE1C9-024E-4998-A467-E664018EC28F", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveCodeRepositoryPool(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return CodePoolService.RemoveCodeRepositoryPool(id);
            });
        }

        [Web.Extension.PermissionFilter("代码仓库池管理", "测试代码仓库连接", "/CodePool/TestCodePoolConnection",
            "FD1F35C3-4F35-43BD-B59A-B97E6A388F10", DefaultRoleId = RoleConstants.AdminRoleId, 
            Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult TestCodePoolConnection(CodeRepositoryPoolDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                return CodePoolService.TestCodeRepositoryPoolConnection(model);
            });
        }
    }
}