using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using ProtoBuf;

namespace KC.Service.DTO.Dict
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class DictTypeDTO : EntityDTO
    {
        public DictTypeDTO()
        {
            DictValues = new List<DictValueDTO>();
        }
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        [MaxLength(128)]
        public string Code { get; set; }

        [DataMember]
        [MaxLength(512)]
        public string Name { get; set; }

        [DataMember]
        public bool IsSys { get; set; }

        [DataMember]
        public virtual ICollection<DictValueDTO> DictValues { get; set; }
    }
}
