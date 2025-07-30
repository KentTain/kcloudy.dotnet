using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 交易信息查询
    /// 商户上送系统返回交易流水，通过交易信息查询，可以查询用户在资金监管平台的所有交易。
    /// </summary>
    public class VisTransferQryModel : BoHaiBaseModel
    {
        /// <summary>
        /// 商户流水号
        /// </summary>
        public string OrgChannelSeqNo { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 交易类型 01:充值 02:提现 03:转账 04:冻结解冻 05:虚拟消费 06：消费贷款 7：解冻虚拟转账（批量） 08：解冻转账（批量） 09：虚拟动账（商户结算使用）
        /// 10：合并消费 11：红包账户充值 12：平台向普通客户转账 13：消费账户提现
        /// </summary>
        public string TransID { get; set; }

        /// <summary>
        /// 账号  12位账号
        /// </summary>
        public string AcctNo { get; set; }

        /// <summary>
        /// 交易状态  00-交易成功01-交易失败 99-交易超时
        /// </summary>
        public string OrgTransStauts { get; set; }
    }
}