using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Admin
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class MobileLocationDTO : EntityBaseDTO
    {
        public int Id { get; set; }

        public string Mobile { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Corp { get; set; }

        public string AreaCode { get; set; }

        public string PostCode { get; set; }
    }
}
