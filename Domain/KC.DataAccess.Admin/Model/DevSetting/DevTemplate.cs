using KC.Enums.Admin;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Admin.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Admin
{
    /// <summary>
    /// 应用开发模板
    /// </summary>
    [Table(Tables.DevTemplate)]
    [Serializable, DataContract(IsReference = true)]
    public class DevTemplate : Entity
    {
        public DevTemplate()
        {
            Type = TemplateType.BackEnd;
        }

        /// <summary>
        /// 模板Id
        /// </summary>
        [Key]
        [DataMember]
        [Display(Name = "模板Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        [DataMember]
        [Display(Name = "模板类型")]
        public TemplateType Type { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [Display(Name = "模板名称")]
        public string Name { get; set; }

        /// <summary>
        /// 模板版本号
        /// </summary>
        [DataMember]
        [MaxLength(32)]
        [Display(Name = "模板版本号")]
        public string Version { get; set; }

        /// <summary>
        /// Git仓库地址
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [Display(Name = "Git仓库地址")]
        public string GitAddress { get; set; }

        /// <summary>
        /// Git是否公开
        /// </summary>
        [DataMember]
        [Display(Name = "Git是否公开")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Git上TagId
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [Display(Name = "Git上TagId")]
        public string GitTagId { get; set; }

        /// <summary>
        /// Git上Tag的哈希Id
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [Display(Name = "Git上Tag的哈希Id")]
        public string GitShaId { get; set; }

        /// <summary>
        /// 是否最新版本
        /// </summary>
        [DataMember]
        [Display(Name = "是否最新版本")]
        public bool IsLatest { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [Display(Name = "描述")]
        public string Description { get; set; }
    }

}
