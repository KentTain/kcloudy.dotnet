using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace KC.WebApi.Pay.Models
{
    /// <summary>
    /// 公共信息
    /// </summary>
    public class MSGHD
    {
        /// <summary>
        /// 交易码 必填 长度：5
        /// </summary>
        
        public string TrCd { get; set; }

        /// <summary>
        /// 交易日期 YYYYMMDD 必填 长度：8
        /// </summary>
        
        public string TrDt { get; set; }

        /// <summary>
        /// 交易时间 HH24MMSS 选填 长度：6
        /// </summary>
        
        public string TrTm { get; set; }

        /// <summary>
        /// 交易发起方(详见数据字典) 必填 长度：2
        /// </summary>
        
        public string TrSrc { get; set; }

        /// <summary>
        ///  合作方编号 必填 长度：8
        /// </summary>
        
        public string PtnCd { get; set; }

        /// <summary>
        /// 托管方编号 必填 长度：8
        /// </summary>
        
        public string BkCd { get; set; }


        /// <summary>
        /// 返回码(000000 成功) 必 长度6
        /// </summary>
        
        public string RspCode { get; set; }
        /// <summary>
        /// 返回信息 选 长度200
        /// </summary>
        
        public string RspMsg { get; set; }
    }
}