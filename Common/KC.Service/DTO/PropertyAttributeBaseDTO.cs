using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using ProtoBuf;

namespace KC.Service.DTO
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public abstract class PropertyAttributeBaseDTO : EntityDTO
    {
        public PropertyAttributeBaseDTO()
        {
            CanEdit = true;
            DataType = Framework.Base.AttributeDataType.String;
        }

        [DataMember]
        public int PropertyAttributeId { get; set; }

        /// <summary>
        /// 属性数据类型
        /// </summary>
        [DataMember]
        public Framework.Base.AttributeDataType DataType { get; set; }
        [DataMember]
        public string DataTypeString { get { return DataType.ToDescription(); } }

        /// <summary>
        /// 属性值名称
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 属性值显示名称
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
        /// 是否必须
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
