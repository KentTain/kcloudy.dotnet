using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public class StdCityDTO
    {
        /// <summary>
        /// 城市Id
        /// </summary>
        [DataMember]
        public int CityId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ProvId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CityCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CityName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int CityOrd { get; set; }

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
