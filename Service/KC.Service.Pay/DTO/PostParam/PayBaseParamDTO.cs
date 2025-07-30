
using KC.Enums.Pay;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class PayBaseParamDTO
    {
        /// <summary>
        /// 支付是否交易成功
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [DataMember]
        [Required]
        public string MemberId { get; set; }
        
        /// <summary>
        /// 操作人名
        /// </summary>
        [DataMember]
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 加密字符串
        /// </summary>
        [DataMember]
        public string EncryptString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [Required]
        public long Timestamp { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// 第三方支付类型
        /// </summary>
        [DataMember]
        public ThirdPartyType ThirdPartyType { get; set; }
    }
}
