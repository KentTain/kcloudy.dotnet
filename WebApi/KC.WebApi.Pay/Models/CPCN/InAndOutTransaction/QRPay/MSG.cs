using KC.WebApi.Pay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.QRPay
{
    public class MSG
    {

        public MSGHD MSGHD { get; set; }

        public CltAcc CltAcc { get; set; }

        public Amt Amt { get; set; }

        public PayInfo PayInfo { get; set; }

        /// <summary>
        /// 发送端标记:0 手机;1PC 端
        /// </summary>
        public int ReqFlg { get; set; }

        /// <summary>
        /// 资金用途(附言)
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// 业务标示 A00 普通入金
        /// </summary>
        public string TrsFlag { get; set; }

        public Srl Srl { get; set; }
    }
}