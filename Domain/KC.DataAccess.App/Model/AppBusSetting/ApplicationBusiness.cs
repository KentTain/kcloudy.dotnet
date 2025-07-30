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
    /// 应用所属业务节点
    /// </summary>
    [Table(Tables.ApplicationBusiness)]
    [Serializable, DataContract(IsReference = true)]
    public class ApplicationBusiness : Entity
    {
        public ApplicationBusiness()
        {
            BusinessType = BusinessType.None;
        }
        /// <summary>
        /// 业务Id
        /// </summary>
        [Key]
        [DataMember]
        [Display(Name = "业务Id")]
        public Guid BusinessId { get; set; }

        /// <summary>
        /// 业务代码（SequenceName--BusinessType：BUT2018120100001）
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [Display(Name = "业务代码")]
        public string BusinessCode { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [DataMember]
        [Display(Name = "业务类型")]
        public BusinessType BusinessType { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [Display(Name = "业务名称")]
        public string BusinessName { get; set; }

        [DataMember]
        [Display(Name = "是否有推送设置")]
        public bool HasPushSetting { get; set; }

        /// <summary>
        /// 业务描述
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [Display(Name = "业务描述")]
        public string Description { get; set; }
        [DataMember]
        public Guid ApplicationId { get; set; }
        [DataMember]
        [ForeignKey("ApplicationId")]
        public Application Application { get; set; }

        [DataMember]
        [Display(Name = "推送设置Id")]
        public Guid? PushSettingId { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        [DataMember]
        [ForeignKey("PushSettingId")]
        public AppApiPushSetting PushSetting { get; set; }
    }

    public interface ISercurityHandler
    {
        void Handler(params object[] inputParms);
    }
}
