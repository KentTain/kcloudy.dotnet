using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using KC.Framework.Exceptions;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.DataAccess.Account;
using KC.DataAccess.Account.Repository;
using KC.Framework.Base;
using KC.Model.Account;
using KC.Service.Account.EventHandlers;
using KC.Service.EFService;
using KC.Service.Events;
using KC.Service.Extension;
using KC.Service.DTO.Account;
using KC.Service.DTO;
using KC.Service.Component;
using Microsoft.AspNetCore.Identity;
using KC.Database.IRepository;
using Microsoft.Extensions.Logging;
using KC.Service.WebApiService.Business;
using KC.Service.DTO.Search;
using KC.Service.DTO.Message;
using KC.Service.Account.Message;
using KC.Service.Util;
using KC.ThirdParty;

namespace KC.Service.Account
{
    public sealed class AccountService : EFServiceBase, IAccountService
    {
        private readonly IMapper _mapper;

        #region Db Repository
        private readonly ComAccountContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IDbRepository<UserTracingLog> _userTracingLogRepository;

        private readonly MessageUtil _messageUtil;
        private readonly IConfigApiService _configApiService;
        private readonly IWorkflowApiService _workflowApiService;

        private readonly UserManager<User> _userManager;
        #endregion

        //public AccountService(
        //    Tenant tenant)
        //    : base(tenant, null)
        //{
        //}

        public AccountService(
            Tenant tenant,
            IConfigApiService configApiService,
            IWorkflowApiService workflowApiService,

            IUserRepository userRepository,
            IOrganizationRepository orgnizationRepository,
            IDbRepository<UserTracingLog> userTracingLogRepository,

            UserManager<User> userManager,
            MessageUtil messageUtil,

            IMapper mapper,
            ComAccountContext dbContext,
            System.Net.Http.IHttpClientFactory clientFactory,
            ILogger<AccountService> logger)
            : base(tenant, clientFactory, logger)
        {
            _configApiService = configApiService;
            _workflowApiService = workflowApiService;

            _userManager = userManager;
            _userRepository = userRepository;
            _organizationRepository = orgnizationRepository;
            _userTracingLogRepository = userTracingLogRepository;

            _messageUtil = messageUtil;

            _mapper = mapper;
            _dbContext = dbContext;
        }

        #region User

        public async Task<bool> ExistUserEmailAsync(string email)
        {
            var isValid = StringExtensions.IsEmail(email);
            if (!isValid || string.IsNullOrEmpty(email)) return false;

            Expression<Func<User, bool>> predicate = m => m.Email.Equals(email);

            return await _userRepository.ExistUserByFilterAsync(predicate);
        }
        public async Task<bool> ExistUserPhoneAsync(string phone)
        {
            var isValid = StringExtensions.IsMobile(phone);
            if (!isValid || string.IsNullOrEmpty(phone)) return false;

            Expression<Func<User, bool>> predicate = m => m.PhoneNumber.Equals(phone);

            return await _userRepository.ExistUserByFilterAsync(predicate);
        }
        public async Task<bool> ExistUserNameAsync(string userName)
        {
            var isValid = userName.Length >= 2 && userName.Length <= 15;
            if (!isValid || string.IsNullOrEmpty(userName)) return false;

            Expression<Func<User, bool>> predicate = m => m.UserName.Equals(userName);

            return await _userRepository.ExistUserByFilterAsync(predicate);
        }

        public List<UserSimpleDTO> FindAllUsersWithRolesAndOrgs()
        {
            var data = _userRepository.FindAllDetailUsers();
            return _mapper.Map<List<UserSimpleDTO>>(data);
        }
        public List<UserSimpleDTO> FindUsersByOrgIds(List<int> orgIds)
        {
            var data = _userRepository.FindUsersByOrgIds(orgIds);
            return _mapper.Map<List<UserSimpleDTO>>(data);
        }
        public List<UserSimpleDTO> FindUsersByRoleIds(List<string> roleIds)
        {
            var data = _userRepository.FindUsersByRoleIds(roleIds);
            return _mapper.Map<List<UserSimpleDTO>>(data);
        }
        public List<UserSimpleDTO> FindUsersByUserIds(List<string> userIds)
        {
            var data = _userRepository.FindUsersWithOrgsByIds(userIds);
            return _mapper.Map<List<UserSimpleDTO>>(data);
        }

        public List<UserSimpleDTO> FindUserManagersByUserId(string userId)
        {
            var userOrgs = _organizationRepository.GetAllOrganizationsWithUsers(c => !c.IsDeleted && c.OrganizationUsers.Any(u => u.UserId.Equals(userId)));
            var users = userOrgs.SelectMany(m => m.Users);
            if (users == null || !users.Any())
                return new List<UserSimpleDTO>();

            var data = users.Where(m => m.Status == WorkflowBusStatus.Approved && m.PositionLevel == PositionLevel.Mananger);
            return _mapper.Map<List<UserSimpleDTO>>(data);
        }
        public List<UserSimpleDTO> FindUserSupervisorsByUserId(string userId)
        {
            var userOrgs = _organizationRepository.FindAll(c => !c.IsDeleted && c.OrganizationUsers.Any(u => u.UserId.Equals(userId)));
            var parentOrgIds = userOrgs.Where(m => m.ParentId != null).Select(m => m.ParentId);
            var higherOrgs = _organizationRepository.GetAllOrganizationsWithUsers(c => !c.IsDeleted && parentOrgIds.Contains(c.Id));

            var users = higherOrgs.SelectMany(m => m.Users);
            if (users == null || !users.Any())
                return new List<UserSimpleDTO>();

            var data = users.Where(m => m.Status == WorkflowBusStatus.Approved && m.PositionLevel == PositionLevel.Mananger);
            return _mapper.Map<List<UserSimpleDTO>>(data);
        }

        public async Task<List<UserSimpleDTO>> FindUsersByDataPermissionFilterAsync(DataPermissionSearchDTO searchModel)
        {
            //if (searchModel.UserIds == null || !searchModel.UserIds.Any())
            //    throw new ArgumentNullException("UserIds", "未传入查询任务的用户Id列表。");

            Expression<Func<User, bool>> predicate = m => m.Status == WorkflowBusStatus.Approved;

            Expression<Func<User, bool>> dataPredicate = m => searchModel.UserIds.Contains(m.Id);
            //Or 当前用户所属角色Id在数据角色权限设置字段（ExecuteRoleIds）当中
            if (searchModel.RoleIds != null && searchModel.RoleIds.Any())
            {
                dataPredicate = dataPredicate.Or(m => m.UserRoles.Any(role => searchModel.RoleIds.Contains(role.RoleId)));
            }
            //Or 当前用户所属部门Id在数据角色权限设置字段（ExecuteOrgIds）当中
            if (searchModel.OrgCodes != null && searchModel.OrgCodes.Any())
            {
                //根据组织编码列表获取组织Id列表
                var orgs = await _organizationRepository.FindAllAsync(m => searchModel.OrgCodes.Contains(m.OrganizationCode));
                var orgIds = orgs.Select(m => m.Id);

                dataPredicate = dataPredicate.Or(m => m.UserOrganizations.Any(org => orgIds.Contains(org.OrganizationId)));
            }

            predicate = predicate.And(dataPredicate);

            var data = await _userRepository.FindUsersByFilterAsync(predicate);
            return _mapper.Map<List<UserSimpleDTO>>(data);
        }
        
        //paging分页参数
        public PaginatedBaseDTO<UserDTO> FindPaginatedUsersByOrgIds(int pageIndex, int pageSize, string email, string phone, string name, WorkflowBusStatus? status, PositionLevel? positionLevel, List<int> orgIds)
        {
            Expression<Func<User, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(email))
            {
                predicate = predicate.And(m => m.Email.Contains(email));
            }
            if (!string.IsNullOrWhiteSpace(phone))
            {
                predicate = predicate.And(m => m.PhoneNumber.Contains(phone));
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.DisplayName.Contains(name));
            }
            if (status.HasValue)
            {
                predicate = predicate.And(m => m.Status.Equals(status));
            }
            if (positionLevel.HasValue)
            {
                predicate = predicate.And(m => m.PositionLevel == positionLevel.Value);
            }
            if (orgIds != null && orgIds.Any())
            {
                if (orgIds.Any(m => m == -1))
                {
                    // 包含未分类组织的用户，传入参数：orgId=-1
                    predicate = predicate.And(m => !m.UserOrganizations.Any() || m.UserOrganizations.Any(o => orgIds.Contains(o.OrganizationId)));
                }
                else
                {
                    predicate = predicate.And(m => m.UserOrganizations.Any(o => orgIds.Contains(o.OrganizationId)));
                }
            }
            var data = _userRepository.FindPagenatedUsersByFilter(pageIndex, pageSize, predicate, "CreateDate", false, true);
            var total = data.Item1;
            var rows = data.Item2;
            var model = new PaginatedBaseDTO<UserDTO>(pageIndex, pageSize, total, _mapper.Map<List<UserDTO>>(rows).ToList());
            return model;
        }

        public async Task<UserSimpleDTO> GetSimpleUserByIdAsync(string userId)
        {
            var model = await _userRepository.FindByIdAsync(userId);
            return _mapper.Map<UserSimpleDTO>(model);
        }
        public async Task<UserSimpleDTO> GetSimpleUserWithOrgsAndRolesByUserIdAsync(string userId)
        {
            var model = await _userRepository.GetUserWithOrgsAndRolesByIdAsync(userId);
            return _mapper.Map<UserSimpleDTO>(model);
        }
        public async Task<UserDTO> GetUserWithOrgsAndRolesByUserIdAsync(string userId)
        {
            var model = await _userRepository.GetUserWithOrgsAndRolesByIdAsync(userId);
            return _mapper.Map<UserDTO>(model);
            //return new UserInfoMapper().Map(model);
        }

        public async Task<UserContactInfoDTO> GetUserContactInfoByIdAsync(string userId)
        {
            var data = await _userRepository.FindByIdAsync(userId);
            return _mapper.Map<UserContactInfoDTO>(data);
        }

        public async Task<IdentityResult> UserRegister(UserRegisterDTO data)
        {
            var existUser = await _userRepository.ExistUserByFilterAsync(m =>  m.PhoneNumber.Equals(data.PhoneNumber));
            if (existUser)
            {
                var message = string.Format("系统已经注册了【{0}】手机号", data.PhoneNumber);
                throw new BusinessPromptException(message);
            }

            var model = new User();
            model.Id = Guid.NewGuid().ToString();
            model.MemberId = _configApiService.GetSeedCodeByName("Member");
            model.UserName = !string.IsNullOrWhiteSpace(data.UserName) ? data.UserName : data.PhoneNumber;
            model.NormalizedUserName = model.UserName.ToUpper();
            model.Email = !string.IsNullOrWhiteSpace(data.Email) ? data.Email : data.PhoneNumber + "@139.com";
            model.NormalizedEmail = model.Email.ToUpper();
            model.DisplayName = data.DisplayName;
            model.PhoneNumber = data.PhoneNumber;
            model.UserType = data.UserType;
            model.PositionLevel = data.UserType == UserType.Company 
                    ? PositionLevel.Mananger
                    : PositionLevel.Staff;
            model.CreateDate = DateTime.UtcNow;
            model.Status = data.UserType == UserType.Staff 
                    ? WorkflowBusStatus.AuditPending
                    : WorkflowBusStatus.Approved;
            model.IsModifyPassword = true;
            model.IsDefaultMobile = true;
            model.Recommended = data.Recommended;

            //根据用户类型，授予用户默认的角色及组织
            AppandUserRolesAndOrgs(model, null, null);

            var password = data.Password;
            var result = await _userManager.CreateAsync(model, password);
            if (result != null && result.Succeeded)
            {
                //注册为组织时，需要创建该组织的相关数据
                if (model.UserType == UserType.Company)
                {
                    //添加组织架构信息
                    var org = new Organization()
                    {
                        ParentId = OrganizationConstants.注册企业_Id,
                        OrganizationType = OrganizationType.Outside,
                        OrganizationCode = _configApiService.GetSeedCodeByName("Organization"),
                        Name = data.CompanyName.IsNullOrEmpty() ? data.DisplayName : data.CompanyName,
                        BusinessType = BusinessType.Sale,
                        Status = WorkflowBusStatus.AuditPending,
                        Leaf = true,
                        Level = 2,
                        IsDeleted = false,
                        ReferenceId3 = data.Recommended,
                    };
                    org.ParentId = OrganizationConstants.注册企业_Id;
                    org.OrganizationUsers.Add(new UsersInOrganizations() { UserId = model.Id, Organization = org });

                    await _organizationRepository.AddAsync(org);
                    org.TreeCode = org.ParentId + "-" + org.Id;
                    await _organizationRepository.ModifyAsync(org, new[] { "TreeCode" });
                    //await _organizationRepository.UpdateExtendFieldsByFilterAsync(m => m.Id == org.Id);
                }
                else if (model.UserType == UserType.Staff)
                {
                    var accountApiUrl = base.GetTenantWebApiDomain(GlobalConfig.AccWebDomain);
                    var wfCode = await _workflowApiService.StartWorkflowAsync(model.Id, model.DisplayName, 
                        new DTO.Workflow.WorkflowStartExecuteData() {
                            WorkflowDefCode = "wfd2021010100001",
                            // 设置应用回调Api地址
                            WorkflowFormType = WorkflowFormType.ModelDefinition,
                            AppAuditSuccessApiUrl = accountApiUrl + "ApproveUser?userId=" + model.Id,
                            AppAuditReturnApiUrl = accountApiUrl + "DisagreeUserAsync?userId=" + model.Id,
                            ExecuteRemark = "自主注册员工提交的审批流程",
                            FormData = new List<DTO.Workflow.WorkflowProFieldDTO>()
                            {
                                new DTO.Workflow.WorkflowProFieldDTO()
                                {
                                    ParentId = null,
                                    Text = "MemberId",
                                    DataType = AttributeDataType.String,
                                    Value = model.MemberId,
                                    DisplayName = "员工编号",
                                },
                                new DTO.Workflow.WorkflowProFieldDTO()
                                {
                                    ParentId = null,
                                    Text = "DisplayName",
                                    DataType = AttributeDataType.String,
                                    Value = model.DisplayName,
                                    DisplayName = "员工姓名",
                                },
                                new DTO.Workflow.WorkflowProFieldDTO()
                                {
                                    ParentId = null,
                                    Text = "PhoneNumber",
                                    DataType = AttributeDataType.String,
                                    Value = model.PhoneNumber,
                                    DisplayName = "员工手机号",
                                },
                                new DTO.Workflow.WorkflowProFieldDTO()
                                {
                                    ParentId = null,
                                    Text = "Email",
                                    DataType = AttributeDataType.String,
                                    Value = model.Email,
                                    DisplayName = "员工邮箱",
                                }
                            }
                        });
                    var log = new UserTracingLog
                    {
                        OperatorId = model.Id,
                        Operator = model.DisplayName,
                        OperateDate = DateTime.UtcNow,
                        Type = !string.IsNullOrEmpty(wfCode)
                                ? Framework.Base.ProcessLogType.Success
                                : Framework.Base.ProcessLogType.Failure,
                        Remark = string.Format("员工【{0}】在{1}注册时，调用流程【编码：{2}】返回流程流水号：{3}，其回调函数地址：{4}",
                                model.DisplayName,
                                DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                                "wfd2021010100001",
                                wfCode,
                                accountApiUrl)
                    };
                    _userTracingLogRepository.Add(log);
                }
            }
            else if (result != null && !result.Succeeded)
            {
                throw new BusinessPromptException(string.Join(",", result.Errors));
            }

            return result;
        }
        public async Task<IdentityResult> CreateUser(UserDTO data, string operatorId, string operatorName,
            string[] organizations, string gussourl)
        {
            var existUser = await _userRepository.ExistUserByFilterAsync(m => m.UserName.Equals(data.UserName) || m.PhoneNumber.Equals(data.PhoneNumber) || m.Email.Equals(data.Email));
            if (existUser)
            {
                var message = string.Format("系统已经注册了【{0}】登录名，或是【{1}】手机号，或是【{2}】邮箱", data.UserName, data.PhoneNumber, data.Email);
                throw new BusinessPromptException(message);
            }

            //创建用户个数限制
            CreateUserLimit();

            data.UserId = Guid.NewGuid().ToString();
            data.MemberId = _configApiService.GetSeedCodeByName("Member");
            string[] Snum = data.UserName.Split(',');
            data.UserName = Snum[Snum.Count() - 1];

            var model = _mapper.Map<User>(data);
            model.Recommended = Tenant.TenantName;
            model.UserName = !string.IsNullOrWhiteSpace(data.UserName) ? data.UserName : data.Email;
            model.CreateDate = DateTime.UtcNow;
            model.Status = WorkflowBusStatus.AuditPending;
            var password = EncryptPasswordUtil.GetRandomString(); //生成用户的初始化登录密码

            var result = await _userManager.CreateAsync(model, password);
            if (result != null && result.Succeeded)
            {
                var organizationId = organizations.Select(m => Int32.Parse(m)).ToList();
                _userRepository.UpdateUserInOrganizations(organizationId, data.UserId);
                await Task.Factory.StartNew(async () =>
                {
                    //创建用户成功发送消息
                    var inputParameter = new Dictionary<string, string> {
                        {"UserId", model.Id},
                        {"UserName", model.Email},
                        {"DisplayName", model.DisplayName},
                        {"Email", model.Email},
                        {"PhoneNumber", data.PhoneNumber},
                        {"Password", password},
                        {"LoginUrl", GlobalConfig.AccWebDomain}
                    };
                    var user = new SendUserDTO()
                    {
                        UserId = model.Id,
                        UserName = model.UserName,
                        DisplayName = model.DisplayName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                    };

                    bool isSucceedSendEmail = _messageUtil.SendMessage(Tenant, UserTemplateGenerator.User_Created, inputParameter, new List<SendUserDTO>() { user });

                    var logs = new UserTracingLog
                    {
                        OperatorId = operatorId,
                        Operator = operatorName,
                        OperateDate = DateTime.UtcNow,
                        Type = (isSucceedSendEmail)
                                ? Framework.Base.ProcessLogType.Success
                                : Framework.Base.ProcessLogType.Failure,
                        Remark = string.Format("{0}在{1}进行了以下操作：{2}！",
                                operatorName,
                                DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                                "创建了用户（" + model.Id + ", " + model.UserName + ")")
                    };
                    _userTracingLogRepository.Add(logs);

                    //ServiceEvents.Register(new UserCreatedHandler(Tenant, data, password));
                });
            }
            else if (result != null && !result.Succeeded)
            {
                throw new BusinessPromptException(string.Join(",", result.Errors));
            }

            return result;
        }
        /// <summary>
        /// 根据用户类型，授予用户默认的角色及组织
        /// </summary>
        /// <param name="model">用户</param>
        /// <param name="roleIds">需要附加的角色</param>
        /// <param name="organizationIds">需要附加的部门</param>
        private void AppandUserRolesAndOrgs(User model, List<string> roleIds, List<int> organizationIds)
        {
            //根据用户类型，授予用户默认的角色
            if (roleIds != null)
            {
                foreach (var roleId in roleIds)
                {
                    if (!model.UserRoles.Any(m => m.RoleId.Equals(roleId)))
                        model.UserRoles.Add(new UsersInRoles() { RoleId = roleId, User = model });
                }
            }
            //根据用户类型，授予用户默认的组织
            if (organizationIds != null)
            {
                model.UserOrganizations.Clear();
                foreach (var orgId in organizationIds)
                {
                    if (!model.UserOrganizations.Any(m => m.OrganizationId.Equals(orgId)))
                        model.UserOrganizations.Add(new UsersInOrganizations() { OrganizationId = orgId, User = model });
                }
            }
            var defaultRoleId = RoleConstants.DefaultRoleId;
            var defaultOrgId = OrganizationConstants.企业_Id;
            switch (model.UserType)
            {
                case UserType.Person:
                    model.IsClient = true;
                    defaultRoleId = RoleConstants.RegisterPersonRoleId;
                    if (roleIds == null
                        || (roleIds != null && !roleIds.Any(m => m.Equals(defaultRoleId))))
                        model.UserRoles.Add(new UsersInRoles() { RoleId = defaultRoleId, User = model });
                    break;
                case UserType.Company:
                    model.IsClient = true;
                    defaultRoleId = RoleConstants.RegisterCompanyRoleId;
                    if (roleIds == null
                        || (roleIds != null && !roleIds.Any(m => m.Equals(defaultRoleId))))
                        model.UserRoles.Add(new UsersInRoles() { RoleId = defaultRoleId, User = model });
                    defaultOrgId = OrganizationConstants.注册企业_Id;
                    if (organizationIds == null
                        || (organizationIds != null && !organizationIds.Any(m => m.Equals(defaultOrgId))))
                        model.UserOrganizations.Add(new UsersInOrganizations() { OrganizationId = defaultOrgId, User = model });
                    break;
                case UserType.Staff:
                    model.IsClient = false;
                    defaultRoleId = RoleConstants.DefaultRoleId;
                    if (roleIds == null
                        || (roleIds != null && !roleIds.Any(m => m.Equals(defaultRoleId))))
                        model.UserRoles.Add(new UsersInRoles() { RoleId = defaultRoleId, User = model });
                    defaultOrgId = OrganizationConstants.人事部_Id;
                    if (organizationIds == null
                        || (organizationIds != null && !organizationIds.Any(m => m.Equals(defaultOrgId))))
                        model.UserOrganizations.Add(new UsersInOrganizations() { OrganizationId = defaultOrgId, User = model });
                    break;
            }
        }

        public bool UpdateUser(UserDTO data, string operatorId, string operatorName, string[] organizations)
        {
            string[] Snum = data.UserName.Split(',');
            data.UserName = Snum[Snum.Count() - 1];
            //同名验证判定
            var exist = _userRepository.ExistUserByFilterAsync(m => !m.Id.Equals(data.UserId) && (m.UserName.Equals(data.UserName) || m.PhoneNumber.Equals(data.PhoneNumber) || m.Email.Equals(data.Email)));
            if (exist.Result)
            {
                var message = string.Format("系统已经注册了【{0}】登录名，或是【{1}】手机号，或是【{2}】邮箱", data.UserName, data.PhoneNumber, data.Email);
                throw new BusinessPromptException(message);
            }

            var model = _mapper.Map<User>(data);
            var organizationIds = organizations.Select(m => int.Parse(m)).ToList();
            _userRepository.UpdateUser(model, organizationIds, false);

            const string logMessage = "{0}进行了以下操作：{1}！";
            var log = new UserTracingLog
            {
                OperatorId = operatorId,
                Operator = operatorName,
                OperateDate = DateTime.UtcNow,
                Type = Framework.Base.ProcessLogType.Success,
                Remark = string.Format(logMessage,
                    operatorName,
                    "更新了用户（" + model.UserName + ")相关信息")
            };
            _userTracingLogRepository.Add(log, false);
            return _dbContext.SaveChanges() > 0; 
        }
        public bool UpdateRoleInUser(string userId, List<string> addList, string operatorId, string operatorName)
        {
            return _userRepository.SaveRoleInUser(userId, addList, operatorId, operatorName);
        }
        public bool UpdateOrganizationInUser(string userId, List<int> addList, string operatorId, string operatorName)
        {
            return _userRepository.SaveOrganizationInUser(userId, addList, operatorId, operatorName);
        }

        public bool RemoveUserById(string id, string operatorId, string operatorName)
        {
            return _userRepository.RemoveUserById(id, operatorId, operatorName);
        }
        public bool ReactUserById(string id, string operatorId, string operatorName)
        {
            return _userRepository.ReactUserById(id, operatorId, operatorName);
        }

        
        #endregion

        #region DownLoad User Excel && Import User data from Excel

        public bool ImportUserDataFromExcel(Stream stream)
        {
            var rows = new NPOIExcelReader(stream).GetWorksheetRowListData();
            var seed = _configApiService.GetSeedEntityByName("Member", rows.Count);
            var min = 10;
            var codePrefix = "ur";

            var newUsers = new List<User>();
            for (var i = 0; i < rows.Count; i++)
            {
                var code = codePrefix + (min + i).ToString().PadLeft(5, '0');

                #region 拼装AspNetUser对象
                var user = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    MemberId = code,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PasswordHash = "AMaw+mzoTNm+VnSIxMrkvPNFWoXu2f84SdacTW8S4UhKm2L63xY8fUVcBlSZh1UZVQ==",
                    SecurityStamp = "4f67bcf7-1ada-415b-924e-eedae494a32c",
                    PositionLevel = PositionLevel.Staff,
                    IsDefaultMobile = true,
                    CreateDate = DateTime.UtcNow,
                };

                var row = rows[i];
                foreach (var cell in row)
                {
                    switch (cell.ColumnName)
                    {
                        case "Id":
                            if (!string.IsNullOrWhiteSpace(cell.CellValue))
                            {
                                user.Id = cell.CellValue;
                            }
                            break;
                        case "用户编号":
                            if (!string.IsNullOrWhiteSpace(cell.CellValue))
                            {
                                user.MemberId = cell.CellValue;
                            }
                            break;
                        case "用户名称":
                            if (!string.IsNullOrWhiteSpace(cell.CellValue))
                            {
                                user.DisplayName = cell.CellValue;
                            }
                            break;
                        case "用户邮箱":
                            if (!string.IsNullOrWhiteSpace(cell.CellValue))
                            {
                                user.UserName = cell.CellValue;
                                user.Email = cell.CellValue;
                            }
                            break;
                        case "用户手机":
                            if (!string.IsNullOrWhiteSpace(cell.CellValue))
                            {
                                user.PhoneNumber = cell.CellValue;
                            }
                            break;
                        case "用户加密密码":
                            if (!string.IsNullOrWhiteSpace(cell.CellValue))
                            {
                                user.PasswordHash = cell.CellValue;
                            }
                            break;
                        case "加密时间戳":
                            if (!string.IsNullOrWhiteSpace(cell.CellValue))
                            {
                                user.SecurityStamp = cell.CellValue;
                            }
                            break;
                    }
                }
                #endregion

                newUsers.Add(user);
            }

            if (!newUsers.Any())
                return true;

            var dbUsers = _userRepository.FindAllDetailUsers();
            foreach (var user in newUsers.ToList())
            {
                var dbCustomer =
                    dbUsers.FirstOrDefault(
                        m => m.Id != null && m.Id.Equals(user.Id));
                if (dbCustomer != null)
                {
                    //dbCustomer.MemberId = user.MemberId;
                    dbCustomer.DisplayName = user.DisplayName;
                    dbCustomer.UserName = user.UserName;
                    dbCustomer.Email = user.Email;
                    dbCustomer.PhoneNumber = user.PhoneNumber;
                    dbCustomer.PasswordHash = user.PasswordHash;
                    dbCustomer.SecurityStamp = user.SecurityStamp;
                    dbCustomer.ContactQQ = user.ContactQQ;
                    var properties = new[] { "DisplayName", "UserName", "Email", "PhoneNumber", "PasswordHash", "SecurityStamp", "ContactQQ" };
                    _userRepository.UpdateUser(dbCustomer, null, false);
                }
                else
                {
                    _userRepository.AddUsers(newUsers, false);
                }
            }

            //using (var scope = ServiceProvider.CreateScope())
            {
                //var dbContext = scope.ServiceProvider.GetRequiredService<ComAccountContext>();
                return _dbContext.SaveChanges() > 0;
            }
        }

        #endregion

        #region Password

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var cResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if(cResult.Succeeded)
            {
                await _userManager.UpdateAsync(user);
            }

            return cResult;
        }

        public async Task<IdentityResult> ChangeAdminRawInfoAsync(string newPassword, string adminEmail, string adminPhone)
        {
            var userId = RoleConstants.AdminUserId;
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ChangePasswordAsync(user, "123456", newPassword);
            if (!result.Succeeded)
                return result;

            if (user != null)
            {
                user.Email = adminEmail;
                user.PhoneNumber = adminPhone;
                return await _userManager.UpdateAsync(user);
            }
            return new IdentityResult();
        }

        public async Task<IdentityResult> ChangeMailPhoneAsync(string userId, string email, string phone)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Email = email;
                user.PhoneNumber = phone;
                return await _userManager.UpdateAsync(user);
            }
            return new IdentityResult();
        }

        public async Task<IdentityResult> AuditUserStatus(string userId, WorkflowBusStatus status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Status = status;
                return await _userManager.UpdateAsync(user);
            }

            return new IdentityResult();
        }
        #endregion

        #region utils
        /// <summary>
        /// 创建用户发送短信
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool CreateUserSendSms(UserDTO data)
        {
            long phone;
            var isSucceed = false;
            var smsContent = string.Format("【财富共赢】尊敬的{0}：您的产融协作员工账号已创建成功, 详情已发送至您的{1}，请登录查看。", data.DisplayName,
                data.Email);
            if (long.TryParse(data.PhoneNumber, out phone))
            {
                var smsInfo = new KC.Model.Component.Queue.SmsInfo
                {
                    Tenant = data.TenantName,
                    Type = KC.Framework.Base.SmsType.Marketing,
                    Phone = new List<long> { phone },
                    SmsContent = smsContent
                };
                isSucceed = new StorageQueueService().InsertSmsQueue(smsInfo);
                Logger.LogInformation((isSucceed ? "插入短信队列成功，短信内容：" : "插入短信队列失败，短信内容：") + smsContent);
            }
            return isSucceed;
        }

        private void CreateUserLimit()
        {
            var limit = "15,50";
            var users = _userRepository.FindAllDetailUsers();
            int count;
            if (Tenant.Version == Framework.Tenant.TenantVersion.Standard)
            {
                var standardTenantCount = limit.Split(',')[0];
                if (int.TryParse(standardTenantCount, out count))
                {
                    if (users.Count >= count)
                    {
                        throw new BusinessPromptException(string.Format("标准版租户创建用户不能超过{0}个", count));
                    }
                }
            }
            if (Tenant.Version == Framework.Tenant.TenantVersion.Professional)
            {
                var professionalTenantCount = limit.Split(',')[1];
                if (int.TryParse(professionalTenantCount, out count))
                {
                    if (users.Count >= count)
                    {
                        throw new BusinessPromptException(string.Format("专业版租户创建用户不能超过{0}个", count));
                    }
                }
            }
        }

        #endregion
        
    }

}
