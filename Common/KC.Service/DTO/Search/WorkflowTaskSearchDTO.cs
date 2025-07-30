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
    public class WorkflowTaskSearchDTO : EntityBaseDTO
    {
        public WorkflowTaskSearchDTO()
        {
            RoleIds = new List<string>();
            OrgCodes = new List<string>();
        }
        /// <summary>
        /// 需要查询的用户Id列表
        /// </summary>
        [DataMember]
        public string UserId { get; set; }
        /// <summary>
        /// 需要查询的角色Id列表
        /// </summary>
        [DataMember]
        public List<string> RoleIds { get; set; }
        /// <summary>
        /// 需要查询的部门Id列表
        /// </summary>
        [DataMember]
        public List<string> OrgCodes { get; set; }
        
    }
}
