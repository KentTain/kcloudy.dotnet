using AutoMapper;
using KC.DataAccess.Account.Repository;
using KC.Database.IRepository;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Account;
using KC.Service.EFService;
using KC.Service.DTO.Account;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Database.EFRepository;
using Microsoft.Extensions.Logging;
using KC.Service.WebApiService.Business;

namespace KC.Service.Account
{
    public class PermissionService : EFServiceBase, IPermissionService
    {
        private readonly IMapper _mapper;

        #region Db Repository
        private EFUnitOfWorkContextBase _unitOfContext;
        private IPermissionRepository _permissionRepository;
        private IDbRepository<UserTracingLog> _userTracingLogRepository;

        #endregion

        //public PermissionService(
        //    Tenant tenant,
        //    System.Net.Http.IHttpClientFactory clientFactory)
        //    : base(tenant, clientFactory)
        //{
        //    var unitOfWorkContext = new ComAccountUnitOfWorkContext(tenant);
        //    _permissionRepository = new PermissionRepository(unitOfWorkContext);
        //    _userTracingLogRepository = new CommonEFRepository<UserTracingLog>(unitOfWorkContext);
        //}

        public PermissionService(
            Tenant tenant,
            IPermissionRepository permissionRepository,
            IDbRepository<UserTracingLog> userTracingLogRepository,

            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<PermissionService> logger)
            : base(tenant, clientFactory, logger)
        {
            //AccountUnitOfWorkContext = new ComAccountUnitOfWorkContext(base.Tenant);

            _mapper = mapper;
            _unitOfContext = unitOfContext;
            _permissionRepository = permissionRepository;
            _userTracingLogRepository = userTracingLogRepository;
        }

        public async Task<List<PermissionSimpleDTO>> FindAllSimplePermissionTreesAsync()
        {
            var permissions = await _permissionRepository.FindAllTreeNodeWithNestChildAsync();
            var groupPermissions = permissions
                .GroupBy(m => m.ApplicationName)
                .Select(k => new { Name = k.Key, Permissions = k.ToList() });
            var result = new List<Permission>();
            foreach (var group in groupPermissions)
            {
                var appGuid = group.Permissions[0].ApplicationId.ToByteArray();
                var appId = BitConverter.ToInt32(appGuid);
                var appName = group.Name;
                var permission = group.Permissions;
                group.Permissions.ForEach(m => m.ParentId = appId);
                result.Add(new Permission() { Id = appId, Name = appName, ChildNodes = permission, Level = 0 });
            }
            return _mapper.Map<List<PermissionSimpleDTO>>(result);
        }

        public List<PermissionSimpleDTO> FindAllPermissions()
        {
            Expression<Func<Permission, bool>> predicate = m => !m.IsDeleted;

            var model = _permissionRepository.FindAll(predicate, m => m.Index, true);

            return _mapper.Map<List<PermissionSimpleDTO>>(model);
        }

        public List<PermissionDTO> FindPermissionsByRoleIds(List<string> roleIds)
        {
            var model = _permissionRepository.GetPermissionsByRoleIds(roleIds);
            return _mapper.Map<List<PermissionDTO>>(model);
        }
        public List<PermissionDTO> FindPermissionsByRoleNames(List<string> roleNames)
        {
            var model = _permissionRepository.GetPermissionsByRoleNames(roleNames);
            return _mapper.Map<List<PermissionDTO>>(model);
        }

        public PaginatedBaseDTO<PermissionDTO> FindPaginatedPermissions(string key, string value, int pageIndex, int pageSize, string sidx, bool isAscent)
        {
            Expression<Func<Permission, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrEmpty(value))
            {
                switch (value.ToLower())
                {
                    case "actionname":
                        predicate = predicate.And(m => m.ActionName.Contains(value));
                        break;
                    case "controllername":
                        predicate = predicate.And(m => m.ControllerName.Contains(value));
                        break;
                }
            }
            var result = _permissionRepository.FindPagenatedListWithCount<Permission>(pageIndex, pageSize,
                        predicate,
                        sidx, isAscent);
            var total = result.Item1;
            var data = _mapper.Map<List<PermissionDTO>>(result.Item2);
            return new PaginatedBaseDTO<PermissionDTO>(pageIndex, pageSize, total, data);
        }

        public async Task<List<PermissionSimpleDTO>> FindRootPermissionsByNameAsync(string name)
        {
            Expression<Func<Permission, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrEmpty(name))
                predicate = predicate.And(m => m.Name.Contains(name));
            var permissions = await _permissionRepository.FindTreeNodesWithNestParentAndChildByFilterAsync(predicate);
            var groupPermissions = permissions
                .GroupBy(m => m.ApplicationName)
                .Select(k => new { Name = k.Key, Permissions = k.ToList() });
            var result = new List<Permission>();
            foreach (var group in groupPermissions)
            {
                var appGuid = group.Permissions[0].ApplicationId.ToByteArray();
                var appId = BitConverter.ToInt32(appGuid);
                var appName = group.Name;
                var permission = group.Permissions;
                group.Permissions.ForEach(m => m.ParentId = appId);
                result.Add(new Permission() { Id = appId, Name = appName, ChildNodes = permission, Level = 0 });
            }

            return _mapper.Map<List<PermissionSimpleDTO>>(result);
        }

        public PermissionDTO GetDetailPermissionById(int id)
        {
            var model = _permissionRepository.GetDetailPermissionById(id);
            return _mapper.Map<PermissionDTO>(model);
        }
        public PermissionDTO GetPermissionByAppIDAndName(Guid? appId, string controllerName, string actionName)
        {
            var data = _permissionRepository.GetPermissionByAppIDAndName(appId, controllerName, actionName);
            return _mapper.Map<PermissionDTO>(data);
        }

        /// <summary>
        /// 判断重复权限
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public bool ExistsPermission(Guid appId, string controllerName, string actionName)
        {
            return _permissionRepository.ExistsPermission(appId, actionName, controllerName);
        }

        public async Task<bool> ExistPermissionNameAsync(int pid, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "权限名称为空.");

            Expression<Func<Permission, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            if (pid == 0)
                predicate = predicate.And(c => c.ParentId == null);
            else
                predicate = predicate.And(c => c.ParentId == pid);

            return await _permissionRepository.ExistByFilterAsync(predicate);
        }
        public bool CreatePermission(PermissionDTO model, string operatorId, string operatorName)
        {
            //using (var scope = ServiceProvider.CreateScope())
            {
                //var _unitOfContext = scope.ServiceProvider.GetRequiredService<ComAccountUnitOfWorkContext>();

                var data = _mapper.Map<Permission>(model);
                const string logMessage = "{0}在{1}进行了以下操作：{2}！";
                var log = new UserTracingLog
                {
                    OperatorId = operatorId,
                    Operator = operatorName,
                    OperateDate = DateTime.UtcNow,
                    Type = Framework.Base.ProcessLogType.Success,
                    Remark = string.Format(logMessage,
                        operatorName,
                        DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                        "创建了权限（权限：" + data.Name + ")")
                };
                _userTracingLogRepository.Add(log, false);
                _permissionRepository.Add(data, false);
                var success = _unitOfContext.Commit() > 0;
                if (success)
                {
                    // TODO: 性能问题
                    _permissionRepository.UpdateExtendFields();
                }

                return success;
            }
        }

        public bool UpdatePermission(PermissionDTO model, string operatorId, string operatorName)
        {
            //using (var scope = ServiceProvider.CreateScope())
            {
                //var _unitOfContext = scope.ServiceProvider.GetRequiredService<ComAccountUnitOfWorkContext>();

                var data = _mapper.Map<PermissionDTO, Permission>(model);
                if (data == null)
                {
                    return true;
                }

                const string logMessage = "{0}在{1}进行了以下操作：{2}！";
                var log = new UserTracingLog
                {
                    OperatorId = operatorId,
                    Operator = operatorName,
                    OperateDate = DateTime.UtcNow,
                    Type = Framework.Base.ProcessLogType.Success,
                    Remark = string.Format(logMessage,
                        operatorName,
                        DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                        "更新了权限（权限：" + data.Name + ")")
                };

                _userTracingLogRepository.Add(log, false);
                _permissionRepository.ModifyAsync(data, null, false);
                var success = _unitOfContext.Commit() > 0;
                if (success)
                {
                    // TODO: 性能问题
                    _permissionRepository.UpdateExtendFields();
                }

                return success;
            }
        }

        public bool DeletePermission(int id, string operatorId, string operatorName)
        {
            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            var log = new UserTracingLog
            {
                OperatorId = operatorId,
                Operator = operatorName,
                OperateDate = DateTime.UtcNow,
                Type = Framework.Base.ProcessLogType.Success,
                Remark = string.Format(logMessage,
                    operatorName,
                    DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                    "删除了权限（权限Id-" + id + ")")
            };

            _userTracingLogRepository.Add(log, false);
            _permissionRepository.SoftRemoveById(id, false);
            return _unitOfContext.Commit() > 0;
        }
        public bool UpdateRoleInPermission(int id, List<string> addList, string operatorId, string operatorName)
        {
            return _permissionRepository.SaveRoleInPremission(id, addList, operatorId, operatorName);
        }

        public async Task<bool> SavePermissionsAsync(List<PermissionDTO> models, Guid appId)
        {
            try
            {
                var data = _mapper.Map<List<PermissionDTO>, List<Permission>>(models);
                return await _permissionRepository.SavePermissionsAsync(data, appId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                if (ex.InnerException != null)
                    Logger.LogError(ex.InnerException, ex.InnerException.Message);
                return false;
            }
        }

    }
}
