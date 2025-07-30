using KC.WebApi.Pay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CPCN.InAndOutTransaction.QRPay.ReturnResponse
{
    public class MSG
    {
        public MSGHD MSGHD { get; set; }

        /// <summary>
        /// 返回地址PayType=6 时为二维码的CODE 地址
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// PayType=6 时返回二维码 图片地址
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// PayType=8 时返回授权码
        /// </summary>
        public string AuthCode { get; set; }

        public Srl Srl { get; set; }
    }
}