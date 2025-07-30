using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KC.WebApi.Pay.Models.CMB
{
    public class SYCOMRETZ
    {
        /// <summary>
        /// 返回代码
        /// </summary>
        public string ERRCOD { get; set; }

        /// <summary>
        /// 错误详细信息
        /// </summary>
        public string ERRDTL { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ERRMSG { get; set; }
    }
}