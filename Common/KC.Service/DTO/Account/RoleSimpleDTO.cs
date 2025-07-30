using System;
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
    public class RoleSimpleDTO : EntityBaseDTO, IEqualityComparer<RoleSimpleDTO>, ICloneable
    {
        public RoleSimpleDTO()
        {
            Users = new List<UserSimpleDTO>();
            MenuNodes = new List<MenuNodeSimpleDTO>();
            Permissions = new List<PermissionSimpleDTO>();
        }

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
        [ProtoMember(5)]
        public bool @checked { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public bool IsSystemRole { get; set; }
        [DataMember]
        [ProtoMember(7)]
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
        [ProtoMember(8)]
        [Display(Name = "创建人")]
        public string CreatedBy { get; set; }
        [DataMember]
        [ProtoMember(9)]
        public DateTime CreatedDate { get; set; }
        [DataMember]
        [ProtoMember(11)]
        public List<UserSimpleDTO> Users { get; set; }
        [DataMember]
        [ProtoMember(13)]
        public List<MenuNodeSimpleDTO> MenuNodes { get; set; }
        [DataMember]
        [ProtoMember(14)]
        public List<PermissionSimpleDTO> Permissions { get; set; }

        public bool Equals(RoleSimpleDTO x, RoleSimpleDTO y)
        {
            return x.RoleId.Equals(y.RoleId, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(RoleSimpleDTO obj)
        {
            return obj.RoleId.GetHashCode();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
