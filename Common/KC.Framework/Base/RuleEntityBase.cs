using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KC.Framework.Extension;

namespace KC.Framework.Base
{
    /// <summary>
    /// 流程条件的规则
    /// </summary>
    public abstract class RuleEntityBase : EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //数据库生成默认值
        public int Id { get; set; }

        /// <summary>
        /// 规则类型
        /// </summary>
        public RuleType RuleType { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        [MaxLength(128)]
        public string FieldName { get; set; }

        /// <summary>
        /// 字段显示名
        /// </summary>
        [MaxLength(512)]
        public string FieldDisplayName { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public RuleOperatorType OperatorType { get; set; }

        /// <summary>
        /// 字段值
        /// </summary>
        [MaxLength(256)]
        public string FieldValue { get; set; }

        public override string ToString()
        {
            return RuleType.ToDescription() + " " + FieldDisplayName + "【" + FieldName + "】" + OperatorType.ToDescription() + " " + FieldValue;
        }
    }
}
