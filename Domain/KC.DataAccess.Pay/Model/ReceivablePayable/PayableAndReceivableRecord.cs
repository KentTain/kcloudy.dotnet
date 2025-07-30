using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Model.Pay.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KC.Model.Pay
{
    [Table(Tables.PayableAndReceivableRecord)]
    public class PayableAndReceivableRecord : Entity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        public string PayableNumber { get; set; }

        /// <summary>
        /// 付款收款时间
        /// </summary>
        public DateTime PayDateTime { get; set; }

        /// <summary>
        /// 付款收款金额,+应收，-应付
        /// </summary>
        public decimal AmountOfMoney { get; set; }

        public PaymentType PaymentType { get; set; }

        public string Remark { get; set; }

         [MaxLength(128)]
        public string Operator{ get; set; }
    }
}
