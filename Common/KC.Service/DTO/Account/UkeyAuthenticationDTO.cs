using KC.Service.DTO;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace KC.Service.DTO.Account
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class UkeyAuthenticationDTO : EntityDTO
    {
        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string MemberId { get; set; }

        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public string UnifiedSocialCreditCode { get; set; }

        /// <summary>
        /// 证书主题
        /// </summary>
        [DataMember]
        public string CertificateSubject { get; set; }

        /// <summary>
        /// 证书序列号
        /// </summary>
        [DataMember]
        public string SerialNumber { get; set; }

        /// <summary>
        /// 证书颁发者
        /// </summary>
        [DataMember]
        public string IssuerDN { get; set; }

        /// <summary>
        /// 证书签名信息
        /// </summary>
        [DataMember]
        public string Signature { get; set; }

        /// <summary>
        /// 其他信息
        /// </summary>
        [DataMember]
        public string OtherInfo { get; set; }

        /// <summary>
        /// 是否需要重新UKey认证
        /// </summary>
        [DataMember]
        public bool ReUKeyAuth { get; set; }

        [DataMember]
        public bool ShowModifiedInfo { get; set; }
    }

    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class UkeyAuthenticationJudgeDTO
    {
        [DataMember]
        public bool Result { get; set; }

        [DataMember]
        public string Code { get; set; }
    }
}
