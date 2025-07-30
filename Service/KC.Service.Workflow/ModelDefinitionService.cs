using AutoMapper;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Workflow;
using KC.Service.DTO;
using KC.Service.Workflow.DTO;
using KC.Service.EFService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KC.Service.Workflow
{
    public interface IModelDefinitionService : IEFService
    {
        #region 表单定义
        PaginatedBaseDTO<ModelDefinitionDTO> FindPaginatedModelDefinitionsByName(int pageIndex, int pageSize, string name, BusinessType? type);

        ModelDefinitionDTO GetModelDefinitionById(int id);

        Task<bool> SaveModelDefinitionWithFieldsAsync(ModelDefinitionDTO model);
        Task<bool> RemoveModelDefinitionByIdAsync(int id, string currentUserId, string currentUserName);
        #endregion

        #region 表单属性
        List<ModelDefFieldDTO> FindAllModelDefFieldsByDefId(int defId);

        Task<bool> RemoveModelDefFieldByIdAsync(int propertyId, string currentUserId, string currentUserName);
        #endregion

        PaginatedBaseDTO<ModelDefLogDTO> FindPaginatedModelDefLogs(int pageIndex, int pageSize, string name);
    }

    public class ModelDefinitionService : EFServiceBase, IModelDefinitionService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private IDbRepository<ModelDefinition> _modelDefinitionRepository;
        private IDbRepository<ModelDefField> _modelDefFieldRepository;
        private IDbRepository<ModelDefLog> _modelDefLogRepository;

        public ModelDefinitionService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IDbRepository<ModelDefinition> modelDefRepository,
            IDbRepository<ModelDefField> modelDefFieldRepository,
            IDbRepository<ModelDefLog> modelDefLogRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<ModelDefinitionService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;

            _modelDefinitionRepository = modelDefRepository;
            _modelDefFieldRepository = modelDefFieldRepository;
            _modelDefLogRepository = modelDefLogRepository;
        }

        #region 表单定义
        public PaginatedBaseDTO<ModelDefinitionDTO> FindPaginatedModelDefinitionsByName(int pageIndex, int pageSize, string name, BusinessType? type)
        {
            Expression<Func<ModelDefinition, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            if (type.HasValue)
            {
                predicate = predicate.And(m => m.BusinessType == type.Value);
            }

            var data = _modelDefinitionRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.Name, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<ModelDefinitionDTO>>(data.Item2);
            return new PaginatedBaseDTO<ModelDefinitionDTO>(pageIndex, pageSize, total, rows);
        }

        public ModelDefinitionDTO GetModelDefinitionById(int id)
        {
            var data = _modelDefinitionRepository.GetById(id);
            return _mapper.Map<ModelDefinitionDTO>(data);
        }

        public async Task<bool> SaveModelDefinitionWithFieldsAsync(ModelDefinitionDTO model)
        {
            var data = _mapper.Map<ModelDefinition>(model);
            //新增表单定义（新增表单定义及相关表单数据）
            var addFields = new List<ModelDefField>();
            if (!model.IsEditMode)
            {
                //保存表单表单，只保存两级的Field，超出不再保存
                foreach (var field in data.PropertyAttributeList)
                {
                    field.PropertyAttributeId = 0;
                    field.ModelDefId = data.PropertyId;
                    field.ModelDefinition = data;
                    addFields.Add(field);
                }

                await _modelDefFieldRepository.AddAsync(addFields, false);
                await _modelDefinitionRepository.AddAsync(data, false);

                //添加日志
                var log = new ModelDefLog()
                {
                    ModelDefId = data.PropertyId,
                    ModelDefName = data.Name,
                    OperatorId = model.CreatedBy,
                    Operator = model.CreatedName,
                    Remark = string.Format("新增表单定义数据: " + data.Name)
                };
                await _modelDefLogRepository.AddAsync(log, false);
            }
            //修改表单定义（保存新的表单属性数据，同时更新表单定义数据）
            else
            {
                _modelDefFieldRepository.Remove(m => m.ModelDefId.Equals(data.PropertyId), false);
                //删除老的表单属性数据，插入新的表单属性数据
                if (data.PropertyAttributeList.Any())
                {
                    //保存表单属性
                    foreach (var field in data.PropertyAttributeList)
                    {
                        field.PropertyAttributeId = 0;
                        field.ModelDefId = data.PropertyId;
                        field.ModelDefinition = data;
                        addFields.Add(field);
                    }
                }

                await _modelDefFieldRepository.AddAsync(addFields, false);
                await _modelDefinitionRepository.ModifyAsync(data, new string[]
                     { "Name", "BusinessType", "Description",
                         "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);

                //添加日志
                var log = new ModelDefLog()
                {
                    ModelDefId = data.PropertyId,
                    ModelDefName = data.Name,
                    OperatorId = model.ModifiedBy,
                    Operator = model.ModifiedName,
                    Remark = string.Format("编辑表单定义数据: " + data.Name)
                };
                await _modelDefLogRepository.AddAsync(log, false);
            }

            return await _unitOfWorkContext.CommitAsync() > 0;
        }

        public async Task<bool> RemoveModelDefinitionByIdAsync(int id, string currentUserId, string currentUserName)
        {
            var data = await _modelDefinitionRepository.GetByIdAsync(id);
            if (data != null)
            {
                data.IsDeleted = true;
                data.ModifiedBy = currentUserId;
                data.ModifiedName = currentUserName;
                data.ModifiedDate = DateTime.UtcNow;
                await _modelDefinitionRepository.ModifyAsync(data, new string[] { "IsDeleted",
                         "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);
            }

            var list = await _modelDefFieldRepository.FindAllAsync(m => m.ModelDefId == id);
            if (list.Any())
            {
                list.ToList().ForEach(m =>
                {
                    m.IsDeleted = true;
                    m.ModifiedBy = currentUserId;
                    m.ModifiedName = currentUserName;
                    m.ModifiedDate = DateTime.UtcNow;
                });
                await _modelDefFieldRepository.ModifyAsync(list, new string[] { "IsDeleted",
                         "ModifiedBy", "ModifiedName", "ModifiedDate" }, false);
            }
            var log = new ModelDefLog()
            {
                ModelDefId = data.PropertyId,
                ModelDefName = data.Name,
                OperatorId = currentUserId,
                Operator = currentUserName,
                Remark = string.Format("删除表单定义数据: " + data.Name)
            };
            await _modelDefLogRepository.AddAsync(log, false);

            return _unitOfWorkContext.Commit() > 0;
        }
        #endregion

        #region 表单属性
        public List<ModelDefFieldDTO> FindAllModelDefFieldsByDefId(int defId)
        {
            Expression<Func<ModelDefField, bool>> predicate = m => !m.IsDeleted && m.ModelDefId == defId;
            var data = _modelDefFieldRepository.FindAll(predicate);
            return _mapper.Map<List<ModelDefFieldDTO>>(data);
        }

        public async Task<bool> RemoveModelDefFieldByIdAsync(int propertyId, string currentUserId, string currentUserName)
        {
            var data = await _modelDefFieldRepository.GetByIdAsync(propertyId);
            data.IsDeleted = true;
            return await _modelDefFieldRepository.ModifyAsync(data, new string[] { "IsDeleted" });
        }
        #endregion

        #region 表单日志
        public PaginatedBaseDTO<ModelDefLogDTO> FindPaginatedModelDefLogs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<ModelDefLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _modelDefLogRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.OperateDate, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<ModelDefLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<ModelDefLogDTO>(pageIndex, pageSize, total, rows);

        }
        #endregion
    }
}
