using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class BoHaiConfigDTO
    {
        public int ConfigId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// 渠道号
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// 合作方的签名证书(含私钥)
        /// </summary>

        public string SignCertName { get; set; }

        /// <summary>
        /// 合作方的私钥口令
        /// </summary>

        public string Password { get; set; }

        /// <summary>
        /// 接口地址
        /// </summary>

        public string PostUrl { get; set; }

        /// <summary>
        /// 版本
        /// </summary>

        public string Version { get; set; }

        /// <summary>
        /// 公钥
        /// </summary>
        public string PubKey { get; set; }

        /// <summary>
        /// 短信地址
        /// </summary>
        public string MessageUrl { get; set; }
    }
}