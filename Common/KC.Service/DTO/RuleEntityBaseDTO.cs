using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KC.Framework.Base;
using ProtoBuf;
using KC.Framework.Extension;

namespace KC.Service.DTO
{
    /// <summary>
    /// 流程条件的规则
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public abstract class RuleEntityBaseDTO : EntityBaseDTO
    {
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 规则类型
        /// </summary>
        [DataMember]
        public RuleType RuleType { get; set; }
        [DataMember]
        public string RuleTypeString { get { return RuleType.ToDescription(); } }

        /// <summary>
        /// 字段名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string FieldName { get; set; }
        /// <summary>
        /// 字段显示名
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string FieldDisplayName { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [DataMember]
        public RuleOperatorType OperatorType { get; set; }
        [DataMember]
        public string OperatorTypeString { get { return OperatorType.ToDescription(); } }

        /// <summary>
        /// 字段值
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string FieldValue { get; set; }

        public override string ToString()
        {
            return RuleType.ToDescription() + " " + FieldDisplayName + "【" + FieldName + "】" + OperatorType.ToDescription() + " " + FieldValue;
        }
    }
}
