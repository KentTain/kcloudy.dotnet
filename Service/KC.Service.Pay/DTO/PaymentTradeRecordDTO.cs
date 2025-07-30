
using KC.Service.DTO;
using KC.Enums.Pay;
using KC.Framework.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class PaymentTradeRecordDTO : EntityDTO
    {
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [DataMember]
        public string MemberId { get; set; }

        [DataMember]
        public string TenantDisplayName { get; set; }
        /// <summary>
        /// 平台生成的流水号
        /// </summary>
        [DataMember]
        public string SrlNo { get; set; }

        /// <summary>
        /// 第三方返回的流水号
        /// </summary>
        [DataMember]
        public string RetSrlNo { get; set; }

        /// <summary>
        /// 第三方支付的名称 默认赋值CPCN
        /// </summary>
        [DataMember]
        public string PaymentName { get; set; }

        /// <summary>
        /// 第三方支付的类型
        /// </summary>
        [DataMember]
        public ThirdPartyType PaymentType { get; set; }
        /// <summary>
        /// 第三方支付的类型
        /// </summary>
        [DataMember]
        public string PaymentTypeStr
        {
            get
            {
                return PaymentType.ToDescription();
            }
        }

        /// <summary>
        /// 请求第三方接口的名称
        /// </summary>
        [DataMember]
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
        [DataMember]
        public string IsSuccessStr
        {
            get
            {
                return IsSuccess?"成功":"失败";
            }
        }
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