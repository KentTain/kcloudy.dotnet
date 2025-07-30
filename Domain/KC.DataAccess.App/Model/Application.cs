using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.App.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace KC.Model.App
{
    /// <summary>
    /// 应用
    /// </summary>
    [Table(Tables.Application)]
    [Serializable, DataContract(IsReference = true)]
    public class Application : Entity
    {
        public Application()
        {
            AppGits = new List<AppGit>();
        }
        /// <summary>
        /// 应用程序Id
        /// </summary>
        [Key]
        [DataMember]
        [Display(Name = "应用程序Id")]
        public Guid ApplicationId { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        [DataMember]
        [Display(Name = "版本")]
        public TenantVersion Version { get; set; }
        /// <summary>
        /// 应用程序编码
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [Display(Name = "应用程序编码")]
        public string ApplicationCode { get; set; }
        /// <summary>
        /// 应用程序名称
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [Display(Name = "应用程序名称")]
        public string ApplicationName { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        [Display(Name = "域名")]
        public string DomainName { get; set; }
        /// <summary>
        /// 站点名称
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [Display(Name = "站点名称")]
        public string WebSiteName { get; set; }
        /// <summary>
        /// 模块涉及程序集
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [Display(Name = "模块涉及程序集")]
        public string AssemblyName { get; set; }
        /// <summary>
        /// 小图标
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        [Display(Name = "小图标")]
        public string SmallIcon { get; set; }
        /// <summary>
        /// 大图标
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        [Display(Name = "大图标")]
        public string BigIcon { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [Display(Name = "描述")]
        public string Description { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        [Display(Name = "排序")]
        public int Index { get; set; }
        /// <summary>
        /// 是否开通工作流
        /// </summary>
        [DataMember]
        [Display(Name = "是否开通工作流")]
        public bool IsEnabledWorkFlow { get; set; }

        public Guid? AppTemplateId { get; set; }

        /// <summary>
        /// 应用模板
        /// </summary>
        [DataMember]
        [ForeignKey("AppTemplateId")]
        public DevTemplate AppTemplate { get; set; }

        /// <summary>
        /// Git列表
        /// </summary>
        [DataMember]
        public List<AppGit> AppGits { get; set; }

        /// <summary>
        /// 应用设置列表
        /// </summary>
        [DataMember]
        public List<AppSetting> AppSettings { get; set; }
    }
}
