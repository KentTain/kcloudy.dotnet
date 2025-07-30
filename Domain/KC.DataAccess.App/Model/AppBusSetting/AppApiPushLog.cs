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
    /// 应用数据推送日志
    /// </summary>
    [Table(Tables.AppApiPushLog)]
    [Serializable, DataContract(IsReference = true)]
    public class AppApiPushLog : EntityBase
    {
        [Key]
        [DataMember]
        public int LogId { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        [DataMember]
        public ProcessLogType Type { get; set; }

        /// <summary>
        /// 日志生成日期（UTC时间）
        /// </summary>
        [DataMember]
        public System.DateTime OperateDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }
        /// <summary>
        /// 应用推送设置Id
        /// </summary>
        [DataMember]
        public Guid PushSettingId { get; set; }
        [DataMember]
        [ForeignKey("PushSettingId")]
        public AppApiPushSetting PushSetting { get; set; }
        /// <summary>
        /// 推送数据的Json字符串
        /// </summary>
        [DataMember]
        public string PushObjectJson { get; set; }
        /// <summary>
        /// 返回对应的Json字符串
        /// </summary>
        [DataMember]
        public string ReturnObjectJson { get; set; }
    }
}
