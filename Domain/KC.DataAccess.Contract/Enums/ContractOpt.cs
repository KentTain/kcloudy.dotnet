using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Enums.Contract
{
    /// <summary>
    /// 
    /// </summary>
    public enum ContractOpt
    {
        /// <summary>
        /// 首次审批
        /// </summary>
        FirstReview,

        /// <summary>
        /// 审批
        /// </summary>
        Review,
        /// <summary>
        /// 退回
        /// </summary>
        Withdraw,

        /// <summary>
        /// 签署
        /// </summary>
        Sign,

        ///// <summary>
        ///// 个人签署
        ///// </summary>
        //PersonalSign,

        /// <summary>
        /// 删除
        /// </summary>
        Delete,

        /// <summary>
        /// 废除
        /// </summary>
        Discard,

        /// <summary>
        /// 最后一位废除
        /// </summary>
        LastDiscard,
    }
}
