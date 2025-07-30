using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Constants
{
    public class AllinpayConfigConstans
    {
        public const string PaymentMethod = "PaymentMethod";
        //字符集
        public const string InputCharset = "inputCharset";
        //跳转地址
        public const string PickupUrl = "pickupUrl";
        //后台通知地址
        public const string ReceiveUrl = "receiveUrl";
        //版本号
        public const string Version = "version";
        //语言种类
        public const string Language = "language";
        //签名类型
        public const string SignType = "signType";
        //收款商户号
        public const string MerchantId = "merchantId";
        //币种类型
        public const string OrderCurrency = "orderCurrency";
        //支付方式
        public const string PayType = "payType";
        //MD5 KEY
        public const string Key = "key";
        //提交地址
        public const string PostUrl = "PostUrl";
        //查询版本号
        public const string QueryVersion = "queryVersion";
        //付款商户号
        public const string PayMerchantId = "PayMerchantId";
        //用户名
        public const string UserName = "UserName";
        //密码
        public const string Password = "Password";
        //交易代码
        public const string TrxCode = "TrxCode";
        //业务代码
        public const string BusinessCode = "BusinessCode";
        //处理级别
        public const string Level = "Level";
        //私钥证书
        public const string SigneCert = "SigneCert";
        //私钥证书密码
        public const string SigneCertPwd = "SigneCertPwd";
        //公钥证书
        public const string VerifyCert = "VerifyCert";
        //ProcessServlet
        public const string ProcessServlet = "ProcessServlet";

        public const string PayAmount = "payAmount";
        /// <summary>
        /// 订单号
        /// </summary>
        public const string OrderNo = "orderNo";

        /// <summary>
        /// 订单金额
        /// </summary>
        public const string OrderAmount = "orderAmount";


        /// <summary>
        /// 订单时间
        /// </summary>
        public const string OrderDatetime = "orderDatetime";

        /// <summary>
        /// MD5 签名
        /// </summary>
        public const string SignMsg = "signMsg";



        public const string MD5Key = "key";

        public const string PaymentOrderId = "paymentOrderId";

        public const string PayDatetime = "payDatetime";

        public const string PayResult = "payResult";

        public const string ErrorCode = "errorCode";

        public const string ReturnDatetime = "returnDatetime";


    }
}
