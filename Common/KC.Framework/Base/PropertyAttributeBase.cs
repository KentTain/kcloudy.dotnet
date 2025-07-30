using KC.Framework.Extension;
using ProtoBuf;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


namespace KC.Framework.Base
{
    /// <summary>
    /// 属性值基类
    /// </summary>
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public abstract class PropertyAttributeBase : Entity
    {
        public PropertyAttributeBase()
        {
            DataType = AttributeDataType.String;
        }

        /// <summary>
        /// 属性值Id
        /// </summary>
        [Key]
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //数据库生成默认值
        public int PropertyAttributeId { get; set; }

        /// <summary>
        /// 属性数据类型
        /// </summary>
        [DataMember]
        public AttributeDataType DataType { get; set; }

        /// <summary>
        /// 属性值名称
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string DisplayName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        [DataMember]
        public string Value { get; set; }
        /// <summary>
        /// 属性扩展值1
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Ext1 { get; set; }
        /// <summary>
        /// 属性扩展值2
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Ext2 { get; set; }
        /// <summary>
        /// 属性扩展值3
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Ext3 { get; set; }
        /// <summary>
        /// 是否能编辑
        /// </summary>
        [DataMember]
        public bool CanEdit { get; set; }

        /// <summary>
        /// 是否为通用属性值
        /// </summary>
        [DataMember]
        public bool IsRequire { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int Index { get; set; }
    }
}
