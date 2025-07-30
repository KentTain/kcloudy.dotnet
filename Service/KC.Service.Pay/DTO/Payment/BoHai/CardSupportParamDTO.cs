using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay.BoHai
{
    /// <summary>
    /// 卡号是否支持渤海云账本查询
    /// </summary>
    public class CardSupportParamDTO : PayBaseParamDTO
    {
        /// <summary>
        /// 银行卡号
        /// </summary>
        [DataMember]
        public string AcctNo { get; set; }
    }
}
