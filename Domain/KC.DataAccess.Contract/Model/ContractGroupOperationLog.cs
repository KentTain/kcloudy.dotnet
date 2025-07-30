using System;
using System.Collections.Generic;
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
    [Table(Tables.ContractGroupOperationLog)]
    public class ContractGroupOperationLog  : ProcessLogBase
    {
        /// <summary>
        /// 主表合同的id
        /// </summary>
        public Guid ContractGroupId { get; set; }

        public ContractGroupProgress ContractGroupProgress { get; set; }

        [ForeignKey("ContractGroupId")]
        public virtual ContractGroup ContractGroup { get; set; }

        /// <summary>
        /// 合同未同步到对方数据的所有者
        /// </summary>
        public string NotContractGroupUsers { get; set; }

        /// <summary>
        /// 合同未同步到平台
        /// </summary>
        public SyncStatus ToPlatFormContractGroup { get; set; }
    }
}
