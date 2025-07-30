using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Enums
{
    public enum ApprovalStatus
    {
        /// <summary>
        /// 等待审批
        /// </summary>
        [Description("等待审批")]
        Pending = 0,

        /// <summary>
        /// 审批通过
        /// </summary>
        [Description("审批通过")]
        Through = 1,

        /// <summary>
        /// 未通过
        /// </summary>
        [Description("未通过")]
        Reject = 2
    }
}
