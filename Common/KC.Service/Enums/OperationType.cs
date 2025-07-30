using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Enums
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum  OperationType
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        Create=0,

        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        Edit =1,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete =2
    }
}
