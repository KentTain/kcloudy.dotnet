using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Service.DTO;

using ProtoBuf;

namespace KC.Service.DTO.Admin
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class ServiceBusPoolDTO:EntityDTO
    {
        public ServiceBusPoolDTO()
        {
            CanEdit = true;
        }

        [DataMember]
        [ProtoMember(1)]
        public int ServiceBusPoolId { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public string Endpoint { get; set; }
        [DataMember]
        [ProtoMember(3)]
        public string AccessName { get; set; }
        [XmlIgnore]
        [ProtoMember(4)]
        public string AccessKeyPasswordHash { get; set; }
        [DataMember]
        [ProtoMember(5)]
        public DateTime? PasswordExpiredTime { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public int TenantCount { get; set; }
        [DataMember]
        [ProtoMember(7)]
        public bool CanEdit { get; set; }
        [DataMember]
        [ProtoMember(8)]
        public CloudType CloudType { get; set; }
        [DataMember]
        [ProtoMember(9)]
        public string Extend1 { get; set; }
        [DataMember]
        [ProtoMember(10)]
        public string Extend2 { get; set; }
        [DataMember]
        [ProtoMember(11)]
        public string Extend3 { get; set; }
        [DataMember]
        [ProtoMember(12)]
        public bool IsEditMode { get; set; }
        [DataMember]
        [ProtoMember(13)]
        public ServiceBusType ServiceBusType { get; set; }
        [DataMember]
        //[ScriptIgnore]
        public string CloudTypeString
        {
            get
            {
                return CloudType.ToDescription();
            }
        }
        [DataMember]
        //[ScriptIgnore]
        public string ServiceBusTypeString
        {
            get
            {
                return ServiceBusType.ToDescription();
            }
        }
        [XmlIgnore]
        //[ScriptIgnore]
        public string AccessKeyPassword
        {
            get
            {
                if (string.IsNullOrEmpty(AccessKeyPasswordHash))
                    return null;

                return EncryptPasswordUtil.DecryptPassword(AccessKeyPasswordHash);
            }
        }

        public string GetServiceBusConnectionString(string privateKey)
        {
            if (string.IsNullOrEmpty(AccessKeyPasswordHash) || string.IsNullOrEmpty(AccessName))
                return KC.Framework.Base.GlobalConfig.ServiceBusConnectionString;

            const string conn = @"Endpoint={0};SharedAccessKeyName={1};SharedAccessKey={2}";
            return string.Format(conn, Endpoint, AccessName,
                string.IsNullOrEmpty(privateKey)
                    ? EncryptPasswordUtil.DecryptPassword(AccessKeyPasswordHash)
                    : EncryptPasswordUtil.DecryptPassword(AccessKeyPasswordHash, privateKey));
        }
    }
}
