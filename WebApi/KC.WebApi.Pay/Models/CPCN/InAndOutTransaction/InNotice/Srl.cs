using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.InNotice
{
    public class Srl
    {
        /// <summary>
        /// 平台交易流水号
        /// </summary>
        public string PlatSrl { get; set; }

        /// <summary>
        /// 原渠道网银入金合作方流水号(TrsFlag= 1 时有值)
        /// </summary>
        public string SrcPtnSrl { get; set; }
    }
}