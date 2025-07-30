using KC.WebApi.Pay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.InNotice
{
    public class MSG
    {
        public MSGHD MSGHD { get; set; }

        public CltAcc CltAcc { get; set; }

        public BkAcc BkAcc { get; set; }

        public Amt Amt { get; set; }

        /// <summary>
        /// 资金用途(附言)
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// 流水
        /// </summary>
        public Srl Srl { get; set; }

        /// <summary>
        /// 业务标示 0:银行发起 1:渠道入金异步通知
        /// </summary>
        public int TrsFlag { get; set; }

        /// <summary>
        /// 1 成功 2 失败
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 交易成功/失败时间(渠道通知时间)格式:YYYYMMDDHH24MISS
        /// </summary>
        public string RestTime { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        public string Opion { get; set; }

        /// <summary>
        /// 原交易日期
        /// </summary>
        public string FDate { get; set; }

        /// <summary>
        /// 原交易时间
        /// </summary>
        public string FTime { get; set; }

        /// <summary>
        /// 备用 1
        /// </summary>
        public string Spec1 { get; set; }

        /// <summary>
        /// 备用 2
        /// </summary>
        public string Spec2 { get; set; }

    }
}