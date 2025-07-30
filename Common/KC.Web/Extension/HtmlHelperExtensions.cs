using System;
using KC.Common;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using KC.Framework.Base;

namespace KC.Web.Extension
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent ActionLinkForPermission(this HtmlHelper htmlHelper, string linkText,
            string actionName,
            string controllerName, object routeValues, object htmlAttributes, Tenant tenant,
            string area = null, string applicationId = null)
        {
            if(string.IsNullOrEmpty(applicationId))
            {
                applicationId = GlobalConfig.CurrentApplication.AppId.ToString();
            }

            //var judge = PermissionJudgeUtil.JudgeActionPermision(controllerName, actionName, tenant, container, area, applicationId);
            var judge = true;
            if (!judge)
            {
                if (!string.IsNullOrWhiteSpace(applicationId) && new Guid(applicationId) == ApplicationConstant.PortalAppId)
                {
                    var htmlDictionary = new Dictionary<string, object>();
                    if (htmlAttributes != null)
                    {
                        var type = htmlAttributes.GetType();
                        var properties = type.GetProperties();
                        if (!properties.IsNullOrEmpty())
                        {
                            var classPropertyInfo =
                                properties.FirstOrDefault(m => m.Name.Equals("class", StringComparison.CurrentCultureIgnoreCase));
                            if (classPropertyInfo == null)
                                htmlDictionary.Add("class", "NoAuth");
                            else
                                htmlDictionary.Add("class", classPropertyInfo.GetValue(htmlAttributes) + " NoAuth");
                            foreach (var info in properties.Where(m => !m.Name.Equals("class", StringComparison.CurrentCultureIgnoreCase)))
                                htmlDictionary.Add(info.Name, info.GetValue(htmlAttributes));
                        }
                    }
                    var routeDictionary = new RouteValueDictionary();
                    if (routeValues != null)
                    {
                        var type = routeValues.GetType();
                        var properties = type.GetProperties();
                        var item = new Dictionary<string, object>();
                        if (!properties.IsNullOrEmpty())
                        {
                            foreach (var info in properties)
                                item.Add(info.Name, info.GetValue(routeValues));
                            routeDictionary = new RouteValueDictionary(item);
                        }
                    }
                    return htmlHelper.ActionLink(linkText, actionName, controllerName, routeDictionary, htmlDictionary); 
                }
                return null;
            }
            return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
        }

        public static IHtmlContent DropDownListForEnum<T>(this HtmlHelper htmlHelper, string name, int? selectedValue=null,
            object htmlAttributes = null, string defaultText = null, List<T> exceptEnums = null)
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Text = string.IsNullOrWhiteSpace(defaultText)?"请选择":defaultText,
                Value = string.Empty
            });
            var dict = new Dictionary<int, string>();

            if (typeof(T).IsEnum)
            {
                if(exceptEnums!=null)
                    dict = EnumExtensions.GetEnumDictionary<T>(exceptEnums);
                else
                    dict = EnumExtensions.GetEnumDictionary<T>();
            }

            if (dict.Any())
            {
                if (!selectedValue.HasValue || selectedValue.Value < 0)
                {
                    selectList.AddRange(dict.Select(m => new SelectListItem
                    {
                        Text = m.Value,
                        Value = m.Key.ToString()
                    }).ToList());
                    selectList[0].Selected = true;
                }
                else
                {
                    selectList.AddRange(dict.Select(m => new SelectListItem
                    {
                        Text = m.Value,
                        Value = m.Key.ToString(),
                        Selected = m.Key == selectedValue.Value
                    }).ToList());
                }
            }

            return htmlHelper.DropDownList(name, selectList,
                (HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
        }


    }
}