using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.OpenAccount.ReturnResponse
{
    public class MSG
    {
        /// <summary>
        /// 公共信息
        /// </summary>
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 成功时返回
        /// </summary>
        
        public CltAcc CltAcc { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        
        public Srl Srl { get; set; }
    }
}
