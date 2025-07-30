using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Account
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class UserContactInfoDTO : EntityBaseDTO
    {
        /// <summary>
        /// 会员Guid
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string UserId { get; set; }
        /// <summary>
        /// 会员登陆名
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        public string UserName { get; set; }
        /// <summary>
        /// 会员编号
        /// </summary>
        [DataMember]
        [MaxLength(20)]
        public string MemberId { get; set; }
        /// <summary>
        /// 会员显示名
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        public string DisplayName { get; set; }
        /// <summary>
        /// 会员Email
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string Email { get; set; }
        /// <summary>
        /// 会员手机号
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        public string PhoneNumber { get; set; }
        [DataMember]
        [MaxLength(20)]
        public string ContactQQ { get; set; }
        [DataMember]
        [MaxLength(20)]
        public string Telephone { get; set; }
        /// <summary>
        /// 微信公众号
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string OpenId { get; set; }
    }
}
