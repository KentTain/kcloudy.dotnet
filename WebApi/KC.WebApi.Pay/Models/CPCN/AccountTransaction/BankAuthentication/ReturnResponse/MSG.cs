using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.BankAuthentication.ReturnResponse
{
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        
        public Srl Srl { get; set; }

        /// <summary>
        /// 认证结果10=成功 20=失败
        /// </summary>
        
        public int Stat { get; set; }

        /// <summary>
        /// 认证结果说明
        /// </summary>
        
        public string Memo { get; set; }

        /// <summary>
        /// 原交易当前可验证次数，当值等于 0 时，系统不再接受该笔交易的验证请求
        /// </summary>
        
        public int AvailableVeriCount { get; set; }
    }
}
