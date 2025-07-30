using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.App
{
    [DataContract]
    public enum ApplicationStatus
    {
        [EnumMember]
        [Description("等待开通")]
        Initial = 0,
        [EnumMember]
        [Description("生成客户数据库")]
        GeneratDataBase = 1,
        [EnumMember]
        [Description("开通Web服务器")]
        OpenWebServer = 2,
        [EnumMember]
        [Description("开通成功")]
        OpenSuccess = 3,
        [EnumMember]
        [Description("已结过期")]
        Expired = 4
    }

    [DataContract]
    public enum OpenServerType
    {
        [EnumMember]
        [Description("创建数据库登录用户")]
        CreateDbUser = 0,
        [EnumMember]
        [Description("生成数据库")]
        OpenDatabase = 1,
        [EnumMember]
        [Description("开通Web服务器")]
        OpenWebServer = 2,
        [EnumMember]
        [Description("更新应用状态")]
        UpdateStatus = 3,
        [EnumMember]
        [Description("回滚数据库")]
        RollbackDatabase = 4,
        [EnumMember]
        [Description("关闭Web服务器")]
        CloseWebServer = 5,
        [EnumMember]
        [Description("发送开通邮件")]
        SendOpenEmail = 6,
        [EnumMember]
        [Description("发送开通短信")]
        SendOpenSms = 7,
        [EnumMember]
        [Description("修改管理员初始信息")]
        ChangeAdminUserRawInfo = 8,
        [EnumMember]
        [Description("创建Gitlab服务")]
        OpenGitlab = 9,
    }


    [DataContract]
    public enum OperationLogType
    {
        [EnumMember]
        [Description("更改应用状态")]
        ChangeStatus = 0,
        [EnumMember]
        [Description("创建应用")]
        OpenApplication = 1,
        [EnumMember]
        [Description("发送邮件")]
        SendEmail = 2,
    }
}
