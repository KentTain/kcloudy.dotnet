using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Contract
{
    public enum ContractStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        [Display(Name = "审核合同")]
        WaitforSubmited = 0,
        /// <summary>
        /// 待签署
        /// </summary>
        [Display(Name = "签署合同")]
        [Description("待签署")]
        WaitSign = 1,

        [Display(Name = "合同生效")]
        [Description("已生效")]
        Complete = 2,
        [Display(Name = "已退回")]
        [Description("已退回")]
        Returned = 3,
        [Display(Name = "作废合同")]
        [Description("待作废")]
        WaitCancel = 4,
        [Display(Name = "作废合同")]
        [Description("已作废")]
        Abandoned = 5

    }

    public enum ContractType
    {
        /// <summary>
        /// 通用合同
        /// </summary>
        [Display(Name = "通用合同")]
        [Description("通用合同")]
        Electronic = 0,

        /// <summary>
        /// 对账单合同
        /// </summary>
        [Description("对账单合同")]
        AccountStatement = 1,

        /// <summary>
        /// 销售合同
        /// </summary>
        [Description("销售合同")]
        Seller = 3,
        /// <summary>
        /// 采购合同
        /// </summary>
        [Description("采购合同")]
        Purchase = 4,

        /// <summary>
        /// 协议合同
        /// </summary>
        [Description("协议合同")]
        Agreement = 5,
        /// <summary>
        /// 放款合同
        /// </summary>
        [Description("放款合同")]
        Lending=6
    }
}
