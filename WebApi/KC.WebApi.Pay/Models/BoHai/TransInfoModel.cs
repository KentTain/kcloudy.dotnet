using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.BoHai
{
    public class TransInfoModel
    {
        /// <summary>
        /// 12位虚拟账号
        /// </summary>
        public string VirlAcctNo { get; set; }

        /// <summary>
        /// 虚拟账户名
        /// </summary>
        public string VirlAcctName { get; set; }

        /// <summary>
        /// 平台会员账户类型
        /// </summary>
        public string VirlAcctType { get; set; }

        /// <summary>
        /// 17位账号
        /// </summary>
        public string VirEleAcctNo { get; set; }

        /// <summary>
        /// 是否开通添金宝 1:开通 0：未开通
        /// </summary>
        public string FundFlag { get; set; }

        /// <summary>
        /// 账户状态0-正常2：锁定
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 产品代码
        /// </summary>
        public string PrdCode { get; set; }

        /// <summary>
        /// 是否设置交易密码 1:是 0：否
        /// </summary>
        public string PwdFlag { get; set; }

    }
}