using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.BankSupportList
{
    /// <summary>
    /// 支持银行列表查询[T1008]
    /// </summary>
    public class MSG
    {
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 查询标记 
        /// UDK 查询支持绑定该行结算账户的银行列表
        /// U3B 查询支持开通快捷支付业务的银行列表
        /// U6G 查询支持企业网银支付的银行列表
        /// U6P 查询支持个人网银支付的银行列表
        /// ALL 查询全部银行列表
        /// </summary>
        public string QryFlag { get; set; }
    }
}
