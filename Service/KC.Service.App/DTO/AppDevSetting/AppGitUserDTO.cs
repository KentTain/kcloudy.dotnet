using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.App
{
    /// <summary>
    /// Git用户设置
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class AppGitUserDTO : EntityDTO
    {
        public AppGitUserDTO()
        {
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        /// <summary>
        /// 分支Id
        /// </summary>
        [DataMember]
        [Display(Name = "Git用户设置Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Git用户Id
        /// </summary>
        [DataMember]
        [MaxLength(64)]
        [Display(Name = "Git用户Id")]
        public string UserId { get; set; }

        /// <summary>
        /// 是否使用Token访问
        /// </summary>
        [DataMember]
        [Display(Name = "是否使用Token访问")]
        public bool IsUseToken { get; set; }

        /// <summary>
        /// Git用户账号
        /// </summary>
        [DataMember]
        [MaxLength(64)]
        [Display(Name = "Git用户账号")]
        public string UserAccount { get; set; }

        /// <summary>
        /// Git密码
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        [Display(Name = "Git密码")]
        public string UserPassword { get; set; }

        /// <summary>
        /// GitToken
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [Display(Name = "GitToken")]
        public string UserToken { get; set; }

        /// <summary>
        /// 是否管理员账号
        /// </summary>
        [DataMember]
        [Display(Name = "是否管理员账号")]
        public bool IsAdmin { get; set; }

        /// <summary>
        /// GitId
        /// </summary>
        [DataMember]
        public Guid AppGitId { get; set; }

        [DataMember]
        public string AppGitAddress { get; set; }
    }

}
