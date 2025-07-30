using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models
{
    public class CPCNConfigDTO
    {

        public int ConfigId { get; set; }

        /// <summary>
        /// 合作方编号
        /// </summary>

        public string PtnCode { get; set; }
        /// <summary>
        /// 资金托管方编号
        /// </summary>

        public string BkCode { get; set; }

        /// <summary>
        /// 合作方的签名证书(含私钥)
        /// </summary>

        public string SingCertName { get; set; }

        /// <summary>
        /// 合作方的私钥口令
        /// </summary>

        public string Password { get; set; }

        /// <summary>
        /// 融资平台的公钥证书
        /// </summary>

        public string EncryptCertName { get; set; }

        /// <summary>
        /// 融资平台融资接口地址
        /// </summary>

        public string FinancingPostUrl { get; set; }

        /// <summary>
        /// 融资平台交易接口地址
        /// </summary>

        public string TransactionPostUrl { get; set; }

        /// <summary>
        /// 版本
        /// </summary>

        public string Version { get; set; }
    }
}
