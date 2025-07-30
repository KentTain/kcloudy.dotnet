using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace KC.Framework.Base
{
    /// <summary>
    /// 属性设置基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class PropertyBase<T> : Entity where T : PropertyAttributeBase 
    {
        public PropertyBase()
        {
            CanEdit = true;
            PropertyAttributeList = new List<T>();
        }

        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //数据库生成默认值
        public int PropertyId { get; set; }

        /// <summary>
        /// 名称
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
        /// 是否能编辑
        /// </summary>
        [DefaultValue(true)]
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

        /// <summary>
        /// 属性值列表
        /// </summary>
        [DataMember]
        public List<T> PropertyAttributeList { get; set; }
    }
}
