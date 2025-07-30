using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.SSO.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        //[Required]
        [EmailAddress]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "手机号")]
        public string PhoneNumber { get; set; }

        [Display(Name = "短信验证码")]
        public string Code { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "密码长度至少6位数.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "确定密码")]
        [Compare("Password", ErrorMessage = "两次输入的密码不匹配.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "企业名称")]
        public string TenantName { get; set; }

        public string ReturnUrl { get; set; }
    }
}
