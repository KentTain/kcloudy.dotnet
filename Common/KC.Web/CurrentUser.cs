using System.Security.Claims;
using KC.Framework.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using ProtoBuf;
using KC.Framework.Base;

namespace KC.Web
{
    [Serializable, DataContract(IsReference = true), ProtoContract]
    public class CurrentUser
    {
        public CurrentUser()
        {
            //MenuPermission = new List<MenuNodeSimpleDTO>();
            //ActionPermission = new List<PermissionSimpleDTO>();
            RoleIds = new List<string>();
            OrganizationIds = new List<int>();
            OrganizationCodes = new List<string>();
        }

        /// <summary>
        /// UserId
        /// </summary>
        [DataMember]
        [ProtoMember(1)]
        public string UserId { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public string UserCode { get; set; }
        [DataMember]
        [ProtoMember(3)]
        public string UserName { get; set; }
        [DataMember]
        [ProtoMember(4)]
        public string UserPhone { get; set; }
        [DataMember]
        [ProtoMember(5)]
        public string UserEmail { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public string UserDisplayName { get; set; }
        [DataMember]
        [ProtoMember(7)]
        public PositionLevel UserPositionLevel { get; set; }
        [DataMember]
        [ProtoMember(8)]
        public string UserTelephone { get; set; }
        [DataMember]
        [ProtoMember(9)]
        public UserType UserType { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [DataMember]
        [ProtoMember(10)]
        public List<string> RoleIds { get; set; }
        /// <summary>
        /// 组织机构
        /// </summary>
        [DataMember]
        [ProtoMember(11)]
        public List<int> OrganizationIds { get; set; }

        [DataMember]
        [ProtoMember(12)]
        public List<string> OrganizationCodes { get; set; }

        [DataMember]
        [ProtoMember(13)]
        public TenantType CurrentTenantTenantType { get; set; }
        [DataMember]
        [ProtoMember(14)]
        public string CurrentTenantName { set; get; }
        [DataMember]
        [ProtoMember(15)]
        public string CurrentTenantNickName { set; get; }
        [DataMember]
        [ProtoMember(16)]
        public string CurrentTenantDisplayName { set; get; }
    }
}
