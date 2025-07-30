using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.InAndOutTransaction.TransactionResultSearch
{
    /// <summary>
    /// 出入金结果查询[T2012]
    /// </summary>
    public class MSG
    {
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 原出入金交易的合作方交易流水号 R 32
        /// </summary>
        
        public string OrgSrl { get; set; }

    }
}
