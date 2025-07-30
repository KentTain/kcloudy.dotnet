using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ProtoBuf;

namespace KC.Service.DTO
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public abstract class EntityDTO : EntityBaseDTO, ICloneable
    {
        protected EntityDTO()
        {
            IsDeleted = false;
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = DateTime.UtcNow;
        }

        /// <summary>
        ///     获取或设置 获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        [DefaultValue(false)]
        [DataMember]
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 创建人Id
        /// </summary>
        [Display(Name = "创建人Id")]
        [DataMember]
        [MaxLength(128)]
        public string CreatedBy { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [Display(Name = "创建人")]
        [DataMember]
        [MaxLength(128)]
        public string CreatedName { get; set; }
        [Display(Name = "创建时间")]
        [DataMember]
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 修改人Id
        /// </summary>
        [Display(Name = "修改人Id")]
        [DataMember]
        [MaxLength(128)]
        public string ModifiedBy { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        [Display(Name = "修改人")]
        [DataMember]
        [MaxLength(128)]
        public string ModifiedName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [Display(Name = "修改时间")]
        [DataMember]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        ///     获取或设置 版本控制标识，用于处理并发
        /// </summary>
        //[ConcurrencyCheck]
        //[Timestamp]
        //public byte[] Timestamp { get; set; }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
