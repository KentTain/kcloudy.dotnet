using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Framework.Base
{
    [ProtoContract]
    [Serializable, DataContract(IsReference = true)]
    public class EFKeyValuePair<T, V>
    {
        public EFKeyValuePair(T key, V value)
        {
            Key = key;
            Value = value;
        }

        [DataMember]
        [ProtoMember(1)]
        public T Key { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public V Value { get; set; }
    }
}
