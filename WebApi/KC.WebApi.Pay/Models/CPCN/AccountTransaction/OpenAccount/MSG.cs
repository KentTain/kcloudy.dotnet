using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KC.WebApi.Pay.Models.AccountTransaction.OpenAccount
{
    /// <summary>
    /// 开户 T1001
    /// </summary>
    public class MSG
    {
        /// <summary>
        /// 公共信息
        /// </summary>
        
        public MSGHD MSGHD { get; set; }
        /// <summary>
        /// 账户信息
        /// </summary>
        
        public CltAcc CltAcc { get; set; }
        /// <summary>
        /// 客户信息
        /// </summary>
        
        public Clt Clt { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        
        public Srl Srl { get; set; }
        /// <summary>
        /// 业务功能标示(1:开户、 2：修改、 3：销户) 必填
        /// </summary>
        
        public int FcFlg { get; set; }

        /// <summary>
        /// 账户类型(1:客户资金账户 2:合作方备付金账户 3:合作方收益账户) 必填
        /// </summary>
        
        public int AccTp { get; set; }
    }
}