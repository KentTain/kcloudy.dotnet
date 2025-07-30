using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.AccountTransaction.OpenAccountResult
{
    /// <summary>
    /// 开户结果查询 T1002
    /// </summary>
    public class MSG
    {
        /// <summary>
        /// 公共信息
        /// </summary>
        
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 开户时合作方交易流水号 R 32
        /// </summary>
        
        public string SrcSrl { get; set; }
    }
}
