using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.Dict;
using KC.Service.Util;
using KC.Service.DTO.Dict;

namespace KC.Web.Dict.Controllers
{
    /// <summary>
    /// 二级菜单：配置管理/配置管理
    /// </summary>
    [Web.Extension.MenuFilter("字典管理", "字典管理", "/Dictionary/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-list-alt", AuthorityId = "9CBD7FE5-E00E-4784-A232-CE7080B19B80",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsExtPage = false, Level = 2)]
    public class DictionaryController : DictBaseController
    {
        private IDictionaryService _dictionaryService => ServiceProvider.GetService<IDictionaryService>();

        public DictionaryController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<DictionaryController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("字典管理", "字典管理", "/Dictionary/Index", "9CBD7FE5-E00E-4784-A232-CE7080B19B80",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = true, ResultType = ResultType.ActionResult)]
        public ActionResult Index()
        {
            //ViewBag.DictionaryTypeList = GetDropDownItemsByEnumWithAll<DictionaryType>();
            return View();
        }

        #region DictType

        /// <summary>
        /// 字典类型列表
        /// </summary>
        /// <param name="searchtext"></param>
        /// <returns></returns>
        //[Web.Extension.PermissionFilter("字典管理", "加载字典类型列表", "/Dictionary/LoadDictTypeList", "003A3A8F-9CC4-493C-A582-7F76F0AF92DE",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "003A3A8F-9CC4-493C-A582-7F76F0AF92DE")]
        public JsonResult LoadDictTypeList(string name)
        {
            var result = new List<DictTypeDTO>(){
                new DictTypeDTO()
                {
                    Id = 0,
                    Name = "所有字典类型",
                }
            };

            var data = _dictionaryService.FindDictTypeList(name);
            if (data != null && data.Count() > 0)
                result.AddRange(data);

            return Json(result);
        }

        /// <summary>
        /// 保存字典类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("字典管理", "保存字典类型", "/Dictionary/SaveDictType", "B88E5DD9-9C29-49FC-8CF6-2C401D86C585",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveDictType([FromBody] List<DictTypeDTO> models)
        {
            return GetServiceJsonResult(() =>
            {
                return _dictionaryService.SaveDictType(models, CurrentUserId, CurrentUserDisplayName);
            });
        }
        /// <summary>
        /// 删除字典类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Web.Extension.PermissionFilter("字典管理", "删除字典类型", "/Dictionary/RemoveDictType", "78F9C5F1-519E-4E74-A34B-341AB8C0BC1E",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveDictType(int id)
        {
            return GetServiceJsonResult(() =>
            {
                return _dictionaryService.RemoveDictType(id);
            });
        }
        #endregion

        #region DictValue

        //[Web.Extension.PermissionFilter("字典管理", "加载字典值列表", "/Dictionary/LoadDictValueList", "09F33BAB-2A6F-45BA-91EA-D300CC1DEC20",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 4, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "09F33BAB-2A6F-45BA-91EA-D300CC1DEC20")]
        public JsonResult LoadDictValueList(int? typeId = 0, int page = 1, int rows = 10, string name = "")
        {
            var result = _dictionaryService.FindPaginatedDictValuesByFilter(typeId, page, rows, name);
            return Json(result);
        }

        [Web.Extension.PermissionFilter("字典管理", "保存数据字典值", "/Dictionary/SaveDictValue", "C607C002-D378-4D48-A445-DDDB47379550",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 5, IsPage = false, ResultType = ResultType.JsonResult)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveDictValue([FromBody] List<DictValueDTO> models, int typeId)
        {
            return GetServiceJsonResult(() =>
            {
                return _dictionaryService.SaveDictValue(models, typeId, CurrentUserId, CurrentUserDisplayName);
            });
        }

        [Web.Extension.PermissionFilter("字典管理", "删除数据字典值", "/Dictionary/RemoveDictValue", "2EE04641-EE06-4E8B-AA63-BE05CA11F836",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 6, IsPage = false, ResultType = ResultType.JsonResult)]
        public JsonResult RemoveDictValue(int id)
        {
            return GetServiceJsonResult(() =>
            {
                var result1 = _dictionaryService.RemoveDictValue(id);
                return result1;
            });
        }

        #endregion
    }
}
