using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Portal.Constants;
using KC.Framework.Base;
using System.Runtime.Serialization;
using System;

namespace KC.Service.DTO.Portal
{
    /// <summary>
    /// 采购日志
    /// </summary>

    [Serializable, DataContract(IsReference = true)]
    public class RecommendRequirementLogDTO : ProcessLogBaseDTO
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
        /// 采购名称
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string RecommendName { get; set; }
    }
}
