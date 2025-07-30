using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.BankOutApply
{
    public class BkAcc
    {
        /// <summary>
        /// 银行账号(卡号) R 32
        /// </summary>
        
        public string AccNo { get; set; }

        /// <summary>
        /// 开户名称 R 128
        /// </summary>
        
        public string AccNm { get; set; }
    }
}
