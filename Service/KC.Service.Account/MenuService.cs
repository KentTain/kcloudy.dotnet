using AutoMapper;
using KC.DataAccess.Account.Repository;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Account;
using KC.Service.EFService;
using KC.Service.DTO.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KC.Service.WebApiService.Business;
using Microsoft.Data.SqlClient;
using System.Text;
using KC.Framework.Exceptions;

namespace KC.Service.Account
{
    public class MenuService : EFServiceBase, IMenuService
    {
        private readonly IMapper _mapper;
        
        #region Db Repository
        private EFUnitOfWorkContextBase _unitOfContext;
        private IMenuNodeRepository _menuNodeRepository;
        private IDbRepository<UserTracingLog> _userTracingLogRepository;

        #endregion

        //public MenuService(
        //    Tenant tenant,
        //    System.Net.Http.IHttpClientFactory clientFactory)
        //    : base(tenant, clientFactory)
        //{
        //    var unitOfWorkContext = new ComAccountUnitOfWorkContext(tenant);
        //    _menuNodeRepository = new MenuNodeRepository(unitOfWorkContext);
        //    _userTracingLogRepository = new CommonEFRepository<UserTracingLog>(unitOfWorkContext);
        //}

        public MenuService(
            Tenant tenant,
            IMenuNodeRepository menuNodeRepository,
            IDbRepository<UserTracingLog> userTracingLogRepository,

            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<MenuService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfContext = unitOfContext;
            _menuNodeRepository = menuNodeRepository;
            _userTracingLogRepository = userTracingLogRepository;
        }


        public List<MenuNodeDTO> FindMenuNodesByIds(List<int> menuIds)
        {
            var data = _menuNodeRepository.GetMenuNodesByIds(menuIds); //.Find(m=>m.ParentId ==null);
            return _mapper.Map<List<MenuNodeDTO>>(data);
        }
        public List<MenuNodeDTO> FindMenuNodesByRoleIds(List<string> roleIds)
        {
            var data = _menuNodeRepository.GetMenuNodesByRoleIds(roleIds);
            return _mapper.Map<List<MenuNodeDTO>>(data);
        }

        public async Task<List<MenuNodeSimpleDTO>> FindAllSimpleMenuTreesAsync()
        {
            Expression<Func<MenuNode, bool>> predicate = m => !m.IsDeleted;
            var data = await _menuNodeRepository.FindTreeNodesWithNestParentAndChildByFilterAsync(predicate);
            return _mapper.Map<List<MenuNodeSimpleDTO>>(data);
        }
        public async Task<List<MenuNodeSimpleDTO>> FindMenuTreesByNameAsync(string name)
        {
            Expression<Func<MenuNode, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrEmpty(name))
                predicate = predicate.And(m => m.Name.Contains(name));
            var result = await _menuNodeRepository.FindTreeNodesWithNestParentAndChildByFilterAsync(predicate);

            return _mapper.Map<List<MenuNodeSimpleDTO>>(result);
        }

        public MenuNodeDTO GetMenuById(int id)
        {
            var data = _menuNodeRepository.GetMenuById(id);
            return _mapper.Map<MenuNodeDTO>(data);
        }
        public MenuNodeDTO GetDetailMenuById(int id)
        {
            var data = _menuNodeRepository.GetDetailMenuById(id);
            return _mapper.Map<MenuNodeDTO>(data);
        }


        public async Task<bool> ExistMenuNameAsync(int pid, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "菜单名称为空.");

            Expression<Func<MenuNode, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            if (pid == 0)
                predicate = predicate.And(c => c.ParentId == null);
            else
                predicate = predicate.And(c => c.ParentId == pid);

            return await _menuNodeRepository.ExistByFilterAsync(predicate);
        }
        public bool SaveMenu(MenuNodeDTO data, string operatorId, string operatorName)
        {
            //using (var scope = ServiceProvider.CreateScope())
            {
                //var _unitOfContext = scope.ServiceProvider.GetRequiredService<ComAccountUnitOfWorkContext>();

                var model = _mapper.Map<MenuNode>(data);
                var log = new UserTracingLog
                {
                    OperatorId = operatorId,
                    Operator = operatorName,
                    OperateDate = DateTime.UtcNow,
                    Type = Framework.Base.ProcessLogType.Success
                };

                const string logMessage = "{0}在{1}进行了以下操作：{2}！";
                if (model.Id == 0)
                {
                    log.Remark = string.Format(logMessage,
                        operatorName,
                        DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                        "添加了菜单--" + model.Name);
                    _menuNodeRepository.Add(model, false);
                    
                    _userTracingLogRepository.Add(log, false);
                     
                }
                else
                {
                    log.Remark = string.Format(logMessage,
                        operatorName,
                        DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                        "编辑了菜单--" + model.Name);
                    _menuNodeRepository.Modify(model, false);
                    _userTracingLogRepository.Add(log, false);
                }
                var success = _unitOfContext.Commit() > 0;
                if (success)
                {
                    // TODO: 性能问题
                    _menuNodeRepository.UpdateExtendFields();
                }

                return success;
            }
        }
        public bool RemoveMenuById(int id, string operatorId, string operatorName)
        {
            //using (var scope = ServiceProvider.CreateScope())
            {
                //var _unitOfContext = scope.ServiceProvider.GetRequiredService<ComAccountUnitOfWorkContext>();

                var entity = new MenuNode()
                {
                    Id = id,
                    IsDeleted = true
                };

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
                        "删除了菜单--菜单Id" + id)
                };

                _menuNodeRepository.SoftRemove(entity, false);
                _userTracingLogRepository.Add(log, false);
                return _unitOfContext.Commit() > 0;
            }
        }
        public bool UpdateRoleInMenu(int id, List<string> addList, string operatorId, string operatorName)
        {
            return _menuNodeRepository.SaveRoleInMenu(id, addList, operatorId, operatorName);
        }

        public async Task<bool> SaveMenusAsync(List<MenuNodeDTO> models, Guid appId)
        {
            try
            {
                var data = _mapper.Map<List<MenuNodeDTO>, List<MenuNode>>(models);
                return await _menuNodeRepository.SaveMenusAsync(data, appId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                if (ex.InnerException != null)
                {
                    if(ex.InnerException is SqlException)
                    {
                        var sqlex = ex.InnerException as SqlException;
                        var sberrors = new StringBuilder();
                        foreach (SqlError err in sqlex.Errors)
                        {
                            string msg = !string.IsNullOrEmpty(DataHelper.GetSqlExceptionMessage(sqlex.Number))
                                ? DataHelper.GetSqlExceptionMessage(sqlex.Number)
                                : err.Message;
                            sberrors.Append("SQL Error: " + err.Number + ", Message: " + msg + Environment.NewLine);
                        }

                        var throwMsg = sberrors.Length > 0 ? sberrors.ToString() : sqlex.Message;

                        Logger.LogError(sqlex, throwMsg, "堆栈消息为：" + sqlex.StackTrace);
                    }
                    else
                    {
                        Logger.LogError(ex.InnerException, ex.InnerException.Message);
                    }
                }
                    
                return false;
            }
        }

    }
}
