using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Framework.Extension;
using KC.Framework.Util;
using KC.Database.EFRepository;
using KC.Service;
using KC.Service.Util;
using KC.Service.Constants;
using KC.Service.EFService;
using KC.Service.DTO.Supplier;
using KC.DataAccess.Supplier.Repository;
using KC.Model.Supplier;
using KC.DataAccess.Supplier;
using KC.Service.DTO.Admin;
using KC.Database.IRepository;
using KC.Service.WebApiService.Business;
using KC.Framework.Tenant;

namespace KC.Service.Supplier
{
    public interface IDictionaryService : IEFService
    {
        #region 城市分类
        ProvinceDTO GetProvinceById(int id);
        //[CachingCallHandler(TimeOutConstants.DefaultCacheTimeOut, Order = 0)]
        List<ProvinceDTO> GetAllProvinces();

        CityDTO GetCityById(int id);
        //[CachingCallHandler(TimeOutConstants.DefaultCacheTimeOut, Order = 0)]
        List<CityDTO> GetAllCities();
        List<CityDTO> GetCitiesByProvinceId(int provinceID);
        #endregion

        #region 行业分类
        List<IndustryClassficationDTO> GetRootIndustryClassfications();
        List<IndustryClassficationDTO> GetIndustryClassficationsByLevel(int level);
        List<IndustryClassficationDTO> GetIndustryClassficationsByName(string name);
        List<IndustryClassficationDTO> GetIndustryClassficationsByParentId(int parentId);

        IndustryClassficationDTO GetIndustryClassficationById(int industryClassificationId);
        #endregion

        MobileLocationDTO GetMobileLocation(string mobilePhone);
    }

    public class DictionaryService : EFServiceBase, IDictionaryService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private IDbRepository<Province> _provinceRepository;
        private IDbRepository<City> _cityRepository;
        private IIndustryClassficationRepository _industryClassficationRepository;
        private MobileLocationRepository _mobileLocationRepository;

        public DictionaryService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IDbRepository<Province> provinceRepository,
            IDbRepository<City> cityRepository,
            IIndustryClassficationRepository industryClassficationRepository,
            MobileLocationRepository mobileLocationRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<DictionaryService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;
            
            _provinceRepository = provinceRepository;
            _cityRepository = cityRepository;

            _mobileLocationRepository = mobileLocationRepository;

            _industryClassficationRepository = industryClassficationRepository;
        }

        #region Province & City

        public List<ProvinceDTO> GetAllProvinces()
        {
            var data = _provinceRepository.FindAll(m => m.Name);
            return _mapper.Map<List<ProvinceDTO>>(data);
        }
        public ProvinceDTO GetProvinceById(int id)
        {
            var data = _provinceRepository.GetById(id);
            return _mapper.Map<ProvinceDTO>(data);
        }

        public List<CityDTO> GetCitiesByProvinceId(int provinceID)
        {
            var data = _cityRepository.FindAll(m => m.ProvinceId == provinceID, m => m.Name);
            return _mapper.Map<List<CityDTO>>(data);
        }

        public ServiceResult<IList<ProvinceDTO>> GetProvinceLists()
        {
            throw new NotImplementedException();
        }

        public List<CityDTO> GetAllCities()
        {
            var data = _cityRepository.FindAll();
            return _mapper.Map<List<CityDTO>>(data);
        }
        public CityDTO GetCityById(int id)
        {
            var data = _cityRepository.GetById(id);
            return _mapper.Map<CityDTO>(data);
        }

        public ProvinceDTO GetProvinceByName(string name)
        {
            var data = _provinceRepository.GetByFilter(o=>o.Name==name);
            return _mapper.Map<ProvinceDTO>(data);
        }

        public CityDTO GetCityByName(string name)
        {
            var data = _cityRepository.GetByFilter(o => o.Name == name);
            return _mapper.Map<CityDTO>(data);
        }
        #endregion 

        #region 行业分类
        public List<IndustryClassficationDTO> GetRootIndustryClassfications()
        {
            var data = _industryClassficationRepository.GetRootIndustryClassification();
            return _mapper.Map<List<IndustryClassficationDTO>>(data);
        }
        public List<IndustryClassficationDTO> GetIndustryClassficationsByLevel(int level)
        {
            var data = _industryClassficationRepository.GetIndustryClassificationList(level);
            var res = _mapper.Map<List<IndustryClassficationDTO>>(data);
            return res;
        }
        public List<IndustryClassficationDTO> GetIndustryClassficationsByName(string name)
        {
            Expression<Func<IndustryClassfication, bool>> predicate = m => m.ParentId == null && !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            var data = _industryClassficationRepository.GetRootIndeustryClassificatiosByFilter(predicate, m => m.Index);
            var res = _mapper.Map<List<IndustryClassficationDTO>>(data);
            return res;
        }

        /// <summary>
        /// 获取行业子类 + List<IndustryClassficationDTO> GetChildIndustryClassficationList(int parentId)
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<IndustryClassficationDTO> GetIndustryClassficationsByParentId(int parentId)
        {
            if (!string.IsNullOrEmpty(parentId.ToString()))
            {
                var data = _industryClassficationRepository.GetChildIndustryClassificationList(parentId);
                var res = _mapper.Map<List<IndustryClassficationDTO>>(data);
                return res;
            }
            return null;
        }
        /// <summary>
        /// 捅过行业id获取行业分类 + IndustryClassficationDTO GetIndustryClassfication(int industryClassificationId)
        /// </summary>
        /// <param name="industryClassificationId"></param>
        /// <returns></returns>
        public IndustryClassficationDTO GetIndustryClassficationById(int industryClassificationId)
        {
            var data = _industryClassficationRepository.GetById(industryClassificationId);
            var res = _mapper.Map<IndustryClassficationDTO>(data);
            //res.IndustryStandard = GetIndustryStandardByIndustryIdandMeasurePointId(industryClassificationId);
            return res;
        }


        #endregion

        public MobileLocationDTO GetMobileLocation(string mobilePhone)
        {
            var data = _mobileLocationRepository.GetMobileLocation(mobilePhone);
            return _mapper.Map<MobileLocationDTO>(data);
        }
    }
}
