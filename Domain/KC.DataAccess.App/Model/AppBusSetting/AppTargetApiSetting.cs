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
    /// 推送目标接口设置
    /// </summary>
    [Table(Tables.AppTargetApiSetting)]
    [Serializable, DataContract(IsReference = true)]
    public class AppTargetApiSetting : EntityBase
    {
        public AppTargetApiSetting()
        {
            Mappings = new List<PushMapping>();
            SecurityParameters = new List<SecurityParameter>();
        }
        /// <summary>
        /// 推送目标接口设置Id
        /// </summary>
        [Key]
        [DataMember]
        public int TargetSettingId { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        [DataMember]
        public string TargetApiUrl { get; set; }
        /// <summary>
        /// 接口安全类型<br/>
        ///     SecurityKey = 0,<br/>
        ///     HeaderKey = 1,<br/>
        ///     OAuth = 2,<br/>
        ///     None = 99,
        /// </summary>
        [DataMember]
        public SecurityType SecurityType { get; set; }
        /// <summary>
        /// 目标接口接收对象命名空间
        /// </summary>
        [DataMember]
        public string TargetObjectNameSpace { get; set; }
        /// <summary>
        /// 安全处理程序
        /// </summary>
        [DataMember]
        public string SercurityHandlerNameSpace { get; set; }
        [DataMember]
        public Guid PushSettingId { get; set; }
        [DataMember]
        [ForeignKey("PushSettingId")]
        public AppApiPushSetting PushSetting { get; set; }

        /// <summary>
        /// 对象映射列表
        /// </summary>
        [DataMember]
        public ICollection<PushMapping> Mappings { get; set; }

        /// <summary>
        /// 对象映射列表
        /// </summary>
        [DataMember]
        public ICollection<SecurityParameter> SecurityParameters { get; set; }
    }
}
