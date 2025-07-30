using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class BoHaiBaseModel
    {
        /// <summary>
        /// 版本号 目前版本号：1.0.0
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 交易请求流水号 char(20)
        /// </summary> 
        public string ReqJnlNo { get; set; }

        /// <summary>
        /// 交易请求日期 yyyyMMdd
        /// </summary>
        public string ReqDate { get; set; }

        /// <summary>
        /// 交易请求时间 yyyyMMddHHmmss
        /// </summary>
        public string ReqTime { get; set; }

        /// <summary>
        /// 商户号 虚账户商户号
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// 渠道号
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// 交易请求服务编号
        /// </summary>
        public string ReqId { get; set; }

    }
}