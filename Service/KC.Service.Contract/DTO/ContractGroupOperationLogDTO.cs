using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Contract;
using KC.Service.DTO;
using KC.Framework.Base;

namespace KC.Service.DTO.Contract
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ContractGroupOperationLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public Guid ContractGroupId { get; set; }

        [DataMember]
        public ContractGroupProgress ContractGroupProgress { get; set; }

        //[DataMember]
        //public ContractGroupDTO ContractGroup { get; set; }

        /// <summary>
        /// 合同未同步到对方数据的所有者
        /// </summary>
        [DataMember]
        public string NotContractGroupUsers { get; set; }

        /// <summary>
        /// 合同未同步到平台
        /// </summary>
        [DataMember]
        public SyncStatus ToPlatFormContractGroup { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [DataMember]
        public string TypeStr { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [DataMember]
        public string OperateDateStr { get { return OperateDate.ToString("yyyy年MM月dd日 HH:mm:ss"); } }
    }
}
