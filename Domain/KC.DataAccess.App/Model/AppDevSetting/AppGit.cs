using KC.Enums.App;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.App.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.App
{
    /// <summary>
    /// 应用Git仓库
    /// </summary>
    [Table(Tables.AppGit)]
    [Serializable, DataContract(IsReference = true)]
    public class AppGit : Entity
    {
        public AppGit()
        {
            GitBranches = new List<AppGitBranch>();
            GitUsers = new List<AppGitUser>();
        }

        /// <summary>
        /// 仓库Id
        /// </summary>
        [Key]
        [DataMember]
        [Display(Name = "仓库Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 仓库地址
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [Display(Name = "仓库地址")]
        public string GitAddress { get; set; }

        /// <summary>
        /// 主开发分支
        /// </summary>
        [DataMember]
        [MaxLength(64)]
        [Display(Name = "主开发分支")]
        public string GitMainBranch { get; set; }

        /// <summary>
        /// 仓库Token
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [Display(Name = "仓库Token")]
        public string GitToken { get; set; }

        /// <summary>
        /// 是否系统分配
        /// </summary>
        [DataMember]
        [Display(Name = "是否系统分配")]
        public bool IsSystem { get; set; }

        /// <summary>
        /// 是否使用
        /// </summary>
        [DataMember]
        [Display(Name = "是否使用")]
        public bool IsActived { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [Display(Name = "描述")]
        public string Description { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public Guid ApplicationId { get; set; }

        [DataMember]
        [ForeignKey("ApplicationId")]
        public Application Application { get; set; }

        /// <summary>
        /// 分支列表
        /// </summary>
        [DataMember]
        public List<AppGitBranch> GitBranches { get; set; }

        /// <summary>
        /// 用户列表
        /// </summary>
        [DataMember]
        public List<AppGitUser> GitUsers { get; set; }
    }

}
