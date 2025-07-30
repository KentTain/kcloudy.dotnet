using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CMB
{
    public class APPAYSAVZ : CMBBaseModel
    {
        /// <summary>
        /// 业务流水号
        /// </summary>
        public string BUSNBR { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        public string ERRCOD { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ERRMSG { get; set; }

        /// <summary>
        /// 记录序号
        /// </summary>
        public string RECNUM { get; set; }

        /// <summary>
        /// 客户参考业务号(ERP系统唯一编号，此编号同一渠道不能重复提交)
        /// </summary>
        public string REFNBR { get; set; }
    }
}