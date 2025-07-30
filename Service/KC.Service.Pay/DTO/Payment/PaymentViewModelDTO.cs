using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class PaymentViewModelDTO : PayBaseParamDTO
    {
        /// <summary>
        /// 支付订单号
        /// </summary>
        [DataMember]
        [Required]
        public string PaymentOrderId { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        [Required]
        public decimal Amount { get; set; }

        /// <summary>
        /// 业务订单号
        /// </summary>
        [DataMember]
        [Required]
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        [DataMember]
        [Required]
        public string OrderType { get; set; }

        /// <summary>
        /// 收款方
        /// </summary>
        [DataMember]
        [Required]
        public string Payee { get; set; }

        /// <summary>
        /// 收付款TenantName
        /// </summary>
        [DataMember]
        [Required]
        public string PayeeTenant { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        [Required]
        public string GoodsName { get; set; }

        /// <summary>
        /// 用途 / 备注
        /// </summary>
        [DataMember]
        [Required]
        public string Usage { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        [DataMember]
        [Required]
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        [DataMember]
        public decimal FeeAmount { get; set; }

        /// <summary>
        /// 付款方式 1  普通订单支付 2收款方收款成功后，再冻结资金
        /// </summary>
        [DataMember]
        [Required]
        public  int PayType { get; set; }

        /// <summary>
        /// 扫码支付 3：支付宝  4：微信
        /// </summary>
        [DataMember]
        public int SecPayType { get; set; }


        #region 展示用

        /// <summary>
        /// 金额展示
        /// </summary>
        [DataMember]
        public string AmountShow { get; set; }

        //手机号
        [DataMember]
        public string Phone { get; set; }

        //密码
        [DataMember]
        public string Password { get; set; }

        //短信验证码
        [DataMember]
        public string PhoneCode { get; set; }

        #endregion
    }
}
