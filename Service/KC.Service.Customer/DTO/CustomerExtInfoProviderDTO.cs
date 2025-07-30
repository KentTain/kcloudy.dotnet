using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Customer
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class CustomerExtInfoProviderDTO : PropertyAttributeBaseDTO
    {
    }
}
