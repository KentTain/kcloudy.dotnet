using KC.Common;
using KC.Framework.Extension;
using KC.Service.DTO;
using KC.Service.DTO.Portal;
using System;
using System.Collections.Generic;

namespace KC.Service.ViewModel.Portal
{
    public class FrontSiteCompanyViewModel
    {
        public FrontSiteCompanyViewModel()
        {
            RecommendOfferings = new List<RecommendOfferingDTO>();
            RecommendRequirements = new List<RecommendRequirementDTO>();
        }
        public string Name { get; set; }

        /// <summary>
        /// 时间 周一至周五
        /// </summary>
        public string ServiceDate { get; set; }
        /// <summary>
        ///  时间   
        /// </summary>
        public string ServiceTime { get; set; }
        /// <summary>
        /// 首页轮播图
        /// </summary>
        public string HomePageSlide { get; set; }
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
        public string ContactEmail { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string ContactTelephone { get; set; }

        /// <summary>
        /// SEO关键字
        /// </summary>
        public string KeyWord { get; set; }
        /// <summary>
        /// SEO描述
        /// </summary>
        public string KeyDescription { get; set; }

        /// <summary>
        /// 公司简介
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// logo
        /// </summary>
        public string LogoImageBlob { get; set; }

        public BlobInfoDTO LogoImage
        {
            get
            {
                if (LogoImageBlob.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(LogoImageBlob);
            }
        }



        public List<RecommendOfferingDTO> RecommendOfferings { get; set; }

        public List<RecommendRequirementDTO> RecommendRequirements { get; set; }
    }
}