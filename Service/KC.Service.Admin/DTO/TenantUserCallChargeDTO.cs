using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.App;
using KC.Service.DTO;

namespace KC.Service.DTO.Admin
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class TenantUserCallChargeDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string SessionId { get; set; }
        [DataMember]
        public string ChargeNbr { get; set; }
        [DataMember]
        public string DisplayNbr { get; set; }
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public DateTime EndTime { get; set; }
        [DataMember]
        public DateTime StarttimeCalled { get; set; }
        [DataMember]
        public string BillSubtype { get; set; }
        [DataMember]
        public long Duration { get; set; }
        [DataMember]
        public long Points { get; set; }
        [DataMember]
        public string TenantName { get; set; }
        [DataMember]
        public string TenantDisplayName { get; set; }
        [DataMember]
        public string Caller { get; set; }
        [DataMember]
        public string CallerPhone { get; set; }
        [DataMember]
        public string BeCaller { get; set; }
        [DataMember]
        public string BeCallerPhone { get; set; }
        [DataMember]
        public bool IsDownloadVoice { get; set; }
        [DataMember]
        public string DownLoadUrl { get; set; }
        [DataMember]
        public string RecordUrl { get; set; }
        [DataMember]
        public CallChargeStatus CallStatus { get; set; }
        public string CallLogResult { get; set; }
    }
}
