using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using KC.Service.DTO;
using KC.Enums.Portal;
using KC.Framework.Extension;

namespace KC.Service.DTO.Portal
{
    /// <summary>
    /// 公司地址
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class CompanyAddressDTO : EntityDTO
    {
        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 公司地址类型：
        ///     公司注册地址，公司收货地址，公司发货地址
        /// </summary>
        [DataMember]
        public AddressType AddressType { get; set; }

        [DataMember]
        public string AddressTypeString { get { return AddressType.ToDescription(); } }

        /// <summary>
        /// 邮编
        /// </summary>
        [DataMember]
        public int ZipCode { get; set; }
        /// <summary>
        /// 是否公布
        /// </summary>
        [DataMember]
        public bool IsPublish { get; set; }

        /// <summary>
        /// 省份Id
        /// </summary>
        [DataMember]
        public int ProvinceId { get; set; }
        /// <summary>
        /// 省份代码
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string ProvinceCode { get; set; }
        /// <summary>
        /// 省份名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [Required(ErrorMessage = "省份不能为空！")]
        public string ProvinceName { get; set; }

        /// <summary>
        /// 城市Id
        /// </summary>
        [DataMember]
        public int CityId { get; set; }
        /// <summary>
        /// 城市代码
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string CityCode { get; set; }
        /// <summary>
        /// 城市名
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        [Required(ErrorMessage = "城市不能为空！")]
        public string CityName { get; set; }
        /// <summary>
        /// 区域Id
        /// </summary>
        [DataMember]
        public int DistrictId { get; set; }
        /// <summary>
        /// 区域代码
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string DistrictCode { get; set; }
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
        /// 街道代码
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string StreetCode { get; set; }
        /// <summary>
        /// 街道名
        /// </summary>
        [MaxLength(256)]
        [DataMember]
        public string StreetName { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        [Required(ErrorMessage = "详细地址不能为空！")]
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

        [MaxLength(1000)]
        [DataMember]
        public string Remark { set; get; }

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
        /// 排序
        /// </summary>
        [DataMember]
        public int Index { get; set; }
        /// <summary>
        /// 企业编号
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }
        [DataMember]
        public string CompanyName{ get; set; }
    }
}
