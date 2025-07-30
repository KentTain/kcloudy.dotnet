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
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Table(Tables.TenantUserServiceApplication)]
    public class TenantUserServiceApplication : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int TenantId { get; set; }
        [DataMember]
        public string TenantName { get; set; }
        [DataMember]
        public string TenantDisplayName { get; set; }
        [DataMember]
        public ConfigType ServiceType { get; set; }
        [DataMember]
        public ServiceAppStatus AppStatus { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        [DataMember]
        public WorkflowBusStatus WorkFlowStatus { get; set; }

        /// <summary>
        /// 客户经理Id
        /// </summary>
        [DataMember]
        public string OperatorId { get; set; }
        /// <summary>
        /// 客户经理名称
        /// </summary>
        [DataMember]
        public string OperatorDisplayName { get; set; }
    }
}
