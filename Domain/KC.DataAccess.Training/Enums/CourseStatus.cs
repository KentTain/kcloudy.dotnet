using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Training
{
    public enum CourseStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        Draft = 0,

        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        Approved = 1,

        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        Disable = 2,

    }
}
