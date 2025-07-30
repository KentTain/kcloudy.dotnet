using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Framework.Tenant
{
    /// <summary>
    /// 业务类型
    /// </summary>
    [DataContract]
    public enum BusinessType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Display(Name = "未知")]
        [Description("未知")]
        [EnumMember]
        None = 0,

        /// <summary>
        /// 信息化
        /// </summary>
        [Display(Name = "信息化")]
        [Description("信息化")]
        [EnumMember]
        IT = 1,
        /// <summary>
        /// 人事
        /// </summary>
        [Display(Name = "人事")]
        [Description("人事")]
        [EnumMember]
        HR = 2,
        /// <summary>
        /// 销售
        /// </summary>
        [Display(Name = "销售")]
        [Description("销售")]
        [EnumMember]
        Sale = 4,
        /// <summary>
        /// 采购
        /// </summary>
        [Display(Name = "采购")]
        [Description("采购")]
        [EnumMember]
        Purchase = 8,
        /// <summary>
        /// 仓储
        /// </summary>
        [Display(Name = "仓储")]
        [Description("仓储")]
        [EnumMember]
        WareHouse = 16,
        /// <summary>
        /// 生产
        /// </summary>
        [Display(Name = "生产")]
        [Description("生产")]
        [EnumMember]
        Manufacturing = 32,
        /// <summary>
        /// 质检
        /// </summary>
        [Display(Name = "质检")]
        [Description("质检")]
        [EnumMember]
        QA = 64,
        /// <summary>
        /// 市场
        /// </summary>
        [Display(Name = "市场")]
        [Description("市场")]
        [EnumMember]
        Marketing = 128,
        /// <summary>
        /// 运营
        /// </summary>
        [Display(Name = "运营")]
        [Description("运营")]
        [EnumMember]
        Operation = 256,
        /// <summary>
        /// 财务
        /// </summary>
        [Display(Name = "财务")]
        [Description("财务")]
        [EnumMember]
        Accounting = 512,
        /// <summary>
        /// 设计
        /// </summary>
        [Display(Name = "设计")]
        [Description("设计")]
        [EnumMember]
        Designer = 1024,
        /// <summary>
        /// 行政
        /// </summary>
        [Display(Name = "行政")]
        [Description("行政")]
        [EnumMember]
        Executor = 2048,
    }

    
}
