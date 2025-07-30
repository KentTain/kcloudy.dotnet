using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.PayTransaction.FreezeAmtSearch
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 原冻结/解冻交易的合作方业务单号
        /// </summary>
        
        public string OrgSrl { get; set; }
    }
}
