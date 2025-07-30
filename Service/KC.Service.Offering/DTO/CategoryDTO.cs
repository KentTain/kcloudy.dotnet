using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;
using KC.Common;
using KC.Enums.Offering;

namespace KC.Service.DTO.Offering
{
    public class CategoryDTO : TreeNodeDTO<CategoryDTO>
    {
        public CategoryDTO()
        {
            Offerings = new List<OfferingDTO>();
            CategoryManagers = new List<CategoryManagerDTO>();
            PropertyProviders = new List<PropertyProviderDTO>();
        }

        public bool IsEditMode { get; set; }

        [MaxLength(1000)]
        public string CategoryImage { get; set; }

        public BlobInfoDTO CategoryImageBlob
        {
            get
            {
                if (string.IsNullOrEmpty(CategoryImage))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(CategoryImage);
            }
        }
        [MaxLength(1000)]
        public string CategoryFile { get; set; }
        public BlobInfoDTO CategoryFileBlob
        {
            get
            {
                if (string.IsNullOrEmpty(CategoryFile))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(CategoryFile);
            }
        }

        /// <summary>
        /// 产品属性定义
        /// </summary>
        public OfferingPropertyType OfferingPropertyType { get; set; }
        /// <summary>
        /// 产品价格定义
        /// </summary>
        public OfferingPriceType OfferingPriceType { get; set; }

        public bool IsShow { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        public virtual ICollection<OfferingDTO> Offerings { get; set; }

        public virtual ICollection<CategoryManagerDTO> CategoryManagers { get; set; }

        public virtual ICollection<PropertyProviderDTO> PropertyProviders { get; set; }
    }
}
