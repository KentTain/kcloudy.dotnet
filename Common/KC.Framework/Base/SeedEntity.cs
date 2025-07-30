using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Framework.Base
{
    [Serializable, DataContract(IsReference = true)]
    public class SeedEntity : EntityBase
    {
        [Key]
        [DataMember]
        public string SeedType { get; set; }
        [DataMember]
        public string SeedValue { get; set; }
        [DataMember]
        public int SeedMin { get; set; }
        [DataMember]
        public int SeedMax { get; set; }
    }
}
