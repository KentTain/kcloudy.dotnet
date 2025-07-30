using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.App;
using KC.Model.Admin.Constants;
using KC.Framework.Base;
using ProtoBuf;

namespace KC.Model.Admin
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    [Table(Tables.TenantUserOperationLog)]
    public class TenantUserOperationLog : ProcessLogBase
    {
        public Guid ApplicationId { get; set; }

        [MaxLength(128)]
        [Display(Name = "应用程序名称")]
        public string ApplicationName { get; set; }

        [MaxLength(128)]
        [Display(Name = "域名")]
        public string DomainName { get; set; }

        public ApplicationStatus AppStatus { get; set; }

        public string TenantName { get; set; }

        public string TenantDisplayName { get; set; }

        public OperationLogType LogType { get; set; }

        public string AdditionalInfo { get; set; }

        public int? TenantUserApplicationId { get; set; }
        [ForeignKey("TenantUserApplicationId")]
        public TenantUserApplication TenantUserApplication { get; set; }
    }
}
