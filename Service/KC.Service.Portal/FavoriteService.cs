using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Framework.Extension;
using KC.Database.EFRepository;

using KC.Service.EFService;
using KC.Service.DTO.Portal;
using KC.Model.Portal;
using KC.Database.IRepository;
using KC.Service.WebApiService.Business;
using KC.Framework.Tenant;
using KC.Enums.Portal;
using KC.Framework.Exceptions;

namespace KC.Service.Portal
{
    public interface IFavoriteService
    {
        bool DelFavoriteInfo(string relationId, string concernedUserId, FavoriteType type, 
             string tenantname);
        FavoriteInfoDTO QueryFavoriteInfo(string relationId, string concernedUserId, FavoriteType type, string tenantname);

        bool AddFavoriteInfoShop(FavoriteInfoDTO favorite);
    }

    public class FavoriteService : EFServiceBase, IFavoriteService
    {
        private readonly IMapper _mapper;
        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }
        private IDbRepository<FavoriteInfo> _favoriteRepository;

        public FavoriteService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IDbRepository<FavoriteInfo> favoriteRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<FavoriteService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;

            _favoriteRepository = favoriteRepository;
        }

        /// <summary>
        /// 根据关注者id和被关注者id查询是否已经关注
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="concernedUserId"></param>
        /// <param name="type"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public bool DelFavoriteInfo(string relationId, string concernedUserId, FavoriteType type, string tenantname)
        {
            Expression<Func<FavoriteInfo, bool>> predicate = m =>
                                m.RelationId.Equals(relationId, StringComparison.CurrentCultureIgnoreCase) && m.CompanyCode.Equals(tenantname) &&
                                m.ConcernedUserId.Equals(concernedUserId, StringComparison.CurrentCultureIgnoreCase) && m.FavoriteType == type && !m.IsDeleted;

            var data = _favoriteRepository.FindAll(predicate).FirstOrDefault();
            if (data != null)
            {
                return _favoriteRepository.Remove(data);
            }
            return false;
        }


        /// <summary>
        /// 根据关注者id和被关注者id查询是否已经关注
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="concernedUserId"></param>
        /// <param name="type"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public FavoriteInfoDTO QueryFavoriteInfo(string relationId, string concernedUserId, FavoriteType type, string tenantname)
        {
            Expression<Func<FavoriteInfo, bool>> predicate = m =>
                                m.RelationId.Equals(relationId, StringComparison.CurrentCultureIgnoreCase) && m.CompanyCode.Equals(tenantname, StringComparison.CurrentCultureIgnoreCase) &&
                                m.ConcernedUserId.Equals(concernedUserId, StringComparison.CurrentCultureIgnoreCase) && m.FavoriteType == type && !m.IsDeleted ;
            return _mapper.Map<FavoriteInfoDTO>(_favoriteRepository.FindAll(predicate).FirstOrDefault());
        }

        public bool AddFavoriteInfoShop(FavoriteInfoDTO favorite)
        {
            var entity =
                _favoriteRepository.FindAll(
                    m => m.FavoriteType == favorite.FavoriteType && m.RelationId == favorite.RelationId && m.CompanyCode == favorite.CompanyCode && m.ConcernedUserId == favorite.ConcernedUserId);
            if (entity != null)
                throw new BusinessApiException("您之前已经收藏过该" + favorite.FavoriteType.ToDescription() + ".");
            return _favoriteRepository.Add(_mapper.Map<FavoriteInfo>(favorite));
        }

    }
}
