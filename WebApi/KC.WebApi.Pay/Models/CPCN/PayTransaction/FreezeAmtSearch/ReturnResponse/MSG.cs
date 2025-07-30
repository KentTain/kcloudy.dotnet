using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.PayTransaction.FreezeAmtSearch.ReturnResponse
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 交易结果:1 成功 2 失败
        /// </summary>
        
        public int State { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        
        public string Opion { get; set; }

        
        public Srl Srl { get; set; }
    }
}
