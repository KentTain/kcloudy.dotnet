using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class BoHaiReturnModel
    {
        /// <summary>
        /// 交易请求流水号
        /// </summary>
        public string ReqJnlNo { get; set; }

        /// <summary>
        /// 交易响应流水号
        /// </summary>
        public string ResJnlNo { get; set; }

        /// <summary>
        /// 交易日期 yyyyMMdd
        /// </summary>
        public string ResDate { get; set; }

        /// <summary>
        /// 交易响应时间 yyyyMMddHHmmss
        /// </summary>
        public string ResTime { get; set; }

        /// <summary>
        /// 交易响应码 成功：000000
        /// </summary>
        public string ResCode { get; set; }

        /// <summary>
        /// 交易响应信息
        /// </summary>
        public string ResMsg { get; set; }
    }
}