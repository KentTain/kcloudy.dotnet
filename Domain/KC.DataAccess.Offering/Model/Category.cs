using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Offering.Constants;
using KC.Framework.Base;
using KC.Enums.Offering;

namespace KC.Model.Offering
{
    [Table(Tables.Category)]
    public class Category : TreeNode<Category>
    {
        public Category()
        {
            Offerings = new List<Offering>();
            CategoryManagers = new List<CategoryManager>();
            PropertyProviders = new List<PropertyProvider>();
        }

        [MaxLength(1000)]
        public string CategoryImage { get; set; }

        [MaxLength(1000)]
        public string CategoryFile { get; set; }

        /// <summary>
        /// 产品属性定义
        /// </summary>
        public OfferingPropertyType OfferingPropertyType { get; set; }
        /// <summary>
        /// 产品价格定义
        /// </summary>
        public OfferingPriceType OfferingPriceType { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        public bool IsShow { get; set; }

        public virtual ICollection<Offering> Offerings { get; set; }

        public virtual ICollection<CategoryManager> CategoryManagers { get; set; }

        public virtual ICollection<PropertyProvider> PropertyProviders { get; set; }

        public virtual ICollection<CategoryOperationLog> CategoryOperationLogs { get; set; }
    }
}
