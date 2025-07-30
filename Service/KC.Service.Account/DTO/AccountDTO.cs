using KC.Framework.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Account
{
    #region User Info
    [Serializable]
    [DataContract(IsReference = true)]
    public class UserManageInfoDTO
    {
        [DataMember]
        public string LocalLoginProvider { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public IEnumerable<UserLoginProviderDTO> Logins { get; set; }
        [DataMember]
        public IEnumerable<ExternalLoginDTO> ExternalLoginProviders { get; set; }
    }

    #endregion

    #region Register
    [Serializable]
    [DataContract(IsReference = true)]
    public class UserRegisterDTO
    {
        [Display(Name = "用户类型")]
        public UserType UserType { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "姓名")]
        public string DisplayName { get; set; }

        [EmailAddress]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "手机号")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "公司名称")]
        public string CompanyName { get; set; }

        [MaxLength(20)]
        [Display(Name = "推荐人编号")]
        public string Recommended { get; set; }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class UserRegisterExternalDTO
    {
        [DataMember]
        [Required]
        [Display(Name = "应用程序Id")]
        public string ApplicationId { get; set; }
        [DataMember]
        [Required]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }
    [Serializable]
    [DataContract(IsReference = true)]
    public class UserRegisterInfoDTO
    {
        //public string ApplicationId { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public bool HasRegistered { get; set; }
        [DataMember]
        public string LoginProvider { get; set; }
    }
    #endregion

    #region Password
    [Serializable]
    [DataContract(IsReference = true)]
    public class ChangePasswordDTO
    {
        [DataMember]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }
        [DataMember]
        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }
        [DataMember]
        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }
    [Serializable]
    [DataContract(IsReference = true)]
    public class SetPasswordDTO
    {
        [DataMember]
        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }
        [DataMember]
        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }
    [Serializable]
    [DataContract(IsReference = true)]
    public class ResetPasswordDTO
    {
        [DataMember]
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string UserId { get; set; }
        [DataMember]
        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }
        [DataMember]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
        [DataMember]
        public string Code { get; set; }
    }
    [Serializable]
    [DataContract(IsReference = true)]
    public class ForgotPasswordDTO
    {
        [DataMember]
        [Required]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }

    #endregion

    #region Login
    [Serializable]
    [DataContract(IsReference = true)]
    public class AddExternalLoginDTO
    {
        [DataMember]
        [Required]
        [Display(Name = "外部访问令牌")]
        public string ExternalAccessToken { get; set; }
    }
    [Serializable]
    [DataContract(IsReference = true)]
    public class ExternalLoginDTO
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string State { get; set; }
    }
    [Serializable]
    [DataContract(IsReference = true)]
    public class UserLoginProviderDTO
    {
        [DataMember]
        [Required]
        [Display(Name = "登录提供程序")]
        public string LoginProvider { get; set; }
        [DataMember]
        [Required]
        [Display(Name = "提供程序密钥")]
        public string ProviderKey { get; set; }
    }
    [Serializable]
    [DataContract(IsReference = true)]
    public class UserLoginDTO
    {
        [DataMember]
        [Required]
        [Display(Name = "应用程序Id")]
        public string ApplicationId { get; set; }
        [DataMember]
        [Required]
        [Display(Name = "电子邮件")]
        [EmailAddress]
        public string Email { get; set; }
        [DataMember]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }
        [DataMember]
        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    #endregion
}
