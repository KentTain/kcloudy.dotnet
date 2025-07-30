using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.OpenAccount.ReturnResponse
{
    public class CltAcc
    {
        /// <summary>
        /// 客户号 R Len:20
        /// </summary>
        
        public string CltNo { get; set; }

        /// <summary>
        /// 平台客户号 R Len:32
        /// </summary>
        
        public string CltPid { get; set; }

        /// <summary>
        /// 资金账号 R Len:24
        /// </summary>
        
        public string SubNo { get; set; }

        /// <summary>
        /// 户名 R Len:128
        /// </summary>
        
        public string CltNm { get; set; }

        /// <summary>
        /// 银行电子账号（跨行收款账号）
        /// </summary>
        
        public string BnkEid { get; set; }

        /// <summary>
        /// 电子账户归属支行号
        /// </summary>
        
        public string OpenBkCd { get; set; }

        /// <summary>
        /// 电子账户归属支行名称
        /// </summary>
        
        public string OpenBkNm { get; set; }

    }
}
