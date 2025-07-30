using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

using KC.Framework.Tenant;
using KC.Web.Blog.Models;
using KC.Service.Blog;
using KC.Web.Config.Models;
using KC.Service.DTO.Blog;

namespace KC.Web.Blog.Controllers
{
    public class HomeController : Web.Controllers.TenantWebBaseController
    {
        private IFrontBlogService blogService => ServiceProvider.GetService<IFrontBlogService>();

        public HomeController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<HomeController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        public IActionResult Index()
        {
            ViewBag.LayoutModel = GetHomeVM();
            return View();
        }

        public IActionResult GetBlogList(int page = 1, int rows = 10, int categoryId = 0, string tag = null, string title = null)
        {
            var result = blogService.FindPagenatedBlogs(page, rows, categoryId, tag, title);
            return Json(result);
        }

        public IActionResult BlogDetail(int id)
        {
            ViewBag.LayoutModel = GetHomeVM();
            var model = blogService.GetBlogDetailById(id);
            return View(model);
        }

        public IActionResult SaveComment(int blogId, string name, string content)
        {
            return GetServiceJsonResult(() =>
            {
                var comment = new CommentDTO();
                comment.BlogId = blogId;
                comment.NickName = name;
                comment.Content = content;
                comment.CreateTime = DateTime.UtcNow;
                return blogService.SaveComment(comment);
            });
        }

        private HomeViewModel GetHomeVM()
        {
            var model = new HomeViewModel();
            model.Title = blogService.GetTitle();
            model.AboutMe = blogService.GetAboutMe();
            model.Tags = blogService.GetAllTags();
            model.TopBlogs = blogService.GetTopBlogTitles();
            model.NewBlogs = blogService.GetNewBlogTitles();
            model.Categories = blogService.GetCategoryNames();

            return model;
        }

        #region 辅助方法
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult DownLoadLogFile(string name)
        {
            var url = ServerPath + "/logs/" + name + ".log";
            return File(url, "text/plain");
        }
        #endregion
    }
}
