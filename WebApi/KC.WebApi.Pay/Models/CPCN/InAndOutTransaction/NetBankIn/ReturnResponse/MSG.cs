using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.NetBankIn.ReturnResponse
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 1 网关跳转
        /// </summary>
        
        public int ProcMode { get; set; }

        /// <summary>
        /// 网关跳转地址(网关跳转必填)
        /// </summary>
        
        public string Url { get; set; }

        
        public Srl Srl { get; set; }
    }
}
