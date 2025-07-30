using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Constants
{
    /// <summary>
    /// 中金支付配置常量
    /// </summary>
    public class CPCNConfigConstants
    {
        /// <summary>
        /// 合作方编号
        /// </summary>
        public const string ptncode = "ptncode";
        /// <summary>
        /// 资金托管方编号
        /// </summary>
        public const string bkcode = "bkcode";

        /// <summary>
        /// 合作方的签名证书(含私钥)
        /// </summary>
        public const string SingCertName = "SingCertName";

        /// <summary>
        /// 合作方的私钥口令
        /// </summary>
        public const string password = "password";

        /// <summary>
        /// 融资平台的公钥证书
        /// </summary>
        public const string EncryptCertName = "EncryptCertName";

        /// <summary>
        /// 融资平台融资接口地址
        /// </summary>
        public const string FinancingPostUrl = "FinancingPostUrl";

        /// <summary>
        /// 融资平台交易接口地址
        /// </summary>
        public const string TransactionPostUrl = "TransactionPostUrl";

        /// <summary>
        /// 版本
        /// </summary>
        public const string Version = "Version";

    }
}
