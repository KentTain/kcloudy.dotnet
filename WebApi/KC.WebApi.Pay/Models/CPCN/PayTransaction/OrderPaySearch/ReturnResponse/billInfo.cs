using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.PayTransaction.OrderPaySearch.ReturnResponse
{
    public class billInfo
    {
        /// <summary>
        ///  支付单号(唯一)
        /// </summary>
        
        public string BillNo { get; set; }

        /// <summary>
        /// 平台交易流水号
        /// </summary>
        
        public string PlatSrl { get; set; }

        /// <summary>
        /// 交易结果:0 失败1 成功2 处理中
        /// </summary>
        
        public string BillState { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        
        public string Opion { get; set; }
    }
}
