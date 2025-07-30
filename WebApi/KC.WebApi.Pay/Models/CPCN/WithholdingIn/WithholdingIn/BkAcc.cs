using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.WebApi.Pay.Models.WithholdingIn.WithholdingIn
{
    public class BkAcc
    {
        /// <summary>
        /// 银行编号 R
        /// </summary>
        
        public string BkId { get; set; }

        /// <summary>
        /// 银行账号(卡号) R
        /// </summary>
        
        public string AccNo { get; set; }

        /// <summary>
        /// 开户名称 R
        /// </summary>
        
        public string AccNm { get; set; }

        /// <summary>
        /// 账户类型(1: 对公; 2: 对私) R
        /// </summary>
        
        public int AccTp { get; set; }

        /// <summary>
        /// 开户证件类型仅支持填 A(AccTp=2 时必填)
        /// </summary>
        
        public string CdTp { get; set; }

        /// <summary>
        /// 证件号码(AccTp=2 时必填)
        /// </summary>
        
        public string CdNo { get; set; }

        /// <summary>
        /// 卡类型 1 借记卡(AccTp=2 时必填)
        /// </summary>
        
        public string CAccTp { get; set; }

        /// <summary>
        /// 是否需要短信确认1：需要2：不需要(AccTp=2 时必填)
        /// </summary>
        
        public string SMSFlg { get; set; }

        /// <summary>
        /// 手机(SMSFlg=1 必填)
        /// </summary>
        
        public string MobNo { get; set; }
    }
}
