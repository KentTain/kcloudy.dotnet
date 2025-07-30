using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Contract
{
    /// <summary>
    /// 系统增值DTO
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class SystemAppreciationDTO
    {
        [DataMember]
        public bool IsSelect { get; set; }

        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 增值服务名称
        /// </summary>
        [DataMember]
        public string AppreciationName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }


        /// <summary>
        /// 单价
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }


        /// <summary>
        /// 定价标准
        /// </summary>
        [DataMember]
        public string PriceStr { get; set; }

        /// <summary>
        /// 计数规则 -1：面议，0：没有期限， 1：天， 2：月， 3：年
        /// </summary>
        [DataMember]
        public int CountingRule { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public float Quantity { get; set; }

        /// <summary>
        /// 服务期限
        /// </summary>
        [DataMember]
        public string QuantityStr { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }
    }

 
}
