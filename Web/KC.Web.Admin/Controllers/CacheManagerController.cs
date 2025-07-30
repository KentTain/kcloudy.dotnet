using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using KC.Service.DTO;
using KC.Service;
using KC.Framework.Tenant;
using KC.Framework.Extension;
using KC.Framework.Base;
using System.Runtime.Serialization;

namespace KC.Web.Admin.Controllers
{
    [Web.Extension.MenuFilter("后台管理", "缓存管理", "/CacheManager/Index",
        Version = TenantConstant.DefaultVersion, TenantType = TenantConstant.DefaultTenantType, SmallIcon = "fa fa-dropbox", AuthorityId = "13FC302E-A75C-4354-816E-2C611957EB05",
        DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsExtPage = false, Level = 2)]
    public class CacheManagerController : AdminBaseController
    {
        public CacheManagerController(
            IServiceProvider serviceProvider,
            ILogger<CacheManagerController> logger)
            : base(serviceProvider, logger)
        {
        }

        [Web.Extension.PermissionFilter("缓存管理", "缓存管理", "/CacheManager/Index", "13FC302E-A75C-4354-816E-2C611957EB05",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = true, ResultType = ResultType.ActionResult, AuthorityId = "13FC302E-A75C-4354-816E-2C611957EB05")]
        public ActionResult Index()
        {
            return View();
        }

        //[Web.Extension.PermissionFilter("缓存管理", "加载缓存列表数据", "/CacheManager/LoadCacheList",
        //    DefaultRoleId = RoleConstants.AdminRoleId, Order = 1, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "96421B0F-A24E-436E-9AC7-CE9E1DCCFA9F")]
        public JsonResult LoadCacheList(int page, int rows, string tenantName = TenantConstant.DbaTenantName, string searchKey = "", string searchValue = "")
        {
            var keyList = GetTenantCaches(tenantName);
            if (!keyList.Any())
                return Json(new PaginatedBaseDTO<CacheDTO>(page, rows, keyList.Count, keyList));

            string name = string.Empty;
            string type = string.Empty;
            if (!string.IsNullOrWhiteSpace(searchKey) && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey.ToLower())
                {
                    case "key":
                        name = searchValue;
                        break;
                    case "type":
                        type = searchValue;
                        break;

                }
            }

            Expression<Func<CacheDTO, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Key.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(type))
            {
                predicate = predicate.And(m => m.Type.Contains(type));
            }

            var newKeyList =
                keyList.Where(predicate.Compile())
                    .OrderBy(m => m.Key)
                    .Skip((page - 1) * rows)
                    .Take(rows)
                    .ToList();

            return Json(new PaginatedBaseDTO<CacheDTO>(page, rows, newKeyList.Count, newKeyList));
        }

        [Web.Extension.PermissionFilter("缓存管理", "删除单个缓存", "/CacheManager/RemoveDatabasePool", "13AD5E0B-6588-4FEA-BCFD-57A4719791AA",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 2, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "13AD5E0B-6588-4FEA-BCFD-57A4719791AA")]
        public JsonResult RemoveCache(string key)
        {
            return GetServiceJsonResult(() =>
            {
                CacheUtil.RemoveCache(key);
                return true;
            });
        }

        [Web.Extension.PermissionFilter("缓存管理", "删除所有缓存", "/CacheManager/RemoveDatabasePool", "AD86A155-3777-4087-AB21-C8A0E22D383C",
            DefaultRoleId = RoleConstants.AdminRoleId, Order = 3, IsPage = false, ResultType = ResultType.JsonResult, AuthorityId = "AD86A155-3777-4087-AB21-C8A0E22D383C")]
        public JsonResult RemoveAllCache()
        {
            return GetServiceJsonResult(() =>
            {
                CacheUtil.RemoveAllCache();
                return true;
            });
        }

        private List<CacheDTO> GetTenantCaches(string tenantName)
        {
            var cacheKeyList = CacheUtil.GetAllCacheKey();
            var keyList = new List<CacheDTO>();

            var tenantCaches = tenantName.Equals(TenantConstant.DbaTenantName, StringComparison.OrdinalIgnoreCase)
                ? cacheKeyList.Where(
                    m => m.StartsWith("com-")
                         || m.StartsWith("__AspSession")
                         || m.StartsWith(tenantName, StringComparison.OrdinalIgnoreCase))
                : cacheKeyList.Where(m => m.StartsWith(tenantName, StringComparison.OrdinalIgnoreCase));
            foreach (var key in tenantCaches)
            {
                var cacheObj = new CacheDTO { Key = key };
                if (key.StartsWith("com-"))
                {
                    cacheObj.Type = "全局缓存";
                }
                else if (key.StartsWith("__AspSession"))
                {
                    cacheObj.Type = "会话缓存";
                }
                else
                {
                    cacheObj.Type = "租户缓存";
                }

                keyList.Add(cacheObj);
            }


            return keyList;
        }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class CacheDTO : EntityBaseDTO
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Type { get; set; }
    }
}