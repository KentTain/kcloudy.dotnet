using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Contract;
using KC.Framework.Base;
using KC.Model.Contract.Constants;

namespace KC.Model.Contract
{
    [Serializable]
    [DataContract(IsReference = true)]
    [Table(Tables.ContractGroup)]
     public  class  ContractGroup:Entity
     {
        [DataMember]
        [Key]
        public Guid Id { get; set; }

        [DataMember]
        public string BlobId { get; set; }

        /// <summary>
        /// 上传用户
        /// </summary>
        [DataMember]
        public string UserId { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        [DataMember]
        public string ContractNo { get; set; }
        /// <summary>
        /// 合同标题
        /// </summary>
        [DataMember]
        public string ContractTitle { get; set; }
            [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
           [DataMember]
        public string ContractContent { get; set; }
        /// <summary>
        /// 盖章处
        /// </summary>
            [DataMember]
        public string ContractFootnote { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
           [DataMember]
        public ContractStatus Statu { get; set; }

        /// <summary>
        /// 分组Id
        /// </summary>
            [DataMember]
        public string GroupId { get; set; }

        /// <summary>
        /// 合同类型
        /// </summary>
        [DataMember]
        public ContractType Type { get; set; }
        [DataMember]
        public Guid? PId { get; set; }

        [DataMember]
        [ForeignKey("PId")]
        public virtual ContractGroup ContractEnclosure { get; set; }

        [DataMember]
        public virtual ICollection<UserContract> UserContract { get; set; }

            [DataMember]
        public virtual ICollection<ContractGroup> ContractEnclosureList { get; set; }

        /// <summary>
        /// 合同操作日志
        /// </summary>
         [DataMember]
        public virtual ICollection<ContractGroupOperationLog> ContractGroupOperationLog { get; set; }

        /// <summary>
         ///  退回和作废回调Url
        /// </summary>
            [DataMember]
        public string Break { get; set; }
        /// <summary>
        /// 退回和作废回调状态
        /// </summary>
        [DataMember]
        public bool BreakStart { get; set; }
        /// <summary>
        /// 盖完回调Url
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
       /// 关联字段（采购单号）
       /// </summary>
       [DataMember]
        public string RelationData { get; set; }
     }
}
