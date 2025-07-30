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
    public class NoSqlPoolDTO : EntityDTO
    {
        public NoSqlPoolDTO()
        {
            CanEdit = true;
        }

        [DataMember]
        [ProtoMember(1)]
        public int NoSqlPoolId { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public string Endpoint { get; set; }
        [DataMember]
        [ProtoMember(3)]
        public string AccessName { get; set; }
        [DataMember]
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
        public NoSqlType NoSqlType { get; set; }
        [DataMember]
        public string CloudTypeString
        {
            get
            {
                return CloudType.ToDescription();
            }
        }
        [DataMember]
        public string NoSqlTypeString
        {
            get
            {
                return NoSqlType.ToDescription();
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

        public string GetNoSqlConnectionString(string privateKey)
        {
            if (string.IsNullOrEmpty(AccessKeyPasswordHash) || string.IsNullOrEmpty(AccessName))
                return KC.Framework.Base.GlobalConfig.NoSqlConnectionString;

            const string conn = @"TableEndpoint={0};DefaultEndpointsProtocol=https;AccountName={1};AccountKey={2}";
                return string.Format(conn, Endpoint, AccessName,
                string.IsNullOrEmpty(privateKey)
                    ? EncryptPasswordUtil.DecryptPassword(AccessKeyPasswordHash)
                    : EncryptPasswordUtil.DecryptPassword(AccessKeyPasswordHash, privateKey));
        }
    }
}
