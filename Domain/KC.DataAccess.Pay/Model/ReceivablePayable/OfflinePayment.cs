using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Model.Pay.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Pay
{
    [Table(Tables.OfflinePayment)]
    public class OfflinePayment : Entity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        public string OrderId { get; set; }

        [MaxLength(128)]
        public string PayableNumber { get; set; }

        public DateTime PayDateTime { get; set; }

        /// <summary>
        /// +收入，-支出
        /// </summary>
        public decimal AmountOfMoney { get; set; }


        public OfflinePaymentStatus Status { get; set; }

        public string Remark { get; set; }

        public string SupplementRemark { get; set; }

        [MaxLength(128)]
        public string BusinessNumber { get; set; }

        public ReceivableSource? ReceivableSource { get; set; }

        public PayableSource? PayableSource { get; set; }

        /// <summary>
        /// 付款方/收款方
        /// </summary>
        [MaxLength(128)]
        public string Customer { get; set; }
    }
}
