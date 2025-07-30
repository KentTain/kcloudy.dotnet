
DROP VIEW IF EXISTS [cTest].[V_RoleMenuNodes]
GO
CREATE VIEW [cTest].[V_RoleMenuNodes]
AS
SELECT   [cTest].sys_MenuNodesInRoles.RoleId, [cTest].sys_Role.Name AS RoleName, [cTest].sys_Role.DisplayName, 
[cTest].sys_Role.Description, [cTest].sys_Role.IsSystemRole, [cTest].sys_MenuNodesInRoles.MenuNodeId, 
[cTest].sys_MenuNode.ParentId, [cTest].sys_MenuNode.Name AS MenuNodeName, [cTest].sys_MenuNode.AreaName,
[cTest].sys_MenuNode.ControllerName, [cTest].sys_MenuNode.ActionName, [cTest].sys_MenuNode.Parameters, 
[cTest].sys_MenuNode.SmallIcon, [cTest].sys_MenuNode.Version, [cTest].sys_MenuNode.TenantType, 
[cTest].sys_MenuNode.IsExtPage, [cTest].sys_MenuNode.TreeCode, [cTest].sys_MenuNode.Leaf, 
[cTest].sys_MenuNode.[Level], [cTest].sys_MenuNode.[Index], [cTest].sys_MenuNode.ApplicationId, 
[cTest].sys_MenuNode.ApplicationName
FROM      [cTest].sys_Role INNER JOIN
[cTest].sys_MenuNodesInRoles ON [cTest].sys_Role.Id = [cTest].sys_MenuNodesInRoles.RoleId INNER JOIN
[cTest].sys_MenuNode ON [cTest].sys_MenuNodesInRoles.MenuNodeId = [cTest].sys_MenuNode.Id
GO

DROP VIEW IF EXISTS [cTest].[V_RolePermissions]
GO
CREATE VIEW [cTest].[V_RolePermissions]
AS
SELECT   [cTest].sys_PermissionsInRoles.RoleId, [cTest].sys_Role.Name AS RoleName, [cTest].sys_Role.DisplayName, 
[cTest].sys_Role.Description, [cTest].sys_Role.IsSystemRole, [cTest].sys_PermissionsInRoles.PermissionId, 
[cTest].sys_Permission.ParentId, [cTest].sys_Permission.Name AS PermissionName, [cTest].sys_Permission.AreaName, 
[cTest].sys_Permission.ControllerName, [cTest].sys_Permission.ActionName, [cTest].sys_Permission.Parameters, 
[cTest].sys_Permission.ResultType, [cTest].sys_Permission.TreeCode, [cTest].sys_Permission.Leaf, 
[cTest].sys_Permission.[Level], [cTest].sys_Permission.[Index], [cTest].sys_Permission.ApplicationId, 
[cTest].sys_Permission.ApplicationName
FROM      [cTest].sys_Role INNER JOIN
[cTest].sys_PermissionsInRoles ON [cTest].sys_Role.Id = [cTest].sys_PermissionsInRoles.RoleId INNER JOIN
[cTest].sys_Permission ON [cTest].sys_PermissionsInRoles.PermissionId = [cTest].sys_Permission.Id
GO

DROP VIEW IF EXISTS [cTest].[V_UserMenuNodes]
GO
CREATE VIEW [cTest].[V_UserMenuNodes]
AS
SELECT   [cTest].sys_UsersInRoles.UserId, [cTest].sys_User.UserName, [cTest].sys_User.Email, [cTest].sys_User.PhoneNumber, 
[cTest].sys_User.PositionLevel, [cTest].sys_User.MemberId, [cTest].sys_User.DisplayName, [cTest].sys_User.CreateDate, 
[cTest].sys_User.ContactQQ, [cTest].sys_User.Telephone, [cTest].sys_User.OpenId, [cTest].sys_User.ReferenceId1, [cTest].sys_User.ReferenceId2, [cTest].sys_User.ReferenceId3, 
[cTest].sys_User.Status, [cTest].sys_User.IsModifyPassword, [cTest].sys_UsersInRoles.RoleId, 
[cTest].sys_Role.Name AS RoleName, [cTest].sys_Role.DisplayName AS RoleDisplayName, 
[cTest].sys_Role.IsSystemRole, [cTest].sys_MenuNodesInRoles.MenuNodeId, [cTest].sys_MenuNode.ParentId, 
[cTest].sys_MenuNode.Name AS MenuNodeName, [cTest].sys_MenuNode.AreaName, 
[cTest].sys_MenuNode.ControllerName, [cTest].sys_MenuNode.ActionName, [cTest].sys_MenuNode.Parameters, 
[cTest].sys_MenuNode.SmallIcon, [cTest].sys_MenuNode.BigIcon, [cTest].sys_MenuNode.Version, 
[cTest].sys_MenuNode.TenantType, [cTest].sys_MenuNode.IsExtPage, [cTest].sys_MenuNode.TreeCode, 
[cTest].sys_MenuNode.Leaf, [cTest].sys_MenuNode.[Level], [cTest].sys_MenuNode.[Index], 
[cTest].sys_MenuNode.ApplicationId, [cTest].sys_MenuNode.ApplicationName
FROM      [cTest].sys_User INNER JOIN
[cTest].sys_UsersInRoles ON [cTest].sys_User.Id = [cTest].sys_UsersInRoles.UserId INNER JOIN
[cTest].sys_Role ON [cTest].sys_UsersInRoles.RoleId = [cTest].sys_Role.Id INNER JOIN
[cTest].sys_MenuNodesInRoles ON [cTest].sys_Role.Id = [cTest].sys_MenuNodesInRoles.RoleId INNER JOIN
[cTest].sys_MenuNode ON [cTest].sys_MenuNodesInRoles.MenuNodeId = [cTest].sys_MenuNode.Id
GO

DROP VIEW IF EXISTS [cTest].[V_UserPermissions]
GO
CREATE VIEW [cTest].[V_UserPermissions]
AS
SELECT   [cTest].sys_UsersInRoles.UserId, [cTest].sys_User.UserName, [cTest].sys_User.Email, [cTest].sys_User.PhoneNumber, 
[cTest].sys_User.DisplayName, [cTest].sys_User.MemberId, [cTest].sys_User.PositionLevel, [cTest].sys_User.CreateDate, 
[cTest].sys_User.Recommended, [cTest].sys_User.ContactQQ, [cTest].sys_User.Telephone, [cTest].sys_User.OpenId, 
[cTest].sys_User.ReferenceId1, [cTest].sys_User.ReferenceId2, [cTest].sys_User.ReferenceId3, [cTest].sys_User.Status, [cTest].sys_User.IsModifyPassword, [cTest].sys_UsersInRoles.RoleId, 
[cTest].sys_Role.Name AS RoleName, [cTest].sys_Role.DisplayName AS RoleDisplayName, 
[cTest].sys_Role.IsSystemRole, [cTest].sys_PermissionsInRoles.PermissionId, [cTest].sys_Permission.ParentId, 
[cTest].sys_Permission.Name AS PermissionName, [cTest].sys_Permission.AreaName, 
[cTest].sys_Permission.ControllerName, [cTest].sys_Permission.ActionName, [cTest].sys_Permission.Parameters, 
[cTest].sys_Permission.ResultType, [cTest].sys_Permission.TreeCode, [cTest].sys_Permission.Leaf, 
[cTest].sys_Permission.[Level], [cTest].sys_Permission.[Index], [cTest].sys_Permission.ApplicationId, 
[cTest].sys_Permission.ApplicationName
FROM      [cTest].sys_UsersInRoles INNER JOIN
[cTest].sys_Role ON [cTest].sys_UsersInRoles.RoleId = [cTest].sys_Role.Id INNER JOIN
[cTest].sys_PermissionsInRoles ON [cTest].sys_Role.Id = [cTest].sys_PermissionsInRoles.RoleId INNER JOIN
[cTest].sys_Permission ON [cTest].sys_PermissionsInRoles.PermissionId = [cTest].sys_Permission.Id INNER JOIN
[cTest].sys_User ON [cTest].sys_UsersInRoles.UserId = [cTest].sys_User.Id
GO
