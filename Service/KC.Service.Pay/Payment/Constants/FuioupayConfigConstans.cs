using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Constants
{
    public class FuioupayConfigConstans
    {
        public const string PaymentMethod = "PaymentMethod";

        public const string PostUrl = "PostUrl";

        /// <summary>
        /// 商户代码,15,富友分配给各合作商户的唯一识别码,必填	
        /// </summary>
        public const string mchnt_cd = "mchnt_cd";

        /// <summary>
        /// 32位商户密钥
        /// </summary>
        public const string mchnt_key = "mchnt_key";

        /// <summary>
        /// 商户订单号,MAX(30)	客户支付后商户网站产生的一个唯一的定单号，该订单号应该在相当长的时间内不重复。富友通过订单号来唯一确认一笔订单的重复性。	必填
        /// </summary>
        public const string order_id = "order_id";

        /// <summary>
        /// 交易金额,MAX(12),客户支付订单的总金额，一笔订单一个，以分为单位。不可以为零，必需符合金额标准。	必填
        /// </summary>
        public const string order_amt = "order_amt";

        /// <summary>
        /// 支付类型		3	‘B2C’ – B2C支付 ‘B2B’ – B2B支付  ‘FYCD’ – 预付卡 ‘SXF’ – 随心富	必填
        /// </summary>
        public const string order_pay_type = "order_pay_type";

        /// <summary>
        /// 页面跳转URL		MAX(200)	商户接收支付结果通知地址	必填
        /// </summary>
        public const string page_notify_url = "page_notify_url";

        /// <summary>
        /// 后台通知URL		MAX(200)	商户接收支付结果后台通知地址	必填
        /// </summary>
        public const string back_notify_url = "back_notify_url";

        /// <summary>
        /// 超时时间		2	1m-15天，m：分钟、h：小时、d天、1c当天有效，	非必填
        /// </summary>
        public const string order_valid_time = "order_valid_time";

        /// <summary>
        /// 银行代码		10	必填	0803010000
        /// </summary>

        public const string iss_ins_cd = "iss_ins_cd";

        /// <summary>
        /// 商品名称		MAX(60)		非必填
        /// </summary>
        public const string goods_name = "goods_name";

        /// <summary>
        /// 商品展示网址	MAX(200)	商品展示地址	必填
        /// </summary>
        public const string goods_display_url = "goods_display_url";

        /// <summary>
        /// 备注		MAX(60)		非必填
        /// </summary>
        public const string rem = "rem";

        /// <summary>
        /// 版本号	MAX(10)	目前填1.0.1	必填
        /// </summary>
        public const string ver = "ver";

        /// <summary>
        /// MD5摘要数据	mchnt_cd+"|" +order_id+"|"+order_amt+"|"+order_pay_type+"|"+page_notify_url+"|"+back_notify_url+"|"+order_valid_time+"|"+iss_ins_cd+"|"+goods_name+"|"+"+goods_display_url+"|"+rem+"|"+ver+"|"+mchnt_key 做md5摘要 其中mchnt_key 为32位的商户密钥，系统分配	必填
        /// </summary>
        public const string md5 = "md5";
    }
}
