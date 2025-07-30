using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KC.WebApi.Pay.Models
{
    /// <summary>
    /// 账户信息
    /// </summary>
    public class CltAcc
    {
        /// <summary>
        /// 客户号 必填 长度 20
        /// </summary>
        
        public string CltNo { get; set; }

        /// <summary>
        /// 资金账号 (T1001 当FcFlg=2/3 （ 2：修改、 3：销户）时必填 长度：24 )
        /// </summary>
        
        public string SubNo { get; set; }

        /// <summary>
        /// 户名 必填 长度：128
        /// </summary>
        
        public string CltNm { get; set; }

    }
}