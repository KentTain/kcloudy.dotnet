using System.Runtime.Serialization;

namespace KC.Service.DTO
{
    public class ReturnMessageDTO : EntityBaseDTO
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
    }

    public class CreditUsageTotalMode
    {
        /// <summary>
        /// 消费总额
        /// </summary>
        public decimal TotalConsumptionMoney { get; set; }
        /// <summary>
        /// 首付款总额
        /// </summary>
        public double TotalDownPayment { get; set; }
        /// <summary>
        /// 保证金总额
        /// </summary>
        public double TotalCashDeposit { get; set; }
        /// <summary>
        /// 已还款总额
        /// </summary>
        public decimal TotalReceivedRepayment { get; set; }
        /// <summary>
        /// 需还款总额
        /// </summary>
        public decimal TotalArrearsAmount { get; set; }
        /// <summary>
        /// 未还款总额
        /// </summary>
        public decimal TotalWaitRepayment { get; set; }
        /// <summary>
        /// 总罚金
        /// </summary>
        public decimal TotalPenaltyAmount { get; set; }
        /// <summary>
        /// 总本金
        /// </summary>
        public double TotalPrincipal { get; set; }
        /// <summary>
        /// 总利息
        /// </summary>
        public double TotalInterest { get; set; }
        /// <summary>
        /// 计息总额度
        /// </summary>
        public decimal TotalDrawAmount { get; set; }


        //var totalDownPayment = result.Sum(m => m.DownPayment);//首付款总额
        //var totalCashDeposit = result.Sum(m => m.CashDeposit);//保证金总额
        //var totalReceivedRepayment = result.Sum(m => m.ReceivedRepayment);//已还总额
        //var totalArrearsAmount = result.Sum(m => m.ArrearsAmount);//需还总本息
        //var totalWaitRepayment = totalArrearsAmount - totalReceivedRepayment;//未还总额
        //var totalConsumptionMoney = result.Sum(m => m.ConsumptionMoney);//消费总额
        //var totalPenaltyAmount = reminds.Sum(m => m.PenaltyAmount);//总罚金测
        //var totalPrincipal = reminds.Sum(m => m.RepaymentInterestDetail.Principal);//总本金
        //var totalInterest = reminds.Sum(m => m.RepaymentInterestDetail.Interest);//总利息
    }
}
