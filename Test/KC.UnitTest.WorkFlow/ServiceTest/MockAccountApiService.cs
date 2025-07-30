using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.DTO.Account;
using KC.Service.DTO.Search;
using KC.Service.WebApiService.Business;
using KC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using KC.Framework.Extension;
using KC.Service.Util;

namespace KC.UnitTest.Workflow
{
    public class MockAccountApiService : IAccountApiService
    {
        public Tenant Tenant => TenantConstant.TestTenantApiAccessInfo;

        #region 组织架构
        /// <summary>
        /// 获取所有的组织及成员（非树结构）
        /// </summary>
        /// <returns></returns>
        public Task<List<OrganizationSimpleDTO>> LoadAllOrganization()
        {
            var allUsers = GetAllUsers();

            var itDept = new OrganizationSimpleDTO()
            {
                Id = WorkflowFixture.wfItDeptId,
                ParentId = WorkflowFixture.wfComDeptId,
                Text = "IT部",
                OrganizationCode = WorkflowFixture.wfItDeptCode,
                OrganizationType = OrganizationType.Internal,
                Users = allUsers.Where(m => m.UserOrgIds.Any(o => o.Equals(WorkflowFixture.wfItDeptId))).ToList()
            };
            var saleDept = new OrganizationSimpleDTO()
            {
                Id = WorkflowFixture.wfSaleDeptId,
                ParentId = WorkflowFixture.wfComDeptId,
                Text = "销售部",
                OrganizationCode = WorkflowFixture.wfSaleDeptCode,
                OrganizationType = OrganizationType.Internal,
                Users = allUsers.Where(m => m.UserOrgIds.Any(o => o.Equals(WorkflowFixture.wfSaleDeptId))).ToList()
            };
            var buyDept = new OrganizationSimpleDTO()
            {
                Id = WorkflowFixture.wfBuyDeptId,
                ParentId = WorkflowFixture.wfComDeptId,
                Text = "采购部",
                OrganizationCode = WorkflowFixture.wfBuyDeptCode,
                OrganizationType = OrganizationType.Internal,
                Users = allUsers.Where(m => m.UserOrgIds.Any(o => o.Equals(WorkflowFixture.wfBuyDeptId))).ToList()
            };
            var hrDept = new OrganizationSimpleDTO()
            {
                Id = WorkflowFixture.wfHrDeptId,
                ParentId = WorkflowFixture.wfComDeptId,
                Text = "人事部",
                OrganizationCode = WorkflowFixture.wfHrDeptCode,
                OrganizationType = OrganizationType.Internal,
                Users = allUsers.Where(m => m.UserOrgIds.Any(o => o.Equals(WorkflowFixture.wfHrDeptId))).ToList()
            };

            var coms = new OrganizationSimpleDTO()
            {
                Id = WorkflowFixture.wfComDeptId,
                ParentId = null,
                Text = "企业",
                OrganizationCode = WorkflowFixture.wfComDeptCode,
                OrganizationType = OrganizationType.Internal,
                Users = allUsers.Where(m => m.UserOrgIds.Any(o => o.Equals(WorkflowFixture.wfComDeptId))).ToList(),
                Children = new List<OrganizationSimpleDTO>() { itDept, saleDept, buyDept, hrDept },
            };

            return Task.Factory.StartNew(() => { return new List<OrganizationSimpleDTO>() { coms, itDept, saleDept, buyDept, hrDept }; });
        }
        /// <summary>
        /// 获取所有的组织及成员（树结构）
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrganizationSimpleDTO>> LoadAllOrgTrees()
        {
            var allOrgs = await LoadAllOrganization();

            return allOrgs.Where(m => m.Id == WorkflowFixture.wfComDeptId).ToList();
        }

        public Task<List<OrganizationSimpleDTO>> LoadOrgTreesByOrgIds(List<int> ids)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取所有的组织及成员（树结构）
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrganizationSimpleDTO>> LoadOrgTreesWithUsers()
        {
            var allOrgs = await LoadAllOrganization();
            return new List<OrganizationSimpleDTO>() {
                allOrgs.FirstOrDefault(m => m.Id == WorkflowFixture.wfComDeptId)
            }; 
        }

        /// <summary>
        /// 获取用户的所属组织及成员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<OrganizationSimpleDTO>> LoadOrganizationsWithUsersByUserId(string userId)
        {
            var allOrgs = await LoadAllOrganization();
            switch (userId)
            {
                case WorkflowFixture.wfItAdminUserId:
                case WorkflowFixture.wfItTestUserId:
                    return new List<OrganizationSimpleDTO>() {
                        allOrgs.FirstOrDefault(m => m.Id == WorkflowFixture.wfItDeptId)
                    };

                case WorkflowFixture.wfSaleAdminUserId:
                case WorkflowFixture.wfSaleTestUserId:
                    return new List<OrganizationSimpleDTO>() {
                        allOrgs.FirstOrDefault(m => m.Id == WorkflowFixture.wfSaleDeptId)
                    };

                case WorkflowFixture.wfBuyAdminUserId:
                case WorkflowFixture.wfBuyTestUserId:
                    return new List<OrganizationSimpleDTO>() {
                        allOrgs.FirstOrDefault(m => m.Id == WorkflowFixture.wfBuyDeptId)
                    };

                case WorkflowFixture.wfHrAdminUserId:
                case WorkflowFixture.wfHrTestUserId:
                    return new List<OrganizationSimpleDTO>() {
                        allOrgs.FirstOrDefault(m => m.Id == WorkflowFixture.wfHrDeptId)
                    };

                default:
                    return new List<OrganizationSimpleDTO>() {
                        allOrgs.FirstOrDefault(m => m.Id == WorkflowFixture.wfComDeptId)
                    };
            }
        }

        /// <summary>
        /// 获取用户的上级组织及成员
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<OrganizationSimpleDTO>> LoadHigherOrganizationsWithUsersByUserId(string userId)
        {
            var allOrgs = await LoadAllOrganization();
            switch (userId)
            {
                case WorkflowFixture.wfComAdminUserId:
                case WorkflowFixture.wfComTestUserId:
                    return new List<OrganizationSimpleDTO>() ;

                default:
                    return new List<OrganizationSimpleDTO>() {
                        allOrgs.FirstOrDefault(m => m.Id == WorkflowFixture.wfComDeptId)
                    };
            }
        }

        
        /// <summary>
        /// 根据组织Ids，获取组织及其下属员工
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<OrganizationSimpleDTO>> LoadOrgTreesWithUsersByOrgIds(OrgTreesAndRolesWithUsersSearchDTO searchModel)
        {
            var orgIds = searchModel.OrgIds;
            var exceptOrgIds = searchModel.ExceptOrgIds;

            Expression<Func<OrganizationSimpleDTO, bool>> predicate = m => true;
            if (orgIds.Any())
            {
                predicate = predicate.And(m => orgIds.Contains(m.Id));
            }
            if (exceptOrgIds.Any())
            {
                predicate = predicate.And(m => !exceptOrgIds.Contains(m.Id));
            }

            var allOrgs = await LoadAllOrganization();
            var data = allOrgs.Where(predicate.Compile()).ToList();
            var resultTree = new List<OrganizationSimpleDTO>();
            foreach (var parent in data.OrderBy(m => m.Index).Where(m => m.ParentId == null))
            {
                KC.Service.Util.TreeNodeUtil.NestSimpleTreeNodeDTO(parent, data, null, null);
                resultTree.Add(parent);
            }

            return resultTree;
        }
        #endregion

        #region 角色
        /// <summary>
        /// 获取所有角色及其下属员工
        /// </summary>
        /// <returns></returns>
        public Task<List<RoleSimpleDTO>> LoadAllRoles()
        {
            var allUsers = GetAllUsers();

            var comManager = new RoleSimpleDTO()
            {
                RoleId = WorkflowFixture.wfComManRoleId,
                RoleName = "GM",
                DisplayName = "总经理",
                Users = allUsers.Where(m => m.UserRoleIds.Any(r => r == WorkflowFixture.wfComManRoleId)).ToList()
            };
            var comAssist = new RoleSimpleDTO()
            {
                RoleId = WorkflowFixture.wfComRoleId,
                RoleName = "GMA",
                DisplayName = "总经理助理",
                Users = allUsers.Where(m => m.UserRoleIds.Any(r => r == WorkflowFixture.wfComRoleId)).ToList()
            };

            var itDept = new RoleSimpleDTO()
            {
                RoleId = WorkflowFixture.wfItManRoleId,
                RoleName = "Admin",
                DisplayName = "系统管理员",
                Users = allUsers.Where(m => m.UserRoleIds.Any(r => r == WorkflowFixture.wfItManRoleId)).ToList()
            };
            var saleManager = new RoleSimpleDTO()
            {
                RoleId = WorkflowFixture.wfSaleManRoleId,
                RoleName = "SM",
                DisplayName = "销售经理",
                Users = allUsers.Where(m => m.UserRoleIds.Any(r => r == WorkflowFixture.wfSaleManRoleId)).ToList()
            };
            var saleAssist = new RoleSimpleDTO()
            {
                RoleId = WorkflowFixture.wfSaleRoleId,
                RoleName = "SMA",
                DisplayName = "销售助理",
                Users = allUsers.Where(m => m.UserRoleIds.Any(r => r == WorkflowFixture.wfSaleRoleId)).ToList()
            };
            var buyManager = new RoleSimpleDTO()
            {
                RoleId = WorkflowFixture.wfBuyManRoleId,
                RoleName = "BM",
                DisplayName = "采购经理",
                Users = allUsers.Where(m => m.UserRoleIds.Any(r => r == WorkflowFixture.wfBuyManRoleId)).ToList()
            };
            var buyAssist = new RoleSimpleDTO()
            {
                RoleId = WorkflowFixture.wfBuyRoleId,
                RoleName = "BMA",
                DisplayName = "采购助理",
                Users = allUsers.Where(m => m.UserRoleIds.Any(r => r == WorkflowFixture.wfBuyRoleId)).ToList()
            };

            var hrManager = new RoleSimpleDTO()
            {
                RoleId = WorkflowFixture.wfHrManRoleId,
                RoleName = "BM",
                DisplayName = "采购经理",
                Users = allUsers.Where(m => m.UserRoleIds.Any(r => r == WorkflowFixture.wfHrManRoleId)).ToList()
            };
            var hrAssist = new RoleSimpleDTO()
            {
                RoleId = WorkflowFixture.wfHrRoleId,
                RoleName = "BMA",
                DisplayName = "采购助理",
                Users = allUsers.Where(m => m.UserRoleIds.Any(r => r == WorkflowFixture.wfHrRoleId)).ToList()
            };

            return Task.Factory.StartNew(() => { return new List<RoleSimpleDTO>() { comManager, comAssist, itDept, saleManager, saleAssist, buyManager, buyAssist, hrManager, hrAssist }; });
        }

        /// <summary>
        /// 根据角色Ids，获取角色及其下属员工
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<RoleSimpleDTO>> LoadRolesWithUsersByRoleIds(OrgTreesAndRolesWithUsersSearchDTO searchModel)
        {
            var roleIds = searchModel.RoleIds;
            var exceptRoleIds = searchModel.ExceptRoleIds;

            Expression<Func<RoleSimpleDTO, bool>> predicate = m => true;
            if (roleIds.Any())
            {
                predicate = predicate.And(m => roleIds.Contains(m.RoleId));
            }
            if (exceptRoleIds.Any())
            {
                predicate = predicate.And(m => !exceptRoleIds.Contains(m.RoleId));
            }

            var allRoles = await LoadAllRoles();
            return allRoles.Where(predicate.Compile()).ToList();
        }

        /// <summary>
        /// 获取用户下的角色及其用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<RoleSimpleDTO>> LoadRolesWithUsersByUserId(string userId)
        {
            var allRoles = await LoadAllRoles();
            return allRoles.Where(r => r.Users.Any(u => u.UserId.Equals(userId))).ToList(); ;
        }

        public Task<RoleSimpleDTO> GetRoleWithUsersByRoleId(string roleId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 用户

        /// <summary>
        /// 根据用户UserId获取用户下的组织及角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<UserSimpleDTO> GetUserWithOrgsAndRolesByUserId(string userId)
        {
            var allUser = GetAllUsers();
            return Task.Factory.StartNew(() => { return allUser.FirstOrDefault(m => m.UserId.Equals(userId)); });
        }

        /// <summary>
        /// 根据用户主管
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<UserSimpleDTO>> LoadUserManagersByUserId(string userId)
        {
            var allOrgs = await LoadAllOrganization();
            var userOrgs = allOrgs.Where(c => c.Users.Any(u => u.UserId.Equals(userId)));
            var users = userOrgs.SelectMany(m => m.Users);
            if (users == null || !users.Any())
                return new List<UserSimpleDTO>();

            return users.Where(m => m.Status == WorkflowBusStatus.Approved && m.PositionLevel == PositionLevel.Mananger).ToList();
        }

        /// <summary>
        /// 根据用户上级主管
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<UserSimpleDTO>> LoadUserSupervisorsByUserId(string userId)
        {
            var allOrgs = await LoadAllOrganization();
            var userOrgs = allOrgs.Where(c => c.Users.Any(u => u.UserId.Equals(userId)));
            var parentOrgIds = userOrgs.Where(m => m.ParentId != null).Select(m => m.ParentId);
            var higherOrgs = allOrgs.Where(c => parentOrgIds.Contains(c.Id));

            var users = higherOrgs.SelectMany(m => m.Users);
            if (users == null || !users.Any())
                return new List<UserSimpleDTO>();

            return users.Where(m => m.Status == WorkflowBusStatus.Approved && m.PositionLevel == PositionLevel.Mananger).ToList();
        }
        
        /// <summary>
        /// 根据当前登录用户的userId、OrgCodes、RoleIds获取所属组织/角色下的用户
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<UserSimpleDTO>> LoadUsersByIdsAndRoleIdsAndOrgCodes(DataPermissionSearchDTO searchModel)
        {
            //if (searchModel.UserIds == null || !searchModel.UserIds.Any())
            //    throw new ArgumentNullException("UserIds", "未传入查询任务的用户Id列表。");

            Expression<Func<UserSimpleDTO, bool>> predicate = m => m.Status == WorkflowBusStatus.Approved;

            Expression<Func<UserSimpleDTO, bool>> dataPredicate = m => searchModel.UserIds.Contains(m.UserId);
            //Or 当前用户所属角色Id在数据角色权限设置字段（ExecuteRoleIds）当中
            if (searchModel.RoleIds != null && searchModel.RoleIds.Any())
            {
                dataPredicate = dataPredicate.Or(m => m.UserRoleIds.Any(id => searchModel.RoleIds.Contains(id)));
            }
            //Or 当前用户所属部门Id在数据角色权限设置字段（ExecuteOrgIds）当中
            if (searchModel.OrgCodes != null && searchModel.OrgCodes.Any())
            {
                //根据组织编码列表获取组织Id列表
                var allOrgs = await LoadAllOrganization();
                var orgs = allOrgs.Where(m => searchModel.OrgCodes.Contains(m.OrganizationCode));
                var orgIds = orgs.Select(m => m.Id);

                dataPredicate = dataPredicate.Or(m => m.UserOrgIds.Any(org => orgIds.Contains(org)));
            }

            predicate = predicate.And(dataPredicate);
            var allUsers = GetAllUsers();
            return allUsers.Where(predicate.Compile()).ToList();
        }

        public Task<List<UserSimpleDTO>> LoadUsersByIds(List<string> userIds)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserSimpleDTO>> LoadUsersByOrgId(int orgId)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserSimpleDTO>> LoadUsersByOrgIds(List<int> orgIds)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserSimpleDTO>> LoadUsersByRoleIds(List<string> roleIds)
        {
            throw new NotImplementedException();
        }

        public Task<UserContactInfoDTO> GetUserContactInfoByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        private List<UserSimpleDTO> GetAllUsers()
        {
            var allUsers = new List<UserSimpleDTO>() {
                // 企业：齐总经理、齐副经理、小齐
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfComAdminUserId,
                    UserName = WorkflowFixture.wfComAdminUserName,
                    DisplayName = WorkflowFixture.wfComAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoleIds = WorkflowFixture.wfComAdminUser_RoleIds,
                    UserRoleNames = new List<string>{ "总经理" },
                    UserOrgIds = WorkflowFixture.wfComAdminUser_OrgIds,
                    UserOrgCodes = WorkflowFixture.wfComAdminUser_OrgCodes,
                    UserOrgNames = new List<string>{ "企业" }
                },
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfComSubAdminUserId,
                    UserName = WorkflowFixture.wfComSubAdminUserName,
                    DisplayName = WorkflowFixture.wfComSubAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoleIds = WorkflowFixture.wfComSubAdminUser_RoleIds,
                    UserRoleNames = new List<string>{ "销售经理,采购经理" },
                    UserOrgIds = WorkflowFixture.wfComSubAdminUser_OrgIds,
                    UserOrgCodes = WorkflowFixture.wfComSubAdminUser_OrgCodes,
                    UserOrgNames = new List<string>{ "销售部,采购部" }
                },
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfComTestUserId,
                    UserName = WorkflowFixture.wfComTestUserName,
                    DisplayName = WorkflowFixture.wfComTestUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Staff,
                    Status = WorkflowBusStatus.Approved,
                    UserRoleIds = WorkflowFixture.wfComTestUser_RoleIds,
                    UserRoleNames = new List<string>{ "总经理助理" },
                    UserOrgIds = WorkflowFixture.wfComTestUser_OrgIds,
                    UserOrgCodes = WorkflowFixture.wfComTestUser_OrgCodes,
                    UserOrgNames = new List<string>{ "企业" }
                },
                
                // IT部：季经理、小季
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfItAdminUserId,
                    UserName = WorkflowFixture.wfItAdminUserName,
                    DisplayName = WorkflowFixture.wfItAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoleIds = WorkflowFixture.wfItAdminUser_RoleIds,
                    UserRoleNames = new List<string>{ "系统管理员" },
                    UserOrgIds = WorkflowFixture.wfItAdminUser_OrgIds,
                    UserOrgCodes = WorkflowFixture.wfItAdminUser_OrgCodes,
                    UserOrgNames = new List<string>{ "IT部" }
                },
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfItTestUserId,
                    UserName = WorkflowFixture.wfItTestUserName,
                    DisplayName = WorkflowFixture.wfItTestUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Staff,
                    Status = WorkflowBusStatus.Approved,
                    UserRoleIds = WorkflowFixture.wfItTestUser_RoleIds,
                    UserRoleNames = new List<string>{ "系统管理员" },
                    UserOrgIds = WorkflowFixture.wfItTestUser_OrgIds,
                    UserOrgCodes = WorkflowFixture.wfItTestUser_OrgCodes,
                    UserOrgNames = new List<string>{ "IT部" }
                },
                
                // 销售部：肖经理、小肖
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfSaleAdminUserId,
                    UserName = WorkflowFixture.wfSaleAdminUserName,
                    DisplayName = WorkflowFixture.wfSaleAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoleIds = WorkflowFixture.wfSaleAdminUser_RoleIds,
                    UserRoleNames = new List<string>{ "销售经理" },
                    UserOrgIds = WorkflowFixture.wfSaleAdminUser_OrgIds,
                    UserOrgCodes = WorkflowFixture.wfSaleAdminUser_OrgCodes,
                    UserOrgNames = new List<string>{ "销售部" }
                },
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfSaleTestUserId,
                    UserName = WorkflowFixture.wfSaleTestUserName,
                    DisplayName = WorkflowFixture.wfSaleTestUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Staff,
                    Status = WorkflowBusStatus.Approved,
                    UserRoleIds = WorkflowFixture.wfSaleTestUser_RoleIds,
                    UserRoleNames = new List<string>{ "销售助理" },
                    UserOrgIds = WorkflowFixture.wfSaleTestUser_OrgIds,
                    UserOrgCodes = WorkflowFixture.wfSaleTestUser_OrgCodes,
                    UserOrgNames = new List<string>{ "销售部" }
                },

                // 采购部：蔡经理、小蔡
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfBuyAdminUserId,
                    UserName = WorkflowFixture.wfBuyAdminUserName,
                    DisplayName = WorkflowFixture.wfBuyAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoleIds = WorkflowFixture.wfBuyAdminUser_RoleIds,
                    UserRoleNames = new List<string>{ "采购经理" },
                    UserOrgIds = WorkflowFixture.wfBuyAdminUser_OrgIds,
                    UserOrgCodes = WorkflowFixture.wfBuyAdminUser_OrgCodes,
                    UserOrgNames = new List<string>{ "采购部" }
                },
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfBuyTestUserId,
                    UserName = WorkflowFixture.wfBuyTestUserName,
                    DisplayName = WorkflowFixture.wfBuyTestUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Staff,
                    Status = WorkflowBusStatus.Approved,
                    UserRoleIds = WorkflowFixture.wfBuyTestUser_RoleIds,
                    UserRoleNames = new List<string>{ "采购助理" },
                    UserOrgIds = WorkflowFixture.wfBuyTestUser_OrgIds,
                    UserOrgCodes = WorkflowFixture.wfBuyTestUser_OrgCodes,
                    UserOrgNames = new List<string>{ "采购部" }
                },
                
                // 人事部：任经理、小任 
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfHrAdminUserId,
                    UserName = WorkflowFixture.wfHrAdminUserName,
                    DisplayName = WorkflowFixture.wfHrAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Mananger,
                    Status = WorkflowBusStatus.Approved,
                    UserRoleIds = WorkflowFixture.wfHrAdminUser_RoleIds,
                    UserRoleNames = new List<string>{ "人事经理" },
                    UserOrgIds = WorkflowFixture.wfHrAdminUser_OrgIds,
                    UserOrgCodes = WorkflowFixture.wfHrAdminUser_OrgCodes,
                    UserOrgNames = new List<string>{ "人事部" }
                },
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfHrTestUserId,
                    UserName = WorkflowFixture.wfHrTestUserName,
                    DisplayName = WorkflowFixture.wfHrTestUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    PositionLevel = PositionLevel.Staff,
                    Status = WorkflowBusStatus.Approved,
                    UserRoleIds = WorkflowFixture.wfHrTestUser_RoleIds,
                    UserRoleNames = new List<string>{ "人事助理" },
                    UserOrgIds = WorkflowFixture.wfHrTestUser_OrgIds,
                    UserOrgCodes = WorkflowFixture.wfHrTestUser_OrgCodes,
                    UserOrgNames = new List<string>{ "人事部" }
                }
            };

            return allUsers;
        }
        #endregion

        #region 未实现
        public Task<List<MenuNodeSimpleDTO>> LoadUserMenusByRoleIds(List<string> roleIds)
        {
            throw new NotImplementedException();
        }

        public Task<List<PermissionSimpleDTO>> LoadUserPermissionsByRoleIds(List<string> roleIds)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveMenusAsync(List<MenuNodeDTO> models, Guid appGuid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SavePermissionsAsync(List<PermissionDTO> models, Guid appGuid)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<UserDTO>> TenantUserLogin(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<bool>> ChangeAdminRawInfo(string password, string adminEmail, string adminPhone)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<bool>> ChangeMailPhoneAsync(string userId, string email, string phone)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            throw new NotImplementedException();
        }


        public ServiceResult<UkeyAuthenticationDTO> GetUkeyAuthenticationByMemberId(string memberId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
