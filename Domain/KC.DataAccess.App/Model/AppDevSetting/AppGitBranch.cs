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
    [Table(Tables.AppGitBranch)]
    [Serializable, DataContract(IsReference = true)]
    public class AppGitBranch : Entity
    {
        public AppGitBranch()
        {
            Type = BranchType.Private;
        }

        /// <summary>
        /// 分支Id
        /// </summary>
        [Key]
        [DataMember]
        [Display(Name = "分支Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 分支类型
        /// </summary>
        [DataMember]
        [Display(Name = "分支类型")]
        public BranchType Type { get; set; }

        /// <summary>
        /// 分支名称
        /// </summary>
        [DataMember]
        [MaxLength(64)]
        [Display(Name = "分支名称")]
        public string Name { get; set; }

        /// <summary>
        /// GitId
        /// </summary>
        [DataMember]
        public Guid AppGitId { get; set; }

        [DataMember]
        [ForeignKey("AppGitId")]
        public AppGit AppGit { get; set; }
    }

}
