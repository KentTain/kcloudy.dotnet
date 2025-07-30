using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class VisGetOTPRetModel : BoHaiReturnModel
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public string ValNum { get; set; }
    }
}