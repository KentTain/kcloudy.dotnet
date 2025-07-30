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
using ProtoBuf;

namespace KC.Service.DTO.CodeGenerate
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class RelationDefDetailDTO : EntityBaseDTO
    {
        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 关系类型
        /// </summary>
        [DataMember]
        [Display(Name = "关系类型")]
        public RelationType RelationType { get; set; }

        /// <summary>
        /// 属性名
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        [Display(Name = "属性名")]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [DataMember]
        [MaxLength(200)]
        [Display(Name = "显示名称")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 主表Id
        /// </summary>
        [DataMember]
        [Display(Name = "主表Id")]
        public int MasterId { get; set; }

        /// <summary>
        /// 主表属性Id
        /// </summary>
        [DataMember]
        [Display(Name = "主表属性Id")]
        public int MasterFieldId { get; set; }

        /// <summary>
        /// 子表Id
        /// </summary>
        [DataMember]
        [Display(Name = "子表Id")]
        public int SlaveId { get; set; }

        /// <summary>
        /// 子表属性Id
        /// </summary>
        [DataMember]
        [Display(Name = "子表属性Id")]
        public int SlaveFieldId { get; set; }

        /// <summary>
        /// 关系模型Id
        /// </summary>
        [DataMember]
        public int RelationDefId { get; set; }

        [DataMember]
        public string RelationDefinitionName { get; set; }
    }
}
