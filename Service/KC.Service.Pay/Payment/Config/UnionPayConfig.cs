using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Config
{
    public class UnionPayConfig
    { /// <summary>
        /// 有卡交易地址
        /// </summary>
        public string CardRequestUrl { get; set; }

        /// <summary>
        /// app交易地址 手机控件支付使用该地址
        /// </summary>
        public string AppRequestUrl { get; set; }

        /// <summary>
        /// 获取验签目录
        /// </summary>
        public string FrontTransUrl { get; set; }

        /// <summary>
        /// 加密公钥证书路径
        /// </summary>
        public string EncryptCert { get; set; }

        /// <summary>
        /// 后台交易地址
        /// </summary>
        public string BackTransUrl { get; set; }

        /// <summary>
        /// 交易状态查询地址
        /// </summary>
        public string SingleQueryUrl { get; set; }

        /// <summary>
        /// 文件传输类交易地址
        /// </summary>
        public string FileTransUrl { get; set; }

        /// <summary>
        /// 加密证书路径
        /// </summary>
        public string SignCertPath { get; set; }

        public string SignCertName { get; set; }
        /// <summary>
        /// 签名证书密码
        /// </summary>
        public string SignCertPwd { get; set; }

        /// <summary>
        /// 获取后台交易地址
        /// </summary>
        public string ValidateCertDir { get; set; }

        /// <summary>
        /// 批量交易地址
        /// </summary>
        public string BatTransUrl { get; set; }
    }
}

