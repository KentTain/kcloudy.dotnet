using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.SDK
{
    public class SDKConfig
    {
        /// <summary>
        /// 有卡交易地址
        /// </summary>
        public static string CardRequestUrl { get; set; }

        /// <summary>
        /// app交易地址 手机控件支付使用该地址
        /// </summary>
        public static string AppRequestUrl { get; set; }

        /// <summary>
        /// 获取验签目录
        /// </summary>
        public static string FrontTransUrl { get; set; }

        /// <summary>
        /// 加密公钥证书路径
        /// </summary>
        public static string EncryptCert { get; set; }

        /// <summary>
        /// 后台交易地址
        /// </summary>
        public static string BackTransUrl { get; set; }

        /// <summary>
        /// 交易状态查询地址
        /// </summary>
        public static string SingleQueryUrl { get; set; }

        /// <summary>
        /// 文件传输类交易地址
        /// </summary>
        public static string FileTransUrl { get; set; }

        /// <summary>
        /// 加密证书路径
        /// </summary>
        public static string SignCertPath { get; set; }
        /// <summary>
        /// 加密证书名字
        /// </summary>
        public static string SignCertName { get; set; }
        /// <summary>
        /// 签名证书密码
        /// </summary>
        public static string SignCertPwd { get; set; }

        /// <summary>
        /// 获取后台交易地址
        /// </summary>
        public static string ValidateCertDir { get; set; }

        /// <summary>
        /// 批量交易地址
        /// </summary>
        public static string BatTransUrl { get; set; }
    }
}
