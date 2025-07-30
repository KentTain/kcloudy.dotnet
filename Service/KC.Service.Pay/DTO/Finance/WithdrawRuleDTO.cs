using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Pay;

namespace KC.Service.DTO.Pay
{
    public class WithdrawRuleDTO
    {
        public int Id { get; set; }

        /// <summary>
        /// 提现费率类型
        /// </summary>
        public WithdrawType WithdrawType { get; set; }

        /// <summary>
        /// 最低提现金额
        /// </summary>
        public decimal MinMoney { get; set; }

        /// <summary>
        /// 最高提现金额
        /// </summary>
        public decimal MaxMoney { get; set; }

        /// <summary>
        /// 单日累计最高提现金额
        /// </summary>
        public decimal DayMaxMoney { get; set; }
        /// <summary>
        /// 提现费率
        /// </summary>
        public double Withdrawlv { get; set; }

        /// <summary>
        /// 到账时间(天数)
        /// </summary>
        public int Paymentdate { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsStart { get; set; }


        /// <summary>
        /// 价格区间最小值（不包含）
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 价格区间最大值（包含）
        /// </summary>
        public decimal Value1 { get; set; }

        /// <summary>
        /// 阶梯收费类型0：按费率收费，1固定金额收费
        /// </summary>
        public int LadderType { get; set; }

        /// <summary>
        /// 阶梯收费类型1固定金额收费的金额
        /// </summary>
        public decimal Value2 { get; set; }
    }
}
