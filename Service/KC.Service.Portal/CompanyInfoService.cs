using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Framework.Extension;
using KC.Database.EFRepository;
using KC.Service.EFService;
using KC.Service.DTO.Portal;
using KC.Model.Portal;
using KC.Database.IRepository;
using KC.DataAccess.Portal.Repository;
using KC.Service.DTO;
using KC.Enums.Portal;
using KC.Framework.Tenant;
using KC.Service.Portal.WebApiService;
using KC.Service.DTO.Admin;
using Microsoft.Extensions.Logging;

namespace KC.Service.Portal
{
    public interface ICompanyInfoService : IEFService
    {
        #region 企业基本信息
        Task<CompanyInfoDTO> GetCompanyInfoAsync();
        Task<CompanyInfoDTO> GetCompanyDetailInfoAsync();
        Task<bool> SaveCompanyInfo(CompanyInfoDTO model, string currentUserId, string currentUserName);
        #endregion

        #region 企业认证信息
        /// <summary>
        /// 获取企业认证信息
        /// </summary>
        Task<CompanyAuthenticationDTO> GetCompanyAuthAsync();

        Task<bool> SaveComAuthentication(CompanyAuthenticationDTO model);
        #endregion

        #region 企业银行账号
        Task<PaginatedBaseDTO<CompanyAccountDTO>> GetPaginatedCompanyAccountsByFilterAsync(int pageIndex, int pageSize, string name, string bankNumber, BankAccountType? type);
        Task<CompanyAccountDTO> GetCompanyAccountByIdAsync(int id);
        Task<bool> SaveCompanyAccountAsync(CompanyAccountDTO model, string currentUserId, string currentUserName);
        Task<bool> SoftRemoveCompanyAccountAsync(int id, string currentUserId, string currentUserName);
        #endregion

        #region 企业联系人

        Task<PaginatedBaseDTO<CompanyContactDTO>> GetPaginatedCompanyContactsByFilterAsync(int pageIndex, int pageSize, string name, string phone, BusinessType? type);
        Task<CompanyContactDTO> GetCompanyContactByIdAsync(int id);
        Task<bool> SaveCompanyContactAsync(CompanyContactDTO model);
        Task<bool> SaveCompanyContactsAsync(List<CompanyContactDTO> models);
        Task<bool> SoftRemoveCompanyContactAsync(int id);

        List<string> LoadAllContactUserIds();
        #endregion

        #region 企业地址
        Task<PaginatedBaseDTO<CompanyAddressDTO>> GetPaginatedCompanyAddresssByFilterAsync(int pageIndex, int pageSize, string name, string contactName, AddressType? type);
        Task<CompanyAddressDTO> GetCompanyAddressByIdAsync(int id);
        Task<bool> SaveCompanyAddressAsync(CompanyAddressDTO model);
        Task<bool> SoftRemoveCompanyAddressAsync(int id, string currentUserId, string currentUserName);
        #endregion

        #region 企业信息维护日志
        Task<PaginatedBaseDTO<CompanyProcessLogDTO>> GetPaginatedCompanyProcessLogsByFilterAsync(int pageIndex, int pageSize, string operatorName, DateTime? startDate, DateTime? endDate);
        Task<bool> RemoveCompanyProcessLogAsync(int id);
        #endregion
    }

    public class CompanyInfoService : EFServiceBase, ICompanyInfoService
    {
        private readonly IMapper _mapper;

        #region Repository
        /// <summary>
        /// ComPortalUnitOfWorkContext </br>
        ///     use ComPortalUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private readonly ITenantSimpleApiService _tenantApiService;

        private ICompanyInfoRepository _companyInfoRepository;
        private ICompanyAuthenticationRepository _companyAuthenticationRepository;
        private IDbRepository<CompanyAccount> _companyAccountRepository;
        private IDbRepository<CompanyContact> _companyContactRepository;
        private IDbRepository<CompanyAddress> _companyAddressRepository;
        private IDbRepository<CompanyProcessLog> _companyProcessLogRepository;

        public CompanyInfoService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,
            ITenantSimpleApiService tenantApiService,

            ICompanyInfoRepository companyInfoRepository,
            ICompanyAuthenticationRepository companyAuthenticationRepository,
            IDbRepository<CompanyAccount> companyAccountRepository,
            IDbRepository<CompanyContact> companyContactRepository,
            IDbRepository<CompanyAddress> companyAddressRepository,
            IDbRepository<CompanyProcessLog> companyProcessLogRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            ILogger<CompanyInfoService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;

            _tenantApiService = tenantApiService;

            _companyInfoRepository = companyInfoRepository;
            _companyAuthenticationRepository = companyAuthenticationRepository;
            _companyAccountRepository = companyAccountRepository;
            _companyContactRepository = companyContactRepository;
            _companyAddressRepository = companyAddressRepository;
            _companyProcessLogRepository = companyProcessLogRepository;
        }
        #endregion

        #region 企业基本信息
        /// <summary>
        /// 获取企业基本信息
        /// </summary>
        public async Task<CompanyInfoDTO> GetCompanyInfoAsync()
        {
            var data = await _companyInfoRepository.GetByFilterAsync(m => true);
            return _mapper.Map<CompanyInfoDTO>(data);
        }
        /// <summary>
        /// 获取企业详情信息
        /// </summary>
        public async Task<CompanyInfoDTO> GetCompanyDetailInfoAsync()
        {
            var data = await _companyInfoRepository.GetCompanyDetailInfoAsync();
            if (data == null)
                return new CompanyInfoDTO();
            return _mapper.Map<CompanyInfoDTO>(data);
        }
        /// <summary>
        /// 保存企业详基本信息
        /// </summary>
        public async Task<bool> SaveCompanyInfo(CompanyInfoDTO model, string currentUserId, string currentUserName)
        {
            var md = _mapper.Map<CompanyInfo>(model);
            if (!model.IsEditMode)
            {
                await _companyInfoRepository.AddAsync(md, false);
                var log = new CompanyProcessLog()
                {
                    CompanyCode = model.CompanyCode,
                    CompanyName = model.CompanyName,
                    OperatorId = currentUserId,
                    Operator = currentUserName,
                    OperateDate = DateTime.UtcNow,
                    Type = Framework.Base.ProcessLogType.Success,
                    Remark = "创建企业基本信息成功"
                };
                await _companyProcessLogRepository.AddAsync(log, false);
            }
            else
            {
                await _companyInfoRepository.ModifyAsync(md, null, false);
                var editlog = new CompanyProcessLog()
                {
                    CompanyCode = model.CompanyCode,
                    CompanyName = model.CompanyName,
                    OperatorId = currentUserId,
                    Operator = currentUserName,
                    OperateDate = DateTime.UtcNow,
                    Type = Framework.Base.ProcessLogType.Success,
                    Remark = "编辑企业基本信息成功"
                };
                await _companyProcessLogRepository.AddAsync(editlog, false);
            }

            var success = await _unitOfWorkContext.CommitAsync() > 0;
            if (success)
            {
                // 删除前端缓存
                var tenantName = Tenant.TenantName;
                var apiMethod1 = typeof(IFrontWebInfoService).GetMethod("GetCompanyDetailInfoAsync");
                var cacheKey1 = Util.CacheKeyUtil.CacheKeyGenerator(tenantName, apiMethod1, null);
                if (!string.IsNullOrEmpty(cacheKey1))
                    Service.CacheUtil.RemoveCache(cacheKey1);

                var tenant = new TenantSimpleDTO()
                {
                    TenantName = Tenant.TenantName,
                    TenantDisplayName = model.CompanyName,
                    TenantLogo = model.CompanyLogo,
                    TenantIntroduction = model.Introduction,
                    BusinessModel = Enum.Parse<KC.Framework.Base.BusinessModel> (model.BusinessModel.ToString()),
                    IndustryId = model.IndustryId,
                    IndustryName = model.IndustryName,
                    ContactName = model.ContactName,
                    ContactPhone = model.ContactPhone,
                    ContactEmail = model.ContactEmail,
                };
                var result = _tenantApiService.UpdateTenantUserBasicInfo(tenant);
                if (!result.Result)
                {
                    Logger.LogError("调用租户同步接口：UpdateTenantUserBasicInfo，失败。");
                }

            }

            return success;
        }
        #endregion

        #region 企业认证信息
        /// <summary>
        /// 获取企业认证信息
        /// </summary>
        public async Task<CompanyAuthenticationDTO> GetCompanyAuthAsync()
        {
            var data = await _companyAuthenticationRepository.GetComAuthenticationAsync();
            var model = _mapper.Map<CompanyAuthenticationDTO>(data);
            if (data != null && data.CompanyInfo != null)
            {
                model.CompanyName = data.CompanyInfo.CompanyName;
            }
            return model;
        }


        /// <summary>
        /// 保存企业详基本信息
        /// </summary>
        public async Task<bool> SaveComAuthentication(CompanyAuthenticationDTO model)
        {
            var company = await _companyInfoRepository.GetByFilterAsync(m => true && model.CompanyCode.Equals(m.CompanyCode));
            if (company == null)
                throw new ArgumentException("未找到该组织的注册信息。");

            model.Status = Framework.Base.WorkflowBusStatus.AuditPending;
            await _companyInfoRepository.ModifyAsync(company, new string[] { "Status" }, false);

            var md = _mapper.Map<CompanyAuthentication>(model);
            md.CompanyCode = company.CompanyCode;
            if (model.IsEditMode)
            {
                await _companyAuthenticationRepository.ModifyAsync(md, null, false);
            }
            else
            {
                await _companyAuthenticationRepository.AddAsync(md, false);
            }

            var success = await _unitOfWorkContext.CommitAsync() > 0;
            if (success)
            {
                var result = _tenantApiService.SaveTenantUserAuthInfo(model);
                if (!result.Result)
                {
                    Logger.LogError("调用租户同步接口：SaveTenantUserAuthInfo，失败。");
                }
            }

            return success;
        }
        #endregion

        #region 银行账号
        /// <summary>
        /// 银行账号分页列表
        /// </summary>
        public async Task<PaginatedBaseDTO<CompanyAccountDTO>> GetPaginatedCompanyAccountsByFilterAsync(int pageIndex, int pageSize, string name, string bankNumber, BankAccountType? type)
        {
            Expression<Func<CompanyAccount, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrEmpty(name))
            {
                predicate = predicate.And(m => m.BankName.Contains(name));
            }
            if (!string.IsNullOrEmpty(bankNumber))
            {
                predicate = predicate.And(m => m.BankNumber.Contains(bankNumber));
            }
            if (type.HasValue)
            {
                predicate = predicate.And(m => m.BankType == type.Value);
            }

            var data = await _companyAccountRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.ModifiedDate, false);
            var total = data.Item1;
            var rows = _mapper.Map<List<CompanyAccountDTO>>(data.Item2);
            return new PaginatedBaseDTO<CompanyAccountDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<CompanyAccountDTO> GetCompanyAccountByIdAsync(int id)
        {
            var data = await _companyAccountRepository.GetByIdAsync(id);
            return _mapper.Map<CompanyAccountDTO>(data);
        }
        /// <summary>
        /// 保存银行账号信息
        /// </summary>
        public async Task<bool> SaveCompanyAccountAsync(CompanyAccountDTO model, string currentUserId, string currentUserName)
        {
            var md = _mapper.Map<CompanyAccount>(model);
            if (!model.IsEditMode)
            {
                await _companyAccountRepository.AddAsync(md, false);
                var log = new CompanyProcessLog()
                {
                    CompanyCode = model.CompanyCode,
                    CompanyName = model.CompanyName,
                    OperatorId = currentUserId,
                    Operator = currentUserName,
                    OperateDate = DateTime.UtcNow,
                    Type = Framework.Base.ProcessLogType.Success,
                    Remark = "创建企业银行账号信息成功"
                };
                await _companyProcessLogRepository.AddAsync(log, false);
                return await _unitOfWorkContext.CommitAsync() > 0;
            }

            var account = await _companyAccountRepository.GetByIdAsync(md.Id);
            await _companyAccountRepository.ModifyAsync(md, null, false);
            if(!account.AccountName.Equals(md.AccountName)
                || !account.BankName.Equals(md.BankName)
                || !account.BankNumber.Equals(md.BankNumber))
            {
                var editlog = new CompanyProcessLog()
                {
                    CompanyCode = model.CompanyCode,
                    CompanyName = model.CompanyName,
                    OperatorId = currentUserId,
                    Operator = currentUserName,
                    OperateDate = DateTime.UtcNow,
                    Type = Framework.Base.ProcessLogType.Success,
                    Remark = string.Format("编辑企业银行账号信息成功, 原账户【{0}|{1}|{2}】更改为【{3}|{4}|{5}】,", account.AccountName, account.BankName, account.BankNumber, md.AccountName, md.BankName, md.BankNumber)
                };
                await _companyProcessLogRepository.AddAsync(editlog, false);
            }
            
            return await _unitOfWorkContext.CommitAsync() > 0;
        }
        /// <summary>
        /// 删除银行账号信息
        /// </summary>
        public async Task<bool> SoftRemoveCompanyAccountAsync(int id, string currentUserId, string currentUserName)
        {
            var account = await _companyAccountRepository.GetByIdAsync(id);
            await _companyAccountRepository.SoftRemoveAsync(account, false);
            var log = new CompanyProcessLog()
            {
                OperatorId = currentUserId,
                Operator = currentUserName,
                OperateDate = DateTime.UtcNow,
                Type = Framework.Base.ProcessLogType.Success,
                Remark = string.Format("成功删除银行账户【{0}: {1}】，备注：数据已备份，可通知管理员进行数据恢复或重新创建进行恢复", account.BankName, account.BankNumber)
            };
            await _companyProcessLogRepository.AddAsync(log, false);
            return await _unitOfWorkContext.CommitAsync() > 0;
        }
        #endregion

        #region 企业联系人

        public async Task<PaginatedBaseDTO<CompanyContactDTO>> GetPaginatedCompanyContactsByFilterAsync(int pageIndex, int pageSize, string name, string phone, BusinessType? type)
        {
            Expression<Func<CompanyContact, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrEmpty(name))
            {
                predicate = predicate.And(m => m.ContactName.Contains(name));
            }
            if (!string.IsNullOrEmpty(phone))
            {
                predicate = predicate.And(m => m.ContactPhoneNumber.Contains(phone));
            }
            if (type.HasValue)
            {
                predicate = predicate.And(m => m.BusinessType == type.Value);
            }

            var data = await _companyContactRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.ModifiedDate, false);
            var total = data.Item1;
            var rows = _mapper.Map<List<CompanyContactDTO>>(data.Item2);
            return new PaginatedBaseDTO<CompanyContactDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<CompanyContactDTO> GetCompanyContactByIdAsync(int id)
        {
            var data = await _companyContactRepository.GetByIdAsync(id);
            return _mapper.Map<CompanyContactDTO>(data);
        }

        public async Task<bool> SaveCompanyContactAsync(CompanyContactDTO model)
        {
            var md = _mapper.Map<CompanyContact>(model);
            return model.Id == 0
                ? await _companyContactRepository.AddAsync(md)
                : await _companyContactRepository.ModifyAsync(md, null);
        }

        public async Task<bool> SaveCompanyContactsAsync(List<CompanyContactDTO> models)
        {
            var md = _mapper.Map<List<CompanyContact>>(models);
            return await _companyContactRepository.AddAsync(md);
        }

        public async Task<bool> SoftRemoveCompanyContactAsync(int id)
        {
            return await _companyContactRepository.SoftRemoveAsync(m => m.Id == id, true) > 0;
        }

        public List<string> LoadAllContactUserIds()
        {
            var users = _companyContactRepository.FindAll(m => !m.IsDeleted);
            return users.Select(m => m.ContactId).ToList();
        }
        #endregion

        #region 企业地址
        /// <summary>
        /// 企业地址分页列表
        /// </summary>
        public async Task<PaginatedBaseDTO<CompanyAddressDTO>> GetPaginatedCompanyAddresssByFilterAsync(int pageIndex, int pageSize, string name, string contactName, AddressType? type)
        {
            Expression<Func<CompanyAddress, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrEmpty(name))
            {
                predicate = predicate.And(m => m.Address.Contains(name));
            }
            if (!string.IsNullOrEmpty(contactName))
            {
                predicate = predicate.And(m => m.ContactName.Contains(contactName));
            }
            if (type.HasValue)
            {
                predicate = predicate.And(m => m.AddressType == type.Value);
            }

            var data = await _companyAddressRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.ModifiedDate, false);
            var total = data.Item1;
            var rows = _mapper.Map<List<CompanyAddressDTO>>(data.Item2);
            return new PaginatedBaseDTO<CompanyAddressDTO>(pageIndex, pageSize, total, rows);
        }

        public async Task<CompanyAddressDTO> GetCompanyAddressByIdAsync(int id)
        {
            var data = await _companyContactRepository.GetByIdAsync(id);
            return _mapper.Map<CompanyAddressDTO>(data);
        }
        /// <summary>
        /// 保存企业地址信息
        /// </summary>
        public async Task<bool> SaveCompanyAddressAsync(CompanyAddressDTO model)
        {
            var md = _mapper.Map<CompanyAddress>(model);
            return model.Id == 0
                ? await _companyAddressRepository.AddAsync(md)
                : await _companyAddressRepository.ModifyAsync(md, null);
        }
        /// <summary>
        /// 删除企业地址信息
        /// </summary>
        public async Task<bool> SoftRemoveCompanyAddressAsync(int id, string currentUserId, string currentUserName)
        {
            var account = await _companyAddressRepository.GetByIdAsync(id);
            await _companyAddressRepository.SoftRemoveAsync(account, false);
            var log = new CompanyProcessLog()
            {
                OperatorId = currentUserId,
                Operator = currentUserName,
                OperateDate = DateTime.UtcNow,
                Type = Framework.Base.ProcessLogType.Success,
                Remark = string.Format("成功删除企业地址信息【{0}】，备注：数据已备份，可通知管理员进行数据恢复或重新创建进行恢复", account.ProvinceName +  account.CityName + account.Address)
            };
            await _companyProcessLogRepository.AddAsync(log, false);
            return await _unitOfWorkContext.CommitAsync() > 0;
        }
        #endregion

        #region 企业信息维护日志
        /// <summary>
        /// 企业信息维护日志分页列表
        /// </summary>
        public async Task<PaginatedBaseDTO<CompanyProcessLogDTO>> GetPaginatedCompanyProcessLogsByFilterAsync(int pageIndex, int pageSize, string operatorName, DateTime? startDate, DateTime? endDate)
        {
            Expression<Func<CompanyProcessLog, bool>> predicate = m => true;
            if (!string.IsNullOrEmpty(operatorName))
            {
                predicate = predicate.And(m => m.Operator.Contains(operatorName));
            }
            if (startDate.HasValue)
            {
                predicate = predicate.And(m => m.OperateDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                predicate = predicate.And(m => m.OperateDate <= endDate.Value);
            }

            var data = await _companyProcessLogRepository.FindPagenatedListWithCountAsync(pageIndex, pageSize, predicate, m => m.OperateDate, false);
            var total = data.Item1;
            var rows = _mapper.Map<List<CompanyProcessLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<CompanyProcessLogDTO>(pageIndex, pageSize, total, rows);
        }

        /// <summary>
        /// 删除企业信息维护日志
        /// </summary>
        public async Task<bool> RemoveCompanyProcessLogAsync(int id)
        {
            return await _companyProcessLogRepository.RemoveAsync(m => m.ProcessLogId == id, true) > 0;
        }
        #endregion
    }
}
