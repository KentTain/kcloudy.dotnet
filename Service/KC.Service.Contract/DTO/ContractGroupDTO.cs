using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Contract;
using KC.Framework.Base;
using KC.Service.DTO;

namespace KC.Service.DTO.Contract
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ContractGroupDTO : EntityDTO
    {
        public ContractGroupDTO()
        {
            UserContract = new List<UserContractDTO>();
            ContractGroupOperationLog = new List<ContractGroupOperationLogDTO>();
            //ContractEnclosureList = new List<ContractGroupDTO>();
        }
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string BlobId { get; set; }
        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string ContractNo { get; set; }
        [DataMember]
        public string ContractTitle { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string ContractContent { get; set; }
        [DataMember]
        public string ContractFootnote { get; set; }

        [DataMember]
        public ContractStatus Statu { get; set; }
        [DataMember]
        public string StatuStr { get; set; }
        [DataMember]
        public string GroupId { get; set; }
        [DataMember]
        public ContractType Type { get; set; }
        [DataMember]
        public string TypeStr { get; set; }
        [DataMember]
        public Guid? PId { get; set; }

        //[DataMember]
        //public ContractGroupDTO ContractEnclosure { get; set; }

        [DataMember]
        public virtual ICollection<UserContractDTO> UserContract { get; set; }
        /// <summary>
        /// 合同操作日志
        /// </summary>
        [DataMember]
        public virtual ICollection<ContractGroupOperationLogDTO> ContractGroupOperationLog { get; set; }
        /// <summary>
        /// 是否有日志
        /// </summary>
        [DataMember]
        public bool HasLogs { get; set; }

        //[DataMember]
        //public virtual ICollection<ContractGroupDTO> ContractEnclosureList { get; set; }



        [DataMember]
        public bool IsEdit { get; set; }
        [DataMember]
        public bool IsLast { get; set; }
        [DataMember]
        public bool IsRelieveAll { get; set; }
        [DataMember]
        public bool IsComfirmFrist { get; set; }
        [DataMember]
        public bool IsRelieve { get; set; }
        [DataMember]
        public bool IsComfirm { get; set; }
        [DataMember]
        public bool IsReturn { get; set; }
        [DataMember]
        public bool IsSign { get; set; }
        [DataMember]
        public bool IsPersonal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public ContractOpt? Opt { get; set; }

        [DataMember]
        public string CurrentUserId { get; set; }

        public string CurrentTenantName { get; set; }

        public string CurrentUserDisplayName { get; set; }
        public string CurrentUserPhone { get; set; }

        /// <summary>
        /// 回调Url
        /// </summary>
        [DataMember]
        public string Break { get; set; }
        /// <summary>
        /// 单个回调状态
        /// </summary>
        [DataMember]
        public bool BreakStart { get; set; }
        /// <summary>
        /// 回调Url
        /// </summary>
        [DataMember]
        public string AllBreak { get; set; }
        /// <summary>
        /// 合同组回调状态
        /// </summary>
        [DataMember]
        public bool AllBreakStart { get; set; }

        /// <summary>
        /// 合同数据同步状态
        /// </summary>
        [DataMember]
        public SyncStatus SyncStatus { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        [DataMember]
        public WorkflowBusStatus WorkFlowStatus { get; set; }

        /// <summary>
        /// 对应平台合同数据的id
        /// </summary>
        [DataMember]
        public string ReferenceId { get; set; }
        /// <summary>
        /// api下载地址
        /// </summary>
        [DataMember]
        public string DownFileUrl { get; set; }
        //时间字符串格式
        [DataMember]
        public string DateTimeStr { get; set; }

        /// <summary>
        /// 关联字段（采购单号）
        /// </summary>
        [DataMember]
        public string RelationData { get; set; }
        [DataMember]
        public string CreatedDateString { get; set; }
    }

    
}
