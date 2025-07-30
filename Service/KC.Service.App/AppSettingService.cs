using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

using AutoMapper;
using KC.Database.EFRepository;
using KC.Service.EFService;
using KC.Service.DTO.App;
using KC.Service.DTO;
using KC.Model.App;
using KC.Framework.Extension;
using KC.Database.IRepository;
using KC.Service.WebApiService.Business;
using KC.Framework.Tenant;
using KC.DataAccess.App.Repository;

namespace KC.Service.App
{
    public interface IAppSettingService : IEFService
    {
        #region 应用配置定义
        Task<List<AppSettingDTO>> FindApplicationSettings(Guid appId);
        Task<List<AppSettingDTO>> FindAllAppSettings(Guid? appId, string name);
        Task<PaginatedBaseDTO<AppSettingDTO>> FindPaginatedAppSettings(int pageIndex, int pageSize, Guid? appId, string name);

        Task<AppSettingDTO> GetAppSettingById(int id, Guid? appId);

        Task<bool> ExistAppSetCode(int id, Guid? appId, string code);
        Task<bool> ExistAppSetName(int id, Guid? appId, string name);
        Task<bool> SaveAppSetting(AppSettingDTO data);
        Task<bool> SaveAppSettingAttrs(List<AppSettingDTO> model);
        Task<bool> RemoveAppSettingById(int id, string userId, string userName);
        #endregion

        #region 应用配置属性定义
        Task<List<AppSettingPropertyDTO>> FindAppSettingPropertysBySettingId(int settingId);

        Task<AppSettingPropertyDTO> GetAppSettingPropertyById(int id);

        Task<bool> ExistAppSetPropName(int id, int settingId, string name);
        Task<bool> SaveAppSettingProperty(AppSettingPropertyDTO data);
        Task<bool> RemoveAppSettingPropertyById(int id);
        #endregion
    }

    public class AppSettingService : EFServiceBase, IAppSettingService
    {
        #region Context && Repository
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        private EFUnitOfWorkContextBase _unitOfContext;
        private readonly IConfigApiService ConfigApiService;

        private IAppSettingRepository _appSettingRepository;
        private IAppSettingPropertyRepository _appSettingPropertyRepository;
        private IDbRepository<ApplicationLog> _appLogRepository;

        public AppSettingService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            IConfigApiService configApiService,

            IAppSettingRepository appSettingRepository,
            IAppSettingPropertyRepository appSettingPropertyRepository,
            IDbRepository<ApplicationLog> appLogRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<ApplicationService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfContext = unitOfContext;
            ConfigApiService = configApiService;

            _appSettingRepository = appSettingRepository;
            _appSettingPropertyRepository = appSettingPropertyRepository;
            _appLogRepository = appLogRepository;
        }
        #endregion

        #region 应用配置定义
        public async Task<List<AppSettingDTO>> FindApplicationSettings(Guid appId)
        { 
            var data = await _appSettingRepository.FindAppSettingsWithAttributesByAppIdAsync(appId);
            return _mapper.Map<List<AppSettingDTO>>(data);
        }
        public async Task<List<AppSettingDTO>> FindAllAppSettings(Guid? appId, string name)
        {
            Expression<Func<AppSetting, bool>> predicate = m => !m.IsDeleted;
            if (appId.HasValue && appId != Guid.Empty)
            {
                predicate = predicate.And(m => m.ApplicationId == appId);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }

            var data = await _appSettingRepository.FindAllAsync(predicate, m => m.ModifiedDate, false);
            return _mapper.Map<List<AppSettingDTO>>(data);
        }
        public async Task<PaginatedBaseDTO<AppSettingDTO>> FindPaginatedAppSettings(int pageIndex, int pageSize, Guid? appId, string name)
        {
            Expression<Func<AppSetting, bool>> predicate = m => !m.IsDeleted;
            if (appId.HasValue && appId != Guid.Empty)
            {
                predicate = predicate.And(m => m.ApplicationId == appId);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }

            var data = await _appSettingRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.ModifiedDate, false);
            var total = data.Item1;
            var rows = _mapper.Map<List<AppSettingDTO>>(data.Item2);
            return new PaginatedBaseDTO<AppSettingDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<AppSettingDTO> GetAppSettingById(int id, Guid? appId)
        {
            if (0 == id)
            {
                var defaultApp = new AppSettingDTO()
                {
                    IsEditMode = false,
                    Code = ConfigApiService.GetSeedCodeByName("AppSetting"),
                    IsRequire = false,
                    CanEdit = true,
                };
                if (appId.HasValue)
                    defaultApp.ApplicationId = appId.Value;
                return defaultApp;
            }

            var data = await _appSettingRepository.GetByIdAsync(id);
            var result = _mapper.Map<AppSettingDTO>(data);
            result.IsEditMode = true;
            return result;
        }

        public async Task<bool> ExistAppSetCode(int id, Guid? appId, string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("Code", "应用编码名称为空.");

            Expression<Func<AppSetting, bool>> predicate = m => !m.IsDeleted && m.Code.Equals(code);
            if (appId.HasValue)
                predicate = predicate.And(c => c.ApplicationId == appId.Value);
            if (0 != id)
                predicate = predicate.And(c => c.PropertyId != id);

            return await _appSettingRepository.ExistByFilterAsync(predicate);
        }

        public async Task<bool> ExistAppSetName(int id, Guid? appId, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Name", "应用名称为空.");

            Expression<Func<AppSetting, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            if (appId.HasValue)
                predicate = predicate.And(c => c.ApplicationId == appId.Value);
            if (0 != id)
                predicate = predicate.And(c => c.PropertyId != id);

            return await _appSettingRepository.ExistByFilterAsync(predicate);
        }

        public async Task<bool> SaveAppSetting(AppSettingDTO model)
        {
            var data = _mapper.Map<AppSetting>(model);
            //新增表单定义（新增表单定义及相关表单数据）
            if (!model.IsEditMode)
            {
                //保存表单表单，只保存两级的Field，超出不再保存
                var addFields = new List<AppSettingProperty>();
                foreach (var field in data.PropertyAttributeList)
                {
                    AttachField(data, field);
                    addFields.Add(field);
                }

                await _appSettingPropertyRepository.AddAsync(addFields, false);
                await _appSettingRepository.AddAsync(data, false);

                //添加日志
                var log = new ApplicationLog()
                {
                    ApplicationId = model.ApplicationId,
                    ApplicationName = model.ApplicationName,
                    appLogType = KC.Enums.App.AppLogType.AppSetting,
                    OperatorId = model.CreatedBy,
                    Operator = model.CreatedName,
                    Remark = string.Format("新增配置数据: " + data.Name)
                };
                await _appLogRepository.AddAsync(log, false);
            }
            //修改表单定义（保存新的表单属性数据，同时更新表单定义数据）
            else
            {
                var dbAppSetting = await _appSettingRepository.GetAppSettingWithAttributesByIdAsync(data.PropertyId);
                dbAppSetting.Name = data.Name;
                dbAppSetting.DisplayName = data.DisplayName;
                dbAppSetting.Description = data.Description;
                dbAppSetting.Index = data.Index;
                dbAppSetting.ModifiedBy = data.ModifiedBy;
                dbAppSetting.ModifiedName = data.ModifiedName;
                dbAppSetting.ModifiedDate = data.ModifiedDate;
                List<int> newIdList = data.PropertyAttributeList.Select(m => m.PropertyAttributeId).ToList();
                List<int> dbIdList = dbAppSetting.PropertyAttributeList.Select(m => m.PropertyAttributeId).ToList();
                List<int> addList = newIdList.Except(dbIdList).ToList();
                List<int> delList = dbIdList.Except(newIdList).ToList();
                List<int> updateList = dbIdList.Intersect(newIdList).ToList();

                //添加新的字段属性
                var addFields = data.PropertyAttributeList
                        .Where(m => addList.Contains(m.PropertyAttributeId))
                        .ToList();
                foreach (var addField in addFields)
                {
                    AttachField(data, addField);
                    dbAppSetting.PropertyAttributeList.Add(addField);
                }

                //更新原有的字段属性
                var updateFields = dbAppSetting.PropertyAttributeList
                        .Where(m => updateList.Contains(m.PropertyAttributeId))
                        .ToList();
                foreach (var dbUpdateField in updateFields)
                {
                    var newField = data.PropertyAttributeList
                            .Where(m => dbUpdateField.PropertyAttributeId == m.PropertyAttributeId)
                            .FirstOrDefault();
                    if (null != newField)
                    {
                        dbUpdateField.Name = newField.Name;
                        dbUpdateField.DisplayName = newField.DisplayName;
                        dbUpdateField.Description = newField.Description;
                        dbUpdateField.Index = newField.Index;
                        dbUpdateField.DataType = newField.DataType;
                        dbUpdateField.CanEdit = newField.CanEdit;
                        dbUpdateField.IsRequire = newField.IsRequire;
                        dbUpdateField.Ext1 = newField.Ext1;
                        dbUpdateField.Ext2 = newField.Ext2;
                        dbUpdateField.Ext3 = newField.Ext3;
                        dbUpdateField.ModifiedBy = data.ModifiedBy;
                        dbUpdateField.ModifiedName = data.ModifiedName;
                        dbUpdateField.ModifiedDate = data.ModifiedDate;
                    }
                }

                //删除已经不存在的字段属性
                var delFields = dbAppSetting.PropertyAttributeList
                        .Where(m => delList.Contains(m.PropertyAttributeId))
                        .ToList();
                foreach (var delField in delFields)
                {
                    dbAppSetting.PropertyAttributeList.Remove(delField);
                    _appSettingPropertyRepository.Remove(delField, false);
                }

                _unitOfContext.Context.Update<AppSetting>(dbAppSetting);

                //添加日志
                var log = new ApplicationLog()
                {
                    ApplicationId = model.ApplicationId,
                    ApplicationName = model.ApplicationName,
                    appLogType = KC.Enums.App.AppLogType.AppSetting,
                    OperatorId = model.ModifiedBy,
                    Operator = model.ModifiedName,
                    Remark = string.Format("编辑应用配置数据: " + data.Name)
                };
                await _appLogRepository.AddAsync(log, false);
            }

            return await _unitOfContext.CommitAsync() > 0;
        }
        private void AttachField(AppSetting data, AppSettingProperty addField)
        {
            addField.PropertyAttributeId = 0;
            addField.AppSetting = data;
            addField.AppSettingCode = data.Code;
            addField.AppSettingId = data.PropertyId;
            addField.IsDeleted = false;
            addField.CreatedBy = data.CreatedBy;
            addField.CreatedName = data.CreatedName;
            addField.CreatedDate = data.CreatedDate;
            addField.ModifiedBy = data.ModifiedBy;
            addField.ModifiedName = data.ModifiedName;
            addField.ModifiedDate = data.ModifiedDate;
        }

        public async Task<bool> SaveAppSettingAttrs(List<AppSettingDTO> model)
        {
            var data = _mapper.Map<List<AppSetting>>(model);
            var addFields = data.SelectMany(m => m.PropertyAttributeList).ToList();
            return await _appSettingPropertyRepository.ModifyAsync(addFields, new string[] { "Value" }, true) > 0;
        }

        public async Task<bool> RemoveAppSettingById(int id, string userId, string userName)
        {
            var model = _appSettingRepository.GetById(id);
            var success = _appSettingRepository.RemoveByIdAsync(id);
            //添加日志
            var log = new ApplicationLog()
            {
                ApplicationId = model.ApplicationId,
                ApplicationName = model.Application.ApplicationName,
                appLogType = KC.Enums.App.AppLogType.AppSetting,
                OperatorId = userId,
                Operator = userName,
                Remark = string.Format("删除配置数据: " + model.Name)
            };
            await _appLogRepository.AddAsync(log, false);
            return true;
        }
        #endregion

        #region 应用配置属性定义
        public async Task<List<AppSettingPropertyDTO>> FindAppSettingPropertysBySettingId(int settingId)
        {
            Expression<Func<AppSettingProperty, bool>> predicate = m => !m.IsDeleted && m.AppSettingId == settingId;
            var data = await _appSettingPropertyRepository.FindAllAsync(predicate, m => m.Index, true);
            return _mapper.Map<List<AppSettingPropertyDTO>>(data);
        }

        public async Task<AppSettingPropertyDTO> GetAppSettingPropertyById(int id)
        {
            var data = await _appSettingPropertyRepository.GetWithSettingByIdAsync(id);
            return _mapper.Map<AppSettingPropertyDTO>(data);
        }

        public async Task<bool> ExistAppSetPropName(int id, int settingId, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Name", "应用名称为空.");

            Expression<Func<AppSettingProperty, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            if (0 != settingId)
                predicate = predicate.And(c => c.AppSettingId == settingId);
            if (0 != id)
                predicate = predicate.And(c => c.PropertyAttributeId != id);

            return await _appSettingPropertyRepository.ExistByFilterAsync(predicate);
        }
        public async Task<bool> SaveAppSettingProperty(AppSettingPropertyDTO data)
        {
            var model = _mapper.Map<AppSettingProperty>(data);
            if (0 == model.PropertyAttributeId || !data.IsEditMode)
            {
                return await _appSettingPropertyRepository.AddAsync(model);
            }
            else
            {
                return await _appSettingPropertyRepository.ModifyAsync(model, true);
            }
        }
        public async Task<bool> RemoveAppSettingPropertyById(int id)
        {
            return await _appSettingPropertyRepository.RemoveByIdAsync(id);
        }
        #endregion
    }
}
