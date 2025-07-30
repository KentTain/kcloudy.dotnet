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
    public class PlatformServiceFeeDTO : EntityDTO
    {
        [DataMember]
        public decimal AlreadyPaidServiceFee { get; set; }

        [DataMember]
        public decimal ServiceFee { get; set; }

        [DataMember]
        public string CreatedDateStr { get; set; }

        [DataMember]
        public string SONumber { get; set; }

        [DataMember]
        public string PONumber { get; set; }

        [DataMember]
        public bool BuyerPayServiceFee { get; set; }

        /// <summary>
        /// 服务费率
        /// </summary>
        [DataMember]
        public decimal Percent { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        [DataMember]
        public double Discount { get; set; }

        [DataMember]
        public string OrderNumber { get; set; }
    }
}
