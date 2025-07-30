using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.DTO;
using ProtoBuf;

namespace KC.Service.DTO.Account
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class RoleDTO : EntityDTO, IEqualityComparer<RoleDTO>, ICloneable   //TreeNodeDTO<RoleDTO> //
    {
        public RoleDTO()
        {
            BusinessType = BusinessType.None;
            UserIds = new List<string>();
            Users = new List<UserDTO>();
            MenuNodes = new List<MenuNodeDTO>();
            Permissions = new List<PermissionDTO>();
        }

        [ProtoMember(1)]
        public bool IsEditMode { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public string RoleId { get; set; }
        [DataMember]
        [MaxLength(256)]
        [ProtoMember(3)]
        public string RoleName { get; set; }
        [DataMember]
        [MaxLength(256)]
        [ProtoMember(4)]
        public string DisplayName { get; set; }
        [DataMember]
        [MaxLength(256)]
        [ProtoMember(5)]
        public string Description { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public bool @checked { get; set; }
        [DataMember]
        [ProtoMember(7)]
        public bool IsSystemRole { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        [DataMember]
        [ProtoMember(8)]
        public BusinessType BusinessType { get; set; }

        [DataMember]
        public string BusinessTypeString
        {
            get
            {
                return BusinessType.ToDescription();
            }
        }
        [DataMember]
        [ProtoMember(10)]
        public List<string> UserIds { get; set; }
        [DataMember]
        [ProtoMember(11)]
        public List<UserDTO> Users { get; set; }
        [DataMember]
        [ProtoMember(12)]
        public List<MenuNodeDTO> MenuNodes { get; set; }
        [DataMember]
        [ProtoMember(13)]
        public List<PermissionDTO> Permissions { get; set; }

        public bool Equals(RoleDTO x, RoleDTO y)
        {
            return x.RoleId.Equals(y.RoleId, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(RoleDTO obj)
        {
            return obj.RoleId.GetHashCode();
        }

        public override object Clone()
        {
            return this.MemberwiseClone();  
        }
    }
}
