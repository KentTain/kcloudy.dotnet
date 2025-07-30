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
    [Table(Tables.OfflineUsageBill)]
    public class OfflineUsageBill : Entity
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

        /// <summary>
        /// 票据号
        /// </summary>
        [MaxLength(128)]
        public string BillNumber { get; set; }

        /// <summary>
        /// 票据类型，true:银票，false:商票
        /// </summary>
        public bool BankBill { get; set; }

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

        /// <summary>
        /// 是否兑付 ,null:等待买方确认付款，false:等待卖方确认收款，true:买方已确认付款或卖方已确认收款
        /// </summary>
        public bool? CashPayment { get; set; }

        /// <summary>
        /// 融资系统领用额度单号，在释放额度时使用
        /// </summary>
        [MaxLength(128)]
        public string CreditUsageDetailId { get; set; }
    }
}
