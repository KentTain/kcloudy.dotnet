using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.NetBankIn
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        
        public CltAcc CltAcc { get; set; }

        
        public Amt Amt { get; set; }

        /// <summary>
        /// 页面通知 URL
        /// </summary>
        
        public string NotificationURL { get; set; }

        /// <summary>
        /// 付款银行编号 R 12
        /// </summary>
        
        public string BankID { get; set; }

        /// <summary>
        /// 网银类型(1:企业网银;2:个人网银) R
        /// </summary>
        
        public int PayAccType { get; set; }

        /// <summary>
        /// 资金用途(附言) 64
        /// </summary>
        
        public string Usage { get; set; }

        
        public Srl Srl { get; set; }
    }
}
