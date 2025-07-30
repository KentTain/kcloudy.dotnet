using KC.Framework.Base;
using KC.Model.Account.Constants;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Account
{
    [Table(Tables.SystemSetting)]
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class SystemSetting : PropertyBase<SystemSettingProperty>
    {
        /// <summary>
        /// 设置系统编号（SequenceName--SystemSetting：SSC2018120100001）
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "设置编号")]
        [ProtoMember(1)]
        public string Code { get; set; }

        /// <summary>
        /// 应用程序Id
        /// </summary>
        [DataMember]
        [Display(Name = "应用程序Id")]
        public Guid? ApplicationId { get; set; }
        /// <summary>
        /// 应用程序名称
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [Display(Name = "应用程序名称")]
        public string ApplicationName { get; set; }
    }
}
