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
    //[ProtoInclude(100, typeof(OrganizationSimpleDTO))]
    //[ProtoInclude(101, typeof (PermissionSimpleDTO))]
    //[ProtoInclude(102, typeof (MenuNodeSimpleDTO))]
    public abstract class TreeNodeSimpleDTO<T> : EntityBaseDTO where T : EntityBaseDTO
    {
        public TreeNodeSimpleDTO()
        {
            this.Children = new List<T>();
        }

        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [DisplayName("标题")]
        public string Text { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        [DataMember]
        [DisplayName("父节点")]
        public int? ParentId { get; set; }

        /// <summary>
        /// 标识树形结构的编码
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [DisplayName("标识编码")]
        public string TreeCode { get; set; }

        /// <summary>
        /// 是否叶节点
        /// </summary>
        [DataMember]
        public bool Leaf { get; set; }

        /// <summary>
        /// 节点深度
        /// </summary>
        [Required]
        [DataMember]
        public int Level { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        [Required]
        public int Index { get; set; }

        [DataMember]
        public List<T> Children { get; set; }

        private bool _checked { get; set; }
        
        [DataMember]
        public bool @checked {
            get
            {
                return _checked;
            }
            set { _checked = value; } }
    }

    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    //[ProtoInclude(100, typeof (OrganizationDTO))]
    //[ProtoInclude(101, typeof (PermissionDTO))]
    //[ProtoInclude(102, typeof (MenuNodeDTO))]
    //[ProtoInclude(103, typeof (IndustryClassficationDTO))]
    //[ProtoInclude(104, typeof (CategoryDTO))]
    //[ProtoInclude(105, typeof (ArticleCategoryDTO))]
    //[ProtoInclude(106, typeof (FlowListsDto))]
    //[ProtoInclude(107, typeof (FlowTypesDto))]
    public abstract class TreeNodeDTO<T> : EntityDTO, ICloneable where T : EntityBaseDTO
    {
        public TreeNodeDTO()
        {
            this.Children = new List<T>();
        }

        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [DisplayName("标题")]
        public string Text { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        [DataMember]
        [DisplayName("父节点")]
        public int? ParentId { get; set; }

        /// <summary>
        /// 标识树形结构的编码:
        /// 一级树节点Id-二级树节点Id-三级树节点Id-四级树节点Id
        /// 1-1-1-1 ~~ 999-999-999-999
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        [DisplayName("标识编码")]
        public string TreeCode { get; set; }

        /// <summary>
        /// 是否叶节点
        /// </summary>
        [DataMember]
        public bool Leaf { get; set; }

        /// <summary>
        /// 节点深度
        /// </summary>
        [Required]
        [DataMember]
        public int Level { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        [Required]
        public int Index { get; set; }

        [DataMember]
        public List<T> Children { get; set; }

        [DataMember]
        public bool @checked { get; set; }

        public override object Clone()
        {
            base.MemberwiseClone();
            return this.MemberwiseClone();
        }
    }

    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class TreeSimpleDTO
    {
        public TreeSimpleDTO()
        {
            this.Children = new List<TreeSimpleDTO>();
        }

        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [DisplayName("标题")]
        public string Text { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        [DataMember]
        [DisplayName("父节点")]
        public int? ParentId { get; set; }

        /// <summary>
        /// 标识树形结构的编码
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        [DisplayName("标识编码")]
        public string TreeCode { get; set; }

        /// <summary>
        /// 节点深度
        /// </summary>
        [Required]
        [DataMember]
        public int Level { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        [MaxLength(512)]
        public string Description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        [Required]
        public int Index { get; set; }

        [DataMember]
        public bool @checked { get; set; }

        [DataMember]
        public Guid ApplicationId { get; set; }

        [DataMember]
        public List<TreeSimpleDTO> Children { get; set; }
    }
}
