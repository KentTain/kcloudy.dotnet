using KC.Framework.Base;
using KC.Model.Contract.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Contract
{
    [Serializable]
    [DataContract(IsReference = true)]
    [Table(Tables.ElectronicPerson)]

    public class ElectronicPerson : Entity //个人用户注册
    {
        [Key]
        [MaxLength(128)]
        public string UserId { get; set; }

        [MaxLength(56)]
        public string UserName { get; set; }

        [DataMember]
        public bool IsSync { get; set; }

        /// <summary>
        /// 邮箱地址  可空 length(56)
        /// </summary>
        [MaxLength(56)]
        [Display(Name = "邮箱地址")]
        public string Email { get; set; }

        /// <summary>
        /// 手机号码  不可空 length(11)
        /// </summary>
        [MaxLength(11)]
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        /// <summary>
        /// 身份证明  不可空 length(18)
        /// </summary>
        [MaxLength(18)]
        [Display(Name = "身份证明")]
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
