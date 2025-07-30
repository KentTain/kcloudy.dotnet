using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using KC.Service.DTO;

namespace KC.Service.DTO.Portal
{

    [Serializable, DataContract(IsReference = true)]
    public class CompanyProcessLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public string CompanyName { get; set; }

        /// <summary>
        /// 企业编号
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }
    }
}
