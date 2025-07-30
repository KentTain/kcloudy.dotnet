using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Database.EFRepository;
using KC.Service.EFService;
using KC.DataAccess.App.Repository;
using KC.Service.DTO.App;
using KC.Service.DTO;
using System.Linq.Expressions;
using KC.Model.App;
using KC.Framework.Extension;
using KC.Database.IRepository;
using KC.Service.WebApiService.Business;
using KC.Framework.Tenant;
using KC.Service.Enums.App;

namespace KC.Service.App
{
    public interface IAppDevelopService : IEFService
    {
        #region 开发模板管理
        Task<List<DevTemplateDTO>> FindAllAppTemplates(TemplateType? type, string name);

        Task<PaginatedBaseDTO<DevTemplateDTO>> FindPaginatedAppTemplates(int pageIndex, int pageSize, TemplateType? type, string name);

        Task<DevTemplateDTO> GetApplicationTemplate(Guid id);
        Task<DevTemplateDTO> GetAppTemplateById(Guid id);

        Task<bool> ExistTemplateName(Guid? id, string name);
        Task<bool> SaveAppTemplate(DevTemplateDTO data);
        Task<bool> RemoveAppTemplate(Guid id);

        #endregion

        #region Git仓库管理
        Task<List<AppGitDTO>> FindAllAppGits(Guid? appId, string address);

        Task<PaginatedBaseDTO<AppGitDTO>> FindPaginatedAppGits(int pageIndex, int pageSize, Guid? appId, string address);

        Task<AppGitDTO> GetAppGitById(Guid id);

        Task<bool> EnableAppGit(Guid id, bool enabled, string userId, string userName);
        bool SaveAppGit(AppGitDTO data);
        bool RemoveAppGit(Guid id);
        #endregion

        #region Git分支管理
        Task<List<AppGitBranchDTO>> FindAllAppGitBranchs(Guid? gitId, string name);

        Task<PaginatedBaseDTO<AppGitBranchDTO>> FindPaginatedAppGitBranchs(int pageIndex, int pageSize, string url);

        Task<AppGitBranchDTO> GetAppGitBranchById(Guid id);

        bool SaveAppGitBranch(AppGitBranchDTO data);
        bool RemoveAppGitBranch(Guid id);

        #endregion

        #region Git用户管理
        Task<List<AppGitUserDTO>> FindAllAppGitUsers(Guid? gitId, string name);

        Task<PaginatedBaseDTO<AppGitUserDTO>> FindPaginatedAppGitUsers(int pageIndex, int pageSize, string url);

        Task<AppGitUserDTO> GetAppGitUserById(Guid id);

        bool SaveAppGitUser(AppGitUserDTO data);
        bool RemoveAppGitUser(Guid id);

        #endregion
    }

    public class AppDevelopService : EFServiceBase, IAppDevelopService
    {
        private readonly IMapper _mapper;

        #region Context && Repository
        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        private EFUnitOfWorkContextBase _unitOfContext;

        private IApplicationRepository _applicationRepository;
        private IDbRepository<DevTemplate> _appTemplateRepository;

        private IDbRepository<AppGit> _AppGitRepository;
        private IDbRepository<AppGitUser> _AppGitUserRepository;
        private IDbRepository<AppGitBranch> _AppGitBranchRepository;

        private IDbRepository<ApplicationLog> _applicationLogRepository;

        private readonly IConfigApiService ConfigApiService;

        public AppDevelopService(
            Tenant tenant,
            IConfigApiService configApiService,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            IApplicationRepository applicationRepository,
            IDbRepository<DevTemplate> appTemplateRepository,
            IDbRepository<AppGit> AppGitRepository,
            IDbRepository<AppGitUser> AppGitUserRepository,
            IDbRepository<AppGitBranch> AppGitBranchRepository,
            IDbRepository<ApplicationLog> applicationLogRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<ApplicationService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfContext = unitOfContext;
            ConfigApiService = configApiService;

            _applicationRepository = applicationRepository;
            _appTemplateRepository = appTemplateRepository;
            _AppGitRepository = AppGitRepository;
            _AppGitUserRepository = AppGitUserRepository;
            _AppGitBranchRepository = AppGitBranchRepository;
            _applicationLogRepository = applicationLogRepository;
        }
        #endregion

        #region 开发模板管理

        public async Task<List<DevTemplateDTO>> FindAllAppTemplates(TemplateType? type, string name)
        {
            Expression<Func<DevTemplate, bool>> predicate = m => !m.IsDeleted;
            if (type.HasValue)
            {
                KC.Enums.App.TemplateType t = Enum.Parse<KC.Enums.App.TemplateType>(type.Value.ToString(), true);
                predicate = predicate.And(m => m.Type == t);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            var data = await _appTemplateRepository.FindAllAsync(predicate, m => m.ModifiedDate, false);
            return _mapper.Map<List<DevTemplateDTO>>(data);
        }

        public async Task<PaginatedBaseDTO<DevTemplateDTO>> FindPaginatedAppTemplates(int pageIndex, int pageSize, TemplateType? type, string name)
        {
            Expression<Func<DevTemplate, bool>> predicate = m => !m.IsDeleted;
            if (type.HasValue)
            {
                KC.Enums.App.TemplateType t = Enum.Parse<KC.Enums.App.TemplateType>(type.Value.ToString(), true);
                predicate = predicate.And(m => m.Type == t);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            var data = await _appTemplateRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.ModifiedDate, false);
            var total = data.Item1;
            var rows = _mapper.Map<List<DevTemplateDTO>>(data.Item2);
            return new PaginatedBaseDTO<DevTemplateDTO>(pageIndex, pageSize, total, rows);
        }
        public async Task<DevTemplateDTO> GetApplicationTemplate(Guid id)
        {
            var data = await _applicationRepository.GetApplicationWithTemplateById(id);
            if (null == data.AppTemplate)
                return null;

            return _mapper.Map<DevTemplateDTO>(data.AppTemplate);
        }

        public async Task<DevTemplateDTO> GetAppTemplateById(Guid id)
        {
            var data = await _appTemplateRepository.GetByIdAsync(id);
            return _mapper.Map<DevTemplateDTO>(data);
        }
        public async Task<bool> ExistTemplateName(Guid? id, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "开发模板名称为空.");

            Expression<Func<DevTemplate, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            if (id.HasValue)
                predicate = predicate.And(c => c.Id != id.Value);

            return await _appTemplateRepository.ExistByFilterAsync(predicate);
        }
        public async Task<bool> SaveAppTemplate(DevTemplateDTO data)
        {
            var model = _mapper.Map<DevTemplate>(data);
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
                return await _appTemplateRepository.AddAsync(model);
            }
            else
            {
                return await _appTemplateRepository.ModifyAsync(model, true);
            }
        }
        public async Task<bool> RemoveAppTemplate(Guid id)
        {
            return await _appTemplateRepository.RemoveByIdAsync(id);
        }

        #endregion

        #region Git仓库管理

        public async Task<List<AppGitDTO>> FindAllAppGits(Guid? appId, string address)
        {
            Expression<Func<AppGit, bool>> predicate = m => !m.IsDeleted;
            if (appId.HasValue && appId != Guid.Empty)
            {
                predicate = predicate.And(m => m.ApplicationId.Equals(appId.Value));
            }
            if (!string.IsNullOrWhiteSpace(address))
            {
                predicate = predicate.And(m => m.GitAddress.Contains(address));
            }
            var data = await _AppGitRepository.FindAllAsync(predicate, m => m.ModifiedDate, false);
            return _mapper.Map<List<AppGitDTO>>(data);
        }

        public async Task<PaginatedBaseDTO<AppGitDTO>> FindPaginatedAppGits(int pageIndex, int pageSize, Guid? appId, string address)
        {
            Expression<Func<AppGit, bool>> predicate = m => !m.IsDeleted;
            if (appId.HasValue && appId != Guid.Empty)
            {
                predicate = predicate.And(m => m.ApplicationId.Equals(appId.Value));
            }
            if (!string.IsNullOrWhiteSpace(address))
            {
                predicate = predicate.And(m => m.GitAddress.Contains(address));
            }
            var data = await _AppGitRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.ModifiedDate, true);
            var total = data.Item1;
            var rows = _mapper.Map<List<AppGitDTO>>(data.Item2);
            return new PaginatedBaseDTO<AppGitDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<AppGitDTO> GetAppGitById(Guid id)
        {
            var data = await _AppGitRepository.GetByIdAsync(id);
            return _mapper.Map<AppGitDTO>(data);
        }

        public bool SaveAppGit(AppGitDTO data)
        {
            var model = _mapper.Map<AppGit>(data);
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
                return _AppGitRepository.Add(model);
            }
            else
            {
                return _AppGitRepository.Modify(model, true);
            }
        }
        public bool RemoveAppGit(Guid id)
        {
            return _AppGitRepository.RemoveById(id);
        }
        public async Task<bool> EnableAppGit(Guid id, bool enabled, string userId, string userName)
        {
            var data = await _AppGitRepository.GetByIdAsync(id);
            data.IsActived = enabled;
            data.ModifiedBy = userId;
            data.ModifiedName = userName;
            data.ModifiedDate = DateTime.UtcNow;
            return await _AppGitRepository.ModifyAsync(data, new string[] { "IsActived", "ModifiedBy", "ModifiedName", "ModifiedDate" });
        }
        #endregion

        #region Git分支管理

        public async Task<List<AppGitBranchDTO>> FindAllAppGitBranchs(Guid? gitId, string name)
        {
            Expression<Func<AppGitBranch, bool>> predicate = m => !m.IsDeleted;
            if (gitId.HasValue)
            {
                predicate = predicate.And(m => m.AppGitId.Equals(gitId.Value));
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            var data = await _AppGitBranchRepository.FindAllAsync(predicate, m => m.ModifiedDate, false);
            return _mapper.Map<List<AppGitBranchDTO>>(data);
        }

        public async Task<PaginatedBaseDTO<AppGitBranchDTO>> FindPaginatedAppGitBranchs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<AppGitBranch, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            var data = await _AppGitBranchRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.ModifiedDate, true);
            var total = data.Item1;
            var rows = _mapper.Map<List<AppGitBranchDTO>>(data.Item2);
            return new PaginatedBaseDTO<AppGitBranchDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<AppGitBranchDTO> GetAppGitBranchById(Guid id)
        {
            var data = await _AppGitBranchRepository.GetByIdAsync(id);
            return _mapper.Map<AppGitBranchDTO>(data);
        }

        public bool SaveAppGitBranch(AppGitBranchDTO data)
        {
            var model = _mapper.Map<AppGitBranch>(data);
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
                return _AppGitBranchRepository.Add(model);
            }
            else
            {
                return _AppGitBranchRepository.Modify(model, true);
            }
        }
        public bool RemoveAppGitBranch(Guid id)
        {
            return _AppGitBranchRepository.RemoveById(id);
        }

        #endregion

        #region Git用户管理

        public async Task<List<AppGitUserDTO>> FindAllAppGitUsers(Guid? gitId, string name)
        {
            Expression<Func<AppGitUser, bool>> predicate = m => !m.IsDeleted;
            if (gitId.HasValue)
            {
                predicate = predicate.And(m => m.AppGitId.Equals(gitId.Value));
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.UserAccount.Contains(name));
            }
            var data = await _AppGitUserRepository.FindAllAsync(predicate, m => m.ModifiedDate, false);
            return _mapper.Map<List<AppGitUserDTO>>(data);
        }

        public async Task<PaginatedBaseDTO<AppGitUserDTO>> FindPaginatedAppGitUsers(int pageIndex, int pageSize, string name)
        {
            Expression<Func<AppGitUser, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.UserAccount.Contains(name));
            }
            var data = await _AppGitUserRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.ModifiedDate, true);
            var total = data.Item1;
            var rows = _mapper.Map<List<AppGitUserDTO>>(data.Item2);
            return new PaginatedBaseDTO<AppGitUserDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<AppGitUserDTO> GetAppGitUserById(Guid id)
        {
            var data = await _AppGitUserRepository.GetByIdAsync(id);
            return _mapper.Map<AppGitUserDTO>(data);
        }

        public bool SaveAppGitUser(AppGitUserDTO data)
        {
            var model = _mapper.Map<AppGitUser>(data);
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
                return _AppGitUserRepository.Add(model);
            }
            else
            {
                return _AppGitUserRepository.Modify(model, true);
            }
        }
        public bool RemoveAppGitUser(Guid id)
        {
            return _AppGitUserRepository.RemoveById(id);
        }

        #endregion
    }
}
