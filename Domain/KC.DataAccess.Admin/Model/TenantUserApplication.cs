using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Enums.App;
using KC.Model.Admin.Constants;
using KC.Framework.Base;
using ProtoBuf;
using KC.Framework.Tenant;

namespace KC.Model.Admin
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    [Table(Tables.TenantUserApplication)]
    public class TenantUserApplication : EntityBase
    {
        public TenantUserApplication()
        {
            //ApplicationModules = new List<TenantUserAppModule>();
            //TenantUserOperationLogs = new List<TenantUserOperationLog>();
        }
        [Key]
        [DataMember]
        [ProtoMember(1)]
        public int Id { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public Guid ApplicationId { get; set; }
        [DataMember]
        [ProtoMember(3)]
        [Display(Name = "应用程序名称")]
        public string ApplicationName { get; set; }
        [DataMember]
        [ProtoMember(4)]
        [Display(Name = "域名")]
        public string DomainName { get; set; }
        [DataMember]
        [ProtoMember(5)]
        public ApplicationStatus AppStatus { get; set; }
        [DataMember]
        [ProtoMember(6)]
        [MaxLength(128)]
        public string WebSiteName { get; set; }
        [DataMember]
        [ProtoMember(7)]
        [Display(Name = "模块涉及程序集")]
        public string AssemblyName { get; set; }
        [DataMember]
        [ProtoMember(8)]
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public TenantUser TenantUser { get; set; }
        //[DataMember]
        //[ProtoMember(9)]
        //public virtual ICollection<TenantUserAppModule> ApplicationModules { get; set; }
        //[XmlIgnore]
        //public virtual ICollection<TenantUserOperationLog> TenantUserOperationLogs { get; set; }
        [DataMember]
        [ProtoMember(10)]
        public TenantVersion Version { get; set; }
    }
}
