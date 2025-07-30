using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Framework.Util;
using KC.Service.DTO.Dict;
using KC.Service.WebApiService.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace KC.Web.Util
{
    public static class SelectItemsUtil
    {
        #region 根据Enum对象

        /// <summary>
        /// 根据Enum对象，获取DropDownItems所需的数据
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="selectedEnumValues">选中的Enum对象值</param>
        /// <param name="excludeEnums">需要排除的Enum类型对象</param>
        /// <param name="firstItemText">自定义首选项，值为空时没首选项</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetSelectItemsByEnum<T>(List<int> selectedEnumValues = null, List<T> excludeEnums = null, string firstItemText = null)
        {
            var result = new List<SelectListItem>();
            if (!firstItemText.IsNullOrEmpty())
                result.Add(new SelectListItem(firstItemText, "",
                    selectedEnumValues == null || !selectedEnumValues.Any()
                        ? true : false));
            var enumList = excludeEnums != null
                    ? EnumExtensions.GetEnumDictionary<T>(excludeEnums)
                    : EnumExtensions.GetEnumDictionary<T>();
            result.AddRange(
                    enumList.Select(
                        item => new SelectListItem
                        {
                            Text = item.Value,
                            Value = item.Key.ToString(),
                            Selected = selectedEnumValues != null && selectedEnumValues.Contains(item.Key),
                        }));

            return result;
        }

        /// <summary>
        /// 根据Enum对象，获取DropDownItems所需的数据
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="selectedEnumValue">选中的Enum对象值</param>
        /// <param name="excludeEnums">需要排除的Enum类型对象</param>
        /// <param name="firstItemText">自定义首选项，值为空时没首选项</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetSelectItemsByEnum<T>(int selectedEnumValue, List<T> excludeEnums = null, string firstItemText = null)
        {
            return GetSelectItemsByEnum(new List<int>() { selectedEnumValue }, excludeEnums, firstItemText);
        }

        /// <summary>
        /// 根据Enum对象，获取DropDownItems所需的数据（添加自定义选项，例如：请选择）
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="excludeEnums">需要排除的Enum类型对象</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetSelectItemsByEnumWithAll<T>(List<T> excludeEnums = null)
        {
            var allText = "请选择";
            return GetSelectItemsByEnum(null, excludeEnums, allText);
        }
        #endregion

        #region 字典值对象（DictValueDTO）
        /// <summary>
        /// 根据字典值对象（DictValueDTO）列表，获取DropDownItems所需的数据
        /// </summary>
        /// <param name="selectedDictCodes">选中的字典值对象值列表</param>
        /// <param name="dictValueList">需要转换的字典值类型对象列表</param>
        /// <param name="firstItemText">自定义首选项，值为空时没首选项</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetSelectItemsByDicts(List<string> selectedDictCodes, List<DictValueDTO> dictValueList, string firstItemText = null)
        {
            var result = new List<SelectListItem>();
            if (dictValueList == null || !dictValueList.Any())
                return result;

            if (!firstItemText.IsNullOrEmpty())
                result.Add(new SelectListItem(firstItemText, "", false));

            foreach (var item in dictValueList)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Code,
                    Selected = selectedDictCodes != null && selectedDictCodes.Contains(item.Code)
                });
            }
            return result;
        }

        /// <summary>
        /// 根据字典值对象（DictValueDTO）列表，获取DropDownItems所需的数据
        /// </summary>
        /// <param name="selectedDictCode">选中的字典值对象值</param>
        /// <param name="dictValueList">需要转换的字典值类型对象列表</param>
        /// <param name="firstItemText">自定义首选项，值为空时没首选项</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetSelectItemsByDicts(string selectedDictCode, List<DictValueDTO> dictValueList, string firstItemText = null)
        {
            return string.IsNullOrEmpty(selectedDictCode)
                ? GetSelectItemsByDicts(new List<string>(), dictValueList, firstItemText)
                : GetSelectItemsByDicts(new List<string>() { selectedDictCode }, dictValueList, firstItemText);
        }

        /// <summary>
        /// 根据字典值对象（DictValueDTO）列表 </br>
        ///     获取DropDownItems所需的数据（添加自定义选项，例如：请选择）
        /// </summary>
        /// <typeparam name="T">Enum类型</typeparam>
        /// <param name="excludeEnums">需要排除的Enum类型对象</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetSelectItemsByDictsWithAll(List<DictValueDTO> dictValueList)
        {
            var allText = "请选择";
            return GetSelectItemsByDicts("", dictValueList, allText);
        }

        /// <summary>
        /// 根据字典类型编码，根据字典值对象（DictValueDTO）列表
        /// </summary>
        /// <param name="dicTypeCode">字典类型编码</param>
        /// <param name="selectedDictCode">选中的字典值对象值</param>
        /// <param name="firstItemText">自定义首选项，值为空时没首选项</param>
        /// <returns></returns>
        public static List<SelectListItem> GetDictValueSelectItemsByTypeCode(IServiceProvider serviceProvider, string dicTypeCode, string selectedDictCode, string firstItemText = null)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var list = new List<SelectListItem>();
                if (string.IsNullOrEmpty(dicTypeCode))
                    return list;

                var service = scope.ServiceProvider.GetRequiredService<IDictionaryApiService>();
                var results = service.LoadAllDictValuesByTypeCode(dicTypeCode);
                if (results == null || !results.Any())
                    return list;

                if (!firstItemText.IsNullOrEmpty())
                    list.Add(new SelectListItem(firstItemText, "", false));

                list.AddRange(results.Select(item => new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Code,
                    Selected = !string.IsNullOrEmpty(selectedDictCode) && selectedDictCode.Equals(item.Code),
                }));

                return list;
            }
        }
        #endregion

        #region 省份、城市
        /// <summary>
        /// 获取省份列表数据
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetProvinceSelectItems(IServiceProvider serviceProvider, int? selectId)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IDictionaryApiService>();
                var results = service.LoadAllProvinces();
                var list = new List<SelectListItem>();
                if (results == null || !results.Any())
                    return list;

                list.Add(new SelectListItem()
                {
                    Text = "请选择省",
                    Value = "",
                });

                list.AddRange(results.Select(model => new SelectListItem
                {
                    Text = model.Name,
                    Value = model.ProvinceId.ToString(),
                    Selected = selectId.HasValue && selectId.Value == model.ProvinceId,
                }));

                return list;
            }
        }

        /// <summary>
        /// 获取城市列表数据
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetCitySelectItems(IServiceProvider serviceProvider, int? selectId)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IDictionaryApiService>();
                var results = service.LoadAllCities();
                var list = new List<SelectListItem>();
                if (results == null || !results.Any())
                    return list;

                list.Add(new SelectListItem()
                {
                    Text = "请选择市",
                    Value = "",
                });

                list.AddRange(results.Select(model => new SelectListItem
                {
                    Text = model.Name,
                    Value = model.Id.ToString(),
                    Selected = selectId.HasValue && selectId.Value == model.Id,
                }));

                return list;
            }
        }

        /// <summary>
        /// 获取城市列表数据
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetCitySelectItemsByProvinceId(IServiceProvider serviceProvider, int provinceId, int? selectId)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IDictionaryApiService>();
                var results = service.LoadCitiesByProvinceId(provinceId);
                var list = new List<SelectListItem>();
                if (results == null || !results.Any())
                    return list;

                list.Add(new SelectListItem()
                {
                    Text = "请选择市",
                    Value = "",
                });

                list.AddRange(results.Select(model => new SelectListItem
                {
                    Text = model.Name,
                    Value = model.Id.ToString(),
                    Selected = selectId.HasValue && selectId.Value == model.Id,
                }));

                return list;
            }
        }
        #endregion
    }
}
