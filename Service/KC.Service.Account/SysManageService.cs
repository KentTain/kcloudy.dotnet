using AutoMapper;
using KC.DataAccess.Account.Repository;
using KC.Database.IRepository;
using KC.Framework.Exceptions;
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
using Microsoft.Extensions.Logging;
using KC.Service.WebApiService.Business;
using KC.Database.Extension;
using KC.Service.Base.ECharts;
using KC.Service.Base.ECharts.axis;
using KC.Service.Base.ECharts.series;
using KC.DataAccess.Account;
using KC.Service.Base.ECharts.style;
using KC.Database.EFRepository;
using KC.Service.DTO.Search;

namespace KC.Service.Account
{
    public class SysManageService : EFServiceBase, ISysManageService
    {
        private readonly IMapper _mapper;

        #region Db Repository
        private ComAccountContext _dbContext;

        private IOrganizationRepository _organizationRepository;
        private IDbRepository<UserTracingLog> _userTracingLogRepository;
        private IDbRepository<UserLoginLog> _userLoginLogRepository;

        #endregion

        //public SysManageService(
        //    Tenant tenant,
        //    System.Net.Http.IHttpClientFactory clientFactory)
        //    : base(tenant, clientFactory)
        //{
        //    var unitOfWorkContext = new ComAccountUnitOfWorkContext(tenant);
        //    _orgnizationRepository = new OrganizationRepository(unitOfWorkContext);
        //    _userTracingLogRepository = new CommonEFRepository<UserTracingLog>(unitOfWorkContext);
        //}

        public SysManageService(
            Tenant tenant,
            IOrganizationRepository orgnizationRepository,
            IDbRepository<UserTracingLog> userTracingLogRepository,
            IDbRepository<UserLoginLog> userLoginLogRepository,

            IMapper mapper,
            ComAccountContext dbContext,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<SysManageService> logger)
            : base(tenant, clientFactory, logger)
        {
            _organizationRepository = orgnizationRepository;
            _userTracingLogRepository = userTracingLogRepository;
            _userLoginLogRepository = userLoginLogRepository;

            _mapper = mapper;
            _dbContext = dbContext;
        }

        #region Organization
        
        public List<OrganizationDTO> FindAllOrgTrees()
        {
            var data = _organizationRepository.FindAllTreeNodeWithNestChild();

            return _mapper.Map<List<OrganizationDTO>>(data);
        }

        public List<OrganizationDTO> FindOrgTreesByIds(List<int> orgIds, string name)
        {
            Expression<Func<Organization, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }
            if (orgIds != null && orgIds.Any())
            {
                predicate = predicate.And(m => orgIds.Contains(m.Id));
            }
            var data = _organizationRepository.FindTreeNodesWithNestParentAndChildByFilter(predicate);

            return _mapper.Map<List<OrganizationDTO>>(data);
        }

        public List<OrganizationDTO> FindOrgTreesByName(string name)
        {
            Expression<Func<Organization, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }

            var data = _organizationRepository.FindTreeNodesWithNestParentAndChildByFilter(predicate);
            return _mapper.Map<List<OrganizationDTO>>(data);
        }

        public List<OrganizationDTO> FindOrgTreesWithUsers()
        {
            Expression<Func<Organization, bool>> predicate = m => !m.IsDeleted;

            var result = new List<OrganizationDTO>();
            var data = _organizationRepository.GetAllOrganizationsWithUsers(predicate);
            var orgs = data.Select(m => AppendOrgWithUsers(m)).ToList();
            foreach (var parent in orgs.Where(m => m.ParentId == null))
            {
                NestOrganizationWithChild(parent, orgs);
                result.Add(parent);
            }
            return result;
        }
        public List<OrganizationDTO> FindOrgTreesWithUsersByRoleIds(List<string> roleIds)
        {
            if (roleIds == null || !roleIds.Any())
                return FindOrgTreesWithUsers();

            var result = new List<OrganizationDTO>();
            var data = _organizationRepository.GetAllOrganizationsWithUsers(c => !c.IsDeleted, true);
            var orgs = data.Select(m => AppendOrgWithUsers(m, roleIds)).ToList();
            foreach (var parent in orgs.Where(m => m.ParentId == null))
            {
                NestOrganizationWithChild(parent, orgs);
                result.Add(parent);
            }
            return result;
        }

        /// <summary>
        /// 查询部门或角色的组织框架（审批中心流程排除选择）
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="depIds"></param>
        /// <returns></returns>
        public List<OrganizationDTO> FindOrgTreesWithUsersByOrgIds(List<int> orgIds, List<int> exceptOrgIds)
        {
            Expression<Func<Organization, bool>> predicate = m => !m.IsDeleted;
            if (orgIds.Any())
            {
                predicate = predicate.And(m => orgIds.Contains(m.Id));
            }
            if (exceptOrgIds.Any())
            {
                predicate = predicate.And(m => !exceptOrgIds.Contains(m.Id));
            }

            var result = new List<OrganizationDTO>();
            var data = _organizationRepository.GetAllOrgsWithNestParentAndChildAndUsers(predicate);
            var orgs = data.Select(m => AppendOrgWithUsers(m)).ToList();
            foreach (var parent in orgs.Where(m => m.ParentId == null))
            {
                NestOrganizationWithChild(parent, orgs);
                result.Add(parent);
            }

            return result;
        }


        /// <summary>
        /// 获取所有部门(不是树状结构)
        /// </summary>
        /// <returns></returns>
        public List<OrganizationDTO> FindAllOrganization()
        {
            var data = _organizationRepository.FindAll(c => !c.IsDeleted);
            return _mapper.Map<List<OrganizationDTO>>(data);
        }

        public List<OrganizationDTO> FindOrganizationsWithUsersByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<OrganizationDTO>();

            var userOrgs = _organizationRepository.GetAllOrganizationsWithUsers(c => !c.IsDeleted && c.OrganizationUsers.Any(u => u.UserId.Equals(userId)));
            var orgs = userOrgs.Select(m => AppendOrgWithUsers(m)).ToList();
            return orgs;
        }

        public List<OrganizationDTO> FindHigherOrganizationsWithUsersByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<OrganizationDTO>();

            var userOrgs = _organizationRepository.GetAllOrganizationsWithUsers(c => !c.IsDeleted && c.OrganizationUsers.Any(u => u.UserId.Equals(userId)));
            var parentOrgIds = userOrgs.Where(m => m.ParentId != null).Select(m => m.ParentId);
            
            var data = _organizationRepository.GetAllOrganizationsWithUsers(c => !c.IsDeleted && parentOrgIds.Contains(c.Id));
            var result = data.Select(m => AppendOrgWithUsers(m)).ToList();
            return result;
        }


        public OrganizationDTO GetOrganizationById(int id)
        {
            var data = _organizationRepository.GetById(id);
            return _mapper.Map<OrganizationDTO>(data);
        }
        public OrganizationDTO GetOrganizationsWithUsersByOrgId(int orgId)
        {
            var data = _organizationRepository.GetOrganizationsWithUsersByOrgId(orgId);
            return AppendOrgWithUsers(data);
        }

        //根据传入部门ID集合  查出所有该部门及其下属的ID集合
        public List<int> FindOrganizationsByIdList(List<int> orgIds)
        {
            if (!orgIds.Any())
                return new List<int>();

            var list = new List<int>();
            Expression<Func<Organization, bool>> predicate = m => !m.IsDeleted;
            var data = _organizationRepository.GetAllOrganizationsWithUsers(predicate);
            var orgs = _mapper.Map<List<OrganizationDTO>>(data);
            for (int i = 0; i < orgIds.Count(); i++)
            {
                var levelList = orgs.FirstOrDefault(c => c.Id == orgIds[i]);
                if (levelList != null)
                {
                    list.Add(levelList.Id);
                    //把子集菜单加到父集菜单中去
                    var orgsWherePar = orgs.Where(m => m.Id == levelList.Id).ToList();
                    foreach (var parent in orgsWherePar)
                    {
                        NestOrganizationWithID(parent, orgs, list);
                    }
                }
            }

            return list.Distinct().ToList();
        }
        private void NestOrganizationWithID(OrganizationDTO parent, List<OrganizationDTO> allOrgs, List<int> list)
        {
            var child = allOrgs.Where(m => m.ParentId.Equals(parent.Id)).ToList();
            parent.Children = child;
            foreach (var children in child.OrderBy(m => m.Index))
            {
                children.ParentName = parent.Text;
                list.Add(children.Id);
                NestOrganizationWithID(children, allOrgs, list);
            }
        }

        public bool ExistOrganizationName(int pid, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "name is null or empty.");

            Expression<Func<Organization, bool>> predicate = m => !m.IsDeleted && m.Name.Equals(name);
            if (pid == 0)
                predicate = predicate.And(c => c.ParentId == null);
            else
                predicate = predicate.And(c => c.ParentId == pid);

            return _organizationRepository.ExistByFilter(predicate);
        }
        public bool SaveOrganization(OrganizationDTO data, string currentUserId)
        {
            Expression<Func<Organization, bool>> predicate = c => !c.IsDeleted && c.OrganizationType == data.OrganizationType && c.Name == data.Text;
            if (string.IsNullOrEmpty(currentUserId))
            {
                predicate = predicate.And(c => c.CreatedBy == null);
            }
            else
            {
                predicate = predicate.And(c => c.CreatedBy == currentUserId);
            }
            if (data.Id > 0)
            {
                predicate = predicate.And(c => c.Id != data.Id);
            }
            var repeatTreeNameTemp = _organizationRepository.ExistByFilter(predicate);
            if (repeatTreeNameTemp)
            {
                throw new BusinessPromptException("名称【" + data.Text + "】已存在,请重新输入！");
            }
            var model = _mapper.Map<Organization>(data);
            model.Name = model.Name.Trim();
            Organization parent = null;
            if (!data.IsDeleted)
            {
                if (model.ParentId.HasValue)
                {
                    parent = _organizationRepository.GetById(model.ParentId.Value);
                    if (parent != null && parent.Level >= 4)
                    {
                        throw new ArgumentException(string.Format("父级:{0} 不能作为父级！", model.Name));
                    }
                    model.ParentNode = parent;
                }
            }
            if (model.Id == 0)
            {
                var res = _organizationRepository.Add(model);
                if (res)
                {
                    if (parent != null)
                    {
                        model.Level = parent.Level + 1;
                        model.TreeCode = parent.TreeCode + DatabaseExtensions.TreeCodeSplitIdWithChar + model.Id;
                    }
                    else
                    {
                        model.Level = 1;
                        model.TreeCode = model.Id.ToString();
                    }
                    model.Leaf = true;
                    _organizationRepository.Modify(model);
                }
                return res;
            }
            else
            {
                var item = _organizationRepository.GetById(model.Id);
                if (parent != null)
                {
                    item.TreeCode = parent.TreeCode + DatabaseExtensions.TreeCodeSplitIdWithChar + model.Id;
                    item.Level = parent.Level + 1;
                    item.Leaf = true;
                }
                item.ParentId = model.ParentId;
                item.Name = model.Name;
                item.TreeCode = model.TreeCode;
                return _organizationRepository.Modify(item, new[] { "ParentId", "Name", "TreeCode", "TreeCode", "Level", "Leaf" });
            }


        }
        public bool RemoveOrganization(int id)
        {
            //Expression<Func<Organization, bool>> predicate = m => m.Id == id;
            var item = _organizationRepository.GetById(id);
            item.IsDeleted = true;
            //return _orgnizationRepository.Remove(id);
            //var data = _mapper.Map<OrganizationDTO>(item);
            return _organizationRepository.Modify(item, new[] { "IsDeleted" });
        }

        private void NestOrganizationWithChild(OrganizationDTO parent, List<OrganizationDTO> allOrgs)
        {
            var child = allOrgs.Where(m => m.ParentId.Equals(parent.Id)).ToList();
            parent.Children = child;
            foreach (var children in child.OrderBy(m => m.Index))
            {
                children.ParentName = parent.Text;
                NestOrganizationWithChild(children, allOrgs);
            }
        }
        private OrganizationDTO AppendOrgWithUsers(Organization data, List<string> roleIds = null, List<string> exceptRoleIds = null)
        {
            var result = _mapper.Map<OrganizationDTO>(data);
            if (!data.OrganizationUsers.Any()) return result;

            if ((roleIds != null && roleIds.Any()) && (exceptRoleIds != null && exceptRoleIds.Any()))
            {
                var selectUsers = data.OrganizationUsers
                    .Select(m => m.User)
                    .Where(m => m.UserRoles.Select(r => r.RoleId).Intersect(roleIds).Any()
                        && !m.UserRoles.Select(r => r.RoleId).Intersect(exceptRoleIds).Any());
            }
            else if (roleIds != null && roleIds.Any())
            {
                var selectUsers = data.OrganizationUsers
                    .Select(m => m.User)
                    .Where(m => m.UserRoles.Select(r => r.RoleId).Intersect(roleIds).Any());
                result.Users = _mapper.Map<List<UserDTO>>(selectUsers);
            }
            else if (exceptRoleIds != null && exceptRoleIds.Any())
            {
                var selectUsers = data.OrganizationUsers
                    .Select(m => m.User)
                    .Where(m => !m.UserRoles.Select(r => r.RoleId).Intersect(exceptRoleIds).Any());
                result.Users = _mapper.Map<List<UserDTO>>(selectUsers);
            }
            else
            {
                result.Users = _mapper.Map<List<UserDTO>>(data.Users);
            }
            return result;
        }

        #endregion

        #region 用户跟踪日志

        public PaginatedBaseDTO<UserTracingLogDTO> FindPaginatedUserTracingLogs(int pageIndex, int pageSize, string name, string order)
        {
            Expression<Func<UserTracingLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _userTracingLogRepository.FindPagenatedListWithCount<UserTracingLog>(
                pageIndex,
                pageSize,
                predicate,
                "OperateDate",
                order.Equals("asc"));

            var total = data.Item1;
            var rows = _mapper.Map<List<UserTracingLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<UserTracingLogDTO>(pageIndex, pageSize, total, rows);
        }

        #endregion

        #region 用户登陆日志
        
        public PaginatedBaseDTO<UserLoginLogDTO> FindPaginatedUserLoginLogs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<UserLoginLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _userLoginLogRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.OperateDate, false);

            var total = data.Item1;
            var rows = _mapper.Map<List<UserLoginLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<UserLoginLogDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<ChartOption> GetUserLoginReportData(DateTime? startDate, DateTime? endDate)
        {
            //获取所有的统计数据
            var startdate = startDate.HasValue ? startDate.Value : DateTime.Now.AddDays(-5);
            var enddate = endDate.HasValue ? endDate.Value : DateTime.Now.AddDays(5);
            Expression<Func<UserLoginLog, bool>> predicate = m => true;
            predicate = predicate.And(m => m.OperateDate >= startdate && m.OperateDate <= enddate);
            var data = await _userLoginLogRepository.FindAllAsync(predicate);
            //查询数据进行统计分组
            var groupType = "登陆次数";
            var legends = new List<string>() { groupType };
            var groupData = data.GroupBy(m => new { m.OperateDate.Date })
                .Select(m => new ReportGroupDTO()
                {
                    Key1 = m.Key.Date.ToString(DateTimeExtensions.FMT_yMd1),
                    KeyName1 = groupType,
                    Value = m.Count()
                })
                .ToList();
            
            //获取EChart所需的x轴数据
            var xData = startdate.FindAllDayStringsInDateDiff(enddate);
            //获取EChart所需的y轴数据
            var yData = new List<Series>();
            foreach(var line in legends)
            {
                var lineData = new List<decimal>();
                foreach (var date in xData)
                {
                    var tempdata = groupData
                        .Where(m => m.Key1.Equals(date) && m.KeyName1.Equals(line))
                        .Sum(m => m.Value);
                    lineData.Add(tempdata);
                }
                //设置折线的样式
                var style = new ItemStyle();
                style.Normal().LineStyle().Width(2).Type(LineStyleType.solid)
                    .Color("rgba(30,144,255,0.8)").ShadowColor("rgba(0,0,0,0.5)")
                    .ShadowBlur(8).ShadowOffsetX(8).ShadowOffsetY(8);
                style.Emphasis().Label().Show(true);
                //设置折线数据
                var ydata = new Line(line).Stack(line).Symbol(SymbolType.emptyCircle).SymbolSize(6);
                ydata.SetItemStyle(style);
                ydata.SetData(lineData);
                yData.Add(ydata);
            }

            var option = new ChartOption();
            option.ToolTip().Trigger(TriggerType.axis);
            option.Legend().Data(legends);
            option.Grid().Y(40).X2(60).Y2(40).X(60); //上、右、下、左
            option.XAxis(new CategoryAxis()
            {
                type = AxisType.category,
                boundaryGap = false,
                data = xData,
            });
            option.YAxis(new ValueAxis()
            {
                type = AxisType.value
            });
            option.Series(yData.ToArray());

            return option;
        }

        public async Task<bool> AddUserLoginLogAsync(UserLoginLogDTO model)
        {
            var data = _mapper.Map<UserLoginLog>(model);
            await _dbContext.UserLoginLogs.AddAsync(data);
            return await _dbContext.SaveChangesAsync() > 0;
            //备注：在使用IdentityContext上下文保存其他自定义对象时，不能直接使用Repository进行保存
            //return await _userLoginLogRepository.AddAsync(data);
        }
        #endregion

        #region 测试TreeNode基类
        public List<OrganizationDTO> Test_GetAllTreeNodeWithNestChild()
        {
            var result = _organizationRepository.FindAllTreeNodeWithNestChild();

            return _mapper.Map<List<OrganizationDTO>>(result);
        }

        public OrganizationDTO Test_GetTreeNodeWithNestChildById(int id)
        {
            var result = _organizationRepository.GetTreeNodeWithNestChildById(id);

            return _mapper.Map<OrganizationDTO>(result);
        }

        public List<OrganizationDTO> Test_GetTreeNodesWithNestParentAndChildById(int id)
        {
            var result = _organizationRepository.FindTreeNodesWithNestParentAndChildById(id);

            return _mapper.Map<List<OrganizationDTO>>(result);
        }

        public List<OrganizationDTO> Test_GetTreeNodesWithNestParentAndChildByFilter(string name)
        {
            Expression<Func<Organization, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrEmpty(name))
                predicate = predicate.And(m => m.Name.Contains(name));
            var result = _organizationRepository.FindTreeNodesWithNestParentAndChildByFilter(predicate);

            return _mapper.Map<List<OrganizationDTO>>(result);
        }

        public bool Test_UpdateExtendFields()
        {
            var result = _organizationRepository.UpdateExtendFields();

            return result;
        }
        #endregion

    }
}
