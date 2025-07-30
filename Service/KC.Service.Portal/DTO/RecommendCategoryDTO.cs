using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KC.Service.DTO.Portal
{

    [Serializable, DataContract(IsReference = true)]
    public class RecommendCategoryDTO : TreeNodeDTO<RecommendCategoryDTO>
    {
        public RecommendCategoryDTO()
        {
            RecommendOfferings = new List<RecommendOfferingDTO>();
            RecommendCustomers = new List<RecommendCustomerDTO>();
            RecommendRequirements = new List<RecommendRequirementDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        [MaxLength(512)]
        [DataMember]
        public string ImageBlob { get; set; }

        [MaxLength(512)]
        [DataMember]
        public string FileBlob { get; set; }

        [MaxLength(4000)]
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsShow { get; set; }

        [DataMember]
        public string ParentName { get; set; }

        [DataMember]
        public ICollection<RecommendOfferingDTO> RecommendOfferings { get; set; }


        [DataMember]
        public ICollection<RecommendCustomerDTO> RecommendCustomers { get; set; }


        [DataMember]
        public ICollection<RecommendRequirementDTO> RecommendRequirements { get; set; }
    }
}
