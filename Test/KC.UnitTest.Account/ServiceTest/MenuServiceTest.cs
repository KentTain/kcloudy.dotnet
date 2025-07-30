using System;
using System.IO;
using System.Linq;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Account;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using KC.Service.DTO.Account;
using System.Collections.Generic;

namespace KC.UnitTest.Account
{
    
    public class MenuServiceTest : AccountTestBase
    {
        private ILogger _logger;
        public MenuServiceTest(CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(MenuServiceTest));
        }

        [Xunit.Fact]
        public async Task Test_SaveMenusAsync()
        {
            #region jsonData
            var jsonObj = @"[{'$id':'1','AreaName':'','ActionName':'Customer','ControllerName':'Home','Parameters':null,'SmallIcon':'fa fa-users','BigIcon':null,'URL':null,'IsExtPage':false,'Description':'一级菜单【客户管理】','ApplicationId':'95e9e18f-0316-4c04-bedc-a8e321431c0a','ApplicationName':'客户管理','ParentName':null,'TenantType':63,'Version':7,'Parent':null,'DefaultRoleId':'0B6DE259-FBEA-401C-A112-4980AF659674','AuthorityId':'E78ED2C1-CA26-4837-B938-9592B07108A7','id':0,'text':'客户管理','ParentId':null,'TreeCode':null,'Leaf':false,'Level':1,'Index':7,'children':[{'$id':'2','AreaName':'','ActionName':'CustomerInfo','ControllerName':'Home','Parameters':null,'SmallIcon':'fa fa-user-circle','BigIcon':null,'URL':null,'IsExtPage':false,'Description':'一级菜单【客户管理】下的二级菜单【我的客户】','ApplicationId':'95e9e18f-0316-4c04-bedc-a8e321431c0a','ApplicationName':'客户管理','ParentName':null,'TenantType':63,'Version':7,'Parent':{'$ref':'1'},'DefaultRoleId':'0B6DE259-FBEA-401C-A112-4980AF659674','AuthorityId':'F44C4007-A301-483A-99ED-B5E04D478D1D','id':0,'text':'我的客户','ParentId':null,'TreeCode':null,'Leaf':false,'Level':2,'Index':1,'children':[{'$id':'3','AreaName':'','ActionName':'Index','ControllerName':'CustomerInfo','Parameters':null,'SmallIcon':'fa fa-user-circle','BigIcon':null,'URL':null,'IsExtPage':false,'Description':'二级菜单【我的客户】下的三级菜单【我的客户】','ApplicationId':'95e9e18f-0316-4c04-bedc-a8e321431c0a','ApplicationName':'客户管理','ParentName':null,'TenantType':63,'Version':7,'Parent':{'$ref':'2'},'DefaultRoleId':'0B6DE259-FBEA-401C-A112-4980AF659674','AuthorityId':'6DDD2C07-9224-4CDF-86D4-B3A072E5540B','id':0,'text':'我的客户','ParentId':null,'TreeCode':null,'Leaf':true,'Level':3,'Index':1,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-08-25T01:53:09.6207258Z','ModifiedBy':null,'ModifiedDate':'2019-08-25T01:53:09.6207258Z'},{'$id':'4','AreaName':'','ActionName':'CustomerDetailInfo','ControllerName':'CustomerInfo','Parameters':null,'SmallIcon':'fa fa-user','BigIcon':null,'URL':null,'IsExtPage':true,'Description':'二级菜单【我的客户】下的三级菜单【客户详情】','ApplicationId':'95e9e18f-0316-4c04-bedc-a8e321431c0a','ApplicationName':'客户管理','ParentName':null,'TenantType':63,'Version':7,'Parent':{'$ref':'2'},'DefaultRoleId':'0B6DE259-FBEA-401C-A112-4980AF659674','AuthorityId':'81A3E5AB-30B8-4A0A-B393-7E646E674BB1','id':0,'text':'客户详情','ParentId':null,'TreeCode':null,'Leaf':true,'Level':3,'Index':2,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-08-25T01:53:09.6217906Z','ModifiedBy':null,'ModifiedDate':'2019-08-25T01:53:09.6217906Z'},{'$id':'5','AreaName':'','ActionName':'GetCustomerContact','ControllerName':'CustomerInfo','Parameters':null,'SmallIcon':'fa fa-user','BigIcon':null,'URL':null,'IsExtPage':true,'Description':'二级菜单【我的客户】下的三级菜单【客户联系人】','ApplicationId':'95e9e18f-0316-4c04-bedc-a8e321431c0a','ApplicationName':'客户管理','ParentName':null,'TenantType':63,'Version':7,'Parent':{'$ref':'2'},'DefaultRoleId':'0B6DE259-FBEA-401C-A112-4980AF659674','AuthorityId':'AAC43DAB-1A8B-47FC-9106-E242518732E7','id':0,'text':'客户联系人','ParentId':null,'TreeCode':null,'Leaf':true,'Level':3,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-08-25T01:53:09.6228463Z','ModifiedBy':null,'ModifiedDate':'2019-08-25T01:53:09.6228463Z'}],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-08-25T01:53:09.6146072Z','ModifiedBy':null,'ModifiedDate':'2019-08-25T01:53:09.6146072Z'},{'$id':'6','AreaName':'','ActionName':'CustomerSea','ControllerName':'Home','Parameters':null,'SmallIcon':'fa fa-ship','BigIcon':null,'URL':null,'IsExtPage':false,'Description':'一级菜单【客户管理】下的二级菜单【客户公海】','ApplicationId':'95e9e18f-0316-4c04-bedc-a8e321431c0a','ApplicationName':'客户管理','ParentName':null,'TenantType':63,'Version':7,'Parent':{'$ref':'1'},'DefaultRoleId':'0B6DE259-FBEA-401C-A112-4980AF659674','AuthorityId':'7D78D062-9E7D-4859-BB72-B6E01785814B','id':0,'text':'客户公海','ParentId':null,'TreeCode':null,'Leaf':false,'Level':2,'Index':2,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-08-25T01:53:09.6159373Z','ModifiedBy':null,'ModifiedDate':'2019-08-25T01:53:09.6159373Z'},{'$id':'7','AreaName':'','ActionName':'CustomerNotification','ControllerName':'Home','Parameters':null,'SmallIcon':'fa fa-shopping-bag','BigIcon':null,'URL':null,'IsExtPage':false,'Description':'一级菜单【客户管理】下的二级菜单【客户营销】','ApplicationId':'95e9e18f-0316-4c04-bedc-a8e321431c0a','ApplicationName':'客户管理','ParentName':null,'TenantType':63,'Version':7,'Parent':{'$ref':'1'},'DefaultRoleId':'0B6DE259-FBEA-401C-A112-4980AF659674','AuthorityId':'53491CFC-8479-4807-881A-635D64056830','id':0,'text':'客户营销','ParentId':null,'TreeCode':null,'Leaf':false,'Level':2,'Index':3,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-08-25T01:53:09.6171256Z','ModifiedBy':null,'ModifiedDate':'2019-08-25T01:53:09.6171256Z'},{'$id':'8','AreaName':'','ActionName':'SendCustomerLog','ControllerName':'Home','Parameters':null,'SmallIcon':'fa fa-file-text','BigIcon':null,'URL':null,'IsExtPage':false,'Description':'一级菜单【客户管理】下的二级菜单【推送日志】','ApplicationId':'95e9e18f-0316-4c04-bedc-a8e321431c0a','ApplicationName':'客户管理','ParentName':null,'TenantType':63,'Version':7,'Parent':{'$ref':'1'},'DefaultRoleId':'0B6DE259-FBEA-401C-A112-4980AF659674','AuthorityId':'71512395-BBE0-4242-8B5F-CE997DD350F9','id':0,'text':'推送日志','ParentId':null,'TreeCode':null,'Leaf':false,'Level':2,'Index':4,'children':[],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-08-25T01:53:09.6181511Z','ModifiedBy':null,'ModifiedDate':'2019-08-25T01:53:09.6181515Z'}],'checked':false,'IsDeleted':false,'CreatedBy':null,'CreatedDate':'2019-08-25T01:53:09.6071367Z','ModifiedBy':null,'ModifiedDate':'2019-08-25T01:53:09.6075656Z'}] ";
            #endregion 

            InjectTenant(TestTenant);

            var menuService = ServiceProvider.GetService<IMenuService>();

            var menus = Common.SerializeHelper.FromJson<List<MenuNodeDTO>>(jsonObj);
            var result = await menuService.SaveMenusAsync(menus, new Guid("95e9e18f-0316-4c04-bedc-a8e321431c0a"));
            Assert.True(result);
        }

    }
}
