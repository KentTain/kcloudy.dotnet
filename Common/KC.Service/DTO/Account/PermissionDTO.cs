using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using KC.Framework.Base;
using ProtoBuf;

namespace KC.Service.DTO.Account
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class PermissionDTO : TreeNodeDTO<PermissionDTO>, IEqualityComparer<PermissionDTO>, ICloneable
    {
        public PermissionDTO()
        {
            Id = 0;
            ParentId = null;
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        [MaxLength(128)]
        [ProtoMember(3)]
        public string AreaName { get; set; } 

        [DataMember]
        [MaxLength(128)]
        [Required]
        [ProtoMember(4)]
        public string ActionName { get; set; }

        [DataMember]
        [MaxLength(128)]
        [Required]
        [ProtoMember(5)]
        public string ControllerName { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public string Parameters { get; set; }

        [DataMember]
        [ProtoMember(7)]
        public ResultType ResultType { get; set; }

        [DataMember]
        [MaxLength(512)]
        [ProtoMember(8)]
        public string Description { get; set; }

        [DataMember]
        [ProtoMember(9)]
        public Guid ApplicationId { get; set; }

        [DataMember]
        [ProtoMember(11)]
        public string ApplicationName { get; set; }

        //[DataMember]
        //[ProtoMember(14)]
        public List<RoleDTO> Role { get; set; }

        [DataMember]
        [ProtoMember(15)]
        public PermissionDTO Parent { get; set; }

        [DataMember]
        [ProtoMember(16)]
        public string DefaultRoleId { get; set; }

        /// <summary>
        /// 菜单的权限控制Id
        /// </summary>
        [DataMember]
        [ProtoMember(17)]
        public string AuthorityId { get; set; }

        public bool Equals(PermissionDTO x, PermissionDTO y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(PermissionDTO obj)
        {
            return obj.Id.GetHashCode();
        }

        public override object Clone()
        {
            base.MemberwiseClone();
            return this.MemberwiseClone();
        }
    }
}
