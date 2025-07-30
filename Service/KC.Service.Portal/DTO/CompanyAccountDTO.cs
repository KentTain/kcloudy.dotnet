using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using KC.Enums.Portal;
using KC.Service.DTO;
using KC.Framework.Extension;

namespace KC.Service.DTO.Portal
{
    [Serializable, DataContract(IsReference = true)]
    public class CompanyAccountDTO : EntityDTO
    {

        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 银行账户类型
        /// </summary>
        [DataMember]
        public BankAccountType BankType { get; set; }

        [DataMember]
        public string BankTypeString { get { return BankType.ToDescription(); } }

        /// <summary>
        /// 开户人姓名
        /// </summary>
        [Required(ErrorMessage = "开户名不能为空！")]
        [DataMember]
        public string AccountName { get; set; }

        /// <summary>
        /// 开户银行
        /// </summary>
        [Required(ErrorMessage = "开户行不能为空！")]
        [DataMember]
        [MaxLength(512)]
        public string BankName { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "请输入正确的银行帐号！")]
        [Required(ErrorMessage = "银行帐号不能为空！")]
        [DataMember]
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
        /// 开户行地址
        /// </summary>
        [DataMember]
        [MaxLength(512)]
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
        [DataMember]
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
        [DataMember]
        public string CompanyCode { get; set; }

        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string BankShowNumber
        {
            get
            {
                var number = string.Empty;
                for (int i = 0; i < BankNumber.Length; i++)
                {
                    if (i < BankNumber.Length - 4)
                    {
                        number += '*';
                    }
                    else
                    {
                        number += BankNumber[i];
                    }
                }
                return number;
            }
        }


    }
}
