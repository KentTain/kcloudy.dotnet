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
    public class ModelDefinitionDTO : PropertyBaseDTO<ModelDefFieldDTO>
    {
        [DataMember]
        public bool IsEditMode { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        [MaxLength(500)]
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// 数据表名称
        /// </summary>
        [MaxLength(200)]
        [DataMember]
        public string TableName { get; set; }

        /// <summary>
        /// 继承类型
        /// </summary>
        [DataMember]
        [Display(Name = "继承类型")]
        public ModelBaseType ModelBaseType { get; set; }

        /// <summary>
        /// 是否记录日志
        /// </summary>
        [DataMember]
        [Display(Name = "是否记录日志")]
        public bool IsUseLog { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public string ApplicationId { get; set; }

    }
}
