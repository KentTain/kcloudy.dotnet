using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ElectronicBillRecordsDTO
    {
        [DataMember]
        public string BillType { get; set; }

        [DataMember]
        public string BillNo { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string AmountStr { get; set; }

        [DataMember]
        public string Date { get; set; }

        [DataMember]
        public string Remark { get; set; }
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
}
