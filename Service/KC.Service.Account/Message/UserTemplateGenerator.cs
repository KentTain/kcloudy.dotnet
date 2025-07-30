using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Common;
using KC.Framework.Tenant;
using KC.Service.Message;
using KC.Service.DTO.Message;
using KC.Service.WebApiService.Business;
using Microsoft.Extensions.Logging;

namespace KC.Service.Account.Message
{
    /// <summary>
    /// 用户管理的消息模板生成器
    /// </summary>
    public class UserTemplateGenerator
    {
        public static string User_Created = "MGT2016010100001";
        public static string User_ResetPassword = "MGT2016010100002";

    }
}
