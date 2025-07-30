using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KC.Service.DTO.Dict
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class DictValueDTO : EntityDTO
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        [MaxLength(128)]
        public string Code { get; set; }
        [DataMember]
        [MaxLength(512)]
        public string Name { get; set; }

        [DataMember]
        public bool IsSelect { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        [DataMember]
        public int DictTypeId { get; set; }
        [DataMember]
        [MaxLength(128)]
        public string DictTypeCode { get; set; }
        [DataMember]
        public string DictTypeName { get; set; }
    }
}
