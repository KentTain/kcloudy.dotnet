using System;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace KC.Service.Component.DTO
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class StorageEntityDTO : StorageEntityBaseDTO
    {
        protected StorageEntityDTO()
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

        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public string CreatedName { get; set; }
        [DataMember]
        public DateTime CreatedDate { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public string ModifiedName { get; set; }
        [DataMember]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        ///     获取或设置 版本控制标识，用于处理并发
        /// </summary>
        //[ConcurrencyCheck]
        //[Timestamp]
        //public byte[] Timestamp { get; set; }
    }
}
