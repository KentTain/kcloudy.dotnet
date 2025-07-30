using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.PaymentBankInfo.ReturnResponse
{
    public class PayBnk
    {
        /// <summary>
        /// 银行编号
        /// </summary>
        
        public string BkId { get; set; }

        /// <summary>
        /// 开户网点编号
        /// </summary>
        
        public string OpenBkCd { get; set; }

        /// <summary>
        /// 开户网点名称
        /// </summary>
        
        public string OpenBkNm { get; set; }

        /// <summary>
        /// 开户网点城市编号
        /// </summary>
        
        public string CityCd { get; set; }
    }
}
