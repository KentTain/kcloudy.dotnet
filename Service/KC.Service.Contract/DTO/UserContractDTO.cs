using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Contract;
using KC.Framework.Extension;
using KC.Service.DTO;

namespace KC.Service.DTO.Contract
{
    public class UserContractDTO : EntityDTO
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string StaffId { get; set; }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public CustomerType CustomerType { get; set; }
        [DataMember]
        public UserContractStatus Statu { get; set; }
        [DataMember]
        public string StatuStr
        {
            get
            {
                return Statu.ToDescription();
            }
        }
        [DataMember]
        public string BreakRemark { get; set; }
        [DataMember]
        public Guid BlobId { get; set; }

        //[DataMember]
        //public ContractGroupDTO ContractGroup { get; set; }

        [DataMember]
        public bool IsEdit { get; set; }
    }
}
