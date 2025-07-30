using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.SettlementAccountInfo.ReturnResponse
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 绑定结果（RspCode=000000 时返回）：1 绑定成功2 申请绑定/变更成功，需人工审核激活
        /// 3 申请绑定/变更成功， 需要短信验证4 申请绑定/变更成功，需要打款验证（被动打款）
        /// </summary>
        
        public int State { get; set; }

        /// <summary>
        /// 结算账户信息
        /// </summary>
        
        public BkAcc BkAcc { get; set; }
    }
}
