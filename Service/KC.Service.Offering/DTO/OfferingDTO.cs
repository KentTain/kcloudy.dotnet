using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Enums.Offering;
using KC.Common;

namespace KC.Service.DTO.Offering
{
    public class OfferingDTO : EntityDTO
    {
        public OfferingDTO()
        {
            DeletedOfferingPropertyIds = new List<int>();
            OfferingProperties = new List<OfferingPropertyDTO>();
            OfferingOperationLogs = new List<OfferingOperationLogDTO>();
        }

        public bool IsEditMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OfferingId { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [MaxLength(16)]
        public string OfferingCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [MaxLength(256)]
        public string OfferingName { get; set; }

        /// <summary>
        /// 商品类型：kc.enums.OfferingType
        /// </summary>
        [MaxLength(128)]
        public string OfferingTypeCode { get; set; }
        [MaxLength(512)]
        public string OfferingTypeName { get; set; }
        /// <summary>
        /// 商品审批状态：kc.enums.OfferingStatus
        /// </summary>
        public OfferingStatus Status { get; set; }
        /// <summary>
        /// 商品单位
        /// </summary>
        [MaxLength(20)]
        public string OfferingUnit { get; set; }

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 默认的商品图片
        /// </summary>
        [MaxLength(1000)]
        public string OfferingImage { get; set; }
        /// <summary>
        /// 默认的商品图片对象
        /// </summary>
        public BlobInfoDTO OfferingImageBlob
        {
            get
            {
                if (string.IsNullOrEmpty(OfferingImage))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(OfferingImage);
            }
        }
        /// <summary>
        /// 数据来自于offeringProperties，商品图片对象列表
        /// </summary>
        public List<BlobInfoDTO> OfferingImageBlobs
        {
            get
            {
                var offeringImageBlobs = new List<BlobInfoDTO>();
                if (OfferingProperties == null || OfferingProperties.Count() <= 0)
                    return offeringImageBlobs;

                List<OfferingPropertyDTO> blobProperties = OfferingProperties.Where(m => m.OfferingPropertyType == OfferingPropertyType.Image).OrderBy(m => m.Index).ToList();
                foreach (var blobProperty in blobProperties)
                {
                    BlobInfoDTO blob = SerializeHelper.FromJson<BlobInfoDTO>(blobProperty.Value);
                    if (blob != null)
                    {
                        //第一次保存时（新创建的），自增列：属性Id为序列化成Json字符串中，故反序列化需要将属性Id进行重新赋值
                        blob.PropertyId = blobProperty.Id;
                        offeringImageBlobs.Add(blob);
                    }
                }
                return offeringImageBlobs;
            }
        }

        /// <summary>
        /// 商品文档对象
        /// </summary>
        [MaxLength(1000)]
        public string OfferingFile { get; set; }
        /// <summary>
        /// 商品文档对象列表
        /// </summary>
        public BlobInfoDTO OfferingFileBlob
        {
            get
            {
                if (string.IsNullOrEmpty(OfferingFile))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(OfferingFile);
            }
        }
        /// <summary>
        /// 商品单价
        /// </summary>
        public decimal? OfferingPrice { get; set; }
        /// <summary>
        /// 商品折扣
        /// </summary>
        public decimal? OfferingDiscount { get; set; }
        /// <summary>
        /// 商品税率
        /// </summary>
        public decimal? OfferingRate { get; set; }
        /// <summary>
        /// 描述(显示时，填充html description)
        /// </summary>
        [MaxLength(4000)]
        public string Description { get; set; }
        /// <summary>
        /// 商品二维码BlobId
        /// </summary>
        [MaxLength(512)]
        public string Barcode { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public List<int> DeletedOfferingPropertyIds { get; set; }

        public List<OfferingPropertyDTO> OfferingProperties { get; set; }

        public List<OfferingOperationLogDTO> OfferingOperationLogs { get; set; }
    }
}
