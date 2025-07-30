using System;
using System.Collections.Generic;
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
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class PermissionSimpleDTO : TreeNodeSimpleDTO<PermissionSimpleDTO>, IEqualityComparer<PermissionSimpleDTO>, ICloneable
    {
        public PermissionSimpleDTO()
        {
        }

        [DataMember]
        [ProtoMember(1)]
        public string AreaName { get; set; } 
        [DataMember]
        [ProtoMember(2)]
        public string ActionName { get; set; }
        [DataMember]
        [ProtoMember(3)]
        public string ControllerName { get; set; }
        [DataMember]
        [ProtoMember(4)]
        public string Parameters { get; set; }
        [DataMember]
        [ProtoMember(5)]
        public ResultType ResultType { get; set; }

        [DataMember]
        [ProtoMember(6)]
        public string Description { get; set; }

        

        [DataMember]
        [ProtoMember(8)]
        public string ParentName { get; set; }

        [DataMember]
        [ProtoMember(9)]
        public string DefaultRoleId { get; set; }
        /// <summary>
        /// 菜单的权限控制Id
        /// </summary>
        [DataMember]
        [ProtoMember(10)]
        public string AuthorityId { get; set; }

        [DataMember]
        [ProtoMember(11)]
        public Guid ApplicationId { get; set; }

        [DataMember]
        [ProtoMember(12)]
        public string ApplicationName { get; set; }

        public bool Equals(PermissionSimpleDTO x, PermissionSimpleDTO y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(PermissionSimpleDTO obj)
        {
            return obj.Id.GetHashCode();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
