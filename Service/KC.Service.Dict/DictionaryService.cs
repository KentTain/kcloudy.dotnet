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
using KC.Service;
using KC.Service.Util;
using KC.Service.Constants;
using KC.Service.EFService;
using KC.Service.DTO.Dict;
using KC.DataAccess.Dict.Repository;
using KC.Model.Dict;
using KC.Database.IRepository;
using KC.Service.WebApiService.Business;
using KC.Service.DTO;
using KC.Framework.Tenant;

namespace KC.Service.Dict
{
    public interface IDictionaryService : IEFService
    {
        #region 城市分类
        ProvinceDTO GetProvinceById(int id);
        //[CachingCallHandler(TimeOutConstants.DefaultCacheTimeOut, Order = 0)]
        List<ProvinceDTO> FindAllProvinces();

        CityDTO GetCityById(int id);
        //[CachingCallHandler(TimeOutConstants.DefaultCacheTimeOut, Order = 0)]
        List<CityDTO> FindAllCities();
        List<CityDTO> FindCitiesByProvinceId(int provinceID);
        #endregion

        #region 行业分类
        List<IndustryClassficationDTO> FindRootIndustryClassfications();
        List<IndustryClassficationDTO> FindIndustryClassficationsByLevel(int level);
        List<IndustryClassficationDTO> FindIndustryClassficationsByName(string name);
        List<IndustryClassficationDTO> FindIndustryClassficationsByParentId(int parentId);
        #endregion

        #region 字典类型
        List<DictTypeDTO> FindDictTypeList(string searchvalue);
        bool SaveDictType(List<DictTypeDTO> models, string operatorId, string operatorName);
        bool RemoveDictType(int id);
        DictTypeDTO GetDictTypeById(int id);
        #endregion

        #region 字典值
        [Extension.CachingCallHandler(TimeOutConstants.DefaultCacheTimeOut + 10000)]
        List<DictValueDTO> FindAllDictValuesByDictTypeCode(string dictTypeCode);
        PaginatedBaseDTO<DictValueDTO> FindPaginatedDictValuesByFilter(int? typeId, int pageIndex, int pageSize, string name);
        
        DictValueDTO GetDictValueById(int id);
        bool SaveDictValue(List<DictValueDTO> models, int typeId, string operatorId, string operatorName);
        bool RemoveDictValue(int id);
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
        private readonly IConfigApiService ConfigApiService;

        private readonly IDbRepository<DictType> _dictTypeRepository;
        private readonly IDbRepository<DictValue> _dictValueRepository;

        private readonly IDbRepository<Province> _provinceRepository;
        private readonly IDbRepository<City> _cityRepository;
        private readonly IIndustryClassficationRepository _industryClassficationRepository;
        private readonly IMobileLocationRepository _mobileLocationRepository;

        public DictionaryService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,
            IConfigApiService configApiService,

            IDbRepository<DictType> dictTypeRepository,
            IDbRepository<DictValue> dictValueRepository,
            IDbRepository<Province> provinceRepository,
            IDbRepository<City> cityRepository,
            IIndustryClassficationRepository industryClassficationRepository,
            IMobileLocationRepository mobileLocationRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<DictionaryService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;
            ConfigApiService = configApiService;

            _dictTypeRepository = dictTypeRepository;
            _dictValueRepository = dictValueRepository;
            _provinceRepository = provinceRepository;
            _cityRepository = cityRepository;

            _mobileLocationRepository = mobileLocationRepository;

            _industryClassficationRepository = industryClassficationRepository;
        }

        #region Province & City

        public List<ProvinceDTO> FindAllProvinces()
        {
            var data = _provinceRepository.FindAll(m => m.Name);
            return _mapper.Map<List<ProvinceDTO>>(data);
        }
        public ProvinceDTO GetProvinceById(int id)
        {
            var data = _provinceRepository.GetById(id);
            return _mapper.Map<ProvinceDTO>(data);
        }

        public List<CityDTO> FindCitiesByProvinceId(int provinceID)
        {
            var data = _cityRepository.FindAll(m => m.ProvinceId == provinceID, m => m.Name);
            return _mapper.Map<List<CityDTO>>(data);
        }

        public List<CityDTO> FindAllCities()
        {
            var data = _cityRepository.FindAll();
            return _mapper.Map<List<CityDTO>>(data);
        }
        public CityDTO GetCityById(int id)
        {
            var data = _cityRepository.GetById(id);
            return _mapper.Map<CityDTO>(data);
        }
        #endregion 

        #region 行业分类
        public List<IndustryClassficationDTO> FindRootIndustryClassfications()
        {
            var data = _industryClassficationRepository.FindAllTreeNodeWithNestChild();
            //var data = _industryClassficationRepository.GetRootIndustryClassification();
            return _mapper.Map<List<IndustryClassficationDTO>>(data);
        }
        public List<IndustryClassficationDTO> FindIndustryClassficationsByLevel(int level)
        {
            var data = _industryClassficationRepository.GetIndustryClassificationList(level);
            var res = _mapper.Map<List<IndustryClassficationDTO>>(data);
            return res;
        }
        public List<IndustryClassficationDTO> FindIndustryClassficationsByName(string name)
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
        public List<IndustryClassficationDTO> FindIndustryClassficationsByParentId(int parentId)
        {
            if (!string.IsNullOrEmpty(parentId.ToString()))
            {
                var data = _industryClassficationRepository.GetChildIndustryClassificationList(parentId);
                var res = _mapper.Map<List<IndustryClassficationDTO>>(data);
                return res;
            }
            return null;
        }


        #endregion

        #region 字典类型
        public List<DictTypeDTO> FindDictTypeList(string name)
        {
            Expression<Func<DictType, bool>> precidate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(name))
            {
                precidate = precidate.And(m => m.Name.Contains(name));
            }
            var model = _dictTypeRepository.FindAll(precidate);
            return _mapper.Map<List<DictTypeDTO>>(model);
        }


        public DictTypeDTO GetDictTypeById(int id)
        {
            var model = _dictTypeRepository.GetById(id);
            return _mapper.Map<DictTypeDTO>(model);
        }

        public bool SaveDictType(List<DictTypeDTO> models, string operatorId, string operatorName)
        {
            var data = _mapper.Map<List<DictType>>(models);
            foreach (var type in data)
            {
                if (type.Id == 0)
                {
                    type.CreatedBy = operatorId;
                    type.CreatedName = operatorName;
                    type.CreatedDate = DateTime.UtcNow;
                    type.ModifiedBy = operatorId;
                    type.ModifiedName = operatorName;
                    type.ModifiedDate = DateTime.UtcNow;
                    type.Code = ConfigApiService.GetSeedCodeByName("DictionaryType");
                    _dictTypeRepository.Add(type, false);
                }
                else
                {
                    type.ModifiedBy = operatorId;
                    type.ModifiedName = operatorName;
                    type.ModifiedDate = DateTime.UtcNow;
                    _dictTypeRepository.Modify(type, new[] { "Name", "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);
                }
            }

            return _unitOfWorkContext.Commit() > 0;
        }

        public bool RemoveDictType(int id)
        {
            return _dictTypeRepository.SoftRemoveById(id);
        }
        #endregion

        #region 字典值

        public PaginatedBaseDTO<DictValueDTO> FindPaginatedDictValuesByFilter(int? typeId, int pageIndex, int pageSize, string name)
        {
            Expression<Func<DictValue, bool>> predicate = m => !m.IsDeleted;
            if (typeId != 0)
            {
                predicate = predicate.And(m => m.DictTypeId.Equals(typeId));
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            var data = _dictValueRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.CreatedDate);

            var total = data.Item1;
            var rows = _mapper.Map<List<DictValueDTO>>(data.Item2);
            return new PaginatedBaseDTO<DictValueDTO>(pageIndex, pageSize, total, rows);
        }

        public List<DictValueDTO> FindAllDictValuesByDictTypeCode(string dictTypeCode)
        {
            if (string.IsNullOrWhiteSpace(dictTypeCode))
                return new List<DictValueDTO>();
            Expression<Func<DictValue, bool>> predicate = m => !m.IsDeleted && m.DictTypeCode.Equals(dictTypeCode);

            var data = _dictValueRepository.FindAll(predicate);
            return _mapper.Map<List<DictValueDTO>>(data);
        }

        public DictValueDTO GetDictValueById(int id)
        {
            var data = _dictValueRepository.GetById(id);
            return _mapper.Map<DictValueDTO>(data);
        }
        public bool SaveDictValue(List<DictValueDTO> data, int typeId, string operatorId, string operatorName)
        {
            var type = _dictTypeRepository.GetById(typeId);
            if (type == null)
                throw new ArgumentNullException("typeId", string.Format("未找到ID【{0}】相关的类型", typeId));

            var models = _mapper.Map<List<DictValue>>(data);
            foreach (var model in models)
            {
                model.DictTypeId = typeId;
                model.DictTypeCode = type.Code;
                if (model.Id == 0)
                {
                    model.CreatedBy = operatorId;
                    model.CreatedName = operatorName;
                    model.CreatedDate = DateTime.UtcNow;
                    model.ModifiedBy = operatorId;
                    model.ModifiedName = operatorName;
                    model.ModifiedDate = DateTime.UtcNow;
                    model.Code = ConfigApiService.GetSeedCodeByName("DictionaryValue");
                    _dictValueRepository.Add(model, false);
                }
                else
                {
                    model.ModifiedBy = operatorId;
                    model.ModifiedName = operatorName;
                    model.ModifiedDate = DateTime.UtcNow;
                    _dictValueRepository.Modify(model, new[] { "Name", "Description", "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);
                }
            }

            var success = _unitOfWorkContext.Commit() > 0;
            if (success)
            {
                var tenantName = Tenant.TenantName;
                // Tenant Api 缓存
                var apiMethod1 = typeof(IDictionaryApiService).GetMethod("LoadAllDictValuesByTypeCode");
                var cacheKey1 = Util.CacheKeyUtil.CacheKeyGenerator(tenantName, apiMethod1, new object[] { type.Code });
                if (string.IsNullOrEmpty(cacheKey1))
                    Service.CacheUtil.RemoveCache(cacheKey1);

                //Tenant Service 缓存
                var serviceMethod1 = typeof(IDictionaryService).GetMethod("FindAllDictValuesByDictTypeCode");
                var cacheKey3 = Util.CacheKeyUtil.CacheKeyGenerator(tenantName, serviceMethod1, new object[] { type.Code });
                if (string.IsNullOrEmpty(cacheKey3))
                    Service.CacheUtil.RemoveCache(cacheKey3);
            }
            return success;
        }

        public bool RemoveDictValue(int id)
        {
            if (id == 0)
                return true;
            return _dictValueRepository.SoftRemoveById(id);
        }

        #endregion

        public MobileLocationDTO GetMobileLocation(string mobilePhone)
        {
            var data = _mobileLocationRepository.GetMobileLocation(mobilePhone);
            return _mapper.Map<MobileLocationDTO>(data);
        }
    }
}
