using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Portal;
using KC.Service;
using KC.Service.DTO.Portal;
using KC.Enums.Portal;

namespace KC.WebApi.Portal.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FavoriteApiController : Web.Controllers.WebApiBaseController
    {
        protected IFavoriteService _favoriteService => ServiceProvider.GetService<IFavoriteService>();

        public FavoriteApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<FavoriteApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        [HttpGet]
        [Route("DelFavorite")]
        public ServiceResult<bool> DelFavorite(string relationId, string concernedUserId, FavoriteType type, Guid appId, string tenantname)
        {
            return GetServiceResult(() =>
            {
                return _favoriteService.DelFavorite(relationId, concernedUserId, type, appId, tenantname);
            });
        }

        [HttpGet]
        [Route("QueryFavorite")]
        public ServiceResult<FavoriteDTO> QueryFavorite(string relationId, string concernedUserId, FavoriteType type, Guid appId, string tenantname)
        {
            return GetServiceResult(() =>
            {
                return _favoriteService.QueryFavorite(relationId, concernedUserId, type, appId, tenantname);
            });
        }

        [HttpGet]
        [Route("AddFavoriteShop")]
        public ServiceResult<bool> AddFavoriteShop(FavoriteDTO favorite)
        {
            return GetServiceResult(() =>
            {
                return _favoriteService.AddFavoriteShop(favorite);
            });
        }
    }
}
