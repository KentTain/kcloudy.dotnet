using KC.Framework.Base;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.DTO;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.App
{
    /// <summary>
    /// 应用所属业务节点
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class AppBusinessSimpleDTO : EntityDTO
    {
        public AppBusinessSimpleDTO()
        {
            BusinessType = BusinessType.None;
        }

        /// <summary>
        /// 业务Id
        /// </summary>
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

        [DataMember]
        public string BusinessTypeStr
        {
            get { return BusinessType.ToDescription(); }
        }

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
    }
}
