using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KC.Web.SSO.Models.AccountViewModels
{
    /// <summary>
    /// 登录输入对象
    /// </summary>
    public class LoginInputModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool RememberLogin { get; set; }
        /// <summary>
        /// 会跳地址
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 租户代码
        /// </summary>
        public string TenantName { get; set; }
        /// <summary>
        /// 租户显示名
        /// </summary>
        public string TenantDisplayName { get; set; }
    }

    public class LoginViewModel : LoginInputModel
    {
        public LoginViewModel()
        {
            ExternalProviders = new List<ExternalProvider>();
        }
        public string RedirectUrl { get; set; }

        public bool AllowRememberLogin { get; set; }
        public bool EnableLocalLogin { get; set; }

        //public bool IsRedirectToTenantLogin { get; set; }

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
        public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
        public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

    }
}
