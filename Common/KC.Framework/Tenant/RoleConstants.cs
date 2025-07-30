using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Framework.Tenant
{
    public class RoleConstants
    {
        static RoleConstants()
        {

        }

        /// <summary>
        /// 系统默认角色，所有人都有的角色
        /// </summary>
        public const string DefaultRoleId = "AC4E0709-0C84-4CDD-98D4-0734E827853C";
        /// <summary>
        /// 注册个人的角色（person）Id：127287A0-78F4-4B29-A111-78784E4205B3
        /// </summary>
        public const string RegisterPersonRoleId = "127287A0-78F4-4B29-A111-78784E4205B3";
        /// <summary>
        /// 注册企业的角色（company）Id：6C31B5EB-545A-4BFC-AF40-603CABFD108A
        /// </summary>
        public const string RegisterCompanyRoleId = "6C31B5EB-545A-4BFC-AF40-603CABFD108A";

        /// <summary>
        /// 系统管理员的角色（admin）Id：B779D882-68ED-4298-B233-AA68065C447B
        /// </summary>
        public const string AdminRoleId = "B779D882-68ED-4298-B233-AA68065C447B";
        public const string AdminRoleName = "系统管理员";

        /// <summary>
        /// 人力资源助理的角色（hr）Id：2C8D8527-2C39-4321-8270-8549870718A9
        /// </summary>
        public const string HrRoleId = "2C8D8527-2C39-4321-8270-8549870718A9";
        /// <summary>
        /// 人力资源经理的角色（hrmanager）Id：79E08D9B-4301-4CE1-BF32-0285004D2B73
        /// </summary>
        public const string HrManagerRoleId = "79E08D9B-4301-4CE1-BF32-0285004D2B73";

        /// <summary>
        /// 行政助理的角色（executor）Id：4DFAD70B-21CE-4940-984B-B858F1303A2F
        /// </summary>
        public const string ExecutorRoleId = "4DFAD70B-21CE-4940-984B-B858F1303A2F";
        /// <summary>
        /// 行政经理的角色（executormanager）Id：654280B9-E850-4AED-8D7F-2DF62E39B5DC
        /// </summary>
        public const string ExecutorManagerRoleId = "654280B9-E850-4AED-8D7F-2DF62E39B5DC";

        /// <summary>
        /// 销售助理的角色（sale）Id：2DE1ABA7-E57D-47EC-9B6E-5E9904072400
        /// </summary>
        public const string SaleRoleId = "2DE1ABA7-E57D-47EC-9B6E-5E9904072400";
        /// <summary>
        /// 销售经理的角色（salemanager）Id：9A3A51A9-2ECD-40DC-990C-015A6AB17BF0
        /// </summary>
        public const string SaleManagerRoleId = "9A3A51A9-2ECD-40DC-990C-015A6AB17BF0";

        /// <summary>
        /// 采购助理的角色（purchase）Id：27B7C724-0A3F-438E-989E-8C7DF5D7989D
        /// </summary>
        public const string PurchaseRoleId = "27B7C724-0A3F-438E-989E-8C7DF5D7989D";
        /// <summary>
        /// 采购经理的角色（purchasemanager）Id：0B6DE259-FBEA-401C-A112-4980AF659674
        /// </summary>
        public const string PurchaseManagerRoleId = "0B6DE259-FBEA-401C-A112-4980AF659674";

        /// <summary>
        /// 仓储助理的角色（warehouse）Id：2B01379C-5DD6-4B1D-AAD4-1151CDEFEDEA
        /// </summary>
        public const string WarehouseRoleId = "2B01379C-5DD6-4B1D-AAD4-1151CDEFEDEA";
        /// <summary>
        /// 仓储经理的角色（warehousemanager）Id：771E4262-7498-46AA-A12F-986B98DB32C0
        /// </summary>
        public const string WarehouseManagerRoleId = "771E4262-7498-46AA-A12F-986B98DB32C0";

        /// <summary>
        /// 生产助理的角色（manufacting）Id：4867C9B1-08A0-4B14-BB80-20CCB53C1D4D
        /// </summary>
        public const string ManufactingRoleId = "4867C9B1-08A0-4B14-BB80-20CCB53C1D4D";
        /// <summary>
        /// 生产经理的角色（manufactingmanager）Id：646E0519-CC98-4C18-9CF9-1117EE086BCB
        /// </summary>
        public const string ManufactingManagerRoleId = "646E0519-CC98-4C18-9CF9-1117EE086BCB";

        /// <summary>
        /// 质检助理的角色（qa）Id：98D8B16B-A191-49A5-ADFB-87BFA9B39EEF
        /// </summary>
        public const string QARoleId = "98D8B16B-A191-49A5-ADFB-87BFA9B39EEF";
        /// <summary>
        /// 质检经理的角色（qamanager）Id：2D309484-3D73-411A-9D16-704AB492DAA3
        /// </summary>
        public const string QAManagerRoleId = "2D309484-3D73-411A-9D16-704AB492DAA3";

        /// <summary>
        /// 市场助理的角色（marketing）Id：9AC124B3-4D17-48BF-8713-8B9763DDB94E
        /// </summary>
        public const string MarketingRoleId = "9AC124B3-4D17-48BF-8713-8B9763DDB94E";
        /// <summary>
        /// 市场经理的角色（marketingmanager）Id：B48B7AAA-CCDF-41CC-85AF-40FFA39BBFAD
        /// </summary>
        public const string MarketingManagerRoleId = "B48B7AAA-CCDF-41CC-85AF-40FFA39BBFAD";

        /// <summary>
        /// 运营助理的角色（operator）Id：A5505492-5CAA-46F6-83A8-B8C75114380F
        /// </summary>
        public const string OperatorRoleId = "A5505492-5CAA-46F6-83A8-B8C75114380F";
        /// <summary>
        /// 运营经理的角色（operatormanager）Id：CAC720DE-1984-4DA6-B800-848B047B33D5
        /// </summary>
        public const string OperatorManagerRoleId = "CAC720DE-1984-4DA6-B800-848B047B33D5";

        /// <summary>
        /// 财务助理的角色（accounting）Id：9BD31CA9-A09E-4D38-AE0F-46765B11C7E9
        /// </summary>
        public const string AccountingRoleId = "9BD31CA9-A09E-4D38-AE0F-46765B11C7E9";
        /// <summary>
        /// 财务经理的角色（accountingmanager）Id：59E88BB5-C1F0-4899-89B7-A32A0714B447
        /// </summary>
        public const string AccountingManagerRoleId = "59E88BB5-C1F0-4899-89B7-A32A0714B447";

        /// <summary>
        /// 设计助理的角色（designer）Id：EE950278-F378-4E8C-9ED9-9DA9EC683479
        /// </summary>
        public const string DesignerRoleId = "EE950278-F378-4E8C-9ED9-9DA9EC683479";
        /// <summary>
        /// 设计经理的角色（designermanager）Id：3CA1AA92-5322-43CE-9913-72AE2ECBB271
        /// </summary>
        public const string DesignerManagerRoleId = "3CA1AA92-5322-43CE-9913-72AE2ECBB271";

        /// <summary>
        /// 总经理助理的角色（gmassistant）Id：6AF27A89-210D-45BF-BDC2-108E2AA4D2D1
        /// </summary>
        public const string GmAssistantRoleId = "6AF27A89-210D-45BF-BDC2-108E2AA4D2D1";
        /// <summary>
        /// 总经理的角色（generalmanager）Id：9DA7A929-CEAC-4C57-B7F6-F08941924041
        /// </summary>
        public const string GeneralManagerRoleId = "9DA7A929-CEAC-4C57-B7F6-F08941924041";

        /// <summary>
        /// 系统管理员的用户Id：ede9edf9-5909-42d0-8563-aaa5c04cd8c8
        /// </summary>
        public const string AdminUserId = "ede9edf9-5909-42d0-8563-aaa5c04cd8c8";
        public const string AdminUserName = "admin";

    }
    public class SysSequenceConstants
    {
        static SysSequenceConstants()
        {

        }

        public const string Organization = "Organization";
        public const string Member = "Member";
        public const string BusinessType = "BusinessType";
        public const string Customer = "Customer";
        public const string Supplier = "Supplier";
        public const string OfferingProvider = "OfferingProvider";
        public const string Offering = "Offering";
        public const string OrderInquire = "OrderInquire";
        public const string OrderPurchase = "OrderPurchase";
        public const string OrderOffering = "OrderOffering";
        public const string PurchaseOrder = "PurchaseOrder";
        public const string SalesOrder = "SalesOrder";
        public const string APInvoice = "APInvoice";
        public const string ARInvoice = "ARInvoice";
        public const string DeliveryNote = "DeliveryNote";
        public const string GoodsReceipt = "GoodsReceipt";
        public const string MessageTemplate = "MessageTemplate";
        public const string FinancingDemand = "FinancingDemand";
    }
}
