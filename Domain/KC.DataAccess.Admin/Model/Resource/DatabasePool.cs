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
using KC.Model.Admin.Constants;
using KC.Framework.Base;
using KC.Framework;
using ProtoBuf;

namespace KC.Model.Admin
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    [Table(Tables.DatabasePool)]
    public class DatabasePool : Entity
    {
        public DatabasePool()
        {
            CanEdit = true;
        }

        [Key]
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
        [DataMember]
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

        //public string GetConnectionString(string key)
        //{
        //    if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Database))
        //        return null;

        //    const string conn = @"Data Source={0};Database={1};User ID={2};Password={3};MultipleActiveResultSets=true;";
        //    return string.Format(conn, Server, Database, UserName, EncryptPasswordUtil.DecryptPassword(UserPasswordHash, key));
        //}

        [XmlIgnore]
        [NotMapped]
        //[ScriptIgnore]
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(Server) || string.IsNullOrEmpty(Database))
                    return null;

                const string conn = @"Server={0};Database={1};User ID={2};Password={3};MultipleActiveResultSets=true;";
                return string.Format(conn, Server, Database, UserName, KC.Framework.SecurityHelper.EncryptPasswordUtil.DecryptPassword(UserPasswordHash));
            }
        }
    }
}
