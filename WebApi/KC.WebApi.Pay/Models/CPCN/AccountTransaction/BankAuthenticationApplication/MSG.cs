using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.BankAuthenticationApplication
{
    /// <summary>
    /// 企业-企业账户认证(打款认证)-申请[T1131]
    /// </summary>
    public class MSG
    {
        public MSGHD MSGHD { get; set; }

        public Srl Srl { get; set; }

        public BkAcc BkAcc { get; set; }
    }
}
