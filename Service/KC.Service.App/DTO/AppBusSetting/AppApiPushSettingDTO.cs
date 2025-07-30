using KC.Framework.Base;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.App
{
    /// <summary>
    /// 业务接口推送设置
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class AppApiPushSettingDTO : EntityDTO
    {
        public AppApiPushSettingDTO()
        {
            AppApiTargetSettings = new List<AppTargetApiSettingDTO>();
            AppApiPushLogs = new List<AppApiPushLogDTO>();
        }

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
        public AppBusinessSimpleDTO Business { get; set; }

        [DataMember]
        [Display(Name = "业务代码")]
        public string BusinessCode
        {
            get
            {
                return Business?.BusinessCode;
            }
        }

        [DataMember]
        [Display(Name = "业务名称")]
        public string BusinessName
        {
            get
            {
                return Business?.BusinessName;
            }
        }

        [DataMember]
        [Display(Name = "业务类型")]
        public string BusinessTypeString
        {
            get
            {
                return Business?.BusinessTypeStr;
            }
        }

        /// <summary>
        /// 推送目标接口设置
        /// </summary>
        [DataMember]
        public ICollection<AppTargetApiSettingDTO> AppApiTargetSettings { get; set; }
        /// <summary>
        /// 推送日志
        /// </summary>
        [DataMember]
        public ICollection<AppApiPushLogDTO> AppApiPushLogs { get; set; }
    }
}
