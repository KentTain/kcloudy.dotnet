using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Extension;

namespace KC.Service.DTO.Workflow
{
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class DefFieldBaseDTO<T> : TreeNodeDTO<T> where T : EntityBaseDTO
    {
        public DefFieldBaseDTO()
        {
            DataType = AttributeDataType.String;
        }

        /// <summary>
        /// 显示名
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        [DataMember]
        public AttributeDataType DataType { get; set; }
        [DataMember]
        public string DataTypeString { get { return DataType.ToDescription(); } }

        /// <summary>
        /// 值
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Value { get; set; }
        /// <summary>
        /// 值1
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Value1 { get; set; }
        /// <summary>
        /// 值2
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Value2 { get; set; }

        [DataMember]
        public bool CanEdit { get; set; }

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
    }
}
