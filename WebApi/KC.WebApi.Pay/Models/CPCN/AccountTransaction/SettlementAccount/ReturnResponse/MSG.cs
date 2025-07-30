using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.SettlementAccount.ReturnResponse
{
    public class MSG
    {
        /// <summary>
        /// 公共信息
        /// </summary>
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 绑定/变更/删除结果（RspCode=000000 时返回）：
        /// 1 绑定/变更/删除成功（此模式下无需验证）2 申请绑定/变更成功，需人工审核激活
        /// 3 申请绑定/变更成功， 需要短信验证4 申请绑定/变更成功，需要打款验证（被动打款）9 绑定/变更/删除失败
        /// </summary>
        
        public int State { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        
        public Srl Srl { get; set; }

    }
}
