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
    public class DatabasePoolDTO : EntityDTO
    {
        public DatabasePoolDTO()
        {
            CanEdit = true;
        }
        [DataMember]
        [ProtoMember(1)]
        public int DatabasePoolId { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public string Server { get; set; }
        [DataMember]
        [ProtoMember(3)]
        public string Database { get; set; }
        [DataMember]
        [ProtoMember(4)]
        public string UserName { get; set; }
        [XmlIgnore]
        [ProtoMember(5)]
        public string UserPasswordHash { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public DateTime? PasswordExpiredTime { get; set; }
        [DataMember]
        [ProtoMember(7)]
        public int TenantCount { get; set; }
        [DataMember]
        [ProtoMember(8)]
        public bool CanEdit { get; set; }

        [DataMember]
        [ProtoMember(9)]
        public CloudType CloudType { get; set; }
        [DataMember]
        [ProtoMember(10)]
        public DatabaseType DatabaseType { get; set; }
        [DataMember]
        [ProtoMember(12)]
        public bool IsEditMode { get; set; }
        [DataMember]
        public string CloudTypeString
        {
            get
            {
                return CloudType.ToDescription();
            }
        }
        [DataMember]
        public string DatabaseTypeString
        {
            get
            {
                return DatabaseType.ToDescription();
            }
        }

        [XmlIgnore]
        //[JsonIgnore]
        public string UserPassword
        {
            get
            {
                if (string.IsNullOrEmpty(UserPasswordHash))
                    return null;

                return EncryptPasswordUtil.DecryptPassword(UserPasswordHash);
            }
        }

        //[XmlIgnore]
        //[JsonIgnore]
        //public string DatabaseConnectionString
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Database) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPasswordHash))
        //            return null;

        //        const string conn = @"Server={0};Database={1};User ID={2};Password={3};MultipleActiveResultSets=true;";
        //        return string.Format(conn, Server, Database, UserName, EncryptPasswordUtil.DecryptPassword(UserPasswordHash));
        //    }
        //}

        public string GetDatabaseConnectionString(string privateKey)
        {
            if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Database) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPasswordHash))
                return null;

            const string conn = @"Server={0};Database={1};User ID={2};Password={3};MultipleActiveResultSets=true;";
            return string.Format(conn, Server, Database, UserName,
                string.IsNullOrEmpty(privateKey)
                    ? EncryptPasswordUtil.DecryptPassword(UserPasswordHash)
                    : EncryptPasswordUtil.DecryptPassword(UserPasswordHash, privateKey));
        }
    }
}
