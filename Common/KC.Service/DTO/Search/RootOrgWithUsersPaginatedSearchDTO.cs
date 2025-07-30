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
    public class RootOrgWithUsersPaginatedSearchDTO : EntityBaseDTO
    {
        public RootOrgWithUsersPaginatedSearchDTO()
        {
            PageIndex = 1;
            PageSize = 10;
            RoleIds = new List<string>();
        }
        [DataMember]
        public List<string> RoleIds { get; set; }
        [DataMember]
        public Guid Appid { get; set; }
        [DataMember]
        public string DepartName { get; set; }
        [DataMember]
        public string DepartCode { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string UserCode { get; set; }
        [DataMember]
        public string SearchKey { get; set; }
        [DataMember]
        public int PageIndex { get; set; }
        [DataMember]
        public int PageSize { get; set; }
    }
}
