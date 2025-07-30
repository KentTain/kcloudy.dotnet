using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Contract
{
    [Serializable]
    [DataContract(IsReference = true)]

    public class ElectronicPersonDTO : EntityDTO //个人用户注册
    {
        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public string TenantName { get; set; }

        [MaxLength(128)]
        [DataMember]
        public string UserId { get; set; }

        [MaxLength(56)]
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public bool IsSync { get; set; }

        /// <summary>
        /// 邮箱地址  可空 length(56)
        /// </summary>
        [MaxLength(56)]
        [Display(Name = "邮箱地址")]
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// 手机号码  不可空 length(11)
        /// </summary>
        [MaxLength(11)]
        [Display(Name = "手机号码")]
        [DataMember]
        public string Mobile { get; set; }

        /// <summary>
        /// 身份证明  不可空 length(18)
        /// </summary>
        [MaxLength(18)]
        [Display(Name = "身份证明")]
        [DataMember]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// 外部系统唯一Id
        /// </summary>
        [MaxLength(128)]
        public string AccountId { get; set; }

        /// <summary>
        /// 印章Id
        /// </summary>
        [MaxLength(256)]
        public string SealId { get; set; }

    }
}
