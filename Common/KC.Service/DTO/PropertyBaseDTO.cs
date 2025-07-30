using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KC.Service.DTO
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    //[ProtoInclude(100, typeof(ServiceDTO))]
    //[ProtoInclude(101, typeof(ServiceProviderDTO))]
    public abstract class PropertyBaseDTO<T> : EntityDTO where T : PropertyAttributeBaseDTO, new()
    {
        public PropertyBaseDTO()
        {
            CanEdit = true;
            PropertyAttributeList = new List<T>();
        }

        [DataMember]
        public virtual int PropertyId { get; set; }

        /// <summary>
        /// 名称
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

        [DataMember]
        public List<T> PropertyAttributeList { get; set; }
    }
}
