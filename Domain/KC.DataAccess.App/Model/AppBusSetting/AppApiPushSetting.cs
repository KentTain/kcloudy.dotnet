using KC.Framework.Base;
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
    /// 业务接口推送设置
    /// </summary>
    [Table(Tables.AppApiPushSetting)]
    [Serializable, DataContract(IsReference = true)]
    public class AppApiPushSetting : Entity
    {
        public AppApiPushSetting()
        {
            AppApiTargetSettings = new List<AppTargetApiSetting>();
            AppApiPushLogs = new List<AppApiPushLog>();
        }

        [Key]
        [DataMember]
        public Guid PushSettingId { get; set; }
        /// <summary>
        /// 推送对象命名空间
        /// </summary>
        [DataMember]
        public string PushObjectNameSpace { get; set; }
        /// <summary>
        /// 业务类型Id
        /// </summary>
        [DataMember]
        [Display(Name = "业务Id")]
        public Guid BusinessId { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        [DataMember]
        [ForeignKey("BusinessId")]
        public ApplicationBusiness Business { get; set; }

        /// <summary>
        /// 推送目标接口设置
        /// </summary>
        [DataMember]
        public ICollection<AppTargetApiSetting> AppApiTargetSettings { get; set; }
        /// <summary>
        /// 推送日志
        /// </summary>
        [DataMember]
        public ICollection<AppApiPushLog> AppApiPushLogs { get; set; }
    }
}
