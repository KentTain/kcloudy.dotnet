using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.TransactionResultSearch.ReturnResponse
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        
        public CltAcc CltAcc { get; set; }

        
        public Amt Amt { get; set; }

        
        public Srl Srl { get; set; }

        /// <summary>
        /// 交易结果:1 成功2 失败3 处理中
        /// </summary>
        
        public int State { get; set; }

        /// <summary>
        /// 交易成功/失败时间(渠道通知时间)出金时指交易成功时间， 不是到账时间格式:YYYYMMDDHH24MISS
        /// </summary>
        
        public string RestTime { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        
        public string Opion { get; set; }

        /// <summary>
        /// 出金结算状态(查询出金结果时返回)0 未结算1 已发送结算申请
        /// </summary>
        
        public string UBalSta { get; set; }

        /// <summary>
        /// 出金结算时间(查询出金结果时返回)格式 YYYYMMDDHH24MISSUBalSta=1 时指成功发送结算申请的时间
        /// </summary>
        
        public string UBalTim { get; set; }

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
