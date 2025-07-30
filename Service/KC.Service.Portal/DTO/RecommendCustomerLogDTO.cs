using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Portal.Constants;
using KC.Framework.Base;
using System;
using System.Runtime.Serialization;

namespace KC.Service.DTO.Portal
{
    /// <summary>
    /// 商品日志
    /// </summary>

    [Serializable, DataContract(IsReference = true)]
    public class RecommendCustomerLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public int RecommendId { get; set; }
        
        /// <summary>
        /// 引用商品/招采Id
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string RecommendRefCode { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string RecommendCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string RecommendName { get; set; }
    }
}
