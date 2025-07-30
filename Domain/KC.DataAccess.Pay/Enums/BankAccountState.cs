using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.Pay
{
    /// <summary>
    /// 银行账户状态
    /// </summary>
    public enum BankAccountState
    {
        [Description("未认证")]
        UnAuthenticated = 1,
        [Description("认证中")]
        Authenticating = 2,
        [Description("认证成功")]
        AuthenticateSuccess = 3,
        [Description("认证失败")]
        AuthenticateFailed = 4,
        [Description("未绑定")]
        UnBinding = 5,
        [Description("已绑定")]
        Binding = 6
    }
}
