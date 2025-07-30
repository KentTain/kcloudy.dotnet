using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.PayTransaction.OrderPay
{
    public class billInfo
    {
        /// <summary>
        ///  付款方资金账号 R 24
        /// </summary>
        
        public string PSubNo { get; set; }

        /// <summary>
        /// 付款方户名 R 128
        /// </summary>
        
        public string PNm { get; set; }

        /// <summary>
        /// 收款方资金账号 R 24
        /// </summary>
        
        public string RSubNo { get; set; }

        /// <summary>
        /// 收款方户名 R 128
        /// </summary>
        
        public string RCltNm { get; set; }

        /// <summary>
        /// 业务单号
        /// </summary>
        
        public string OrderNo { get; set; }

        /// <summary>
        /// 支付单号(唯一) R 32
        /// </summary>
        
        public string BillNo { get; set; }

        /// <summary>
        /// 支付金额 R 
        /// </summary>
        
        public Decimal AclAmt { get; set; }

        /// <summary>
        /// 付款方手续费,暂定 0 R
        /// </summary>
        
        public Decimal PayFee { get; set; }

        /// <summary>
        /// 收款方手续费,暂定 0 R
        /// </summary>
        
        public Decimal PayeeFee { get; set; }

        /// <summary>
        /// 币种，默认“CNY” R
        /// </summary>
        
        public string CcyCd { get; set; }

        /// <summary>
        /// 资金用途(附言) 128
        /// </summary>
        
        public string Usage { get; set; }


    }
}
