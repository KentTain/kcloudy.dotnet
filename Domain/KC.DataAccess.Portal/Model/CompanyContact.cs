using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Portal.Constants;

namespace KC.Model.Portal
{
    [Table(Tables.CompanyContact)]
    public class CompanyContact : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public BusinessType BusinessType { get; set; }

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
        /// 联系人邮箱
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ContactEmail { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ContactTelephone { get; set; }

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
        public string CompanyCode { get; set; }
        [ForeignKey("CompanyCode")]
        public CompanyInfo CompanyInfo { get; set; }
    }
}
