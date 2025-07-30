using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    [ProtoContract]
    public class ReceivableDTO : EntityDTO
    {
        public ReceivableDTO()
        {
            Logs = new List<PayableAndReceivableRecordDTO>();
        }

        [ProtoMember(1)]
        [DataMember]
        public int Id { get; set; }

        [ProtoMember(2)]
        [DataMember]
        public string PayableNumber { get; set; }

        [ProtoMember(3)]
        [DataMember]
        public ReceivableType Type { get; set; }

        [ProtoMember(4)]
        [DataMember]
        public ReceivableSource Source { get; set; }

        [DisplayName("TypeName")]
        [DataMember]
        public string TypeName { get { return Type.ToDescription(); } }

        [DisplayName("SourceName")]
        [DataMember]
        public string SourceName { get { return Source.ToDescription(); } }

        [ProtoMember(5)]
        [DisplayName("OrderId")]
        [DataMember]
        public string OrderId { get; set; }

        [ProtoMember(6)]
        [DisplayName("OrderAmount")]
        [DataMember]
        public decimal OrderAmount { get; set; }

        [ProtoMember(7)]
        [DisplayName("ReceivableAmount")]
        [DataMember]
        public decimal ReceivableAmount { get; set; }

        [ProtoMember(8)]
        [DisplayName("AlreadyPayAmount")]
        [DataMember]
        public decimal AlreadyPayAmount { get; set; }

        [ProtoMember(9)]
        [DataMember]
        public DateTime StartDate { get; set; }

        [DisplayName("StartDateStr")]
        [DataMember]
        public string StartDateStr { get { return StartDate.ToString("yyyy-MM-dd"); } }

        [ProtoMember(10)]
        [DisplayName("Customer")]
        [DataMember]
        public string Customer { get; set; }

        [ProtoMember(11)]
        [DataMember]
        public string CustomerTenant { get; set; }

        [ProtoMember(12)]
        [DisplayName("Remark")]
        [DataMember]
        public string Remark { get; set; }

        [DisplayName("Status")]
        public string Status
        {
            get
            {
                if (AlreadyPayAmount == 0)
                    return "未支付";
                return "支付中";
            }
        }

        [DataMember]
        [DisplayName("UnPaidTotal")]
        public decimal UnPaidTotal
        {
            get
            {
                return ReceivableAmount - AlreadyPayAmount;
            }
        }

        public bool CanConfirmReceivables { get; set; }

        [DataMember]
        public List<PayableAndReceivableRecordDTO> Logs { get; set; }
    }

}
