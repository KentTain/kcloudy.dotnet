
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using KC.Web.Extension;

namespace KC.Web.Base
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [StringLength(16, ErrorMessage = "密码长度必须在{2}位与{1}位之间。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        //[RegisterPasswordValidation]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }
}