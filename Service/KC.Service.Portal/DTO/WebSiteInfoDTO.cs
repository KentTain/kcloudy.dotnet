using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.ComponentModel.DataAnnotations;
using KC.Common;

namespace KC.Service.DTO.Portal
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class WebSiteInfoDTO : EntityDTO
    {
        [DataMember]
        public bool IsEditMode { get; set; }
        [DataMember]
        public int Id { get; set; }
        [MaxLength(128)]
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 时间 周一至周五
        /// </summary>
        [MaxLength(50)]
        [DataMember]
        public string ServiceDate { get; set; }
        /// <summary>
        ///  时间   
        /// </summary>
        [MaxLength(50)]
        [DataMember]
        public string ServiceTime { get; set; }
        /// <summary>
        /// logo
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        public string LogoImage { get; set; }
        /// <summary>
        /// 租户Logo对象
        /// </summary>
        [DataMember]
        public BlobInfoDTO LogoImageBlob
        {
            get
            {
                if (string.IsNullOrEmpty(LogoImage))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(LogoImage);
            }
        }
        /// <summary>
        /// 二维码
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string QRCode { get; set; }
        /// <summary>
        /// 首页轮播图
        /// </summary>
        [MaxLength(2046)]
        [DataMember]
        public string HomePageSlide { get; set; }
        /// <summary>
        /// 首页轮播图对象
        /// </summary>
        [DataMember]
        public List<BlobInfoDTO> HomePageSlideBlobs
        {
            get
            {
                if (string.IsNullOrEmpty(HomePageSlide))
                    return null;

                return SerializeHelper.FromJson<List<BlobInfoDTO>>(HomePageSlide);
            }
        }
        /// <summary>
        /// 主图
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string MainImage { get; set; }
        /// <summary>
        /// 主图对象
        /// </summary>
        [DataMember]
        public BlobInfoDTO MainImageBlob
        {
            get
            {
                if (string.IsNullOrEmpty(MainImage))
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(MainImage);
            }
        }
        /// <summary>
        /// 默认联系人UserId
        /// </summary>
        [DataMember]
        [MaxLength(128)]
        public string ContactId { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        [MaxLength(50)]
        [DataMember]
        public string ContactName { get; set; }

        /// <summary>
        /// 联系人QQ
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ContactQQ { get; set; }

        /// <summary>
        /// 联系人微信
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ContactWeixin { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ContactTelephone { get; set; }

        /// <summary>
        /// SEO关键字
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        public string KeyWord { get; set; }
        /// <summary>
        /// SEO描述
        /// </summary>
        [MaxLength(2000)]
        [DataMember]
        public string KeyDescription { get; set; }

        [MaxLength(20)]
        [DataMember]
        public string SkinCode { get; set; }

        [MaxLength(200)]
        [DataMember]
        public string SkinName { get; set; }

        [DataMember]
        public string CompanyInfo { get; set; }
    }
}
