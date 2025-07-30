using KC.Enums;
using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Model.Pay.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    [Table(Tables.PaymentTradeRecord)]
    //支付相关的操作流水
    public class PaymentTradeRecord : Entity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [DataMember]
        public string MemberId { get; set; }

        /// <summary>
        /// 平台生成的流水号
        /// </summary>
        [DataMember]
        [MaxLength(40)]
        public string SrlNo { get; set; }

        /// <summary>
        /// 第三方返回的流水号
        /// </summary>
        [DataMember]
        [MaxLength(40)]
        public string RetSrlNo { get; set; }

        /// <summary>
        /// 第三方支付的名称 默认赋值CPCN
        /// </summary>
        [DataMember]
        [MaxLength(30)]
        public string PaymentName { get; set; }

        /// <summary>
        /// 第三方支付的类型
        /// </summary>
        [DataMember]
        public ThirdPartyType PaymentType { get; set; }


        /// <summary>
        /// 请求第三方接口的名称
        /// </summary>
        [DataMember]
        [MaxLength(30)]
        public string InterfaceName { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [DataMember]
        public PaymentOperationType OperationType { get; set; }

        /// <summary>
        /// 是否操作成功
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 返回的代码
        /// </summary>
        [DataMember]
        public string RetCode { get; set; }

        /// <summary>
        /// 返回的消息
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// 关联Id
        /// </summary>
        [DataMember]
        public int ReferenceId { get; set; }

        /// <summary>
        /// 请求的XML
        /// </summary>
        [DataMember]
        public string PostXML { get; set; }

        /// <summary>
        /// 返回的XML
        /// </summary>
        [DataMember]
        public string ReturnXML { get; set; }

    }
}
