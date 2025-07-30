using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;

namespace KC.Service.DTO.Search
{
    [Serializable, DataContract(IsReference = true)]
    public class OrgTreesAndRolesWithUsersSearchDTO : EntityBaseDTO
    {
        public OrgTreesAndRolesWithUsersSearchDTO()
        {
            RoleIds = new List<string>();
            OrgIds = new List<int>();
            ExceptRoleIds = new List<string>();
            ExceptOrgIds = new List<int>();
        }
        [DataMember]
        public int Type { get; set; }
        /// <summary>
        /// 需要查询的角色Id列表
        /// </summary>
        [DataMember]
        public List<string> RoleIds { get; set; }
        /// <summary>
        /// 需要查询的角色Id列表
        /// </summary>
        [DataMember]
        public List<int> OrgIds { get; set; }
        /// <summary>
        /// 需要排除的角色Id列表
        /// </summary>
        [DataMember]
        public List<string> ExceptRoleIds { get; set; }
        /// <summary>
        /// 需要排除的角色Id列表
        /// </summary>
        [DataMember]
        public List<int> ExceptOrgIds { get; set; }
    }

    [Serializable, DataContract(IsReference = true)]
    public class OrgTreesWithUsersSearchDTO : EntityBaseDTO
    {
        public OrgTreesWithUsersSearchDTO()
        {
            OrgIds = new List<int>();
            ExceptOrgIds = new List<int>();
        }
        [DataMember]
        public int Type { get; set; }
        /// <summary>
        /// 需要查询的角色Id列表
        /// </summary>
        [DataMember]
        public List<int> OrgIds { get; set; }
        /// <summary>
        /// 需要排除的角色Id列表
        /// </summary>
        [DataMember]
        public List<int> ExceptOrgIds { get; set; }
    }

    [Serializable, DataContract(IsReference = true)]
    public class RolesWithUsersSearchDTO : EntityBaseDTO
    {
        public RolesWithUsersSearchDTO()
        {
            RoleIds = new List<string>();
            ExceptRoleIds = new List<string>();
        }
        [DataMember]
        public int Type { get; set; }
        /// <summary>
        /// 需要查询的角色Id列表
        /// </summary>
        [DataMember]
        public List<string> RoleIds { get; set; }
        /// <summary>
        /// 需要排除的角色Id列表
        /// </summary>
        [DataMember]
        public List<string> ExceptRoleIds { get; set; }
    }
}
