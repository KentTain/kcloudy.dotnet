using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.CodeGenerate;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.CodeGenerate.Constants;

namespace KC.Model.CodeGenerate
{
    [Table(Tables.ModelDefField)]
    public class ModelDefField : PropertyAttributeBase
    {
        /// <summary>
        /// 显示名
        /// </summary>
        [DataMember]
        [MaxLength(100)]
        public string DisplayName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        [MaxLength(4000)]
        public string Description { get; set; }
        /// <summary>
        /// 是否为主键字段
        /// </summary>
        [DataMember]
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否为主键字段
        /// </summary>
        [DataMember]
        public PrimaryKeyType? PrimaryKeyType { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        [DataMember]
        public bool IsNotNull { get; set; }
        /// <summary>
        /// 是否唯一
        /// </summary>
        [DataMember]
        public bool IsUnique { get; set; }
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

        /// <summary>
        /// 关联对象Id
        /// </summary>
        [DataMember]
        [MaxLength(64)]
        public string RelateObjectId { get; set; }

        /// <summary>
        /// 关联对象属性Id
        /// </summary>
        [DataMember]
        [MaxLength(64)]
        public string RelateObjFieldId { get; set; }

        [DataMember]
        public int ModelDefId { get; set; }
        [DataMember]
        [ForeignKey("ModelDefId")]
        public ModelDefinition ModelDefinition { get; set; }
    }
}
