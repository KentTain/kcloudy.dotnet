using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

using AutoMapper;
using KC.Database.EFRepository;
using KC.Service.EFService;
using KC.Service.DTO.CodeGenerate;
using KC.Service.DTO;
using KC.Model.CodeGenerate;
using KC.Framework.Extension;
using KC.Database.IRepository;
using KC.Service.WebApiService.Business;
using KC.Framework.Tenant;
using KC.DataAccess.CodeGenerate.Repository;

namespace KC.Service.CodeGenerate
{
    public interface IRelationDefinitionService : IEFService
    {
        #region RelationDefinition

        Task<PaginatedBaseDTO<RelationDefinitionDTO>> FindPaginatedRelationDefsByFilterAsync(int pageIndex, int pageSize, string appId, string busName);

        Task<RelationDefinitionDTO> GetRelationDefByIdAsync(int id);
        bool SaveRelationDef(RelationDefinitionDTO data);
        bool RemoveRelationDefById(int id);
        #endregion

        #region RelationDefDetail
        Task<List<RelationDefDetailDTO>> FindAllRelationDefDetailListAsync(int settingId);

        Task<RelationDefDetailDTO> GetRelationDefDetailByIdAsync(int id);
        bool SaveRelationDefDetail(RelationDefDetailDTO data);
        bool RemoveRelationDefDetailById(int id);
        #endregion
    }

    public class RelationDefinitionService : EFServiceBase, IRelationDefinitionService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        private EFUnitOfWorkContextBase _unitOfContext;

        private IRelationDefinitionRepository _appApiPushSettingRepository;
        private IDbRepository<RelationDefDetail> _appTargetApiSettingRepository;

        public RelationDefinitionService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            IRelationDefinitionRepository appApiPushSettingRepository,
            IDbRepository<RelationDefDetail> appTargetApiSettingRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<ModelDefinitionService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfContext = unitOfContext;
            
            _appApiPushSettingRepository = appApiPushSettingRepository;
            _appTargetApiSettingRepository = appTargetApiSettingRepository;
        }

        #region RelationDefinition

        public async Task<PaginatedBaseDTO<RelationDefinitionDTO>> FindPaginatedRelationDefsByFilterAsync(int pageIndex, int pageSize, string appId, string busName)
        {
            Expression<Func<RelationDefinition, bool>> predicate = m => !m.IsDeleted;
            if (string.IsNullOrEmpty(appId))
            {
                predicate = predicate.And(m => m.ApplicationId == appId);
            }

            var data = await _appApiPushSettingRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.CreatedDate, false);
            var total = data.Item1;
            var rows = _mapper.Map<List<RelationDefinitionDTO>>(data.Item2);
            return new PaginatedBaseDTO<RelationDefinitionDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<RelationDefinitionDTO> GetRelationDefByIdAsync(int id)
        {
            var data = await _appApiPushSettingRepository.GetByIdAsync(id);
            return _mapper.Map<RelationDefinitionDTO>(data);
        }
        public bool SaveRelationDef(RelationDefinitionDTO data)
        {
            var model = _mapper.Map<RelationDefinition>(data);
            if (model.Id == 0)
            {
                return _appApiPushSettingRepository.Add(model);
            }
            else
            {
                return _appApiPushSettingRepository.Modify(model, true);
            }
        }
        public bool RemoveRelationDefById(int id)
        {
            return _appApiPushSettingRepository.SoftRemoveById(id);
        }
        #endregion

        #region RelationDefDetail
        public async Task<List<RelationDefDetailDTO>> FindAllRelationDefDetailListAsync(int settingId)
        {
            Expression<Func<RelationDefDetail, bool>> predicate = m => m.Id.Equals(settingId);
            var data = await _appTargetApiSettingRepository.FindAllAsync<bool>(predicate);
            return _mapper.Map<List<RelationDefDetailDTO>>(data);
        }

        public async Task<RelationDefDetailDTO> GetRelationDefDetailByIdAsync(int id)
        {
            var data = await _appTargetApiSettingRepository.GetByIdAsync(id);
            return _mapper.Map<RelationDefDetailDTO>(data);
        }
        public bool SaveRelationDefDetail(RelationDefDetailDTO data)
        {
            var model = _mapper.Map<RelationDefDetail>(data);
            if (model.Id == 0)
            {
                return _appTargetApiSettingRepository.Add(model);
            }
            else
            {
                return _appTargetApiSettingRepository.Modify(model, true);
            }
        }
        public bool RemoveRelationDefDetailById(int id)
        {
            return _appTargetApiSettingRepository.RemoveById(id);
        }
        #endregion
    }
}
