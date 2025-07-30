using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.WithholdingIn.WithholdingIn.ReturnResponse
{
    public class Qpy
    {
        /// <summary>
        /// 交易结果:1 成功2 失败3 处理中
        /// </summary>
        
        public int State { get; set; }

        /// <summary>
        /// 失败原因
        /// </summary>
        
        public string Opion { get; set; }
    }
}
