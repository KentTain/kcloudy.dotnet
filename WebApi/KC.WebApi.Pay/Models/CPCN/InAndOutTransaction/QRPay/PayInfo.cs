using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.QRPay
{
    public class PayInfo
    {
        /// <summary>
        /// 支付方式：6：正扫支付 8：公众号支付
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 支付方式二级分类3：支付宝 PayType = 6 / 8 必输 4：微信 PayType = 6 / 8 必输
        /// </summary>
        public int SecPayType { get; set; }

        /// <summary>
        /// 订单标题 PayType=6/8 时必输
        /// 64 
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 商品描述（微信平台配置的 商品标记，用于满减和优惠券）PayType=6/8 时必输 
        /// 32
        /// </summary>
        public string GoodsDesc { get; set; }

        /// <summary>
        /// 用户 ID PayType=8 时必输 微信：openid  支付宝：buyer_user_id
        /// </summary>
        public string UserID { get; set; }
    }
}