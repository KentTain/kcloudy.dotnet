using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.TransferVoucher.ReturnResponse
{
    /// <summary>
    /// 可重复组件(0 至 N 条)
    /// </summary>
    public class QuyDa
    {
        /// <summary>
        /// 日期 YYYYMMDD
        /// </summary>
        
        public string dte { get; set; }

        /// <summary>
        /// 时间 HH24MISS
        /// </summary>
        
        public string tme { get; set; }

        /// <summary>
        /// 资金账号
        /// </summary>
        
        public string SubNo { get; set; }

        /// <summary>
        /// 电子账号
        /// </summary>
        
        public string Bnkvid { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        
        public string BillNo { get; set; }

        /// <summary>
        /// 合作方交易流水号
        /// </summary>
        
        public string PtnSrl { get; set; }

        /// <summary>
        /// 平台交易流水号
        /// </summary>
        
        public string PlatSrl { get; set; }


        /// <summary>
        /// BkCd=LFYH0001 时指：业务类型
        /// 01-一般入金
        /// 02-电子账户入金
        /// 03-一般出金
        /// 04-超网出金
        /// 05-订单支付
        /// 06-出金转账
        /// 07-跨行转账
        /// 08-跨行收款(超网)
        /// 09-跨行收款(大小额)
        /// </summary>
        
        public string tye { get; set; }

        /// <summary>
        /// 交易金额(单位:分)
        /// </summary>
        
        public decimal AclAmt { get; set; }

        /// <summary>
        /// 手续费金额(单位:分)
        /// </summary>
        
        public decimal FeeAmt { get; set; }

        /// <summary>
        /// 资金余额(单位:分)
        /// </summary>
        
        public decimal BalAmt { get; set; }

        /// <summary>
        /// 打印校验码
        /// </summary>
        
        public string VerifyCode { get; set; }

        /// <summary>
        /// 对方账号
        /// </summary>
        
        public string AccountNo { get; set; }

        /// <summary>
        /// 对方账户名称
        /// </summary>
        
        public string AccountNm { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        
        public string Usage { get; set; }

        /// <summary>
        /// 币种，默认“CNY”
        /// </summary>
        
        public string CcyCd { get; set; }
    }
}
