using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using KC.Framework.Tenant;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.App
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ApplicationDTO : EntityDTO
    {
        public ApplicationDTO()
        {
            AppSettings = new List<AppSettingDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

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
        /// 站点名称
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [Display(Name = "站点名称")]
        public string WebSiteName { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        [Display(Name = "域名")]
        public string DomainName { get; set; }
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

        [DataMember]
        [Display(Name = "小图标")]
        public string IconCls
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(SmallIcon))
                    return SmallIcon;

                return "ic_item";
            }
            set
            {
                SmallIcon = value;
            }
        }

        /// <summary>
        /// 大图标
        /// </summary>
        [DataMember]
        [MaxLength(256)]
        [Display(Name = "大图标")]
        public string BigIcon { get; set; }

        [DataMember]
        [Display(Name = "大图标")]
        public string BigIconUrl
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(BigIcon))
                    return BigIcon;

                return "/Content/themes/icons/large_picture.png";
            }

            set
            {
                BigIcon = value;
            }
        }
        [DataMember]
        public bool IsEnabledWorkFlow { get; set; }
        [DataMember]
        [MaxLength(512)]
        [Display(Name = "描述")]
        public string Description { get; set; }
        [DataMember]
        public int Index { get; set; }

        public Guid? AppTemplateId { get; set; }
        /// <summary>
        /// 应用模板
        /// </summary>
        [DataMember]
        public DevTemplateDTO AppTemplate { get; set; }

        /// <summary>
        /// 应用设置列表
        /// </summary>
        [DataMember]
        public List<AppSettingDTO> AppSettings { get; set; }

    }

    [Serializable, DataContract(IsReference = true)]
    public class OpenApplicatin
    {
        public OpenApplicatin(Guid appId, string domainName)
        {
            ApplicationId = appId;
            DomainName = domainName;
        }
        [DataMember]
        public Guid ApplicationId { get; set; }
        [DataMember]
        public string DomainName { get; set; }
    }
}
