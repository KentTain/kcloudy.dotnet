using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Framework.Extension;
using ProtoBuf;

namespace KC.Service.DTO
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public abstract class ProcessLogBaseDTO : EntityBaseDTO
    {
        public ProcessLogBaseDTO()
        {
            OperateDate = DateTime.UtcNow;
        }
        [DataMember]
        [ProtoMember(1)]
        public int ProcessLogId { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public Framework.Base.ProcessLogType Type { get; set; }
        [XmlIgnore]
        [DataMember]
        [ProtoMember(3)]
        public string TypeString
        {
            get
            {
                return Type.ToDescription();
            }
        }

        [MaxLength(128)]
        [DataMember]
        [ProtoMember(4)]
        public string OperatorId { get; set; }
        [MaxLength(50)]
        [DataMember]
        [ProtoMember(5)]
        public string Operator { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public System.DateTime OperateDate { get; set; }
        [MaxLength(4000)]
        [DataMember]
        [ProtoMember(7)]
        public string Remark { get; set; }

    }
}
