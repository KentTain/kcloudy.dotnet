using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.BankSupportList.ReturnResponse
{
    /// <summary>
    /// 银行信息
    /// </summary>
    public class BankInfo
    {
        /// <summary>
        /// 银行编号(详见《银行编码.pdf》 )
        /// </summary>
        
        public string BkId { get; set; }

        /// <summary>
        ///  银行名称
        /// </summary>
        
        public string BkNm { get; set; }

        /// <summary>
        /// 业务开通状态(0 未开通,1已开通,2 已暂停)(QryFlag不等于 ALL 时有效)
        /// </summary>
        
        public int Sta { get; set; }

        /// <summary>
        /// 暂停原因(Sta=2 时有效)
        /// </summary>
        
        public string Reason { get; set; }
    }
}
