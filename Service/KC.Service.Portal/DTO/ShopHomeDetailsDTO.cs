using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KC.Service.DTO.Portal
{

    [Serializable, DataContract(IsReference = true)]
    public class ShopHomeDetailsDTO
    {
        public ShopHomeDetailsDTO()
        {
            ShopInfoAll = new ShopInfoAllDTO();
        }
        public ShopInfoAllDTO ShopInfoAll { get; set; }
    }

    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ShopInfoAllDTO
    {
        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public string Years { get; set; }
        [DataMember]
        public string LogImgUrl { get; set; }
        [DataMember]
        public string Level { get; set; }
        [DataMember]
        public string BusinessModel { get; set; }
        [DataMember]
        public string CompanyAdress { get; set; }
        [DataMember]
        public string ContactPerson { get; set; }
        [DataMember]
        public string CustomerServicePhone { get; set; }

        [DataMember]
        public string CustomerServiceQQ { get; set; }

        [DataMember]
        public string QRCode { get; set; }

        [DataMember]
        public string ServiceTime { get; set; }

        [DataMember]
        public string ServiceDate { get; set; }

        [DataMember]
        public string ImgUrl { get; set; }

        [DataMember]
        public string CompanyInfo { get; set; }

        [DataMember]
        public string StoreDescription { get; set; }

        [DataMember]
        public string StoreKeywords { get; set; }

        [DataMember]
        public string OfferingContactName { get; set; }
        [DataMember]
        public string OfferingContactPhone { get; set; }
        [DataMember]
        public string OfferingContactQQ { get; set; }
        [DataMember]
        public string IndustryName { get; set; }
        [DataMember]
        public string ShopHomePageSlide { get; set; }
        [DataMember]
        public string SkinUrl { get; set; }

        [DataMember]
        public string PlatformDomain { get; set; }
    }
}
