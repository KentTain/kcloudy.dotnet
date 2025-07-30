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
using KC.Framework.Base;

namespace KC.Service.DTO.Account
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class OrganizationSimpleDTO : TreeNodeSimpleDTO<OrganizationSimpleDTO>
    {
        public OrganizationSimpleDTO()
        {
            Users = new List<UserSimpleDTO>();
        }
        [DataMember]
        public string ParentName { get; set; }
        /// <summary>
        /// 组织编号（SequenceName--Organization：ORG2018120100001）
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "组织编号")]
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 组织类型
        /// </summary>
        [DataMember]
        public OrganizationType OrganizationType { get; set; }
        [DataMember]
        public string OrganizationTypeString
        {
            get
            {
                return OrganizationType.ToDescription();
            }
        }
        /// <summary>
        /// 业务类型
        /// </summary>
        [Display(Name = "业务类型")]
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
        public int Status { get; set; }
        
        /// <summary>
        /// 外部编号1
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [Display(Name = "外部编号1")]
        public string ReferenceId1 { get; set; }

        /// <summary>
        /// 外部编号2
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [Display(Name = "外部编号2")]
        public string ReferenceId2 { get; set; }
        /// <summary>
        /// 外部编号3
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [Display(Name = "外部编号3")]
        public string ReferenceId3 { get; set; }
      
        [DataMember]
        public List<UserSimpleDTO> Users { get; set; }
    }
}
