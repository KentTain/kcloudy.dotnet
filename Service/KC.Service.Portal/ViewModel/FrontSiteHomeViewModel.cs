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
        /// ʱ�� ��һ������
        /// </summary>
        [DataMember]
        public string ServiceDate { get; set; }
        /// <summary>
        ///  ʱ��   
        /// </summary>
        [DataMember]
        public string ServiceTime { get; set; }
        /// <summary>
        /// ��ҳ�ֲ�ͼ
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
        /// ��ϵ������
        /// </summary>
        [DataMember]
        public string ContactEmail { get; set; }

        /// <summary>
        /// ��ϵ���ֻ�
        /// </summary>
        [DataMember]
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// ��ϵ�˵绰
        /// </summary>
        [DataMember]
        public string ContactTelephone { get; set; }

        /// <summary>
        /// SEO�ؼ���
        /// </summary>
        [DataMember]
        public string KeyWord { get; set; }
        /// <summary>
        /// SEO����
        /// </summary>
        [DataMember]
        public string KeyDescription { get; set; }

        /// <summary>
        /// ��˾���
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