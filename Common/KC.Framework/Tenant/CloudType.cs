using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Framework.Tenant
{
    [DataContract]
    public enum CloudType
    {
        /// <summary>
        /// 本地系统
        /// </summary>
        [EnumMember]
        [Description("本地系统")]
        FileSystem = 0,

        /// <summary>
        /// 微软云
        /// </summary>
        [EnumMember]
        [Description("微软云")]
        Azure = 1,

        /// <summary>
        /// 腾讯云
        /// </summary>
        [EnumMember]
        [Description("腾讯云")]
        TencentCloud = 2,

        /// <summary>
        /// 阿里云
        /// </summary>
        [EnumMember]
        [Description("阿里云")]
        Aliyun = 3,

        /// <summary>
        /// 亚马逊
        /// </summary>
        [EnumMember]
        [Description("亚马逊")]
        AWS = 4,

        /// <summary>
        /// 华为云
        /// </summary>
        [EnumMember]
        [Description("华为云")]
        HuaweiCloud = 5,
    }

    /// <summary>
    /// 检查枚举是否包含某个元素： 
    ///     int iversion = 3; //其值可以是：1--通用版；2--专业版；3--通用版+专业版；4--定制版；5--通用版+定制版；6--专业版+定制版；7--通用版+专业版+定制版
    ///     TenantVersion version = (TenantVersion) Enum.Parse(typeof(TenantVersion), iversion);
    ///     bool hasFlag = ((version & TenantVersion.Professional) != 0);//true
    /// 
    ///     iversion = 4;
    ///     var versions = TenantVersion.Standard | TenantVersion.Professional;
    ///     hasFlag =  ((version & versions) != 0);//false
    /// </summary>
    [Flags]
    [DataContract]
    public enum TenantVersion
    {
        [Description("通用版")]
        [Display(Name = "通用版")]
        [EnumMember]
        Standard = 0x01,

        [Description("专业版")]
        [Display(Name = "专业版")]
        [EnumMember]
        Professional = 0x02,

        [Description("集团版")]
        [Display(Name = "集团版")]
        [EnumMember]
        Group = 0x04,

        [Description("定制版")]
        [Display(Name = "定制版")]
        [EnumMember]
        Customized = 0x08,
    }

    [Flags]
    [DataContract]
    public enum TenantType
    {
        /// <summary>
        /// 企业
        /// </summary>
        [Display(Name = "企业")]
        [Description("企业")]
        [EnumMember]
        Enterprise = 1,
        /// <summary>
        /// 机构
        /// </summary>
        [Display(Name = "机构")]
        [Description("机构")]
        [EnumMember]
        Institution = 2,
        /// <summary>
        /// 银行
        /// </summary>
        [Display(Name = "银行")]
        [Description("银行")]
        [EnumMember]
        Bank = 4,
    }

    [DataContract]
    public enum DomainLevel
    {
        [Description("一级域名")]
        [EnumMember]
        LevelOne = 0,
        [Description("二级域名")]
        [EnumMember]
        LevelTwo = 1,
        [Description("三级域名")]
        [EnumMember]
        LevelThree = 2,
    }

}
