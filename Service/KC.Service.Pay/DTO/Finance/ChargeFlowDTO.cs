using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ChargeFlowDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 采购订单号
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        [DataMember]
        public string ConsumptionMoney { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        public string ConsumptionDate { get; set; }

        [DataMember]
        public string CashStatus { get; set; }

        [DataMember]
        public string Remark { get; set; }
    }
}
