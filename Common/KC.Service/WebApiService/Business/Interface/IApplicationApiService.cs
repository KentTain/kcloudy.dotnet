using KC.Service.DTO.App;
using KC.Framework.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KC.Service.WebApiService.Business
{
    public interface IApplicationApiService
    {
        /// <summary>
        /// 获取所有的简单的应用信息
        /// </summary>
        /// <returns>ServiceResult<List<ApplicationInfo>></returns>
        Task<ServiceResult<List<ApplicationInfo>>> LoadAllSimpleApplicationsAsync();
        /// <summary>
        /// 获取所有的应用信息
        /// </summary>
        /// <returns>ServiceResult<List<ApplicationDTO>></returns>
        Task<ServiceResult<List<ApplicationDTO>>> LoadAllApplicationsAsync();
    }
}