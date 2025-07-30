using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.DataAccess.Account.Repository;
using KC.Model.Account;
using KC.Service.EFService;
using KC.Service.DTO.Account;
using KC.Service.DTO;
using KC.Database.IRepository;
using Microsoft.Extensions.Logging;
using KC.Service.WebApiService.Business;
using KC.Database.EFRepository;
using KC.Service.DTO.Search;

namespace KC.Service.Account
{
    public sealed class RoleService : EFServiceBase, IRoleService
    {
        private readonly IMapper _mapper;

        #region Db Repository

        private EFUnitOfWorkContextBase _unitOfContext;
        private IRoleRepository _roleRespository;
        private IDbRepository<UserTracingLog> _userTracingLogRepository;

        #endregion

        public RoleService(
            Tenant tenant,
            IRoleRepository roleRespository, 
            IDbRepository<UserTracingLog> userTracingLogRepository,

            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<RoleService> logger)
            : base(tenant, clientFactory, logger)
        {
            //dbContext = new ComAccountDatabaseInitializer().Create(tenant);
            //dbContext.ChangeDatabaseWithTenant(tenant);

            _mapper = mapper;
            _unitOfContext = unitOfContext;
            _roleRespository = roleRespository;
            _userTracingLogRepository = userTracingLogRepository;
        }

        
        public async Task<List<MenuNodeSimpleDTO>> FindUserMenusByRoleIdsAsync(List<string> roleIds)
        {
            Expression<Func<MenuNode, bool>> predict = m => !m.IsDeleted;
            var model = await _roleRespository.FindRolesMenusByIdsAsync(roleIds);
            var menus = model.SelectMany(m => m.RoleMenuNodes).Select(m => m.MenuNode);
            if (menus.Any())
                return _mapper.Map<List<MenuNodeSimpleDTO>>(menus)
                    .Distinct(m => m.Id)
                    .OrderBy(m => m.Index)
                    .ToList();
            return new List<MenuNodeSimpleDTO>();
        }
        public async Task<List<PermissionSimpleDTO>> FindUserPermissionsByRoleIdsAsync(List<string> roleIds)
        {
            var model = await _roleRespository.FindRolesPermissionsByIdsAsync(roleIds);
            var menus = model.SelectMany(m => m.RolePermissions).Select(m => m.Permission);

            if (menus.Any())
                return _mapper.Map<List<PermissionSimpleDTO>>(menus)
                    .Distinct(new PermissionSimpleDTO())
                    .OrderBy(m => m.Index)
                    .ToList();

            return new List<PermissionSimpleDTO>();
        }

        public List<RoleDTO> FindRolesByIds(List<string> roleIds)
        {
            var model = roleIds.Any()
                    ? _roleRespository.FindByFilter(m => !m.IsDeleted && roleIds.Contains(m.Id))
                    : _roleRespository.FindByFilter(m => !m.IsDeleted);
            return _mapper.Map<List<RoleDTO>>(model);
        }
        public List<RoleDTO> FindRolesWithUsersByUserId(string userId)
        {
            Expression<Func<Role, bool>> predicate = m => !m.IsDeleted && m.RoleUsers.Any(r => r.UserId.Equals(userId));
            var model = _roleRespository.FindRoleWithUsersByFilter(predicate, m => m.Name);
            return _mapper.Map<List<RoleDTO>>(model);
        }

        public async Task<List<RoleSimpleDTO>> FindAllSimpleRolesAsync()
        {
            Expression<Func<Role, bool>> predicate = m => !m.IsDeleted;
            var model = await _roleRespository.FindByFilterAsync(predicate);
            return _mapper.Map<List<RoleSimpleDTO>>(model);
        }

        public async Task<List<RoleDTO>> FindRolesWithUsersByRoleIds(IEnumerable<string> roleIds, IEnumerable<string> exceptRoleIds)
        {
            Expression<Func<Role, bool>> predicate = m => !m.IsDeleted;
            if (roleIds.Any())
            {
                predicate = predicate.And(m => roleIds.Contains(m.Id));
            }
            if (exceptRoleIds.Any())
            {
                predicate = predicate.And(m => !exceptRoleIds.Contains(m.Id));
            }

            var model = await _roleRespository.FindRoleWithUsersByFilterAsync(predicate, m => m.Name);
            return _mapper.Map<List<RoleDTO>>(model);
        }

        public PaginatedBaseDTO<RoleDTO> FindPagenatedRoleList(int pageIndex, int pageSize, string displayName, string currentUserId)
        {
            Expression<Func<Role, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                predicate = predicate.And(m => m.DisplayName.Equals(displayName));
            }
            if (!string.IsNullOrWhiteSpace(currentUserId))
            {
                predicate = predicate.And(m => m.CreatedBy.Equals(currentUserId));
            }
            var data = _roleRespository.FindPagenatedRoles(pageIndex, pageSize, predicate, m => m.DisplayName, true, false);
            var total = data.Item1;
            var rows = _mapper.Map<List<RoleDTO>>(data.Item2);
            return new PaginatedBaseDTO<RoleDTO>(pageIndex, pageSize, total, rows);
        }

        public RoleDTO GetDetailRoleByRoleId(string roleId)
        {
            var model = _roleRespository.GetDetailRoleByRoleId(roleId);
            return _mapper.Map<RoleDTO>(model);
        }
        public RoleDTO GetRoleWithUsersByRoleId(string roleId)
        {
            Expression<Func<Role, bool>> predicate = m => !m.IsDeleted && m.Id.Equals(roleId);
            var model = _roleRespository.FindRoleWithUsersByFilter(predicate, m => m.Name).FirstOrDefault();
            return _mapper.Map<RoleDTO>(model);
        }
        public bool ExistsRole(string roleLoweredName)
        {
            return _roleRespository.ExistsRole(m => !m.IsDeleted && m.Name.Equals(roleLoweredName.ToLower()));
        }

        public bool CreateRole(RoleDTO role, string operatorId, string operatorName)
        {
            if (string.IsNullOrEmpty(role.RoleName))
                throw new ArgumentNullException("RoleName", "角色名称为空.");

            var exist = _roleRespository.ExistsRole(m => !m.IsDeleted && m.Name.Equals(role.RoleName.ToLower()));
            if (exist)
                throw new ArgumentException("当前应用已存在该角色,请勿重复提交");

            var model = _mapper.Map<Role>(role);
            var isSuccess = _roleRespository.CreateRole(model);

            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            var log = new UserTracingLog
            {
                OperatorId = operatorId,
                Operator = operatorName,
                OperateDate = DateTime.UtcNow,
                Type = isSuccess
                    ? Framework.Base.ProcessLogType.Success
                    : Framework.Base.ProcessLogType.Failure,
                Remark = string.Format(logMessage,
                    operatorName,
                    DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                    "创建了角色（" + model.Id + ", " + model.Name + ")")
            };
            _userTracingLogRepository.Add(log);

            return isSuccess;
        }
        public bool UpdateRole(RoleDTO role, string operatorId, string operatorName)
        {
            if (string.IsNullOrEmpty(role.RoleName))
                throw new ArgumentNullException("RoleName", "角色名称为空.");

            Expression<Func<Role, bool>> predicate = m => !m.IsDeleted && !m.Id.Equals(role.RoleId) && m.Name.Equals(role.RoleName.ToLower()) ;
            var exist = _roleRespository.ExistsRole(predicate);
            if (exist)
                throw new ArgumentException("当前应用已存在该角色,请勿重复提交");

            var model = _mapper.Map<Role>(role);
            var isSuccess = _roleRespository.UpdateRole(model);

            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            var log = new UserTracingLog
            {
                OperatorId = operatorId,
                Operator = operatorName,
                OperateDate = DateTime.UtcNow,
                Type = isSuccess
                    ? Framework.Base.ProcessLogType.Success
                    : Framework.Base.ProcessLogType.Failure,
                Remark = string.Format(logMessage,
                    operatorName,
                    DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                    "创建了角色（" + model.Id + ", " + model.Name + ")")
            };
            _userTracingLogRepository.Add(log);

            return isSuccess;
        }
        public bool RomoveRole(string roleId, string operatorId, string operatorName)
        {
            return _roleRespository.RomoveRole(roleId, operatorId, operatorName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="newIds">新增的权限</param>
        /// <returns></returns>
        public bool UpdatePermissionInRole(string roleId, List<int> newIds, string operatorId, string operatorName)
        {
            //当前角色
            return _roleRespository.SavePermissionInRole(roleId, newIds, operatorId, operatorName);
        }
        public bool UpdateMenuInRole(string roleId, List<int> newIds, string operatorId, string operatorName)
        {
            return _roleRespository.SaveMenuInRole(roleId, newIds, operatorId, operatorName);
        }
        public bool UpdateUserInRole(string roleId, List<string> addList, List<string> delList, string operatorId, string operatorName)
        {
            return _roleRespository.SaveUserInRole(roleId, addList, delList, operatorId, operatorName);
        }
    }

}
