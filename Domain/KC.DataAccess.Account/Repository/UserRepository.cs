using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Model.Account;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using KC.Framework.Base;

namespace KC.DataAccess.Account.Repository
{
    public class UserRepository :
        UserStore<User, Role, ComAccountContext, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>, IUserStore<User>, IUserRepository
    {
        public UserRepository(ComAccountContext dbContext)
            : base(dbContext)
        {
        }
        
        public IList<User> FindAllDetailUsers(bool isApproved = true)
        {
            if (isApproved)
            {
                return Users
                    .Include(fun => fun.UserRoles)
                    .ThenInclude(r => r.Role)
                    .Include(m => m.UserOrganizations)
                    .ThenInclude(r => r.Organization)
                    .AsNoTracking()
                    .Where(m => m.Status == Framework.Base.WorkflowBusStatus.Approved).ToList();
            }
            else
            {
                return Users
                    .Include(fun => fun.UserRoles)
                    .ThenInclude(r => r.Role)
                    .Include(m => m.UserOrganizations)
                    .ThenInclude(r => r.Organization)
                    .AsNoTracking().ToList();
            }
        }
        /// <summary>
        /// 不Include外键表
        /// </summary>
        /// <returns></returns>
        public IList<User> FindAllSimpleUsers()
        {
            return Users.AsNoTracking()
                .Where(m => m.Status == Framework.Base.WorkflowBusStatus.Approved)
                .ToList();
        }

        public IList<User> FindUsersByIds(List<string> ids)
        {
            return Users
                .AsNoTracking()
                .Where(m => m.Status == Framework.Base.WorkflowBusStatus.Approved && ids.Contains(m.Id))
                .ToList();
        }
        public IList<User> FindUsersWithOrgsByIds(List<string> ids)
        {
            return Users
                .Include(m => m.UserOrganizations)
                .ThenInclude(r => r.Organization)
                .AsNoTracking()
                .Where(m => m.Status == Framework.Base.WorkflowBusStatus.Approved && ids.Contains(m.Id))
                .ToList();
        }
        /// <summary>
        /// 查询角色所属用户
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public IList<User> FindUsersByRoleIds(List<string> roleIds)
        {
            return Users.AsNoTracking()
                .Where(model => model.UserRoles.Any(role => roleIds.Contains(role.RoleId) && model.Status == Framework.Base.WorkflowBusStatus.Approved))
                .ToList();
        }
        /// <summary>
        /// 查询部门所属用户
        /// </summary>
        /// <param name="origIds"></param>
        /// <returns></returns>
        public IList<User> FindUsersByOrgIds(List<int> origIds)
        {
            return Users.AsNoTracking()
                .Where(model => model.Status == Framework.Base.WorkflowBusStatus.Approved && model.UserOrganizations.Any(org => origIds.Contains(org.OrganizationId)))
                .ToList();
        }

        public Task<List<User>> FindUsersByFilterAsync(Expression<Func<User, bool>> predicate)
        {
            return Users.AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }

        //paging 不要分页参数 特定方法使用
        public Tuple<int, IList<User>> FindPagenatedUsersByFilter(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<User, bool>> predicate, string sortProperty, bool ascending = true, bool paging = true)
        {
            var q = Users
                .Where(predicate)
                .Include(m => m.UserOrganizations)
                .ThenInclude(r => r.Organization)
                .Include(m => m.UserRoles)
                .ThenInclude(r => r.Role)
                .AsNoTracking();

            int recordCount = q.Count();
            if (!paging)
            {
                var results = q.SingleOrderBy(sortProperty, ascending).ToList();
                return new Tuple<int, IList<User>>(recordCount, results);
            }
            var result = q
                .SingleOrderBy(sortProperty, ascending)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new Tuple<int, IList<User>>(recordCount, result);
        }
        public Tuple<int, IList<User>> GetPaginatedUsersByOrgIds(int pageIndex, int pageSize, List<int> origIds)
        {
            var q = Users.AsNoTracking()
                .Where(model => model.UserOrganizations.Any(org => origIds.Contains(org.OrganizationId)));

            var recordCount = q.Count();
            var result = q.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new Tuple<int, IList<User>>(recordCount, result);
        }

        public Task<User> GetUserByFilterAsync(Expression<Func<User, bool>> predicate)
        {
            return Users.FirstOrDefaultAsync(predicate);
        }
        public Task<User> GetUserWithOrgsAndRolesByIdAsync(string userId)
        {
            return Users
                .Include(u => u.UserOrganizations)
                .ThenInclude(r => r.Organization)
                .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == userId);
        }
        public Task<User> FindByIdAsync(string userId)
        {
            return Users.AsNoTracking().FirstOrDefaultAsync(m => m.Id.Equals(userId));
        }
        public Task<User> FindByPhoneAsync(string phone)
        {
            return Users
                .FirstOrDefaultAsync(m => m.PhoneNumber == phone);
        }
        public Task<User> FindByWxOpenIdAsync(string openId)
        {
            return Users.AsNoTracking().FirstOrDefaultAsync(m => m.OpenId.Equals(openId));
        }

        public async Task<bool> ExistUserByFilterAsync(Expression<Func<User, bool>> predicate)
        {
            return await Users.AnyAsync(predicate);
        }

        public bool AddUsers(List<User> users, bool isSave = true)
        {
            users.ForEach(user =>
            {
                EntityState state = Context.Entry(user).State;
                if (state == EntityState.Detached)
                {
                    Context.Entry(user).State = EntityState.Added;
                }
                //Context.Set<AspNetUser>().Add(m);
            });

            return !isSave || this.Context.SaveChanges() > 0;
        }

        public bool SaveOrganizationInUser(string id, List<int> newIds, string operatorId, string operatorName)
        {
            var user = base.Context.Set<User>()
                .Include(m => m.UserOrganizations)
                .ThenInclude(r => r.Organization)
                .FirstOrDefault(m => m.Id == id);
            if (user == null)
                throw new ArgumentException("id", string.Format("系统不存在用户Id={0}的用户。", id));

            if (newIds == null)
                throw new ArgumentNullException("newIds", "传入参数--newIds：用户Id列表为空。");

            var dbIdList = new List<int>();
            dbIdList.AddRange(user.UserOrganizations.Select(roleDto => roleDto.OrganizationId));

            var delList = dbIdList.Except(newIds).ToList();
            var addList = newIds.Except(dbIdList).ToList();

            var allRoles = base.Context.Set<UsersInOrganizations>().Include(m => m.Organization).ToList();
            var addDetailRoles = allRoles.Where(m => addList.Contains(m.OrganizationId)).ToList();
            foreach (var role in addList)
            {
                base.Context.Set<UsersInOrganizations>().Add(new UsersInOrganizations() { OrganizationId = role, UserId = id });
                //user.UserRoles.Add(role);
            }

            var delDetalRoles = allRoles.Where(m => delList.Contains(m.OrganizationId)).ToList();
            var appDeletedIds = delDetalRoles.Select(m => m.OrganizationId);
            var delr = user.UserOrganizations.Where(m => appDeletedIds.Contains(m.OrganizationId)).ToList();
            foreach (var role in delr)
            {
                base.Context.Set<UsersInOrganizations>().Remove(role);
                //user.UserRoles.Remove(role);
            }

            if (!addDetailRoles.Any() && !delr.Any())
                return true;

            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            string operMessage = string.Format("更新了用户（{0}）的角色--添加了角色({1})，删除了角色({2})",
                 user.UserName,
                addDetailRoles.Select(m => m.OrganizationId + ": " + m.Organization?.Name).ToCommaSeparatedString(),
                delDetalRoles.Select(m => m.OrganizationId + ": " + m.Organization?.Name).ToCommaSeparatedString());
            var log = new UserTracingLog
            {
                OperatorId = operatorId,
                Operator = operatorName,
                OperateDate = DateTime.UtcNow,
                Type = Framework.Base.ProcessLogType.Success,
                Remark = string.Format(logMessage,
                    operatorName,
                    DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                    operMessage)
            };
            base.Context.Set<UserTracingLog>().Add(log);

            return base.Context.SaveChanges() > 0;
        }
        public bool SaveRoleInUser(string id, List<string> newIds, string operatorId, string operatorName)
        {
            var user = base.Context.Set<User>()
                .Include(m => m.UserRoles)
                .ThenInclude(r => r.Role)
                .FirstOrDefault(m => m.Id == id);
            if (user == null)
                throw new ArgumentException("id", string.Format("系统不存在用户Id={0}的用户。", id));

            if (newIds == null)
                throw new ArgumentNullException("newIds", "传入参数--newIds：用户Id列表为空。");

            var dbIdList = new List<string>();
            dbIdList.AddRange(user.UserRoles.Select(roleDto => roleDto.RoleId));

            var delList = dbIdList.Except(newIds).ToList();
            var addList = newIds.Except(dbIdList).ToList();

            var allRoles = base.Context.Set<Role>().ToList();
            var addDetailRoles = allRoles.Where(m => addList.Contains(m.Id)).ToList();
            foreach (var role in addList)
            {
                base.Context.Set<UsersInRoles>().Add(new UsersInRoles() { RoleId = role, UserId = id });
            }

            var delDetalRoles = allRoles.Where(m => delList.Contains(m.Id)).ToList();
            var delr = user.UserRoles.Where(m => delList.Contains(m.RoleId)).ToList();
            foreach (var role in delr)
            {
                base.Context.Set<UsersInRoles>().Remove(role);
            }

            if (!addList.Any() && !delr.Any())
                return true;

            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            string operMessage = string.Format("更新了用户（{0}）的角色--添加了角色({1})，删除了角色({2})",
                 user.UserName,
                addDetailRoles.Select(m => m.Id + ": " + m.Name).ToCommaSeparatedString(),
                delDetalRoles.Select(m => m.Id + ": " + m.Name).ToCommaSeparatedString());
            var log = new UserTracingLog
            {
                OperatorId = operatorId,
                Operator = operatorName,
                OperateDate = DateTime.UtcNow,
                Type = Framework.Base.ProcessLogType.Success,
                Remark = string.Format(logMessage,
                    operatorName,
                    DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                    operMessage)
            };
            base.Context.Set<UserTracingLog>().Add(log);

            return base.Context.SaveChanges() > 0;
        }

        public bool UpdateUser(User user, List<int> newOrgIds = null, bool isSave = true)
        {
            var dbUser = this.Context.Set<User>()
                .Include(m => m.UserOrganizations)
                .FirstOrDefault(m => user.Id == m.Id);

            dbUser.PositionLevel = user.PositionLevel;
            dbUser.IsDefaultMobile = user.IsDefaultMobile;
            if (!string.IsNullOrEmpty(user.Email))
                dbUser.Email = user.Email;
            if (!string.IsNullOrEmpty(user.UserName))
                dbUser.UserName = user.UserName;
            if (!string.IsNullOrEmpty(user.ReferenceId1))
                dbUser.ReferenceId1 = user.ReferenceId1;
            if (!string.IsNullOrEmpty(user.DisplayName))
                dbUser.DisplayName = user.DisplayName;
            if (!string.IsNullOrEmpty(user.Telephone))
                dbUser.Telephone = user.Telephone;
            if (!string.IsNullOrEmpty(user.PhoneNumber))
                dbUser.PhoneNumber = user.PhoneNumber;
            if (!string.IsNullOrEmpty(user.ContactQQ))
                dbUser.ContactQQ = user.ContactQQ;
            if (!string.IsNullOrEmpty(user.PasswordHash))
                dbUser.PasswordHash = user.PasswordHash;
            if (!string.IsNullOrEmpty(user.SecurityStamp))
                dbUser.SecurityStamp = user.SecurityStamp;

            //更新用户所属组织架构
            if (newOrgIds.Any())
            {
                var dbIdList = new List<int>();
                dbIdList.AddRange(dbUser.UserOrganizations.Select(m => m.OrganizationId));

                var delList = dbIdList.Except(newOrgIds).ToList();
                var addList = newOrgIds.Except(dbIdList).ToList();

                var delr = dbUser.UserOrganizations
                    .Where(m => delList.Contains(m.OrganizationId))
                    .ToList();
                foreach (var delOrg in delr)
                {
                    dbUser.UserOrganizations.Remove(delOrg);
                }
                foreach (var orgId in addList)
                {
                    dbUser.UserOrganizations.Add(new UsersInOrganizations() { UserId = user.Id, OrganizationId = orgId });
                }
            }

            return !isSave || this.Context.SaveChanges() > 0;
        }
        public bool UpdateUserInOrganizations(List<int> newIds, string userId, bool isSave = true)
        {
            var dbUsersInOrgs = this.Context.Set<UsersInOrganizations>()
                .Where(m => userId == m.UserId).ToList();

            var dbIdList = new List<int>();
            dbIdList.AddRange(dbUsersInOrgs.Select(m => m.OrganizationId));

            var delList = dbIdList.Except(newIds).ToList();
            var addList = newIds.Except(dbIdList).ToList();

            var delr = dbUsersInOrgs
                .Where(m => delList.Contains(m.OrganizationId))
                .ToList();
            foreach (var delOrg in delr)
            {
                this.Context.Set<UsersInOrganizations>().Remove(delOrg);
            }
            foreach (var orgId in addList)
            {
                this.Context.Set<UsersInOrganizations>().Add(new UsersInOrganizations() { UserId = userId, OrganizationId = orgId });
            }

            return !isSave || this.Context.SaveChanges() > 0;
        }
        public bool UpdateUserInRoles(List<string> newIds, string userId, bool isSave = true)
        {
            var dbUsersInOrgs = this.Context.Set<UsersInRoles>()
                .Where(m => userId == m.UserId).ToList();

            var dbIdList = new List<string>();
            dbIdList.AddRange(dbUsersInOrgs.Select(m => m.RoleId));

            var delList = dbIdList.Except(newIds).ToList();
            var addList = newIds.Except(dbIdList).ToList();

            var delr = dbUsersInOrgs
                .Where(m => delList.Contains(m.RoleId))
                .ToList();
            foreach (var delOrg in delr)
            {
                this.Context.Set<UsersInRoles>().Remove(delOrg);
            }
            foreach (var orgId in addList)
            {
                this.Context.Set<UsersInRoles>().Add(new UsersInRoles() { UserId = userId, RoleId = orgId });
            }

            return !isSave || this.Context.SaveChanges() > 0;
        }

        public bool RemoveUserById(string userId, string operatorId, string operatorName)
        {
            var user = base.Context.Set<User>()
                    .Include(m => m.UserOrganizations)
                    .Include(m => m.UserRoles)
                    .FirstOrDefault(m => m.Id == userId);
            if (user == null)
                return true;

            Context.RemoveRange(user.UserRoles);
            Context.RemoveRange(user.UserOrganizations);
            Context.Remove(user);

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
                   "删除了用户（" + user.Id + ": " + user.UserName + ")")
            };
            Context.Set<UserTracingLog>().Add(log);

            return Context.SaveChanges() > 0;
        }
        public bool ReactUserById(string userId, string operatorId, string operatorName)
        {
            var user =
                base.Context.Set<User>()
                    .Include(m => m.UserOrganizations)
                    .FirstOrDefault(m => m.Id == userId);

            if (user == null)
                return true;
            Context.Entry(user).Property("Status").IsModified = true;
            
            const string logMessage = "{0}在{1}进行了以下操作：{2}！";
            UserTracingLog log;
            // 草稿（Draft）-->提交审核（AuditPending）
            // 冻结（Disagree）-->提交审核（AuditPending）
            if (user.Status == WorkflowBusStatus.Draft 
                || user.Status == WorkflowBusStatus.Disagree)
            {
                user.Status = WorkflowBusStatus.AuditPending;
                log = new UserTracingLog
                {
                    OperatorId = operatorId,
                    Operator = operatorName,
                    OperateDate = DateTime.UtcNow,
                    Type = ProcessLogType.Success,
                    Remark = string.Format(logMessage,
                   operatorName,
                   DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                   "提交了用户（" + user.Id + ": " + user.UserName + ")的审核")
                };
            }
            // 提交审核（AuditPending）-->启用（Approved）
            else if (user.Status == WorkflowBusStatus.AuditPending )
            {
                user.Status = WorkflowBusStatus.Approved;
                log = new UserTracingLog
                {
                    OperatorId = operatorId,
                    Operator = operatorName,
                    OperateDate = DateTime.UtcNow,
                    Type = ProcessLogType.Success,
                    Remark = string.Format(logMessage,
                   operatorName,
                   DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                   "启用了用户（" + user.Id + ": " + user.UserName + ")")
                };
            }
            // 启用（Approved）-->冻结（Disagree）
            else
            {
                //去除绑定角色
                SaveRoleInUser(userId, new List<string>(), operatorId, operatorName);

                user.Status = WorkflowBusStatus.Disagree;
                log = new UserTracingLog
                {
                    OperatorId = operatorId,
                    Operator = operatorName,
                    OperateDate = DateTime.UtcNow,
                    Type = Framework.Base.ProcessLogType.Success,
                    Remark = string.Format(logMessage,
                                    operatorName,
                                    DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                                    "冻结了用户（" + user.Id + ": " + user.UserName + ")")
                };
            }

            Context.Set<UserTracingLog>().Add(log);

            return Context.SaveChanges() > 0;
        }


        public async Task<IList<Role>> FindRolesByRoleIdsAysnc(List<string> roleIds)
        {
            return await Context.Set<Role>().AsNoTracking().Where(m => roleIds.Contains(m.Id)).ToListAsync();
        }

    }
}
