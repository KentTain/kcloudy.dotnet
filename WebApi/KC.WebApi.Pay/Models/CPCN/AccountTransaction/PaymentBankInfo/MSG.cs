using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.PaymentBankInfo
{
    /// <summary>
    /// 支付行信息模糊查询[T1017]
    /// </summary>
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 查询标记UDK:查询支持绑定该行结算账户的网点列表 R
        /// </summary>
        
        public string QryFlag { get; set; }

        /// <summary>
        /// 银行编号
        /// </summary>
        
        public string BkId { get; set; }

        /// <summary>
        /// 开户网点编号
        /// </summary>
        
        public string OpenBkCd { get; set; }

        /// <summary>
        /// 开户网点名称-模块匹配
        /// </summary>
        
        public string OpenBkNm { get; set; }

        /// <summary>
        /// 开户网点城市编号
        /// </summary>
        
        public string CityCd { get; set; }

        /// <summary>
        /// 查询笔数，至多 20 笔 R
        /// </summary>
        
        public int QueryNum { get; set; }
    }
}
