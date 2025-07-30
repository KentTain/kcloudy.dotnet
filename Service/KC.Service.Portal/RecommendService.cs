using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KC.Database.EFRepository;
using KC.Service.EFService;
using KC.Model.Portal;
using KC.DataAccess.Portal.Repository;
using KC.Enums.Portal;
using KC.Service.DTO;
using KC.Framework.Extension;
using KC.Service.DTO.Portal;
using KC.Framework.Tenant;
using KC.Service.WebApiService.Business;
using KC.Database.IRepository;

namespace KC.Service.Portal
{
    public interface IRecommendService : IEFService
    {
        PaginatedBaseDTO<RecommendInfoDTO> LoadPaginatedRecommendInfosByFilter(int pageIndex, int pageSize, int? categoryId, string name);

        #region 推荐企业
        PaginatedBaseDTO<RecommendCustomerDTO> FindPaginatedRecommendCustomers(int pageIndex, int pageSize, string name, RecommendStatus? Status);

        RecommendCustomerDTO GetRedCustomerById(int id);

        bool AuditRedCustomerById(int id, bool isApprove, string comment, string currentUserId, string currentUserDisplayName);
        bool TakeOffRedCustomerById(int id, string currentUserId, string currentUserDisplayName);
        bool RemoveRedCustomerById(int id, string currentUserId, string currentUserDisplayName);


        bool SaveRedCustomer(RecommendCustomerDTO model);

        PaginatedBaseDTO<RecommendCustomerLogDTO> FindPaginatedRecommendCustomerLogs(int pageIndex, int pageSize, string name);
        #endregion

        #region 推荐商品
        PaginatedBaseDTO<RecommendOfferingDTO> FindPaginatedRecommendOfferings(int pageIndex, int pageSize, string name, RecommendStatus? Status);

        RecommendOfferingDTO GetRedOfferingById(int id);

        bool AuditRedOfferingById(int id, bool isApprove, string comment, string currentUserId, string currentUserDisplayName);
        bool TakeOffRedOfferingById(int id, string currentUserId, string currentUserDisplayName);
        bool RemoveRedOfferingById(int id, string currentUserId, string currentUserDisplayName);

        bool SaveRedOffering(RecommendOfferingDTO model);

        PaginatedBaseDTO<RecommendOfferingLogDTO> FindPaginatedRecommendOfferingLogs(int pageIndex, int pageSize, string name);
        #endregion

        #region 推荐需求
        PaginatedBaseDTO<RecommendRequirementDTO> FindPaginatedRecommendRequirements(int pageIndex, int pageSize, string name, RecommendStatus? Status);

        RecommendRequirementDTO GetRedRequirementById(int id);

        bool AuditRedRequirementById(int id, bool isApprove, string comment, string currentUserId, string currentUserDisplayName);
        bool TaskOffRedRequirementById(int id, string currentUserId, string currentUserDisplayName);
        bool RemoveRedRequirementById(int id, string currentUserId, string currentUserDisplayName);

        bool SaveRedRequirement(RecommendRequirementDTO model);

        PaginatedBaseDTO<RecommendRequirementLogDTO> FindPaginatedRecommendRequirementLogs(int pageIndex, int pageSize, string name);
        #endregion
    }

    public class RecommendService : EFServiceBase, IRecommendService
    {
        private readonly IMapper _mapper;

        #region Construction & Repository
        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private readonly IConfigApiService ConfigApiService;
        private readonly IRecommendCategoryRepository _redCategoryRepository;
        private readonly IRecommendCustomerRepository _redCustomerRepository;
        private readonly IDbRepository<RecommendCustomerLog> _redCustomerLogRepository;

        private readonly IRecommendOfferingRepository _redOfferingRepository;
        private readonly IDbRepository<RecommendOfferingLog> _redOfferingLogRepository;

        private readonly IRecommendRequirementRepository _redRequirementRepository;
        private readonly IDbRepository<RecommendRequirementLog> _redRequirementLogRepository;

        public RecommendService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfWorkContext,
            IConfigApiService configApiService,

            IRecommendCategoryRepository redCategoryRepository,

            IRecommendCustomerRepository redCustomerRepository,
            IDbRepository<RecommendCustomerLog> redCustomerLogRepository,

            IRecommendOfferingRepository redOfferingRepository,
            IDbRepository<RecommendOfferingLog> redOfferingLogRepository,

            IRecommendRequirementRepository redRequirementRepository,
            IDbRepository<RecommendRequirementLog> redRequirementLogRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<RecommendService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;
            ConfigApiService = configApiService;

            _redCategoryRepository = redCategoryRepository;

            _redCustomerRepository = redCustomerRepository;
            _redCustomerLogRepository = redCustomerLogRepository;

            _redOfferingRepository = redOfferingRepository;
            _redOfferingLogRepository = redOfferingLogRepository;

            _redRequirementRepository = redRequirementRepository;
            _redRequirementLogRepository = redRequirementLogRepository;
        }
        #endregion

        public PaginatedBaseDTO<RecommendInfoDTO> LoadPaginatedRecommendInfosByFilter(int pageIndex, int pageSize, int? categoryId, string name)
        {
            var pPageSize = pageSize / 3;
            #region RecommendCustomer
            Expression<Func<RecommendCustomer, bool>> predicateCustomer = m => !m.IsDeleted;
            //未分类
            if (categoryId.HasValue && categoryId.Value < 0)
            {
                predicateCustomer = predicateCustomer.And(m => m.CategoryId == null);
            }
            //已分类
            else if (categoryId.HasValue && categoryId.Value > 0)
            {
                predicateCustomer = predicateCustomer.And(m => m.CategoryId.Equals(categoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicateCustomer = predicateCustomer.And(m => m.RecommendName.Contains(name.Trim()));
            }

            var resultCustomer = _redCustomerRepository.FindPagenatedListWithCount(pageIndex, pPageSize, predicateCustomer, m => m.ModifiedDate, false);
            var totalCustomer = resultCustomer.Item1;
            var rowsCustomer = _mapper.Map<List<RecommendInfoDTO>>(resultCustomer.Item2);
            #endregion

            #region RecommendOffering
            Expression<Func<RecommendOffering, bool>> predicateOffering = m => !m.IsDeleted;
            //未分类
            if (categoryId.HasValue && categoryId.Value < 0)
            {
                predicateOffering = predicateOffering.And(m => m.CategoryId == null);
            }
            //已分类
            else if (categoryId.HasValue && categoryId.Value > 0)
            {
                predicateOffering = predicateOffering.And(m => m.CategoryId.Equals(categoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicateOffering = predicateOffering.And(m => m.RecommendName.Contains(name.Trim()));
            }

            var resultOffering = _redOfferingRepository.FindPagenatedListWithCount(pageIndex, pPageSize, predicateOffering, m => m.ModifiedDate, false);
            var totalOffering = resultOffering.Item1;
            var rowsOffering = _mapper.Map<List<RecommendInfoDTO>>(resultOffering.Item2);
            #endregion

            #region RecommendRequirement
            Expression<Func<RecommendRequirement, bool>> predicateRequirement = m => !m.IsDeleted;
            //未分类
            if (categoryId.HasValue && categoryId.Value < 0)
            {
                predicateRequirement = predicateRequirement.And(m => m.CategoryId == null);
            }
            //已分类
            else if (categoryId.HasValue && categoryId.Value > 0)
            {
                predicateRequirement = predicateRequirement.And(m => m.CategoryId.Equals(categoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicateRequirement = predicateRequirement.And(m => m.RecommendName.Contains(name.Trim()));
            }

            var resultRequirement = _redRequirementRepository.FindPagenatedListWithCount(pageIndex, pPageSize, predicateRequirement, m => m.ModifiedDate, false);
            var totalRequirement = resultRequirement.Item1;
            var rowsRequirement = _mapper.Map<List<RecommendInfoDTO>>(resultRequirement.Item2);
            #endregion

            var total = totalCustomer + totalOffering + totalRequirement;
            var rows = rowsCustomer.Union(rowsOffering).Union(rowsOffering).ToList();
            return new PaginatedBaseDTO<RecommendInfoDTO>(pageIndex, pageSize, total, rows);
        }

        #region 推荐企业

        public PaginatedBaseDTO<RecommendCustomerDTO> FindPaginatedRecommendCustomers(int pageIndex, int pageSize, string name, RecommendStatus? status)
        {
            Expression<Func<RecommendCustomer, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.RecommendName.Contains(name));
            }
            if (status.HasValue)
            {
                predicate = predicate.And(m => m.Status == status.Value);
            }
            var data = _redCustomerRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.Index, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<RecommendCustomerDTO>>(data.Item2);
            return new PaginatedBaseDTO<RecommendCustomerDTO>(pageIndex, pageSize, total, rows);
        }
        public RecommendCustomerDTO GetRedCustomerById(int id)
        {
            var result = _redCustomerRepository.FindRedCustomerById(id);
            return _mapper.Map<RecommendCustomerDTO>(result);
        }

        public bool AuditRedCustomerById(int id, bool isApprove, string comment, string currentUserId, string currentUserDisplayName)
        {
            var k = _redCustomerRepository.GetById(id);
            if (k.Status == RecommendStatus.AuditPending)
            {
                k.Status = isApprove ? RecommendStatus.Approved : RecommendStatus.Disagree;
            }

            var log = new RecommendCustomerLog()
            {
                RecommendId = k.RecommendId,
                RecommendRefCode = k.RecommendRefCode,
                RecommendCode = k.RecommendCode,
                RecommendName = k.RecommendName,
                OperatorId = currentUserId,
                Operator = currentUserDisplayName,
                OperateDate = k.ModifiedDate,
                Remark = string.Format("将推荐企业【{0}】进行审核，审核结果：{1}，审核意见：{2}。", k.RecommendName, isApprove ? "同意":"不同意", comment),
            };
            _redCustomerLogRepository.Add(log);

            return _redCustomerRepository.Modify(k, new[] { "Status" });
        }
        public bool TakeOffRedCustomerById(int id, string currentUserId, string currentUserDisplayName)
        {
            var k = _redCustomerRepository.GetById(id);
            if (k.Status == RecommendStatus.Approved)
            {
                k.Status = RecommendStatus.Trash;
            }

            var log = new RecommendCustomerLog()
            {
                RecommendId = k.RecommendId,
                RecommendRefCode = k.RecommendRefCode,
                RecommendCode = k.RecommendCode,
                RecommendName = k.RecommendName,
                OperatorId = currentUserId,
                Operator = currentUserDisplayName,
                OperateDate = k.ModifiedDate,
                Remark = string.Format("将推荐企业【{0}】进行下架处理。", k.RecommendName),
            };
            _redCustomerLogRepository.Add(log);

            return _redCustomerRepository.Modify(k, new[] { "Status" });
        }
        public bool RemoveRedCustomerById(int id, string currentUserId, string currentUserDisplayName)
        {
            var k = _redCustomerRepository.GetById(id);
            k.IsDeleted = true;

            var log = new RecommendCustomerLog()
            {
                RecommendId = k.RecommendId,
                RecommendRefCode = k.RecommendRefCode,
                RecommendCode = k.RecommendCode,
                RecommendName = k.RecommendName,
                OperatorId = currentUserId,
                Operator = currentUserDisplayName,
                OperateDate = k.ModifiedDate,
                Remark = string.Format("将推荐企业【{0}】进行删除。", k.RecommendName),
            };
            _redCustomerLogRepository.Add(log);

            return _redCustomerRepository.Modify(k, new[] { "IsDeleted" });
        }
        
        public bool SaveRedCustomer(RecommendCustomerDTO model)
        {
            var entity = _mapper.Map<RecommendCustomer>(model);
            var success = false;
            var msg = model.RecommendId == 0
                    ? "创建了新的文件"
                    : "修改了文件的内容";
            if (model.RecommendId == 0)
            {
                success = _redCustomerRepository.Add(entity);
            }
            else
            {
                success = _redCustomerRepository.Modify(entity);
            }

            if (success)
            {
                _redCustomerLogRepository.Add(new RecommendCustomerLog()
                {
                    RecommendId = entity.RecommendId,
                    RecommendRefCode = entity.RecommendRefCode,
                    RecommendCode = entity.RecommendCode,
                    RecommendName = entity.RecommendName,
                    OperatorId = model.ModifiedBy,
                    Operator = model.ModifiedName,
                    OperateDate = model.ModifiedDate,
                    Remark = msg,
                });
            }

            return success;
        }

        public PaginatedBaseDTO<RecommendCustomerLogDTO> FindPaginatedRecommendCustomerLogs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<RecommendCustomerLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _redCustomerLogRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.OperateDate, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<RecommendCustomerLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<RecommendCustomerLogDTO>(pageIndex, pageSize, total, rows);

        }
        #endregion

        #region 推荐商品

        public PaginatedBaseDTO<RecommendOfferingDTO> FindPaginatedRecommendOfferings(int pageIndex, int pageSize, string name, RecommendStatus? status)
        {
            Expression<Func<RecommendOffering, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.RecommendName.Contains(name));
            }
            if (status.HasValue)
            {
                predicate = predicate.And(m => m.Status == status.Value);
            }
            var data = _redOfferingRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.Index, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<RecommendOfferingDTO>>(data.Item2);
            return new PaginatedBaseDTO<RecommendOfferingDTO>(pageIndex, pageSize, total, rows);
        }
        public RecommendOfferingDTO GetRedOfferingById(int id)
        {
            var result = _redOfferingRepository.FindRedOfferingById(id);
            return _mapper.Map<RecommendOfferingDTO>(result);
        }

        public bool AuditRedOfferingById(int id, bool isApprove, string comment, string currentUserId, string currentUserDisplayName)
        {
            var k = _redOfferingRepository.GetById(id);
            if (k.Status == RecommendStatus.AuditPending)
            {
                k.Status = isApprove ? RecommendStatus.Approved : RecommendStatus.Disagree;
            }

            var log = new RecommendOfferingLog()
            {
                RecommendId = k.RecommendId,
                RecommendRefCode = k.RecommendRefCode,
                RecommendCode = k.RecommendCode,
                RecommendName = k.RecommendName,
                OperatorId = currentUserId,
                Operator = currentUserDisplayName,
                OperateDate = k.ModifiedDate,
                Remark = string.Format("将推荐企业【{0}】进行审核，审核结果：{1}，审核意见：{2}。", k.RecommendName, isApprove ? "同意" : "不同意", comment),
            };
            _redOfferingLogRepository.Add(log);

            return _redOfferingRepository.Modify(k, new[] { "Status" });
        }
        public bool TakeOffRedOfferingById(int id, string currentUserId, string currentUserDisplayName)
        {
            var k = _redOfferingRepository.GetById(id);
            if (k.Status == RecommendStatus.Approved)
            {
                k.Status = RecommendStatus.Trash;
            }

            var log = new RecommendOfferingLog()
            {
                RecommendId = k.RecommendId,
                RecommendRefCode = k.RecommendRefCode,
                RecommendCode = k.RecommendCode,
                RecommendName = k.RecommendName,
                OperatorId = currentUserId,
                Operator = currentUserDisplayName,
                OperateDate = k.ModifiedDate,
                Remark = string.Format("将推荐商品【{0}】进行下架处理。", k.RecommendName),
            };
            _redOfferingLogRepository.Add(log);

            return _redOfferingRepository.Modify(k, new[] { "Status" });
        }
        public bool RemoveRedOfferingById(int id, string currentUserId, string currentUserDisplayName)
        {
            var k = _redOfferingRepository.GetById(id);
            k.IsDeleted = true;

            var log = new RecommendOfferingLog()
            {
                RecommendId = k.RecommendId,
                RecommendRefCode = k.RecommendRefCode,
                RecommendCode = k.RecommendCode,
                RecommendName = k.RecommendName,
                OperatorId = currentUserId,
                Operator = currentUserDisplayName,
                OperateDate = k.ModifiedDate,
                Remark = string.Format("将推荐企业【{0}】进行删除。", k.RecommendName),
            };
            _redOfferingLogRepository.Add(log);

            return _redOfferingRepository.Modify(k, new[] { "IsDeleted" });
        }

        public bool SaveRedOffering(RecommendOfferingDTO model)
        {
            var entity = _mapper.Map<RecommendOffering>(model);
            var success = false;
            var msg = model.RecommendId == 0
                    ? "创建了新的文件"
                    : "修改了文件的内容";
            if (model.RecommendId == 0)
            {
                success = _redOfferingRepository.Add(entity);
            }
            else
            {
                success = _redOfferingRepository.Modify(entity);
            }

            if (success)
            {
                _redOfferingLogRepository.Add(new RecommendOfferingLog()
                {
                    RecommendId = entity.RecommendId,
                    RecommendRefCode = entity.RecommendRefCode,
                    RecommendCode = entity.RecommendCode,
                    RecommendName = entity.RecommendName,
                    OperatorId = model.ModifiedBy,
                    Operator = model.ModifiedName,
                    OperateDate = model.ModifiedDate,
                    Remark = msg,
                });
            }

            return success;
        }

        public PaginatedBaseDTO<RecommendOfferingLogDTO> FindPaginatedRecommendOfferingLogs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<RecommendOfferingLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _redOfferingLogRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.OperateDate, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<RecommendOfferingLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<RecommendOfferingLogDTO>(pageIndex, pageSize, total, rows);

        }
        #endregion

        #region 推荐需求

        public PaginatedBaseDTO<RecommendRequirementDTO> FindPaginatedRecommendRequirements(int pageIndex, int pageSize, string name, RecommendStatus? status)
        {
            Expression<Func<RecommendRequirement, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.RecommendName.Contains(name));
            }
            if (status.HasValue)
            {
                predicate = predicate.And(m => m.Status == status.Value);
            }
            var data = _redRequirementRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.Index, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<RecommendRequirementDTO>>(data.Item2);
            return new PaginatedBaseDTO<RecommendRequirementDTO>(pageIndex, pageSize, total, rows);
        }
        public RecommendRequirementDTO GetRedRequirementById(int id)
        {
            var result = _redRequirementRepository.FindRedRequirementById(id);
            return _mapper.Map<RecommendRequirementDTO>(result);
        }

        public bool AuditRedRequirementById(int id, bool isApprove, string comment, string currentUserId, string currentUserDisplayName)
        {
            var k = _redRequirementRepository.GetById(id);
            if (k.Status == RecommendStatus.AuditPending)
            {
                k.Status = isApprove ? RecommendStatus.Approved : RecommendStatus.Disagree;
            }

            var log = new RecommendRequirementLog()
            {
                RecommendId = k.RecommendId,
                RecommendRefCode = k.RecommendRefCode,
                RecommendCode = k.RecommendCode,
                RecommendName = k.RecommendName,
                OperatorId = currentUserId,
                Operator = currentUserDisplayName,
                OperateDate = k.ModifiedDate,
                Remark = string.Format("将推荐采购信息【{0}】进行审核，审核结果：{1}，审核意见：{2}。", k.RecommendName, isApprove ? "同意" : "不同意", comment),
            };
            _redRequirementLogRepository.Add(log);

            return _redRequirementRepository.Modify(k, new[] { "Status" });
        }
        public bool TaskOffRedRequirementById(int id, string currentUserId, string currentUserDisplayName)
        {
            var k = _redRequirementRepository.GetById(id);
            if (k.Status == RecommendStatus.Approved)
            {
                k.Status = RecommendStatus.Trash;
            }

            var log = new RecommendRequirementLog()
            {
                RecommendId = k.RecommendId,
                RecommendRefCode = k.RecommendRefCode,
                RecommendCode = k.RecommendCode,
                RecommendName = k.RecommendName,
                OperatorId = currentUserId,
                Operator = currentUserDisplayName,
                OperateDate = k.ModifiedDate,
                Remark = string.Format("将推荐采购需求【{0}】进行下架处理。", k.RecommendName),
            };
            _redRequirementLogRepository.Add(log);

            return _redRequirementRepository.Modify(k, new[] { "IsDeleted" });
        }

        public bool RemoveRedRequirementById(int id, string currentUserId, string currentUserDisplayName)
        {
            var k = _redRequirementRepository.FindRedRequirementById(id);
            k.IsDeleted = true;

            var log = new RecommendRequirementLog()
            {
                RecommendId = k.RecommendId,
                RecommendRefCode = k.RecommendRefCode,
                RecommendCode = k.RecommendCode,
                RecommendName = k.RecommendName,
                OperatorId = currentUserId,
                Operator = currentUserDisplayName,
                OperateDate = k.ModifiedDate,
                Remark = string.Format("将推荐企业【{0}】进行删除。", k.RecommendName),
            };
            _redRequirementLogRepository.Add(log);

            return _redRequirementRepository.Modify(k, new[] { "IsDeleted" });
        }
        public bool SaveRedRequirement(RecommendRequirementDTO model)
        {
            var entity = _mapper.Map<RecommendRequirement>(model);
            var success = false;
            var msg = model.RecommendId == 0
                    ? "创建了新的文件"
                    : "修改了文件的内容";
            if (model.RecommendId == 0)
            {
                success = _redRequirementRepository.Add(entity);
            }
            else
            {
                success = _redRequirementRepository.Modify(entity);
            }

            if (success)
            {
                _redRequirementLogRepository.Add(new RecommendRequirementLog()
                {
                    RecommendId = entity.RecommendId,
                    RecommendRefCode = entity.RecommendRefCode,
                    RecommendCode = entity.RecommendCode,
                    RecommendName = entity.RecommendName,
                    OperatorId = model.ModifiedBy,
                    Operator = model.ModifiedName,
                    OperateDate = model.ModifiedDate,
                    Remark = msg,
                });
            }

            return success;
        }

        public PaginatedBaseDTO<RecommendRequirementLogDTO> FindPaginatedRecommendRequirementLogs(int pageIndex, int pageSize, string name)
        {
            Expression<Func<RecommendRequirementLog, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Operator.Contains(name));
            }

            var data = _redRequirementLogRepository.FindPagenatedListWithCount(
                pageIndex, pageSize, predicate, m => m.OperateDate, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<RecommendRequirementLogDTO>>(data.Item2);
            return new PaginatedBaseDTO<RecommendRequirementLogDTO>(pageIndex, pageSize, total, rows);

        }
        #endregion
    }
}
