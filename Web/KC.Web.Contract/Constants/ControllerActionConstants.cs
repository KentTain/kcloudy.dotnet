using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.Contract.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string CurrencySign = "CurrencySign";
        public const string ElectronicSign = "ElectronicSign";

        public const string ContractManager = "ContractManager";
        public const string ContractTemplate = "ContractTemplate";
    }
    public sealed class ActionName : ActionNameBase
    {
        public sealed class ElectronicSign
        {
            public const string IsSeal = "IsSeal";
            public const string GetElectronicSignServiceClause = "GetElectronicSignServiceClause";
            public const string GenerateVerfiyPhoneCode = "GenerateVerfiyPhoneCode";
            public const string SubmitSeal = "SubmitSeal";
            public const string SubmitSealPhone = "SubmitSealPhone";
            public const string DeleteSealAccount = "DeleteSealAccount";

            public const string DeletePersonAccount = "DeletePersonAccount";
        }

        public sealed class ContractManager
        {
            public const string GetContractPageList = "GetContractPageList";
            public const string SaveContract = "SaveContract";
            public const string SignContract = "SignContract";


            public const string RemoveCurrencySignService = "RemoveCurrencySignService";
            public const string SynchronousCurrencySignData = "SynchronousCurrencySignData";
            public const string ComfirmContract = "ComfirmContract";
            public const string RetutrnContract = "RetutrnContract";
            public const string IsSeal = "IsSeal";
            public const string GetMyContract = "GetMyContract";
            public const string GetSignContract = "GetSignContract";
            public const string Details = "Details";
            public const string GetMySignContract = "GetMySignContract";
            public const string SetEnclosure = "SetEnclosure";
            public const string GetESignContract = "GetESignContract";
            public const string Relieve = "Relieve";
            public const string OrderUpPDFToContract = "OrderUpPDFToContract";
            public const string OrderUpPDFToContract_New = "OrderUpPDFToContract_New";

            public const string GetDetails = "GetDetails";
            public const string CheckContractStatu = "CheckContractStatu";
            public const string CheckContractStatuToId = "CheckContractStatuToId";
            public const string GetUserList = "GetUserList";
            public const string EditCurrencySignService = "EditCurrencySignService";
            public const string GetContract = "GetContract";
            public const string CheckCurrencySign = "CheckCurrencySign";
            public const string CurrencySignOperationLog = "CurrencySignOperationLog";
            public const string GetMyContractLog = "GetMyContractLog";
            public const string GetRootWorkFlowLists = "GetRootWorkFlowLists";
            public const string FlowList = "FlowList";
            public const string LoadDetailByPfid = "GetStepDetailByParentId";
            public const string SetWorkFlows = "StartFlow";
            public const string GetCodeFormUrl = "GetCodeFormUrl";
            public const string AddWatermark = "AddWatermark";
            public const string WordToPDF = "WordToPDF";
            public const string AbandonedContract = "AbandonedContract";
            public const string GetcontractTotal = "GetcontractTotal";

            public const string Signing = "Signing";
            public const string DownloadContract = "DownloadContract";
            public const string LoadContractGroupLogs = "LoadContractGroupLogs";
            public const string CommonLog = "CommonLog";

        }

        public sealed class ContractTemplate
        {
            public const string LoadContractTemplate = "LoadContractTemplate";
            public const string GetContractTemplateForm = "GetContractTemplateForm";
            public const string SaveContractTemplate = "SaveContractTemplate";
            public const string DeleteContractTemplate = "DeleteContractTemplate";
            public const string DownloadContractTemplate = "DownloadContractTemplate";

        }
    }
}