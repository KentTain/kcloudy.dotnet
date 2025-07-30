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
using KC.Service.DTO.Config;

namespace KC.Service.Pay.WebApiService.Platform
{
    public class PaymentApiService : IdSrvOAuth2ClientRequestBase, IPaymentApiService
    {
        private const string ServiceName = "KC.Service.Store.WebApiService.Platform.P_PaymentApiService";
        public PaymentApiService(
            Tenant tenant,
            System.Net.Http.IHttpClientFactory httpClient,
            Microsoft.Extensions.Logging.ILogger<PaymentApiService> logger)
            : base(tenant ?? TenantConstant.DbaTenantApiAccessInfo, httpClient, logger)
        {

        }

        /// <summary>
        /// 根据memberName 获取认证信息
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public ServiceResult<bool> IsOpenPayment(string tenantName)
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".IsOpenPayment",
                PaymentApiServerUrl + "PaymentService/IsOpenPayment?tenantName=" + tenantName,
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
        public ServiceResult<bool> IsOpenPaymentPhone(string tenantName)
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".IsOpenPaymentPhone",
                PaymentApiServerUrl + "PaymentService/IsOpenPaymentPhone?tenantName=" + tenantName,
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

        public ServiceResult<PaymentOperationLogDTO> GetPaymentInfo(string tenantName, ThirdPartyType thirdPartyType)
        {
            ServiceResult<PaymentOperationLogDTO> result = null;
            WebSendGet<ServiceResult<PaymentOperationLogDTO>>(
                ServiceName + ".GetPaymentInfo",
                PaymentApiServerUrl + "PaymentService/GetPaymentInfo?tenantName=" + tenantName + "&thirdPartyType=" + thirdPartyType,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaymentOperationLogDTO>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }


        public ServiceResult<List<BankAccountDTO>> GetBankAccountByMemberId(string memberId, string searchInfo)
        {
            ServiceResult<List<BankAccountDTO>> result = null;
            WebSendGet<ServiceResult<List<BankAccountDTO>>>(
                ServiceName + ".GetBankAccountByMemberId",
                PaymentApiServerUrl + "PaymentService/GetBankAccountByMemberId?memberId=" + memberId + "&searchInfo=" + searchInfo,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<BankAccountDTO>>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }
        public ServiceResult<BankAccountDTO> GetBindBankAccount(string memberId)
        {
            ServiceResult<BankAccountDTO> result = null;
            WebSendGet<ServiceResult<BankAccountDTO>>(
                ServiceName + ".GetBindBankAccount",
                PaymentApiServerUrl + "PaymentService/GetBindBankAccount?memberId=" + memberId,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<BankAccountDTO>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        public ServiceResult<bool> UpdatePaymentRecord(OnlinePaymentRecord entity)
        {
            ServiceResult<bool> result = null;
            var postData = SerializeHelper.ToJson(entity);
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".UpdatePaymentRecord",
                PaymentApiServerUrl + "SupplyChainService/UpdatePaymentRecord", postData,
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

        public ServiceResult<OnlinePaymentRecord> GetPlatPaymentRecord(OnlinePaymentRecord entity)
        {
            ServiceResult<OnlinePaymentRecord> result = null;
            var postData = SerializeHelper.ToJson(entity);
            WebSendPost<ServiceResult<OnlinePaymentRecord>>(
                ServiceName + ".GetPlatPaymentRecord",
                PaymentApiServerUrl + "SupplyChainService/GetPlatPaymentRecord", postData,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<OnlinePaymentRecord>(ServiceResultType.Error, errorMessage);
                }, true);

            return result;
        }

        public ServiceResult<ConfigEntityDTO> GetConfigEntityBySign(int sign, ConfigType type)
        {
            ServiceResult<ConfigEntityDTO> result = null;
            WebSendGet<ServiceResult<ConfigEntityDTO>>(
                ServiceName + ".GetConfigEntityBySign",
                PaymentApiServerUrl + "PaymentService/GetConfigEntityBySign?sign=" + sign + "&type=" + (int)type,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<ConfigEntityDTO>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        /// <summary>
        /// 根据memberName 获取认证信息
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public ServiceResult<UnitAuthenticationDTO> GetUnitAuthenticationByMemberName(string memberName)
        {
            ServiceResult<UnitAuthenticationDTO> result = null;
            WebSendGet<ServiceResult<UnitAuthenticationDTO>>(
                ServiceName + ".GetUnitAuthenticationByMemberName",
                PaymentApiServerUrl + "PaymentService/GetUnitAuthenticationByMemberName?memberName=" + memberName,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<UnitAuthenticationDTO>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        /// <summary>
        /// 根据支持的类型 获取中金支付支持的银行列表
        /// </summary>
        /// <param name="busiType"></param>
        /// <returns></returns>
        public ServiceResult<List<SupplyBankDTO>> GetSupplyBanksByBusiType(string busiType)
        {
            ServiceResult<List<SupplyBankDTO>> result = null;
            WebSendGet<ServiceResult<List<SupplyBankDTO>>>(
                ServiceName + ".GetSupplyBanksByBusiType",
                PaymentApiServerUrl + "PaymentService/GetSupplyBanksByBusiType?busiType=" + busiType,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<SupplyBankDTO>>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }


        /// <summary>
        /// 获取省份列表
        /// </summary>
        /// <returns></returns>
        public ServiceResult<List<StdProvinceDTO>> GetStrandardProvinceList()
        {
            ServiceResult<List<StdProvinceDTO>> result = null;
            WebSendGet<ServiceResult<List<StdProvinceDTO>>>(
                ServiceName + ".GetStrandardProvinceList",
                PaymentApiServerUrl + "PaymentService/GetStrandardProvinceList",
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<StdProvinceDTO>>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        /// <summary>
        /// 获取城市列表
        /// </summary>
        /// <param name="provinceID"></param>
        /// <returns></returns>
        public ServiceResult<List<StdCityDTO>> GetStrandardCityList(string provinceCode)
        {
            ServiceResult<List<StdCityDTO>> result = null;
            WebSendGet<ServiceResult<List<StdCityDTO>>>(
                ServiceName + ".GetStrandardCityList",
                PaymentApiServerUrl + "PaymentService/GetStrandardCityList?provinceCode=" + provinceCode,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<StdCityDTO>>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        public ServiceResult<List<BF_ZD_PAYBANKDTO>> GetPayBankList(string cityCode, string bankCode, string branchBankCode, string searchName)
        {
            ServiceResult<List<BF_ZD_PAYBANKDTO>> result = null;
            WebSendGet<ServiceResult<List<BF_ZD_PAYBANKDTO>>>(
                ServiceName + ".GetPayBankList",
                PaymentApiServerUrl + "PaymentService/GetPayBankList?cityCode=" + cityCode + "&bankCode=" + bankCode + "&branchBankCode=" + branchBankCode + "&searchName=" + searchName,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<BF_ZD_PAYBANKDTO>>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        /// <summary>
        /// 保存流水号信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ServiceResult<bool> SavePaymentTradeRecords(PaymentTradeRecordDTO entity)
        {
            ServiceResult<bool> result = null;
            var postData = SerializeHelper.ToJson(entity);
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".SavePaymentTradeRecords",
                PaymentApiServerUrl + "PaymentService/SavePaymentTradeRecords", postData,
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

        /// <summary>
        /// 保存虚拟户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ServiceResult<bool> SaveOpenAccount(OpenAccountDTO entity)
        {
            ServiceResult<bool> result = null;
            var postData = SerializeHelper.ToJson(entity);
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".SaveOpenAccount",
                PaymentApiServerUrl + "PaymentService/SaveOpenAccount", postData,
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

        /// <summary>
        /// 根据租户Id获取中金账户信息
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public ServiceResult<PaymentBankAccountDTO> GetPaymentBankAccountByMemberId(string memberId, ThirdPartyType type = ThirdPartyType.CPCNConfigSign)
        {
            ServiceResult<PaymentBankAccountDTO> result = null;
            WebSendGet<ServiceResult<PaymentBankAccountDTO>>(
                ServiceName + ".GetPaymentBankAccountByMemberId",
                PaymentApiServerUrl + "PaymentService/GetPaymentBankAccountByMemberId?memberId=" + memberId + "&type=" + type,
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

        /// <summary>
        /// 根据租户Id获取账户列表
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public ServiceResult<List<PaymentBankAccountDTO>> GetPaymentAccounts(string memberId)
        {
            ServiceResult<List<PaymentBankAccountDTO>> result = null;
            WebSendGet<ServiceResult<List<PaymentBankAccountDTO>>>(
                ServiceName + ".GetPaymentAccounts",
                PaymentApiServerUrl + "PaymentService/GetPaymentAccounts?memberId=" + memberId,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<PaymentBankAccountDTO>>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        /// <summary>
        /// 根据银行卡ID获取银行信息
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <returns></returns>
        public ServiceResult<BankAccountDTO> GetBankAccountById(int bankAccountId)
        {
            ServiceResult<BankAccountDTO> result = null;
            WebSendGet<ServiceResult<BankAccountDTO>>(
                ServiceName + ".GetBankAccountById",
                PaymentApiServerUrl + "PaymentService/GetBankAccountById?id=" + bankAccountId + "&memberId=" + Tenant.TenantName,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<BankAccountDTO>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        public ServiceResult<bool> RemoveBankById(int id)
        {
            ServiceResult<bool> result = null;
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".RemoveBankById",
                PaymentApiServerUrl + "PaymentService/RemoveBankById?BankId=" + id, "",
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

        public ServiceResult<bool> SaveBankAccount(BankAccountDTO bankAccount)
        {
            ServiceResult<bool> result = null;
            var postData = SerializeHelper.ToJson(bankAccount);
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".SaveBankAccount",
                PaymentApiServerUrl + "PaymentService/SaveBankAccount", postData,
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
        public ServiceResult<List<BankAccountDTO>> GetMemberBankByBankNumber(string bankNumber)
        {
            ServiceResult<List<BankAccountDTO>> result = null;
            WebSendGet<ServiceResult<List<BankAccountDTO>>>(
                ServiceName + ".GetMemberBankByBankNumber",
                PaymentApiServerUrl + "PaymentService/GetMemberBankByBankNumber?bankNumber=" + bankNumber,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<BankAccountDTO>>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }


        /// <summary>
        /// 保存银行验证申请信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ServiceResult<bool> SaveBankAuthenticationAppliction(BankAuthenticationApplicationDTO entity)
        {
            ServiceResult<bool> result = null;
            var postData = SerializeHelper.ToJson(entity);
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".SaveBankAuthenticationAppliction",
                PaymentApiServerUrl + "PaymentService/SaveBankAuthenticationAppliction", postData,
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

        /// <summary>
        /// 查询最后一条成功的流水记录
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="operationType"></param>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        public ServiceResult<PaymentTradeRecordDTO> GetTradeRecordsByLastSuccessRecord(string memberId, PaymentOperationType operationType, int referenceId)
        {
            ServiceResult<PaymentTradeRecordDTO> result = null;
            WebSendGet<ServiceResult<PaymentTradeRecordDTO>>(
                ServiceName + ".GetTradeRecordsByLastSuccessRecord",
                PaymentApiServerUrl + "PaymentService/GetTradeRecordsByLastSuccessRecord?memberId=" + memberId + "&operationType=" + operationType + "&referenceId=" + referenceId,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<PaymentTradeRecordDTO>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        /// <summary>
        /// 绑定银行卡后的操作
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ServiceResult<bool> BindBankAccount(BindBankAccountDTO entity)
        {
            ServiceResult<bool> result = null;
            var postData = SerializeHelper.ToJson(entity);
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".BindBankAccount",
                PaymentApiServerUrl + "PaymentService/BindBankAccount", postData,
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

        /// <summary>
        /// 根据支付订单Id获取支付记录
        /// </summary>
        /// <param name="paymentOrderId"></param>
        /// <returns></returns>
        public ServiceResult<OnlinePaymentRecordDTO> GetOnlinePaymentRecordByPaymentId(string paymentOrderId)

        {
            ServiceResult<OnlinePaymentRecordDTO> result = null;
            WebSendGet<ServiceResult<OnlinePaymentRecordDTO>>(
                ServiceName + ".GetOnlinePaymentRecordByPaymentId",
                PaymentApiServerUrl + "PaymentService/GetOnlinePaymentRecordByPaymentId?paymentOrderId=" + paymentOrderId,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<OnlinePaymentRecordDTO>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }

        public ServiceResult<bool> SavePaymentPhoneById(int paymentInfoId, string phone)
        {
            ServiceResult<bool> result = null;
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".SavePaymentPhoneById",
                PaymentApiServerUrl + "PaymentService/SavePaymentPhoneById?paymentInfoId=" + paymentInfoId + "&phone=" + phone, "",
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

        public ServiceResult<bool> SavePaymentPasswordById(int paymentInfoId, string password)
        {
            ServiceResult<bool> result = null;
            WebSendPost<ServiceResult<bool>>(
                ServiceName + ".SavePaymentPasswordById",
                PaymentApiServerUrl + "PaymentService/SavePaymentPasswordById?paymentInfoId=" + paymentInfoId + "&password=" + password, "",
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

        public ServiceResult<bool> IsOnlinePaymentSuccess(string paymentOrderId)
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".IsOnlinePaymentSuccess",
                PaymentApiServerUrl + "PaymentService/IsOnlinePaymentSuccess?paymentOrderId=" + paymentOrderId,
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

        /// <summary>
        /// 根据MembrId获取租户来源
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        //public ServiceResult<MemberSource> GetMemberSourceByMemberId(string memberId)
        //{
        //    ServiceResult<MemberSource> result = null;
        //    WebSendGet<ServiceResult<MemberSource>>(
        //        ServiceName + ".GetMemberSourceByMemberId",
        //        PaymentApiServerUrl + "PaymentService/GetMemberSourceByMemberId?memberId=" + memberId,
        //        ApplicationConstant.PayScope,
        //        callback =>
        //        {
        //            result = callback;
        //        },
        //        (httpStatusCode, errorMessage) =>
        //        {
        //            result = new ServiceResult<MemberSource>(ServiceResultType.Error, errorMessage);
        //        }, true);
        //    return result;
        //}

        /// <summary>
        /// 检查默认的支付是否开通
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public ServiceResult<bool> CheckDefalutPayOpen(string memberId)
        {
            ServiceResult<bool> result = null;
            WebSendGet<ServiceResult<bool>>(
                ServiceName + ".CheckDefalutPayOpen",
                PaymentApiServerUrl + "PaymentService/CheckDefalutPayOpen?memberId=" + memberId,
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

        /// <summary>
        /// 根据配置类型获取配置List
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ServiceResult<List<ConfigEntity>> GetConfigsByType(ConfigType type)
        {
            ServiceResult<List<ConfigEntity>> result = null;
            WebSendGet<ServiceResult<List<ConfigEntity>>>(
                ServiceName + ".GetConfigsByType",
                PaymentApiServerUrl + "PaymentService/GetConfigsByType?type=" + type,
                ApplicationConstant.PayScope,
                callback =>
                {
                    result = callback;
                },
                (httpStatusCode, errorMessage) =>
                {
                    result = new ServiceResult<List<ConfigEntity>>(ServiceResultType.Error, errorMessage);
                }, true);
            return result;
        }
    }
}
