using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Common;
using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.Pay;
using KC.Service;

using KC.Service.DTO.Pay;
using KC.Service.WebApiService.Business;
using KC.Service.DTO;
using KC.Service.DTO.Dict;
using KC.Common.ToolsHelper;

namespace KC.Service.Pay.WebApiService.Platform
{
    public class FinanceApiService : IdSrvOAuth2ClientRequestBase, IFinanceApiService
    {
        private const string ServiceName = "KC.Service.Store.WebApiService.Platform.FinanceApiService";
        public FinanceApiService(
            Tenant tenant,
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<FinanceApiService> logger)
            : base(tenant ?? TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {

        }

        public ServiceResult<PaginatedBaseDTO<ChargeFlowDTO>> GetRechargeRecords(DateTime? startDate, DateTime? endDate, RechargeStatus? status, int pageIndex = 1, int pageSize = 10)
        {
            ServiceResult<PaginatedBaseDTO<ChargeFlowDTO>> result = null;
            WebSendGet<ServiceResult<PaginatedBaseDTO<ChargeFlowDTO>>>(
                ServiceName + ".GetRechargeRecords",
                PaymentApiServerUrl + "Finance/GetRechargeRecords?startDate=" + startDate + "&endDate=" + endDate + "&status=" + status + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<ChargeFlowDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<PaginatedBaseDTO<ChargeFlowDTO>> GetWithdrawalsRecords(DateTime? startDate, DateTime? endDate, WithdrawalsStatus? status, int pageIndex = 1, int pageSize = 10)
        {
            ServiceResult<PaginatedBaseDTO<ChargeFlowDTO>> result = null;
            WebSendGet<ServiceResult<PaginatedBaseDTO<ChargeFlowDTO>>>(
                ServiceName + ".GetWithdrawalsRecords",
                PaymentApiServerUrl + "Finance/GetWithdrawalsRecords?startDate=" + startDate + "&endDate=" + endDate + "&status=" + status + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<ChargeFlowDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<PaginatedBaseDTO<PlatformServiceFeeDTO>> GetPlatformServiceFeeRecords(bool? isPay, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10)
        {
            ServiceResult<PaginatedBaseDTO<PlatformServiceFeeDTO>> result = null;
            WebSendGet<ServiceResult<PaginatedBaseDTO<PlatformServiceFeeDTO>>>(
                ServiceName + ".GetPlatformServiceFeeRecords",
                PaymentApiServerUrl + "Finance/GetPlatformServiceFeeRecords?isPay=" + isPay + "&startDate=" + startDate + "&endDate=" + endDate + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<PlatformServiceFeeDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<decimal> GetNeedPayServiceFeeAmount()
        {
            ServiceResult<decimal> result = null;
            WebSendGet<ServiceResult<decimal>>(
                ServiceName + ".GetNeedPayServiceFeeAmount",
                PaymentApiServerUrl + "Finance/GetNeedPayServiceFeeAmount",
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<decimal>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<Tuple<decimal, decimal>> GetAssetSummary()
        {
            ServiceResult<Tuple<decimal, decimal>> result = null;
            WebSendGet<ServiceResult<Tuple<decimal, decimal>>>(
                ServiceName + ".GetAssetSummary",
                PaymentApiServerUrl + "Finance/GetAssetSummary",
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<Tuple<decimal, decimal>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<decimal> GetTodayWithdrawAmount()
        {
            ServiceResult<decimal> result = null;
            WebSendGet<ServiceResult<decimal>>(
                ServiceName + ".GetTodayWithdrawAmount",
                PaymentApiServerUrl + "Finance/GetTodayWithdrawAmount",
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<decimal>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<List<WithdrawRuleDTO>> GetWithdrawRule()
        {
            ServiceResult<List<WithdrawRuleDTO>> result = null;
            WebSendGet<ServiceResult<List<WithdrawRuleDTO>>>(
                ServiceName + ".GetWithdrawRule",
                PaymentApiServerUrl + "Finance/GetWithdrawRule",
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<WithdrawRuleDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }


        public ServiceResult<List<ElectronicBillRecordsDTO>> QueryPlatElectronicBill(DateTime startDate,
            DateTime endDate, string supplierId, string buyerId, List<string> alreadyReconcileds)
        {
            ServiceResult<List<ElectronicBillRecordsDTO>> result = null;
            var postData = SerializeHelper.ToJson(alreadyReconcileds);
            WebSendPost<ServiceResult<List<ElectronicBillRecordsDTO>>>(
                ServiceName + ".QueryPlatElectronicBill",
                PaymentApiServerUrl + "Finance/QueryPlatElectronicBill?startDate=" + startDate + "&endDate=" + endDate + "&supplierId=" + supplierId + "&buyerId=" + buyerId,
                ApplicationConstant.PayScope,
                postData,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<ElectronicBillRecordsDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<List<ElectronicBillRecordsDTO>> GetElectronicBillRecords(string orderId)
        {
            ServiceResult<List<ElectronicBillRecordsDTO>> result = null;
            WebSendGet<ServiceResult<List<ElectronicBillRecordsDTO>>>(
                ServiceName + ".GetElectronicBillRecords",
                PaymentApiServerUrl + "Finance/GetElectronicBillRecords?orderId=" + orderId,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<ElectronicBillRecordsDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<List<ElectronicBillRecordsDTO>> GetElectronicBillRecordsOfAccountStatement(Guid id)
        {
            ServiceResult<List<ElectronicBillRecordsDTO>> result = null;
            WebSendGet<ServiceResult<List<ElectronicBillRecordsDTO>>>(
                ServiceName + ".GetElectronicBillRecordsOfAccountStatement",
                PaymentApiServerUrl + "Finance/GetElectronicBillRecordsOfAccountStatement?id=" + id,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<ElectronicBillRecordsDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<List<JsonDictionaryDTO>> GetOrderStillNeedPayAmount(List<string> orders)
        {
            ServiceResult<List<JsonDictionaryDTO>> result = null;
            WebSendPost<ServiceResult<List<JsonDictionaryDTO>>>(
                ServiceName + ".GetOrderStillNeedPayAmount",
                PaymentApiServerUrl + "Finance/QueryOrderStillNeedPayAmount", SerializeHelper.ToJson(orders),
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<JsonDictionaryDTO>>(ServiceResultType.Error, errorMessage);
                });

            return result;
        }


        public ServiceResult<PaginatedBaseDTO<PaymentRecordDTO>> GetCashReceipts(string customer, string num,
           DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10)
        {
            ServiceResult<PaginatedBaseDTO<PaymentRecordDTO>> result = null;
            WebSendGet<ServiceResult<PaginatedBaseDTO<PaymentRecordDTO>>>(
                ServiceName + ".GetCashReceipts",
                PaymentApiServerUrl + "Finance/GetCashReceipts?customer=" + customer + "&num=" + num + "&startDate=" + startDate + "&endDate=" + endDate + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<PaymentRecordDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<PaginatedBaseDTO<PaymentRecordDTO>> GetCashExpenditure(string vendor, string num,
          DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10)
        {
            ServiceResult<PaginatedBaseDTO<PaymentRecordDTO>> result = null;
            WebSendGet<ServiceResult<PaginatedBaseDTO<PaymentRecordDTO>>>(
                ServiceName + ".GetCashExpenditure",
                PaymentApiServerUrl + "Finance/GetCashExpenditure?vendor=" + vendor + "&num=" + num + "&startDate=" + startDate + "&endDate=" + endDate + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<PaymentRecordDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<PaginatedBaseDTO<ElectronicBillDTO>> GetUsedBill(string vendor, string num,
         DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10)
        {
            ServiceResult<PaginatedBaseDTO<ElectronicBillDTO>> result = null;
            WebSendGet<ServiceResult<PaginatedBaseDTO<ElectronicBillDTO>>>(
                ServiceName + ".GetUsedBill",
                PaymentApiServerUrl + "Finance/GetUsedBill?vendor=" + vendor + "&num=" + num + "&startDate=" + startDate + "&endDate=" + endDate + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<ElectronicBillDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<PaginatedBaseDTO<ElectronicBillDTO>> GetBill(string customer, string num,
         DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10)
        {
            ServiceResult<PaginatedBaseDTO<ElectronicBillDTO>> result = null;
            WebSendGet<ServiceResult<PaginatedBaseDTO<ElectronicBillDTO>>>(
                ServiceName + ".GetBill",
                PaymentApiServerUrl + "Finance/GetBill?customer=" + customer + "&num=" + num + "&startDate=" + startDate + "&endDate=" + endDate + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<ElectronicBillDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<List<UnionBankNumberDTO>> QueryUnionBankNumber(int bankId, int cityCode)
        {
            ServiceResult<List<UnionBankNumberDTO>> result = null;
            WebSendGet<ServiceResult<List<UnionBankNumberDTO>>>(
                ServiceName + ".QueryUnionBankNumber",
                PaymentApiServerUrl + "Finance/QueryUnionBankNumber?bankId=" + bankId + "&cityCode=" + cityCode,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<UnionBankNumberDTO>>(ServiceResultType.Error, errorMessage);
                });

            return result;
        }

        public ServiceResult<bool> QueryWaitSigninBill()
        {
            ServiceResult<bool> result = null;
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".QueryWaitSigninBill",
                PaymentApiServerUrl + "Finance/QueryWaitSigninBill", null,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, errorMessage);
                });

            return result;
        }

        public ServiceResult<PaginatedBaseDTO<JsonDictionaryDTO>> LoadFrozenRecords(int pageIndex = 1, int pageSize = 10, string order = "asc")
        {
            ServiceResult<PaginatedBaseDTO<JsonDictionaryDTO>> result = null;
            WebSendPost<ServiceResult<PaginatedBaseDTO<JsonDictionaryDTO>>>(
                ServiceName + ".LoadFrozenRecords",
                PaymentApiServerUrl + "Finance/LoadFrozenRecords?memberId=" + Tenant.TenantName + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize + "&order=" + order, null,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<JsonDictionaryDTO>>(ServiceResultType.Error, errorMessage);
                });

            return result;
        }

        public ServiceResult<bool> ReleaseFreezeCapital(decimal amount, string orderId)
        {
            var currentDate = DateTime.UtcNow;
            var timestamp = OtherUtilHelper.ConvertDateTimeToTimeStamp(currentDate);
            ServiceResult<bool> result = null;
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".ReleaseFreezeCapital",
                PaymentApiServerUrl + "Finance/ReleaseFreezeCapital?amount=" + amount + "&memberId=" + Tenant.TenantName +
                "&orderId=" + orderId + "&timestamp=" + timestamp, null,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, errorMessage);
                });
            return result;
        }

        public ServiceResult<decimal> GetFrozenByOrderId(string orderId)
        {
            ServiceResult<decimal> result = null;
            WebSendGet<ServiceResult<decimal>>(
                ServiceName + ".GetFrozenByOrderId",
                PaymentApiServerUrl + "Finance/GetFrozenByOrderId?orderId=" + orderId,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<decimal>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        public ServiceResult<bool> ApplyForRecharge(decimal amount, string orderid, bool? state, string errormessage = "")
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".ApplyForRecharge",
                PaymentApiServerUrl + "Finance/ApplyForRecharge?amount=" + amount + "&orderid=" + orderid + "&state=" + state + "&errormessage=" + errormessage,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<bool>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }
        public ServiceResult<Tuple<bool, string>> ApplyForWithdrawal(decimal amount, string billNo, bool? state, string errormessage = "", int paymentdate = 0)
        {
            ServiceResult<Tuple<bool, string>> result = null;

            WebSendGet<ServiceResult<Tuple<bool, string>>>(
                ServiceName + ".ApplyForWithdrawal",
                PaymentApiServerUrl + "Finance/ApplyForWithdrawal?amount=" + amount + "&billNo=" + billNo + "&state=" + state + "&errormessage=" + errormessage + "&paymentdate=" + paymentdate,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<Tuple<bool, string>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }


        /// <summary>
        /// 获取平台的账号信息
        /// </summary>
        /// <param name="thirdPartyType"></param>
        /// <returns></returns>
        public ServiceResult<PaymentBankAccountDTO> GetPlatformAccount(ThirdPartyType thirdPartyType)
        {
            ServiceResult<PaymentBankAccountDTO> result = null;
            WebSendGet<ServiceResult<PaymentBankAccountDTO>>(
                ServiceName + ".GetPlatformAccount",
                PaymentApiServerUrl + "PaymentService/GetPlatformAccount?thirdPartyType=" + thirdPartyType,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaymentBankAccountDTO>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        public ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> GetOnlinePaymentRecordByPage(DateTime? beginTime, DateTime? endTime, string memberId = "", string peeMemberId = "", int pageIndex = 1, int pageSize = 10)
        {
            ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> result = null;
            var url = "PaymentService/GetOnlinePaymentRecordByPage";
            url = url + "?beginTime=" + beginTime;
            url = url + "&endTime=" + endTime;
            url = url + "&memberId=" + memberId;
            url = url + "&peeMemberId=" + peeMemberId;
            url = url + "&pageIndex=" + pageIndex;
            url = url + "&pageSize=" + pageSize;
            WebSendGet<ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>>>(
                ServiceName + ".GetOnlinePaymentRecordByPage",
                PaymentApiServerUrl + url,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>>();
                }, true);
            return result;
        }

        public ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> GetOnlinePaymentRecordsByFilter(DateTime? beginTime, DateTime? endTime, bool isExpended, string memberId = "", List<string> memberIds = null, int pageIndex = 1, int pageSize = 10)
        {
            var postData = memberIds == null ? string.Empty : SerializeHelper.ToJson(memberIds);
            ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> result = null;
            var url = "PaymentService/GetOnlinePaymentRecordsByFilter";
            url = url + "?beginTime=" + beginTime;
            url = url + "&endTime=" + endTime;
            url = url + "&isExpended=" + isExpended;
            url = url + "&memberId=" + memberId;
            url = url + "&memberIds=" + postData;
            url = url + "&pageIndex=" + pageIndex;
            url = url + "&pageSize=" + pageSize;
            WebSendGet<ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>>>(
                ServiceName + ".GetOnlinePaymentRecordsByFilter",
                PaymentApiServerUrl + url,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>>();
                }, true);
            return result;
        }

        public ServiceResult<PaginatedBaseDTO<CashUsageDetailDTO>> LoadTransactionFlows(CashType? type, TradingAccount? account, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10)
        {
            ServiceResult<PaginatedBaseDTO<CashUsageDetailDTO>> result = null;
            WebSendGet<ServiceResult<PaginatedBaseDTO<CashUsageDetailDTO>>>(
                ServiceName + ".LoadCashUsages",
                PaymentApiServerUrl + "Finance/LoadCashUsages?type=" + type + "&account=" + account + "&startDate=" + startDate + "&endDate=" + endDate + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<CashUsageDetailDTO>>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> GetOnlinePaymentRecordsTop10(string memberId, int pageIndex = 1, int pageSize = 10)
        {
            ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> result = null;
            var apiUrl = string.Format("{0}PaymentService/GetOnlinePaymentRecordsTop10?memberId={1}&pageIndex={2}&pageSize{3}", PaymentApiServerUrl, memberId, pageIndex, pageSize);
            WebSendGet<ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>>>(
                ServiceName + ".GetOnlinePaymentRecordsTop10",
                apiUrl,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>>();
                }, true);
            return result;
        }

        public ServiceResult<ThirdPartyType> GetCashUsageDetailType(string orderId)
        {
            ServiceResult<ThirdPartyType> result = null;
            WebSendGet<ServiceResult<ThirdPartyType>>(
                ServiceName + ".GetCashUsageDetailType",
                PaymentApiServerUrl + "PaymentService/GetCashUsageDetailType?orderId=" + orderId,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<ThirdPartyType>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }
    }
}
