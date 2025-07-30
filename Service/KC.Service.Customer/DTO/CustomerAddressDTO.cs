using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using KC.Enums.Customer;

namespace KC.Service.DTO.Customer
{
    /// <summary>
    /// 公司地址
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class CustomerAddressDTO : EntityDTO
    {
        [Key]
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 公司地址类型：
        ///     公司注册地址，公司收货地址，公司发货地址
        /// </summary>
        [DataMember]
        public AddressType AddressType { get; set; }
        /// <summary>
        /// 省份Id
        /// </summary>
        [DataMember]
        public int ProvinceId { get; set; }
        /// <summary>
        /// 省份名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ProvinceName { get; set; }
        /// <summary>
        /// 城市Id
        /// </summary>
        [DataMember]
        public int CityId { get; set; }
        /// <summary>
        /// 城市名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string CityName { get; set; }
        /// <summary>
        /// 区域Id
        /// </summary>
        [DataMember]
        public int DistrictId { get; set; }
        /// <summary>
        /// 区域名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string DistrictName { get; set; }
        /// <summary>
        /// 街道Id
        /// </summary>
        [DataMember]
        public int StreetId { get; set; }
        /// <summary>
        /// 街道名
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string StreetName { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        [MaxLength(1024)]
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 地图经度坐标
        /// </summary>
        [DataMember]
        public Decimal LongitudeX { get; set; }

        /// <summary>
        /// 地图纬度坐标
        /// </summary>
        [DataMember]
        public Decimal LatitudeY { get; set; }
        /// <summary>
        /// 百度地图地址
        /// </summary>
        [DataMember]
        [MaxLength(1000)]
        public string BaiduMapUrl { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ZipCode { get; set; }

        /// <summary>
        /// 联系人UserId
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
        /// 是否为默认地址
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 客户内部引用Id
        /// </summary>
        public string ReferenceId { get; set; }
        /// <summary>
        /// 引用Id2
        /// </summary>
        public string ReferenceId2 { get; set; }
        /// <summary>
        /// 引用Id3
        /// </summary>
        public string ReferenceId3 { get; set; }

        [MaxLength(1000)]
        [DataMember]
        public string Remark { set; get; }

        public int CustomerId { get; set; }
        public CustomerInfoDTO CustomerInfo { get; set; }

    }
}
