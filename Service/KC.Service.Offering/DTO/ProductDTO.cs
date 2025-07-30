using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Common;
using KC.Enums.Offering;
using KC.Model.Offering;

namespace KC.Service.DTO.Offering
{
    public class ProductDTO : EntityDTO
    {
        public ProductDTO()
        {
            ProductProperties = new List<ProductPropertyDTO>();
        }
        /// <summary>
        /// 
        /// </summary>
        public int ProductId { get; set; }

        [MaxLength(16)]
        public string ProductCode { get; set; }

        [MaxLength(256)]
        public string ProductName { get; set; }

        [MaxLength(20)]
        public string ProductUnit { get; set; }

        public bool IsEnabled { get; set; }
        [MaxLength(1000)]
        public string ProductImage { get; set; }
        /// <summary>
        /// 默认的商品图片对象
        /// </summary>
        public BlobInfoDTO ProductImageBlob
        {
            get
            {
                if (string.IsNullOrEmpty(ProductImage))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(ProductImage);
            }
        }
        /// <summary>
        /// 数据来自于offeringProperties，商品图片对象列表
        /// </summary>
        public List<BlobInfoDTO> ProductImageBlobs
        {
            get
            {
                var offeringFileBlobs = new List<BlobInfoDTO>();
                if (ProductProperties == null || ProductProperties.Count() <= 0)
                    return offeringFileBlobs;

                List<ProductPropertyDTO> blobProperties = ProductProperties.Where(m => m.ProductPropertyType == ProductPropertyType.Image).OrderBy(m => m.Index).ToList();
                foreach (var blobProperty in blobProperties)
                {
                    BlobInfoDTO blob = SerializeHelper.FromJson<BlobInfoDTO>(blobProperty.Value);
                    if (blob != null)
                    {
                        //第一次保存时（新创建的），自增列：属性Id为序列化成Json字符串中，故反序列化需要将属性Id进行重新赋值
                        blob.PropertyId = blobProperty.Id;
                        offeringFileBlobs.Add(blob);
                    }
                }
                return offeringFileBlobs;
            }
        }

        [MaxLength(1000)]
        public string ProductFile { get; set; }
        /// <summary>
        /// 商品文档对象列表
        /// </summary>
        public BlobInfoDTO ProductFileBlob
        {
            get
            {
                if (string.IsNullOrEmpty(ProductFile))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(ProductFile);
            }
        }
        public decimal? ProductPrice { get; set; }

        public decimal? ProductDiscount { get; set; }

        public decimal? ProductRate { get; set; }

        [MaxLength(512)]
        public string Barcode { get; set; }

        public int Index { get; set; }

        public int OfferingId { get; set; }
        public virtual OfferingDTO Offering { get; set; }

        public virtual ICollection<ProductPropertyDTO> ProductProperties { get; set; }
    }
}
