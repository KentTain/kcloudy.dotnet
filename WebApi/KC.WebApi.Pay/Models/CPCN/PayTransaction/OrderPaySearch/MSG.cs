using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.PayTransaction.OrderPaySearch
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 待查询支付单号(唯一)
        /// </summary>
        
        public string BillNo { get; set; }
    }
}
