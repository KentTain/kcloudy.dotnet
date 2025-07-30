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
using KC.Service.WebApiService.Business;
using KC.Service.Contract.WebApiService.Business;
using KC.Service.Contract.WebApiService.ThridParty;
using KC.Model.Contract;
using KC.DataAccess.Contract.Repository;
using KC.Service.DTO;
using KC.Framework.Extension;
using KC.Service.DTO.Contract;
using KC.Enums.Contract;
using KC.Service;

using KC.Common;
using Newtonsoft.Json;
using KC.Common.FileHelper;
using KC.Database.IRepository;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Framework.SecurityHelper;
using KC.Common.HttpHelper;
using System.IO;
using KC.Storage.Util;

namespace KC.Service.Contract
{
    public interface IContractService : IEFService
    {
        #region 合同组

        string AddContractGroup(ContractGroupDTO model);

        bool UpdateContractGroupTo(ContractGroupDTO source);
        string UpdateUserContract(string blobId, string tenantName, UserContractStatus userContractStatu);
        string UpdateContractAndUserContract(string userContractId, ContractStatus contractGroupStatu, WorkflowBusStatus workFlowStatu, UserContractStatus userContractStatu, string remark);

        string DeleteContractGroup(string contractGroupId);
        bool RemoveSingContractServices(Guid id);
        #endregion

        #region 合同
        Task<PaginatedBaseDTO<ContractGroupDTO>> GetContractPageListAsync(int page, int rows, DateTime? startTime, DateTime? endTime, string key, ContractStatus? contractStatu, ContractType? contracttype, CustomerType? customerType, string currentUserId, string currentTenantName, string currentDispayName);//, string bannedcontract
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderInfoId">采购单号</param>
        /// <param name="currentUserId"></param>
        /// <param name="currentUserName"></param>
        /// <param name="fileName"></param>
        /// <param name="blobId"></param>
        /// <param name="sell"></param>
        /// <returns></returns>
        ReturnMessageDTO OrderUpPDFToContract(string orderInfoId, string currentUserId, string currentUserName, string fileName, string blobId, string contractNo);

        PaginatedBaseDTO<ContractGroupOperationLogDTO> LoadContractGroupLogs(Guid contractGroupId, int page, int rows);

        ContractGroupDTO GetContractGroupById(Guid id);
        ContractGroupDTO GetContractGroupById(string id);
        ContractGroupDTO GetContractGroupByRelationData(string id);

        bool EditContractStatu(ContractGroupDTO contractGroupModel);

        ServiceResult<bool> GetDetails(Guid id);
        ServiceResult<bool> EditCurrencySignService(ContractGroupDTO model, string filename, string currentUserId);

        ServiceResult<bool> AbandonedContract(string currentUserName, string applyOperatorName, Guid id,
            string remark = "", string pdfX = "", string pdfY = "", string url = "");

        ServiceResult<bool> RemoveCurrencySignService(Guid id, string currentUserName, string currentUserDisplayName);

        ServiceResult<bool> ComfirmContract(string applyOperatorId, string applyOperatorName, Guid id, bool isComfirmFrist = false, string SignAdress = "");
        ServiceResult<bool> RetutrnContract(string currentUserName, string applyOperatorName, Guid id, string remark = "", string url = "", string userId = "");

        bool ReApplyRemoveCurrencySignService(ContractGroupDTO contractGroupDto, string currentUserName, string currentUserDisplayName, int processLogId = 0, int delType = 0);

        bool ReApplyConfirmCurrencySign(ContractGroupDTO contractGroup, string applyOperatorId, string applyOperatorName, int processLogId = 0, bool isStatu = false);

        int ReApplyPlatFormConfirmCurrencySign(string applyOperatorId, string applyOperatorName, ContractGroupDTO contractGroup, bool isStatu = false);

        ServiceResult<bool> SignContract(string currentUserid, string currentUserName, string applyOperatorName, bool personal, string code, Guid id, string url = "", int pdfX = 0, int pdfY = 0, string posPage = "0", int signType = 1);
        ServiceResult<bool> SignContractFun(string currentUserid, string currentUserName, string applyOperatorName, bool personal, string code, Guid id, string url = "", int pdfX = 0, int pdfY = 0, string posPage = "0", int signType = 1);

        bool HandleSignCallbackFail(Guid id, string url);
        #endregion

        #region 合同模板

        List<ContractTemplateDTO> LoadContractTemplates();
        Task<List<ContractTemplateDTO>> GetContractTemplate(ContractType? type);

        ContractTemplateDTO GetContractTemplateById(int id);

        Task<ContractTemplateValueDTO> GetMyContractTemplateById(int id);
        Task<ReturnMessageDTO> ModifyContractTemplate(ContractTemplateValueDTO model);

        bool SaveContractTemplate(ContractTemplateValueDTO model);

        bool SoftRemoveContractTemplate(int id);

        ServiceResult<bool> MakeAddValueServiceContract(string currentUserId, string currentUserName, string fileName, string blobId, string contractNo, decimal money);

        #endregion
    }

    public class ContractService : EFServiceBase, IContractService
    {
        private readonly IMapper _mapper;
        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        protected EFUnitOfWorkContextBase _unitOfWorkContext { get; private set; }

        private readonly IElectronicSignApiService ElectronicSignApiService;
        private readonly IContractApiService ContractApiService;

        private readonly IContractGroupRepository ContractGroupRepository;
        private readonly IUserContractRepository UserContractRepository;

        private readonly IDbRepository<ContractTemplate> ContractTemplateRepository;
        private readonly IDbRepository<ContractGroupOperationLog> ContractGroupOperationLogRepository;

        private readonly ITenantUserApiService TenantUserApiService;

        public ContractService(
            Tenant tenant,
            IMapper mapper,
            ITenantUserApiService tenantUserApiService,
            EFUnitOfWorkContextBase unitOfWorkContext,

            IElectronicSignApiService currencySignApiService,
            IContractApiService currencyApiService,

            IContractGroupRepository contractGroupRepository,
            IUserContractRepository userContractRepository,
            IDbRepository<ContractTemplate> contractTemplateRepository,
            IDbRepository<ContractGroupOperationLog> contractGroupOperationLogRepository,

            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<ContractService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfWorkContext = unitOfWorkContext;
            TenantUserApiService = tenantUserApiService;

            ElectronicSignApiService = currencySignApiService;
            ContractApiService = currencyApiService;
            ContractGroupRepository = contractGroupRepository;
            UserContractRepository = userContractRepository;
            ContractTemplateRepository = contractTemplateRepository;
            ContractGroupOperationLogRepository = contractGroupOperationLogRepository;
        }

        #region 合同组
        public bool UpdateContractGroupTo(ContractGroupDTO source)
        {
            return SubmitEditSingContractServices(_mapper.Map<ContractGroup>(source));
        }

        /// <summary>
        /// 编辑合同和新增合同模型
        /// </summary>
        /// <returns></returns>
        public bool SubmitEditSingContractServices(ContractGroup contractGroup)
        {
            contractGroup.PId = null;
            if (contractGroup.Id != Guid.Empty)
            {
                var r = ContractGroupRepository.GetContractGroup(contractGroup.Id);
                if (r != null)
                {
                    if (r.UserContract != null && r.UserContract.Any())
                    {
                        var item = r.UserContract.ToList();
                        for (var i = 0; i < item.Count; i++)
                            UserContractRepository.RemoveById(item[i], false);
                    }
                    ContractGroupRepository.RemoveById(r, false);
                }
            }
            contractGroup.CreatedDate = DateTime.UtcNow;
            return ContractGroupRepository.Add(contractGroup);
        }

        public string AddContractGroup(ContractGroupDTO model)
        {
            if (model == null)
            {
                return "数据库不能为空";
            }
            if (!model.UserContract.Any())
            {
                return "数据错误";
            }
            var contractGroup = ContractGroupRepository.GetContractGroupById(model.Id);
            if (contractGroup != null)
            {
                var userContractList = contractGroup.UserContract.Where(m => m.UserId != contractGroup.UserId).ToList();
                foreach (var userContract in userContractList)
                {
                    userContract.Statu = UserContractStatus.Submited;
                }
                UserContractRepository.Modify(userContractList, new[] { "Statu" }, false);
                contractGroup.WorkFlowStatus = WorkflowBusStatus.AuditPending;
                contractGroup.BlobId = model.BlobId;
                contractGroup.Statu = ContractStatus.WaitSign;
                contractGroup.ContractTitle = model.ContractTitle;
                contractGroup.ContractNo = model.ContractNo;
                ContractGroupRepository.Modify(contractGroup, new[] { "Statu", "WorkFlowStatus", "BlobId", "ContractTitle", "ContractNo" }, false);
                if (_unitOfWorkContext.Commit() > 0)
                    return string.Empty;
                return "操作失败";
            }
            var group = _mapper.Map<ContractGroup>(model);
            group.ReferenceId = model.Id.ToString();
            group.WorkFlowStatus = WorkflowBusStatus.AuditPending;
            var addResult = ContractGroupRepository.Add(group);
            if (addResult)
                return string.Empty;
            return "添加失败";
        }
        //确认操作更新状态
        public string UpdateUserContract(string blobId, string tenantName, UserContractStatus userContractStatu)
        {
            if (string.IsNullOrWhiteSpace(blobId))
                return "blobId不能为空";
            var contractGroup = ContractGroupRepository.GetContractByBlobId(blobId);
            if (contractGroup == null)
            {
                return "更新签署方合同状态时未找到合同";
            }
            if (!contractGroup.UserContract.Any())
            {
                return "数据错误";
            }
            if (!(contractGroup.UserContract.Any(m => m.Statu != UserContractStatus.Agree && m.UserId.ToLower() != tenantName.ToLower())))
                contractGroup.Statu = ContractStatus.WaitSign;
            ContractGroupRepository.Modify(contractGroup, new[] { "Statu" }, false);
            var myUserContract = contractGroup.UserContract.FirstOrDefault(m => m.UserId.ToLower() == tenantName.ToLower());
            if (myUserContract == null)
                return "数据错误";
            myUserContract.Statu = userContractStatu;
            UserContractRepository.Modify(myUserContract, new[] { "Statu" }, false);
            if (_unitOfWorkContext.Commit() > 0)
                return string.Empty;
            return "更新状态失败";
        }
        //发起签署更新合同状态
        public string UpdateContractAndUserContract(string userContractId, ContractStatus contractGroupStatu, WorkflowBusStatus workFlowStatu, UserContractStatus userContractStatu, string remark)
        {
            var id = Guid.Empty;
            if (string.IsNullOrWhiteSpace(userContractId))
            {
                return "数据错误";
            }
            id = Guid.Parse(userContractId);
            var userContract = UserContractRepository.FindAll(m => !m.IsDeleted && m.Id == id).FirstOrDefault();
            if (userContract == null)
                return "数据错误";
            userContract.Statu = userContractStatu;
            userContract.BreakRemark = remark;
            UserContractRepository.Modify(userContract, new[] { "Statu", "BreakRemark" });
            var contractGroup = ContractGroupRepository.FindAll(m => !m.IsDeleted && m.Id == userContract.BlobId).FirstOrDefault();
            if (contractGroup == null)
                return "未找到合同";
            if (contractGroup.UserId.ToLower() == Tenant.TenantName.ToLower())
                contractGroup.WorkFlowStatus = workFlowStatu;
            contractGroup.Statu = contractGroupStatu;
            ContractGroupRepository.Modify(contractGroup, new[] { "Statu", "WorkFlowStatus" }, false);
            if (_unitOfWorkContext.Commit() > 0)
                return string.Empty;
            return "操作失败";
        }

        public string DeleteContractGroup(string contractGroupId)
        {
            var id = Guid.Empty;
            if (string.IsNullOrWhiteSpace(contractGroupId))
            {
                return "数据错误";
            }
            id = Guid.Parse(contractGroupId);
            var contractGroup = ContractGroupRepository.FindAll(m => !m.IsDeleted && m.Id == id).FirstOrDefault();
            if (contractGroup == null)
            {
                return "未找到合同信息";
            }
            contractGroup.IsDeleted = true;
            var deleteResult = ContractGroupRepository.Modify(contractGroup, new[] { "IsDeleted" });
            if (deleteResult)
                return string.Empty;
            return "操作失败";
        }

        public bool RemoveSingContractServices(Guid id)
        {
            var contractGroup = ContractGroupRepository.FindAll(m => !m.IsDeleted && m.Id == id).FirstOrDefault();
            if (contractGroup != null)
            {
                contractGroup.IsDeleted = true;
                return ContractGroupRepository.Modify(contractGroup, new[] { "IsDeleted" });
            }
            return false;
        }
        #endregion

        #region 合同
        /// <summary>
        /// 同步读取合同列表数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="bannedcontract"></param>
        /// <param name="key"></param>
        /// <param name="contractStatu"></param>
        /// <param name="contracttype"></param>
        /// <param name="customerType"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentTenantName"></param>
        /// <param name="currentDispayName"></param>
        /// <returns></returns>
        public async Task<PaginatedBaseDTO<ContractGroupDTO>> GetContractPageListAsync(int page, int rows, DateTime? startTime, DateTime? endTime, string key, ContractStatus? contractStatu, ContractType? contracttype, CustomerType? customerType, string currentUserId, string currentTenantName, string currentDispayName)//, string bannedcontract
        {
            Expression<Func<ContractGroup, bool>> predicate = m => !m.IsDeleted;
            if (!string.IsNullOrEmpty(key))
            {
                predicate = predicate.And(m => m.ContractTitle.Contains(key) || m.ContractNo.Contains(key) || m.UserName.Contains(key) || m.RelationData.Equals(key, StringComparison.OrdinalIgnoreCase));
            }
            if (contracttype.HasValue)
            {
                predicate = predicate.And(m => m.Type == contracttype.Value);
            }
            if (contractStatu.HasValue)
            {
                predicate = predicate.And(m => m.Statu == contractStatu.Value);
            }
            //if (!string.IsNullOrEmpty(bannedcontract))
            //{
            //    var inttype = bannedcontract.Split(',');
            //    foreach (var item in inttype)
            //    {
            //        var itemi = int.Parse(item);
            //        predicate = predicate.And(m => m.Type != (ContractType)itemi);
            //    }
            //}
            if (customerType.HasValue && customerType.Value == CustomerType.Personal)
            {
                if (!string.IsNullOrEmpty(currentUserId))
                {
                    predicate = predicate.And(m => m.UserContract.Any(c => c.CustomerType == customerType.Value && c.StaffId.Equals(currentUserId, StringComparison.OrdinalIgnoreCase)));
                }
            }
            else
            {
                predicate = predicate.And(m => m.UserContract.Any(c => c.UserId.Equals(currentTenantName, StringComparison.OrdinalIgnoreCase) && c.UserName.Equals(currentDispayName, StringComparison.OrdinalIgnoreCase)));
            }
            if (startTime.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate >= startTime.Value);
            }
            if (endTime.HasValue)
            {
                predicate = predicate.And(m => m.CreatedDate <= endTime.Value);
            }
            var tempData = await ContractGroupRepository.GetContractsByFilterAsync(page, rows, predicate, m => m.CreatedDate, true, m => m.UserContract);
            var mapRows = _mapper.Map<List<ContractGroupDTO>>(tempData.Item2);
            var returnRows = GetContractGroupDTOList(mapRows);
            return new PaginatedBaseDTO<ContractGroupDTO>(page, rows, tempData.Item1, returnRows);
        }

        /// <summary>
        /// 操作权限控制
        /// </summary>
        /// <returns></returns>
        private List<ContractGroupDTO> GetContractGroupDTOList(List<ContractGroupDTO> model)
        {
            for (int i = 0; i < model.Count; i++)
            {

                switch (model[i].Statu)
                {
                    case ContractStatus.WaitforSubmited:
                        //等待审核（上传方，和签署方）
                        if (model[i].UserContract.Count(m => m.Statu.Equals(UserContractStatus.Comfirm)) > 0 && model[i].UserId.Equals(Tenant.TenantName, StringComparison.OrdinalIgnoreCase))
                        {
                            model[i].IsComfirmFrist = true;
                        }
                        else if (model[i].UserContract.Count(m => m.Statu.Equals(UserContractStatus.Submited) && m.UserId.Equals(Tenant.TenantName, StringComparison.OrdinalIgnoreCase)) > 0 && !model[i].UserId.Equals(Tenant.TenantName, StringComparison.OrdinalIgnoreCase))
                        {
                            model[i].IsComfirm = true;
                        }
                        if (model[i].UserContract.Count(m => (m.Statu.Equals(UserContractStatus.Submited) || m.Statu.Equals(UserContractStatus.Comfirm)) && m.UserId.Equals(Tenant.TenantName, StringComparison.OrdinalIgnoreCase)) > 0)
                        {
                            model[i].IsReturn = true;
                        }

                        break;
                    case ContractStatus.WaitSign:
                        if (model[i].UserContract.Count(m => m.Statu.Equals(UserContractStatus.WaitSign) && m.UserId.Equals(Tenant.TenantName, StringComparison.OrdinalIgnoreCase)) > 0)
                        {
                            model[i].IsSign = true;
                        }
                        //签署中
                        break;
                    case ContractStatus.Returned:
                        if (Tenant.TenantName.ToLower() == model[i].UserId.ToLower() && model[i].Type == ContractType.Electronic)//上传方被退回，通用合同，可以编辑
                        {
                            model[i].IsEdit = true;
                        }
                        model[i].IsRelieve = true;
                        break;
                    case ContractStatus.Complete:
                        if (model[i].Type != ContractType.Seller && model[i].Type != ContractType.Purchase)
                        {
                            model[i].IsRelieveAll = true;//这里需要根据自己的业务去定义作废的权限
                        }
                        break;
                    case ContractStatus.WaitCancel:
                        if (model[i].UserContract.Count(m => m.Statu.Equals(UserContractStatus.WaitAbandoned) && m.UserId.Equals(Tenant.TenantName, StringComparison.OrdinalIgnoreCase)) > 0)
                        {
                            model[i].IsRelieveAll = true;
                            //判断是不是最后一个作废
                            if (model[i].UserContract.Count(m => !m.UserId.Equals(Tenant.TenantName, StringComparison.CurrentCultureIgnoreCase) && m.Statu == UserContractStatus.Abandoned) == model[i].UserContract.Count - 1)
                            {
                                model[i].IsLast = true;
                            }
                        }
                        break;
                    case ContractStatus.Abandoned:
                        model[i].IsRelieve = true;
                        break;
                }
            }
            return model;
        }

        /// <summary>
        /// 上传pdf到合同
        /// </summary>
        /// <param name="orderInfoId"></param>
        /// <param name="currentUserId"></param>
        /// <param name="currentUserName"></param>
        /// <param name="fileName"></param>
        /// <param name="blobId"></param>
        /// <param name="contractNo"></param>
        /// <returns></returns>
        public ReturnMessageDTO OrderUpPDFToContract(string orderInfoId, string currentUserId, string currentUserName, string fileName, string blobId, string contractNo)
        {
            var msg = new ReturnMessageDTO() { Success = false };
            throw new NotImplementedException();
        }

        /// <summary>
        /// 读取合同操作日志
        /// </summary>
        /// <param name="contractGroupId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public PaginatedBaseDTO<ContractGroupOperationLogDTO> LoadContractGroupLogs(Guid contractGroupId, int page, int rows)
        {
            Expression<Func<ContractGroupOperationLog, bool>> predicate = m => true;
            predicate = predicate.And(m => m.ContractGroupId == contractGroupId);
            var result = ContractGroupOperationLogRepository.FindAll(predicate);
            if (result.Any())
            {
                var data = _mapper.Map<List<ContractGroupOperationLogDTO>>(result);
                return new PaginatedBaseDTO<ContractGroupOperationLogDTO>(page, rows, data.Count, data);
            }
            return new PaginatedBaseDTO<ContractGroupOperationLogDTO>(page, rows, 0, new List<ContractGroupOperationLogDTO>());
        }

        /// <summary>
        /// 读取指定合同
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContractGroupDTO GetContractGroupById(Guid id)
        {
            var result = ContractGroupRepository.GetContractGroup(id);
            return _mapper.Map<ContractGroupDTO>(result);
        }

        /// <summary>
        /// 读取指定合同
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ContractGroupDTO GetContractGroupById(string id)
        {
            return _mapper.Map<ContractGroupDTO>(ContractGroupRepository.GetContract(id));
        }

        public ContractGroupDTO GetContractGroupByRelationData(string id)
        {
            return _mapper.Map<ContractGroupDTO>(ContractGroupRepository.GetContractByRelationData(id));
        }

        /// <summary>
        /// 修改合同状态
        /// </summary>
        /// <param name="contractGroupModel"></param>
        /// <returns></returns>
        public bool EditContractStatu(ContractGroupDTO contractGroupModel)
        {
            var contractGroup = _mapper.Map<ContractGroup>(contractGroupModel);
            UserContractRepository.Modify(contractGroup.UserContract.ToList(), new[] { "Statu", "BreakRemark", "ModifiedDate" }, false);
            ContractGroupRepository.Modify(new List<ContractGroup> { contractGroup }, new[] { "ModifiedDate", "Statu", "SyncStatus", "ContractTitle", "AllBreakStart", "BreakStart", "Break" }, false);
            return _unitOfWorkContext.Commit() > 0;
        }

        /// <summary>
        /// 合同详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserName"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public ServiceResult<bool> GetDetails(Guid id)
        {
            var contract = ContractGroupRepository.GetContractGroup(id);
            if (contract == null)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "此合同不存在");
            }
            return new ServiceResult<bool>(ServiceResultType.Success, SerializeHelper.ToJson(_mapper.Map<ContractGroupDTO>(contract), false, true));
        }

        /// <summary>
        /// 新增/编辑合同
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filename"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public ServiceResult<bool> EditCurrencySignService(ContractGroupDTO model, string filename, string currentUserId)
        {
            //查平台的编号
            var ckNo = ElectronicSignApiService.FindContractByNo(model.ContractNo, model.ReferenceId);
            if (!ckNo.Result)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "该合同编号’" + model.ContractNo + "'已被使用，请填写其他编号");
            }
            bool bl = false;
            model.SyncStatus = SyncStatus.SyncPending;
            if (model.Id == Guid.Empty)
            {
                if (model.Type != ContractType.Lending)
                {
                    model.Type = ContractType.Electronic;
                }
                //增加
                model.GroupId = Guid.NewGuid().ToString();
                model.Id = Guid.NewGuid();

                foreach (var t in model.UserContract)
                {
                    t.Id = Guid.NewGuid();
                    if (t.CustomerType == CustomerType.Personal)
                    {
                        t.Statu = UserContractStatus.Agree;
                    }
                }
                //if (model.UserContract.Any(m=>m.CustomerType== CustomerType.Personal))
                //{
                //    model.Type = ContractType.Seller;
                //}
                var contractGroupModel = _mapper.Map<ContractGroup>(model);
                if (ContractGroupRepository.Add(new List<ContractGroup> { contractGroupModel }) > 0)
                {
                    bl = true;
                }
                else
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
                }
            }
            else
            {
                //编辑当前合同
                var contractGroupModel = ContractGroupRepository.GetContractIncludeUserContract(m => !m.IsDeleted && m.Id.Equals(model.Id));
                if (contractGroupModel == null)
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
                }
                if (contractGroupModel.Type != ContractType.Electronic || contractGroupModel.UserId.ToLower() != model.UserId.ToLower() || contractGroupModel.SyncStatus == SyncStatus.SyncFailed)
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "您没有权限编辑");
                }
                if (contractGroupModel.UserContract.Any(m => m.Statu == UserContractStatus.Sign))
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "当前状态不可编辑");
                }
                //等待发起签署
                if (!model.BlobId.Equals(contractGroupModel.BlobId, StringComparison.OrdinalIgnoreCase))
                {
                    bl = true;
                }
                contractGroupModel.ContractNo = model.ContractNo;
                contractGroupModel.ContractTitle = model.ContractTitle;
                contractGroupModel.BlobId = model.BlobId;
                contractGroupModel.GroupId = model.GroupId;
                contractGroupModel.SyncStatus = model.SyncStatus;
                //等待上传方确认
                contractGroupModel.Statu = ContractStatus.WaitforSubmited;
                var userContractList = _mapper.Map<List<UserContract>>(model.UserContract);
                userContractList.ForEach(m =>
                {
                    m.Statu = UserContractStatus.Comfirm;
                    m.BlobId = contractGroupModel.Id;
                    m.Id = Guid.NewGuid();
                });
                var removeUserContract = contractGroupModel.UserContract.ToList();
                UserContractRepository.RemoveById(removeUserContract, false);
                UserContractRepository.Add(userContractList, false);
                var success = ContractGroupRepository.Modify(contractGroupModel);
                if (!success)
                    return new ServiceResult<bool>(ServiceResultType.Error, "操作失败，请稍后重试.");
            }
            #region 保存文件
            if (bl)
            {
                var success = BlobUtil.CopyTempsToClientBlob(Tenant, new List<string>() { model.BlobId }, currentUserId);
                return new ServiceResult<bool>(success ? ServiceResultType.Success : ServiceResultType.Error, success ? "提交成功" : "保存文件失败");
            }
            #endregion

            return new ServiceResult<bool>(ServiceResultType.Success, "提交成功");
        }

        public bool ModifyUserContractp(List<UserContract> model, bool bl)
        {
            return UserContractRepository.Modify(model, new[] { "UserId", "UserName", "Key", "Statu", "BreakRemark", "ModifiedDate" }, bl) > 0;
        }

        #region 作废合同

        public ServiceResult<bool> AbandonedContract(string currentUserName, string applyOperatorName, Guid id,
            string remark = "", string pdfX = "", string pdfY = "", string url = "")
        {
            if (string.IsNullOrEmpty(remark))
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "作废原因必填");
            }

            var contractGroup = ContractGroupRepository.GetContractGroup(id);
            if (contractGroup == null || contractGroup.UserContract == null || !contractGroup.UserContract.Any())
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
            }
            if (contractGroup.Statu == ContractStatus.Abandoned)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "该合同已作废不可重复作废");
            }

            var userContract = contractGroup.UserContract.FirstOrDefault(m => m.UserId.Equals(Tenant.TenantName, StringComparison.CurrentCultureIgnoreCase));
            if (userContract == null)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
            }
            if (!(userContract.Statu == UserContractStatus.Sign || userContract.Statu == UserContractStatus.WaitAbandoned) && (contractGroup.Statu == ContractStatus.Complete || contractGroup.Statu == ContractStatus.WaitCancel))
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "当前状态不可作废");
            }
            bool last = false;
            userContract.BreakRemark = remark;

            #region 判断“我方”是不是最后一个人作废（最后一个自动加上作废印章）
            if (contractGroup.UserContract.Count(m => !m.UserId.Equals(Tenant.TenantName,
                StringComparison.CurrentCultureIgnoreCase) && m.Statu == UserContractStatus.Abandoned) ==
                contractGroup.UserContract.Count - 1)
            {
                contractGroup.UserContract.ToList().ForEach(m =>
                {
                    userContract.Statu = UserContractStatus.Abandoned;
                });
                //更改所有合同状态
                last = true;
                contractGroup.Statu = ContractStatus.Abandoned;
                var userid = contractGroup.UserId;//TenantName
                var blobId = contractGroup.BlobId;
                var contractTenant = TenantUserApiService.GetTenantByName(userid).Result;
                var blob = BlobUtil.GetBlobById(contractTenant, userid, blobId, false);
                if (!(blob != null && blob.Data != null))
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "未找到文件");
                }
                //添加作废章
                var r = ElectronicSignApiService.AddSignatureToPdf(blob.Data, blob.FileName, pdfX,
                      pdfY, "", 2, "", "0", "0", Tenant.TenantName);
                if (!(r.Result != null && r.Result.Item1 == 100))
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
                }

                var encodeFileName = Base64Provider.EncodeString(r.Result.Item2);
                var blobInfo = new BlobInfo(blobId, FileType.PDF.ToString(), DocFormat.Pdf.ToString(), encodeFileName, "pdf", r.Result.Item3);
                BlobUtil.SaveBlob(contractTenant, userid, blobInfo);
            }
            else
            {

                if (contractGroup.Statu == ContractStatus.Complete)
                {
                    contractGroup.UserContract.ToList().ForEach(m =>
                    {
                        m.Statu = UserContractStatus.WaitAbandoned;
                    });
                }
                userContract.Statu = UserContractStatus.Abandoned;
                //更改所有合同状态
                contractGroup.Statu = ContractStatus.WaitCancel;
            }
            #endregion

            if (last)
            {
                #region 回调作废的url

                bool start = true;

                if (!string.IsNullOrEmpty(contractGroup.Break))
                {
                    var resultParam = new List<ReturnParam>();
                    foreach (var uC in contractGroup.UserContract)
                    {
                        var returnParam = new ReturnParam();
                        returnParam.blobId = contractGroup.BlobId;
                        returnParam.stampState = UserContractStatus.Abandoned.ToString();
                        returnParam.remark = uC.BreakRemark;
                        returnParam.keyword = uC.Key;
                        resultParam.Add(returnParam);
                    }
                    #region 单合同
                    Tuple<bool, string> result;
                    if (contractGroup.Type == ContractType.Agreement)
                    {
                        result = WebClientDownload1(contractGroup.Break + "&token=" + MD5Provider.Hash(contractGroup.BlobId), SerializeHelper.ToJson<List<ReturnParam>>(resultParam));
                    }
                    else
                    {
                        result = WebClientDownload1(url + contractGroup.Break + "&token=" + MD5Provider.Hash(contractGroup.BlobId), SerializeHelper.ToJson<List<ReturnParam>>(resultParam));
                    }

                    if (!result.Item1)
                    {
                        start = false;
                    }
                    #endregion
                }
                contractGroup.BreakStart = start;
                #endregion
            }
            //修改合同状态
            var bl = EditContract(contractGroup);
            if (!bl) return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
            //同步
            var reInt1 = ReApplyPlatFormConfirmCurrencySign(currentUserName, applyOperatorName, _mapper.Map<ContractGroupDTO>(contractGroup), true);
            if (reInt1 > 0)
            {
                #region 发送通知消息(作废合同)
                ////查出所有签署的公司
                //var tenantUserList1 = new List<string>();
                //tenantUserList1.AddRange(contractGroup.UserContract.Select(item => item.UserId));
                ////分别给每个公司发送消息
                //foreach (string t in tenantUserList1)
                //{
                //    var firstOrDefault = contractGroup.UserContract.FirstOrDefault(
                //        m => m.UserId.Equals(t, StringComparison.CurrentCultureIgnoreCase));
                //    if (firstOrDefault != null)
                //    {
                //        var email = firstOrDefault.CreatedBy;
                //        CurrencyApiService.SendContractMessage(t, contractGroup.Id, email, "Contract_Return", remark);
                //    }
                //}
                return new ServiceResult<bool>(ServiceResultType.Success, "操作成功");
                #endregion
            }
            return new ServiceResult<bool>(ServiceResultType.Error, "操作失败,请及时查看日志");
        }
        /// <summary>
        /// 修改合同状态
        /// </summary>
        /// <param name="contractGroupModel"></param>
        /// <returns></returns>
        public bool EditContract(ContractGroup contractGroupModel)
        {
            UserContractRepository.Modify(contractGroupModel.UserContract, new[] { "UserId", "UserName", "Key", "Statu", "BreakRemark", "ModifiedDate" }, false);
            ContractGroupRepository.Modify(contractGroupModel,
                    new[] { "BlobId", "ContractTitle", "ContractContent", "ContractFootnote","ModifiedDate","Statu","PId","SyncStatus","AllBreakStart","BreakStart" }
                    , false);
            return _unitOfWorkContext.Commit() > 0;
        }
        #endregion

        #region 删除合同
        /// <summary>
        /// 删除合同
        /// </summary>
        public ServiceResult<bool> RemoveCurrencySignService(Guid id, string currentUserName, string currentUserDisplayName)
        {
            var contractGroup = ContractGroupRepository.GetContractGroup(id);
            var contractGroupDto = _mapper.Map<ContractGroupDTO>(contractGroup);
            if (!(contractGroup != null && contractGroup.SyncStatus == SyncStatus.SyncSuccess && (contractGroup.Statu == ContractStatus.Abandoned || contractGroup.Statu == ContractStatus.Returned)))
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "合同数据没有同步成功，不能删除");
            }

            int delType = 0;
            //删除合同（先删除平台，再删除签署公司，最后删除自己）
            if (contractGroup.ReferenceId != null)
            {
                ElectronicSignApiService.RemoveCurrencySignService(contractGroup.ReferenceId);
                //if (result0 == null || !result0.success || !result0.Result) return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
            }
            else
            {
                delType = 1;
            }
            //删除签署公司和本地
            var res = ReApplyRemoveCurrencySignService(contractGroupDto, currentUserName, currentUserDisplayName, 0, delType);
            return !res ? new ServiceResult<bool>(ServiceResultType.Error, "操作失败，请查看日志") : new ServiceResult<bool>(ServiceResultType.Success, "操作成功");
        }

        public bool ReApplyRemoveCurrencySignService(ContractGroupDTO contractGroupDto, string currentUserName, string currentUserDisplayName, int processLogId = 0, int delType = 0)
        {
            //删除本地的数据
            if (!RemoveSingContractServices(contractGroupDto.Id))//删除本库失败
            {
                #region 新增删除本库失败日志
                var dlog = new ContractGroupOperationLog()
                {
                    ContractGroupId = contractGroupDto.Id,
                    ContractGroupProgress = ContractGroupProgress.DelToCreateFail,
                    OperatorId = currentUserName,
                    Operator = currentUserDisplayName,
                    OperateDate = DateTime.UtcNow.ToLocalDateTime(),
                    Type = ProcessLogType.Failure,
                    NotContractGroupUsers = Tenant.TenantName,
                    ToPlatFormContractGroup = SyncStatus.SyncSuccess,
                    Remark =
                            string.Format("合同标题（{0}），合同编号（{1}）删除失败", contractGroupDto.ContractTitle,
                        contractGroupDto.ContractNo)
                };
                ContractGroupOperationLogRepository.Add(dlog);
                contractGroupDto.UserContract = null;
                contractGroupDto.ContractGroupOperationLog = null;
                ContractGroupRepository.Modify(_mapper.Map<ContractGroup>(contractGroupDto), new[] { "SyncStatus", "ModifiedBy", "ModifiedDate" });
                return false;

                #endregion
            }
            return true;

        }

        public static Tuple<bool, string> WebClientDownload1(string url, string postString)
        {
            return HttpWebRequestHelper.WebClientDownload(url, postString);
        }

        #endregion

        #region 退回合同(同步平台和签署公司同“提交合同”)
        public ServiceResult<bool> RetutrnContract(string currentUserName, string applyOperatorName, Guid id, string remark = "", string url = "", string userId = "")
        {
            if (string.IsNullOrEmpty(remark))
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "退回原因必填");
            }

            var contractGroup = ContractGroupRepository.GetContractGroup(id);
            if (contractGroup == null || contractGroup.UserContract == null || !contractGroup.UserContract.Any())
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
            }
            if (contractGroup.Statu == ContractStatus.WaitSign)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "已全部确认，不可退回");
            }
            var userContract = contractGroup.UserContract.FirstOrDefault(m => m.UserId.Equals(Tenant.TenantName, StringComparison.CurrentCultureIgnoreCase));
            if (userContract == null)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
            }
            if (userContract.Statu == UserContractStatus.Sign)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "您已盖章，不可退回");
            }

            contractGroup.UserContract.ToList().ForEach(m =>
            {
                userContract.Statu = userContract.UserId.Equals(m.UserId,
                    StringComparison.CurrentCultureIgnoreCase) ? UserContractStatus.Disagree : UserContractStatus.Comfirm;
            });
            userContract.BreakRemark = remark;
            //更改所有合同状态
            contractGroup.Statu = ContractStatus.Returned;

            #region 回调退回的url

            bool start = true;

            if (!string.IsNullOrEmpty(contractGroup.Break))
            {
                var resultParam = new List<ReturnParam>();
                foreach (var uC in contractGroup.UserContract)
                {
                    var returnParam = new ReturnParam();
                    returnParam.blobId = contractGroup.BlobId;
                    returnParam.stampState = UserContractStatus.Disagree.ToString();
                    returnParam.remark = uC.BreakRemark;
                    returnParam.keyword = uC.Key;
                    resultParam.Add(returnParam);
                }
                #region 单合同
                Tuple<bool, string> result;
                if (contractGroup.Type == ContractType.Agreement)
                {
                    result = WebClientDownload1(contractGroup.Break + "&token=" + MD5Provider.Hash(contractGroup.BlobId), SerializeHelper.ToJson<List<ReturnParam>>(resultParam));
                }
                else
                {
                    url = url + contractGroup.Break + "&token=" + MD5Provider.Hash(contractGroup.BlobId);
                    result = WebClientDownload1(url, SerializeHelper.ToJson<List<ReturnParam>>(resultParam));
                    //result = HttpHelp.WebClientDownload(url, SerializeHelper.ToJson<List<ReturnParam>>(resultParam));
                }

                if (!result.Item1)
                {
                    start = false;
                }
                #endregion
            }
            contractGroup.BreakStart = start;
            #endregion

            //修改合同状态
            var bl = EditContract(contractGroup);
            if (!bl) return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
            //同步(如果是上传方退回，则不需要同步)

            var isTo = false;
            if (contractGroup.UserId.Equals(Tenant.TenantName, StringComparison.CurrentCultureIgnoreCase))
            {
                isTo = true;
            }
            if (isTo)
            {

                #region 发送通知消息(退回合同)
                //查出所有签署的公司
                var tenantUserList1 = new List<string>();
                tenantUserList1.AddRange(contractGroup.UserContract.Select(item => item.UserId));
                //分别给每个公司发送消息
                foreach (string t in tenantUserList1)
                {
                    var firstOrDefault = contractGroup.UserContract.FirstOrDefault(
                        m => m.UserId.Equals(t, StringComparison.CurrentCultureIgnoreCase));
                    if (firstOrDefault != null)
                    {
                        var email = firstOrDefault.CreatedBy;
                        ContractApiService.SendContractMessage(t, contractGroup.Id, email, "Contract_Return", remark);
                    }
                }
                return new ServiceResult<bool>(ServiceResultType.Success, "操作成功");
                #endregion
            }
            return new ServiceResult<bool>(ServiceResultType.Error, "操作失败,请及时查看日志");
        }

        #region 订单合同

        public ServiceResult<bool> CheckContractStatu(string orderInfoId, string currentUserId, string currentUserName)
        {
            var result = ContractGroupRepository.GetByFilter(m => m.RelationData.Equals(orderInfoId) && m.Statu != ContractStatus.Returned && m.Statu != ContractStatus.Abandoned);
            if (result != null)
            {
                if (result.Statu == ContractStatus.WaitforSubmited)
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "您已上传，等耐心等待审核", result.Id.ToString());
                }
                if (result.Statu == ContractStatus.WaitSign)
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "您已上传，等耐心等待盖章", result.Id.ToString());
                }
                if (result.Statu == ContractStatus.Returned)
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "（注意：您上传的订单合同审核未通过，请重新上传订单合同）", result.Id.ToString());
                }
            }
            return new ServiceResult<bool>(ServiceResultType.Success, "成功");
        }


        /// <summary>
        /// 订单取消，更新合同的作废状态
        /// </summary>
        /// <param name="orderId">平台订单号</param>
        /// <returns></returns>
        public ServiceResult<bool> OrderUpContractStatu(string orderId, string currentUserName, string applyOperatorName)
        {
            var contract = ContractGroupRepository.GetContractIncludeUserContract(m => m.RelationData.Equals(orderId) && !m.IsDeleted);
            if (contract == null || contract.UserContract == null)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "该合同不存在");
            }
            contract.Statu = ContractStatus.Abandoned;
            ContractGroupRepository.Modify(contract, new[] { "Statu" });
            var reInt1 = ReApplyPlatFormConfirmCurrencySign(currentUserName, applyOperatorName, _mapper.Map<ContractGroupDTO>(contract), true);
            if (reInt1 > 0)
            {
                return new ServiceResult<bool>(ServiceResultType.Success, "成功");
            }
            return new ServiceResult<bool>(ServiceResultType.Error, "操作失败,请及时查看日志");
        }
        #endregion

        #region 盖章(同步平台和签署公司同“提交合同”)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUserName"></param>
        /// <param name="applyOperatorName"></param>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <param name="pdfX"></param>
        /// <param name="pdfY"></param>
        /// <param name="posPage"></param>
        /// <param name="signType">签章类型:1单页签章,2多页签章，3骑缝章，4关键字签章</param>
        /// <returns></returns>
        public ServiceResult<bool> SignContract(string currentUserid, string currentUserName, string applyOperatorName, bool personal, string code, Guid id, string url = "", int pdfX = 0, int pdfY = 0, string posPage = "0", int signType = 1)
        {
            var unSignCount = 0;//当前还有几方未盖章
            var modifyContractResult = false; //修改合同状态结果
            var lastSign = false;//最后一方盖章
            var lastSignBackResult = true;//盖章完成回调结果
            #region 条件判断

            if (string.IsNullOrEmpty(code))
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "验证码不能为空");
            }
            var resultParam = new List<ReturnParam>();
            var contractGroup = ContractGroupRepository.GetContractGroup(id);
            if (contractGroup == null || contractGroup.Statu != ContractStatus.WaitSign)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "当前状态不能盖章");
            }
            if (contractGroup.SyncStatus != SyncStatus.SyncSuccess)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "当前状态不可操作，请刷新页面");
            }
            var userContract = contractGroup.UserContract.FirstOrDefault(m => m.UserId.Equals(Tenant.TenantName, StringComparison.OrdinalIgnoreCase));
            if (userContract == null)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
            }
            if (userContract.Statu != UserContractStatus.WaitSign)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "当前状态不可操作，请刷新页面");
            }
            var userId = contractGroup.UserId;//TenantName
            var blobId = contractGroup.BlobId;

            var contractTenant = TenantUserApiService.GetTenantByName(userId).Result;
            var blob = BlobUtil.GetBlobById(contractTenant, userId, blobId, false);
            if (blob == null || blob.Data == null)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "未找到文件");
            }
            #endregion

            foreach (var uC in contractGroup.UserContract)
            {
                if (uC.Statu != UserContractStatus.Sign)
                {
                    unSignCount++;
                }
                var returnParam = new ReturnParam();
                returnParam.blobId = contractGroup.BlobId;
                returnParam.stampState = UserContractStatus.Sign.ToString();
                returnParam.remark = uC.BreakRemark;
                returnParam.keyword = uC.Key;
                resultParam.Add(returnParam);
            }

            lastSign = unSignCount == 1;

            if (unSignCount == contractGroup.UserContract.Count)//第一次盖章
            {
                var stream = new MemoryStream(blob.Data);
                var pdf = ThirdParty.WatermarkHelper.AddWatermarkToPdf(stream, "https://www.starlu.com/ " + System.DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"));
                blob.Data = pdf;
            }

            #region 盖章参数

            var XCoordinate = 140;//骑缝章的y轴坐标
            var j = 0;
            var userContractOrderbyIdList = contractGroup.UserContract.OrderBy(m => m.UserId);
            foreach (var uc in userContractOrderbyIdList)
            {
                j++;
                if (uc.UserId.Equals(Tenant.TenantName, StringComparison.OrdinalIgnoreCase))
                {
                    XCoordinate = XCoordinate * j;
                }
            }
            var isKey = userContract.Key;
            if (signType == 4)//关键字盖章
            {
                pdfX = 10;
            }
            else
            {
                isKey = "";
            }
            #endregion

            //若未生成，弹出一个提示框：“融资租赁三方买卖合同待生成，请留意站内信或邮件通知。” 
            //若已生成，提示“请签署融资租赁三方买卖合同。立即前往 取消”
            bool financeLeaseContract = false;//融资租赁交易合同，三方合同
            var modifyOrderBranchStateResult = true;//交易合同，修改订单分支状态结果
            
            string TempUsetId = Tenant.TenantName;
            if (personal)
            {
                TempUsetId = currentUserid;
            }
            var signResult = ElectronicSignApiService.AddSignatureToPdf(blob.Data, blob.FileName, pdfX.ToString(), pdfY.ToString(), isKey, signType, code, posPage, XCoordinate.ToString(), TempUsetId, personal);

            if (signResult.Result == null)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "盖章失败");
            }
            if (signResult.Result.Item1 != 100)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, signResult.Result.Item2);
            }
            #region 更改当公司合同盖章的状态

            if (!lastSign) //当前合同至少还有两个人没盖章
            {
                userContract.Statu = UserContractStatus.Sign;
                var userContractList = new List<UserContract>();
                userContractList.Add(userContract);
                modifyContractResult = ModifyUserContractp(userContractList, true);
            }
            else//最后一个盖章
            {
                contractGroup.Statu = ContractStatus.Complete;
                contractGroup.UserContract.ToList().ForEach(m => m.Statu = UserContractStatus.Sign);
                //最后一个盖章，回调AllBreak Url
                if (!string.IsNullOrEmpty(contractGroup.AllBreak))
                {

                    Tuple<bool, string> result;
                    if (contractGroup.Type == ContractType.Agreement)
                    {
                        result =
                            WebClientDownload1(
                                contractGroup.AllBreak + "&token=" + MD5Provider.Hash(contractGroup.BlobId),
                                SerializeHelper.ToJson<List<ReturnParam>>(resultParam));
                        lastSignBackResult = result.Item1;
                    }
                    else if (contractGroup.Type != ContractType.AccountStatement)//对账单特殊处理
                    {
                        result =
                            WebClientDownload1(
                                url + contractGroup.AllBreak + "&token=" +
                                MD5Provider.Hash(contractGroup.BlobId),
                                SerializeHelper.ToJson<List<ReturnParam>>(resultParam));
                        lastSignBackResult = result.Item1;
                    }
                }
                contractGroup.AllBreakStart = lastSignBackResult;

                modifyContractResult = EditContract(contractGroup);
            }

            #endregion

            if (!modifyContractResult)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "盖章成功，但是修改业务数据失败！");
            }

            #region 盖章成功之后，保存文件，修改数据状态

            var encodeFileName = Base64Provider.EncodeString(signResult.Result.Item2);
            var blobInfo = new BlobInfo(blobId, FileType.PDF.ToString(), DocFormat.Pdf.ToString(), encodeFileName, "pdf", signResult.Result.Item3);
            BlobUtil.SaveBlob(contractTenant, userId, blobInfo);
            //先修改平台合同数据，再修改关联方合同数据
            ReApplyPlatFormConfirmCurrencySign(currentUserName, applyOperatorName, _mapper.Map<ContractGroupDTO>(contractGroup), true);
            if (!lastSignBackResult)
            {
                #region 新增签署之后回调失败日志

                var dlog = new ContractGroupOperationLog()
                {
                    ContractGroupId = contractGroup.Id,
                    ContractGroupProgress = ContractGroupProgress.UpdateToCallBackFail,
                    OperatorId = currentUserName,
                    Operator = applyOperatorName,
                    OperateDate = DateTime.UtcNow.ToLocalDateTime(),
                    Type = ProcessLogType.Failure,
                    NotContractGroupUsers = "",
                    ToPlatFormContractGroup = SyncStatus.SyncSuccess,
                    Remark = contractGroup.Statu.ToDisplayName() + " (同步业务)"
                };
                ContractGroupOperationLogRepository.Add(dlog, false);

                #endregion
            }

            #region 发送通知消息(签署成功)
            //查出所有签署的公司
            var tenantUserList = new List<string>();
            tenantUserList.AddRange(contractGroup.UserContract.Select(item => item.UserId));
            //分别给每个公司发送消息
            foreach (string t in tenantUserList)
            {
                var firstOrDefault = contractGroup.UserContract.FirstOrDefault(
                    m => m.UserId.Equals(t, StringComparison.CurrentCultureIgnoreCase));
                if (firstOrDefault != null)
                {
                    var email = firstOrDefault.CreatedBy;
                    //CurrencyApiService.SendContractMessage(t, contractGroup.Id, email, "Contract_Sign");
                }
            }
            #endregion

            #region 系统收费合同，专用提示语
            var systemFeeContract = contractGroup.Type == ContractType.Electronic
                && (contractGroup.ContractTitle == "产融协作系统增值服务合同" || contractGroup.ContractTitle == "云服务协议")
                && !string.IsNullOrEmpty(contractGroup.RelationData);
            if (systemFeeContract)
            {
                return new ServiceResult<bool>(ServiceResultType.NoChanged, "合同签署成功！请前往完成支付，谢谢！", "");
            }
            #endregion

            #region 对账单盖章，特殊处理，每一方盖章之后都需要回调

            if (contractGroup.Type == ContractType.AccountStatement)
            {
                var postUrl = string.Format("{0}&token={1}&userDisplayName={2}", url + contractGroup.AllBreak, MD5Provider.Hash(contractGroup.BlobId), applyOperatorName);
                WebClientDownload1(postUrl, string.Empty);
            }

            #endregion

            return new ServiceResult<bool>(ServiceResultType.Success, "操作成功");
            #endregion
        }


        public ServiceResult<bool> SignContractFun(string currentUserid, string currentUserName, string applyOperatorName, bool personal, string code, Guid id, string url = "", int pdfX = 0, int pdfY = 0, string posPage = "0", int signType = 1)
        {
            var lockKey = "SignContract+" + id;
            var result = new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
            new KC.Component.Util.RedisDistributedLock().DoDistributedLock(lockKey, () =>
            {
                result = SignContract(currentUserid, currentUserName, applyOperatorName, personal, code, id, url, pdfX, pdfY, posPage, signType);
            });
            return result;
        }

        #endregion

        #region  审核合同 (同步平台和签署公司同“提交合同”)
        public ServiceResult<bool> ComfirmContract(string applyOperatorId, string applyOperatorName, Guid id, bool isComfirmFrist = false, string SignAdress = "")
        {
            //非上传人发起审核 合同Id
            var contract = ContractGroupRepository.GetContractGroup(id);
            if (contract == null)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "该合同不存在");
            }

            //判断该编号是否存在contractNo
            //查平台的编号
            var ckNo = ElectronicSignApiService.FindContractByNo(contract.ContractNo, contract.ReferenceId);
            if (!ckNo.Result)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "该合同编号’" + contract.ContractNo + "'已被使用，请填写其他编号");
            }

            if (isComfirmFrist)
            {
                #region 上传方审核
                if (contract.Statu != ContractStatus.WaitforSubmited ||
                    !contract.UserId.Equals(Tenant.TenantName, StringComparison.OrdinalIgnoreCase))
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "您没有权限操作");
                }
                contract.SyncStatus = SyncStatus.SyncPending;
                //发起签署
                if (contract.UserContract.Any())
                {
                    contract.UserContract.ToList().ForEach(m =>
                    {
                        if (m.UserId.Equals(contract.UserId, StringComparison.OrdinalIgnoreCase))
                        {
                            m.Statu = UserContractStatus.Agree;
                        }
                        else
                        {
                            m.Statu = UserContractStatus.Submited;
                        }
                        if (m.CustomerType == CustomerType.Personal)
                        {
                            m.Statu = UserContractStatus.Agree;
                        }
                    });
                    var personalCount = contract.UserContract.Count(m => m.CustomerType == CustomerType.Personal);
                    if (personalCount == contract.UserContract.Count() - 1)
                    {
                        contract.Statu = ContractStatus.WaitSign;
                        contract.UserContract.ToList().ForEach(m =>
                        {
                            m.Statu = UserContractStatus.WaitSign;
                        });
                    }
                }
                var result = EditContract(contract);
                if (!result) return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
                //同步
                var reInt = ReApplyPlatFormConfirmCurrencySign(applyOperatorId, applyOperatorName, _mapper.Map<ContractGroupDTO>(contract));
                if (reInt <= 0) return new ServiceResult<bool>(ServiceResultType.Error, "操作失败,请及时查看日志");

                #region 发送通知消息(发起签署)

                //查出所有签署的公司
                var tenantUserList = new List<string>();
                tenantUserList.AddRange(contract.UserContract.Select(item => item.UserId));
                //分别给每个公司发送消息
                foreach (var t in contract.UserContract)
                {
                    switch (t.CustomerType)
                    {
                        case CustomerType.Personal:
                            //if (!string.IsNullOrEmpty(t.StaffId))
                            //{
                            //    var firstPersonal = contract.UserContract.FirstOrDefault(m => m.UserId.Equals(t.UserId, StringComparison.CurrentCultureIgnoreCase));
                            //    if (firstPersonal != null)
                            //    {
                            //        var email = firstPersonal.CreatedBy;
                            //        CurrencyApiService.SendContractMessage(t.UserId, contract.Id, email);
                            //    }
                            //}
                            break;
                        case CustomerType.Organization:
                        case CustomerType.Company:
                            var firstOrDefault = contract.UserContract.FirstOrDefault(m => m.UserId.Equals(t.UserId, StringComparison.CurrentCultureIgnoreCase));
                            if (firstOrDefault != null)
                            {
                                var email = firstOrDefault.CreatedBy;
                                ContractApiService.SendContractMessage(t.UserId, contract.Id, email);
                            }
                            break;
                    }
                }

                #endregion

                #endregion

                return new ServiceResult<bool>(ServiceResultType.Success, "操作成功");
            }
            else
            {
                #region 非发起方审核合同
                //合同用户Id
                var userContract = UserContractRepository.GetByFilter(m => m.BlobId == id && m.UserId.Equals(Tenant.TenantName, StringComparison.CurrentCultureIgnoreCase));
                if (userContract == null)
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
                }
                if (userContract.Statu != UserContractStatus.Submited)
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "当前状态不可操作，请刷新页面");
                }
                contract.SyncStatus = SyncStatus.SyncPending;

                var i = contract.UserContract.Count(uC => uC.Statu != UserContractStatus.Agree);
                //判断是不是最后一个确认合同
                var bl = false;
                if (i > 1)
                {
                    //不是最后一个
                    //上传用户确认 发起签署之后确认合同
                    userContract.Statu = UserContractStatus.Agree;
                    var userContractList = new List<UserContract>();
                    userContractList.Add(userContract);
                    bl = ModifyUserContractp(userContractList, true);
                }
                else
                {
                    contract.UserContract.ToList().ForEach(m => m.Statu = UserContractStatus.WaitSign);
                    contract.Statu = ContractStatus.WaitSign;
                    //修改合同状态
                    bl = EditContract(contract);
                }
                //给买家发送站内信和邮件
                bool isSendbuy = false;
                var buySourceOrder = string.Empty;
                var buytenant = string.Empty;
                if (!bl) return new ServiceResult<bool>(ServiceResultType.Error, "操作失败");
                //同步
                var reInt1 = ReApplyPlatFormConfirmCurrencySign(applyOperatorId, applyOperatorName, _mapper.Map<ContractGroupDTO>(contract), true);
                if (reInt1 <= 0) return new ServiceResult<bool>(ServiceResultType.Error, "操作失败,请及时查看日志");

                #region 发送通知消息(确认合同)
                //分别给每个公司发送消息
                foreach (var t in contract.UserContract)
                {
                    switch (t.CustomerType)
                    {
                        case CustomerType.Personal:
                            break;
                        case CustomerType.Organization:
                        case CustomerType.Company:
                            var firstOrDefault = contract.UserContract.FirstOrDefault(m => m.UserId.Equals(t.UserId, StringComparison.CurrentCultureIgnoreCase));
                            if (firstOrDefault != null)
                            {
                                var email = firstOrDefault.CreatedBy;
                                ContractApiService.SendContractMessage(t.UserId, contract.Id, email, "Contract_Yes");
                            }
                            break;
                    }
                }
                #endregion
                #endregion
            }

            return new ServiceResult<bool>(ServiceResultType.Success, "操作成功");
        }

        #endregion
        #endregion

        #region 提交合同（同步平台，签署公司）

        /// <summary>
        /// 先修改平台合同状态，再修改关联方合同状态
        /// </summary>
        /// <param name="applyOperatorId"></param>
        /// <param name="applyOperatorName"></param>
        /// <param name="contractGroup"></param>
        /// <param name="isStatu">true:修改合同数据，false:新增合同数据</param>
        /// <returns></returns>
        public int ReApplyPlatFormConfirmCurrencySign(string applyOperatorId, string applyOperatorName, ContractGroupDTO contractGroup, bool isStatu = false)
        {
            try
            {
                var result0 = ElectronicSignApiService.ConfirmCurrencySign(contractGroup, isStatu);
                var success = result0 != null && result0.Result > 0;

                var dlog = new ContractGroupOperationLog()
                {
                    ContractGroupId = contractGroup.Id,
                    ContractGroupProgress = success ? ContractGroupProgress.Success : ContractGroupProgress.UpdateToPlatFormFail,
                    OperatorId = applyOperatorId,
                    Operator = applyOperatorName,
                    OperateDate = DateTime.UtcNow.ToLocalDateTime(),
                    Type = success ? ProcessLogType.Success : ProcessLogType.Failure,
                    NotContractGroupUsers = "",
                    ToPlatFormContractGroup = success ? SyncStatus.SyncSuccess : SyncStatus.SyncFailed,
                    Remark = contractGroup.Statu.ToDisplayName() + " (同步平台)"
                };
                ContractGroupOperationLogRepository.Add(dlog, false);
                var contractGroupModify = ContractGroupRepository.GetContractGroup(contractGroup.Id);
                contractGroupModify.SyncStatus = success ? SyncStatus.SyncSuccess : SyncStatus.SyncFailed;
                contractGroupModify.UserContract = null;
                contractGroupModify.ContractGroupOperationLog = null;
                if (success)
                    contractGroupModify.ReferenceId = result0.Result.ToString();
                ContractGroupRepository.Modify(contractGroupModify, new[] { "ReferenceId", "SyncStatus", "ModifiedBy", "ModifiedDate" },
                    false);
                var result = _unitOfWorkContext.Commit();
                if (result > 0 && success)//需同步数据到合同关联方
                {
                    var reBool = ReApplyConfirmCurrencySign(_mapper.Map<ContractGroupDTO>(contractGroupModify), applyOperatorId, applyOperatorName, 0, isStatu);
                    if (!reBool)
                    {
                        return -1;//同步到签署公司失败
                    }
                }
                return result0.Result;
            }
            catch (Exception ex)
            {
                return 0;
                throw ex;
            }
        }

        /// <summary>
        /// 合同签署后，修改关联方数据状态
        /// </summary>
        /// <param name="contractGroup"></param>
        /// <param name="applyOperatorId"></param>
        /// <param name="applyOperatorName"></param>
        /// <param name="processLogId"></param>
        /// <param name="isStatu">true:修改合同数据，false:新增合同数据</param>
        /// <returns></returns>
        public bool ReApplyConfirmCurrencySign(ContractGroupDTO contractGroup, string applyOperatorId, string applyOperatorName, int processLogId = 0, bool isStatu = false)
        {
            #region 更新之前的错误日志状态
            var operationLoglist = ContractGroupOperationLogRepository.FindAll(m => m.ContractGroupId == contractGroup.Id && m.Type > 0);
            if (operationLoglist.Any())
            {
                foreach (var t in operationLoglist)
                {
                    t.Type = ProcessLogType.Success;
                }
                ContractGroupOperationLogRepository.Modify(operationLoglist, new[] { "Type" }, false);
            }
            #endregion
            var contractGroupModify = _mapper.Map<ContractGroup>(contractGroup);
            var tenantUserList = new List<UserContract>();
            if (processLogId != 0)//部分公司未同步成功
            {
                var operationLog = ContractGroupOperationLogRepository.GetById(processLogId);
                tenantUserList = contractGroupModify.UserContract.Where(m => m.UserId.Contains(operationLog.NotContractGroupUsers)).ToList();
            }
            else
            {
                var mapUserlist = _mapper.Map<List<UserContract>>(contractGroup.UserContract.Where(m => !m.UserId.Equals(Tenant.TenantName, StringComparison.OrdinalIgnoreCase)));
                tenantUserList = mapUserlist;
            }
            var noticeFailDisplayNames = string.Empty;
            var noticeFails = string.Empty;
            contractGroup.SyncStatus = SyncStatus.SyncSuccess;
            contractGroup.ContractGroupOperationLog = null;
            var currencySignApiService = new ServiceResult<bool>();
            foreach (var t in tenantUserList)
            {
                if (t.CustomerType == CustomerType.Company || !string.IsNullOrEmpty(t.StaffId))
                {
                    try
                    {
                        if (isStatu)//修改合同数据
                        {
                            currencySignApiService = ContractApiService.EditContractStatuTo(contractGroup, t.UserId);
                        }
                        else//新增合同数据
                        {
                            #region 如果是销售合同，更新同步类型为采购合同

                            if (contractGroup.Type == ContractType.Seller)
                            {
                                contractGroup.Type = ContractType.Purchase;
                            }
                            //else if (contractGroup.Type == ContractType.Purchase)
                            //{
                            //    contractGroup.Type = ContractType.Seller;
                            //}
                            #endregion
                            currencySignApiService = ContractApiService.UpdateContractGroupTo(contractGroup, t.UserId);

                        }
                        if (!(currencySignApiService != null && currencySignApiService.success && currencySignApiService.Result))
                        {
                            noticeFails += t + ",";
                            noticeFailDisplayNames += t.UserName + ",";
                        }
                    }
                    catch (Exception)
                    {
                        noticeFails += t + ",";
                        noticeFailDisplayNames += t.UserName + ",";
                    }
                }

            }
            contractGroupModify = ContractGroupRepository.GetContractGroup(contractGroup.Id);
            var existsNoticeFail = !string.IsNullOrWhiteSpace(noticeFails);
            var dlog = new ContractGroupOperationLog()
            {
                ContractGroupId = contractGroupModify.Id,
                ContractGroupProgress = existsNoticeFail ? ContractGroupProgress.UpdateToOtherFail : ContractGroupProgress.Success,
                OperatorId = applyOperatorId,
                Operator = applyOperatorName,
                OperateDate = DateTime.UtcNow.ToLocalDateTime(),
                Type = existsNoticeFail ? ProcessLogType.Failure : ProcessLogType.Success,
                NotContractGroupUsers = noticeFails.TrimEnd(','),
                ToPlatFormContractGroup = SyncStatus.SyncSuccess,
                Remark = contractGroup.Statu.ToDisplayName()
            };
            if (existsNoticeFail)
                dlog.Remark = string.Format(contractGroup.Statu.ToDisplayName() + "（{0}）", noticeFailDisplayNames.TrimEnd(','));
            ContractGroupOperationLogRepository.Add(dlog, false);
            contractGroupModify.SyncStatus = existsNoticeFail ? SyncStatus.SyncFailed : SyncStatus.SyncSuccess;
            contractGroupModify.UserContract = null;
            contractGroupModify.ContractGroupOperationLog = null;
            ContractGroupRepository.Modify(contractGroupModify, new[] { "SyncStatus", "ModifiedBy", "ModifiedDate" }, false);
            _unitOfWorkContext.Commit();
            return !existsNoticeFail;
        }
        #endregion

        #region 处理合同盖完回调api失败（saas目前只有单合同回调）

        public bool HandleSignCallbackFail(Guid id, string url)
        {
            var contractGroupModel = GetContractGroupById(id);
            if (contractGroupModel == null)
            {
                return false;
            }
            if (!(contractGroupModel.BreakStart || contractGroupModel.AllBreakStart))
            {
                return false;
            }
            var result = new Tuple<bool, string>(false, null);
            var resultParam = new List<ReturnParam>();
            foreach (var uC in contractGroupModel.UserContract)
            {
                var returnParam = new ReturnParam();
                returnParam.blobId = contractGroupModel.BlobId;
                returnParam.stampState = UserContractStatus.Sign.ToString();
                returnParam.remark = uC.BreakRemark;
                returnParam.keyword = uC.Key;
                resultParam.Add(returnParam);
            }
            #region 回调
            var postUrl = string.Empty;
            if (!contractGroupModel.AllBreakStart)
            {
                if (contractGroupModel.Type == ContractType.Agreement)
                {
                    postUrl = contractGroupModel.AllBreak + "&token=" + MD5Provider.Hash(contractGroupModel.BlobId);
                }
                else
                {
                    postUrl = url + contractGroupModel.AllBreak + "&token=" + MD5Provider.Hash(contractGroupModel.BlobId);
                }
                contractGroupModel.AllBreakStart = true;
            }
            if (!contractGroupModel.BreakStart)
            {
                if (contractGroupModel.Type == ContractType.Agreement)
                {
                    postUrl = contractGroupModel.Break + "&token=" + MD5Provider.Hash(contractGroupModel.BlobId);
                }
                else
                {
                    postUrl = url + contractGroupModel.Break + "&token=" + MD5Provider.Hash(contractGroupModel.BlobId);
                }
                contractGroupModel.BreakStart = true;
            }
            result = WebClientDownload1(postUrl, SerializeHelper.ToJson<List<ReturnParam>>(resultParam));

            #endregion

            if (result.Item1)
            {
                if (EditContract(_mapper.Map<ContractGroup>(contractGroupModel)))
                {
                    #region 更新之前的错误日志状态
                    var operationLoglist = ContractGroupOperationLogRepository.FindAll(
                         m => m.ContractGroupId == id && m.Type > 0);
                    if (operationLoglist.Count > 0)
                    {
                        foreach (var t in operationLoglist)
                        {
                            t.Type = ProcessLogType.Success;
                        }
                        ContractGroupOperationLogRepository.Modify(operationLoglist, new[] { "Type" });
                    }
                    #endregion
                }
                return true;
            }
            return false;
        }

        #endregion
        #endregion

        #region 合同模板
        public List<ContractTemplateDTO> LoadContractTemplates()
        {
            return _mapper.Map<List<ContractTemplateDTO>>(ContractTemplateRepository.FindAll(m => !m.TransactionTypeName.Contains("融资租赁")).OrderBy(m => m.Id).ToList());
        }

        /// <summary>
        /// 同步读取合同模板数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<List<ContractTemplateDTO>> GetContractTemplate(ContractType? type)
        {
            Expression<Func<ContractTemplate, bool>> predicate = m => !m.TransactionTypeName.Contains("融资租赁");
            if (type.HasValue)
            {
                predicate = predicate.And(m => m.Type == type.Value);
            }
            var result = await ContractTemplateRepository.FindAllAsync(predicate);
            var mapModel = _mapper.Map<List<ContractTemplateDTO>>(result);
            return mapModel;
        }
        public ContractTemplateDTO GetContractTemplateById(int id)
        {
            var data = ContractTemplateRepository.GetById(id);
            return _mapper.Map<ContractTemplateDTO>(data);
        }
        /// <summary>
        /// 读取指定合同模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ContractTemplateValueDTO> GetMyContractTemplateById(int id)
        {
            ContractTemplate model = await ContractTemplateRepository.GetByIdAsync(id);
            if (model != null && !string.IsNullOrEmpty(model.ContractValue))
            {
                return Common.SerializeHelper.FromJson<ContractTemplateValueDTO>(model.ContractValue);
            }
            return new ContractTemplateValueDTO();
        }

        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ReturnMessageDTO> ModifyContractTemplate(ContractTemplateValueDTO model)
        {
            var msg = new ReturnMessageDTO() { Success = false };
            if (model == null)
            {
                msg.ErrorMessage = "传入对象不能为空！";
                return msg;
            }
            var result = ContractTemplateRepository.GetById(model.Id);
            if (result != null)
            {
                result.ContractValue = JsonConvert.SerializeObject(model);
            }
            msg.Success = await ContractTemplateRepository.ModifyAsync(result, new[] { "ContractValue" });
            return msg;
        }

        public bool SaveContractTemplate(ContractTemplateValueDTO model)
        {
            var result = ContractTemplateRepository.GetById(model.Id);
            if (result != null)
            {
                result.ContractValue = JsonConvert.SerializeObject(model);
            }
            return ContractTemplateRepository.Modify(result);
        }

        public ServiceResult<bool> MakeAddValueServiceContract(string currentUserId, string currentUserName, string fileName, string blobId, string contractNo, decimal money)
        {
            var result = ContractGroupRepository.GetByFilter(m => m.BlobId.Equals(blobId) && m.Statu != ContractStatus.Complete && m.Type == ContractType.Electronic);
            if (result != null)
            {
                return new ServiceResult<bool>(ServiceResultType.Error, "该合同已生成，请立即签署");
            }
            var keyX = "甲方（盖章）";
            var keyY = "乙方（盖章）";
            var contract = new ContractGroup();
            contract.Id = Guid.NewGuid();
            contract.ContractNo = contractNo;
            contract.ContractTitle = fileName;
            contract.BlobId = blobId;
            contract.UserId = Tenant.TenantName;
            contract.UserName = Tenant.TenantDisplayName;
            contract.CreatedBy = currentUserName;
            contract.Statu = ContractStatus.WaitSign;
            contract.GroupId = Guid.NewGuid().ToString();
            contract.Type = ContractType.Electronic;
            contract.Break = "Admin/AdminHome/CallBackValueAdded?contractId=" + contract.Id + "&amount=" + money;
            contract.AllBreak = contract.Break;
            contract.RelationData = money.ToString();
            contract.SyncStatus = SyncStatus.SyncSuccess;

            var userContracts = new List<UserContract>();
            userContracts.Add(new UserContract { CreatedBy = currentUserName, Id = Guid.NewGuid(), UserId = Tenant.TenantName, UserName = Tenant.TenantDisplayName, Key = keyX, CustomerType = CustomerType.Company, Statu = UserContractStatus.WaitSign });
            //大陆之星ur2014101000011 GetStarluTenantName()
            var dalu = "ur2014101000011";
            userContracts.Add(new UserContract { CreatedBy = currentUserName, Id = Guid.NewGuid(), UserId = dalu, UserName = "大陆之星股份有限公司", Key = keyY, CustomerType = CustomerType.Company, Statu = UserContractStatus.Sign });
            contract.UserContract = userContracts;
            if (ContractGroupRepository.Add(new List<ContractGroup> { contract }) > 0)
            {
                //大陆之星自动签章（关键字签章）
                #region 盖章参数
                var userid = contract.UserId;//TenantName
                var BlobId = contract.BlobId;
                var blob = BlobUtil.GetBlobById(Tenant, userid, BlobId, false);
                if (!(blob != null && blob.Data != null))
                {
                    return new ServiceResult<bool>(ServiceResultType.Error, "未找到文件");
                }
                var isKey = keyY;
                var r = ElectronicSignApiService.AddSignatureToPdf(blob.Data, blob.FileName, "120", "18", isKey, 4, "", "0", "0", dalu);
                if (r.Result != null)
                {
                    if (r.Result.Item1 == 100)
                    {
                        var encodeFileName = Base64Provider.EncodeString(r.Result.Item2);
                        var blobInfo = new BlobInfo(blobId, FileType.PDF.ToString(), DocFormat.Pdf.ToString(), encodeFileName, "pdf", r.Result.Item3);
                        BlobUtil.SaveBlob(Tenant, userid, blobInfo);
                        var result0 = ReApplyPlatFormConfirmCurrencySign(currentUserId, currentUserName, _mapper.Map<ContractGroupDTO>(contract));
                        if (result0 > 0)
                        {
                            return new ServiceResult<bool>(ServiceResultType.Success, "生成合同成功");
                        }
                        else
                        {
                            return new ServiceResult<bool>(ServiceResultType.Error, "平台生成合同失败");
                        }
                    }
                }
                #endregion
            }
            return new ServiceResult<bool>(ServiceResultType.Error, "生成合同失败");
        }


        public bool SaveContractTemplate(int id, string content)
        {
            var result = ContractTemplateRepository.GetById(id);
            if (result != null)
            {
                result.Content = content;
            }
            return ContractTemplateRepository.Modify(result, new[] { "Content" });
        }

        public bool SoftRemoveContractTemplate(int id)
        {
            return ContractTemplateRepository.SoftRemoveById(id);
        }
        #endregion

    }

    public class ReturnParam
    {
        public string blobId { get; set; }
        //public string userAccount { get; set; }
        public string stampState { get; set; }
        public string remark { get; set; }
        public string keyword { get; set; }
    }
}
