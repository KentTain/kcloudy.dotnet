using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Message;
using KC.Service.DTO.Config;
using KC.Framework.Base;
using KC.Service;
using KC.Service.DTO.Message;
using KC.Service.Enums.Message;
using KC.Service.DTO;

namespace KC.WebApi.Message.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class NewsBulletinApiController : Web.Controllers.WebApiBaseController
    {
        protected INewsBulletinService _newsBulletinService => ServiceProvider.GetService<INewsBulletinService>();

        public NewsBulletinApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<NewsBulletinApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 获取最新的新闻公告
        /// </summary>
        /// <returns>获取10条最新的新闻公告</returns>
        [HttpGet]
        [Route("LoadLatestNewsBulletins")]
        public ServiceResult<List<NewsBulletinDTO>> LoadLatestNewsBulletins(NewsBulletinType? type)
        {
            return GetServiceResult(() =>
            {
                return _newsBulletinService.FindTop10NewsBulletins(type);
            });
        }

        /// <summary>
        /// 获取新闻公告的分页数据
        /// </summary>
        /// <param name="pageIndex">分页页码</param>
        /// <param name="pageSize">分页大小（不能超过100）</param>
        /// <param name="categoryId">所属分类</param>
        /// <param name="title">新闻公告的标题</param>
        /// <param name="type">新闻公告的类型</param>
        /// <returns>新闻公告的分页数据</returns>
        [HttpGet]
        [Route("LoadPaginatedNewsBulletins")]
        public ServiceResult<PaginatedBaseDTO<NewsBulletinDTO>> LoadPaginatedNewsBulletins(int pageIndex, int pageSize, int? categoryId, string title, NewsBulletinType? type)
        {
            return GetServiceResult(() =>
            {
                return _newsBulletinService.LoadPaginatedNewsBulletinsByFilter(pageIndex, pageSize,  categoryId, title, type);
            });
        }

        /// <summary>
        /// 获取新闻公告的详情
        /// </summary>
        /// <param name="id">新闻公告的Id号</param>
        /// <returns>新闻公告的详情</returns>
        [HttpGet]
        [Route("GetNewsBulletinById")]
        public ServiceResult<NewsBulletinDTO> GetNewsBulletinById(int id)
        {
            return GetServiceResult(() =>
            {
                return _newsBulletinService.GetNewsBulletinById(id);
            });
        }
    }
}
