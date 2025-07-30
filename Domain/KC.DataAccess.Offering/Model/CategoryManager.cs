using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Offering.Constants;
using KC.Framework.Base;
using KC.Enums.Offering;

namespace KC.Model.Offering
{
    [Table(Tables.CategoryManager)]
    public class CategoryManager : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(128)]
        public string UserId { get; set; }
        /// <summary>
        /// 用户系统编号（SequenceName--Member：USR2018120100001）
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "用户系统编号")]
        public string MemberId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [MaxLength(256)]
        [Display(Name = "用户名")]
        public string UserName { get; set; }
        /// <summary>
        /// 用户显示名
        /// </summary>
        [MaxLength(512)]
        [Display(Name = "用户显示名")]
        public string DisplayName { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        [Display(Name = "用户邮箱")]
        public string Email { get; set; }
        /// <summary>
        /// 用户手机号
        /// </summary>
        [Display(Name = "用户手机号")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 用户QQ号
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "用户QQ号")]
        public string ContactQQ { get; set; }
        /// <summary>
        /// 用户座机号
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "用户座机号")]
        public string Telephone { get; set; }
        /// <summary>
        /// 用户微信号
        /// </summary>
        [MaxLength(128)]
        [Display(Name = "用户微信号")]
        public string OpenId { get; set; }
        /// <summary>
        /// 是否为默认联系人
        /// </summary>
        [Display(Name = "是否为默认联系人")]
        public Boolean IsDefault { get; set; }
        /// <summary>
        /// 是否生效
        /// </summary>
        [Display(Name = "是否生效")]
        public Boolean IsValid { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

    }
}
