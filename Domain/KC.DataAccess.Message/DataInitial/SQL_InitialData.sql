
--DELETE FROM [cTest].[msg_MessageTemplate]
--DELETE FROM [cTest].[msg_MessageClass]
--DELETE FROM [cTest].[msg_MessageCategory]
GO

-----------------------MessageType.HR=2------用户管理：系统管理---------------
INSERT [cTest].[msg_MessageCategory] ([ParentId], [Name], [TreeCode], [Leaf], [Level], [Index], [Description], [ReferenceId], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (NULL, N'用户管理', NULL, 1, 1, 1, NULL, NULL, 0, N'admin@cfwin.com', CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), N'admin@cfwin.com', CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2))
GO
declare @categoryId INT
set @categoryId= (SELECT @@IDENTITY )
update [cTest].[msg_MessageCategory] set [TreeCode]=@categoryId where [Id]=@categoryId

--用户创建成功--MGT2016010100001--KC.Service.Account.Message.UserTemplateGenerator
INSERT [cTest].[msg_MessageClass] ([Type], [Name], [Code], [ReplaceParametersString], [Desc], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ApplicationId],[ApplicationName],[MessageCategoryId]) 
VALUES (1, N'用户创建成功', N'MGT2016010100001', N'UserId,UserName,DisplayName,Email,PhoneNumber,Password,LoginUrl', N'可用参数包括：{UserId}，{UserName}，{DisplayName}，{Email}，{PhoneNumber}，{Password}，{LoginUrl}', 0, 0, N'admin@cfwin.com', CAST(0x0000A51A009A9690 AS DateTime), N'admin@cfwin.com', CAST(0x0000A530006ABEBA AS DateTime), N'45672506-DDB7-4D57-AD44-BD0AB136B556', '账户管理', @categoryId)

declare @classId INT
set @classId= (SELECT @@IDENTITY )
INSERT [cTest].[msg_MessageTemplate] ([MessageClassId], [TemplateType], [Name], [Subject], [Content], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
VALUES (@classId, 0, N'用户创建成功', N'用户创建成功', N'<p>用户：{DisplayName}已经创建成功！</p>', 0, N'admin@cfwin.com', CAST(0x0000A51A009A9638 AS DateTime), N'admin@cfwin.com', CAST(0x0000A530006ABEF4 AS DateTime))
INSERT [cTest].[msg_MessageTemplate] ([MessageClassId], [TemplateType], [Name], [Subject], [Content], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
VALUES (@classId, 1, N'用户创建成功', N'用户创建成功', N'<p>用户：{DisplayName}已经创建成功！</p>', 0, N'admin@cfwin.com', CAST(0x0000A51A009A9638 AS DateTime), N'admin@cfwin.com', CAST(0x0000A530006ABEF4 AS DateTime))
--使用阿里云SMS的模板：Subject=阿里云模板Code；Content=所需替换的Json字符串，例如：{"name":"17744949695"}
INSERT [cTest].[msg_MessageTemplate] ([MessageClassId], [TemplateType], [Name], [Subject], [Content], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
VALUES (@classId, 2, N'用户创建成功', N'SMS_202806584', N'{"name":"{DisplayName}"}', 0, N'admin@cfwin.com', CAST(0x0000A51A009A9638 AS DateTime), N'admin@cfwin.com', CAST(0x0000A530006ABEF4 AS DateTime))


--重置后台用户密码--MGT2016010100002--Com.Service.Account.Message.UserTemplateGenerator
INSERT [cTest].[msg_MessageClass] ([Type], [Name], [Code], [ReplaceParametersString], [Desc], [Index], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [ApplicationId],[ApplicationName],[MessageCategoryId]) 
VALUES (1, N'重置登录密码', N'MGT2016010100002', N'UserId,UserName,DisplayName,Email,PhoneNumber,Password,LoginUrl', N'可用参数包括：{UserId}，{UserName}，{DisplayName}，{Email}，{PhoneNumber}，{Password}，{LoginUrl}', 0, 0, N'admin@cfwin.com', CAST(0x0000A51A00A7FC54 AS DateTime), N'admin@cfwin.com', CAST(0x0000A53000684A39 AS DateTime), N'45672506-DDB7-4D57-AD44-BD0AB136B556', '账户管理', @categoryId)

set @classId= (SELECT @@IDENTITY )
INSERT [cTest].[msg_MessageTemplate] ([MessageClassId], [TemplateType], [Name], [Subject], [Content], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
VALUES (@classId, 0, N'重置登录密码', N'重置登录密码', N'<p>您好，{DisplayName}！<br/>点击链接重置密码：<a href="$ResetPasswordUrl" target="_blank">$ResetPasswordUrl</a></p>', 0, N'admin@cfwin.com', CAST(0x0000A51A00A7FC4C AS DateTime), N'admin@cfwin.com', CAST(0x0000A530006849A8 AS DateTime))
INSERT [cTest].[msg_MessageTemplate] ([MessageClassId], [TemplateType], [Name], [Subject], [Content], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
VALUES (@classId, 1, N'重置登录密码', N'重置登录密码', N'<p>您好，{DisplayName}！<br/>点击链接重置密码：<a href="$ResetPasswordUrl" target="_blank">$ResetPasswordUrl</a></p>', 0, N'admin@cfwin.com', CAST(0x0000A51A00A7FC4C AS DateTime), N'admin@cfwin.com', CAST(0x0000A530006849A8 AS DateTime))
GO

