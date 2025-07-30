using System;
using System.Collections.Generic;
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
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class QueuePoolDTO : EntityDTO
    {
        [DataMember]
        [ProtoMember(1)]
        public int QueuePoolId { get; set; }
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
        public QueueType QueueType { get; set; }
        [DataMember]
        //[JsonIgnore]
        public string CloudTypeString
        {
            get
            {
                return CloudType.ToDescription();
            }
        }
        [DataMember]
        //[JsonIgnore]
        public string QueueTypeString
        {
            get
            {
                return QueueType.ToDescription();
            }
        }
        [XmlIgnore]
        //[JsonIgnore]
        public string AccessKeyPassword
        {
            get
            {
                if (string.IsNullOrEmpty(AccessKeyPasswordHash))
                    return null;

                return EncryptPasswordUtil.DecryptPassword(AccessKeyPasswordHash);
            }
        }

        public string GetQueueConnectionString(string privateKey)
        {
            if (string.IsNullOrEmpty(AccessKeyPasswordHash) || string.IsNullOrEmpty(AccessName))
                return KC.Framework.Base.GlobalConfig.QueueConnectionString;

            const string conn = @"QueueEndpoint={0};DefaultEndpointsProtocol=https;AccountName={1};AccountKey={2}";
            return string.Format(conn, Endpoint, AccessName,
                string.IsNullOrEmpty(privateKey)
                    ? EncryptPasswordUtil.DecryptPassword(AccessKeyPasswordHash)
                    : EncryptPasswordUtil.DecryptPassword(AccessKeyPasswordHash, privateKey));
        }
    }
}
