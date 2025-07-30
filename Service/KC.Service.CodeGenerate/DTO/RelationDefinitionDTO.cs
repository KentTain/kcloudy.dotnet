using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.CodeGenerate.Constants;
using ProtoBuf;

namespace KC.Service.DTO.CodeGenerate
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class RelationDefinitionDTO : EntityDTO
    {
        public RelationDefinitionDTO()
        {
            DefDetails = new List<RelationDefDetailDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        [Display(Name = "类名")]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [DataMember]
        [MaxLength(200)]
        [Display(Name = "显示名称")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember]
        public string ApplicationId { get; set; }

        [DataMember]
        public virtual ICollection<RelationDefDetailDTO> DefDetails { get; set; }
    }
}
