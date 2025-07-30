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
    [Table(Tables.UserContract)]
    public class UserContract : Entity
    {
        [Key]
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// 签署人
        /// </summary>
        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string StaffId { get; set; }

        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 盖章关键字
        /// </summary>
        [DataMember]
        public string Key { get; set; }
        /// <summary>
        /// 签署状态
        /// </summary>
        [DataMember]
        public UserContractStatus Statu { get; set; }
        /// <summary>
        /// 退回说明
        /// </summary>

        [DataMember]
        public string BreakRemark { get; set; }
        [DataMember]
        public CustomerType CustomerType { get; set; }

        /// <summary>
        /// 合同Id
        /// </summary>
        [DataMember]
        public Guid BlobId { get; set; }
        [ForeignKey("BlobId")]
        [DataMember]
        public virtual ContractGroup ContractGroup { get; set; }

    }
}
