using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.WithholdingIn.WithholdingInSearch.ReturnResponse
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        
        public CltAcc CltAcc { get; set; }

        
        public Amt Amt { get; set; }

        
        public Srl Srl { get; set; }

        
        public string TrsFlag { get; set; }

        
        public Qpy Qpy { get; set; }

        /// <summary>
        /// 交易成功/失败时间(渠道通知时间)格式:YYYYMMDDHH24MISS
        /// </summary>
        
        public string RestTime { get; set; }

        /// <summary>
        /// 资金用途(附言)
        /// </summary>
        
        public string Usage { get; set; }

        /// <summary>
        /// 原交易日期
        /// </summary>
        
        public string FDate { get; set; }

        /// <summary>
        /// 原交易时间
        /// </summary>
        
        public string FTime { get; set; }

        /// <summary>
        /// 备用 1
        /// </summary>
        
        public string Spec1 { get; set; }

        /// <summary>
        /// 备用 2
        /// </summary>
        
        public string Spec2 { get; set; }

    }
}
