using KC.Framework.Base;
using KC.Service.DTO;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.App
{
    /// <summary>
    /// 应用数据推送日志
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class AppApiPushLogDTO : EntityBaseDTO
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
        public AppApiPushSettingDTO PushSetting { get; set; }
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
