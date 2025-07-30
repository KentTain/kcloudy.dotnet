using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public class StdProvinceDTO
    {
        /// <summary>
        /// 省份Id
        /// </summary>
        [DataMember]
        public int ProvId { get; set; }

        /// <summary>
        /// 省份编码
        /// </summary>
        [DataMember]
        public string ProvCode { get; set; }

        /// <summary>
        /// 省份名称
        /// </summary>
        [DataMember]
        public string ProvName { get; set; }

        /// <summary>
        /// 省份排序
        /// </summary>
        [DataMember]
        public int ProvOrd { get; set; }

        /// <summary>
        /// 备用1
        /// </summary>
        [DataMember]
        public string Spec16 { get; set; }

        /// <summary>
        /// 备用2
        /// </summary>
        [DataMember]
        public string Spec32 { get; set; }
    }
}
