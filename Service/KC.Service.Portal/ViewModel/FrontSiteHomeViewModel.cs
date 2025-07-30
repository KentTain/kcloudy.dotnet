using KC.Common;
using KC.Framework.Extension;
using KC.Service.DTO;
using KC.Service.DTO.Message;
using KC.Service.DTO.Portal;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KC.Service.ViewModel.Portal
{
    [Serializable, DataContract(IsReference = true)]
    public class FrontSiteHomeViewModel
    {
        public FrontSiteHomeViewModel()
        {
            LatestNews = new List<NewsBulletinDTO>();
            Categories = new List<RecommendCategoryDTO>();
            RecommendCompanies = new List<RecommendInfoDTO>();
            RecommendOfferings = new List<RecommendInfoDTO>();
            RecommendRequirements = new List<RecommendInfoDTO>();
        }

        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 时间 周一至周五
        /// </summary>
        [DataMember]
        public string ServiceDate { get; set; }
        /// <summary>
        ///  时间   
        /// </summary>
        [DataMember]
        public string ServiceTime { get; set; }
        /// <summary>
        /// 首页轮播图
        /// </summary>
        [DataMember]
        public string HomePageSlide { get; set; }
        [DataMember]
        public List<BlobInfoDTO> HomePageSlideImages
        {
            get
            {
                if (HomePageSlide.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<List<BlobInfoDTO>>(HomePageSlide);
            }
        }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        [DataMember]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        [DataMember]
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        [DataMember]
        public string ContactTelephone { get; set; }

        /// <summary>
        /// SEO关键字
        /// </summary>
        [DataMember]
        public string KeyWord { get; set; }
        /// <summary>
        /// SEO描述
        /// </summary>
        [DataMember]
        public string KeyDescription { get; set; }

        /// <summary>
        /// 公司简介
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// logo
        /// </summary>
        [DataMember]
        public string LogoImageBlob { get; set; }

        [DataMember]
        public BlobInfoDTO LogoImage
        {
            get
            {
                if (LogoImageBlob.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(LogoImageBlob);
            }
        }

        [DataMember]
        public List<NewsBulletinDTO> LatestNews { get; set; }

        [DataMember]
        public List<RecommendCategoryDTO> Categories { get; set; }

        [DataMember]
        public List<RecommendInfoDTO> RecommendCompanies { get; set; }

        [DataMember]
        public List<RecommendInfoDTO> RecommendOfferings { get; set; }

        [DataMember]
        public List<RecommendInfoDTO> RecommendRequirements { get; set; }
    }
}