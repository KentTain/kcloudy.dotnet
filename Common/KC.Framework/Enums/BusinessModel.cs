using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Framework.Base
{
    /// <summary>
    /// 企业经营模式
    /// </summary>
    [DataContract]
    public enum BusinessModel
    {
        /// <summary>
        /// 生产代工型
        /// </summary>
        [Description("生产代工型")]
        [EnumMember]
        ProductionFoundry = 1,

        /// <summary>
        /// 设计和销售型
        /// </summary>
        [Description("设计和销售型")]
        [EnumMember]
        DesignAndSales = 2,

        /// <summary>
        /// 生产和销售型
        /// </summary>
        [Description("生产和销售型")]
        [EnumMember]
        ProductionAdnSales = 4,

        /// <summary>
        /// 设计、生产和销售型
        /// </summary>
        [Description("设计、生产和销售型")]
        [EnumMember]
        DesignAndProductionAndSales = 8,

        /// <summary>
        /// 信息服务型
        /// </summary>
        [Description("信息服务型")]
        [EnumMember]
        InformationService = 16,

        /// <summary>
        /// 供应链金融
        /// </summary>
        [Description("供应链金融")]
        [EnumMember]
        SupplyChainFinance = 32,

        /// <summary>
        /// 商业保理
        /// </summary>
        [Description("商业保理")]
        [EnumMember]
        CommercialFactoring = 64,

        /// <summary>
        /// 融资租赁
        /// </summary>
        [Description("融资租赁")]
        [EnumMember]
        FinanceLease = 128,

        /// <summary>
        /// 小额贷款
        /// </summary>
        [Description("小额贷款")]
        [EnumMember]
        SmallLoan = 256,
    }
}
