using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Enums
{
    public enum DisplayPosition
    {
        /// <summary>
        /// 上中
        /// </summary>
        [Description("上中")]
        TopCenter = 0,

        /// <summary>
        /// 中左
        /// </summary>
        [Description("中左")]
        MiddleLeft = 1,

        /// <summary>
        /// 中右
        /// </summary>
        [Description("中右")]
        MiddleRight = 2,

        /// <summary>
        /// 正中
        /// </summary>
        [Description("正中")]
        MiddleCenter = 3,

        /// <summary>
        /// 下中
        /// </summary>
        [Description("下中")]
        BottomCenter = 4,

        /// <summary>
        /// 上左
        /// </summary>
        [Description("上左")]
        TopLeft = 5,

        /// <summary>
        /// 上右
        /// </summary>
        [Description("上右")]
        TopRight = 6
    }
}
