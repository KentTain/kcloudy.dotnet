using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Admin.Constants;
using KC.Framework.Base;
using ProtoBuf;

namespace KC.Model.Admin
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    [Table(Tables.TenantUserAppModule)]
    public class TenantUserAppModule : EntityBase
    {
        [Key]
        [DataMember]
        [ProtoMember(1)]
        public int Id { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public Guid ModuleId { get; set; }
        [DataMember]
        [ProtoMember(3)]
        [Display(Name = "模块名称")]
        public string ModuleName { get; set; }
        [DataMember]
        [ProtoMember(4)]
        [Display(Name = "模块描述")]
        public string Description { get; set; }
        [DataMember]
        [ProtoMember(5)]
        [Display(Name = "模块涉及程序集")]
        public string AssemblyName { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual TenantUserApplication Application { get; set; }
    }
}
