using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CMB
{

    public class ERPAYSTAX
    {
        /// <summary>
        /// 客户参考业务号  ERP系统唯一编号 不可空
        /// </summary>
        public string REFNBR { get; set; }

        /// <summary>
        /// 业务流水号 CBS生成的流水号 不可空
        /// </summary>
        public string BUSNBR { get; set; }
    }
}