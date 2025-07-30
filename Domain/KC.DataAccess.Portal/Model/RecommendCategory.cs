using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Portal.Constants;
using KC.Framework.Base;

namespace KC.Model.Portal
{
    [Table(Tables.RecommendCategory)]
    public class RecommendCategory : TreeNode<RecommendCategory>
    {
        public RecommendCategory()
        {
            RecommendOfferings = new List<RecommendOffering>();
            RecommendCustomers = new List<RecommendCustomer>();
            RecommendRequirements = new List<RecommendRequirement>();
        }

        [MaxLength(512)]
        public string ImageBlob { get; set; }

        [MaxLength(512)]
        public string FileBlob { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        public bool IsShow { get; set; }

        public ICollection<RecommendOffering> RecommendOfferings { get; set; }


        public ICollection<RecommendCustomer> RecommendCustomers { get; set; }


        public ICollection<RecommendRequirement> RecommendRequirements { get; set; }
    }
}
