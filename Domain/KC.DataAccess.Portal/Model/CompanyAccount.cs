using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Portal;
using KC.Framework.Base;
using KC.Model.Portal.Constants;

namespace KC.Model.Portal
{
    [Table(Tables.CompanyAccount)]
    public class CompanyAccount : Entity
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// 银行账户类型
        /// </summary>
        public BankAccountType BankType { get; set; }

        /// <summary>
        /// 开户名
        /// </summary>
        [MaxLength(1024)]
        public string AccountName { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        [MaxLength(512)]
        public string BankName { get; set; }
        /// <summary>
        /// 开户银行帐号
        /// </summary>
        [MaxLength(128)]
        public string BankNumber { get; set; }

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
        /// 开户行地址
        /// </summary>
        [MaxLength(1024)]
        public string BankAddress { get; set; }

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

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(512)]
        public string Remark { get; set; }

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
