using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.OutSettleAccountTime
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        
        public CltAcc CltAcc { get; set; }

        
        public Srl Srl { get; set; }

        /// <summary>
        /// 结算状态1 已发送结算申请
        /// </summary>
        
        public int UBalSta { get; set; }

        /// <summary>
        /// 结算时间格式 YYYYMMDDHH24MISSUBalSta=1 时指成功发送结算申请的时间
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
        ///  原交易时间
        /// </summary>
        
        public string FTime { get; set; }

        /// <summary>
        ///  备用 1
        /// </summary>
        
        public string Spec1 { get; set; }

        /// <summary>
        /// 备用 2
        /// </summary>
        
        public string Spec2 { get; set; }
    }
}
