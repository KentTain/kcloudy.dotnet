using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Database.EFRepository;
using KC.Service.EFService;
using KC.DataAccess.CodeGenerate.Repository;
using KC.Service.DTO.CodeGenerate;
using KC.Service.DTO;
using KC.Service.DTO.App;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using KC.Service;
using KC.Model.CodeGenerate;
using KC.Framework.Extension;
using KC.Database.IRepository;
using KC.Service.WebApiService.Business;
using KC.Framework.Tenant;

namespace KC.Service.CodeGenerate
{
    public interface IModelDefinitionService : IEFService
    {
        #region ModelDefinition

        Task<List<ModelDefinitionDTO>> FindAllModelDefinitionsAsync();

        Task<PaginatedBaseDTO<ModelDefinitionDTO>> FindPaginatedModelDefinitionsByFilterAsync(int pageIndex, int pageSize, string name);

        Task<ModelDefinitionDTO> GetModelDefinitionByIdAsync(int id);

        bool SaveModelDefinition(ModelDefinitionDTO data);
        bool RemoveModelDefinition(int id);
        #endregion

        #region ModelDefinition Module

        Task<List<ModelDefFieldDTO>> FindModulesByAppIdAsync(int defId);
        Task<ModelDefFieldDTO> GetModuleByIdAsync(int id);
        bool AddModule(ModelDefFieldDTO data);
        bool SaveModule(ModelDefFieldDTO data);
        bool RemoveModuleById(int id);
        #endregion

    }

    public class ModelDefinitionService : EFServiceBase, IModelDefinitionService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        private EFUnitOfWorkContextBase _unitOfContext;

        private IModelDefinitionRepository _applicationRepository;
        private IDbRepository<ModelDefField> _applicationModuleRepository;
        private IRelationDefinitionRepository _applicationBusinessRepository;
        private readonly IConfigApiService ConfigApiService;

        public ModelDefinitionService(
            Tenant tenant,
            IConfigApiService configApiService,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            IModelDefinitionRepository applicationRepository,
            IDbRepository<ModelDefField> applicationModuleRepository,
            IRelationDefinitionRepository applicationBusinessRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<ModelDefinitionService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfContext = unitOfContext;
            ConfigApiService = configApiService;

            _applicationRepository = applicationRepository;
            _applicationModuleRepository = applicationModuleRepository;
            _applicationBusinessRepository = applicationBusinessRepository;
        }

        #region ModelDefinition

        public async Task<List<ModelDefinitionDTO>> FindAllModelDefinitionsAsync()
        {
            var data = await _applicationRepository.GetAllModelDefinitionsAsync();
            return _mapper.Map<List<ModelDefinitionDTO>>(data);
        }

        public async Task<PaginatedBaseDTO<ModelDefinitionDTO>> FindPaginatedModelDefinitionsByFilterAsync(int pageIndex, int pageSize, string name)
        {
            Expression<Func<ModelDefinition, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            //var dbaTenant = TenantUtil.GetDbaTenantUser();
            //var applicationRepository = new ModelDefinitionRespository(dbaTenant);
            var data = await _applicationRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.CreatedDate, true);
            var total = data.Item1;
            var rows = _mapper.Map<List<ModelDefinitionDTO>>(data.Item2);
            return new PaginatedBaseDTO<ModelDefinitionDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<ModelDefinitionDTO> GetModelDefinitionByIdAsync(int id)
        {
            var data = await _applicationRepository.GetByIdAsync(id);
            return _mapper.Map<ModelDefinitionDTO>(data);
        }

        public bool SaveModelDefinition(ModelDefinitionDTO data)
        {
            var model = _mapper.Map<ModelDefinition>(data);
            if (model.PropertyId == 0)
            {
                return _applicationRepository.Add(model);
            }
            else
            {
                return _applicationRepository.Modify(model, true);
            }
        }
        public bool RemoveModelDefinition(int id)
        {
            return _applicationRepository.RemoveById(id);
        }

        #endregion

        #region ModelDefField

        public async Task<List<ModelDefFieldDTO>> FindModulesByAppIdAsync(int defId)
        {
            var data = await _applicationModuleRepository.FindAllAsync(m => m.ModelDefId == defId, m => m.Index, true);
            return _mapper.Map<List<ModelDefFieldDTO>>(data);
        }
        public async Task<ModelDefFieldDTO> GetModuleByIdAsync(int id)
        {
            var data = await _applicationModuleRepository.GetByIdAsync(id);
            return _mapper.Map<ModelDefFieldDTO>(data);
        }
        public bool AddModule(ModelDefFieldDTO data)
        {
            var model = _mapper.Map<ModelDefField>(data);
            return _applicationModuleRepository.Add(model);
        }
        public bool SaveModule(ModelDefFieldDTO data)
        {
            var model = _mapper.Map<ModelDefField>(data);
            if (model.PropertyAttributeId == 0)
            {
                return _applicationModuleRepository.Add(model);
            }
            else
            {
                return _applicationModuleRepository.Modify(model, true);
            }
        }
        public bool RemoveModuleById(int id)
        {
            return _applicationModuleRepository.RemoveById(id);
        }

        #endregion

    }
}
