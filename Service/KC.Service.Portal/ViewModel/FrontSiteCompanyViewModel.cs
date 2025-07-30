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
        /// ʱ�� ��һ������
        /// </summary>
        public string ServiceDate { get; set; }
        /// <summary>
        ///  ʱ��   
        /// </summary>
        public string ServiceTime { get; set; }
        /// <summary>
        /// ��ҳ�ֲ�ͼ
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
        /// ��ϵ������
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// ��ϵ���ֻ�
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// ��ϵ�˵绰
        /// </summary>
        public string ContactTelephone { get; set; }

        /// <summary>
        /// SEO�ؼ���
        /// </summary>
        public string KeyWord { get; set; }
        /// <summary>
        /// SEO����
        /// </summary>
        public string KeyDescription { get; set; }

        /// <summary>
        /// ��˾���
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