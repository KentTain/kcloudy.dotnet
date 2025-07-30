using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using KC.Framework.Extension;
using KC.Framework.Tenant;

namespace KC.Service.DTO.Portal
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class CompanyContactDTO : EntityDTO
    {
        [DataMember]
        public bool IsEditMode { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public BusinessType BusinessType { get; set; }
        [DataMember]
        public string BusinessTypeString { get { return BusinessType.ToDescription(); } }

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
        [Required(ErrorMessage = "联系人姓名不能为空！")]
        public string ContactName { get; set; }
        /// <summary>
        /// 联系人邮箱
        /// </summary>
        [MaxLength(100)]
        [DataMember]
        [Required(ErrorMessage = "邮箱不能为空！")]
        public string ContactEmail { get; set; }
        [MaxLength(20)]
        [DataMember]
        [Required(ErrorMessage = "手机不能为空！")]
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 联系人QQ
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ContactQQ { get; set; }

        /// <summary>
        /// 联系人微信
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ContactWeixin { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ContactTelephone { get; set; }
        /// <summary>
        /// 联系人职位
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string PositionName { get; set; }
        /// <summary>
        /// 是否公布
        /// </summary>
        [DataMember]
        public bool IsPublish { get; set; }

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
    }
}
