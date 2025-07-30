using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public class SupplyBankDTO
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 银行编号
        /// </summary>
        [DataMember]
        public string BankId { get; set; }

        /// <summary>
        /// 中金支付编制的银行编号
        /// </summary>
        [DataMember]
        public string UBankId { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        [DataMember]
        public string BankName { get; set; }

        /// <summary>
        /// 业务类型
        /// UDK 支持绑定该行结算账户;
        /// U3B 支持对私快捷支付充值中金-储蓄卡;
        /// U6G 支持对公网银充值;
        /// U6P 支持对私网银充值;
        /// </summary>
        [DataMember]
        public string BusiType { get; set; }

        /// <summary>
        ///状态(1:正常; 2:停用)
        /// </summary>
        [DataMember]
        public int Sta { get; set; }

        /// <summary>
        /// 备用1
        /// </summary>
        [DataMember]
        public string Spec16 { get; set; }

        /// <summary>
        /// 备用2
        /// </summary>
        [DataMember]
        public string Spec32 { get; set; }
    }
}
