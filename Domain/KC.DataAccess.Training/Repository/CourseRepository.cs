using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using KC.Database.EFRepository;
using KC.DataAccess.Training;
using Microsoft.EntityFrameworkCore;
using KC.Model.Training;

namespace KC.DataAccess.Account.Repository
{
    public class CourseRepository : EFRepositoryBase<Course>, ICourseRepository
    {
        public CourseRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        //public ConfigEntityRepository(Tenant tenant)
        //    : base(new ComConfigUnitOfWorkContext(tenant))
        //{
        //}

        public bool AddEmailConfig()
        {
            var dbContext = EFContext.Context as ComTrainingContext;
            var tenantName = dbContext.TenantName;
            var sql = @"SET IDENTITY_INSERT [dba].[sys_ConfigEntity] ON 
INSERT [dba].[sys_ConfigEntity] ([ConfigId], [ConfigType], [ConfigName], [ConfigDescription], [ConfigXml], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
VALUES (9, 1, N'邮件服务', N'Outlook邮件服务', NULL, 0, N'admin@cfwin.com', CAST(N'2015-12-03 08:07:51.817' AS DateTime), N'admin@cfwin.com', CAST(N'2015-12-03 08:07:57.163' AS DateTime))
GO
SET IDENTITY_INSERT [dba].[sys_ConfigEntity] OFF
GO";
            sql += @"INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'Smtp服务器地址', N'', 9, N'Server', N'smtp.partner.outlook.cn', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.587' AS DateTime), N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.637' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'端口号', N'', 9, N'Port', N'587', 2, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.587' AS DateTime), N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.637' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'用户名', N'', 9, N'Account', N'xxx@cfwin.com', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.587' AS DateTime), N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.637' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'用户密码', N'', 9, N'Password', N'xxx', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.587' AS DateTime), N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.637' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'是否使用SSL', N'', 9, N'EnableSsl', N'true',1, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.587' AS DateTime), N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.637' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'是否检查密码', N'', 9, N'EnablePwdCheck', N'true',1, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.587' AS DateTime), N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.637' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'是否启用配置', N'为true时，使用配置的邮件服务器，为false时（测试环境），发送至测试邮箱（TestInBoxs）', 9, N'EnableMail', N'true',1, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.587' AS DateTime), N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.637' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'邮件确认链接有效时间', N'电子邮件确认链接有效时间(分钟)', 9, N'EffectiveMinuteCount', N'30',1, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.587' AS DateTime), N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.637' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'签名设置', N'企业签名设置', 9, N'CompanySign', N'',1, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.587' AS DateTime), N'admin@cfwin.com', CAST(N'2015-06-09 06:41:00.637' AS DateTime))
GO
SET IDENTITY_INSERT [dba].[sys_ConfigPropertyAttribute] OFF
GO";

            return base.ExecuteSql(sql.Replace("[dba]", "[" + tenantName + "]"));
        }

        public bool AddSmsConfig()
        {
            var dbContext = EFContext.Context as ComTrainingContext;
            var tenantName = dbContext.TenantName;
            var sql = @"SET IDENTITY_INSERT [dba].[sys_ConfigEntity] ON 
INSERT [dba].[sys_ConfigEntity] ([ConfigId], [ConfigType], [ConfigName], [ConfigDescription], [ConfigXml], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (13, 2, N'创蓝-营销短信', N'创蓝-营销短信', NULL, 0, N'admin@cfwin.com', CAST(N'2015-08-05 10:47:02.757' AS DateTime), N'admin@cfwin.com', CAST(N'2015-08-14 02:30:26.297' AS DateTime))
GO
SET IDENTITY_INSERT [dba].[sys_ConfigEntity] OFF
GO";
            sql += @"INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'发送地址', N'发送地址，此项必须，只需要修改值', 13, N'SmsUrl', N'http://xxx.xxx.xxx.xxx/msg/HttpBatchSendSM', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.273' AS DateTime), N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.473' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'用户名', N'平台账号，此项必须，只需要修改值', 13, N'UserAccount', N'xxxxx', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.273' AS DateTime), N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.473' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'密码', N'平台账号密码，此项必须，只需要修改值', 13, N'Password', N'xxxxx', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.273' AS DateTime), N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.473' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'类型', N'0：通知短信；1：营销短信；2：语音短信', 13, N'Type', N'1', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.273' AS DateTime), N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.473' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'平台名称', N'需要结合程序中实现的Class名称使用。', 13, N'ProviderName', N'CL', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.273' AS DateTime), N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.473' AS DateTime))
";

            return base.ExecuteSql(sql.Replace("[dba]", "[" + tenantName + "]"));
        }

        public bool AddCallCenterConfig()
        {
            var dbContext = EFContext.Context as ComTrainingContext;
            var tenantName = dbContext.TenantName;
            var sql = @"SET IDENTITY_INSERT [dba].[sys_ConfigEntity] ON 
INSERT [dba].[sys_ConfigEntity] ([ConfigId], [ConfigType], [ConfigName], [ConfigDescription], [ConfigXml], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (12, 5, N'长鑫盛通呼叫中心', N'长鑫盛通呼叫中心', NULL, 0, N'admin@cfwin.com', CAST(N'2015-12-03 08:07:51.817' AS DateTime), N'admin@cfwin.com', CAST(N'2015-12-03 08:07:57.163' AS DateTime))
GO
SET IDENTITY_INSERT [dba].[sys_ConfigEntity] OFF
GO";
            sql += @"INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'平台名称', N'需要结合程序中实现的Class名称使用。', 12, N'ProviderName', N'UNCALL', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.273' AS DateTime), N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.473' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'发送地址', N'发送地址，此项必须，只需要修改值', 12, N'SmsUrl', N'http://xxx.xxx.xxx.xxx/', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.273' AS DateTime), N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.473' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'Value1', N'语音文件下载地址，此项必须，只需要修改值', 12, N'Value1', N'uncall_api/downloadFile.php?f_path=', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.273' AS DateTime), N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.473' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'Value2', N'拨打手机前加拨', 12, N'Value2', N'', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.273' AS DateTime), N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.473' AS DateTime))
INSERT [dba].[sys_ConfigPropertyAttribute] ([DisplayName], [Description], [ConfigId], [Name], [Value], [DataType], [CanEdit], [IsProviderAttr], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (N'Value3', N'开通呼叫中心服务所属省市', 12, N'Value3', N'广东深圳', 0, 0, 0, 0, 0,  N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.273' AS DateTime), N'admin@cfwin.com', CAST(N'2015-08-05 11:17:21.473' AS DateTime))
";

            return base.ExecuteSql(sql.Replace("[dba]", "[" + tenantName + "]"));
        }

        public Course GetConfigWithAttributesById(int id)
        {
            return Entities.AsNoTracking()
                .Include(m => m.TeacherSelects)
                .ThenInclude(m => m.Teacher)
                .FirstOrDefault(c => c.CourseId == id && !c.IsDeleted);
        }
        public Course GetConfigWithAttributesByName(string configName)
        {
            return Entities.AsNoTracking()
                .Include(m => m.TeacherSelects)
                .ThenInclude(m => m.Teacher)
                .FirstOrDefault(c => c.Name == configName && !c.IsDeleted);
        }

        public List<Course> GetConfigsWithAttributesByFilter(Expression<Func<Course, bool>> predicate)
        {
            var result = Entities
                .Include(m => m.TeacherSelects)
                .ThenInclude(m => m.Teacher)
                .AsNoTracking()
                .Where(predicate)
                .ToList();
            return result;
        }
    }
}
