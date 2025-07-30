using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.NetBankInSearch
{
    /// <summary>
    /// 出入金结果查询[T2012]
    /// </summary>
    public class MSG
    {

        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// T2005/T2006 交易请求报文中合作方交易流水号
        /// </summary>

        public string OrgSrl { get; set; }

    }
}
