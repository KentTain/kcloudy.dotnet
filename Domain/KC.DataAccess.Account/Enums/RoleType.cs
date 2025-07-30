using System.ComponentModel;
using System.Runtime.Serialization;

namespace KC.Enums.Account
{
    [DataContract]
    public enum RoleType
    {
        [EnumMember]
        [Description("系统管理员")]
        SystemAdmin = 0,

        [EnumMember]
        [Description("管理员")]
        Admin = 1,

        /// <summary>
        /// 客服人员
        /// </summary>
        [EnumMember]
        [Description("客服人员")]
        ClientService = 5,

        /// <summary>
        /// 客服经理
        /// </summary>
        [EnumMember]
        [Description("客服经理")]
        CustomerServiceManager = 6,


        /// <summary>
        /// 信审人员
        /// </summary>
        [EnumMember]
        [Description("信审人员")]
        CreditLevel = 7,

        /// <summary>
        /// 信审经理
        /// </summary>
        [EnumMember]
        [Description("信审经理")]
        CreditLevelManager = 8,




        /// <summary>
        /// 客户经理
        /// </summary>
        [EnumMember]
        [Description("客户经理")]
        SalesManager = 10,

        /// <summary>
        /// 市场部团队经理
        /// </summary>
        [Description("市场部团队经理")]
        SalesTeamLeader = 11,

        /// <summary>
        /// 产品经理
        /// </summary>
        [EnumMember]
        [Description("产品经理")]
        ProductManager = 15,

        /// <summary>
        /// 产品部团队经理
        /// </summary>
        [EnumMember]
        [Description("产品部团队经理")]
        ProdTeamLeader = 16,

        /// <summary>
        /// 评审委员会
        /// </summary>
        [EnumMember]
        [Description("评审委员会")]
        ReviewCommittee = 17,

        /// <summary>
        /// 银行工作人员
        /// </summary>
        [EnumMember]
        [Description("银行工作人员")]
        BankEmployee = 20,

        /// <summary>
        /// 采购员
        /// </summary>
        [EnumMember]
        [Description("采购员")]
        Purchaser = 25,

        /// <summary>
        /// 采购经理
        /// </summary>
        [EnumMember]
        [Description("采购经理")]
        PurchaseManager = 30,

        /// <summary>
        /// 采购经理
        /// </summary>
        [EnumMember]
        [Description("商城管理员")]
        ShopMananger = 31,

        /// <summary>
        /// 电话客服
        /// </summary>
        [EnumMember]
        [Description("电话客服")]
        PhoneClientService = 32,

        /// <summary>
        /// 产品编辑
        /// </summary>
        [EnumMember]
        [Description("产品编辑")]
        ProductEditor = 33,

        /// <summary>
        /// 转账管理人员
        /// </summary>
        [EnumMember]
        [Description("转账管理人员")]
        TransferAccountsManager = 34,

        /// <summary>
        /// ERP实施
        /// </summary>
        [EnumMember]
        [Description("ERP实施")]
        ERPImplementation = 35
    }
}
