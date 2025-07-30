using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class VisTransferQryRetModel : BoHaiReturnModel
    {
        /// <summary>
        /// 记录总数
        /// </summary>
        public int TotalNum { get; set; }

        /// <summary>
        /// 交易循环体
        /// </summary>
        public List<VisTransInfoModel> TransInfoList { get; set; }
    }
}