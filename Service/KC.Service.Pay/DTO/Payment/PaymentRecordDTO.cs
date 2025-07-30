using KC.Service.DTO;
using KC.Enums.Pay;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class PaymentRecordDTO : EntityDTO
    {
        public PaymentRecordDTO()
        {
            Orders = new List<PaymentOfOrderDTO>();
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string SellerTenatName { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string Seller { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string BuyerTenatName { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string Buyer { get; set; }

        [DataMember]
        public Guid? AccountStatementId { get; set; }

        [DataMember]
        public string AccountStatementNumber { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string PayNumber { get; set; }

        [DataMember]
        public bool IsConfirm { get; set; }

        [DataMember]
        public List<PaymentOfOrderDTO> Orders { get; set; }

        public string PayDateTime { get { return ModifiedDate.AddHours(8).ToString("yyyy-MM-dd HH:mm"); } }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class PaymentOfOrderDTO : EntityDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid PaymentRecordId { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string OrderNumber { get; set; }

        [DataMember]
        public string PONumber { get; set; }

        [DataMember]
        public string SONumber { get; set; }
    }
}
