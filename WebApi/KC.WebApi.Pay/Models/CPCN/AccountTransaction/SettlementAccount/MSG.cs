using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.SettlementAccount
{
    /// <summary>
    /// 结算账户维护 T1004
    /// </summary>
    public class MSG
    {
        /// <summary>
        /// 公共信息
        /// </summary>
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 账户信息
        /// </summary>
        
        public CltAcc CltAcc { get; set; }

        /// <summary>
        /// 结算账户信息(结算账户信息=1/2时填写)FcFlg=1/2 时，必填
        /// </summary>
        
        public BkAcc BkAcc { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        
        public Srl Srl { get; set; }

        /// <summary>
        /// 业务功能标示(1:绑定、 2： 变更、 3：删除)
        /// </summary>
        
        public int FcFlg { get; set; }
    }
}
