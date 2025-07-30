using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace KC.Framework.Base
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ConfigAttribute : PropertyAttributeBase
    {
        [DataMember]
        public int ConfigId { get; set; }

        [DataMember]
        public bool IsFileAttr { get; set; }

        [ForeignKey("ConfigId")]
        public virtual ConfigEntity ConfigEntity { get; set; }
    }
}
