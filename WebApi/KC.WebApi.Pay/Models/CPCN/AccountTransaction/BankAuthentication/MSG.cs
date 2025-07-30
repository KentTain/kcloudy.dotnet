using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.BankAuthentication
{
    /// <summary>
    /// 企业-企业账户认证(打款认证)-验证[T1132]
    /// </summary>
    public class MSG
    {
        public MSGHD MSGHD { get; set; }


        public Srl Srl { get; set; }

        public decimal Amount { get; set; }
    }
}
