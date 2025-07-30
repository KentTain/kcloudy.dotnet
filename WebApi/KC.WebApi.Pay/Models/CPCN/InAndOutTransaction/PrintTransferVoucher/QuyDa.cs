using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.PrintTransferVoucher
{
    public class QuyDa
    {
        /// <summary>
        /// 日期 YYYYMMDD
        /// </summary>
        
        public string dte { get; set; }

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
        /// 打印校验码
        /// </summary>
        
        public string VerifyCode { get; set; }
    }
}
