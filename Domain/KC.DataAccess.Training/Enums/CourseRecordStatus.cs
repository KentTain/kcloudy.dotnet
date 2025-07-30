using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Training
{
    public enum CourseRecordStatus
    {
        /// <summary>
        /// 等待确认
        /// </summary>
        [Description("等待确认")]
        Pending = 0,

        /// <summary>
        /// 扫码确认
        /// </summary>
        [Description("扫码确认")]
        QrConfirm = 1,

        /// <summary>
        /// 手动确认
        /// </summary>
        [Description("手动确认")]
        ManualConfirm = 2,

        /// <summary>
        /// 自动确认
        /// </summary>
        [Description("自动确认")]
        AutoConfirm = 3,
    }
}
