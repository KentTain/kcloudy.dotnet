using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Framework.Tenant;
using KC.Framework.Base;
using KC.Framework;
using ProtoBuf;
using KC.Framework.SecurityHelper;
using KC.Framework.Extension;

namespace KC.Service.DTO.Admin
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class CodeRepositoryPoolDTO : EntityDTO
    {
        public CodeRepositoryPoolDTO()
        {
            CanEdit = true;
        }

        [DataMember]
        [ProtoMember(1)]
        public int CodePoolId { get; set; }
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
        public CodeType CodeType { get; set; }
        [DataMember]
        [ProtoMember(10)]
        public bool IsEditMode { get; set; }
        [DataMember]
        [ProtoMember(11)]
        public string Extend1 { get; set; }
        [DataMember]
        [ProtoMember(12)]
        public string Extend2 { get; set; }
        [DataMember]
        [ProtoMember(13)]
        public string Extend3 { get; set; }

        [DataMember]
        public string CloudTypeString
        {
            get
            {
                return CloudType.ToDescription();
            }
        }
        [DataMember]
        public string CodeTypeString
        {
            get
            {
                return CodeType.ToDescription();
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

        public string GetCodeConnectionString(string privateKey)
        {
            if (string.IsNullOrEmpty(AccessKeyPasswordHash) || string.IsNullOrEmpty(AccessName))
                return KC.Framework.Base.GlobalConfig.CodeConnectionString;

            const string conn = @"CodeEndpoint={0};AccountName={1};AccountKey={2}";
            return string.Format(conn, Endpoint, AccessName,
            string.IsNullOrEmpty(privateKey)
                ? EncryptPasswordUtil.DecryptPassword(AccessKeyPasswordHash)
                : EncryptPasswordUtil.DecryptPassword(AccessKeyPasswordHash, privateKey));
        }
    }
}
