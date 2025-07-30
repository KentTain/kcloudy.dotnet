using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Workflow.Constants;

namespace KC.Model.Workflow
{
    [Table(Tables.ModelDefField)]
    public class ModelDefField : PropertyAttributeBase
    {
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否为主键字段
        /// </summary>
        [DataMember]
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否为执行人字段
        /// </summary>
        [DataMember]
        public bool IsExecutor { get; set; }

        /// <summary>
        /// 是否为条件判断字段
        /// </summary>
        [DataMember]
        public bool IsCondition { get; set; }

        public int ModelDefId { get; set; }
        [ForeignKey("ModelDefId")]
        public virtual ModelDefinition ModelDefinition { get; set; }
    }
}
