﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Web.Extension
{
    public class RouteConvention : IApplicationModelConvention
    {
        /// <summary>
        /// 定义一个路由前缀变量
        /// </summary>
        private readonly AttributeRouteModel _centralPrefix;

        /// <summary>
        /// 调用时传入指定的路由前缀
        /// </summary>
        /// <param name="routeTemplateProvider"></param>
        public RouteConvention(IRouteTemplateProvider routeTemplateProvider)
        {
            _centralPrefix = new AttributeRouteModel(routeTemplateProvider);

        }


        /// <summary>
        /// 接口的Apply方法
        /// </summary>
        /// <param name="application"></param>
        public void Apply(ApplicationModel application)
        {
            //遍历所有的 Controller
            foreach (var controller in application.Controllers)
            {

                // 1、已经标记了 RouteAttribute 的 Controller
                // 如果在控制器中已经标注有路由了，则会在路由的前面再添加指定的路由内容。
                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        // 在 当前路由上 再 添加一个 路由前缀
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_centralPrefix, selectorModel.AttributeRouteModel);
                    }
                }

                // 2、没有标记 RouteAttribute 的 Controller
                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (unmatchedSelectors.Any())
                {
                    foreach (var selectorModel in unmatchedSelectors)
                    {
                        // 添加一个 路由前缀
                        selectorModel.AttributeRouteModel = _centralPrefix;

                    }
                }
            }
        }
    }

    public static class MvcOptionsExtensions
    {
        /// <summary>
        /// 扩展方法，使用实例：
        /// services.AddMvc(option => { 
        ///     option.Filters.Add(typeof(MyAuthorizeFilter));
        ///     option.UseCentralRoutePrefix(new RouteAttribute("/api/v1"));//在各个控制器添加前缀（没有特定的路由前面添加前缀）
        ///     //opt.UseCentralRoutePrefix(new RouteAttribute("api/[controller]/[action]"));
        /// });
        /// </summary>
        /// <param name="opts"></param>
        /// <param name="routeAttribute"></param>
        public static void UseCentralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
        {
            // 添加我们自定义 实现IApplicationModelConvention的RouteConvention
            opts.Conventions.Insert(0, new RouteConvention(routeAttribute));
        }
    }

}
