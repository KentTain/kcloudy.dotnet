using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class VisTransInfoModel
    {
        /// <summary>
        /// 平台流水号
        /// </summary>
        public string OrgTransSeqNo { get; set; }

        /// <summary>
        /// 平台清算日期
        /// </summary>
        public string OrgClearDate { get; set; }

        /// <summary>
        /// 流水金额
        /// </summary>
        public string OrgTransAmount { get; set; }

        /// <summary>
        /// 流水响应码
        /// </summary>
        public string OrgRspCode { get; set; }

        /// <summary>
        /// 流水信息
        /// </summary>
        public string OrgRespMsg { get; set; }

        /// <summary>
        /// 交易状态 00-交易成功 01-交易失败 99-交易超时
        /// </summary>
        public string OrgTransStauts { get; set; }

        /// <summary>
        /// 平台号
        /// </summary>
        public string MerInstId { get; set; }

        /// <summary>
        /// 付款虚拟客户号
        /// </summary>
        public string TransPayEleCifNo { get; set; }

        /// <summary>
        /// 付款虚拟账号或付款账号
        /// </summary>
        public string TransPayAcctNo { get; set; }

        /// <summary>
        /// 付款账户类型
        /// </summary>
        public string TransPayAcctType { get; set; }

        /// <summary>
        /// 付款账户名称
        /// </summary>
        public string TransPayAcctName { get; set; }

        /// <summary>
        /// 收款虚拟客户号
        /// </summary>
        public string TransRcvEleCifNo { get; set; }

        /// <summary>
        /// 收款虚拟账号或收款账号
        /// </summary>
        public string TransRcvAcctNo { get; set; }

        /// <summary>
        /// 收款账户类型
        /// </summary>
        public string TransRcvAcctType { get; set; }

        /// <summary>
        /// 收款账户名称
        /// </summary>
        public string TransRcvAcctName { get; set; }

        /// <summary>
        /// 付款方余额
        /// </summary>
        public string BalanceAmt { get; set; }

        /// <summary>
        /// 收款方余额
        /// </summary>
        public string BalanceRcvAmt { get; set; }

        /// <summary>
        /// 消费类型
        /// </summary>
        public string ConsumeType { get; set; }

        /// <summary>
        /// 交易名称（中文）
        /// </summary>
        public string TransIdAlias { get; set; }

        /// <summary>
        /// 交易码
        /// </summary>
        public string TransId { get; set; }

        /// <summary>
        /// 交易备注
        /// </summary>
        public string TransRemark { get; set; }

        /// <summary>
        /// 商户流水号
        /// </summary>
        public string TransMerSeqNo { get; set; }

        /// <summary>
        /// 商户订单日期
        /// </summary>
        public string TransMerDate { get; set; }

        /// <summary>
        /// 商户订单时间
        /// </summary>
        public string TransMerDateTime { get; set; }
    }
}