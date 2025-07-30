using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Service.DTO;
using KC.Enums.Contract;

namespace KC.Service.DTO.Contract
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ContractGroupAPIModel : EntityDTO
    {
        public ContractGroupAPIModel()
        {
            UserContract = new List<UserContractAPIModel>();
            ContractGroupOperationLog = new List<ContractGroupOperationLogDTO>();
            //ContractEnclosureList = new List<ContractGroupAPIModel>();
        }
        /// <summary>
        /// 合同主键Id
        /// </summary>
         [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// 合同文件Id
        /// </summary>
         [DataMember]
        public string BlobId { get; set; }
        /// <summary>
        /// 上传人租户代码
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
        /// <summary>
        /// 上传人企业名称
        /// </summary>
        [DataMember]
         public string UserName { get; set; }
        /// <summary>
        /// 合同主体（暂不使用）
        /// </summary>
        [DataMember]
         public string ContractContent { get; set; }
        /// <summary>
        /// 合同底部签署信息（暂不使用）
        /// </summary>
        [DataMember]
         public string ContractFootnote { get; set; }
        /// <summary>
        /// 合同状态
        /// </summary>
        [DataMember]
         public ContractStatus Statu { get; set; }
        /// <summary>
        /// 合同状态名称
        /// </summary>
        [DataMember]
         public string StatuStr { get; set; }
        /// <summary>
        /// 分组id（暂未使用）
        /// </summary>
        [DataMember]
         public string GroupId { get; set; }
        /// <summary>
        /// 合同类型
        /// </summary>
        [DataMember]
         public ContractType Type { get; set; }
        /// <summary>
        /// 合同类型字符串格式
        /// </summary>
        [DataMember]
         public string TypeStr { get; set; }
        /// <summary>
        /// 合同父级id（关联合同时使用，现在已取消关联合同业务）
        /// </summary>
        [DataMember]
         public Guid? PId { get; set; }

         //[DataMember]
         //public ContractGroupAPIModel ContractEnclosure { get; set; }
        /// <summary>
        /// 合同签署人表
        /// </summary>
        [DataMember]
         public virtual ICollection<UserContractAPIModel> UserContract { get; set; }
        /// <summary>
        /// 合同操作日志
        /// </summary>
         [DataMember]
         public virtual ICollection<ContractGroupOperationLogDTO> ContractGroupOperationLog { get; set; }

         //[DataMember]
         //public virtual ICollection<ContractGroupAPIModel> ContractEnclosureList { get; set; }


        /// <summary>
        /// 是否可编辑（控制前台权限）
        /// </summary>
        [DataMember]
         public bool IsEdit { get; set; }
        /// <summary>
        /// 是否最后一个签署（控制前台权限）
        /// </summary>
        [DataMember]
         public bool IsLast { get; set; }
        /// <summary>
        /// 是否可以作废（控制前台权限）
        /// </summary>
        [DataMember]
         public bool IsRelieveAll { get; set; }
        /// <summary>
        /// 是否第一个审核合同（控制前台权限）
        /// </summary>
        [DataMember]
         public bool IsComfirmFrist { get; set; }
        /// <summary>
        /// 是否可以删除（控制前台权限）
        /// </summary>
        [DataMember]
         public bool IsRelieve { get; set; }
        /// <summary>
        /// 是否可以确认合同（控制前台权限）
        /// </summary>
        [DataMember]
         public bool IsComfirm { get; set; }
        /// <summary>
        /// 是否可以退回合同（控制前台权限）
        /// </summary>
        [DataMember]
         public bool IsReturn { get; set; }
        /// <summary>
        /// 是否可以签署合同（控制前台权限）
        /// </summary>
        [DataMember]
         public bool IsSign { get; set; }


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
        /// <summary>
        /// 时间字符串格式
        /// </summary>
        [DataMember]
        public string DateTimeStr { get; set; }

        /// <summary>
        /// 关联字段（采购单号）
        /// </summary>
           [DataMember]
        public string RelationData { get; set; }
        /// <summary>
        /// 上传时间字符串格式
        /// </summary>
        [DataMember]
        public string CreatedDateString { get; set; }
    }
}
