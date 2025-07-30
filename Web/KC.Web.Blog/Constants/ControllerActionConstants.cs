using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.Blog.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string Blog = "Blog";
    }

    public sealed class ActionName : ActionNameBase
    {
        public sealed class Blog
        {
            public const string LoadBlogList = "LoadBlogList";
            public const string GetBlogForm = "GetBlogForm";
            public const string SaveBlog = "SaveBlog";
            public const string RemoveBlog = "RemoveBlog";

            public const string LoadAllCategories = "LoadAllCategories";
            public const string GetCategoryForm = "GetCategoryForm";
            public const string SaveCategory = "SaveCategory";
            public const string RemoveCategory = "RemoveCategory";

            public const string SaveSettings = "SaveSettings";
        }

        public new sealed class Home
        {
            public const string GetBlogList = "GetBlogList";
            public const string BlogDetail = "BlogDetail";

            public const string SaveComment = "SaveComment";
        }
    }
}