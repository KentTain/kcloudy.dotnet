using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.BankSupportList.ReturnResponse
{
    public class MSG
    {
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 银行信息列表
        /// </summary>
        public List<BankInfo> BankInfos { get; set; }
    }
}
