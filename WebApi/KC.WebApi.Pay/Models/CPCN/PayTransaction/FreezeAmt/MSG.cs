using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.PayTransaction.FreezeAmt
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        
        public CltAcc CltAcc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        
        public Amt Amt { get; set; }

        /// <summary>
        /// 冻结业务标示 A00 冻结B00 解冻 R
        /// </summary>
        
        public string TrsFlag { get; set; }

        /// <summary>
        /// 资金用途(附言)
        /// </summary>
        
        public string Usage { get; set; }

        
        public Srl Srl { get; set; }

    }
}
