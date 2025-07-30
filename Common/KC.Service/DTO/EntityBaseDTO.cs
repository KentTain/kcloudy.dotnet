using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace KC.Service.DTO
{
    
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public abstract class EntityBaseDTO
    {
    }

}
