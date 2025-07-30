using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Enums.Admin
{
    public enum WorkOrderStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        Draft = 0,
        /// <summary>
        /// 提交
        /// </summary>
        [Description("提交")]
        Submit = 1,
        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        Audit = 2,
        /// <summary>
        /// 已解决
        /// </summary>
        [Description("已解决")]
        Completed = 3,
    }

    public enum WorkOrderType
    {
        /// <summary>
        /// 系统问题
        /// </summary>
        [Description("系统问题")]
        SystemProblems = 101,
        /// <summary>
        /// 系统建议
        /// </summary>
        [Description("系统建议")]
        SystemAdvice = 102,
    }
}
