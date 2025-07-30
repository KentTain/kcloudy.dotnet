
using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.App
{
    /// <summary>
    /// 推送目标接口设置
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class AppTargetApiSettingDTO : EntityBaseDTO
    {
        public AppTargetApiSettingDTO()
        {
            Mappings = new List<PushMappingDTO>();
            SecurityParameters = new List<SecurityParameterDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        /// <summary>
        /// 推送目标接口设置Id
        /// </summary>
        [DataMember]
        public int TargetSettingId { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        [DataMember]
        public string TargetApiUrl { get; set; }
        /// <summary>
        /// 接口安全类型
        ///     OAuth = 0,
        ///     SecurityKey = 1,
        ///     None = 99,
        /// </summary>
        [DataMember]
        public SecurityType SecurityType { get; set; }

        [DataMember]
        public string SecurityTypeString
        {
            get
            {
                return SecurityType.ToDescription();
            }
        }
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
        public AppApiPushSettingDTO PushSetting { get; set; }

        /// <summary>
        /// 对象映射列表
        /// </summary>
        [DataMember]
        public ICollection<PushMappingDTO> Mappings { get; set; }

        /// <summary>
        /// 对象映射列表
        /// </summary>
        [DataMember]
        public ICollection<SecurityParameterDTO> SecurityParameters { get; set; }
    }
}
