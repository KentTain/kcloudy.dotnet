using System;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Framework.Base;
using ProtoBuf;
using KC.Service.Enums.Admin;

namespace KC.Service.DTO.Admin
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class TenantUserOpenAppErrorLogDTO : ProcessLogBaseDTO
    {
        public TenantUserOpenAppErrorLogDTO()
        {
            Type = ProcessLogType.Failure;
        }

        [DataMember]
        [ProtoMember(2)]
        public string TenantDisplayName { get; set; }
        [DataMember]
        [ProtoMember(5)]
        public OpenServerType OpenServerType { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public string OpenServerTypeName { get; set; }
        [DataMember]
        [ProtoMember(7)]
        public int TenantId { get; set; }

        [DataMember]
        [ProtoMember(8)]
        public string TenantName { get; set; }

        [XmlIgnore]
        public bool IsShowUpgradeDbBtn {
            get
            {
                return ((OpenServerType == OpenServerType.CreateDbUser || OpenServerType == OpenServerType.OpenDatabase)
                        && Type == ProcessLogType.Failure);
            } 
        }

        [XmlIgnore]
        public bool IsShowUpgradeWebBtn
        {
            get
            {
                return (OpenServerType == OpenServerType.OpenWebServer && Type == ProcessLogType.Failure);
            }
        }

        [XmlIgnore]
        public bool IsShowRollbackDbBtn
        {
            get
            {
                return (OpenServerType == OpenServerType.RollbackDatabase && Type == ProcessLogType.Failure);
            }
        }

        [XmlIgnore]
        public bool IsShowResendOpenAppEmailBtn
        {
            get
            {
                return (OpenServerType == OpenServerType.SendOpenEmail && Type == ProcessLogType.Failure);
            }
        }
    }
}
