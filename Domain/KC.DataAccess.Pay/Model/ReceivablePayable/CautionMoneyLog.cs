using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Model.Pay.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    [Table(Tables.CautionMoneyLog)]
    public class CautionMoneyLog : Entity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string OrderNum { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string OtherParty { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string Operator { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string ActionName { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public CautionMoneyPayStatus Status { get; set; }

        [DataMember]
        public string Remark { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string PaymentNumber { get; set; }

        /// <summary>
        /// 应收/应付
        /// </summary>
        [DataMember]
        public bool IsReceivable { get; set; }
    }
}
