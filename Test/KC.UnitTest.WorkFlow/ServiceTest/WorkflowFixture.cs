using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Service.DTO.Account;
using KC.Service.DTO.Workflow;
using KC.Service.Enums.Workflow;
using KC.Service.WebApiService.Business;
using KC.Service.Workflow;
using KC.Service.Workflow.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KC.UnitTest.Workflow
{
    /// <summary>
    /// 测试前初始化的测试数据</br>
    ///     所有测试单元启动前只初始化一次数据，所有测试单元结束后进行删除
    /// </summary>
    public class WorkflowFixture : CommonFixture
    {
        #region 表单字段值，未设置流程定义ID：WorkflowDefId
        /// <summary>
        /// 表单字段（field1）：int类型，主键
        /// </summary>
        public static WorkflowDefFieldDTO field1_int = new WorkflowDefFieldDTO()
        {
            Id = 0,
            ParentId = null,
            Text = "field-1",
            DataType = AttributeDataType.Int,
            Value = new Random().Next(10000).ToString(),
            DisplayName = "编号【测试】",
            Description = "编号【测试】：int类型，主键",
            Leaf = true,
            Level = 1,
            CanEdit = false,
            IsPrimaryKey = true,
            IsCondition = false,
            IsExecutor = false,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
        };
        /// <summary>
        /// 表单字段（field2）：string类型，执行人Id
        /// </summary>
        public static WorkflowDefFieldDTO field2_string = new WorkflowDefFieldDTO()
        {
            Id = 0,
            ParentId = null,
            Text = "field-2",
            DataType = AttributeDataType.String,
            Value = RoleConstants.AdminUserId,
            DisplayName = "执行人【测试】",
            Description = "执行人【测试】：string类型，执行人Id",
            Leaf = true,
            Level = 1,
            CanEdit = false,
            IsPrimaryKey = false,
            IsCondition = false,
            IsExecutor = true,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
        };
        /// <summary>
        /// 表单字段（field3）：currancy类型，判断条件-金额
        /// </summary>
        public static WorkflowDefFieldDTO field3_currency = new WorkflowDefFieldDTO()
        {
            Id = 0,
            ParentId = null,
            Text = "field-3",
            DataType = AttributeDataType.Decimal,
            Value = "15.26",    //条件：node5【field-3>=20】 node6【field-3>=40 && field-5=true】
            DisplayName = "金额【测试】",
            Description = "金额【测试】：currancy类型，判断条件-金额；条件：node5【field-3>=20】 node6【field-3>=40 && field-5=true】",
            Leaf = true,
            Level = 1,
            CanEdit = false,
            IsPrimaryKey = false,
            IsCondition = true,
            IsExecutor = false,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
        };
        /// <summary>
        /// 表单字段（field4）：datetime类型，创建日期
        /// </summary>
        public static WorkflowDefFieldDTO field4_datetime = new WorkflowDefFieldDTO()
        {
            Id = 0,
            ParentId = null,
            Text = "field-4",
            DataType = AttributeDataType.DateTime,
            Value = "2021-01-02",
            DisplayName = "创建日期【测试】",
            Description = "创建日期【测试】：datetime类型，创建日期",
            Leaf = true,
            Level = 1,
            CanEdit = false,
            IsPrimaryKey = false,
            IsCondition = false,
            IsExecutor = false,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
        };
        /// <summary>
        /// 表单字段（field5）：bool类型，判断条件-是否上级主管审批
        /// </summary>
        public static WorkflowDefFieldDTO field5_bool = new WorkflowDefFieldDTO()
        {
            Id = 0,
            ParentId = null,
            Text = "field-5",
            DataType = AttributeDataType.Bool,
            Value = "false",    //条件：node6【field-3>=40 && field-5=true】
            DisplayName = "是否借款【测试】",
            Description = "是否借款【测试】：bool类型，是否借款；条件：node6【field-3>=40 && field-5=true",
            Leaf = true,
            Level = 1,
            CanEdit = false,
            IsPrimaryKey = false,
            IsCondition = true,
            IsExecutor = false,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
        };
        /// <summary>
        /// 表单字段（field6）：list类型，明细列表
        /// </summary>
        public static WorkflowDefFieldDTO field6_list = new WorkflowDefFieldDTO()
        {
            Id = 0,
            ParentId = null,
            Text = "field-6",
            DataType = AttributeDataType.List,
            Value = "[{'field-6-1':1,'field-6-2':'test-1','field-6-3':'2021-01-31'}," +
                    " {'field-6-1':2,'field-6-2':'test-2','field-6-3':'2021-02-31'}," +
                    " {'field-6-1':3,'field-6-2':'test-3','field-6-3':'2021-03-31'}]",
            DisplayName = "费用明细【测试】",
            Description = "费用明细【测试】：list类型，明细列表",
            Leaf = false,
            Level = 1,
            CanEdit = false,
            IsPrimaryKey = false,
            IsCondition = false,
            IsExecutor = false,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
            //针对类型List<Object>中Object的定义
            Children = new List<WorkflowDefFieldDTO>()
            {
                new WorkflowDefFieldDTO()
                {
                    Id = 0,
                    Text = "field-6-1",
                    DataType = AttributeDataType.Int,
                    DisplayName = "编号【测试】",
                    Description = "编号【测试】：int类型，列表主键",
                    Leaf = true,
                    Level = 2,
                    CanEdit = false,
                    IsPrimaryKey = true,
                    IsCondition = false,
                    IsExecutor = false,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                },
                new WorkflowDefFieldDTO()
                {
                    Id = 0,
                    Text = "field-6-2",
                    DataType = AttributeDataType.String,
                    DisplayName = "费用名称【测试】",
                    Description = "费用名称【测试】：string类型",
                    Leaf = true,
                    Level = 2,
                    CanEdit = false,
                    IsPrimaryKey = false,
                    IsCondition = false,
                    IsExecutor = false,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                },
                new WorkflowDefFieldDTO()
                {
                    Id = 0,
                    Text = "field-6-3",
                    DataType = AttributeDataType.DateTime,
                    DisplayName = "费用发生日期【测试】",
                    Description = "费用发生日期【测试】：datetime类型",
                    Leaf = true,
                    Level = 2,
                    CanEdit = false,
                    IsPrimaryKey = false,
                    IsCondition = false,
                    IsExecutor = false,
                    IsDeleted = false,
                    CreatedBy = RoleConstants.AdminUserId,
                    CreatedName = RoleConstants.AdminUserName,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedBy = RoleConstants.AdminUserId,
                    ModifiedName = RoleConstants.AdminUserName,
                    ModifiedDate = DateTime.UtcNow,
                },
                //new WorkflowDefFieldDTO()
                //{
                //    Id = 0,
                //    Text = "field-6-4",
                //    DataType = AttributeDataType.String,
                //    DisplayName = "备注-6-4",
                //    Description = "备注-6-4：String类型，备注说明",
                //    Leaf = true,
                //    Level = 2,
                //    CanEdit = false,
                //    IsPrimaryKey = true,
                //    IsCondition = false,
                //    IsExecutor = false,
                //    IsDeleted = false,
                //    CreatedBy = RoleConstants.AdminUserId,
                //    CreatedName = RoleConstants.AdminUserName,
                //    CreatedDate = DateTime.UtcNow,
                //    ModifiedBy = RoleConstants.AdminUserId,
                //    ModifiedName = RoleConstants.AdminUserName,
                //    ModifiedDate = DateTime.UtcNow,
                //},
                //new WorkflowDefFieldDTO()
                //{
                //    Id = 0,
                //    Text = "field-6-5",
                //    DataType = AttributeDataType.Bool,
                //    DisplayName = "是否借款-6-5",
                //    Description = "是否借款-6-5：bool类型，是否上级主管审批",
                //    Leaf = true,
                //    Level = 2,
                //    CanEdit = false,
                //    IsPrimaryKey = true,
                //    IsCondition = false,
                //    IsExecutor = false,
                //    IsDeleted = false,
                //    CreatedBy = RoleConstants.AdminUserId,
                //    CreatedName = RoleConstants.AdminUserName,
                //    CreatedDate = DateTime.UtcNow,
                //    ModifiedBy = RoleConstants.AdminUserId,
                //    ModifiedName = RoleConstants.AdminUserName,
                //    ModifiedDate = DateTime.UtcNow,
                //}
            }
        };
        #endregion

        /// <summary>
        /// 组织架构： </br>
        ///  ----企业（主管：齐总经理(角色：总经理)、员工：小齐(角色：总经理助理)）</br>
        ///      ----销售部 </br>
        ///          ----分管经理： 齐副经理（角色：销售经理、采购经理） </br>
        ///          ----主管：     肖经理(角色：销售经理)  </br>
        ///          ----员工：     小肖(角色：销售助理)） </br>
        ///      ----采购部 </br>
        ///          ----分管经理： 齐副经理（角色：销售经理、采购经理） </br>
        ///          ----主管：     蔡经理(角色：采购经理)、 </br>
        ///          ----员工：     小蔡(角色：采购助理)） </br>
        ///      ----人事部 </br>
        ///          ----主管：     任经理(角色：人事经理) </br>
        ///          ----员工：     小任(角色：人事助理)）
        ///      ----IT部 </br>
        ///          ----主管：     季经理(角色：系统管理员) </br>
        ///          ----员工：     小季(角色：系统管理员)）
        /// </summary>
        #region 流程任务(NodeType=Task)中的审核人设置数据
        #region 企业：齐总经理、齐副经理、小齐
        //一级组织：企业
        public const int wfComDeptId = OrganizationConstants.企业_Id;
        public const string wfComDeptCode = OrganizationConstants.企业_Code;

        public const string wfComRoleId = RoleConstants.GmAssistantRoleId;
        public const string wfComManRoleId = RoleConstants.GeneralManagerRoleId;

        /// <summary>
        /// 齐总经理：总经理（企业）
        /// </summary>
        public const string wfComAdminUserId = "FD4A2460-A18B-4EA9-8933-597FAA02EA0C";
        public const string wfComAdminUserName = "齐总经理";
        public static List<string> wfComAdminUser_RoleIds = new List<string>() { wfComManRoleId };
        public static List<int> wfComAdminUser_OrgIds = new List<int>() { wfComDeptId };
        public static List<string> wfComAdminUser_OrgCodes = new List<string>() { wfComDeptCode };
        /// <summary>
        /// 齐副经理：分管经理（销售部、采购部）
        /// </summary>
        public const string wfComSubAdminUserId = "DD5E4F06-B3D2-4ADA-9EF5-35CC168C54D0";
        public const string wfComSubAdminUserName = "齐副经理";
        public static List<string> wfComSubAdminUser_RoleIds = new List<string>() { wfSaleManRoleId, wfBuyManRoleId };
        public static List<int> wfComSubAdminUser_OrgIds = new List<int>() { wfSaleDeptId, wfBuyDeptId };
        public static List<string> wfComSubAdminUser_OrgCodes = new List<string>() { wfSaleDeptCode, wfBuyDeptCode };
        /// <summary>
        /// 小齐：总经理助理（企业）
        /// </summary>
        public const string wfComTestUserId = "37F7E998-A232-436B-BC57-3550AEE97ABB";
        public const string wfComTestUserName = "小齐";
        public static List<string> wfComTestUser_RoleIds = new List<string>() { wfComRoleId };
        public static List<int> wfComTestUser_OrgIds = new List<int>() { wfComDeptId };
        public static List<string> wfComTestUser_OrgCodes = new List<string>() { wfComDeptCode };
        #endregion

        #region 销售部：齐副经理、肖经理、小肖

        //二级级组织：销售部
        public const int wfSaleDeptId = OrganizationConstants.销售部_Id;
        public const string wfSaleDeptCode = OrganizationConstants.销售部_Code;

        public const string wfSaleRoleId = RoleConstants.SaleRoleId;
        public const string wfSaleManRoleId = RoleConstants.SaleManagerRoleId;

        /// <summary>
        /// 肖经理：销售经理（销售部）
        /// </summary>
        public const string wfSaleAdminUserId = "70072027-F68A-4729-9A36-A068A56A0173";
        public const string wfSaleAdminUserName = "肖经理";
        public static List<string> wfSaleAdminUser_RoleIds = new List<string>() { wfSaleManRoleId };
        public static List<int> wfSaleAdminUser_OrgIds = new List<int>() { wfSaleDeptId };
        public static List<string> wfSaleAdminUser_OrgCodes = new List<string>() { wfSaleDeptCode };
        /// <summary>
        /// 小肖：销售助理（销售部）
        /// </summary>
        public const string wfSaleTestUserId = "A3D0B891-2C04-4E85-81E5-9BEB7452D172";
        public const string wfSaleTestUserName = "小肖";
        public static List<string> wfSaleTestUser_RoleIds = new List<string>() { wfSaleRoleId };
        public static List<int> wfSaleTestUser_OrgIds = new List<int>() { wfSaleDeptId };
        public static List<string> wfSaleTestUser_OrgCodes = new List<string>() { wfSaleDeptCode };
        #endregion

        #region 采购部：齐副经理、蔡经理、小蔡
        //二级级组织：采购部
        public const int wfBuyDeptId = OrganizationConstants.采购部_Id;
        public const string wfBuyDeptCode = OrganizationConstants.采购部_Code;

        public const string wfBuyRoleId = RoleConstants.PurchaseRoleId;
        public const string wfBuyManRoleId = RoleConstants.PurchaseManagerRoleId;

        /// <summary>
        /// 蔡经理：采购经理（销售部）
        /// </summary>
        public const string wfBuyAdminUserId = "E393915D-E4B5-4A05-A84C-88ABFF58CE19";
        public const string wfBuyAdminUserName = "蔡经理";
        public static List<string> wfBuyAdminUser_RoleIds = new List<string>() { wfBuyManRoleId };
        public static List<int> wfBuyAdminUser_OrgIds = new List<int>() { wfBuyDeptId };
        public static List<string> wfBuyAdminUser_OrgCodes = new List<string>() { wfBuyDeptCode };
        /// <summary>
        /// 小蔡：采购助理（销售部）
        /// </summary>
        public const string wfBuyTestUserId = "0A9836BF-30A1-46E1-B27A-2E8AAC06D305";
        public const string wfBuyTestUserName = "小蔡";
        public static List<string> wfBuyTestUser_RoleIds = new List<string>() { wfBuyRoleId };
        public static List<int> wfBuyTestUser_OrgIds = new List<int>() { wfBuyDeptId };
        public static List<string> wfBuyTestUser_OrgCodes = new List<string>() { wfBuyDeptCode };
        #endregion

        #region 人事部：任经理、小任
        //二级级组织：IT部
        public const int wfHrDeptId = OrganizationConstants.人事部_Id;
        public const string wfHrDeptCode = OrganizationConstants.人事部_Code;

        public const string wfHrManRoleId = RoleConstants.HrManagerRoleId;
        public const string wfHrRoleId = RoleConstants.HrRoleId;

        /// <summary>
        /// 任经理：人事经理（人事部）
        /// </summary>
        public const string wfHrAdminUserId = "53DC1661-4392-4AF6-81D1-6C559F65921D";
        public const string wfHrAdminUserName = "任经理";
        public static List<string> wfHrAdminUser_RoleIds = new List<string>() { wfHrManRoleId };
        public static List<int> wfHrAdminUser_OrgIds = new List<int>() { wfHrDeptId };
        public static List<string> wfHrAdminUser_OrgCodes = new List<string>() { wfHrDeptCode };
        /// <summary>
        /// 小任：人事助理（人事部）
        /// </summary>
        public const string wfHrTestUserId = "D85A6760-A310-44EE-B6A3-3DB95EA22463";
        public const string wfHrTestUserName = "小任";
        public static List<string> wfHrTestUser_RoleIds = new List<string>() { wfHrManRoleId };
        public static List<int> wfHrTestUser_OrgIds = new List<int>() { wfHrDeptId };
        public static List<string> wfHrTestUser_OrgCodes = new List<string>() { wfHrRoleId };
        #endregion

        #region IT部：季经理、小季
        //二级级组织：IT部
        public const int wfItDeptId = OrganizationConstants.IT部_Id;
        public const string wfItDeptCode = OrganizationConstants.IT部_Code;

        public const string wfItManRoleId = RoleConstants.AdminRoleId;

        /// <summary>
        /// 季经理：系统管理员（IT部）
        /// </summary>
        public const string wfItAdminUserId = "7FD0C40E-4F2E-4FEB-922B-B7A4487DE875";
        public const string wfItAdminUserName = "季经理";
        public static List<string> wfItAdminUser_RoleIds = new List<string>() { wfItManRoleId };
        public static List<int> wfItAdminUser_OrgIds = new List<int>() { wfItDeptId };
        public static List<string> wfItAdminUser_OrgCodes = new List<string>() { wfItDeptCode };
        /// <summary>
        /// 小季：系统管理员（IT部）
        /// </summary>
        public const string wfItTestUserId = "59E88BB5-C1F0-4899-89B7-A32A0714B447";
        public const string wfItTestUserName = "小季";
        public static List<string> wfItTestUser_RoleIds = new List<string>() { wfItManRoleId };
        public static List<int> wfItTestUser_OrgIds = new List<int>() { wfItDeptId };
        public static List<string> wfItTestUser_OrgCodes = new List<string>() { wfItDeptCode };
        #endregion

        //审批人设置：组织/角色/用户设置
        public const string node3_ExecuteOrgCodes = wfComDeptCode + "," + wfSaleDeptCode;
        public const string node3_ExecuteOrgNames = "企业, 销售部";
        public const string node3_ExecuteRoleIds = wfComManRoleId + "," + wfSaleManRoleId;
        public const string node3_ExecuteRoleNames = "总经理, 销售经理";
        public const string node3_ExecuteUserIds = wfSaleAdminUserId;
        public const string node3_ExecuteUserNames = wfSaleAdminUserName;
        #endregion

        #region 流程节点数据（Node），未设置节点值：WorkflowDefId、PrevNodeCode、NextNodeCode、ReturnNodeCode

        /// <summary>
        /// 开始节点(node1)
        /// </summary>
        public static WorkflowDefNodeDTO node1_start = new WorkflowDefNodeDTO()
        {
            Id = Guid.NewGuid(),
            Code = "wfn2012020200001",
            Name = "node1-start",
            NodeType = WorkflowNodeType.Start,
            Type = WorkflowType.SingleLine,
            LocTop = 120,
            LocLeft = 240,
            ExecutorSetting = ExecutorSetting.Executor,
            OrgCodes = null,
            OrgNames = null,
            RoleIds = null,
            RoleNames = null,
            UserIds = null,
            UserNames = null,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
        };
        /// <summary>
        /// 任务节点(node2)：单人审批任务 </br>
        ///     执行人【CreatorManager：发起人的主管--销售经理:齐副经理, 肖经理】
        /// </summary>
        public static WorkflowDefNodeDTO node2_task = new WorkflowDefNodeDTO()
        {
            Id = Guid.NewGuid(),
            Code = "wfn2012020200002",
            Name = "node2-task",
            Type = WorkflowType.SingleLine,
            NodeType = WorkflowNodeType.Task,
            LocTop = 240,
            LocLeft = 210,
            ExecutorSetting = ExecutorSetting.CreatorManager,
            OrgCodes = null,
            OrgNames = null,
            RoleIds = null,
            RoleNames = null,
            UserIds = null,
            UserNames = null,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
        };
        /// <summary>
        /// 任务节点(node3)：多人审批(权重50%通过)任务 </br>
        ///     执行人【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        /// </summary>
        public static WorkflowDefNodeDTO node3_task = new WorkflowDefNodeDTO()
        {
            Id = Guid.NewGuid(),
            Code = "wfn2012020200003",
            Name = "node3-task",
            Type = WorkflowType.WeightLine,
            WeightValue = 0.5M,
            NodeType = WorkflowNodeType.Task,
            LocTop = 360,
            LocLeft = 210,
            ExecutorSetting = ExecutorSetting.Executor,
            OrgCodes = node3_ExecuteOrgCodes,
            OrgNames = node3_ExecuteOrgNames,
            RoleIds = node3_ExecuteRoleIds,
            RoleNames = node3_ExecuteRoleNames,
            UserIds = node3_ExecuteUserIds,
            UserNames = node3_ExecuteUserNames,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
        };
        /// <summary>
        /// 任务节点(node4)：权重审批任务 </br>
        ///     执行人【SubmitterSuperior：提交审批人的主管--肖经理】
        /// </summary>
        public static WorkflowDefNodeDTO node4_task = new WorkflowDefNodeDTO()
        {
            Id = Guid.NewGuid(),
            Code = "wfn2012020200004",
            Name = "node4-task",
            Type = WorkflowType.SingleLine,
            NodeType = WorkflowNodeType.Task,
            LocTop = 480,
            LocLeft = 210,
            ExecutorSetting = ExecutorSetting.SubmitterSuperior,
            OrgCodes = null,
            OrgNames = null,
            RoleIds = null,
            RoleNames = null,
            UserIds = null,
            UserNames = null,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
        };
        /// <summary>
        /// 条件节点(node5)：执行条件【field3>=20】
        /// </summary>
        public static WorkflowDefNodeDTO node5_condition = new WorkflowDefNodeDTO()
        {
            Id = Guid.NewGuid(),
            Code = "wfn2012020200005",
            Name = "node5-condition",
            Type = WorkflowType.SingleLine,
            NodeType = WorkflowNodeType.Condition,
            LocTop = 600,
            LocLeft = 240,
            ExecutorSetting = ExecutorSetting.Executor,
            OrgCodes = null,
            OrgNames = null,
            RoleIds = null,
            RoleNames = null,
            UserIds = null,
            UserNames = null,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
            Rules = new List<WorkflowDefNodeRuleDTO>()
                {
                    new WorkflowDefNodeRuleDTO()
                    {
                        RuleType = RuleType.And,
                        FieldName = field3_currency.Text, //金额
                        OperatorType = RuleOperatorType.GreaterThanAndEqual,
                        FieldValue = "20",
                    },
                }
        };
        /// <summary>
        /// 条件节点(node6)：执行条件【field5=true 并且 field3>=40】
        /// </summary>
        public static WorkflowDefNodeDTO node6_condition = new WorkflowDefNodeDTO()
        {
            Id = Guid.NewGuid(),
            Code = "wfn2012020200006",
            Name = "node6-condition",
            Type = WorkflowType.SingleLine,
            NodeType = WorkflowNodeType.Condition,
            LocTop = 720,
            LocLeft = 240,
            ExecutorSetting = ExecutorSetting.CreatorManager,
            OrgCodes = null,
            OrgNames = null,
            RoleIds = null,
            RoleNames = null,
            UserIds = null,
            UserNames = null,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
            Rules = new List<WorkflowDefNodeRuleDTO>()
                {
                    new WorkflowDefNodeRuleDTO()
                    {
                        RuleType = RuleType.None,
                        FieldName = field5_bool.Text, //是否上级主管审批
                        OperatorType = RuleOperatorType.Equal,
                        FieldValue = "true",
                    },
                    new WorkflowDefNodeRuleDTO()
                    {
                        RuleType = RuleType.And,
                        FieldName = field3_currency.Text, //金额
                        OperatorType = RuleOperatorType.GreaterThanAndEqual,
                        FieldValue = "40",
                    },
                }
        };
        /// <summary>
        /// 结束节点(node9)
        /// </summary>
        public static WorkflowDefNodeDTO node9_end = new WorkflowDefNodeDTO()
        {
            Id = Guid.NewGuid(),
            Code = "wfn2012020200009",
            Name = "node9-end",
            Type = WorkflowType.SingleLine,
            NodeType = WorkflowNodeType.End,
            LocTop = 740,
            LocLeft = 240,
            ExecutorSetting = ExecutorSetting.Executor,
            OrgCodes = null,
            OrgNames = null,
            RoleIds = null,
            RoleNames = null,
            UserIds = null,
            UserNames = null,
            IsDeleted = false,
            CreatedBy = RoleConstants.AdminUserId,
            CreatedName = RoleConstants.AdminUserName,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = RoleConstants.AdminUserId,
            ModifiedName = RoleConstants.AdminUserName,
            ModifiedDate = DateTime.UtcNow,
        };
        #endregion

        #region 流程定义（WorkflowDefinition）

        /// <summary>
        /// 单人审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node2->End:node9
        /// </summary>
        public static Guid wfDefId_1 = new Guid("641D0B9E-EDFF-400B-BFC4-0022F4E2C6E0");
        /// <summary>
        /// 流程编码：641D0B9E-EDFF-400B-BFC4-0022F4E2C6E0 </br>
        /// 单人审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node2->End:node9
        /// </summary>
        public static WorkflowDefinitionDTO WorkflowDef_1
        {
            get
            {
                var wfDefinition = InitWorkflowDef(1);
                wfDefinition.Id = wfDefId_1;

                wfDefinition.Name = "单人审批:" + wfDefinition.Name;
                //流程图：Start:node1->Task:node2->End:node9
                node1_start.WorkflowDefId = wfDefinition.Id;
                node1_start.NextNodeCode = node2_task.Code;

                node2_task.WorkflowDefId = wfDefinition.Id;
                node2_task.PrevNodeCode = node1_start.Code;
                node2_task.NextNodeCode = node9_end.Code;

                node9_end.WorkflowDefId = wfDefinition.Id;
                node9_end.PrevNodeCode = node2_task.Code;

                field1_int.WorkflowDefId = wfDefinition.Id;
                field2_string.WorkflowDefId = wfDefinition.Id;
                field3_currency.WorkflowDefId = wfDefinition.Id;
                field4_datetime.WorkflowDefId = wfDefinition.Id;
                field5_bool.WorkflowDefId = wfDefinition.Id;
                field6_list.WorkflowDefId = wfDefinition.Id;
                field6_list.Children.ForEach(m => m.WorkflowDefId = wfDefinition.Id);

                wfDefinition.WorkflowNodes = new List<WorkflowDefNodeDTO>()
                {
                    node1_start, node2_task, node9_end
                };
                wfDefinition.WorkflowFields = new List<WorkflowDefFieldDTO>()
                {
                    field1_int, field2_string, field3_currency, field4_datetime, field5_bool, field6_list
                };
                return wfDefinition;
            }
        }

        /// <summary>
        /// 多人审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node3->End:node9
        /// </summary>
        public static Guid wfDefId_2 = new Guid("A56B55FC-F9C9-4BD5-BAE3-ACB5CFD4BB3B");
        /// <summary>
        /// 流程编码：A56B55FC-F9C9-4BD5-BAE3-ACB5CFD4BB3B </br>
        /// 多人审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node3->End:node9
        /// </summary>
        public static WorkflowDefinitionDTO WorkflowDef_2
        {
            get
            {
                var wfDefinition = InitWorkflowDef(2);
                wfDefinition.Id = wfDefId_2;

                wfDefinition.Name = "多人权重审批:" + wfDefinition.Name;
                //流程图：Start:node1->Task:node3->End:node9
                node1_start.WorkflowDefId = wfDefinition.Id;
                node1_start.NextNodeCode = node3_task.Code;

                node3_task.WorkflowDefId = wfDefinition.Id;
                node3_task.PrevNodeCode = node1_start.Code;
                node3_task.NextNodeCode = node9_end.Code;

                node9_end.WorkflowDefId = wfDefinition.Id;
                node9_end.PrevNodeCode = node3_task.Code;

                field1_int.WorkflowDefId = wfDefinition.Id;
                field2_string.WorkflowDefId = wfDefinition.Id;
                field3_currency.WorkflowDefId = wfDefinition.Id;
                field4_datetime.WorkflowDefId = wfDefinition.Id;
                field5_bool.WorkflowDefId = wfDefinition.Id;
                field6_list.WorkflowDefId = wfDefinition.Id;
                field6_list.Children.ForEach(m => m.WorkflowDefId = wfDefinition.Id);

                wfDefinition.WorkflowNodes = new List<WorkflowDefNodeDTO>()
                {
                    node1_start, node3_task, node9_end
                };
                wfDefinition.WorkflowFields = new List<WorkflowDefFieldDTO>()
                {
                    field1_int, field2_string, field3_currency, field4_datetime, field5_bool, field6_list
                };
                return wfDefinition;
            }
        }

        /// <summary>
        /// 串联审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node3->Task:node2->End:node9
        /// </summary>
        public static Guid wfDefId_3 = new Guid("CB625CF0-C777-4FA9-91EF-F79CB13307A7");
        /// <summary>
        /// 流程编码：CB625CF0-C777-4FA9-91EF-F79CB13307A7 </br>
        /// 串联审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node3->Task:node2->End:node9
        /// </summary>
        public static WorkflowDefinitionDTO WorkflowDef_3
        {
            get
            {
                var wfDefinition = InitWorkflowDef(3);
                wfDefinition.Id = wfDefId_3;

                wfDefinition.Name = "串联审批:" + wfDefinition.Name;
                //流程图：Start:node1->Task:node3->Task:node2->End:node9
                node1_start.WorkflowDefId = wfDefinition.Id;
                node1_start.NextNodeCode = node3_task.Code;

                node3_task.WorkflowDefId = wfDefinition.Id;
                node3_task.PrevNodeCode = node1_start.Code;
                node3_task.NextNodeCode = node2_task.Code;

                node2_task.WorkflowDefId = wfDefinition.Id;
                node2_task.PrevNodeCode = node3_task.Code;
                node2_task.NextNodeCode = node9_end.Code;

                node9_end.WorkflowDefId = wfDefinition.Id;
                node9_end.PrevNodeCode = node2_task.Code;

                field1_int.WorkflowDefId = wfDefinition.Id;
                field2_string.WorkflowDefId = wfDefinition.Id;
                field3_currency.WorkflowDefId = wfDefinition.Id;
                field4_datetime.WorkflowDefId = wfDefinition.Id;
                field5_bool.WorkflowDefId = wfDefinition.Id;
                field6_list.WorkflowDefId = wfDefinition.Id;
                field6_list.Children.ForEach(m => m.WorkflowDefId = wfDefinition.Id);

                wfDefinition.WorkflowNodes = new List<WorkflowDefNodeDTO>()
                {
                    node1_start, node3_task, node2_task, node9_end
                };
                wfDefinition.WorkflowFields = new List<WorkflowDefFieldDTO>()
                {
                    field1_int, field2_string, field3_currency, field4_datetime, field5_bool, field6_list
                };
                return wfDefinition;
            }
        }

        /// <summary>
        /// 条件前置审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Condtion:node5->Task:node2->End:node9
        /// </summary>
        public static Guid wfDefId_4 = new Guid("6DA8C252-FD6F-4F01-A446-10CB690CF932");
        /// <summary>
        /// 流程编码：6DA8C252-FD6F-4F01-A446-10CB690CF932 </br>
        /// 条件前置审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Condtion:node5->Task:node2->End:node9
        /// </summary>
        public static WorkflowDefinitionDTO WorkflowDef_4
        {
            get
            {
                var wfDefinition = InitWorkflowDef(4);
                wfDefinition.Id = wfDefId_4;

                wfDefinition.Name = "条件前置审批:" + wfDefinition.Name;
                //流程图：Start:node1->Condtion:node5->Task:node2->End:node9
                node1_start.WorkflowDefId = wfDefinition.Id;
                node1_start.NextNodeCode = node5_condition.Code;

                node5_condition.WorkflowDefId = wfDefinition.Id;
                node5_condition.PrevNodeCode = node1_start.Code;
                node5_condition.NextNodeCode = node2_task.Code;
                node5_condition.ReturnNodeCode = node9_end.Code;

                node2_task.WorkflowDefId = wfDefinition.Id;
                node2_task.PrevNodeCode = node5_condition.Code;
                node2_task.NextNodeCode = node9_end.Code;

                node9_end.WorkflowDefId = wfDefinition.Id;
                node9_end.PrevNodeCode = node2_task.Code;

                field1_int.WorkflowDefId = wfDefinition.Id;
                field2_string.WorkflowDefId = wfDefinition.Id;
                field3_currency.WorkflowDefId = wfDefinition.Id;
                field4_datetime.WorkflowDefId = wfDefinition.Id;
                field5_bool.WorkflowDefId = wfDefinition.Id;
                field6_list.WorkflowDefId = wfDefinition.Id;
                field6_list.Children.ForEach(m => m.WorkflowDefId = wfDefinition.Id);

                wfDefinition.WorkflowNodes = new List<WorkflowDefNodeDTO>()
                {
                    node1_start, node5_condition, node2_task, node9_end
                };
                wfDefinition.WorkflowFields = new List<WorkflowDefFieldDTO>()
                {
                    field1_int, field2_string, field3_currency, field4_datetime, field5_bool, field6_list
                };
                return wfDefinition;
            }
        }

        /// <summary>
        /// 条件后置审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node2->Condtion:node5->End:node9
        /// </summary>
        public static Guid wfDefId_5 = new Guid("E16DBE43-9EF8-4C24-9F03-37EBF2EB16A2");
        /// <summary>
        /// 流程编码：E16DBE43-9EF8-4C24-9F03-37EBF2EB16A2 </br>
        /// 条件后置审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node2->Condtion:node5->End:node9
        /// </summary>
        public static WorkflowDefinitionDTO WorkflowDef_5
        {
            get
            {
                var wfDefinition = InitWorkflowDef(5);
                wfDefinition.Id = wfDefId_5;

                wfDefinition.Name = "条件后置审批:" + wfDefinition.Name;
                //流程图：Start:node1->Task:node2->Condtion:node5->End:node9
                node1_start.WorkflowDefId = wfDefinition.Id;
                node1_start.NextNodeCode = node2_task.Code;

                node2_task.WorkflowDefId = wfDefinition.Id;
                node2_task.PrevNodeCode = node1_start.Code;
                node2_task.NextNodeCode = node5_condition.Code;

                node5_condition.WorkflowDefId = wfDefinition.Id;
                node5_condition.PrevNodeCode = node2_task.Code;
                node5_condition.NextNodeCode = node9_end.Code;
                node5_condition.ReturnNodeCode = node1_start.Code;

                node9_end.WorkflowDefId = wfDefinition.Id;
                node9_end.PrevNodeCode = node5_condition.Code;

                field1_int.WorkflowDefId = wfDefinition.Id;
                field2_string.WorkflowDefId = wfDefinition.Id;
                field3_currency.WorkflowDefId = wfDefinition.Id;
                field4_datetime.WorkflowDefId = wfDefinition.Id;
                field5_bool.WorkflowDefId = wfDefinition.Id;
                field6_list.WorkflowDefId = wfDefinition.Id;
                field6_list.Children.ForEach(m => m.WorkflowDefId = wfDefinition.Id);

                wfDefinition.WorkflowNodes = new List<WorkflowDefNodeDTO>()
                {
                    node1_start, node2_task, node5_condition, node3_task, node9_end
                };
                wfDefinition.WorkflowFields = new List<WorkflowDefFieldDTO>()
                {
                    field1_int, field2_string, field3_currency, field4_datetime, field5_bool, field6_list
                };
                return wfDefinition;
            }
        }

        /// <summary>
        /// 条件串联审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Condtion:node5->Condtion:node6->Task:node2->End:node9
        /// </summary>
        public static Guid wfDefId_6 = new Guid("95B837AD-2F4A-4D78-B6CC-3DEBE8EA536D");
        /// <summary>
        /// 流程编码：95B837AD-2F4A-4D78-B6CC-3DEBE8EA536D </br>
        /// 条件串联审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Condtion:node5->Condtion:node6->Task:node2->End:node9
        /// </summary>
        public static WorkflowDefinitionDTO WorkflowDef_6
        {
            get
            {
                var wfDefinition = InitWorkflowDef(6);
                wfDefinition.Id = wfDefId_6;

                wfDefinition.Name = "条件串联审批:" + wfDefinition.Name;
                //流程图：Start:node1->Condtion:node5->Condtion:node6->Task:node2->End:node9
                node1_start.WorkflowDefId = wfDefinition.Id;
                node1_start.NextNodeCode = node5_condition.Code;

                node5_condition.WorkflowDefId = wfDefinition.Id;
                node5_condition.PrevNodeCode = node1_start.Code;
                node5_condition.NextNodeCode = node6_condition.Code;
                node5_condition.ReturnNodeCode = node9_end.Code;

                node6_condition.WorkflowDefId = wfDefinition.Id;
                node6_condition.PrevNodeCode = node5_condition.Code;
                node6_condition.NextNodeCode = node2_task.Code;
                node6_condition.ReturnNodeCode = node9_end.Code;

                node2_task.WorkflowDefId = wfDefinition.Id;
                node2_task.PrevNodeCode = node6_condition.Code;
                node2_task.NextNodeCode = node9_end.Code;

                node9_end.WorkflowDefId = wfDefinition.Id;
                node9_end.PrevNodeCode = node2_task.Code;

                field1_int.WorkflowDefId = wfDefinition.Id;
                field2_string.WorkflowDefId = wfDefinition.Id;
                field3_currency.WorkflowDefId = wfDefinition.Id;
                field4_datetime.WorkflowDefId = wfDefinition.Id;
                field5_bool.WorkflowDefId = wfDefinition.Id;
                field6_list.WorkflowDefId = wfDefinition.Id;
                field6_list.Children.ForEach(m => m.WorkflowDefId = wfDefinition.Id);

                wfDefinition.WorkflowNodes = new List<WorkflowDefNodeDTO>()
                {
                    node1_start, node5_condition, node6_condition, node2_task, node9_end
                };
                wfDefinition.WorkflowFields = new List<WorkflowDefFieldDTO>()
                {
                    field1_int, field2_string, field3_currency, field4_datetime, field5_bool, field6_list
                };
                return wfDefinition;
            }
        }

        /// <summary>
        /// 复杂条件前置：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Condtion:node5->Condtion:node6->Task:node2->End:node9
        /// </summary>
        public static Guid wfDefId_7 = new Guid("F3E5F535-EDDF-46D2-A9A1-F7C90DAEA9EE");
        /// <summary>
        /// 流程编码：F3E5F535-EDDF-46D2-A9A1-F7C90DAEA9EE </br>
        /// 复杂条件前置：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
        /// </summary>
        public static WorkflowDefinitionDTO WorkflowDef_7
        {
            get
            {
                var wfDefinition = InitWorkflowDef(7);
                wfDefinition.Id = wfDefId_7;

                wfDefinition.Name = "复杂条件前置:" + wfDefinition.Name;
                //流程图：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
                node1_start.WorkflowDefId = wfDefinition.Id;
                node1_start.NextNodeCode = node5_condition.Code;

                node5_condition.WorkflowDefId = wfDefinition.Id;
                node5_condition.PrevNodeCode = node1_start.Code;
                node5_condition.NextNodeCode = node2_task.Code;
                node5_condition.ReturnNodeCode = node9_end.Code;

                node2_task.WorkflowDefId = wfDefinition.Id;
                node2_task.PrevNodeCode = node5_condition.Code;
                node2_task.NextNodeCode = node6_condition.Code;

                node6_condition.WorkflowDefId = wfDefinition.Id;
                node6_condition.PrevNodeCode = node2_task.Code;
                node6_condition.NextNodeCode = node3_task.Code;
                node6_condition.ReturnNodeCode = node9_end.Code;

                node3_task.WorkflowDefId = wfDefinition.Id;
                node3_task.PrevNodeCode = node6_condition.Code;
                node3_task.NextNodeCode = node9_end.Code;

                node9_end.WorkflowDefId = wfDefinition.Id;
                node9_end.PrevNodeCode = node3_task.Code;

                field1_int.WorkflowDefId = wfDefinition.Id;
                field2_string.WorkflowDefId = wfDefinition.Id;
                field3_currency.WorkflowDefId = wfDefinition.Id;
                field4_datetime.WorkflowDefId = wfDefinition.Id;
                field5_bool.WorkflowDefId = wfDefinition.Id;
                field6_list.WorkflowDefId = wfDefinition.Id;
                field6_list.Children.ForEach(m => m.WorkflowDefId = wfDefinition.Id);

                wfDefinition.WorkflowNodes = new List<WorkflowDefNodeDTO>()
                {
                    node1_start, node5_condition, node2_task, node6_condition, node3_task, node9_end
                };
                wfDefinition.WorkflowFields = new List<WorkflowDefFieldDTO>()
                {
                    field1_int, field2_string, field3_currency, field4_datetime, field5_bool, field6_list
                };
                return wfDefinition;
            }
        }

        /// <summary>
        /// 复杂审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node2->Condtion:node5->Task:node3->Condtion:node6->End:node9
        /// </summary>
        public static Guid wfDefId_8 = new Guid("06E95126-8306-4D7A-81FB-F6FA7063E8C9");
        /// <summary>
        /// 流程编码：06E95126-8306-4D7A-81FB-F6FA7063E8C9 </br>
        /// 复杂审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Task:node2->Condtion:node5->Task:node3->Condtion:node6->End:node9
        /// </summary>
        public static WorkflowDefinitionDTO WorkflowDef_8
        {
            get
            {
                var wfDefinition = InitWorkflowDef(8);
                wfDefinition.Id = wfDefId_8;

                wfDefinition.Name = "复杂审批:" + wfDefinition.Name;
                //流程图：Start:node1->Task:node2->Condtion:node5->Task:node3->Condtion:node6->End:node9
                node1_start.WorkflowDefId = wfDefinition.Id;
                node1_start.NextNodeCode = node2_task.Code;

                node2_task.WorkflowDefId = wfDefinition.Id;
                node2_task.PrevNodeCode = node1_start.Code;
                node2_task.NextNodeCode = node5_condition.Code;

                node5_condition.WorkflowDefId = wfDefinition.Id;
                node5_condition.PrevNodeCode = node2_task.Code;
                node5_condition.NextNodeCode = node3_task.Code;
                node5_condition.ReturnNodeCode = node9_end.Code;

                node3_task.WorkflowDefId = wfDefinition.Id;
                node3_task.PrevNodeCode = node5_condition.Code;
                node3_task.NextNodeCode = node6_condition.Code;

                node6_condition.WorkflowDefId = wfDefinition.Id;
                node6_condition.PrevNodeCode = node3_task.Code;
                node6_condition.NextNodeCode = node9_end.Code;
                node6_condition.ReturnNodeCode = node2_task.Code;

                node9_end.WorkflowDefId = wfDefinition.Id;
                node9_end.PrevNodeCode = node6_condition.Code;

                field1_int.WorkflowDefId = wfDefinition.Id;
                field2_string.WorkflowDefId = wfDefinition.Id;
                field3_currency.WorkflowDefId = wfDefinition.Id;
                field4_datetime.WorkflowDefId = wfDefinition.Id;
                field5_bool.WorkflowDefId = wfDefinition.Id;
                field6_list.WorkflowDefId = wfDefinition.Id;
                field6_list.Children.ForEach(m => m.WorkflowDefId = wfDefinition.Id);

                wfDefinition.WorkflowNodes = new List<WorkflowDefNodeDTO>()
                {
                    node1_start, node5_condition, node2_task, node6_condition, node3_task, node9_end
                };
                wfDefinition.WorkflowFields = new List<WorkflowDefFieldDTO>()
                {
                    field1_int, field2_string, field3_currency, field4_datetime, field5_bool, field6_list
                };
                return wfDefinition;
            }
        }

        /// <summary>
        /// 复杂条件前置回退串联审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Condtion:node5->Condtion:node6->Task:node2->End:node9
        /// </summary>
        public static Guid wfDefId_9 = new Guid("AF3C3CAC-5AB2-4A2F-BEFA-43B263238A8F");
        /// <summary>
        /// 流程编码：AF3C3CAC-5AB2-4A2F-BEFA-43B263238A8F </br>
        /// 复杂条件前置回退串联审批：Admin有三个任务节点，Test有两个任务节点 </br>
        /// 流程定义：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
        /// </summary>
        public static WorkflowDefinitionDTO WorkflowDef_9
        {
            get
            {
                var wfDefinition = InitWorkflowDef(9);
                wfDefinition.Id = wfDefId_9;

                wfDefinition.Name = "复杂条件前置回退串联审批:" + wfDefinition.Name;
                //流程图：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
                node1_start.WorkflowDefId = wfDefinition.Id;
                node1_start.NextNodeCode = node5_condition.Code;

                node5_condition.WorkflowDefId = wfDefinition.Id;
                node5_condition.PrevNodeCode = node1_start.Code;
                node5_condition.NextNodeCode = node2_task.Code;
                node5_condition.ReturnNodeCode = node9_end.Code;

                node2_task.WorkflowDefId = wfDefinition.Id;
                node2_task.PrevNodeCode = node5_condition.Code;
                node2_task.NextNodeCode = node6_condition.Code;

                node6_condition.WorkflowDefId = wfDefinition.Id;
                node6_condition.PrevNodeCode = node2_task.Code;
                node6_condition.NextNodeCode = node3_task.Code;
                node6_condition.ReturnNodeCode = node5_condition.Code;

                node3_task.WorkflowDefId = wfDefinition.Id;
                node3_task.PrevNodeCode = node6_condition.Code;
                node3_task.NextNodeCode = node9_end.Code;

                node9_end.WorkflowDefId = wfDefinition.Id;
                node9_end.PrevNodeCode = node3_task.Code;

                field1_int.WorkflowDefId = wfDefinition.Id;
                field2_string.WorkflowDefId = wfDefinition.Id;
                field3_currency.WorkflowDefId = wfDefinition.Id;
                field4_datetime.WorkflowDefId = wfDefinition.Id;
                field5_bool.WorkflowDefId = wfDefinition.Id;
                field6_list.WorkflowDefId = wfDefinition.Id;
                field6_list.Children.ForEach(m => m.WorkflowDefId = wfDefinition.Id);

                wfDefinition.WorkflowNodes = new List<WorkflowDefNodeDTO>()
                {
                    node1_start, node5_condition, node2_task, node6_condition, node3_task, node9_end
                };
                wfDefinition.WorkflowFields = new List<WorkflowDefFieldDTO>()
                {
                    field1_int, field2_string, field3_currency, field4_datetime, field5_bool, field6_list
                };
                return wfDefinition;
            }
        }

        private static WorkflowDefinitionDTO InitWorkflowDef(int index)
        {
            var wfDefCode = "wfd201202020000" + index;
            var wfDefVersion = "wfv201202020000" + index;
            var wfDefinition = new WorkflowDefinitionDTO()
            {
                Code = wfDefCode,
                Version = wfDefVersion,
                Name = "费用报销【测试】-" + index,
                Status = WorkflowBusStatus.Approved,
                DefDeadlineInterval = 7,
                DefMessageTemplateCode = "mgt2012020200001",
                Description = "费用报销【测试】-" + index,
                SecurityType = SecurityType.None,
                IsDeleted = false,
                CreatedBy = RoleConstants.AdminUserId,
                CreatedName = RoleConstants.AdminUserName,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = RoleConstants.AdminUserId,
                ModifiedName = RoleConstants.AdminUserName,
                ModifiedDate = DateTime.UtcNow,
            };

            return wfDefinition;
        }

        #endregion

        #region 流程实例（WorkflowProcess）
        /// <summary>
        /// 流程实例0，表单数据如下：</br>
        ///     formField1, formField2, formField3, formField4, formField5, formField6
        /// </summary>
        public static List<WorkflowProFieldDTO> wfProcessForm
        {
            get
            {
                var config = KC.Service.Workflow.AutoMapper.AutoMapperConfiguration.Configure();
                var mapper = config.CreateMapper();

                var formField1_int = mapper.Map<WorkflowProFieldDTO>(field1_int);
                var formField2_string = mapper.Map<WorkflowProFieldDTO>(field2_string);
                var formField3_currency = mapper.Map<WorkflowProFieldDTO>(field3_currency);
                var formField4_datetime = mapper.Map<WorkflowProFieldDTO>(field4_datetime);
                var formField5_decimal = mapper.Map<WorkflowProFieldDTO>(field5_bool);
                var formField6_list = mapper.Map<WorkflowProFieldDTO>(field6_list);
                formField6_list.Children = new List<WorkflowProFieldDTO>()
                {
                    #region 对象1：{'field-6-1':1,'field-6-2':'test-1','field-6-3':'2021-01-31'}
                    new WorkflowProFieldDTO()
                    {
                        Id = 1,
                        Text = "field-6-1",
                        Value = "1",
                        DataType = AttributeDataType.Int,
                        DisplayName = "测试字段-6-1",
                        Description = "测试字段-6-1：int类型，列表主键",
                        Leaf = true,
                        Level = 2,
                        IsPrimaryKey = true,
                        IsCondition = false,
                        IsExecutor = false,
                    },
                    new WorkflowProFieldDTO()
                    {
                        Id = 1,
                        Text = "field-6-2",
                        Value = "test-1",
                        DataType = AttributeDataType.String,
                        DisplayName = "测试字段-6-2",
                        Description = "测试字段-6-2：string类型",
                        Leaf = true,
                        Level = 2,
                        IsPrimaryKey = false,
                        IsCondition = false,
                        IsExecutor = false,
                    },
                    new WorkflowProFieldDTO()
                    {
                        Id = 1,
                        Text = "field-6-3",
                        Value = "2021-01-31",
                        DataType = AttributeDataType.DateTime,
                        DisplayName = "测试字段-6-3",
                        Description = "测试字段-6-3：datetime类型",
                        Leaf = true,
                        Level = 2,
                        IsPrimaryKey = false,
                        IsCondition = false,
                        IsExecutor = false,
                    },
                    #endregion

                    #region 对象2：{'field-6-1':2,'field-6-2':'test-2','field-6-3':'2021-02-31'}
                    new WorkflowProFieldDTO()
                    {
                        Id = 2,
                        Text = "field-6-1",
                        Value = "2",
                        DataType = AttributeDataType.Int,
                        DisplayName = "测试字段-6-1",
                        Description = "测试字段-6-1：int类型，列表主键",
                        Leaf = true,
                        Level = 2,
                        IsPrimaryKey = true,
                        IsCondition = false,
                        IsExecutor = false,
                    },
                    new WorkflowProFieldDTO()
                    {
                        Id = 2,
                        Text = "field-6-2",
                        Value = "test-2",
                        DataType = AttributeDataType.String,
                        DisplayName = "测试字段-6-2",
                        Description = "测试字段-6-2：string类型",
                        Leaf = true,
                        Level = 2,
                        IsPrimaryKey = false,
                        IsCondition = false,
                        IsExecutor = false,
                    },
                    new WorkflowProFieldDTO()
                    {
                        Id = 2,
                        Text = "field-6-3",
                        Value = "2021-02-31",
                        DataType = AttributeDataType.DateTime,
                        DisplayName = "测试字段-6-3",
                        Description = "测试字段-6-3：datetime类型",
                        Leaf = true,
                        Level = 2,
                        IsPrimaryKey = false,
                        IsCondition = false,
                        IsExecutor = false,
                    },
                    #endregion

                    #region 对象3：{'field-6-1':3,'field-6-2':'test-3','field-6-3':'2021-03-31'}
                    new WorkflowProFieldDTO()
                    {
                        Id = 3,
                        Text = "field-6-1",
                        Value = "3",
                        DataType = AttributeDataType.Int,
                        DisplayName = "测试字段-6-1",
                        Description = "测试字段-6-1：int类型，列表主键",
                        Leaf = true,
                        Level = 2,
                        IsPrimaryKey = true,
                        IsCondition = false,
                        IsExecutor = false,
                    },
                    new WorkflowProFieldDTO()
                    {
                        Id = 3,
                        Text = "field-6-2",
                        Value = "test-3",
                        DataType = AttributeDataType.String,
                        DisplayName = "测试字段-6-2",
                        Description = "测试字段-6-2：string类型",
                        Leaf = true,
                        Level = 2,
                        IsPrimaryKey = false,
                        IsCondition = false,
                        IsExecutor = false,
                    },
                    new WorkflowProFieldDTO()
                    {
                        Id = 3,
                        Text = "field-6-3",
                        Value = "2021-03-31",
                        DataType = AttributeDataType.DateTime,
                        DisplayName = "测试字段-6-3",
                        Description = "测试字段-6-3：datetime类型",
                        Leaf = true,
                        Level = 2,
                        IsPrimaryKey = false,
                        IsCondition = false,
                        IsExecutor = false,
                    },
                    #endregion
                };

                return new List<WorkflowProFieldDTO>() { formField1_int, formField2_string, formField3_currency, formField4_datetime, formField5_decimal, formField6_list };
            }
        }

        /// <summary>
        /// 说明：单人审批测试， </br>
        ///     （小肖）启动后，而后由（肖经理）执行单人审核任务
        /// 使用流程定义1【641D0B9E-EDFF-400B-BFC4-0022F4E2C6E0】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Task:node2->End:node9
        ///     流程类型：Task:node2：WorkFlowType.SingleLine
        ///     执行人设置：Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】 </br>
        ///     流程实例使用表单：wfProcessForm
        /// 执行流程
        ///     启动后的流程图：Start:node1->Task:node2(true)->End:node9 </br>
        ///     流转后的流程图：Start:node1->Task:node2->End:node9
        /// </summary>
        public static string wfInstance_1;

        /// <summary>
        /// 说明：多人审批(权重50%通过)测试 </br>
        ///     （小肖）启动后，而后由（肖经理、齐副经理、齐总经理）执行多人审核任务 </br>
        /// 使用流程定义2【A56B55FC-F9C9-4BD5-BAE3-ACB5CFD4BB3B】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Task:node3->End:node9
        ///     流程类型：Task:node3：WorkFlowType.MultiLine
        ///     执行人设置：Task:node3:【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     流程实例使用表单：wfProcessForm
        /// 执行流程
        ///     第一次流转(肖经理-不同意)后的流程图：Start:node1->Task:node3(true)->End:node9
        ///     第二次流转(齐副经理-同意)后的流程图：Start:node1->Task:node3(true)->End:node9
        ///     第三次流转(齐总经理-同意)后的流程图：Start:node1->Task:node3->End:node9
        /// </summary>
        public static string wfInstance_2;

        /// <summary>
        /// 说明：串联审批测试，由node3-task、node2-task组成串联任务 </br>
        ///     启动后为一个多人审批（权重50%通过）任务节点（Task:node3），而后为一个单人审批任务节点（Task:node2），流程进行四次流转处理
        /// 使用流程定义3【CB625CF0-C777-4FA9-91EF-F79CB13307A7】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Task:node3->Task:node2->End:node9
        ///     流程类型：Task:node3：WorkFlowType.MultiLine、Task:node2：WorkFlowType.SingleLine
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     流程实例使用表单：wfProcessForm_2
        ///     表单数据：field-1-2=ede9edf9-5909-42d0-8563-aaa5c04cd8c8、field-1-3=15.26
        ///     条件处理过程：node3：条件为true，执行下一个流程节点：Task:node4
        ///                 node3：条件为false，执行下一个流程节点：Task:node2
        /// 执行流程
        ///     启动后的流程图：Start:node1->Task:node3(true)->Task:node2->End:node9
        ///     第一次流转(肖经理-不同意)后的流程图：Start:node1->Task:node3(true)->Task:node2->End:node9
        ///     第二次流转(齐副经理-同意)后的流程图：Start:node1->Task:node3(true)->Task:node2->End:node9
        ///     第三次流转(齐总经理-同意)后的流程图：Start:node1->Task:node3->Task:node2(true)->End:node9
        ///     第四次流转(肖经理-同意)后的流程图：Start:node1->Task:node3->Task:node2->End:node9
        /// </summary>
        public static string wfInstance_3;

        /// <summary>
        /// 说明：条件前置审批测试 </br>
        ///     生成1个条件前置流程实例，启动后为一个条件节点（Condition:node5【field3>=20】），而后进行流转处理（Submit：同意）
        /// 使用流程定义4【6DA8C252-FD6F-4F01-A446-10CB690CF932】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Condition:node5->Task:node2->End:node9
        ///     流程类型：Task:node2：WorkFlowType.SingleLine
        ///     流程节点规则：Condition:node5：【field-3 大于等于 20】
        ///     流程实例使用表单：：wfProcessForm_1
        ///     表单数据：field-3=23、field-5=true
        ///     条件处理过程：node5：条件为true，执行下一个流程节点：Task:node2
        ///                 node5：条件为false，执行下一个流程节点：End:node9
        /// 执行过程：
        ///     启动后的流程图：Start:node1->Condition:node5->Task:node2(true)->End:node9 
        ///     流转后的流程图：Start:node1->Condition:node5->Task:node2->End:node9
        /// </summary>
        public static string wfInstance_4;

        /// <summary>
        /// 说明：条件后置审批测试 </br>
        ///     生成1个条件后置流程实例，启动后流转处理（Submit：同意），而后进行一个条件节点（Condition:node5【field3>=20】）
        /// 使用流程定义5【E16DBE43-9EF8-4C24-9F03-37EBF2EB16A2】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Task:node2->Condition:node5->End:node9
        ///     流程类型：Task:node2：WorkFlowType.SingleLine
        ///     流程节点规则：Condition:node5：【field-3 大于等于 20】
        ///     流程实例使用表单：：wfProcessForm_1
        ///     表单数据：field-3=23、field-5=true
        ///     条件处理过程：node5：条件为true，执行下一个流程节点：Task:node2
        ///                 node5：条件为false，执行下一个流程节点：End:node9
        /// 执行过程：
        ///     启动后的流程图：Start:node1->Task:node2(true)->Condition:node5->End:node9 
        ///     流转后的流程图：Start:node1->Task:node2->Condition:node5->End:node9
        /// </summary>
        public static string wfInstance_5;

        /// <summary>
        /// 说明：条件串联审批测试 </br>
        ///     生成1个条件后置流程实例，启动后为两个条件节点（Condition:node5【field3>=20】、Condition:node6【field3>=40 && field5==true】），而后进行流转处理（Submit：同意）
        /// 使用流程定义6【95B837AD-2F4A-4D78-B6CC-3DEBE8EA536D】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Condition:node5->Condition:node6->Task:node2->End:node9
        ///     流程类型：Task:node2：WorkFlowType.SingleLine
        ///     流程节点规则：
        ///         Condition:node5：【field-3 大于等于 20】
        ///         Condition:node6：【field-3 大于等于 40 并且 field-5 等于 true】
        ///     流程实例使用表单：wfProcessForm_1
        ///     表单数据：field-3=43、field-5=true
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Condition:node6（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        /// 执行过程：
        ///     启动后的流程图：Start:node1->Condition:node5->Condition:node6->Task:node2(true)->End:node9 
        ///     流转后的流程图：Start:node1->Condition:node5->Condition:node6->Task:node2->End:node9
        /// </summary>
        public static string wfInstance_6;

        /// <summary>
        /// 说明：复杂条件前置审批 </br>
        ///     使用流程定义7，生成1个条件前置的复杂流程实例，启动后由两个条件及任务节点（Condition:node5【field3>=20】、Task:node2）及（Condition:node6【field3>=40 && field5==true】、Task:node3）组成
        /// 使用流程定义7【A56B55FC-F9C9-4BD5-BAE3-ACB5CFD4BB3B】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
        ///     流程类型：Task:node2：WorkFlowType.SingleLine、Task:node3：WorkFlowType.MultiLine
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     流程节点规则：
        ///         Condition:node5：【field-3 大于等于 20】
        ///         Condition:node6：【field-3 大于等于 40 并且 field-5 等于 true】
        ///     流程实例使用表单：：wfProcessForm
        ///     表单数据：field-3=43、field-5=true
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node3（满足）
        ///                         条件为false，流程结束：End:node9
        /// 执行过程：
        ///     启动后的流程图：Start:node1->Condtion:node5->Task:node2(current)->Condtion:node6->Task:node3->End:node9 
        ///     流转后的流程图：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
        /// </summary>
        public static string wfInstance_7_1;

        /// <summary>
        /// 说明：复杂条件前置审批 </br>
        ///     使用流程定义7，生成1个条件前置的复杂流程实例，启动后由两个条件及任务节点（Condition:node5【field3>=20】、Task:node2）及（Condition:node6【field3>=40 && field5==true】、Task:node3）组成
        /// 使用流程定义7【A56B55FC-F9C9-4BD5-BAE3-ACB5CFD4BB3B】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
        ///     流程类型：Task:node2：WorkFlowType.SingleLine、Task:node3：WorkFlowType.MultiLine
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     流程节点规则：
        ///         Condition:node5：【field-3 大于等于 20】
        ///         Condition:node6：【field-3 大于等于 40 并且 field-5 等于 true】
        ///     流程实例使用表单：：wfProcessForm
        ///     表单数据：field-3=43、field-5=true
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node3（满足）
        ///                         条件为false，流程结束：End:node9
        /// 执行过程：
        ///     启动后的流程图：Start:node1->Condtion:node5->Task:node2(current)->Condtion:node6->Task:node3->End:node9 
        ///     流转后的流程图：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
        /// </summary>
        public static string wfInstance_7_2;

        /// <summary>
        /// 说明：复杂条件后置审批 </br>
        ///     使用流程定义7，生成1个条件前置的复杂流程实例，启动后由两个条件及任务节点（Task:node2、Condition:node5【field3>=20】）及（Task:node3、Condition:node6【field3>=40 && field5==true】）组成
        /// 使用流程定义7【A56B55FC-F9C9-4BD5-BAE3-ACB5CFD4BB3B】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Task:node2->Condtion:node5->Task:node3->Condtion:node6->End:node9
        ///     流程类型：Task:node2：WorkFlowType.SingleLine、Task:node3：WorkFlowType.MultiLine
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     流程节点规则：
        ///         Condition:node5：【field-3 大于等于 20】
        ///         Condition:node6：【field-3 大于等于 40 并且 field-5 等于 true】
        ///     流程实例使用表单：：wfProcessForm
        ///     表单数据：field-3=43、field-5=true
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node3（满足）
        ///                         条件为false，流程结束：End:node9
        /// 执行过程：
        ///     启动后的流程图：Start:node1->Task:node2(current)->Condtion:node5->Task:node3->Condtion:node6->End:node9 
        ///     流转后的流程图：Start:node1->Task:node2->Condtion:node5->Task:node3->Condtion:node6->End:node9
        /// </summary>
        public static string wfInstance_8_1;

        /// <summary>
        /// 说明：复杂条件后置审批 </br>
        ///     使用流程定义7，生成1个条件前置的复杂流程实例，启动后由两个条件及任务节点（Task:node2、Condition:node5【field3>=20】）及（Task:node3、Condition:node6【field3>=40 && field5==true】）组成
        /// 使用流程定义7【A56B55FC-F9C9-4BD5-BAE3-ACB5CFD4BB3B】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Task:node2->Condtion:node5->Task:node3->Condtion:node6->End:node9
        ///     流程类型：Task:node2：WorkFlowType.SingleLine、Task:node3：WorkFlowType.MultiLine
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     流程节点规则：
        ///         Condition:node5：【field-3 大于等于 20】
        ///         Condition:node6：【field-3 大于等于 40 并且 field-5 等于 true】
        ///     流程实例使用表单：：wfProcessForm
        ///     表单数据：field-3=43、field-5=true
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node3（满足）
        ///                         条件为false，流程结束：End:node9
        /// 执行过程：
        ///     启动后的流程图：Start:node1->Task:node2(current)->Condtion:node5->Task:node3->Condtion:node6->End:node9 
        ///     流转后的流程图：Start:node1->Task:node2->Condtion:node5->Task:node3->Condtion:node6->End:node9
        /// </summary>
        public static string wfInstance_8_2;

        /// <summary>
        /// 说明：复杂条件前置审批 </br>
        ///     使用流程定义9，生成1个条件前置的复杂流程实例，启动后由两个条件及任务节点（Condition:node5【field3>=20】、Task:node2）及（Condition:node6【field3>=40 && field5==true】、Task:node3）组成
        /// 使用流程定义7【A56B55FC-F9C9-4BD5-BAE3-ACB5CFD4BB3B】生成流程实例，流程说明如下：
        ///     流程图：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
        ///     流程类型：Task:node2：WorkFlowType.SingleLine、Task:node3：WorkFlowType.MultiLine
        ///     执行人设置：
        ///         Task:node2【CreatorManager：发起人的主管--销售经理：齐副经理, 肖经理】
        ///         Task:node3【Executor：组织/角色/用户--组织:【企业, 销售部】角色:【总经理, 销售经理】用户:【肖经理】】
        ///     流程节点规则：
        ///         Condition:node5：【field-3 大于等于 20】
        ///         Condition:node6：【field-3 大于等于 40 并且 field-5 等于 true】
        ///     流程实例使用表单：：wfProcessForm
        ///     表单数据：field-3=23、field-5=true
        ///     条件处理过程：node5-执行条件【field3>=20】
        ///                         条件为true，下一流程：Task:node2（满足）
        ///                         条件为false，流程结束：End:node9
        ///                  node6-执行条件【field3>=40 && field5==true】
        ///                         条件为true，下一流程：Task:node3
        ///                         条件为false，流程结束：Condition:node5（满足）
        /// 执行过程：
        ///     启动后的流程图：Start:node1->Condtion:node5->Task:node2(current)->Condtion:node6->Task:node3->End:node9 
        ///     流转后的流程图：Start:node1->Condtion:node5->Task:node2->Condtion:node6->Task:node3->End:node9
        /// </summary>
        public static string wfInstance_9;

        #endregion

        /// <summary>
        /// 是否开启删除初始化数据
        /// </summary>
        private const bool IsDeleteInitData = false;
        public override void SetUpData()
        {
            InjectTenant(TestTenant);
            //InjectTenant(DbaTenant);

            using (var scope = Services.BuildServiceProvider().CreateScope())
            {
                var cache = scope.ServiceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                Service.CacheUtil.Cache = cache;
            }

            #region AutoMapper对象注入
            var profiles = KC.Service.AutoMapper.AutoMapperConfiguration.GetAllProfiles()
                .Union(KC.Service.Workflow.AutoMapper.AutoMapperConfiguration.GetAllProfiles());

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(profiles);
            });
            Services.AddSingleton(config);
            var mapper = config.CreateMapper();
            Services.AddSingleton(mapper);

            #endregion

            KC.Service.Util.DependencyInjectUtil.InjectService(Services);
            KC.Service.Workflow.DependencyInjectUtil.InjectService(Services);

            var users = new List<UserSimpleDTO>() {
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfItAdminUserId,
                    UserName = WorkflowFixture.wfItAdminUserName,
                    DisplayName = WorkflowFixture.wfItAdminUserName,
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    Status = WorkflowBusStatus.Approved
                },
                new UserSimpleDTO()
                {
                    UserId = WorkflowFixture.wfItTestUserId,
                    UserName = "tester",
                    DisplayName = "tester",
                    PhoneNumber = "17744949695",
                    Email = "kcloudy@163.com",
                    Status = WorkflowBusStatus.Disagree
                }
            };
            var orgs = new List<OrganizationSimpleDTO>() {
                new OrganizationSimpleDTO()
                {
                    Id = 1,
                    ParentId = null,
                    Text = "测试组织",
                    OrganizationCode = "TestOrgCode",
                    Users = new List<UserSimpleDTO>() {
                        new UserSimpleDTO()
                        {
                            UserId = WorkflowFixture.wfItAdminUserId,
                            UserName = WorkflowFixture.wfItAdminUserName,
                            DisplayName = WorkflowFixture.wfItAdminUserName,
                            PhoneNumber = "17744949695",
                            Email = "kcloudy@163.com",
                            Status = WorkflowBusStatus.Approved
                        },
                        new UserSimpleDTO()
                        {
                            UserId = WorkflowFixture.wfItTestUserId,
                            UserName = "tester",
                            DisplayName = "tester",
                            PhoneNumber = "17744949695",
                            Email = "kcloudy@163.com",
                            Status = WorkflowBusStatus.Disagree
                        }
                    }
                }
            };

            //var mockAccountApiService = new Moq.Mock<IAccountApiService>();
            //mockAccountApiService.Setup(m => m.LoadUsersByIdsAndRoleIdsAndOrgCodes(null).Result).Returns(users);
            //mockAccountApiService.Setup(m => m.LoadOrganizationsWithUsersByUserId(WorkflowFixture.wfItTestUserId).Result).Returns(orgs);
            //Services.AddScoped(typeof(IAccountApiService), serviceProvider => mockAccountApiService.Object);

            Services.AddScoped<IAccountApiService, MockAccountApiService>();

            _logger = LoggerFactory.CreateLogger(nameof(WorkflowFixture));
            _serviceProvider = Services.BuildServiceProvider();

            var isSuccess = true;
            //新增流程定义：641D0B9E-EDFF-400B-BFC4-0022F4E2C6E0
            using (var scope = _serviceProvider.CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                isSuccess = wfService.SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDef_1).Result;
            }

            //新增流程定义：A56B55FC-F9C9-4BD5-BAE3-ACB5CFD4BB3B
            using (var scope = _serviceProvider.CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                isSuccess = wfService.SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDef_2).Result;
            }

            //新增流程定义：CB625CF0-C777-4FA9-91EF-F79CB13307A7
            using (var scope = _serviceProvider.CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                isSuccess = wfService.SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDef_3).Result;
            }

            //新增流程定义：6DA8C252-FD6F-4F01-A446-10CB690CF932
            using (var scope = _serviceProvider.CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                isSuccess = wfService.SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDef_4).Result;
            }

            //新增流程定义：E16DBE43-9EF8-4C24-9F03-37EBF2EB16A2
            using (var scope = _serviceProvider.CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                isSuccess = wfService.SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDef_5).Result;
            }

            //新增流程定义：95B837AD-2F4A-4D78-B6CC-3DEBE8EA536D
            using (var scope = _serviceProvider.CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                isSuccess = wfService.SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDef_6).Result;
            }

            //新增流程定义：F3E5F535-EDDF-46D2-A9A1-F7C90DAEA9EE
            using (var scope = _serviceProvider.CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                isSuccess = wfService.SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDef_7).Result;
            }

            //新增流程定义：06E95126-8306-4D7A-81FB-F6FA7063E8C9
            using (var scope = _serviceProvider.CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                isSuccess = wfService.SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDef_8).Result;
            }

            //新增流程定义：AF3C3CAC-5AB2-4A2F-BEFA-43B263238A8F
            using (var scope = _serviceProvider.CreateScope())
            {
                var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                isSuccess = wfService.SaveWorkflowDefinitionWithFieldsAndNodesAsync(WorkflowDef_9).Result;
            }

            _logger.LogInformation("----生成流程定义【" + wfDefId_1 + "、" + wfDefId_2 + "、" + wfDefId_3 + "、" + wfDefId_4 + "、" + wfDefId_5 + "、" + wfDefId_6 + "、" + wfDefId_7 + "、" + wfDefId_8 + "、" + wfDefId_9 + "】 ? " + isSuccess.ToString());
        }

        /* 删除脚本：流程定义数据
        SELECT * FROM [cTest].[wf_WorkflowDefinition]
        SELECT * FROM [cTest].[wf_WorkflowDefField]
        SELECT * FROM [cTest].[wf_WorkflowDefNode]
        SELECT * FROM [cTest].[wf_WorkflowDefNodeRule]
        GO

        delete [cTest].[wf_WorkflowDefField] where ISNULL(ParentId, 1)!=1
        GO
        delete [cTest].[wf_WorkflowDefField] where ISNULL(ParentId, 1)=1
        delete [cTest].[wf_WorkflowDefNodeRule]
        delete [cTest].[wf_WorkflowDefNode]
        delete [cTest].[wf_WorkflowDefinition]
        GO
        */
        /* 删除脚本：流程版本数据
        SELECT * FROM [cTest].[wf_WorkflowVerDefinition]
        SELECT * FROM [cTest].[wf_WorkflowVerDefField]
        SELECT * FROM [cTest].[wf_WorkflowVerDefNode]
        SELECT * FROM [cTest].[wf_WorkflowVerDefNodeRule]
        GO

        delete [cTest].[wf_WorkflowVerDefField] where ISNULL(ParentId, 1)!=1
        GO
        delete [cTest].[wf_WorkflowVerDefField] where ISNULL(ParentId, 1)=1
        delete [cTest].[wf_WorkflowVerDefNodeRule]
        delete [cTest].[wf_WorkflowVerDefNode]
        delete [cTest].[wf_WorkflowVerDefinition]
        GO
        */
        /* 删除脚本：流程实例数据
        SELECT * FROM [cTest].[wf_WorkflowProcess]
        SELECT * FROM [cTest].[wf_WorkflowProField]
        SELECT * FROM [cTest].[wf_WorkflowProTask] order by ProcessId,CreatedDate
        SELECT * FROM [cTest].[wf_WorkflowProTaskRule]
        GO

        delete [cTest].[wf_WorkflowProField] where ISNULL(ParentId, 1)!=1
        GO
        delete [cTest].[wf_WorkflowProField] where ISNULL(ParentId, 1)=1
        delete [cTest].[wf_WorkflowProTaskRule]
        delete [cTest].[wf_WorkflowProTaskExecute]
        delete [cTest].[wf_WorkflowProTask]
        delete [cTest].[wf_WorkflowProcess]
        GO
        */
        public override void TearDownData()
        {
            try
            {
                if (IsDeleteInitData)
                {
                    //删除生成的流程定义数据：
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var wfService = scope.ServiceProvider.GetService<IWorkflowDefinitionService>();
                        var success1 = wfService.RemoveWorkflowDefinitionWithFieldsAsync(wfDefId_1, RoleConstants.AdminUserId, RoleConstants.AdminUserName).Result;
                        var success2 = wfService.RemoveWorkflowDefinitionWithFieldsAsync(wfDefId_2, RoleConstants.AdminUserId, RoleConstants.AdminUserName).Result;
                        var success3 = wfService.RemoveWorkflowDefinitionWithFieldsAsync(wfDefId_3, RoleConstants.AdminUserId, RoleConstants.AdminUserName).Result;
                        var success4 = wfService.RemoveWorkflowDefinitionWithFieldsAsync(wfDefId_4, RoleConstants.AdminUserId, RoleConstants.AdminUserName).Result;
                        var success5 = wfService.RemoveWorkflowDefinitionWithFieldsAsync(wfDefId_5, RoleConstants.AdminUserId, RoleConstants.AdminUserName).Result;
                        var success6 = wfService.RemoveWorkflowDefinitionWithFieldsAsync(wfDefId_6, RoleConstants.AdminUserId, RoleConstants.AdminUserName).Result;
                        var success7 = wfService.RemoveWorkflowDefinitionWithFieldsAsync(wfDefId_7, RoleConstants.AdminUserId, RoleConstants.AdminUserName).Result;
                        var success8 = wfService.RemoveWorkflowDefinitionWithFieldsAsync(wfDefId_8, RoleConstants.AdminUserId, RoleConstants.AdminUserName).Result;
                        var success9 = wfService.RemoveWorkflowDefinitionWithFieldsAsync(wfDefId_9, RoleConstants.AdminUserId, RoleConstants.AdminUserName).Result;
                        _logger.LogInformation("----删除流程实定义【" + wfDefId_1 + "、" + wfDefId_2 + "、" + wfDefId_3 + "、" + wfDefId_4 + "、" + wfDefId_5 + "、" + wfDefId_6 + "、" + wfDefId_7 + "、" + wfDefId_8 + "、" + wfDefId_9 + "】成功");
                    }

                    //删除生成的流程实例数据：
                    using (var scope = Services.BuildServiceProvider().CreateScope())
                    {
                        var wfService = scope.ServiceProvider.GetService<IWorkflowProcessService>();
                        var success1 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfInstance_1).Result;
                        var success2 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfInstance_2).Result;
                        var success3 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfInstance_3).Result;
                        var success4 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfInstance_4).Result;
                        var success5 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfInstance_5).Result;
                        var success6 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfInstance_6).Result;
                        var success7_1 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfInstance_7_1).Result;
                        var success7_2 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfInstance_7_2).Result;
                        var success8_1 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfInstance_8_1).Result;
                        var success8_2 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfInstance_8_2).Result;
                        var success9_1 = wfService.RemoveWorkflowProcessWithTasksByCodeAsync(wfInstance_9).Result;
                        _logger.LogInformation("----删除流程实例【" + wfInstance_1 + "、" + wfInstance_2 + "、" + wfInstance_3 + "、" + wfInstance_4 + "、" + wfInstance_5 + "、" + wfInstance_6 + "、" + wfInstance_7_1 + "、" + wfInstance_7_2 + "、" + wfInstance_8_1 + "、" + wfInstance_8_2 + "、" + wfInstance_9 + "】成功。");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

    }
}
