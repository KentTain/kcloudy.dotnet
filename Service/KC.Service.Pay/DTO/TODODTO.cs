using KC.Enums.Pay;
using KC.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    /// <summary>
    /// 待办事项
    /// </summary>
    public class TODODTO
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public TODOType Type { get; set; }

        public string TypeName { get { return Type.ToDescription(); } }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime DateTime { get; set; }

        public string DateTimeStr
        {
            get
            {
                return DateTime.AddHours(8).ToString("yyyy-MM-dd HH:mm");
            }
        }

        /// <summary>
        /// 付款单对应的主键id
        /// </summary>
        public string RelevantId { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 来源，付款时使用
        /// </summary>
        public string SourceName
        {
            get
            {
                if (!Source.HasValue)
                    return string.Empty;
                return Source.Value.ToDescription();
            }
        }

        /// <summary>
        /// 来源，付款时使用
        /// </summary>
        public PayableSource? Source { get; set; }
    }
}
