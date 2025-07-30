using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Database.EFRepository;

using KC.Service.EFService;
using KC.Service.DTO.Portal;
using KC.DataAccess.Portal.Repository;
using KC.Model.Portal;
using KC.Database.IRepository;
using KC.Framework.Tenant;
using KC.Service.ViewModel.Portal;
using KC.Service.Portal.WebApiService.Business;
using KC.Service.Enums.Message;
using KC.Service.DTO;
using KC.Framework.Extension;
using KC.Enums.Portal;

namespace KC.Service.Portal
{
    public interface IFrontWebInfoService
    {
        #region 站点信息
        /// <summary>
        /// 站点数据：缓存20天
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler(20 * 24 * 60)]
        Task<WebSiteInfoDTO> GetWebSiteInfoAsync();

        /// <summary>
        /// 站点数据：缓存20天
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler(20 * 24 * 60)]
        Task<CompanyInfoDTO> GetCompanyDetailInfoAsync();

        [Extension.CachingCallHandler(7 * 24 * 60)]
        Task<List<WebSitePageDTO>> LoadWebSitePagesBySkinCode(string code);
        /// <summary>
        /// 首页相关数据：缓存7天
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler(7 * 24 * 60)]
        Task<WebSitePageDTO> GetWebSitePageDetailBySkinCode(string code, string url);

        /// <summary>
        /// 自定义页面的相关数据：缓存7天
        /// </summary>
        /// <returns></returns>
        [Extension.CachingCallHandler(7 * 24 * 60)]
        Task<WebSitePageDTO> GetWebSitePageDetailById(Guid id);
        #endregion

        #region 推荐商品

        Task<int> GetOfferingTotalCountAsync(int? categoryId, string name);
        Task<List<RecommendInfoDTO>> FindTop10OfferingsAsync();
        Task<PaginatedBaseDTO<RecommendOfferingDTO>> LoadPaginatedOfferingsByNameAsync(int pageIndex, int pageSize, int? categoryId, string name);

        Task<RecommendOfferingDTO> GetOfferingByIdAsync(int id);

        #endregion

        #region 推荐采购需求

        Task<int> GetRequirementTotalCountAsync(int? categoryId, string name);
        Task<List<RecommendInfoDTO>> FindTop10RequirementsAsync();
        Task<PaginatedBaseDTO<RecommendRequirementDTO>> LoadPaginatedRequirementsByNameAsync(int pageIndex, int pageSize, int? categoryId, string name, RequirementType? nameType);
        Task<RecommendRequirementDTO> GetRequirementByIdAsync(int id);
        #endregion
    }

    public class FrontWebInfoService : EFServiceBase, IFrontWebInfoService
    {
        private readonly IMapper _mapper;

        #region Construct & Repository
        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }
        private readonly INewsBulletinApiService _newsBulletinApiService;

        private readonly IWebSiteInfoRepository _webSiteInfoRepository;
        private readonly IDbRepository<WebSiteLink> _webSiteLinkRepository;
        private readonly IWebSitePageRepository _webSitePageRepository;

        private ICompanyInfoRepository _companyInfoRepository;

        private readonly IRecommendCategoryRepository _redCategoryRepository;
        private readonly IRecommendCustomerRepository _redCustomerRepository;
        private readonly IRecommendOfferingRepository _redOfferingRepository;
        private readonly IRecommendRequirementRepository _redRequirementRepository;

        //private IDbRepository<Favorite> _favoriteRepository;

        public FrontWebInfoService(
            Tenant tenant,
            IMapper mapper,

            INewsBulletinApiService newsBulletinApiService,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IWebSiteInfoRepository webInfoRepository,
            IDbRepository<WebSiteLink> webLinkRepository,
            IWebSitePageRepository webSitePageRepository,

            ICompanyInfoRepository companyInfoRepository,

            IRecommendCategoryRepository redCategoryRepository,
            IRecommendCustomerRepository redCustomerRepository,
            IRecommendOfferingRepository redOfferingRepository,
            IRecommendRequirementRepository redRequirementRepository,
            //IDbRepository<Favorite> favoriteRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<FrontWebInfoService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;

            _webSitePageRepository = webSitePageRepository;
            _webSiteInfoRepository = webInfoRepository;
            _webSiteLinkRepository = webLinkRepository;

            _companyInfoRepository = companyInfoRepository;
            _redCategoryRepository = redCategoryRepository;
            _redCustomerRepository = redCustomerRepository;
            _redOfferingRepository = redOfferingRepository;
            _redRequirementRepository = redRequirementRepository;

            _newsBulletinApiService = newsBulletinApiService;
            //_favoriteRepository = favoriteRepository;
        }
        #endregion

        #region 站点信息

        public async Task<WebSiteInfoDTO> GetWebSiteInfoAsync()
        {
            // 站点基本信息
            var websiteInfo = await _webSiteInfoRepository.GetByFilterAsync(m => !m.IsDeleted);

            return _mapper.Map<WebSiteInfoDTO>(websiteInfo);
        }

        /// <summary>
        /// 获取企业详情信息
        /// </summary>
        public async Task<CompanyInfoDTO> GetCompanyDetailInfoAsync()
        {
            var data = await _companyInfoRepository.GetCompanyDetailInfoAsync();
            if (data == null)
                return new CompanyInfoDTO();
            return _mapper.Map<CompanyInfoDTO>(data);
        }


        public async Task<List<WebSitePageDTO>> LoadWebSitePagesBySkinCode(string code)
        {
            var data = await _webSitePageRepository.FindPagenatedListAsync(1, 5, m => m.IsEnable && m.SkinCode.Equals(code), m=> m.Index, true);

            return _mapper.Map<List<WebSitePageDTO>>(data);
        }
        public async Task<WebSitePageDTO> GetWebSitePageDetailBySkinCode(string code, string url)
        {
            var data = await _webSitePageRepository.GetWebSiteColumnDetailBySkinCodeAsync(code, url);

            return _mapper.Map<WebSitePageDTO>(data);
        }

        public async Task<WebSitePageDTO> GetWebSitePageDetailById(Guid id)
        {
            var data = await _webSitePageRepository.GetWebSiteColumnDetailInfoAsync(id);

            return _mapper.Map<WebSitePageDTO>(data);
        }

        #endregion

        #region 推荐商品

        public async Task<int> GetOfferingTotalCountAsync(int? categoryId, string name)
        {
            Expression<Func<RecommendOffering, bool>> predicate = m => !m.IsDeleted && m.Status == KC.Enums.Portal.RecommendStatus.Approved;
            if (categoryId.HasValue)
            {
                predicate = predicate.And(m => m.CategoryId != null && m.CategoryId == categoryId.Value);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.RecommendName.Contains(name));
            }

            return await _redOfferingRepository.GetRecordCountAsync(predicate);
        }
        public async Task<List<RecommendInfoDTO>> FindTop10OfferingsAsync()
        {
            Expression<Func<RecommendOffering, bool>> predicate = m => !m.IsDeleted
                    && m.Status == KC.Enums.Portal.RecommendStatus.Approved;
            var data = await _redOfferingRepository.FindPagenatedListAsync(1, 10, predicate, m => m.IsTop);
            return _mapper.Map<List<RecommendInfoDTO>>(data);
        }

        public async Task<PaginatedBaseDTO<RecommendOfferingDTO>> LoadPaginatedOfferingsByNameAsync(int pageIndex, int pageSize, int? categoryId, string name)
        {
            Expression<Func<RecommendOffering, bool>> predicate = m => !m.IsDeleted && m.Status == KC.Enums.Portal.RecommendStatus.Approved;
            if (categoryId.HasValue && categoryId.Value != 0)
            {
                predicate = predicate.And(m => m.CategoryId != null && m.CategoryId == categoryId.Value);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.RecommendName.Contains(name));
            }

            var data = await _redOfferingRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.Index, false);
            var total = data.Item1;
            var rows = _mapper.Map<List<RecommendOfferingDTO>>(data.Item2);
            return new PaginatedBaseDTO<RecommendOfferingDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<RecommendOfferingDTO> GetOfferingByIdAsync(int id)
        {
            var data = await _redOfferingRepository.GetByIdAsync(id);
            return _mapper.Map<RecommendOfferingDTO>(data);
        }
        #endregion

        #region 推荐采购需求

        public async Task<int> GetRequirementTotalCountAsync(int? categoryId, string name)
        {
            Expression<Func<RecommendRequirement, bool>> predicate = m => !m.IsDeleted && m.Status == KC.Enums.Portal.RecommendStatus.Approved;
            if (categoryId.HasValue)
            {
                predicate = predicate.And(m => m.CategoryId != null && m.CategoryId == categoryId.Value);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.RecommendName.Contains(name));
            }

            return await _redRequirementRepository.GetRecordCountAsync(predicate);
        }
        public async Task<List<RecommendInfoDTO>> FindTop10RequirementsAsync()
        {
            Expression<Func<RecommendRequirement, bool>> predicate = m => !m.IsDeleted
                    && m.Status == KC.Enums.Portal.RecommendStatus.Approved;
            var data = await _redRequirementRepository.FindPagenatedListAsync(1, 10, predicate, m => m.IsTop);
            return _mapper.Map<List<RecommendInfoDTO>>(data);
        }
        public async Task<PaginatedBaseDTO<RecommendRequirementDTO>> LoadPaginatedRequirementsByNameAsync(int pageIndex, int pageSize, int? categoryId, string name, RequirementType? nameType)
        {
            Expression<Func<RecommendRequirement, bool>> predicate = m => !m.IsDeleted && m.Status == KC.Enums.Portal.RecommendStatus.Approved;
            if (categoryId.HasValue && categoryId.Value != 0)
            {
                predicate = predicate.And(m => m.CategoryId != null && m.CategoryId == categoryId.Value);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.RecommendName.Contains(name));
            }
            if (nameType.HasValue)
            {
                predicate = predicate.And(m => m.RequirementType == nameType.Value);
            }

            var data = await _redRequirementRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.Index, false);
            var total = data.Item1;
            var rows = _mapper.Map<List<RecommendRequirementDTO>>(data.Item2);
            return new PaginatedBaseDTO<RecommendRequirementDTO>(pageIndex, pageSize, total, rows);
        }
        public async Task<RecommendRequirementDTO> GetRequirementByIdAsync(int id)
        {
            var data = await _redRequirementRepository.GetByIdAsync(id);
            return _mapper.Map<RecommendRequirementDTO>(data);
        }
        #endregion

    }
}
