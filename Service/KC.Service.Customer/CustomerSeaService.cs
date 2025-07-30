using System;
using System.Collections.Generic;
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
using KC.Model.Customer;
using KC.Service.EFService;
using KC.Service.WebApiService.Business;
using KC.Service.DTO;
using KC.Service.DTO.Customer;
using KC.Database.IRepository;
using KC.Service.Customer.WebApiService.Business;
using System.Threading.Tasks;
using KC.Framework.Tenant;

namespace KC.Service.Customer
{
    public interface ICustomerSeaService : IEFService
    {
        #region 客户公海
        Task<PaginatedBaseDTO<CustomerSeasInfoDTO>> GetPaginatedCustomerSeasByFilterAsync(int pageIndex, int pageSize,
           string searchKey, string searchValue, CompanyType? customerType, string currentUserId, string operatorName, int? timeSpan);

        bool TransferCutomerToSeas(List<int> customerIds, string operatorId, string operatorName);
        bool PickCustomerFromSeas(List<int> ids, List<int> customerIds, string operatorId, string operatorName);
        bool CanTransferCutomerToSeas(string operatorId);

        #endregion

        #region Customer Extend Info Provider

        List<CustomerExtInfoProviderDTO> GetAllCustomerExtInfoProviers(string searchKey, string searchValue);
        CustomerExtInfoProviderDTO GetCustomerExtInfoProvidersById(int id);
        bool SaveCustomerExtInfoProvider(CustomerExtInfoProviderDTO model);
        bool SoftRemoveCustomerExtInfoProvider(int id);

        #endregion
    }

    public class CustomerSeaService : EFServiceBase, ICustomerSeaService
    {
        private readonly IMapper _mapper;

        #region Db Repository

        protected EFUnitOfWorkContextBase CustomerUnitOfWorkContext { get; private set; }
        //protected EFUnitOfWorkContextBase AccountUnitOfWorkContext { get; private set; }

        private readonly ICustomerApiService _customerApiService;
        private readonly IDictionaryApiService _dictionaryApiService;

        private readonly ICustomerSeasRepository _customerSeasRepository;
        private readonly ICustomerManagerRepository _customerManagerRepository;
        private readonly IDbRepository<CustomerExtInfoProvider> _customerExtInfoProviderRepository;

        private readonly IAccountApiService AccountApiService;
        #endregion

        public CustomerSeaService(
            Tenant tenant,
            IMapper mapper,
            IAccountApiService accountApiService,
            ITenantUserApiService tenantUserApiService,
            IConfigApiService configApiService,
            IDictionaryApiService dictionaryApiService,
            ICustomerApiService customerApiService,
            EFUnitOfWorkContextBase unitOfWorkContext,
            
            ICustomerSeasRepository customerSeasRepository,
            ICustomerManagerRepository customerManagerRepository,
            IDbRepository<CustomerExtInfoProvider> customerExtInfoProviderRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<CustomerService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            CustomerUnitOfWorkContext = unitOfWorkContext;
            _customerApiService = customerApiService;
            _dictionaryApiService = dictionaryApiService;

            _customerSeasRepository = customerSeasRepository;
            _customerManagerRepository = customerManagerRepository;
            _customerExtInfoProviderRepository = customerExtInfoProviderRepository;

            AccountApiService = accountApiService;
        }

        #region 客户公海

        /// <summary>
        /// 公海列表
        /// </summary>
        public async Task<PaginatedBaseDTO<CustomerSeasInfoDTO>> GetPaginatedCustomerSeasByFilterAsync(int pageIndex, int pageSize,
            string searchKey, string searchValue, CompanyType? customerType, string currentUserId, string operatorName,
            int? timeSpan)
        {
            Expression<Func<CustomerSeas, bool>> predicate = m => true;

            var includeOrganizationIds = new List<int>();

            //所有部门
            //var organizations = _organizationRepository.GetAllOrganizationsWithUsers(o => !o.IsDeleted);
            var organizations = await AccountApiService.LoadAllOrganization();
            if (organizations.Any())
            {
                //保证所有下级部门都是末级部门
                var childOrgnizationIds = await Util.DataPermitUtil.LoadCurrentUserRelationOrganizationIds(AccountApiService, currentUserId, false, false, true);
                if (organizations.Any() && !childOrgnizationIds.Any(o => organizations.Any(p => p.ParentId == o)))
                {
                    //看下级部门客户公海
                    includeOrganizationIds.AddRange(childOrgnizationIds);
                }

                //保证自己必须是末级部门
                var selfOrganizationIds = await Util.DataPermitUtil.LoadCurrentUserRelationOrganizationIds(AccountApiService, currentUserId, true, false);
                if (organizations.Any() && !selfOrganizationIds.Any(o => organizations.Any(p => p.ParentId == o)))
                {
                    //看同级部门和同一部门公海
                    var selfAndSiblingsOrganizationIds = await Util.DataPermitUtil.LoadCurrentUserRelationOrganizationIds(AccountApiService, currentUserId);
                    includeOrganizationIds.AddRange(selfAndSiblingsOrganizationIds);
                }
            }

            if (includeOrganizationIds.Any())
                predicate = predicate.And(m => m.OrganizationId.HasValue && includeOrganizationIds.Contains(m.OrganizationId.Value));

            if (customerType.HasValue)
            {
                predicate = predicate.And(m => m.CustomerInfo.CompanyType == customerType);
            }
            if (!string.IsNullOrEmpty(operatorName))
            {
                predicate = predicate.And(m => m.Operator.Contains(operatorName));
            }
            //if (timeSpan.HasValue)
            //{
            //    switch (timeSpan)
            //    {
            //        case 1:
            //            predicate = predicate.And(m => m.OperateDate >= DbFunctions.AddDays(DateTime.UtcNow, -7));
            //            break;
            //        case 2:
            //            predicate = predicate.And(m => m.OperateDate >= DbFunctions.AddMonths(DateTime.UtcNow, -1));
            //            break;
            //        case 3:
            //            predicate = predicate.And(m => m.OperateDate >= DbFunctions.AddMonths(DateTime.UtcNow, -3));
            //            break;
            //        case 4:
            //            predicate = predicate.And(m => m.OperateDate >= DbFunctions.AddMonths(DateTime.UtcNow, -6));
            //            break;
            //        case 5:
            //            predicate = predicate.And(m => m.OperateDate < DbFunctions.AddMonths(DateTime.UtcNow, -6));
            //            break;
            //    }
            //}
            if (!string.IsNullOrWhiteSpace(searchKey) && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey)
                {
                    case "CustomerName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.CustomerInfo.CustomerName.Contains(searchValue));
                        }
                        break;
                    //case "ContactPhoneNumber":
                    //    if (!string.IsNullOrWhiteSpace(searchValue))
                    //    {
                    //        predicate = predicate.And(m => m.CustomerInfo.ContactPhoneNumber.Contains(searchValue));
                    //    }
                    //    break;
                    case "ContactName":
                        if (!string.IsNullOrWhiteSpace(searchValue))
                        {
                            predicate = predicate.And(m => m.CustomerInfo.ContactName.Contains(searchValue));
                        }
                        break;
                }
            }
            var data = _customerSeasRepository.GetCustomerSeasInfo(predicate, true, pageIndex, pageSize);
            var total = data.Item1;
            var rows = _mapper.Map<List<CustomerSeasInfoDTO>>(data.Item2);
            return new PaginatedBaseDTO<CustomerSeasInfoDTO>(pageIndex, pageSize, total, rows);
        }

        /// <summary>
        /// 转移客户到公海
        /// </summary>
        /// <param name="customerIds">客户Id集合</param>
        /// <param name="operatorId">当前转移人Id</param>
        /// <param name="operatorName">当前转移人姓名</param>
        /// <returns></returns>
        public bool TransferCutomerToSeas(List<int> customerIds, string operatorId, string operatorName)
        {
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

            List<CustomerSeas> customerSeases = new List<CustomerSeas>();
            foreach (var targetCustomerManager in customerManagers)
            {
                if (targetCustomerManager.CustomerId.HasValue)
                {
                    CustomerSeas customerSeas = new CustomerSeas
                    {
                        CustomerId = targetCustomerManager.CustomerId.Value,
                        OperatorId = operatorId,
                        Operator = operatorName,
                        OperateDate = DateTime.UtcNow,
                        OrganizationId = targetCustomerManager.OrganizationId
                    };
                    customerSeases.Add(customerSeas);
                    //更改跟进人记录状态
                    targetCustomerManager.IsInSeas = true;
                }
            }
            if (!customerManagers.Any())
            {
                 throw new BusinessPromptException("只能转移跟进人是自己的客户！");
            }
            _customerManagerRepository.Modify(customerManagers, new[] {"IsInSeas"}, false);
            _customerSeasRepository.Add(customerSeases.ToList(), false);
            CustomerUnitOfWorkContext.Commit();
            return true;
        }

        /// <summary>
        /// 从公海中捕捞客户
        /// </summary>
        /// <param name="ids">公海客户数据Id</param>
        ///  <param name="customerIds">客户Ids</param>
        /// <param name="operatorId">当前操作人Id</param>
        /// <param name="operatorName">当前操作人姓名</param>
        /// <returns></returns>
        public bool PickCustomerFromSeas(List<int> ids, List<int> customerIds, string operatorId, string operatorName)
        {
            IList<CustomerSeas> customerSeases;
            if (customerIds != null)
            {
                Expression<Func<CustomerSeas, bool>> predicate = o => true;
                if (!string.IsNullOrEmpty(operatorId))
                {
                    predicate = predicate.And(o => o.OperatorId == operatorId);
                }
                if (customerIds != null && customerIds.Any())
                {
                    predicate = predicate.And(o => customerIds.Contains(o.CustomerId));
                }
                customerSeases = _customerSeasRepository.FindAll(predicate);
            }
            else
                customerSeases = _customerSeasRepository.FindAll(o => ids.Contains(o.Id));

            List<CustomerManager> modifiedCustomerManagerList = new List<CustomerManager>();
            List<CustomerManager> addedCustomerManagerList = new List<CustomerManager>();
            foreach (var customerSeas in customerSeases)
            {
                Expression<Func<CustomerManager, bool>> predicate = o => true;
                if (!string.IsNullOrEmpty(operatorId))
                {
                    predicate = predicate.And(o => o.CustomerManagerId == operatorId);
                }
                var custIds = new List<int> {customerSeas.CustomerId};
                if (custIds != null && custIds.Any())
                {
                    predicate = predicate.And(o => custIds.Contains(o.CustomerId.Value));
                }
                var modifiedCustomerManagers = _customerManagerRepository.GetCustomerManagersById(predicate);
                if (modifiedCustomerManagers.Any())
                {
                    modifiedCustomerManagers.ForEach(o => o.IsInSeas = null);
                    modifiedCustomerManagerList.AddRange(modifiedCustomerManagers);
                }
                else
                {
                    var userOrgs = AccountApiService.GetUserWithOrgsAndRolesByUserId(operatorId).Result;
                    var userOrgIds = AccountApiService.GetUserWithOrgsAndRolesByUserId(operatorId).Result.UserOrgIds.FirstOrDefault();
                    var addedCustomerManager = new CustomerManager
                    {
                        CustomerId = customerSeas.CustomerId,
                        CustomerManagerId = operatorId,
                        CustomerManagerName = operatorName,
                        OrganizationId = userOrgs == null || !userOrgs.UserOrgIds.Any() ? 0 : userOrgs.UserOrgIds.FirstOrDefault(),
                        OrganizationName = userOrgs == null || !userOrgs.UserOrgIds.Any() ? null : userOrgs.UserOrgNames.FirstOrDefault()
                    };
                    addedCustomerManagerList.Add(addedCustomerManager);
                }
            }
            if (modifiedCustomerManagerList.Any())
                _customerManagerRepository.Modify(modifiedCustomerManagerList, null, false);
            if (addedCustomerManagerList.Any())
                _customerManagerRepository.Add(addedCustomerManagerList, false);
            _customerSeasRepository.RemoveById(customerSeases, false);
            CustomerUnitOfWorkContext.Commit();
            return true;
        }

        /// <summary>
        /// 操作人是否可以转移客户到公海,是否可以从公海中捕捞客户（只有末级部门的人员才可以）
        /// </summary>
        /// <param name="operatorId">当前操作人Id</param>
        /// <returns></returns>
        public bool CanTransferCutomerToSeas(string operatorId)
        {
            var userOrgs = AccountApiService.LoadOrganizationsWithUsersByUserId(operatorId).Result;
            if (userOrgs == null && !userOrgs.Any())
                return false;

            //所有部门
            var organizations = AccountApiService.LoadAllOrganization().Result;
            if (organizations == null && !organizations.Any())
                return false;

            if (!userOrgs.Any(o => organizations.Any(p => p.ParentId == o.Id)))
                return true;
            return false;
        }
        
        #endregion

        #region Customer Extend Info Provider

        public List<CustomerExtInfoProviderDTO> GetAllCustomerExtInfoProviers(string searchKey, string searchValue)
        {
            Expression<Func<CustomerExtInfoProvider, bool>> predicate = m => !m.IsDeleted;

            if (!string.IsNullOrWhiteSpace(searchKey) && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (searchKey)
                {
                    case "Name":
                        predicate = predicate.And(m => m.Name.Contains(searchValue));
                        break;
                    case "ModifiedBy":
                        predicate = predicate.And(m => m.ModifiedBy.Contains(searchValue));
                        break;
                }
            }
            var data = _customerExtInfoProviderRepository.FindAll(predicate);
            return _mapper.Map<List<CustomerExtInfoProviderDTO>>(data);
        }

        public CustomerExtInfoProviderDTO GetCustomerExtInfoProvidersById(int id)
        {
            var data = _customerExtInfoProviderRepository.GetById(id);
            //var data = _customerExtInfoProviderRepository.Find(m => m.PropertyAttributeId == id && !m.IsDeleted).FirstOrDefault();
            return _mapper.Map<CustomerExtInfoProviderDTO>(data);
        }

        public bool SaveCustomerExtInfoProvider(CustomerExtInfoProviderDTO model)
        {
            var md = _mapper.Map<CustomerExtInfoProvider>(model);
            return model.PropertyAttributeId == 0
                ? _customerExtInfoProviderRepository.Add(md)
                : _customerExtInfoProviderRepository.Modify(md, null);
        }

        public bool SoftRemoveCustomerExtInfoProvider(int id)
        {
            return _customerExtInfoProviderRepository.SoftRemoveById(id);
        }

        #endregion
    }
}
