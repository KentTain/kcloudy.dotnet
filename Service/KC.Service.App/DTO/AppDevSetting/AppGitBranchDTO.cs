using KC.Enums.App;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.App
{
    /// <summary>
    /// 应用Git仓库
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class AppGitBranchDTO : EntityDTO
    {
        public AppGitBranchDTO()
        {
            Type = BranchType.Private;
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        /// <summary>
        /// 分支Id
        /// </summary>
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
        public string AppGitAddress { get; set; }
    }

}
