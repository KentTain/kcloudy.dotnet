using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.Customer.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string CustomerInfo = "CustomerInfo";
        public const string CustomerSea = "CustomerSea";
        public const string Notification = "Notification";
        public const string Report = "Report";
        public const string CustomerExtraInfoProvider = "CustomerExtraInfoProvider";
    }

    public sealed class ActionName : ActionNameBase
    {
        public sealed class CustomerInfo
        {
            public const string LoadCustomerInfoList = "LoadCustomerInfoList";
            public const string GetCustomerInfoForm = "GetCustomerInfoForm";
            public const string SaveCustomerInfoForm = "SaveCustomerInfoForm";
            public const string RemoveCustomerInfo = "RemoveCustomerInfo";
            public const string ReleaseCustomerInfo = "ReleaseCustomerInfo";
            public const string RemoveCustomerContactSoft = "RemoveCustomerContactSoft";
            public const string DownLoadExcelTemplate = "DownLoadExcelTemplate";
            public const string UploadExcelTemplate = "UploadExcelTemplate";
            public const string CustomerDetail = "CustomerDetail";
            //public const string UploadExcelToUpdateCustomers = "UploadExcelToUpdateCustomers";
            public const string ExistPersonalCustomerPhoneNumber = "ExistPersonalCustomerPhoneNumber";

            public const string GetTenantSendingForm = "GetTenantSendingForm";
            public const string LoadTenantUserList = "LoadTenantUserList";
            public const string SendUserToTenant = "SendUserToTenant";

            public const string GetCustomerContact = "GetCustomerContact";
            public const string GetCustomerContactByCustomerId = "GetCustomerContactByCustomerId";
            public const string SaveCustomerContactForm = "SaveCustomerContactForm";
            public const string CustomerContactForm = "CustomerContactForm";
            public const string RemoveCustomerContact = "RemoveCustomerContact";
            public const string RemoveCustomerContactsoft = "RemoveCustomerContactsoft";
            public const string GetCustomerInfoById = "GetCustomerInfoById";
            public const string CustomerDetailInfo = "CustomerDetailInfo";

            //CustomerLog
            public const string CustomerChangeLogList = "CustomerChangeLogList";
            public const string LoadCustomerChangeLogList = "LoadCustomerChangeLogList";
            public const string CustomeChangeLogForm = "CustomeChangeLogForm";
            public const string EditCustomeChangeLog = "EditCustomeChangeLog";
            public const string RemoveCustomeChangeLog = "RemoveCustomeChangeLog";

            //Tracinglog
            public const string GetActivityLogListByCustomerInfoId = "GetActivityLogListByCustomerInfoId";
            public const string CustomerTracingLogList = "CustomerTracingLogList";
            public const string LoadCustomerTracingLogList = "LoadCustomerTracingLogList";
            public const string CustomerTracingLogForm = "CustomerTracingLogForm";
            public const string SaveCustomerTracingLog = "SaveCustomerTracingLog";
            public const string RomoveCustomerTracingLog = "RomoveCustomerTracingLog";

            public const string CallCustomer = "CallCustomer";
            public const string StopCallCustomer = "StopCallCustomer";
            //CustomerExtraInfo
            public const string GetCustomerExtraInfoList = "GetCustomerExtraInfoList";
            public const string LoadCustomerExtraInfoListByCustomerId = "LoadCustomerExtraInfoListByCustomerId";
            public const string RemoveCustomerExtInfo = "RemoveCustomerExtInfo";
            public const string SaveCustomerExtInfo = "SaveCustomerExtInfo";
            public const string SaveTextValue = "SaveTextValue";
            public const string GenerateEditorData = "GenerateEditorData";
            public const string SaveAttribute = "SaveAttribute";
            public const string RomoveAttr = "RomoveAttr";

            public const string ShareCustomerWithManager = "ShareCustomerWithManager";
            public const string AssignCustomerList = "AssignCustomerList";
            public const string ReassignCustomerToManager = "ReassignCustomerToManager";
            public const string TransferCustomerToOtherManager = "TransferCustomerToOtherManager";
            public const string GetCustomerManagersByCustomerId = "GetCustomerManagersByCustomerId";

            public const string GetCustomerFieldCustomForm = "GetCustomerFieldCustomForm";
            public const string SaveCustomerFieldCustom = "SaveCustomerFieldCustom";

        }

        public sealed class CustomerSea
        {
            //customerSeas
            public const string TransferCutomerToSeas = "TransferCutomerToSeas";
            public const string PickCustomerFromSeas = "PickCustomerFromSeas";
            public const string LoadCustomerSeasList = "LoadCustomerSeasList";

            //CustomerExtraInfoProvider
            public const string CustomerExtInfoProviderForm = "CustomerExtInfoProviderForm";
            public const string LoadCustomerExtInfoProviderList = "LoadCustomerExtInfoProviderList";
            public const string SaveCustomerExtInfoProviderForm = "SaveCustomerExtInfoProviderForm";
            public const string RemoveCustomerExtInfoProvider = "RemoveCustomerExtInfoProvider";

            public const string GetAllCustomerExtInfoProviers = "GetAllCustomerExtInfoProviers";

        }

        public sealed class Notification
        {
            public const string SendNotification = "SendNotification";
            public const string GetCustomerSendingForm = "GetCustomerSendingForm";
            public const string LoadSelectedCustomerInfoList = "LoadSelectedCustomerInfoList";
            public const string GetCCListForm = "GetCCListForm";
            public const string GetCCList = "LoadCCList";
            public const string GetAddList = "LoadAddList";

            public const string CallCustomer = "CallCustomer";
            public const string StopCallCustomer = "StopCallCustomer";

            public const string LoadNotificationList = "GetNotificationApplicationList";
            public const string UpdateStatus = "UpdateStatus";

            public const string GetNotificationDetail = "GetNotificationDetail";

            public const string SaveNotificationForm = "SaveNotificationForm";
            public const string GetNotificationForm = "GetNotificationForm";

            public const string LoadCustomerContactInfos = "LoadCustomerContactInfos";
            public const string GetSelectContactForm = "GetSelectContactForm";
            public const string CustomerTracingLogList = "CustomerTracingLogList";
            public const string GetCustomerTracingLogList = "GetCustomerTracingLogList";
            public const string GetCustomerTracingLogForm = "GetCustomerTracingLogForm";
            public const string SaveCustomerTracingLog = "SaveCustomerTracingLog";
            public const string GetCustomerContactsByCustomerType = "GetCustomerContactsByCustomerType";
            public const string GetCustomerCompanyByCustomerContact = "GetCustomerCompanyByCustomerContact";
            public const string RomoveCustomerTracingLog = "RomoveCustomerTracingLog";
            public const string Sms = "Sms";
            public const string Call = "Call";
            public const string Email = "Email";
            public const string PopCustomerForm = "PopCustomerForm";
            public const string SavePopCustomerInfo = "SavePopCustomerInfo";
        }

        public sealed class Report
        {
            public const string CustomerTraceInfoReport = "CustomerTraceInfoReport";
            public const string GetCustomerTraceInfoReport = "GetCustomerTraceInfoReport";

            public const string CustomerSendToTenantReport = "CustomerSendToTenantReport";
            public const string GetCustomerSendToTenantReport = "GetCustomerSendToTenantReport";
        }
    }
}