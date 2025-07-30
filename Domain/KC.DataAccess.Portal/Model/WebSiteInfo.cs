using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using KC.Framework.Base;
using KC.Model.Portal.Constants;

namespace KC.Model.Portal
{
    [Table(Tables.WebSiteInfo)]
    public class WebSiteInfo : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// 时间 周一至周五
        /// </summary>
        [MaxLength(50)]
        public string ServiceDate { get; set; }
        /// <summary>
        ///  时间   
        /// </summary>
        [MaxLength(50)]
        public string ServiceTime { get; set; }
        /// <summary>
        /// logo
        /// </summary>
        [MaxLength(1000)]
        public string LogoImage { get; set; }
        /// <summary>
        /// 二维码
        /// </summary>
        [MaxLength(1000)]
        public string QRCode { get; set; }
        /// <summary>
        /// 首页轮播图
        /// </summary>
        [MaxLength(4000)]
        public string HomePageSlide { get; set; }
        /// <summary>
        /// 主图
        /// </summary>
        [MaxLength(1000)]
        public string MainImage { get; set; }

        /// <summary>
        /// 默认联系人UserId
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
        /// SEO关键字
        /// </summary>
        [MaxLength(1000)]
        [DataMember]
        public string KeyWord { get; set; }
        /// <summary>
        /// SEO描述
        /// </summary>
        [MaxLength(2000)]
        [DataMember]
        public string KeyDescription { get; set; }

        [MaxLength(20)]
        [DataMember]
        public string SkinCode { get; set; }

        [MaxLength(200)]
        [DataMember]
        public string SkinName { get; set; }

        [DataMember]
        public string CompanyInfo { get; set; }
    }
}
