using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    /// <summary>
    /// 签约添金宝
    /// </summary>
    public class VisTJBSignModel : BoHaiBaseModel
    {
        /// <summary>
        /// 平台会员号
        /// </summary>
        public string MerUserId { get; set; }

        /// <summary>
        /// 平台会员账户类型  01:基本户 02:保证金户 03:结算户（用于校验交易密码）
        /// </summary>
        public string VirlAcctType { get; set; }

        /// <summary>
        /// 交易密码  Flag为非1时必输注：Des加密密文
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否验证密码 非1(默认0)：验密   1：不验密
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 产品代码 BC001
        /// </summary>
        public string ProdCode { get; set; }
    }
}