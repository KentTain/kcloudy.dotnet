using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using KC.Framework.Tenant;
using KC.Service.Blog;
using KC.Service.DTO.Blog;
using KC.Web.Blog.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using KC.Framework.Base;

namespace KC.Web.Blog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : BlogBaseController
    {
        private IBlogService blogService => ServiceProvider.GetService<IBlogService>();

        public BlogController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<BlogController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        #region 文章管理：Blog

        [Web.Extension.MenuFilter("博客管理", "文章管理", "/Admin/Blog/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "8D394F65-CC55-4589-957F-B05C675ACA98",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsExtPage = false, Level = 2)]
        [Web.Extension.PermissionFilter("文章管理", "文章管理", "/Admin/Blog/Index", "8D394F65-CC55-4589-957F-B05C675ACA98",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Index()
        {
            return View();
        }

        //[Web.Extension.PermissionFilter("文章管理", "加载文章列表", "/Admin/Blog/LoadBlogList", "DE8438E4-E9E8-48FC-AD31-CF8B7E4F6783",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult LoadBlogList(int page, int rows, string title)
        {
            var result = blogService.FindPagenatedBlogs(page, rows, title);
            return Json(result);
        }

        [HttpGet]
        [Web.Extension.MenuFilter("博客管理", "文章保存页面", "/Admin/Blog/GetBlogForm",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "FABAD067-01C1-435C-9429-44C0B2D2B825",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = true, Level = 2)]
        public PartialViewResult GetBlogForm(int id)
        {
            List<SelectListItem> selectItems = new List<SelectListItem>();
            var categories = blogService.GetAllCategories();
            var model = new BlogDTO();
            if (id != 0)
            {
                model = blogService.GetBlogById(id);
                model.IsEditMode = true;
                foreach (var category in categories)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Text = category.Name,
                        Value = category.Id.ToString(),
                        Selected = model.CategoryId == category.Id,
                    });
                }
            }
            else
            {
                var setDefault = true;
                foreach (var category in categories)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Text = category.Name,
                        Value = category.Id.ToString(),
                        Selected = setDefault,
                    });
                    setDefault = false;
                }
            }
            ViewBag.CategoryList = selectItems;

            return PartialView("BlogForm", model);
        }

        [Web.Extension.PermissionFilter("文章管理", "保存文章", "/Admin/Blog/SaveBlog", "686B465A-6163-48E3-A825-6137BB774688",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult SaveBlog(BlogDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                var isTop = Request.Form["IsTop"];
                model.IsTop = !string.IsNullOrEmpty(isTop) ? isTop != "false" : false;
                model.UserId = CurrentUserId;
                model.UserName = CurrentUserDisplayName;
                if (!model.IsEditMode)
                {
                    model.CreateTime = DateTime.UtcNow;
                    model.CreatedName = CurrentUserDisplayName;
                    model.CreatedBy = CurrentUserId;
                    model.CreatedDate = DateTime.UtcNow;
                }
                model.ModifiedName = CurrentUserDisplayName;
                model.ModifiedBy = CurrentUserId;
                model.ModifiedDate = DateTime.UtcNow;

                return blogService.SaveBlog(model);
            });
        }

        [Web.Extension.PermissionFilter("文章管理", "删除文章", "/Admin/Blog/RemoveBlog", "0A333E11-A1F3-4F70-8377-11ECC7EE5D86",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 5, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveBlog(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return blogService.SoftRemoveBlogById(id);
            });
        }

        #endregion

        #region 分类管理：Category
        [Web.Extension.MenuFilter("博客管理", "分类管理", "/Admin/Blog/CategoryList",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "07C172C4-4055-4069-BA0B-B6B880AAE7D7",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 2)]
        [Web.Extension.PermissionFilter("分类管理", "分类管理", "/Admin/Blog/CategoryList", "07C172C4-4055-4069-BA0B-B6B880AAE7D7",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult CategoryList()
        {
            return View();
        }

        //[Web.Extension.PermissionFilter("文章管理", "加载分类列表", "/Admin/Blog/LoadConfigList", "CD036638-3150-4A48-8DB5-07C967426E04",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult LoadAllCategories()
        {
            var result = blogService.GetAllCategories();
            return Json(result);
        }

        public PartialViewResult GetCategoryForm(int id)
        {
            var model = new CategoryDTO();
            if (id != 0)
            {
                model = blogService.GetCategoryById(id);
                model.IsEditMode = true;
            }
            return PartialView("_categoryForm", model);
        }

        [Web.Extension.PermissionFilter("分类管理", "保存分类", "/Admin/Blog/SaveCategory", "BCEDDE0D-96BE-42E0-BA68-539DC6203442",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveCategory(CategoryDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                return blogService.SaveCategory(model);
            });
        }

        [Web.Extension.PermissionFilter("分类管理", "删除分类", "/Admin/Blog/RemoveCategory", "D79CC1FF-4CF0-49FE-8349-9F65BE42751C",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult RemoveCategory(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return blogService.RemoveCategoryById(id);
            });
        }

        #endregion

        #region 系统设置：Setting

        [Web.Extension.MenuFilter("博客管理", "系统设置", "/Admin/Blog/Setting",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "F5121042-E2B2-4532-A9A8-73DD71EB9C70",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 2)]
        [Web.Extension.PermissionFilter("系统设置", "系统设置", "/Admin/Blog/Setting", "F5121042-E2B2-4532-A9A8-73DD71EB9C70",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = true, ResultType = ResultType.ActionResult)]
        public IActionResult Setting()
        {
            var model = blogService.GetPrivateSetting(CurrentUserId, CurrentUserDisplayName);
            return View(model);
        }

        [Web.Extension.PermissionFilter("系统设置", "保存设置", "/Admin/Blog/SaveSettings", "1EAA370D-0EE2-4AD4-92D8-D054871B185D",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public IActionResult SaveSettings(PrivateSettingDTO model)
        {
            return GetServiceJsonResult(() =>
            {
                model.UserId = CurrentUserId;
                model.UserName = CurrentUserDisplayName;

                return blogService.SaveSetting(model);
            });
        }

        #endregion
    }
}
