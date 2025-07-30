using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using AutoMapper;
using KC.Framework.Exceptions;
using KC.Framework.Extension;
using KC.DataAccess.Customer.Repository;
using KC.Database.EFRepository;
using KC.Enums.CRM;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Customer;
using KC.Model.Component.Queue;
using KC.Service.CallCenter;
using KC.Service.EFService;
using KC.Service.Util;
using KC.Service.WebApiService.Business;
using KC.Service.DTO;
using KC.Service.DTO.Customer;
using KC.Service.Component;
using KC.ThirdParty;
using KC.Database.IRepository;
using KC.Service;
using KC.Service.Customer.WebApiService.Business;
using KC.Service.DTO.Account;
using System.Threading.Tasks;
using KC.Service.Customer.WebApiService;

namespace KC.Service.Customer
{
    public interface ICustomerService : IEFService
    {
        #region Customer Info

        List<CustomerInfoDTO> GetCustomerInfosByIds(List<int> idList);
        List<CustomerInfoDTO> GetCustomerInfosByFilter(int pageIndex, int pageSize, string searchKey, string searchValue);

        Task<PaginatedBaseDTO<CustomerInfoDTO>> GetPaginatedCustomerInfosByFilterAsync(int pageIndex, int pageSize,
            string searchKey, string searchValue, Dictionary<string, int> typeList, List<string> roleList,
            string currentUserId, bool onlyShowClientSerivce, string customerManangeName, DateTime? createdStartTime,
            DateTime? createdEndTime, string businessScope, string area, bool onlyShowAssignedCustomer);


        PaginatedBaseDTO<CustomerInfoDTO> GetPaginatedCustomerInfosByIdList(int pageIndex, int pageSize,
            List<int> ids, string viewName = "");

        CustomerInfoDTO GetCustomerInfoById(int id);

        Tuple<int, string> SendMess(string id, int currentstatus, string DisplayName, string currentUserId,
            bool isSingle);

        Tuple<int, string> SaveCustomerInfo(CustomerInfoDTO model, string operatorName, string operatorId);

        bool ImportCustomersFromOtherTenant(List<CustomerInfoDTO> data);
        bool RemoveCustomerInfoByIds(List<int> ids, string currentUserName);
        bool EditCustomer(string id, string introduction);

        /// <summary>
        /// 推送客户给其他租户
        /// </summary>
        /// <param name="customerIds"></param>
        /// <param name="tenantNames"></param>
        /// <param name="opertorId"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        string SendCustomersToTenant(List<int> customerIds, List<string> tenantNames, string opertorId,
            string operatorName);

        /// <summary>
        /// 是否存在个人客户的手机号
        /// </summary>
        /// <param name="customerId">客户Id</param>
        /// <param name="phoneNumber">联系人手机号码</param>
        /// <returns>存在返回：true，否则返回：false</returns>
        bool ExistPersonalCustomerPhoneNumber(int customerId,string phoneNumber);

        #region 客服分配

        bool ShareCustomerWithManager(List<string> selectedUsers, string operatorId);

        bool ReassignCustomerToOtherManager(List<string> selectedUserIds, List<int> customerIds, string operatorId);

        /// <summary>
        /// 转移客户
        /// </summary>
        /// <param name="selectdUserId">客服Id</param>
        /// <param name="customerIds">客户Ids</param>
        /// <param name="operatorId">当前操作人</param>
        /// <returns></returns>
        bool TransferCustomerToOtherManager(string selectdUserId, List<int> customerIds, string operatorId);

        /// <summary>
        /// 获取某客户分配到当前操作人下级部门的情况
        /// </summary>
        /// <param name="customerId">客户Id</param>
        /// <param name="operatorId">当前操作人Id</param>
        /// <returns></returns>
        Task<string> GetCustomerAssignedInfoAsync(int customerId, string operatorId);

        Task<List<CustomerManagerDTO>> GetCustomerManagersByCustomerIdAsync(string currentUserId, int customerId,
            List<string> roleList);

        #endregion

        #region DownLoad Customer Excel && Import Customer data from Excel

        byte[] GetCustomerTemplate(string templateUrl);

        bool ImportCustomerDataFromExcel(Stream stream, string managerId, string managerName, ref int importToRowIndex,
            StringBuilder businessExceptions);
        List<CustomerInfoDTO> GetExcelCustomerData(byte[] excelData);

        DataTable SearchCustomerTable(string searchKey, string searchValue, Dictionary<string, int> typeList,
            List<string> roleList,
            string currentUserId, bool onlyShowClientSerivce);

        #endregion

        #endregion

        #region Customer Contact Info

        List<CustomerContactDTO> GetAllCustomerContacts();
        List<CustomerContactDTO> GetCustomerContactsByCustomerId(int customerId, bool isDelete);

        PaginatedBaseDTO<CustomerContactDTO> GetPaginatedCustomerContactsByCustomerId(int pageIndex, int pageSize,
            string searchKey, string searchValue, int id);

        PaginatedBaseDTO<CustomerContactInfoDTO> GetPaginatedCustomerContactInfos(int pageIndex, int pageSize, string currentUserId, List<string> roleList, 
            CompanyType? companyType, string viewName, string searchKey, string searchValue);

        CustomerContactDTO GetCustomerContactsById(int id);
        bool EditCustomerContact(CustomerContactDTO model);
        bool CreateCustomerContact(CustomerContactDTO model);
        bool RemoveCustomerContact(int id);
        bool RemoveCustomerContactSoft(int id);

        #endregion

        #region Customer change log

        List<CustomerChangeLogDTO> GetAllCustomerChangeLogs();


        PaginatedBaseDTO<CustomerChangeLogDTO> GetCustomerChangeLogsByCustomerInfoId(int pageIndex, int pageSize,
            string searchKey, string name,
            string startTime, string endTime, int customerId);

        CustomerChangeLogDTO GetCustomerChangeLogDTO(int id);
        bool EditCustomerChangeLogDTO(CustomerChangeLogDTO model);
        bool AddCustomerChangeLogDTO(CustomerChangeLogDTO model);
        bool RemoveCustomerChangeLogDTO(int id);

        #endregion

        #region Customer Tracing Info

        PaginatedBaseDTO<CustomerTracingLogDTO> GetActivityLogListByCustomerInfoId(int id, int pageIndex, int pageSize,
            string customerName, string activityName, string operatoruser, string startTime, string endTime,
            TracingType? type);

        PaginatedBaseDTO<CustomerTracingLogDTO> GetCustomerTraceInfoReport(int pageIndex, int pageSize,
            string currentUserId, List<string> roleList, int? currentContactId,
            string searchKey, string searchValue, string startTime, string endTime, CompanyType? companyType,
            TracingType? type,
            ProcessLogType? processLogType, CallState? callState);

        CustomerTracingLogDTO GetActivityLogById(int id);

        bool SaveCustomerTracingLogDTO(CustomerTracingLogDTO model);
        bool SaveCustomerTracingLogDTO(IList<CustomerTracingLogDTO> model);

        bool RomoveCustomerTracingLogDTO(List<int> ids);

        List<CustomerContactInfoDTO> GetCustomerContactsByCompanyType(CompanyType companyType);
        List<CustomerContactInfoDTO> GetCustomerCompanyByCustomerContact(int customerContactId);

        #endregion

        #region Customer SendToTenant Log

        PaginatedBaseDTO<CustomerSendToTenantLogDTO> GetCustomerSendLogListByFilter(int pageIndex,
            int pageSize, string tenantName, string tenantDisplayName, string operatoruser, string startTime,
            string endTime, ProcessLogType? type);

        #endregion

        #region CustomerExtInfo

        bool SaveCustomerExtInfoDtos(List<CustomerExtInfoDTO> list);
        List<CustomerExtInfoDTO> GetCustomerExtInfoDtosByCustomerId(int CustomerId);
        bool RomoveCustomerExtInfoDto(int id);
        bool RomoveCustomerExtInfoDtoSoft(int id);

        PaginatedBaseDTO<CustomerExtInfoDTO> GetPaginatedCustomerExtInfoByCustomerId(int pageIndex, int pageSize,
            int customerId);

        #endregion

        #region Call Center

        string IsNotBusy(string currentUserPhone);

        /// <summary>
        /// 拨打电话
        ///     record--主叫号码类型: 0: 座机；1: 手机
        /// </summary>
        /// <param name="customerContactId"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentUserName"></param>
        /// <param name="currentUserPhone"></param>
        /// <param name="customerId"></param>
        /// <param name="customerName"></param>
        ///  <param name="contactName"></param>
        /// <param name="customerPhoneNumber"></param>
        /// <param name="record">
        /// <param name="referenceId"></param>
        /// 主叫号码类型
        ///     0: 座机；1: 手机
        /// </param>
        /// <returns></returns>
        string RunCallContact(int customerContactId,string currentUserId, string currentUserName, string currentUserPhone, int customerId,
            string customerName, string contactName, string customerPhoneNumber, CallType record, out string referenceId);

        bool StopCallContact(string customerPhoneNumber, string message);

        bool SavePopCustomerInfo(CustomerContactInfoDTO model, string referenceId, string callRemark);

        #endregion

        #region 群发申请

        NotificationApplicationDTO GetApplicatioByIds(int id);

        PaginatedBaseDTO<NotificationApplicationDTO> GetNotificationApplication(int pageIndex, int pageSize,
            string searchValue, int status, string startTime, string endTime, List<string> roleList,
            string currentUserId, bool onlyShowClientSerivce);

        bool SaveSendApplication(NotificationApplicationDTO data);
        bool SaveApplication(NotificationApplicationDTO data, string[] properties);

        #endregion
    }

    public class CustomerService : EFServiceBase, ICustomerService
    {
        private readonly IMapper _mapper;

        #region Db Repository

        protected EFUnitOfWorkContextBase CustomerUnitOfWorkContext { get; private set; }
        //protected EFUnitOfWorkContextBase AccountUnitOfWorkContext { get; private set; }

        private readonly ICustomerApiService _customerApiService;
        private readonly IDictionaryApiService _dictionaryApiService;

        private readonly ICustomerInfoRepository _customerInfoRepository;
        private readonly ICustomerManagerRepository _customerManagerRepository;
        private readonly ICustomerContactRepository _customerContactRepository;
        
        private readonly IDbRepository<CustomerExtInfoProvider> _customerExtInfoProviderRepository;
        private readonly IDbRepository<CustomerExtInfo> _customerExtInfoRepository;
        
        private readonly IDbRepository<CustomerChangeLog> _customerChangeLogRepository;
        private readonly IDbRepository<CustomerSendToTenantLog> _customerSendToTenantLogRepository;
        private readonly ICustomerTracingLogRepository _customerTracingLogRepository;

        private readonly IDbRepository<NotificationApplication> _notificationApplicationRepository;

        private readonly ITenantSimpleApiService TenantUserApiService;
        private readonly IConfigApiService ConfigApiService;

        private readonly IAccountApiService AccountApiService;
        #endregion

        public CustomerService(
            Tenant tenant,
            IMapper mapper,
            IAccountApiService accountApiService,
            ITenantSimpleApiService tenantUserApiService,
            IConfigApiService configApiService,
            IDictionaryApiService dictionaryApiService,
            ICustomerApiService customerApiService,
            EFUnitOfWorkContextBase unitOfWorkContext,

            ICustomerInfoRepository customerInfoRepository,
            ICustomerManagerRepository customerManagerRepository,
            ICustomerContactRepository customerContactRepository,
            IDbRepository<CustomerExtInfoProvider> customerExtInfoProviderRepository,
            IDbRepository<CustomerExtInfo> customerExtInfoRepository,
            IDbRepository<CustomerChangeLog> customerChangeLogRepository,
            IDbRepository<CustomerSendToTenantLog> customerSendToTenantLogRepository,
            ICustomerTracingLogRepository customerTracingLogRepository,
            IDbRepository<NotificationApplication> notificationApplicationRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<CustomerService> logger)
            : base(tenant,  clientFactory, logger)
        {
            _mapper = mapper;
            CustomerUnitOfWorkContext = unitOfWorkContext;
            TenantUserApiService = tenantUserApiService;
            ConfigApiService = configApiService;
            AccountApiService = accountApiService;

            _customerApiService = customerApiService;
            _dictionaryApiService = dictionaryApiService;

            _customerInfoRepository = customerInfoRepository;
            _customerManagerRepository = customerManagerRepository;

            _customerContactRepository = customerContactRepository;

            _customerExtInfoProviderRepository = customerExtInfoProviderRepository;
            _customerExtInfoRepository = customerExtInfoRepository;

            _customerChangeLogRepository = customerChangeLogRepository;
            _customerSendToTenantLogRepository = customerSendToTenantLogRepository;
            _customerTracingLogRepository = customerTracingLogRepository;

            _notificationApplicationRepository = notificationApplicationRepository;
        }

        #region Customer Info

        /// <summary>
        /// 生成用户信息列表 - List<CustomerInfoDTO> GenerateCustomerInfoList(string idList)
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public List<CustomerInfoDTO> GetCustomerInfosByIds(List<int> idList)
        {
            var model = _customerInfoRepository.GetCustomerInfosByIds(idList);
            return _mapper.Map<List<CustomerInfoDTO>>(model);
        }

        /// <summary>
        /// 抄送弹窗分页筛选
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchKey"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public List<CustomerInfoDTO> GetCustomerInfosByFilter(int pageIndex, int pageSize, string searchKey,
            string searchValue)
        {
            Expression<Func<CustomerInfo, bool>> predicate = m => true;

            if (!string.IsNullOrWhiteSpace(searchKey)
                && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey)
                {
                    case "CustomerName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.CustomerName.Contains(searchValue));
                        }
                        break;
                    case "ContactName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.ContactName.Contains(searchValue));
                        }
                        break;
                    //case "ContactEmail":
                    //    if (!string.IsNullOrWhiteSpace(searchValue))
                    //    {
                    //        predicate = predicate.And(m => m.ContactEmail.Contains(searchValue));
                    //    }
                    //    break;
                }
            }
            var data = _customerInfoRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.CreatedDate,
                false);
            var rows = _mapper.Map<List<CustomerInfoDTO>>(data.Item2);
            return rows;
        }

        /// <summary>
        /// 加载分页过的客户列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchKey"></param>
        /// <param name="searchValue"></param>
        /// <param name="typeList"></param>
        /// <param name="roleList"></param>
        /// <param name="currentUserId"></param>
        /// <param name="onlyShowClientSerivce">是否只显示分配给客服人员的客户（true：只显示分配给客服人员的客户）</param>
        /// <param name="customerManangeName">客户经理名称</param>
        /// <param name="createdStartTime"></param>
        /// <param name="createdEndTime"></param>
        /// <param name="businessScope"></param>
        /// <param name="area"></param>
        /// <param name="onlyShowAssignedCustomer">是否只显示已经分配过客服的客户（true：只显示分配过客服的客户；false：显示分配过和没分配过客服的客户）</param>
        /// <returns></returns>
        public async Task<PaginatedBaseDTO<CustomerInfoDTO>> GetPaginatedCustomerInfosByFilterAsync(int pageIndex, int pageSize,
            string searchKey, string searchValue, Dictionary<string, int> typeList, List<string> roleList,
            string currentUserId, bool onlyShowClientSerivce, string customerManangeName, DateTime? createdStartTime,
            DateTime? createdEndTime, string businessScope, string area, bool onlyShowAssignedCustomer)
        {
            var userOrg = await AccountApiService.LoadOrganizationsWithUsersByUserId(currentUserId);
            if (!userOrg.Any())
                throw new BusinessPromptException("不存在部门信息");

            var customerIds = GetCurrentUserCanOperateCustomerIds(currentUserId, roleList);
            Expression<Func<CustomerInfo, bool>> predicate = m => !m.IsDeleted && customerIds.Contains(m.CustomerId);
            if (!string.IsNullOrWhiteSpace(customerManangeName))
            {
                predicate =
                    predicate.And(m => m.CustomerManagers.Any(n => n.CustomerManagerName.Contains(customerManangeName)));
            }
            if (createdStartTime.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate >= createdStartTime.Value);
            }
            if (createdEndTime.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate <= createdEndTime.Value);
            }

            #region searchKey & searchValue

            if (!string.IsNullOrWhiteSpace(searchKey)
                && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey)
                {
                    case "CustomerName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.CustomerName.Contains(searchValue));
                        }
                        break;
                    case "ContactName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.ContactName.Contains(searchValue));
                        }
                        break;
                    //case "ContactPhoneNumber":
                    //    if (!string.IsNullOrWhiteSpace(searchValue))
                    //    {
                    //        predicate = predicate.And(m => m.ContactPhoneNumber.Contains(searchValue));
                    //    }
                    //    break;
                    //case "ContactQQ":
                    //    if (!string.IsNullOrWhiteSpace(searchValue))
                    //    {
                    //        predicate = predicate.And(m => m.ContactQQ.Contains(searchValue));
                    //    }
                    //    break;
                    //case "ContactEmail":
                    //    if (!string.IsNullOrWhiteSpace(searchValue))
                    //    {
                    //        predicate = predicate.And(m => m.ContactEmail.Contains(searchValue));
                    //    }
                    //    break;
                    case "RecommandedUserName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.RecommandedUserName.Contains(searchValue));
                        }
                        break;
                    case "OrganizationName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate =
                                predicate.And(
                                    m => m.CustomerManagers.Any(n => n.OrganizationName.Contains(searchValue)));
                        }
                        break;
                }
            }

            #endregion

            #region 4 types selection operation

            if (typeList.Count != 0)
            {
                foreach (var kvp in typeList)
                {
                    if (kvp.Value != 100)
                    {
                        switch (kvp.Key)
                        {
                            case "companyType":
                                var companyType = ((CompanyType)typeList["companyType"]);
                                predicate = predicate.And(m => m.CompanyType == companyType);
                                break;
                            case "clientType":
                                var clientType = ((ClientType)typeList["clientType"]);
                                predicate = predicate.And(m => m.ClientType == clientType);
                                break;
                            case "customerSource":
                                var customerSource = ((CustomerSource)typeList["customerSource"]);
                                predicate = predicate.And(m => m.CustomerSource == customerSource);
                                break;
                        }
                    }
                }
            }

            #endregion

            var data = await _customerInfoRepository.FindPagenatedListWithCountAsync(
                pageIndex,
                pageSize,
                predicate,
                m => m.CustomerId,
                false);
            var total = data.Item1;
            var rows = _mapper.Map<List<CustomerInfoDTO>>(data.Item2);
            var result = new PaginatedBaseDTO<CustomerInfoDTO>(pageIndex, pageSize, total, rows);
            return result;
        }

        public DataTable SearchCustomerTable(string searchKey, string searchValue, Dictionary<string, int> typeList,
            List<string> roleList,
            string currentUserId, bool onlyShowClientSerivce)
        {
            Expression<Func<CustomerInfo, bool>> predicate = m => true;
            string sqlstr =
                "select CustomerName,CompanyType,ClientType,CustomerLevel,ContactName,ContactPhoneNumber,ContactEmail,RecommandedUserName,CustomerManangeName,OrganizationName from DevDB.crm_CustomerInfo ";
            string whereStr = "";

            #region searchKey & searchValue

            SqlParameter[] sqlparams;
            if (!string.IsNullOrWhiteSpace(searchKey) && !string.IsNullOrWhiteSpace(searchValue))
            {
                sqlparams = new SqlParameter[5];
            }
            else
            {
                sqlparams = new SqlParameter[4];
            }
            if (!string.IsNullOrWhiteSpace(searchKey)
                && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey)
                {
                    case "CustomerName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            sqlparams[0] = new SqlParameter("CustomerName", searchValue);
                            whereStr = "Where CustomerName=@CustomerName";
                        }
                        break;
                    case "ContactName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            sqlparams[0] = new SqlParameter("ContactName", searchValue);
                            whereStr = "Where ContactName=@ContactName";
                        }
                        break;
                    case "ContactPhoneNumber":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            sqlparams[0] = new SqlParameter("ContactPhoneNumber", searchValue);
                            whereStr = "Where ContactPhoneNumber=@ContactPhoneNumber";
                        }
                        break;
                    case "ContactQQ":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            sqlparams[0] = new SqlParameter("ContactQQ", searchValue);
                            whereStr = "Where ContactQQ=@ContactQQ";
                        }
                        break;
                    case "ContactEmail":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            sqlparams[0] = new SqlParameter("ContactEmail", searchValue);
                            whereStr = "Where ContactEmail=@ContactEmail";
                        }
                        break;
                    case "RecommandedUserName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            sqlparams[0] = new SqlParameter("RecommandedUserName", searchValue);
                            whereStr = "Where RecommandedUserName=@RecommandedUserName";
                        }
                        break;
                    case "CustomerManangeName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            sqlparams[0] = new SqlParameter("CustomerManangeName", searchValue);
                            whereStr = "Where CustomerManangeName=@CustomerManangeName";
                        }
                        break;
                    case "OrganizationName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            sqlparams[0] = new SqlParameter("OrganizationName", searchValue);
                            whereStr = "Where OrganizationName=@OrganizationName";
                        }
                        break;
                }
            }

            #endregion

            #region 4 types selection operation

            if (typeList.Count != 0)
            {
                foreach (var kvp in typeList)
                {
                    if (kvp.Value != 100)
                    {
                        switch (kvp.Key)
                        {
                            case "companyType":
                                var companyType = ((CompanyType)typeList["companyType"]);
                                if (sqlparams.Length == 5)
                                {
                                    sqlparams[1] = new SqlParameter("CompanyType", companyType);
                                    whereStr = whereStr + " And CompanyType=@CompanyType";
                                }
                                else
                                {
                                    sqlparams[0] = new SqlParameter("CompanyType", companyType);
                                    whereStr = whereStr + "Where CompanyType=@CompanyType";
                                }
                                predicate = predicate.And(m => m.CompanyType == companyType);
                                break;
                            case "clientType":
                                var clientType = ((ClientType)typeList["clientType"]);
                                if (sqlparams.Length == 5)
                                {
                                    sqlparams[2] = new SqlParameter("ClientType", clientType);
                                    whereStr = whereStr + " And ClientType=@ClientType";
                                }
                                else
                                {
                                    sqlparams[1] = new SqlParameter("ClientType", clientType);
                                    whereStr = whereStr + "Where ClientType=@ClientType";
                                }
                                predicate = predicate.And(m => m.ClientType == clientType);
                                break;
                            case "customerSource":
                                var customerSource = ((CustomerSource)typeList["customerSource"]);
                                if (sqlparams.Length == 5)
                                {
                                    sqlparams[4] = new SqlParameter("CustomerSource", customerSource);
                                    whereStr = whereStr + " And CustomerSource=@CustomerSource";
                                }
                                else
                                {
                                    sqlparams[3] = new SqlParameter("CustomerSource", customerSource);
                                    whereStr = whereStr + "Where CustomerSource=@CustomerSource";
                                }
                                predicate = predicate.And(m => m.CustomerSource == customerSource);
                                break;
                        }
                    }
                }
            }

            #endregion

            return null;
        }

        /// <summary>
        /// 根据客户Id集合获取客户分页数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="ids">客户Id集合</param>
        /// <param name="viewName">消息类型</param>
        /// <returns></returns>
        public PaginatedBaseDTO<CustomerInfoDTO> GetPaginatedCustomerInfosByIdList(int pageIndex, int pageSize,
            List<int> ids, string viewName = "")
        {
            var customersDto = GetCustomerInfosByIds(ids);

            var total = customersDto.Count;
            var rows = customersDto.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            return new PaginatedBaseDTO<CustomerInfoDTO>(pageIndex, pageSize, total, rows);
        }

        public CustomerInfoDTO GetCustomerInfoById(int id)
        {
            var data = _customerInfoRepository.GetById(id);
            return _mapper.Map<CustomerInfoDTO>(data);
        }

        public Tuple<int, string> SaveCustomerInfo(CustomerInfoDTO data, string operatorName, string operatorId)
        {
            //var user = _userRepository.FindByIdIncludeOrganizations(operatorId);
            var userOrgs = AccountApiService.LoadOrganizationsWithUsersByUserId(operatorId).Result;
            if (!userOrgs.Any())
                throw new BusinessPromptException("不存在部门信息");
            var model = _mapper.Map<CustomerInfo>(data);

            if (string.IsNullOrWhiteSpace(model.ContactName) || string.IsNullOrWhiteSpace(model.CustomerName))
            {
                throw new BusinessPromptException("联系人和客户名称不能为空!");
            }

            if (model.CustomerId == 0)
            {
                #region 先判断这个客户是什么类型,再判断数据可是否存在该客户

                Expression<Func<CustomerInfo, bool>> predicate =
                    m => m.CustomerName == data.CustomerName && !m.IsDeleted;
                var oldcustomerInfo = _customerInfoRepository.FindAll(predicate);
                if (oldcustomerInfo.Count > 0)
                {
                    if (data.CompanyType == CompanyType.Supplier &&
                        oldcustomerInfo.Count(m => m.CompanyType == CompanyType.Supplier) > 0)
                    {
                        throw new BusinessPromptException("该企业名称已存在，不能再添加！");
                    }
                    if (data.CompanyType == CompanyType.Institute &&
                    oldcustomerInfo.Count(m => m.CompanyType == CompanyType.Institute) > 0)
                    {
                        throw new BusinessPromptException("该机构名称已存在，不能再添加！");
                    }
                }

                #endregion

                model.CustomerCode = ConfigApiService.GetSeedCodeByName("Customer");

                //客服经理
                var customerManager = new CustomerManager
                {
                    CustomerManagerId = operatorId,
                    CustomerManagerName = operatorName,
                    OrganizationId = userOrgs.FirstOrDefault().Id,
                    OrganizationName = userOrgs.FirstOrDefault().Text
                };
                model.CustomerManagers.Add(customerManager);

                //添加默认的联系人
                if (!string.IsNullOrWhiteSpace(model.ContactName))
                {
                    var customerContact = new CustomerContact
                    {
                        ContactName = model.ContactName,
                        //PositionName = model.PositionName,
                        //ContactEmail = model.ContactEmail,
                        //ContactPhoneNumber = model.ContactPhoneNumber,
                        //ContactTelephone = model.ContactTelephone,
                        //ContactQQ = model.ContactQQ,
                        IsDefault = true
                    };
                    model.CustomerContacts.Add(customerContact);
                }
                var success = AddCustomerInfo(model);
                return new Tuple<int, string>(model.CustomerId, success ? string.Empty : "添加客户信息失败！");
            }
            else
            {
                var oldModel = _customerInfoRepository.GetById(model.CustomerId);
                if (operatorName != oldModel.CreatedBy)
                    throw new BusinessPromptException("不能编辑其他人创建的客户！");

                var newModel = model;
                var bl = false;
                if (oldModel.IsDeleted)
                {
                    return new Tuple<int, string>(model.CustomerId, bl ? string.Empty : "该客户已经被删除！");
                }

                #region 先判断这个客户是什么类型,再判断数据可是否存在该客户

                Expression<Func<CustomerInfo, bool>> predicate =
                    m => m.CustomerName == data.CustomerName && !m.IsDeleted && m.CustomerId != model.CustomerId;
                var oldcustomerInfo = _customerInfoRepository.FindAll(predicate);
                if (oldcustomerInfo.Count > 0)
                {
                    if (data.CompanyTypeStr == "企业" &&
                        oldcustomerInfo.Count(m => m.CompanyType == CompanyType.Supplier) > 0)
                    {
                        throw new BusinessPromptException("该企业名称已存在，不能再添加！");
                    }
                }

                #endregion

                bl = _customerInfoRepository.Modify(model);
                if (bl)
                {
                    //修改默认联系人，没有就新增

                    var list =
                        GetCustomerContactsByCustomerId(model.CustomerId, false).SingleOrDefault(m => m.IsDefault == true);
                    var customerContact = new CustomerContact
                    {
                        CustomerId = model.CustomerId,
                        ContactName = model.ContactName,
                        //PositionName = model.PositionName,
                        //ContactEmail = model.ContactEmail,
                        //ContactPhoneNumber = model.ContactPhoneNumber,
                        //ContactTelephone = model.ContactTelephone,
                        //ContactQQ = model.ContactQQ,
                        IsDefault = true
                    };
                    if (list != null && list.Id > 0)
                    {
                        customerContact.Id = list.Id;
                        _customerContactRepository.ModifyAsync(customerContact, null);
                    }
                    else
                    {
                        _customerContactRepository.Add(customerContact);
                    }

                    if (oldModel != newModel)
                    {
                        var attributes = new[]
                        {
                            "CompanyType", "ContactEmail", "ContactName", "ContactPhoneNumber", "ContactTelephone",
                            "ContactQQ", "CustomerName", "RecommandedUserName"
                            , "ClientType", "CustomerLevel", "CustomerSource", "PositionName", "ProviceName", "CityName"
                        };
                        var attributesName = new[]
                        {
                            "客户类型", "联系人邮箱", "联系人", "联系人手机", "联系人座机", "QQ", "客户名称", "推荐人"
                            , "客户状态", "客户等级", "录入方式", "职务", "省份", "市名"
                        };

                        var list1 = GenerateChangeLogs<CustomerInfo>(attributes, newModel, oldModel, operatorName,
                            operatorId, oldModel.CustomerId,
                            oldModel.CustomerName, attributesName);
                        _customerChangeLogRepository.Add(list1);
                    }
                }
                return new Tuple<int, string>(model.CustomerId, bl ? string.Empty : "编辑客户信息失败！");
            }
        }

        private bool AddCustomerInfo(CustomerInfo model, bool isSave = true)
        {
            _customerInfoRepository.Add(model, false);
            var cusExtInfoProviders = _customerExtInfoProviderRepository.FindAll();
            var cusExtInfos = _mapper.Map<List<CustomerExtInfo>>(cusExtInfoProviders);
            cusExtInfos.ForEach(m =>
            {
                //m.CustomerId = model.CustomerId;
                m.CustomerInfo = model;
            });
            _customerExtInfoRepository.Add(cusExtInfos, false);

            return isSave && CustomerUnitOfWorkContext.Commit() > 0;
        }

        private List<CustomerChangeLog> GenerateChangeLogs<T>(string[] attributes, T newData, T oldData, string userName,
            string userId, int customerId, string customerName, string[] attributesName)
        {
            var logs = new List<CustomerChangeLog>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0, len = attributes.Count(); i < len; i++)
            {
                var attribute =
                    properties.FirstOrDefault(m => m.Name.Equals(attributes[i], StringComparison.OrdinalIgnoreCase));
                if (null == attribute)
                    continue;
                var baseType = attribute.PropertyType.BaseType;
                var newValue = attribute.GetValue(newData);
                var oldValue = attribute.GetValue(oldData);

                var newValueStr = null == newValue ? string.Empty : newValue.ToString();
                var oldValueStr = null == oldValue ? string.Empty : oldValue.ToString();
                if (null != baseType && baseType.Name.Equals("Enum"))
                {
                    switch (attribute.PropertyType.Name)
                    {
                        case "ClientType":
                            var temp = (ClientType)newValue;
                            newValueStr = temp.ToDescription();
                            temp = (ClientType)oldValue;
                            oldValueStr = temp.ToDescription();
                            break;
                        case "ClientSource":
                            var temp2 = (CustomerSource)newValue;
                            newValueStr = temp2.ToDescription();
                            temp2 = (CustomerSource)oldValue;
                            oldValueStr = temp2.ToDescription();
                            break;
                        case "CompanyType":
                            var temp3 = (CompanyType)newValue;
                            newValueStr = temp3.ToDescription();
                            temp3 = (CompanyType)oldValue;
                            oldValueStr = temp3.ToDescription();
                            break;

                    }
                }

                if (!newValueStr.Equals(oldValueStr, StringComparison.OrdinalIgnoreCase))
                {
                    //if (attribute.IsDefined(typeof (DisplayAttribute), false))
                    //{
                    //    var displayAttr = attribute.GetCustomAttributes(typeof (DisplayAttribute)).FirstOrDefault();

                    //}
                    var log = new CustomerChangeLog();
                    log.NewValue = newValueStr;
                    log.OldValue = oldValueStr;
                    log.Operator = userName;
                    log.OperatorId = userId;
                    log.AttributeName = attributesName[i];
                    log.CustomerId = customerId;
                    log.CustomerName = customerName;
                    log.OperateDate = DateTime.Now;
                    logs.Add(log);
                }
            }
            return logs;
        }

        public bool ImportCustomersFromOtherTenant(List<CustomerInfoDTO> data)
        {
            if (!data.Any())
                return true;

            var count = data.Count;
            var seed = ConfigApiService.GetSeedEntityByName("Customer", count);
            var min = seed.SeedMin;
            var codePrefix = seed.SeedValue.Substring(0, seed.SeedValue.Length - 5);
            for (var i = 0; i < count; i++)
            {
                data[i].CustomerCode = codePrefix + (min + i).ToString().PadLeft(5, '0');
                data[i].CustomerId = 0;
                data[i].ClientType = ClientType.Potential;
                data[i].CustomerSource = CustomerSource.ThirdParty;
            }

            var newCustomers = _mapper.Map<List<CustomerInfo>>(data);
            if (!newCustomers.Any())
                return true;

            //先判断这个客户是什么类型,再判断数据可是否存在该客户
            Expression<Func<CustomerInfo, bool>> predicate = m => !m.IsDeleted;
            var dbCustomers = _customerInfoRepository.FindAll(predicate);
            CustomerInfo dbCustomer = new CustomerInfo();
            foreach (var customerInfo in newCustomers.ToList())
            {
                if (customerInfo.CompanyType.ToString() == "Personal" ||
                    customerInfo.CompanyType.ToString() == "Internal")
                {
                    dbCustomer =
                        dbCustomers.FirstOrDefault(
                            m =>
                                m.CustomerName != null &&
                                m.CustomerName.Equals(customerInfo.CustomerName, StringComparison.OrdinalIgnoreCase) );
                }
                else
                {
                    dbCustomer =
                        dbCustomers.FirstOrDefault(
                            m =>
                                m.CustomerName != null &&
                                m.CustomerName.Equals(customerInfo.CustomerName, StringComparison.OrdinalIgnoreCase));
                }
                if (dbCustomer == null)
                {
                    customerInfo.CustomerExtInfos.ForEach(m =>
                    {
                        m.CustomerId = 0;
                        m.CustomerInfo = customerInfo;
                        m.CustomerExtInfoProviderId = null;
                        m.CustomerExtInfoProvider = null;
                    });

                    AddCustomerInfo(customerInfo, false);
                }
            }
            CustomerUnitOfWorkContext.Commit();
            return true;
        }

        public bool RemoveCustomerInfoByIds(List<int> ids, string currentUserName)
        {
            var customers = _customerInfoRepository.GetCustomerInfosByIds(ids);
            if (customers != null)
            {
                var customersCreatedBy = customers.Select(o => o.CreatedBy);
                if (customersCreatedBy.Any(o => o != currentUserName))
                {
                    throw new BusinessPromptException("不能删除其他人创建的客户！");
                }
            }
            Expression<Func<CustomerTracingLog, bool>> predicate = m => true;
            predicate = predicate.And(m => ids.Contains(m.CustomerId.Value));
            var customerTracingLogdata = _customerTracingLogRepository.FindAll(predicate);
            for (int i = 0; i < customerTracingLogdata.Count; i++)
            {
                if (ids.Contains(customerTracingLogdata[i].CustomerId.Value))
                {
                    ids.Remove(customerTracingLogdata[i].CustomerId.Value);
                }
            }
            Expression<Func<CustomerChangeLog, bool>> newpredicate = m => true;
            newpredicate = newpredicate.And(m => ids.Contains(m.CustomerId));
            var customerChangeLogLogdata = _customerChangeLogRepository.FindAll(newpredicate);
            for (int i = 0; i < customerChangeLogLogdata.Count; i++)
            {
                if (ids.Contains(customerChangeLogLogdata[i].CustomerId))
                {
                    ids.Remove(customerChangeLogLogdata[i].CustomerId);
                }
            }
            Expression<Func<CustomerInfo, bool>> predicate1 = m => ids.Contains(m.CustomerId);
            return _customerInfoRepository.SoftRemove(predicate1) > 0;
        }

        /// <summary>
        /// 推送客户给其他租户
        /// </summary>
        /// <param name="customerIds"></param>
        /// <param name="tenantNames"></param>
        /// <param name="opertorId"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        public string SendCustomersToTenant(List<int> customerIds, List<string> tenantNames, string opertorId,
            string operatorName)
        {
            var customers = GetCustomerInfosByIds(customerIds);
            if (!customers.Any())
                return "可推送的客户列表为空！";

            var success = true;
            var message = new StringBuilder();
            var logs = new List<CustomerSendToTenantLog>();
            var tenants = TenantUserApiService.GetTenantByTenantNames(tenantNames).Result;
            foreach (var tenantName in tenantNames)
            {
                var tenant = tenants.FirstOrDefault(m => m.TenantName.Equals(tenantName, StringComparison.OrdinalIgnoreCase));
                if (tenant == null)
                    continue;

                var existsIds = GetSendedCustomerIds(tenantName);
                var sendCustomers = existsIds != null && existsIds.Any()
                    ? customers.Where(m => !existsIds.Contains(m.CustomerId)).ToList()
                    : customers;

                if (!sendCustomers.Any())
                    continue;

                var customerIdList = sendCustomers.Select(m => m.CustomerId).ToList();
                var customerNameList = sendCustomers.Select(m => m.CustomerName).ToList();

                sendCustomers.ForEach(m =>
                {
                    m.CustomerId = 0;
                    m.ClientType = ClientType.Potential;
                    m.CustomerSource = CustomerSource.ThirdParty;
                    m.ReferenceId = m.CustomerCode;
                    m.RecommandedUserId = base.Tenant.TenantName;
                    m.RecommandedUserName = base.Tenant.TenantDisplayName;
                });

                //调用每个Tenant的Api接口，接收所传输的Customer数据：tenantName.api.cfwin.com/Customer/ImportCustomersFromOtherTenant
                var result = _customerApiService.SendCustomersToTenant(sendCustomers);
                if (!result.success || !result.Result)
                {
                    success = false;
                    message.AppendLine("错误消息：" + result.message + "; 详细信息：" + result.LogMessage);
                }

                logs.Add(new CustomerSendToTenantLog()
                {
                    TenantName = tenant.TenantName,
                    TenantDisplayName = tenant.TenantDisplayName,
                    CustomerIdList = string.Join(", ", customerIdList),
                    OperateDate = DateTime.UtcNow,
                    Operator = operatorName,
                    OperatorId = opertorId,
                    Type = result.success ? ProcessLogType.Success : ProcessLogType.Failure,
                    Remark = string.Format("员工（{0}）于{1}推送客户（{2}）给企业（{3}）--{4}。",
                        operatorName, DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                        string.Join(", ", customerNameList), tenant.TenantDisplayName,
                        result.success ? "成功" : "失败")
                });
            }

            if (logs.Any())
            {
                _customerSendToTenantLogRepository.Add(logs, false);
                CustomerUnitOfWorkContext.Commit();
            }

            if (!success)
                return message.ToString();

            return string.Empty;
        }

        public bool EditCustomer(string id, string introduction)
        {
            Expression<Func<CustomerInfo, bool>> predicate = m => m.CustomerId.ToString() == id && !m.IsDeleted;
            var cusID = _customerInfoRepository.FindAll(predicate);
            if (cusID.Count > 0)
            {
                cusID[0].Introduction = introduction;
                return _customerInfoRepository.Modify(cusID[0]);
            }
            else
            {
                cusID[0] = new CustomerInfo()
                {
                    Introduction = introduction
                };
                return _customerInfoRepository.Add(cusID[0]);
            }
        }

        public bool ExistPersonalCustomerPhoneNumber(int customerId, string phoneNumber)
        {
            var data = _customerInfoRepository.FindAll(
                o =>
                    ((customerId == 0) ||
                      o.CustomerId != customerId) &&!o.IsDeleted);
            return data.Count > 0;
        }

        public bool ExistCompanyCustomerName(string customerName)
        {
            var data = _customerInfoRepository.FindAll(
                o =>
                    o.CustomerName == customerName &&
                    (o.CompanyType == CompanyType.Institute || o.CompanyType == CompanyType.Supplier) &&
                    !o.IsDeleted);
            return data.Count > 0;
        }

        #endregion

        #region Customer Contact Info

        public List<CustomerContactDTO> GetAllCustomerContacts()
        {
            var data = _customerContactRepository.FindAll(m => m.CreatedDate);
            return _mapper.Map<List<CustomerContactDTO>>(data);
        }

        public List<CustomerContactDTO> GetCustomerContactsByCustomerId(int customerId, bool isDelete = true)
        {
            var data = _customerContactRepository.FindAll(m => m.CustomerId == customerId && m.IsDeleted == isDelete);
            return _mapper.Map<List<CustomerContactDTO>>(data);
        }

        public CustomerContactDTO GetCustomerContactsById(int id)
        {
            var data = _customerContactRepository.GetById(id);
            return _mapper.Map<CustomerContactDTO>(data);
        }

        public bool CreateCustomerContact(CustomerContactDTO model)
        {
            var md = _mapper.Map<CustomerContact>(model);
            ProcessDefaultCustomer(model);
            var bl = _customerContactRepository.Add(md);
            return bl;
        }

        public bool EditCustomerContact(CustomerContactDTO model)
        {
            var md = _mapper.Map<CustomerContact>(model);
            ProcessDefaultCustomer(model);
            var bl = _customerContactRepository.Modify(md, null);
            return bl;
        }

        private void ProcessDefaultCustomer(CustomerContactDTO model)
        {
            var customer = _customerInfoRepository.GetById(model.CustomerId);
            var customerContract = GetCustomerContactsByCustomerId(model.CustomerId, false);
            string messagePrefix = "个人客户类型";
            if (customer.CompanyType == CompanyType.Institute)
                messagePrefix = "机构客户类型";
            if (customer.CompanyType == CompanyType.Supplier)
                messagePrefix = "企业客户类型";

            if (model.Id == 0 &&
                customerContract.Any(
                    o => o.ContactName == model.ContactName && o.ContactPhoneMumber == model.ContactPhoneMumber))
            {
                throw new BusinessPromptException(messagePrefix + "已存在该联系人！");
            }
            if (customer.CompanyType == CompanyType.Customer)
            {
                if (string.IsNullOrEmpty(model.ContactPhoneMumber))
                {
                    throw new BusinessPromptException(messagePrefix + "联系人手机必填！");
                }
                if (model.Id == 0)
                {
                    if (customerContract.All(o => o.ContactName != model.ContactName))
                    {
                        throw new BusinessPromptException(messagePrefix + "最多只能添加一个联系人！");
                    }
                }
            }
            if (model.IsDefault)
            {
                var mapList = _mapper.Map<List<CustomerContact>>(customerContract);
                foreach (var customerContact in mapList)
                {
                    customerContact.IsDefault = false;
                    if (customer.CompanyType == CompanyType.Customer)
                    {
                        customerContact.ContactName = model.ContactName;
                    }
                    _customerContactRepository.ModifyAsync(customerContact, null);
                }
                //修改主表联系人数据
                if (customer.CompanyType == CompanyType.Customer)
                {
                    customer.CustomerName = model.ContactName;
                }

                customer.ContactName = model.ContactName;
                //customer.ContactEmail = model.ContactEmail;
                //customer.ContactPhoneNumber = model.ContactPhoneMumber;
                //customer.ContactQQ = model.ContactQQ;
                //customer.ContactTelephone = model.ContactTelephone;
                //customer.PositionName = model.PositionName;

                _customerInfoRepository.Modify(customer);
            }
        }

        public bool RemoveCustomerContact(int id)
        {
            var bl = _customerContactRepository.RemoveById(id);
            return bl;
        }

        public bool RemoveCustomerContactSoft(int id)
        {
            var model = this.GetCustomerContactsById(id);
            model.IsDeleted = true;
            var md = _mapper.Map<CustomerContact>(model);
            var bl = _customerContactRepository.Modify(md, null);
            return bl;
        }

        public PaginatedBaseDTO<CustomerContactDTO> GetPaginatedCustomerContactsByCustomerId(int pageIndex, int pageSize,
            string searchKey, string searchValue, int id)
        {
            Expression<Func<CustomerContact, bool>> predicate = m => true && !m.IsDeleted;
            if (id != 0)
            {
                predicate = predicate.And(m => m.CustomerId.Equals(id));
            }

            if (!string.IsNullOrWhiteSpace(searchKey)
                && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey)
                {
                    case "ContactName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.ContactName.Contains(searchValue));
                        }
                        break;
                    case "ContactPhoneMumber":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.ContactPhoneNumber.Contains(searchValue));
                        }
                        break;
                    case "ContactQQ":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.ContactQQ.Contains(searchValue));
                        }
                        break;
                }
            }
            var data = _customerContactRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.CreatedDate,
                false);
            var total = data.Item1;
            var rows = _mapper.Map<List<CustomerContactDTO>>(data.Item2);
            return new PaginatedBaseDTO<CustomerContactDTO>(pageIndex, pageSize, total, rows);
        }

        public PaginatedBaseDTO<CustomerContactInfoDTO> GetPaginatedCustomerContactInfos(int pageIndex, int pageSize,
            string currentUserId,List<string> roleList, CompanyType? companyType, string viewName, string searchKey, string searchValue)
        {
            var customerIds = GetCurrentUserCanOperateCustomerIds(currentUserId,roleList);
            Expression<Func<CustomerContact, bool>> predicate = m => customerIds.Contains(m.CustomerId);
            if (companyType.HasValue)
            {
                predicate = predicate.And(o => o.CustomerInfo.CompanyType == companyType);
            }
            if (viewName == "Email")
            {
                predicate = predicate.And(o => !string.IsNullOrEmpty(o.ContactEmail));
            }
            if (viewName == "Sms")
            {
                predicate = predicate.And(o => !string.IsNullOrEmpty(o.ContactPhoneNumber));
            }
            if (!string.IsNullOrWhiteSpace(searchKey) && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey)
                {
                    case "ContactName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.ContactName.Contains(searchValue));
                        }
                        break;
                    case "ContactPhoneNumber":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.ContactPhoneNumber.Contains(searchValue));
                        }
                        break;
                    case "ContactEmail":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.ContactEmail.Contains(searchValue));
                        }
                        break;
                    case "AffiliatedCompany":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate =
                                predicate.And(
                                    m =>
                                        (m.CustomerInfo.CustomerName.Contains(searchValue) &&
                                         (m.CustomerInfo.CompanyType == CompanyType.Supplier ||
                                          m.CustomerInfo.CompanyType == CompanyType.Institute)) ||
                                        ( m.CustomerInfo.CompanyType == CompanyType.Customer));
                        }
                        break;
                    case "PositionName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.PositionName.Contains(searchValue));
                        }
                        break;
                    case "CustomerManagerName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate =
                                predicate.And(
                                    m => m.CustomerInfo.CustomerManagers.Any(o => o.CustomerManagerName.Contains(searchValue)));
                        }
                        break;
                    case "OrganizationName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate =
                                predicate.And(m => m.CustomerInfo.CustomerManagers.Any(o => o.OrganizationName.Contains(searchValue)));
                        }
                        break;
                }
            }
            var data = _customerContactRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate);
            var total = data.Item1;
            var rows = _mapper.Map<List<CustomerContactInfoDTO>>(data.Item2);
            return new PaginatedBaseDTO<CustomerContactInfoDTO>(pageIndex, pageSize, total, rows);
        }

        #endregion

        #region Customer Change Log

        public List<CustomerChangeLogDTO> GetAllCustomerChangeLogs()
        {
            var data = _customerChangeLogRepository.FindAll(m => m.CustomerName);
            return _mapper.Map<List<CustomerChangeLogDTO>>(data);
        }

        public PaginatedBaseDTO<CustomerChangeLogDTO> GetCustomerChangeLogsByCustomerInfoId(int pageIndex, int pageSize,
            string searchKey, string searchValue, string startTime, string endTime, int customerId)
        {
            Expression<Func<CustomerChangeLog, bool>> predicate = m => m.CustomerId == customerId;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            DateTime.TryParse(startTime, out startDate);
            DateTime.TryParse(endTime, out endDate);

            if (startDate != DateTime.MinValue)
            {
                predicate = predicate.And(m => m.OperateDate >= startDate);
            }
            if (endDate != DateTime.MinValue)
            {
                endDate = endDate.AddDays(1);
                predicate = predicate.And(m => m.OperateDate <= endDate);
            }
            if (!string.IsNullOrWhiteSpace(searchKey)
                && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey)
                {
                    case "AttributeName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.AttributeName.Contains(searchValue));
                        }
                        break;
                    case "Operator":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.Operator.Contains(searchValue));
                        }
                        break;
                }
            }
            var data = _customerChangeLogRepository.FindPagenatedListWithCount(pageIndex,
                pageSize,
                predicate,
                m => m.OperateDate,
                false);
            return new PaginatedBaseDTO<CustomerChangeLogDTO>(pageIndex, pageSize, data.Item1,
                _mapper.Map<List<CustomerChangeLogDTO>>(data.Item2));
        }

        public CustomerChangeLogDTO GetCustomerChangeLogDTO(int id)
        {
            var data = _customerChangeLogRepository.GetById(id);
            return _mapper.Map<CustomerChangeLogDTO>(data);
        }

        public bool EditCustomerChangeLogDTO(CustomerChangeLogDTO model)
        {
            var customerChangeLog = _mapper.Map<CustomerChangeLog>(model);
            return _customerChangeLogRepository.Modify(customerChangeLog, null);
        }

        public bool AddCustomerChangeLogDTO(CustomerChangeLogDTO model)
        {
            var customerChangeLog = _mapper.Map<CustomerChangeLog>(model);
            return _customerChangeLogRepository.Add(customerChangeLog);
        }

        public bool RemoveCustomerChangeLogDTO(int id)
        {
            return _customerChangeLogRepository.RemoveById(id);
        }

        #endregion

        #region Customer Tracing Info

        public List<CustomerTracingLogDTO> GetAllCustomerTracingLogs()
        {
            var data = _customerTracingLogRepository.FindAll(m => m.CustomerName);
            return _mapper.Map<List<CustomerTracingLogDTO>>(data);
        }

        public PaginatedBaseDTO<CustomerTracingLogDTO> GetActivityLogListByCustomerInfoId(int id, int pageIndex,
            int pageSize, string customerName, string activityName, string operatoruser, string startTime,
            string endTime, TracingType? type)
        {
            Expression<Func<CustomerTracingLog, bool>> predicate = m => true;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            DateTime.TryParse(startTime, out startDate);
            DateTime.TryParse(endTime, out endDate);

            if (startDate != DateTime.MinValue)
            {
                predicate = predicate.And(m => m.OperateDate >= startDate);
            }
            if (endDate != DateTime.MinValue)
            {
                endDate = endDate.AddDays(1);
                predicate = predicate.And(m => m.OperateDate <= endDate);
            }
            if (!string.IsNullOrEmpty(customerName))
            {
                predicate = predicate.And(m => m.CustomerName.Contains(customerName));
            }
            if (!string.IsNullOrEmpty(activityName))
            {
                predicate = predicate.And(m => m.ActivityName.Contains(activityName));
            }
            if (!string.IsNullOrEmpty(operatoruser))
            {
                predicate = predicate.And(m => m.Operator.Contains(operatoruser));
            }
            if (type != null)
            {
                predicate = predicate.And(m => m.TracingType == type);
            }

            predicate = predicate.And(m => m.CustomerId == id);
            var data = _customerTracingLogRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.OperateDate,
                false);

            return new PaginatedBaseDTO<CustomerTracingLogDTO>(pageIndex, pageSize, data.Item1,
                _mapper.Map<List<CustomerTracingLogDTO>>(data.Item2));
        }

        /// <summary>
        /// 查询客户跟踪记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="currentUserId">当前操作人</param>
        /// <param name="roleList">当前操作人权限集合</param>
        /// <param name="currentContactId">当前联系人Id</param>
        /// <param name="companyType">客户类型</param>
        /// <param name="searchKey">searchKey</param>
        /// <param name="searchValue">searchValue</param>
        /// <param name="startTime">开始日期</param>
        /// <param name="endTime">结束日期</param>
        /// <param name="type">跟踪类型</param>
        /// <param name="processLogType">跟踪结果（成功或失败）</param>
        /// <param name="callState">通话状态</param>
        /// <returns></returns>
        public PaginatedBaseDTO<CustomerTracingLogDTO> GetCustomerTraceInfoReport(int pageIndex, int pageSize, string currentUserId, List<string> roleList, int? currentContactId,
            string searchKey, string searchValue, string startTime, string endTime, CompanyType? companyType, TracingType? type,
            ProcessLogType? processLogType, CallState? callState)
        {
            var customerIds = GetCurrentUserCanOperateCustomerIds(currentUserId,roleList).Select(o => (int?) o);
            Expression<Func<CustomerTracingLog, bool>> predicate = m => customerIds.Contains(m.CustomerId);
            DateTime startDate;
            DateTime endDate;
            DateTime.TryParse(startTime, out startDate);
            DateTime.TryParse(endTime, out endDate);

            if (!string.IsNullOrWhiteSpace(searchKey) && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey)
                {
                    case "customerName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate =
                                predicate.And(
                                    m =>
                                        m.CustomerName.Contains(searchValue) ||
                                        ( m.CustomerInfo.CompanyType == CompanyType.Customer));
                        }
                        break;
                    case "contactName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.ContactName.Contains(searchValue));
                        }
                        break;
                    case "customerManagerName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate =
                                predicate.And(
                                    m =>
                                        m.CustomerInfo.CustomerManagers.Any(
                                            n => n.CustomerManagerName.Contains(searchValue)));
                        }
                        break;

                    case "organizationName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate =
                                predicate.And(
                                    m =>
                                        m.CustomerInfo.CustomerManagers.Any(
                                            n => n.OrganizationName.Contains(searchValue)));
                        }
                        break;
                    case "title":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.Title.Contains(searchValue));
                        }
                        break;
                    case "content":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.Remark.Contains(searchValue));

                        }
                        break;
                    case "callerPhone":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.CallerPhone.Contains(searchValue));
                        }
                        break;
                    case "calleePhone":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.CalleePhone.Contains(searchValue));
                        }
                        break;
                }
            }
            if (currentContactId.HasValue)
            {
                if (currentContactId.Value != 0)
                    predicate = predicate.And(m => m.CustomerContactId == currentContactId.Value);
            }
            if (startDate != DateTime.MinValue)
            {
                predicate = predicate.And(m => m.OperateDate >= startDate);
            }
            if (endDate != DateTime.MinValue)
            {
                endDate = endDate.AddDays(1);
                predicate = predicate.And(m => m.OperateDate <= endDate);
            }
            if (companyType.HasValue)
            {
                predicate = predicate.And(m => m.CustomerInfo.CompanyType == companyType);

            }
            if (type.HasValue)
            {
                if (type == TracingType.EmailNotify)
                    predicate =
                        predicate.And(
                            m =>
                                m.TracingType == TracingType.EmailNotify || m.TracingType == TracingType.MassEmailNotify);
                else if (type == TracingType.SmsNotify)
                    predicate =
                        predicate.And(
                            m => m.TracingType == TracingType.SmsNotify || m.TracingType == TracingType.MassSmsNotify);
                else
                    predicate = predicate.And(m => m.TracingType == type);
            }
            if (processLogType.HasValue)
            {
                predicate = predicate.And(m => m.Type == processLogType);
            }
            if (callState.HasValue)
            {
                predicate = predicate.And(m => m.CallState == callState);
            }

            var data = _customerTracingLogRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.OperateDate,
                false);
            List<CustomerTracingLogDTO> rows = _mapper.Map<List<CustomerTracingLogDTO>>(data.Item2);
            rows.ForEach(
                o =>
                {
                    if (o.CustomerId.HasValue)
                    {
                        var cst = _customerInfoRepository.GetByFilter(p => p.CustomerId == o.CustomerId);
                        o.CompanyTypeName = cst.CompanyType.ToDescription();
                        o.CustomerName = o.CustomerName;
                    }
                });
            return new PaginatedBaseDTO<CustomerTracingLogDTO>(pageIndex, pageSize, data.Item1, rows);
        }

        public CustomerTracingLogDTO GetActivityLogById(int id)
        {
            var data = _customerTracingLogRepository.GetById(id);
            return _mapper.Map<CustomerTracingLogDTO>(data);
        }

        public bool SaveCustomerTracingLogDTO(CustomerTracingLogDTO model)
        {
            bool bl = false;
            var data = _mapper.Map<CustomerTracingLog>(model);
            if (model.ProcessLogId == 0)
            {
                bl = _customerTracingLogRepository.Add(data);
            }
            else
            {
                bl = _customerTracingLogRepository.Modify(data, null);
            }
            return bl;
        }

        public bool SaveCustomerTracingLogDTO(IList<CustomerTracingLogDTO> model)
        {
            int count = 0;
            IList<CustomerTracingLog> addCustomerTracingLogs = new List<CustomerTracingLog>();
            IList<CustomerTracingLog> modifyCustomerTracingLogs = new List<CustomerTracingLog>();
            var data = _mapper.Map<IList<CustomerTracingLog>>(model);
            foreach (var customerTracingLog in data)
            {
                if (customerTracingLog.ProcessLogId == 0)
                    addCustomerTracingLogs.Add(customerTracingLog);
                else
                    modifyCustomerTracingLogs.Add(customerTracingLog);
            }
            if (addCustomerTracingLogs.Count > 0)
                count = _customerTracingLogRepository.Add(addCustomerTracingLogs);
            if (modifyCustomerTracingLogs.Count > 0)
                count = _customerTracingLogRepository.Modify(modifyCustomerTracingLogs);

            return count > 0;
        }

        public bool RomoveCustomerTracingLogDTO(List<int> ids)
        {
            var data = _customerTracingLogRepository.FindAll(o => ids.Contains(o.ProcessLogId));
            return _customerTracingLogRepository.RemoveById(data.AsEnumerable());
        }

        public List<CustomerContactInfoDTO> GetCustomerContactsByCompanyType(CompanyType companyType)
        {
            var data = _customerContactRepository.GetCustomerContactsByCompanyType(companyType);
            return _mapper.Map<List<CustomerContactInfoDTO>>(data);
        }

        public List<CustomerContactInfoDTO> GetCustomerCompanyByCustomerContact(int customerContactId)
        {
            var data = _customerContactRepository.GetCustomerCompanyByCustomerContact(customerContactId);
            return _mapper.Map<List<CustomerContactInfoDTO>>(data);
        }

        #endregion

        #region Customer SendToTenant Log

        public PaginatedBaseDTO<CustomerSendToTenantLogDTO> GetCustomerSendLogListByFilter(int pageIndex,
            int pageSize, string tenantName, string tenantDisplayName, string operatoruser, string startTime,
            string endTime, ProcessLogType? type)
        {
            Expression<Func<CustomerSendToTenantLog, bool>> predicate = m => true;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            DateTime.TryParse(startTime, out startDate);
            DateTime.TryParse(endTime, out endDate);

            if (startDate != DateTime.MinValue)
            {
                predicate = predicate.And(m => m.OperateDate >= startDate);
            }
            if (endDate != DateTime.MinValue)
            {
                endDate = endDate.AddDays(1);
                predicate = predicate.And(m => m.OperateDate <= endDate);
            }
            if (!string.IsNullOrEmpty(tenantName))
            {
                predicate = predicate.And(m => m.TenantName.Contains(tenantName));
            }
            if (!string.IsNullOrEmpty(tenantDisplayName))
            {
                predicate = predicate.And(m => m.TenantDisplayName.Contains(tenantDisplayName));
            }
            if (!string.IsNullOrEmpty(operatoruser))
            {
                predicate = predicate.And(m => m.Operator.Contains(operatoruser));
            }
            if (type != null)
            {
                predicate = predicate.And(m => m.Type == type);
            }
            var data = _customerSendToTenantLogRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.OperateDate,
                false);

            return new PaginatedBaseDTO<CustomerSendToTenantLogDTO>(pageIndex, pageSize, data.Item1,
                _mapper.Map<List<CustomerSendToTenantLogDTO>>(data.Item2));
        }

        private List<int> GetSendedCustomerIds(string tenantName)
        {
            Expression<Func<CustomerSendToTenantLog, bool>> predicate =
                m => m.TenantName.Equals(tenantName, StringComparison.OrdinalIgnoreCase);
            var data = _customerSendToTenantLogRepository.FindAll(predicate).FirstOrDefault();
            if (data != null)
            {
                var strIds = data.CustomerIdList;
                return strIds.Split(',').Select(m => Convert.ToInt32(m)).ToList();
            }

            return null;
        }

        #endregion

        #region   Customer Extent Info

        /// <summary>
        /// 当前客户其他属性
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public List<CustomerExtInfoDTO> GetCustomerExtInfoDtosByCustomerId(int CustomerId)
        {
            var data = _customerExtInfoRepository.FindAll(m => m.CustomerId == CustomerId & !m.IsDeleted);
            return _mapper.Map<List<CustomerExtInfoDTO>>(data);
        }

        public CustomerExtInfoDTO GetCustomerExtInfoDtoByPropertyAttributeId(int propertyAttributeId)
        {
            var data =
                _customerExtInfoRepository.FindAll(m => m.PropertyAttributeId == propertyAttributeId).FirstOrDefault();
            return _mapper.Map<CustomerExtInfoDTO>(data);
        }

        public PaginatedBaseDTO<CustomerExtInfoDTO> GetPaginatedCustomerExtInfoByCustomerId(int pageIndex, int pageSize,
            int customerId)
        {
            Expression<Func<CustomerExtInfo, bool>> predicate = m => true && !m.IsDeleted;
            //检查库中是否已有相关会员的扩展信息
            //如有则直接查询

            #region if (CheckRecordByCustomerId(customerId))

            if (CheckRecordByCustomerId(customerId))
            {
                if (customerId != 0)
                {
                    predicate = predicate.And(m => m.CustomerId == customerId);
                }
            }
            #endregion

            //如无则从provider中提取相关扩展信息附加到扩展信息中
            #region else

            else
            {
                var extInfoList = new List<CustomerExtInfoDTO>();
                var providerList = _customerExtInfoProviderRepository.FindAll(m => m.IsRequire && !m.IsDeleted);
                foreach (var customerExtInfoProviderDto in providerList)
                {
                    var customerExtInfoDto = new CustomerExtInfoDTO()
                    {
                        CustomerId = customerId,
                        CustomerExtInfoProviderId = customerExtInfoProviderDto.PropertyAttributeId,
                        DataType = customerExtInfoProviderDto.DataType,
                        Name = customerExtInfoProviderDto.Name,
                        Value = customerExtInfoProviderDto.Value,
                        CanEdit = true,
                        IsRequire = true
                    };
                    extInfoList.Add(customerExtInfoDto);
                }
                this.SaveCustomerExtInfoDtos(extInfoList);
                predicate = predicate.And(m => m.CustomerId == customerId);
            }

            #endregion

            var data = _customerExtInfoRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.PropertyAttributeId);
            var total = data.Item1;
            var rows = _mapper.Map<List<CustomerExtInfoDTO>>(data.Item2);
            return new PaginatedBaseDTO<CustomerExtInfoDTO>(pageIndex, pageSize, total, rows);
        }

        private bool CheckRecordByCustomerId(int customerId)
        {
            var recordList = this.GetCustomerExtInfoDtosByCustomerId(customerId);
            if (recordList.Count == 0)
            {
                return false;
            }
            return true;
        }

        public bool RomoveCustomerExtInfoDto(int id)
        {
            return _customerExtInfoRepository.RemoveById(id);
        }

        public bool RomoveCustomerExtInfoDtoSoft(int id)
        {
            var data = this.GetCustomerExtInfoDtoByPropertyAttributeId(id);
            data.IsDeleted = true;
            var rec = _mapper.Map<CustomerExtInfo>(data);
            return _customerExtInfoRepository.Modify(rec, true);
        }

        public bool SaveCustomerExtInfoDtos(List<CustomerExtInfoDTO> list)
        {
            if (!list.Any())
            {
                return false;
            }
            var addList = new List<CustomerExtInfoDTO>();
            var updateList = new List<CustomerExtInfoDTO>();
            foreach (var customerExtInfoDto in list)
            {
                if (customerExtInfoDto.PropertyAttributeId > 0)
                {
                    updateList.Add(customerExtInfoDto);
                }
                else
                {
                    addList.Add(customerExtInfoDto);
                }
            }
            int i = 0;
            if (addList.Any())
            {
                i += CreateCustomerExtInfo(addList);
            }
            if (updateList.Any())
            {
                i += ModifyCustomerExtInfo(updateList);
            }
            return i > 0;
        }

        public int CreateCustomerExtInfo(List<CustomerExtInfoDTO> list)
        {
            var data = _mapper.Map<List<CustomerExtInfo>>(list);
            return _customerExtInfoRepository.Add(data);
        }

        public int ModifyCustomerExtInfo(List<CustomerExtInfoDTO> list)
        {
            var data = _mapper.Map<List<CustomerExtInfo>>(list);
            return _customerExtInfoRepository.Modify(data);
        }

        #endregion

        #region DownLoad Customer Excel && Import Customer data from Excel

        public byte[] GetCustomerTemplate(string templateUrl)
        {
            var bytes = File.ReadAllBytes(templateUrl);

            return bytes;
        }

        public List<CustomerInfoDTO> GetExcelCustomerData(byte[] excelData)
        {
            var result = new List<CustomerInfoDTO>();

            #region 获取Excel模板数据

            using (var er = new NPOIExcelReader(excelData))
            {
                var rows = er.GetWorksheetRowListData("Sheet1");

                var count = rows.Count - 1;
                var seed = ConfigApiService.GetSeedEntityByName("Customer", count);
                var min = seed.SeedMin;
                var codePrefix = seed.SeedValue.Substring(0, seed.SeedValue.Length - 5);
                for (var i = 1; i < rows.Count; i++)
                {
                    var cusType = 0;
                    var companyType = CompanyType.Institute;
                    if (int.TryParse(rows[i][2].CellValue, out cusType))
                    {
                        companyType = (CompanyType)cusType;
                    }

                    var customer = new CustomerInfoDTO()
                    {
                        ReferenceId = rows[i][0].CellValue,
                        CustomerCode = codePrefix + (min + i - 1).ToString().PadLeft(5, '0'),
                        CustomerName = rows[i][1].CellValue,
                        CompanyType = companyType,
                        ClientType = ClientType.Potential,
                        CustomerSource = CustomerSource.Import,
                        ContactName = rows[i][3].CellValue,
                        CustomerContacts = new List<CustomerContactDTO>()
                        {
                            new CustomerContactDTO()
                            {
                                ContactName = rows[i][3].CellValue,
                                PositionName = rows[i][4].CellValue,
                                ContactEmail = rows[i][5].CellValue,
                                ContactPhoneMumber = rows[i][6].CellValue,
                                ContactTelephone = rows[i][7].CellValue,
                                ContactQQ = rows[i][8].CellValue,
                                IsDefault = true,
                            }
                        }
                    };

                    result.Add(customer);
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// Excel导入客户信息：使用NPOI
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="managerId"></param>
        /// <param name="managerName"></param>
        /// <param name="importToRowIndex"></param>
        /// <param name="businessExceptions"></param>
        /// <returns></returns>
        public bool ImportCustomerDataFromExcel(Stream stream, string managerId, string managerName, ref int importToRowIndex, StringBuilder businessExceptions)
        {
            var rows = new NPOIExcelReader(stream).GetWorksheetRowListData();
            if (rows == null || rows.Count == 1)
                throw new BusinessPromptException("请导入正确格式的excel数据！");

            //var user = _userRepository.FindByIdIncludeOrganizations(managerId);
            var user = AccountApiService.GetUserWithOrgsAndRolesByUserId(managerId).Result;
            if (user == null)
                throw new BusinessPromptException("不存在该用户！");
            if (user.UserOrgIds == null || !user.UserOrgIds.Any())
                throw new BusinessPromptException("不存在部门信息");

            var newCustomers = PrepareImportCustomerData(rows, user, managerId, managerName,ref importToRowIndex,businessExceptions);
            if (!newCustomers.Any())
                return false;

            var dbCustomers = _customerInfoRepository.FindAll(m => !m.IsDeleted); //数据库的客户数据
            CustomerInfo dbCustomer = new CustomerInfo();
            var newCustomercount = 0; //excel的客户数据
            var newCustomer = new List<CustomerInfo>();
            foreach (var customerInfo in newCustomers.ToList())
            {
                //数据库的客户信息
                newCustomer.Add(customerInfo);
                if (customerInfo.CompanyType == CompanyType.Customer)
                {
                    //系统中不存在
                    dbCustomer =
                        dbCustomers.FirstOrDefault(
                            m =>
                                m.CustomerName != null && m.CompanyType == CompanyType.Customer);
                    //排除excel中重复数据
                    newCustomercount =
                        newCustomer.Count(
                            t =>
                                !t.IsDeleted && 
                                t.CompanyType == customerInfo.CompanyType);
                }
                else
                {
                    //系统中不存在
                    dbCustomer =
                        dbCustomers.FirstOrDefault(
                            m =>
                                m.CustomerName != null &&
                                (m.CompanyType == CompanyType.Supplier || m.CompanyType == CompanyType.Institute) &&
                                m.CustomerName.Equals(customerInfo.CustomerName, StringComparison.OrdinalIgnoreCase));
                    //排除excel中重复数据
                    newCustomercount =
                        newCustomer.Count(
                            m =>
                                !m.IsDeleted && m.CompanyType == customerInfo.CompanyType &&
                                m.CustomerName == customerInfo.CustomerName);
                }
                if (dbCustomer == null)
                {
                    if (newCustomercount <= 1)
                    {
                        AddCustomerInfo(customerInfo, false);
                    }
                }
            }
            return CustomerUnitOfWorkContext.Commit() > 0;
        }
        
        /// <summary>
        /// 验证excel导入的客户数据
        /// </summary>
        /// <param name="row"></param>
        /// <param name="rowIndex"></param>
        /// <param name="errorCount"></param>
        /// <param name="businessExceptions"></param>
        /// <returns></returns>
        private bool ValidImportCustomerData(List<RowValue> row, int rowIndex, ref int errorCount,
             StringBuilder businessExceptions)
        {
            bool isPersonalCustomer = row.Any(m => m.ColumnName == "客户类型" & m.CellValue == "1");
            bool isOrgOrCompanyCustomer =
                row.Any(m => m.ColumnName == "客户类型" & (m.CellValue == "0" || m.CellValue == "2"));

            bool valid = true;
            if (row.Count(m => m.ColumnName == "客户名称" & !string.IsNullOrEmpty(m.CellValue)) == 0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的客户名称必填！");
            }
            if (row.Count(m => m.ColumnName == "联系人" & !string.IsNullOrEmpty(m.CellValue)) == 0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的联系人必填！");
            }
            if (
                row.Count(
                    m =>
                        m.ColumnName == "联系人手机" & !string.IsNullOrEmpty(m.CellValue) & !m.CellValue.IsMobile()) > 0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的联系人手机格式有误！");
            }
            if (
                row.Count(
                    m =>
                        m.ColumnName == "联系人手机" & !string.IsNullOrEmpty(m.CellValue) & isPersonalCustomer &
                        ExistPersonalCustomerPhoneNumber(0, m.CellValue)) > 0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行联系人手机在系统个人客户中已存在！");
            }
            if (
               row.Count(
                   m =>
                       m.ColumnName == "客户名称" & !string.IsNullOrEmpty(m.CellValue) & isOrgOrCompanyCustomer &
                       ExistCompanyCustomerName(m.CellValue)) > 0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行客户名称已经存在！");
            }
            if (
                row.Count(
                    m =>
                        m.ColumnName == "身份证" & !string.IsNullOrEmpty(m.CellValue) & !m.CellValue.IsIdCard() &
                        isPersonalCustomer) > 0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的身份证格式有误！");
            }
            if (
                row.Count(
                    m =>
                        (m.ColumnName == "联系人邮箱") & !string.IsNullOrEmpty(m.CellValue) &
                        !StringExtensions.IsEmail(m.CellValue)) >
                0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的联系人邮箱格式有误！");
            }
            if (
                row.Count(
                    m =>
                        (m.ColumnName == "公司邮箱") & !string.IsNullOrEmpty(m.CellValue) &
                        !StringExtensions.IsEmail(m.CellValue) & isOrgOrCompanyCustomer) >
                0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的公司邮箱格式有误！");
            }
            if (
                row.Count(
                    m =>
                        m.ColumnName == "联系人QQ" & !string.IsNullOrEmpty(m.CellValue) & !m.CellValue.IsQQ()) > 0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的联系人QQ格式有误！");
            }
            if (
                row.Count(
                    m =>
                        m.ColumnName == "联系人微信" & !string.IsNullOrEmpty(m.CellValue) & !m.CellValue.EngNum()) > 0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的联系人微信格式有误！");
            }
            if (
                row.Count(
                    m =>
                        m.ColumnName == "联系人座机" & !string.IsNullOrEmpty(m.CellValue) & !m.CellValue.IsTelephone()) >
                0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的联系人座机格式有误！");
            }
            if (
                row.Count(
                    m =>
                        m.ColumnName == "统一社会信用代码" & !string.IsNullOrEmpty(m.CellValue) & !m.CellValue.LenEngNum(18)) >
                0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的统一社会信用代码格式有误！");
            }
            if (
                row.Count(
                    m =>
                        m.ColumnName == "公司电话" & !string.IsNullOrEmpty(m.CellValue) & !m.CellValue.IsTelephone()) >
                0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的公司电话有误！");
            }
            if (
                row.Count(
                    m =>
                        m.ColumnName == "注册资金" & !string.IsNullOrEmpty(m.CellValue) & !m.CellValue.IsNumber() &
                        isOrgOrCompanyCustomer) > 0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的注册资金必须是整数！");
            }
            if (
                row.Count(
                    m =>
                        m.ColumnName == "银行账号" & !string.IsNullOrEmpty(m.CellValue) & !m.CellValue.IsBankAccount() &
                        isPersonalCustomer) >
                0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的银行账号格式有误！");
            }
            if (
                row.Count(
                    m =>
                        m.ColumnName == "公司网站" & !string.IsNullOrEmpty(m.CellValue) & !m.CellValue.IsUrl() &
                        isOrgOrCompanyCustomer) > 0)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的公司网站格式有误！");
            }
            if (row.Count(m => m.ColumnName == "联系人手机" & !string.IsNullOrEmpty(m.CellValue)) == 0 &
                row.Count(m => m.ColumnName == "联系人座机" & !string.IsNullOrEmpty(m.CellValue)) == 0 &
                isOrgOrCompanyCustomer)
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的联系人手机、座机至少填一项！");
            }
            if (
                !(row.Count(
                    m =>
                        m.ColumnName == "客户类型" &
                        (m.CellValue == "0" || m.CellValue == "1" || m.CellValue == "2")) > 0))
            {
                errorCount++;
                valid = false;
                if (errorCount > 20) return false;
                businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的客户类型有误！");
            }
            if (isPersonalCustomer)
            {
                if (
                    !(row.Count(
                        t => t.ColumnName == "联系人手机" & !string.IsNullOrEmpty(t.CellValue)) > 0))
                {
                    errorCount++;
                    valid = false;
                    if (errorCount > 20) return false;
                    businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的联系人手机必填！");
                }
                if (
                    !(row.Count(t => t.ColumnName == "联系人" & !string.IsNullOrEmpty(t.CellValue)) >
                      0 &&
                      row.Find(m => m.ColumnName == "客户名称").CellValue.Trim() ==
                      row.Find(m => m.ColumnName == "联系人").CellValue.Trim()))
                {
                    errorCount++;
                    valid = false;
                    if (errorCount > 20) return false;
                    businessExceptions.AppendLine("您导入excel的第" + rowIndex + "行的客户名称和联系人必须一致！");
                }
            }
            return valid;
        }

        /// <summary>
        /// 准备excel导入的客户数据
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="user"></param>
        /// <param name="managerId"></param>
        /// <param name="managerName"></param>
        /// <param name="importToRowIndex"></param>
        /// <param name="businessExceptions"></param>
        /// <returns></returns>
        private List<CustomerInfo> PrepareImportCustomerData(List<List<RowValue>> rows, UserSimpleDTO user, string managerId, string managerName, ref int importToRowIndex, StringBuilder businessExceptions)
        {
            var seed = ConfigApiService.GetSeedEntityByName("Customer", rows.Count);
            var min = seed.SeedMin;
            var codePrefix = seed.SeedValue.Substring(0, seed.SeedValue.Length - 5);
            var newCustomers = new List<CustomerInfo>();
            int n = 0;
            int errorCount = 0;
            var allProvinces = _dictionaryApiService.LoadAllProvinces();
            var allCities = _dictionaryApiService.LoadAllCities();
            for (var i = 0; i < rows.Count; i++)
            {
                if (n == 0)
                {
                    n++;
                    continue;
                }
                var code = codePrefix + (min + i).ToString().PadLeft(5, '0');

                var customer = new CustomerInfo
                {
                    CustomerCode = code,
                    ClientType = ClientType.Potential,
                    CustomerSource = CustomerSource.Import,
                };

                List<RowValue> row = rows[i];
                bool isValid = ValidImportCustomerData(row, i, ref errorCount, businessExceptions);
                if (!isValid)
                    continue;
                if (errorCount > 20)
                    break;
                importToRowIndex = i;
                var provinceId = 0;
                foreach (var cell in row)
                {
                    switch (cell.ColumnName)
                    {
                        case "客户名称":
                            customer.CustomerName = cell.CellValue;
                            break;
                        case "客户编号":
                            customer.ReferenceId = cell.CellValue;
                            break;
                        case "客户类型":
                            var iType = 1;
                            var issuccess = int.TryParse(cell.CellValue, out iType);
                            if (issuccess)
                            {
                                try
                                {
                                    customer.CompanyType = (CompanyType)iType;
                                }
                                catch
                                {
                                    customer.CompanyType = CompanyType.Customer;
                                }
                            }
                            break;

                        case "联系人":
                            customer.ContactName = cell.CellValue;
                            break;
                        //case "联系人职务":
                        //    customer.PositionName = cell.CellValue;
                        //    break;
                        //case "联系人邮箱":
                        //    customer.ContactEmail = cell.CellValue;
                        //    break;
                        //case "联系人手机":
                        //    customer.ContactPhoneNumber = cell.CellValue;
                        //    break;
                        //case "联系人座机":
                        //    customer.ContactTelephone = cell.CellValue;
                        //    break;
                        //case "联系人QQ":
                        //    customer.ContactQQ = cell.CellValue;
                        //    break;
                        //case "联系人微信":
                        //    customer.ContactWeixin = cell.CellValue;
                        //    break;
                        //case "详细地址":
                        //    customer.Address = cell.CellValue;
                        //    break;
                        //case "省名称":
                        //    if (!string.IsNullOrWhiteSpace(cell.CellValue))
                        //    {
                        //        var provinceName = cell.CellValue;
                        //        if (!provinceName.Contains("省"))
                        //        {
                        //            provinceName += "省";
                        //        }
                        //        provinceId = allProvinces.FirstOrDefault(o => o.Name == provinceName).ProvinceId;
                        //        if (provinceId != 0)
                        //        {
                        //            customer.ProviceID = provinceId;
                        //            customer.ProviceName = provinceName;
                        //        }
                        //    }
                        //    break;
                        //case "市名称":
                        //    if (!string.IsNullOrWhiteSpace(cell.CellValue))
                        //    {
                        //        var cityName = cell.CellValue;
                        //        if (!cityName.Contains("市"))
                        //        {
                        //            cityName += "市";
                        //        }
                        //        var cityId = allCities.FirstOrDefault(o => o.Name == cityName).Id;
                        //        var provinceCities = allCities.Where(o => o.ProvinceId == provinceId && o.Id == cityId);
                        //        if (cityId != 0 && provinceCities != null)
                        //        {
                        //            customer.CityID = cityId;
                        //            customer.CityName = cityName;
                        //        }
                        //    }
                        //    break;
                        //case "身份证":
                        //    if (customer.CompanyType == CompanyType.Personal)
                        //        customer.IdCard = cell.CellValue;
                        //    break;
                        //case "银行账号":
                        //    if (customer.CompanyType == CompanyType.Personal)
                        //        customer.BankAccount = cell.CellValue;
                        //    break;
                        //case "所属单位":
                        //    if (customer.CompanyType == CompanyType.Personal)
                        //        customer.AffiliatedCompany = cell.CellValue;
                        //    break;
                        //case "所属部门":
                        //    if (customer.CompanyType == CompanyType.Personal)
                        //        customer.AffiliatedDepartment = cell.CellValue;
                        //    break;

                        //case "法人代表":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.LegalPerson = cell.CellValue;
                        //    break;
                        //case "注册号":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.RegistrationNumber = cell.CellValue;
                        //    break;
                        //case "注册资金":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.RegisteredAssets = Convert.ToInt32(cell.CellValue);
                        //    break;
                        //case "注册地址":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.RegisteredAddress = cell.CellValue;
                        //    break;
                        //case "组织机构代码":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.OrganizationCodeNumber = cell.CellValue;
                        //    break;
                        //case "登记机关":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.RegistrationAuthority = cell.CellValue;
                        //    break;
                        //case "统一社会信用代码":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.UnifiedSocialCreditCode = cell.CellValue;
                        //    break;
                        //case "公司电话":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.CompanyPhone = cell.CellValue;
                        //    break;
                        //case "公司邮箱":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.CompanyEmail = cell.CellValue;
                        //    break;
                        //case "公司网站":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.CompanyWebsite = cell.CellValue;
                        //    break;
                        //case "所属行业":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.Industry = cell.CellValue;
                        //    break;
                        //case "营业期限":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.BusinessTerm = cell.CellValue;
                        //    break;
                        //case "经营规模":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.BusinessScale = cell.CellValue;
                        //    break;
                        //case "经营范围":
                        //    if (customer.CompanyType != CompanyType.Personal)
                        //        customer.BusinessScope = cell.CellValue;
                        //    break;
                    }
                }

                //客户经理
                var customerManager = new CustomerManager
                {
                    OrganizationId = user.UserOrgIds.FirstOrDefault(),
                    OrganizationName = user.UserOrgNames.FirstOrDefault(),
                    CustomerManagerId = managerId,
                    CustomerManagerName = managerName,
                };
                customer.CustomerManagers.Add(customerManager);

                //客户联系人
                var customerContact = new CustomerContact
                {
                    ContactName = customer.ContactName,
                    //PositionName = customer.PositionName,
                    //ContactEmail = customer.ContactEmail,
                    //ContactPhoneNumber = customer.ContactPhoneNumber,
                    //ContactTelephone = customer.ContactTelephone,
                    //ContactQQ = customer.ContactQQ
                };
                customer.CustomerContacts.Add(customerContact);
                newCustomers.Add(customer);
            }
            return newCustomers;
        }
        #endregion

        #region call center

        public string IsNotBusy(string currentUserPhone)
        {
            var config = ConfigApiService.GetTenantCallUncallConfig(Tenant);
            return CallUtil.IsNotBusy(config, currentUserPhone);
        }

        /// <summary>
        /// 拨打电话
        /// </summary>
        ///  <param name="customerContactId">联系人Id</param>
        /// <param name="currentUserId">客服Id</param>
        /// <param name="currentUserName">客服姓名</param>
        /// <param name="currentUserPhone">客服电话</param>
        /// <param name="customerId">客户Id</param>
        /// <param name="customerName">客户姓名</param>
        /// <param name="contactName">联系人姓名</param>
        /// <param name="customerPhoneNumber">客户电话</param>
        /// <param name="referenceId">当前通话记录会话Id</param>
        /// <param name="record">
        /// 主叫号码类型
        ///     0: 座机；1: 手机
        /// </param>
        /// <returns></returns>
        public string RunCallContact(int customerContactId,string currentUserId, string currentUserName, string currentUserPhone,
            int customerId, string customerName, string contactName, string customerPhoneNumber, CallType record, out string referenceId)
        {
            string sesstionId;
            var config = ConfigApiService.GetTenantCallUncallConfig(Tenant);
            var message = CallUtil.CallContact(config, currentUserPhone, customerPhoneNumber, record,
                out sesstionId);
            if (string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(sesstionId))
            {
                var customerTracingLog = new CustomerTracingLog
                {
                    ReferenceId = sesstionId,
                    CustomerName = customerName,
                    OperatorId = currentUserId,
                    Operator = currentUserName,
                    OperateDate = DateTime.UtcNow,
                    TracingType = TracingType.CallNotify,
                    CustomerContactId = customerContactId,
                    ContactName=contactName,
                    Caller = currentUserName,
                    CallerPhone = currentUserPhone,
                    Callee = customerName,
                    CalleePhone = customerPhoneNumber,
                    StartTime = DateTime.UtcNow,
                    CallState = CallState.Failed
                };
                if (customerId != 0)
                {
                    customerTracingLog.CustomerId = customerId;
                }
                _customerTracingLogRepository.Add(customerTracingLog);

                switch (config.ProviderName.ToLower())
                {
                    case "uncall":
                        //长鑫盛通呼叫平台，手机外呼不支持通话录音及通话记录
                        if (record == CallType.Telephone)
                        {
                            var extNum = currentUserPhone.GetExtensionNumber();
                            var uncallEntity = new UncallEntity()
                            {
                                Tenant = base.Tenant.TenantName,
                                SessionId = sesstionId,
                                CallerName = currentUserName,
                                Caller = currentUserPhone,
                                CallerExt = extNum,
                                Becaller = customerPhoneNumber,
                                BecallerName = customerName,
                            };
                            //测试时候注释掉，太耗时了
                            bool isSuccess = new StorageQueueService().InsertUncallQueue(uncallEntity);
                            //LogUtil.LogInfo("长鑫盛通手机外呼，插入队列" + (isSuccess ? "成功" : "失败"));
                        }
                        break;
                }
                //var tenantUserCallCharge = new TenantUserCallChargeDTO
                //{
                //    SessionId = sesstionId,
                //    TenantName = base.Tenant.TenantName,
                //    TenantDisplayName = base.Tenant.TenantDisplayName,
                //    Caller = currentUserName,
                //    CallerPhone = currentUserPhone,
                //    BeCaller = customerName,
                //    BeCallerPhone = customerPhoneNumber,
                //    StartTime = DateTime.UtcNow,
                //    EndTime = DateTime.UtcNow,
                //    StarttimeCalled = DateTime.UtcNow,
                //    IsDownloadVoice = false,
                //    CallStatus = CallChargeStatus.CallStart,
                //};

                //var result = TenantUserApiService.SaveCallCharge(tenantUserCallCharge);
                //referenceId = sesstionId;
                //LogUtil.LogInfo("保存租户通话记录:" + (result.Result ? "成功" : "失败"));
                //return result.success && result.Result ? null : "生成通话记录失败。";

                referenceId = sesstionId;
                return null;
            }

            EmailUtil.SendAdministratorMail("呼叫语音服务出错，请查看错误信息", message);
            referenceId = sesstionId;
            return message;
        }

        public bool StopCallContact(string customerPhoneNumber, string message)
        {
            var config = ConfigApiService.GetTenantCallUncallConfig(Tenant); 
            var isSuccess = CallUtil.StopCallContact(config, customerPhoneNumber, message);
            return isSuccess;
        }

        /// <summary>
        /// 呼叫弹屏修改客户信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="referenceId">当前通话会话Id</param>
        /// <param name="callRemark">当前通话备注</param>
        /// <returns></returns>
        public bool SavePopCustomerInfo(CustomerContactInfoDTO model, string referenceId,string callRemark)
        {
            CustomerInfo customer = _customerInfoRepository.GetById(model.CustomerId);
            CustomerContact customerContact = _customerContactRepository.GetById(model.CustomerContactId);
            customer.CompanyType = model.CompanyType;
            customer.ClientType = model.ClientType;
            if (customerContact.IsDefault)
            {
                customer.ContactName = model.ContactName;
                //customer.PositionName = model.PositionName;
                //customer.ContactPhoneNumber = model.ContactPhoneNumber;
            }
            customerContact.ContactName = model.ContactName;
            customerContact.PositionName = model.PositionName;
            customerContact.ContactPhoneNumber = model.ContactPhoneNumber;

            if (model.CompanyType == CompanyType.Supplier || model.CompanyType == CompanyType.Institute)
            {
                bool existCustomerName =
                    _customerInfoRepository.GetByFilter(
                        o =>
                            !o.IsDeleted && o.CustomerName == model.AffiliatedCompany &&
                            o.CustomerName != customer.CustomerName) != null;
                if (existCustomerName)
                    throw new BusinessPromptException("已存在相同名称的企业或机构名称！");
                customer.CustomerName = model.AffiliatedCompany;
            }
            //保存通话备注
            if (!string.IsNullOrEmpty(referenceId))
            {
                var customerTracingLog = _customerTracingLogRepository.GetByFilter(o => o.ReferenceId == referenceId);
                customerTracingLog.Remark = callRemark;
                _customerTracingLogRepository.Modify(customerTracingLog, false);
            }
            _customerInfoRepository.Modify(customer, false);
            _customerContactRepository.Modify(customerContact, false);
            return CustomerUnitOfWorkContext.Commit() > 0;
        }

        #endregion

        #region 客服分配

        /// <summary>
        /// 自动分配客户
        /// </summary>
        /// <param name="selectedUsers">客服Ids</param>
        /// <param name="operatorId">当前操作人</param>
        /// <returns></returns>
        public bool ShareCustomerWithManager(List<string> selectedUsers, string operatorId)
        {
            //var canAssginUserRole = new List<string>() { RoleType.ClientService.ToString() };
            //Expression<Func<AspNetRole, bool>> predicate = m => canAssginUserRole.Contains(m.Name);
            //var clientServiceRole = _roleRepoRespository.FindRoleWithUsersByFilter(predicate, m => m.Name).FirstOrDefault();
            var clientServiceRole = AccountApiService.GetRoleWithUsersByRoleId(RoleConstants.ManufactingRoleId).Result;

            if (clientServiceRole == null)
                return true;

            var userIds = clientServiceRole.Users.Select(m => m.UserId).ToList();
            if (!userIds.Any())
                return true;
            bool ckrole = true;
            for (int i = 0; i < selectedUsers.Count; i++)
            {
                if (!userIds.Contains(selectedUsers[i]))
                {
                    ckrole = false;
                    break;
                }
            }
            //获取操作人所在的部门信息(只有最高权限的可以分配所有部门的)
            //var user = _userRepository.FindByIdIncludeOrganizations(operatorId);
            var user = AccountApiService.GetUserWithOrgsAndRolesByUserId(operatorId).Result;
            if (!ckrole && user.UserOrgIds.Count() <= 0)
            {
                throw new BusinessPromptException("请选择自己所掌管权限的客服！");
            }

            // 客服平均分配所有的客户
            //var allClientSerices = _userRepository.FindByIdsIncludeOrganizations(selectedUsers);
            var allClientSerices = AccountApiService.LoadUsersByIds(selectedUsers).Result;
            if (allClientSerices.Any(o => user.UserOrgIds.Any(m => o.UserOrgIds.Contains(m))))
            {
                throw new BusinessPromptException("只能分配客户给直接下属部门人员！");
            }

            //获取未分配客户经理的客户列表
            var unassignCustomers = _customerInfoRepository.GetUnassignCustomerInfos();
            var unassignCustomerIds = unassignCustomers.Select(m => m.CustomerId).ToList();
            if (!unassignCustomerIds.Any())
                return true;

            var csAssignedCustomerCount = allClientSerices.ToDictionary(clientService => clientService.UserId,
                clientService => 0);
            var assignCustomerToClientService = GetAssignClientService(csAssignedCustomerCount, unassignCustomerIds);
            var customerManagers = new List<CustomerManager>();
            foreach (var keyValuePair in assignCustomerToClientService)
            {
                var clientService = allClientSerices.FirstOrDefault(m => m.UserId == keyValuePair.Key);
                if (clientService == null) continue;
                var organService = clientService.UserOrgIds.FirstOrDefault();
                if (organService == null) continue;

                KeyValuePair<string, List<int>> pair = keyValuePair;
                var unassignCustomer = unassignCustomers.Where(m => pair.Value.Contains(m.CustomerId));
                foreach (var customerInfo in unassignCustomer)
                {
                    var customerManager = new CustomerManager
                    {
                        CustomerId = customerInfo.CustomerId,
                        CustomerManagerId = clientService.UserId,
                        CustomerManagerName = clientService.DisplayName,
                        OrganizationId = clientService.UserOrgIds.FirstOrDefault(),
                        OrganizationName = clientService.UserOrgNames.FirstOrDefault()
                    };
                    customerManagers.Add(customerManager);
                }
            }
            //查看sqlserver被锁的表以及如何解锁: http://www.2cto.com/database/201211/170680.html
            return _customerManagerRepository.Add(customerManagers) > 0;
        }

        /// <summary>
        /// 重新分配客户
        /// </summary>
        /// <param name="selectedUserIds">客服Ids</param>
        /// <param name="customerIds">客户Ids</param>
        /// <param name="operatorId">当前操作人</param>
        /// <returns></returns>
        public bool ReassignCustomerToOtherManager(List<string> selectedUserIds, List<int> customerIds,
            string operatorId)
        {
            //获取操作人所在的部门信息(只有最高权限的可以分配所有部门的)
            var userOrgs = AccountApiService.LoadOrganizationsWithUsersByUserId(operatorId).Result;
            //var clientServiceUsers = _userRepository.FindByIdsIncludeOrganizations(selectedUserIds); //选择的跟进人
            var clientServiceUsers = AccountApiService.LoadUsersByIds(selectedUserIds).Result;
            if (clientServiceUsers.Any(o => userOrgs.Any(m => o.UserOrgIds.Contains(m.Id))))
            {
                throw new BusinessPromptException("只能分配客户给直接下属部门人员！");
            }

            //所有部门
            //var organizations = _organizationRepository.GetAllOrganizationsWithUsers(o => !o.IsDeleted);
            var organizations = AccountApiService.LoadAllOrganization().Result;
            if (clientServiceUsers.Count >= 2)
            {
                if (
                    clientServiceUsers
                        .Count(m => !m.UserOrgIds.Any(o => organizations.Any(p => p.ParentId == o))) >= 2)
                {
                    throw new BusinessPromptException("只能分配客户给一个末级部门人员！");
                }
            }

            var customers = _customerInfoRepository.GetCustomerInfosByIds(customerIds);
            var addCustomerManagers = new List<CustomerManager>();
            var allOrgsWithUsers = AccountApiService.LoadOrgTreesWithUsers().Result;
            Expression<Func<CustomerManager, bool>> predicate = o => true;
            foreach (var customer in customers)
            {
                var custIds = new List<int> { customer.CustomerId };
                foreach (var clientServiceUser in clientServiceUsers)
                {
                    //判断分配的客服所有下级部门是否存在已经分配记录
                    var orgIds = clientServiceUser.UserOrgIds.ToList();
                    //var organizationIds =
                    //    _organizationRepository.GetAllOrganizationsByParentOrganizationIds(orgIds)
                    //        .Select(o => (int?) o.Id)
                    //        .ToList();
                    var organizationIds = AccountApiService.LoadOrgTreesByOrgIds(orgIds).Result
                        .Select(o => (int?) o.Id)
                        .ToList();
                    if (organizationIds.Count > 0)
                    {
                        predicate = o => true;
                        if (custIds != null && custIds.Any())
                        {
                            predicate = predicate.And(o => custIds.Contains(o.CustomerId.Value));
                        }
                        if (organizationIds != null && organizationIds.Any())
                        {
                            predicate = predicate.And(o => organizationIds.Contains(o.OrganizationId));
                        }
                        bool hasChildOrganizationReassigned =
                            _customerManagerRepository.GetCustomerManagersById(predicate).Any();
                        if (hasChildOrganizationReassigned)
                            break;
                    }

                    predicate = o => true;
                    if (custIds != null && custIds.Any())
                    {
                        predicate = predicate.And(o => custIds.Contains(o.CustomerId.Value));
                    }

            //判断系统是否存在末级人员分配了该客户
            bool hasLastClientServiceUserForCustomer =
                        _customerManagerRepository.GetCustomerManagersById(predicate)
                            .Any(o => organizations.All(p => p.ParentId != o.OrganizationId));
            
                    //判断当前分配的客服是否只在末级部门
                    bool clientServiceIsLastOrganization =
                        !organizations.Any(o => clientServiceUser.UserOrgIds.Any(p => p == o.ParentId));
                    if (!(hasLastClientServiceUserForCustomer && clientServiceIsLastOrganization))
                    {
                        if (clientServiceUser.UserOrgIds.Any(o => userOrgs.Any(p => p.Id == o)))
                        {
                            predicate = o => true;
                            if (!string.IsNullOrEmpty(clientServiceUser.UserId))
                            {
                                predicate = predicate.And(o => o.CustomerManagerId == clientServiceUser.UserId);
                            }
                            if (custIds.Any())
                            {
                                predicate = predicate.And(o => custIds.Contains(o.CustomerId.Value));
                            }

                            if (
                                !_customerManagerRepository.GetCustomerManagersById(predicate).Any())
                            {
                                var clientServiceUserOrgIds =
                                    clientServiceUser.UserOrgIds
                                        .FirstOrDefault(o => userOrgs.Any(p => p.Id == o));
                                var clientServiceUserOrgNames =
                                    clientServiceUser.UserOrgNames
                                        .FirstOrDefault(o => userOrgs.Any(p => p.Text == o));
                                //要新增的跟进人
                                var addCustomerManager = new CustomerManager
                                {
                                    CustomerId = customer.CustomerId,
                                    CustomerManagerId = clientServiceUser.UserId,
                                    CustomerManagerName = clientServiceUser.DisplayName,
                                    OrganizationId = clientServiceUserOrgIds,
                                    OrganizationName =
                                        clientServiceUserOrgNames == null
                                            ? string.Empty
                                            : clientServiceUserOrgNames
                                };
                                addCustomerManagers.Add(addCustomerManager);
                            }
                        }
                    }
                }
            }
            if (addCustomerManagers.Count == 0)
            {
                throw new BusinessPromptException("您选择的客户已经分配了客服！");
            }
            _customerManagerRepository.Add(addCustomerManagers, false);
            CustomerUnitOfWorkContext.Commit();
            return true;
        }

        /// <summary>
        /// 转移客户
        /// </summary>
        /// <param name="selectdUserId">客服Id</param>
        /// <param name="customerIds">客户Ids</param>
        /// <param name="operatorId">当前操作人</param>
        /// <returns></returns>
        public bool TransferCustomerToOtherManager(string selectdUserId, List<int> customerIds, string operatorId)
        {
            if (selectdUserId == operatorId)
            {
                throw new BusinessPromptException("没必要转移给自己！");
            }
            //获取操作人所在的部门信息(只有最高权限的可以分配所有部门的)
            //var user = _userRepository.FindByIdIncludeOrganizations(operatorId);
            var userOrgs = AccountApiService.LoadOrganizationsWithUsersByUserId(operatorId).Result;
            
            //var clientServiceUser = _userRepository.FindByIdIncludeOrganizations(selectdUserId); //选择的客服
            var clientServiceUser = AccountApiService.GetUserWithOrgsAndRolesByUserId(selectdUserId).Result;
            if (!userOrgs.Any(o => clientServiceUser.UserOrgIds.Any(p => p == o.Id)))
            {
                throw new BusinessPromptException("只能在同级部门转移客户！");
            }

            //var organizations = _organizationRepository.GetAllOrganizationsWithUsers(o => !o.IsDeleted);
            var organizations = AccountApiService.LoadAllOrganization().Result;
            if (organizations.Any(o => userOrgs.Any(p => p.Id == o.ParentId)))
            {
                throw new BusinessPromptException("只有末级客服才能转移客户！");
            }
            Expression<Func<CustomerManager, bool>> predicate = o => true;
            if (!string.IsNullOrEmpty(operatorId))
            {
                predicate = predicate.And(o => o.CustomerManagerId == operatorId);
            }
            if (customerIds != null && customerIds.Any())
            {
                predicate = predicate.And(o => customerIds.Contains(o.CustomerId.Value));
            }
            var customerManagers = _customerManagerRepository.GetCustomerManagersById(predicate);
            if (!customerManagers.Any())
            {
                throw new BusinessPromptException("只能转移自己的客户！");
            }

            foreach (var customerManager in customerManagers)
            {
                predicate = o => true;
                if (!string.IsNullOrEmpty(clientServiceUser.UserId))
                {
                    predicate = predicate.And(o => o.CustomerManagerId == clientServiceUser.UserId);
                }
                if (customerIds != null && customerIds.Any())
                {
                    predicate = predicate.And(o => customerIds.Contains(o.CustomerId.Value));
                }
                List<CustomerManager> forUpdateCustomerManagers = new List<CustomerManager>();
                if (!_customerManagerRepository.GetCustomerManagersById(predicate).Any())
                {
                    int clientServiceUserOrgIds = 0;
                    string clientServiceUserOrgNames = null;
                    for (var i=0; i< clientServiceUser.UserOrgIds.Count(); i++)
                    {
                        if (userOrgs.Any(p => p.Id == clientServiceUser.UserOrgIds[i]))
                        {
                            clientServiceUserOrgIds = clientServiceUser.UserOrgIds[i];
                            clientServiceUserOrgNames = clientServiceUser.UserOrgNames[i];
                            break;
                        }
                    }

                    customerManager.CustomerManagerId = clientServiceUser.UserId;
                    customerManager.CustomerManagerName = clientServiceUser.DisplayName;
                    customerManager.OrganizationId = clientServiceUserOrgIds;
                    customerManager.OrganizationName = clientServiceUserOrgNames == null
                        ? string.Empty
                        : clientServiceUserOrgNames;
                    forUpdateCustomerManagers.Add(customerManager);
                }
                if (!forUpdateCustomerManagers.Any())
                {
                    throw new BusinessPromptException("您选择的客户已经被转移过了！");
                }
            }
            return _customerManagerRepository.Modify(customerManagers) > 0;
        }

        /// <summary>
        /// 将未分配的客户平均分配给每个客服
        ///     单元测试：KC.UT.Account.ComponentTest.TypeExtensionsTest.Test_GetAssignClientService
        /// </summary>
        /// <param name="csAssignedCustomerCount">每个客服已经分配的客户数</param>
        /// <param name="unassignCustomerIds">未分配的客户Id列表</param>
        /// <returns>UserId, List<CustomerId></returns>
        private Dictionary<string, List<int>> GetAssignClientService(Dictionary<string, int> csAssignedCustomerCount,
            List<int> unassignCustomerIds)
        {
            var assignCustomerToClientService = new Dictionary<string, List<int>>(); //UserId, List<CustomerId>
            foreach (var customerId in unassignCustomerIds)
            {
                var minAssginedClientService = csAssignedCustomerCount.OrderBy(m => m.Value).FirstOrDefault();
                var userId = minAssginedClientService.Key;
                csAssignedCustomerCount.Remove(userId);
                csAssignedCustomerCount.Add(userId, minAssginedClientService.Value + 1);
                if (assignCustomerToClientService.ContainsKey(userId))
                {
                    var customerIds = assignCustomerToClientService[userId];
                    customerIds.Add(customerId);
                    assignCustomerToClientService.Remove(userId);
                    assignCustomerToClientService.Add(userId, customerIds);
                }
                else
                {
                    assignCustomerToClientService.Add(userId, new List<int>() { customerId });
                }
            }

            return assignCustomerToClientService;
        }

        public async Task<string> GetCustomerAssignedInfoAsync(int customerId, string operatorId)
        {
            Expression<Func<CustomerManager, bool>> predicate = o => true;
            var customerIds = new List<int> {customerId};
            if (customerIds != null && customerIds.Any())
            {
                predicate = predicate.And(o => customerIds.Contains(o.CustomerId.Value));
            }
            
            var customerManagers = _customerManagerRepository.GetCustomerManagersById(predicate);

            var currentUserSubOrgnizations = await Util.DataPermitUtil.LoadCurrentUserRelationOrganizationIds(AccountApiService, operatorId, false, false, true);
            //某客户分配给当前操作人下级部门的部门数量总和
            var assignedOrganizationsCount =
                customerManagers.GroupBy(o => o.OrganizationId).Count(o => currentUserSubOrgnizations.Any(p => p == o.Key));

            var result = string.Empty;
            if (currentUserSubOrgnizations.Any())
            {
                if (assignedOrganizationsCount == 0)
                    result = "0/0";
                else
                    result = assignedOrganizationsCount + "/" + currentUserSubOrgnizations.Count();
            }
            return result;
        }

        /// <summary>
        /// 查找客户分配后的跟进人(先直接下级部门，再自己)
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="customerId"></param>
        /// <param name="roleList"></param>
        /// <returns></returns>
        public async Task<List<CustomerManagerDTO>> GetCustomerManagersByCustomerIdAsync(string currentUserId, int customerId,
            List<string> roleList)
        {
            Expression<Func<CustomerManager, bool>> predicate = o => true;
            //查看同级别部门跟进人（仅限客服经理）
            var siblingsCustomerManagers = new List<CustomerManager>();
            if (roleList.Contains(RoleConstants.SaleRoleId) || roleList.Contains(RoleConstants.SaleManagerRoleId))
            {
                var selfAndSiblingsOrganizationIds = await Util.DataPermitUtil.LoadCurrentUserRelationOrganizationIds(AccountApiService, currentUserId);
                if (selfAndSiblingsOrganizationIds.Any())
                {
                    predicate = o => true;
                    var customerIds = new List<int> { customerId };
                    if (customerIds != null && customerIds.Any())
                    {
                        predicate = predicate.And(o => customerIds.Contains(o.CustomerId.Value));
                    }
                    if (selfAndSiblingsOrganizationIds != null && selfAndSiblingsOrganizationIds.Any())
                    {
                        //predicate = predicate.And(o => selfAndSiblingsOrganizationIds.Contains(o.OrganizationId));
                        predicate = predicate.And(o => o.OrganizationId.HasValue && selfAndSiblingsOrganizationIds.Contains(o.OrganizationId.Value));
                    }
                    var customerMangers = _customerManagerRepository.GetCustomerManagersById(predicate)
                        .Where(o => o.CustomerManagerId != currentUserId);
                    siblingsCustomerManagers.AddRange(customerMangers);
                }
            }
            //查看直接下级跟进人和跟进人是自己
            List<CustomerManager> customerManagers = new List<CustomerManager>();
            var customerManger = new List<CustomerManager>();
            var childOrganizationIds = await Util.DataPermitUtil.LoadCurrentUserRelationOrganizationIds(AccountApiService, currentUserId, false, false, true);
            if (childOrganizationIds.Any())
            {
                predicate = o => true;
                var customerIds = new List<int> { customerId };
                if (customerIds != null && customerIds.Any())
                {
                    predicate = predicate.And(o => customerIds.Contains(o.CustomerId.Value));
                }
                if (childOrganizationIds != null && childOrganizationIds.Any())
                {
                    //predicate = predicate.And(o => childOrganizationIds.Contains(o.OrganizationId));
                    predicate = predicate.And(o => o.OrganizationId.HasValue && childOrganizationIds.Contains(o.OrganizationId.Value));
                }
                customerManger = _customerManagerRepository.GetCustomerManagersById(predicate);
            }
            if (customerManger.Any())
            {
                //直接下级部门
                customerManagers.AddRange(customerManger);
            }
            else
            {
                var allOrgsWithUsers = AccountApiService.LoadOrgTreesWithUsers().Result;
                foreach (var orgId in childOrganizationIds)
                {
                    //查找直接下级部门的所有子部门中是否有人分配过该客户
                    //var allChildOrganizationIds =
                    //    _organizationRepository.GetAllOrganizationsByParentOrganizationIds(new List<int?> {orgId})
                    //        .Select(o => (int?) o.Id)
                    //        .ToList();
                    var allChildOrganizationIds = allOrgsWithUsers.Where(m => m.ParentId.Equals(orgId))
                            .Select(o => (int?)o.Id)
                            .ToList();
                    if (allChildOrganizationIds.Count > 0)
                    {
                        predicate = o => true;
                        var customerIds = new List<int> { customerId };
                        if (customerIds != null && customerIds.Any())
                        {
                            predicate = predicate.And(o => customerIds.Contains(o.CustomerId.Value));
                        }
                        if (allChildOrganizationIds != null && allChildOrganizationIds.Any())
                        {
                            predicate = predicate.And(o => allChildOrganizationIds.Contains(o.OrganizationId));
                        }
                        customerManger = _customerManagerRepository.GetCustomerManagersById(predicate);
                        if (customerManger.Any())
                        {
                            //查找该部门的管理岗位人员
                            var addCustomerManager = new CustomerManager
                            {
                                OrganizationId = orgId,
                                //OrganizationName = orgId.HasValue && allOrgsWithUsers.Any(m => m.id.Equals(orgId.Value))
                                //    ? allOrgsWithUsers.FirstOrDefault(m => m.id.Equals(orgId.Value)).Name 
                                //    : string.Empty
                                OrganizationName = allOrgsWithUsers.Any(m => m.Id.Equals(orgId))
                                    ? allOrgsWithUsers.FirstOrDefault(m => m.Id.Equals(orgId)).Text
                                    : string.Empty
                                
                            };

                            var user = allOrgsWithUsers.SelectMany(m => m.Users)
                                .Where(m => m.PositionLevel > 0)
                                .OrderByDescending(o => o.PositionLevel)
                                .FirstOrDefault();
                            if (user != null)
                                addCustomerManager.CustomerManagerName = user.DisplayName;
                            else
                            {
                                //user = orgId.HasValue
                                //    ? AccountApiService.GetUsersByOrgId(orgId.Value).Result.FirstOrDefault()
                                //    : null;
                                var auser = AccountApiService.LoadUsersByOrgId(orgId).Result.FirstOrDefault();
                                if (auser != null)
                                    addCustomerManager.CustomerManagerName = auser.DisplayName;
                            }
                            customerManagers.Add(addCustomerManager);
                        }
                    }
                }
            }
            //查找自己是跟进人的记录 
            if (!customerManger.Any())
            {
                predicate = o => true;
                if (!string.IsNullOrEmpty(currentUserId))
                {
                    predicate = predicate.And(o => o.CustomerManagerId == currentUserId);
                }
                var customerIds = new List<int> { customerId };
                if (customerIds != null && customerIds.Any())
                {
                    predicate = predicate.And(o => customerIds.Contains(o.CustomerId.Value));
                }
                customerManger = _customerManagerRepository.GetCustomerManagersById(predicate);
                customerManagers.AddRange(customerManger);
            }
            customerManagers.AddRange(siblingsCustomerManagers);
            return _mapper.Map<List<CustomerManagerDTO>>(customerManagers);
        }

        #endregion

        #region 群发申请

        public bool SaveSendApplication(NotificationApplicationDTO data)
        {
            var model = _mapper.Map<NotificationApplication>(data);
            if (model.ApplicationId == 0)
            {
                return _notificationApplicationRepository.Add(model);
            }
            else
            {
                return _notificationApplicationRepository.Modify(model, true);
            }
        }

        /// <summary>
        /// 修改部分Application列值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public bool SaveApplication(NotificationApplicationDTO data, string[] properties)
        {
            bool bl = false;
            var model = _mapper.Map<NotificationApplication>(data);
            if (model.ApplicationId > 0)
            {
                bl = _notificationApplicationRepository.Modify(model, properties, true);
            }
            return bl;
        }

        /// <summary>
        /// 得到邮件/短信列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="status"></param>
        /// <param name="endTime"></param>
        /// <param name="roleList"></param>
        /// <param name="currentUserId"></param>
        /// <param name="onlyShowClientSerivce"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public PaginatedBaseDTO<NotificationApplicationDTO> GetNotificationApplication(int pageIndex, int pageSize,
            string searchValue, int status, string startTime, string endTime, List<string> roleList,
            string currentUserId, bool onlyShowClientSerivce)
        {
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            DateTime.TryParse(startTime, out startDate);
            DateTime.TryParse(endTime, out endDate);

            Expression<Func<NotificationApplication, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                predicate =
                    predicate.And(
                        m =>
                            m.Title.Contains(searchValue) || m.Content.Contains(searchValue) ||
                            m.viewName.Contains(searchValue));
            }

            if (status > 0)
            {
                var currStatus = (WorkflowBusStatus)status;
                predicate = predicate.And(m => m.Status == currStatus);
            }
            if (startDate != DateTime.MinValue)
            {
                predicate = predicate.And(m => m.ApplicantDateTime >= startDate);
            }
            if (endDate != DateTime.MinValue)
            {
                endDate = endDate.AddDays(1);
                predicate = predicate.And(m => m.ApplicantDateTime <= endDate);
            }
            if (onlyShowClientSerivce
                && roleList.Contains(RoleConstants.SaleRoleId)
                && !string.IsNullOrWhiteSpace(currentUserId))
            {
                predicate =
                    predicate.And(m => m.ApplicantUserId.Equals(currentUserId, StringComparison.OrdinalIgnoreCase));
            }
            else if (!roleList.Contains(RoleConstants.SaleRoleId)
                     && !roleList.Contains(RoleConstants.SaleManagerRoleId))
            {
                return new PaginatedBaseDTO<NotificationApplicationDTO>(pageIndex, pageSize, 0,
                    new List<NotificationApplicationDTO>());
            }

            var data = _notificationApplicationRepository.FindPagenatedListWithCount(
                pageIndex,
                pageSize,
                predicate,
                m => m.ApplicantDateTime,
                false);

            var total = data.Item1;
            var rows = _mapper.Map<List<NotificationApplicationDTO>>(data.Item2);
            return new PaginatedBaseDTO<NotificationApplicationDTO>(pageIndex, pageSize, total, rows);
        }

        /// <summary>
        /// 获取当前短信/邮件的详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NotificationApplicationDTO GetApplicatioByIds(int id)
        {
            //var model = _NotificationApplicationRepository.GetNotificationApplicationByIds(id);
            //return _mapper.Map<List<NotificationApplicationDTO>>(model);
            var data = _notificationApplicationRepository.GetById(id);
            if (data.viewName == "SMS")
            {
                data.Content = data.Content.HtmlToTxt();
            }
            return _mapper.Map<NotificationApplicationDTO>(data);
        }

        #region 发送邮件/短信相关操作

        /// <summary>
        /// 发送短信/邮件信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentstatus"></param>
        /// <param name="currentUserDisplayName"></param>
        /// <param name="currentUserId"></param>
        /// <param name="isSingle"></param>
        /// <returns></returns>
        public Tuple<int, string> SendMess(string id, int currentstatus, string DisplayName, string currentUserId,
            bool isSingle)
        {
            string mess = "";
            int num = 0;
            bool bl = false;
            if (isSingle)
            {
                if (Convert.ToInt32(id) > 0)
                {
                    bl = SendMethods(Convert.ToInt32(id), currentstatus, DisplayName);
                    if (bl)
                    {
                        num = Convert.ToInt32(id);
                    }
                }

            }
            else
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    foreach (var tempid in id.Split(','))
                    {
                        if (Convert.ToInt32(tempid) > 0)
                            bl = SendMethods(Convert.ToInt32(tempid), currentstatus, DisplayName);
                        if (bl)
                        {
                            num = Convert.ToInt32(tempid);
                        }
                    }
                }
            }
            return new Tuple<int, string>(num, bl ? string.Empty : mess);
        }

        /// <summary>
        /// 发送方法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentstatus"></param>
        /// <param name="currentUserDisplayName"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        private bool SendMethods(int id, int currentstatus, string DisplayName)
        {
            bool bl = false;
            var notificationModel = _notificationApplicationRepository.GetById(id);
            if (notificationModel == null)
                return false;
            var names = notificationModel.CustomerNames;

            var sendUserId = notificationModel.ApplicantUserId;
            //var sendUserName = notificationModel.ApplicantUserName;
            if (notificationModel.viewName == "SMS")
            {
                notificationModel.Content = notificationModel.Content.HtmlToTxt();
            }
            var sendtoList = notificationModel.SendTo.ArrayFromCommaDelimitedStrings().ToList();
            var mode = new NotificationApplicationDTO
            {
                ApplicationId = id,
                Status = (WorkflowBusStatus)currentstatus,
            };
            string[] properties = { "ApplicationId", "Status" };
            if (currentstatus == 2) //通过审核
            {
                bool isok = false;
                if (notificationModel.viewName == "SMS") //发送短信
                {
                    //var text = ClearHtml(NotificationModel.Content);
                    var text = notificationModel.Content.HtmlToTxt();
                    isok = SendSMS(sendtoList, text, sendUserId, names, DisplayName);
                }
                else if (notificationModel.viewName == "Email") //发送邮箱
                {
                    var ccAfter = new List<string>();
                    if (!string.IsNullOrWhiteSpace(notificationModel.CcTo) && notificationModel.CcTo != "\0")
                    {
                        ccAfter = LoadCCList(notificationModel.CcTo).Distinct().ToList();
                    }

                    //var text = ClearHtml(NotificationModel.Content);
                    //var text = NotificationModel.Content.HtmlToTxt();
                    isok = SendEmail(sendtoList, notificationModel.Title, notificationModel.Content, ccAfter, sendUserId,
                        names, DisplayName);
                }
                if (isok)
                {
                    bl = SaveApplication(mode, properties);
                }
            }
            else if (currentstatus == 3) //未通过审核
            {
                bl = SaveApplication(mode, properties);
            }
            return bl;
        }

        /// <summary>
        /// 获取选中的抄送人 + List<string> LoadCCList(string emails)
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        private List<string> LoadCCList(string emails)
        {
            List<string> emailList = emails.Split(',').ToList();
            List<string> ccList = new List<string>();
            foreach (var email in emailList)
            {
                var trimedEmail = email.Trim();
                ccList.Add(trimedEmail);
            }
            if (string.IsNullOrEmpty(emails))
            {
                ccList = null;
            }
            return ccList;
        }

        /// <summary>
        /// 发送短信 - JsonResult SendSMS(List<string> phoneList, string contents, List<CustomerInfoDTO> customerInfoList)
        /// </summary>
        /// <param name="phoneList"></param>
        /// <param name="contents"></param>
        /// <param name="currentUserDisplayName"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        private bool SendSMS(List<string> phoneList, string contents, string currentUserId, string names,
            string DisplayName)
        {
            var phones = new List<long>();
            foreach (var no in phoneList)
            {
                long phoneNum;
                if (!string.IsNullOrEmpty(no)
                    && long.TryParse(no, out phoneNum))
                {
                    phones.Add(phoneNum);
                }
            }
            var smsInfo = new SmsInfo()
            {
                Tenant = base.Tenant.TenantName,
                Type = SmsType.Marketing,
                Phone = phones,
                SmsContent = contents,
            };
            var isSucceed = new StorageQueueService().InsertSmsQueue(smsInfo);
            InsertTrackingLog("SMS", isSucceed, contents, DisplayName, currentUserId, names, phoneList, null, null);
            return isSucceed;
        }

        /// <summary>
        /// 发送邮件 - JsonResult SendEmail(List<string> emailList, string subject, string contents, List<CustomerInfoDTO> customerInfoList, List<string> ccList)
        /// </summary>
        /// <param name="emailList"></param>
        /// <param name="subject"></param>
        /// <param name="contents"></param>
        /// <param name="ccList"></param>
        /// <param name="currentUserDisplayName"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        private bool SendEmail(List<string> emailList, string subject, string contents, List<string> ccList,
            string currentUserId, string names, string DisplayName)
        {
            var needSendMails = emailList.Where(m => !string.IsNullOrEmpty(m)).Distinct().ToList();

            var emailInfo = new EmailInfo()
            {
                UserId = currentUserId,
                Tenant = base.Tenant.TenantName,
                EmailTitle = subject,
                EmailBody = contents,
                SendTo = needSendMails,
                CcTo = ccList,
            };
            var isSucceed = new StorageQueueService().InsertEmailQueue(emailInfo);
            //var isSucceed = (new TenantStorageQueueService(base.Tenant)).InserEmailQueue(mail);
            InsertTrackingLog("Email", isSucceed, contents, DisplayName, currentUserId, names, null, needSendMails,
                ccList);
            return isSucceed;
        }

        /// <summary>
        /// 存入发送记录 - InsertTrackingLog(string viewName, bool isSucceed, string contents, List<string> phoneList = null, List<string> emailList = null, List<string> ccList = null)
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="isSucceed"></param>
        /// <param name="contents"></param>
        /// <param name="currentUserId"></param>
        /// <param name="phoneList"></param>
        /// <param name="emailList"></param>
        /// <param name="ccList"></param>
        /// <param name="currentUserDisplayName"></param>
        private void InsertTrackingLog(string viewName, bool isSucceed, string contents, string DisplayName,
            string currentUserId, string names, List<string> phoneList = null, List<string> emailList = null,
            List<string> ccList = null)
        {
            switch (viewName)
            {
                case "SMS":
                    var smsModel = new CustomerTracingLog
                    {
                        OperateDate = DateTime.UtcNow,
                        Operator = DisplayName,
                        OperatorId = currentUserId,
                        Type = isSucceed ? ProcessLogType.Success : ProcessLogType.Failure,

                        TracingType = TracingType.SmsNotify,
                        CustomerName = names,
                        SendTo = names,
                        Remark = contents,
                        //Remark = string.Format(
                        //"员工（{0}）于{1}发送短信通知给客户——状态：{2}，客户手机号码列表（{3}），短信内容：{4}。",
                        //currentUserDisplayName, DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"), isSucceed ? "成功" : "失败", string.Join(", ", phoneList), contents)

                    };
                    _customerTracingLogRepository.Add(smsModel);

                    break;
                case "Email":

                    var emailModel = new CustomerTracingLog
                    {
                        OperateDate = DateTime.UtcNow,
                        Operator = DisplayName,
                        OperatorId = currentUserId,
                        Type = isSucceed ? ProcessLogType.Success : ProcessLogType.Failure,
                        TracingType = TracingType.EmailNotify,
                        CustomerName = names,
                        SendTo = names,
                        Remark = contents,
                        //Remark = string.Format(
                        //"员工（{0}）于{1}发送邮件通知给客户——状态：{2}，客户邮箱列表（{3}），邮件内容：{4}。",
                        //currentUserDisplayName, DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"), isSucceed ? "成功" : "失败", string.Join(", ", emailList), contents)


                    };
                    _customerTracingLogRepository.Add(emailModel);

                    if (ccList != null && ccList.Any())
                    {
                        var ccModel = new CustomerTracingLog
                        {
                            OperateDate = DateTime.UtcNow,
                            Operator = DisplayName,
                            OperatorId = currentUserId,
                            Type = isSucceed ? ProcessLogType.Success : ProcessLogType.Failure,
                            TracingType = TracingType.EmailNotify,
                            CustomerName = names,
                            SendTo = names,
                            Remark = contents,
                            //Remark = string.Format(
                            //    "员工（{0}）于{1}抄送邮件通知给客户——状态：{2}，客户邮箱列表（{3}），邮件内容：{4}。",
                            //    currentUserDisplayName, DateTime.UtcNow.ToLocalDateTimeStr("yyyy-MM-dd HH:mm:ss"),
                            //    isSucceed ? "成功" : "失败", string.Join(", ", ccList), contents)

                        };
                        if (!ccModel.IsNullOrEmpty())
                        {
                            _customerTracingLogRepository.Add(ccModel);
                        }
                    }
                    break;
            }

        }

        #endregion

        #endregion

        /// <summary>
        /// 查询当前操作人在其权限内可以操作的客户的Id集合
        /// </summary>
        /// <param name="currentUserId">当前操作人</param>
        /// <param name="roleList">当前操作人的角色集合</param>
        /// <returns></returns>
        public List<int> GetCurrentUserCanOperateCustomerIds(string currentUserId, List<string> roleList)
        {
            //var user = _userRepository.FindByIdIncludeOrganizations(currentUserId);
            var userOrg = AccountApiService.LoadOrganizationsWithUsersByUserId(currentUserId).Result;
            Expression<Func<CustomerInfo, bool>> predicateCustomer = m => !m.IsDeleted;

            //所有下级部门Id（循环下下级）
            var orgIds = userOrg.Select(o => o.Id).ToList();
            //var orgs = _organizationRepository.GetAllOrganizationsByParentOrganizationIds(orgIds);
            var orgs = AccountApiService.LoadOrgTreesByOrgIds(orgIds).Result;
            var organizationIds = orgs.Select(o => (int?)o.Id).ToList();

            //除了企业最高组织，其他只能看自己部门的（不能看推送过来的人）
            if (userOrg.Count(m => m.Level == 1) <= 0 && !string.IsNullOrWhiteSpace(currentUserId))
            {
                //var organizations = _organizationRepository.GetAllOrganizationsWithUsers(o => !o.IsDeleted);
                var organizations = AccountApiService.LoadAllOrganization().Result;
                var childOrgnizationIds = Util.DataPermitUtil.LoadCurrentUserRelationOrganizationIds(AccountApiService, currentUserId, false, false, true).Result;
                var selfOrganizationIds = Util.DataPermitUtil.LoadCurrentUserRelationOrganizationIds(AccountApiService, currentUserId, true, false).Result;
                //自己是末级部门
                if (selfOrganizationIds.Any() && !selfOrganizationIds.Any(o => organizations.Any(p => p.ParentId == o)))
                {
                    predicateCustomer =
                        predicateCustomer.And(
                            m =>
                                m.CustomerManagers.Any(o => o.IsInSeas == null && o.CustomerManagerId == currentUserId));
                }
                //所有下级是末级部门
                else if (childOrgnizationIds.Any() &&
                         !childOrgnizationIds.Any(o => organizations.Any(p => p.ParentId == o)))
                {
                    if (roleList.Contains(RoleConstants.SaleRoleId) || roleList.Contains(RoleConstants.SaleManagerRoleId))
                    {
                        //客户经理角色
                        var selfAndSblingsOrgIds = Util.DataPermitUtil.LoadCurrentUserRelationOrganizationIds(AccountApiService, currentUserId).Result;
                        predicateCustomer =
                            predicateCustomer.And(
                                m =>
                                    m.CustomerManagers.All(o => o.IsInSeas == null) &&
                                    //m.CustomerManagers.Any(o => selfAndSblingsOrgIds.Contains(o.OrganizationId) ||
                                    //                            organizationIds.Contains(o.OrganizationId)));
                                    m.CustomerManagers.Any(o => o.OrganizationId.HasValue 
                                                    && selfAndSblingsOrgIds.Contains(o.OrganizationId.Value) 
                                                    || organizationIds.Contains(o.OrganizationId.Value)));
                    }
                    else
                    {
                        predicateCustomer =
                            predicateCustomer.And(
                                m =>
                                    m.CustomerManagers.All(o => o.IsInSeas == null) &&
                                    m.CustomerManagers.Any(
                                        o =>
                                            o.CustomerManagerId == currentUserId ||
                                            organizationIds.Contains(o.OrganizationId)));
                    }
                }
                else
                {
                    if (roleList.Contains(RoleConstants.SaleRoleId) || roleList.Contains(RoleConstants.SaleManagerRoleId))
                    {
                        //客服经理角色
                        var selfAndSblingsOrgIds = Util.DataPermitUtil.LoadCurrentUserRelationOrganizationIds(AccountApiService, currentUserId).Result;
                        predicateCustomer =
                            predicateCustomer.And(
                                m =>
                                    m.CustomerManagers.Any(
                                        o =>
                                        //selfAndSblingsOrgIds.Contains(o.OrganizationId) ||
                                        //    organizationIds.Contains(o.OrganizationId)));
                                        o.OrganizationId.HasValue 
                                        && selfAndSblingsOrgIds.Contains(o.OrganizationId.Value) 
                                        || organizationIds.Contains(o.OrganizationId.Value)));
                    }
                    else
                    {
                        predicateCustomer =
                            predicateCustomer.And(
                                m =>
                                    m.CustomerManagers.Any(
                                        o =>
                                            o.CustomerManagerId == currentUserId ||
                                            organizationIds.Contains(o.OrganizationId)));
                    }
                }
            }
            //具有最高权限的人
            Expression<Func<CustomerInfo, bool>> predicateCustomer1 = m => true;
            if (userOrg.Count(m => m.Level == 1) > 0)
            {
                predicateCustomer1 =
                    predicateCustomer1.And(m => m.CustomerManagers.Any(n => string.IsNullOrEmpty(n.CustomerManagerId)));
                predicateCustomer = predicateCustomer.Or(predicateCustomer1);
            }

            //能够查看联系人的客户Ids
            var customerIds = _customerInfoRepository.FindAll(predicateCustomer).Select(o => o.CustomerId).ToList();
            return customerIds;
        }

    }
}
