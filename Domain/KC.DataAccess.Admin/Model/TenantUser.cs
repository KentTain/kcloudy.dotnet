using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Admin.Constants;
using ProtoBuf;

namespace KC.Model.Admin
{
    [Table(Tables.TenantUser)]
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class TenantUser : Tenant
    {
        public TenantUser()
        {
            CanEdit = true;
            Applications = new List<TenantUserApplication>();
            TenantSettings = new List<TenantUserSetting>();
            OpenApplicationErrorLogs = new List<TenantUserOpenAppErrorLog>();
        }

        [DataMember]
        [ProtoMember(2)]
        public bool CanEdit { get; set; }
        //[MaxLength(128)]
        [DataMember]
        [ProtoMember(3)]
        public string ReferenceId { get; set; }
        [DataMember]
        [ProtoMember(4)]
        public int DatabasePoolId { get; set; }
        [DataMember]
        [ProtoMember(5)]
        [ForeignKey("DatabasePoolId")]
        public DatabasePool OwnDatabase { get; set; }

        [DataMember]
        [ProtoMember(6)]
        public int? StoragePoolId { get; set; }
        [DataMember]
        [ProtoMember(7)]
        [ForeignKey("StoragePoolId")]
        public StoragePool OwnStorage { get; set; }

        [DataMember]
        [ProtoMember(8)]
        public int? QueuePoolId { get; set; }
        [DataMember]
        [ProtoMember(9)]
        [ForeignKey("QueuePoolId")]
        public QueuePool OwnQueue { get; set; }

        [DataMember]
        [ProtoMember(10)]
        public int? NoSqlPoolId { get; set; }
        [DataMember]
        [ProtoMember(11)]
        [ForeignKey("NoSqlPoolId")]
        public NoSqlPool OwnNoSql { get; set; }

        [DataMember]
        [ProtoMember(12)]
        public int? ServiceBusPoolId { get; set; }
        [DataMember]
        [ProtoMember(13)]
        [ForeignKey("ServiceBusPoolId")]
        public ServiceBusPool ServiceBusPool { get; set; }

        [DataMember]
        [ProtoMember(14)]
        public int? VodPoolId { get; set; }
        [DataMember]
        [ProtoMember(15)]
        [ForeignKey("VodPoolId")]
        public VodPool VodPool { get; set; }

        [DataMember]
        [ProtoMember(16)]
        public int? CodePoolId { get; set; }
        [DataMember]
        [ProtoMember(17)]
        [ForeignKey("CodePoolId")]
        public CodeRepositoryPool CodePool { get; set; }

        /// <summary>
        /// 企业经营模式
        /// </summary>
        [DataMember]
        [ProtoMember(46)]
        public BusinessModel BusinessModel { get; set; }

        [MaxLength(128)]
        [DataMember]
        [ProtoMember(47)]
        public string IndustryId { get; set; }

        [MaxLength(1024)]
        [DataMember]
        [ProtoMember(48)]
        public string IndustryName { get; set; }

        [DataMember]
        [ProtoMember(49)]
        public TenantUserAuthentication AuthenticationInfo { get; set; }

        [ProtoMember(18)]
        [XmlIgnore]
        public virtual IList<TenantUserApplication> Applications { get; set; }

        //[ProtoMember(19)]
        [XmlIgnore]
        public virtual IList<TenantUserOpenAppErrorLog> OpenApplicationErrorLogs { get; set; }

        [DataMember]
        [ProtoMember(45)]
        public virtual ICollection<TenantUserSetting> TenantSettings { get; set; }

        public string GetOwnDomain()
        {
            return TenantSettings.ToList().Any(s => s.TenantId == TenantId
                    && s.Name.Equals(TenantConstant.PropertyName_OwnDomainSetting, StringComparison.OrdinalIgnoreCase))
                        ? TenantSettings.ToList().FirstOrDefault(t => t.TenantId == TenantId && t.Name.Equals(TenantConstant.PropertyName_OwnDomainSetting, StringComparison.OrdinalIgnoreCase)).Value
                        : string.Empty;
        }
    }
}
