using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Enums.App;
using KC.Service.DTO;

namespace KC.Service.DTO.Admin
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class TenantUserOperationLogDTO : ProcessLogBaseDTO
    {
        public TenantUserOperationLogDTO()
        {
            AppStatusName = AppStatus.ToDescription();
        }


        [DataMember]
        public Guid ApplicationId { get; set; }

        [DataMember]
        public string ApplicationName { get; set; }

        [DataMember]
        public string DomainName { get; set; }

        [DataMember]
        public ApplicationStatus AppStatus { get; set; }

        [DataMember]
        public string AppStatusName { get; set; }

        [DataMember]
        public int? TenantUserApplicationId { get; set; }

        [DataMember]
        public TenantUserApplicationDTO TenantUserApplication { get; set; }
        [DataMember]
        public string TenantName { get; set; }
        [DataMember]
        public string TenantDisplayName { get; set; }
        [DataMember]
        public OperationLogType LogType { get; set; }
        [DataMember]
        public string logtypeName { get; set; }
        [DataMember]
        public string AdditionalInfo { get; set; }
    }
}
