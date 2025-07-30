using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Training
{
    public enum AccountStatus
    {
        /// <summary>
        /// 未使用
        /// </summary>
        [Description("未使用")]
        UnUse = 0,

        /// <summary>
        /// 使用中
        /// </summary>
        [Description("使用中")]
        Using = 1,

        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        Disable = 2,

    }
}
