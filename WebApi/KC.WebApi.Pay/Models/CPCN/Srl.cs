using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models
{
    public class Srl
    {

        /// <summary>
        /// 合作方交易流水号 R 32
        /// </summary>
        
        public string PtnSrl { get; set; }

        /// <summary>
        /// 平台交易流水号 R 32 返回
        /// </summary>
        
        public string PlatSrl { get; set; }
    }
}
