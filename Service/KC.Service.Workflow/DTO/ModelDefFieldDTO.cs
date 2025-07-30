using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.Workflow.DTO
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ModelDefFieldDTO : PropertyAttributeBaseDTO
    {
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

        [DataMember]
        public int ModelDefId { get; set; }
        [DataMember]
        public virtual ModelDefinitionDTO ModelDefinition { get; set; }
    }
}
