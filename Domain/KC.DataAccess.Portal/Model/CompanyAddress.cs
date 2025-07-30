using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using KC.Enums.Portal;
using KC.Framework.Base;
using KC.Model.Portal.Constants;

namespace KC.Model.Portal
{
    /// <summary>
    /// 公司地址
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [Table(Tables.CompanyAddress)]
    public class CompanyAddress : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [MaxLength(1024)]
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 地图经度坐标
        /// </summary>
        [DataMember]
        public decimal LongitudeX { get; set; }

        /// <summary>
        /// 地图纬度坐标
        /// </summary>
        [DataMember]
        public decimal LatitudeY { get; set; }

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
        /// 是否公布
        /// </summary>
        [DataMember]
        public bool IsPublish { get; set; }

        [MaxLength(1000)]
        [DataMember]
        public string Remark { set; get; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int Index { get; set; }

        /// <summary>
        /// 企业编号
        /// </summary>
        public string CompanyCode { get; set; }
        [ForeignKey("CompanyCode")]
        public CompanyInfo CompanyInfo { get; set; }

    }
}
