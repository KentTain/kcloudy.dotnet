using KC.Framework.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Web.Util
{
    public static class ViewEngineUtil
    {
        /// <summary>
        /// 使用RazorViewEngine,生成视图页的字符串
        /// </summary>
        /// <param name="viewName">视图名称，例如：Home/Index</param>
        /// <param name="model">传入视图的ViewModel对象</param>
        /// <returns>视图字符串</returns>
        public static async Task<string> RenderToStringAsync(IServiceProvider ServiceProvider, string viewName, object model)
        {
            try
            {
                var httpContext = new DefaultHttpContext { RequestServices = ServiceProvider };
                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

                var razorViewEngine = ServiceProvider.GetService<IRazorViewEngine>();
                var tempDataProvider = ServiceProvider.GetService<ITempDataProvider>();
                using (var sw = new StringWriter())
                {
                    var viewResult = razorViewEngine.FindView(actionContext, viewName, false);

                    if (viewResult.View == null)
                    {
                        throw new ArgumentNullException($"{viewName} does not match any available view");
                    }

                    var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    };

                    var viewContext = new ViewContext(
                        actionContext,
                        viewResult.View,
                        viewDictionary,
                        new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                        sw,
                        new HtmlHelperOptions()
                    );

                    await viewResult.View.RenderAsync(viewContext);
                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("{0} RenderToStringAsync throw exception: {1} StackTrace: {2}", viewName, ex.Message, ex.StackTrace));
                return string.Empty;
            }
        }
    }
}
