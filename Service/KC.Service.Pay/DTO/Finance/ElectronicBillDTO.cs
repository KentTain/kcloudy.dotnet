using KC.Service.DTO;
using KC.Enums.Pay;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ElectronicBillDTO : EntityDTO
    {
        public ElectronicBillDTO()
        {
            Orders = new List<ElectronicBillOfOrderDTO>();
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public BankBillStatus BankBillStatus { get; set; }

        [DataMember]
        public string BillNumber { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public bool IsBankAcceptance { get; set; }

        [DataMember]
        public string SellerTenatName { get; set; }

        [DataMember]
        public string Seller { get; set; }

        [DataMember]
        public string BuyerTenatName { get; set; }

        [DataMember]
        public string Buyer { get; set; }

        [DataMember]
        public Guid? AccountStatementId { get; set; }

        [DataMember]
        public string AccountStatementNumber { get; set; }

        /// <summary>
        /// 收方账号
        /// </summary>
        [DataMember]
        public string CRTACC { get; set; }

        /// <summary>
        /// 收方账户名
        /// </summary>
        [DataMember]
        public string CRTNAM { get; set; }

        /// <summary>
        /// 收方联行号
        /// </summary>
        [DataMember]
        public string CRTBRD { get; set; }

        /// <summary>
        /// 担保出票支付模式
        /// </summary>
        [DataMember]
        public bool GRNTEE { get; set; }

        /// <summary>
        /// 支付号
        /// </summary>
        [DataMember]
        public string PayNumber { get; set; }

        [DataMember]
        public bool ConfirmPay { get; set; }

        [DataMember]
        public List<ElectronicBillOfOrderDTO> Orders { get; set; }

        public string PayDateTime { get { return ModifiedDate.AddHours(8).ToString("yyyy-MM-dd HH:mm"); } }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class ElectronicBillOfOrderDTO : EntityDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        public Guid ElectronicBillId { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string OrderNumber { get; set; }


        [DataMember]
        public string PONumber { get; set; }

        [DataMember]
        public string SONumber { get; set; }

        [DataMember]
        public string PayableNumber { get; set; }
    }
}
