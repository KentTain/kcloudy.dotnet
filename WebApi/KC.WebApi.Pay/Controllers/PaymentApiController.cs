using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Pay;
using KC.Service;
using KC.Service.DTO.Pay;
using KC.Service.DTO;
using System.Reflection;
using KC.Enums.Pay;
using KC.Model.Pay;

namespace KC.WebApi.Pay.Controllers
{
    /// <summary>
    /// 文档管理
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class PaymentApiController : Web.Controllers.WebApiBaseController
    {
        private IPaymentService PaymentService => ServiceProvider.GetService<IPaymentService>();
        
        public PaymentApiController(
            Tenant tenant,
            IServiceProvider serviceProvider,
            ILogger<PaymentApiController> logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 是否开通支付
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="thirdPartyType"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ServiceResult<bool> IsOpenPayment(string tenantName)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  var result = PaymentService.IsOpenPayment(tenantName, ThirdPartyType.CPCNConfigSign);
                  return result;
              },
              Logger);
        }

        /// <summary>
        /// 是否设置了交易手机或者密码
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ServiceResult<bool> IsOpenPaymentPhone(string tenantName)
        {
            var result = new ServiceResult<bool>();
            result.success = true;
            result.Result = true;
            var paymentinfo = PaymentService.GetPaymentInfo(tenantName, ThirdPartyType.CPCNConfigSign);
            if (paymentinfo != null && !string.IsNullOrEmpty(paymentinfo.Phone) && !string.IsNullOrEmpty(paymentinfo.TradePassword))
            {
                result.message = paymentinfo.Phone;
            }
            else
            {
                result.message = "请先设置交易密码和验证手机号再进行操作";
                result.success = false;
                result.Result = false;
            }
            return result;
        }

        /// <summary>
        /// 根据PaymentInfoId 保存交易手机号
        /// </summary>
        /// <param name="paymentInfoId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost]
        public ServiceResult<bool> SavePaymentPhoneById(int paymentInfoId, string phone)
        {
            return ServiceWrapper.Invoke(
               ServiceName,
               MethodBase.GetCurrentMethod().Name,
               () =>
               {
                   return PaymentService.SavePaymentPhoneById(paymentInfoId, phone);
               }, Logger);
        }

        /// <summary>
        /// 根据PaymentInfoId 保存交易密码
        /// </summary>
        /// <param name="paymentInfoId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public ServiceResult<bool> SavePaymentPasswordById(int paymentInfoId, string password)
        {
            return ServiceWrapper.Invoke(
               ServiceName,
               MethodBase.GetCurrentMethod().Name,
               () =>
               {
                   return PaymentService.SavePaymentPasswordById(paymentInfoId, password);
               }, Logger);
        }

        /// <summary>
        /// 获取支付信息
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        [HttpGet]
        public ServiceResult<PaymentInfoDTO> GetPaymentInfo(string tenantName, ThirdPartyType thirdPartyType)
        {
            return ServiceWrapper.Invoke(
             ServiceName,
             MethodBase.GetCurrentMethod().Name,
             () =>
             {
                 var result = PaymentService.GetPaymentInfo(tenantName, thirdPartyType);
                 return result;
             }, Logger);
        }

        /// <summary>
        /// 获取银行信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResult<BankAccountDTO> GetBankAccountById(int id, string memberId)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  var result = PaymentService.GetBankAccountById(id, memberId);
                  return result;
              }, Logger);
        }

        /// <summary>
        /// 获取绑定的银行卡信息
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ServiceResult<BankAccountDTO> GetBindBankAccount(string memberId, ThirdPartyType type = ThirdPartyType.CPCNConfigSign)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  var result = PaymentService.GetBindBankAccount(memberId, type);
                  return result;
              }, Logger);
        }



        /// <summary>
        /// 根据Id删除银行卡 软删除
        /// </summary>
        /// <param name="BankId"></param>
        /// <returns></returns>
        [HttpPost]
        public ServiceResult<bool> RemoveBankById(int BankId)
        {
            return ServiceWrapper.Invoke(
                ServiceName,
                MethodBase.GetCurrentMethod().Name,
                () =>
                {
                    return PaymentService.RemoveBankById(BankId);
                }, Logger);
        }

        /// <summary>
        /// 修改银行卡
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public ServiceResult<bool> SaveBankAccount(BankAccountDTO bankAccount)
        {
            return ServiceWrapper.Invoke(
               ServiceName,
               MethodBase.GetCurrentMethod().Name,
               () =>
               {
                   return PaymentService.SaveBankAccount(bankAccount);
               }, Logger);
        }

        /// <summary>
        /// 根据银行卡号获取银行列表
        /// </summary>
        /// <param name="bankNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public ServiceResult<List<BankAccount>> GetMemberBankByBankNumber(string bankNumber)
        {
            return ServiceWrapper.Invoke(
               ServiceName,
               MethodBase.GetCurrentMethod().Name,
               () =>
               {
                   return PaymentService.GetMemberBankByBankNumber(bankNumber);
               }, Logger);
        }

        /// <summary>
        /// 获取银行账号信息
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        [HttpGet]
        public ServiceResult<List<BankAccount>> GetBankAccountByMemberId(string memberId, string searchInfo)
        {
            return ServiceWrapper.Invoke(
             ServiceName,
             MethodBase.GetCurrentMethod().Name,
             () =>
             {
                 var result = PaymentService.GetBankAccountByMemberId(memberId, searchInfo);
                 return result;
             }, Logger);
        }

        /// <summary>
        /// 获取平台账户
        /// </summary>
        /// <param name="thirdPartyType"></param>
        /// <returns></returns>
        [HttpGet]
        public ServiceResult<PaymentBankAccountDTO> GetPlatformAccount(ThirdPartyType thirdPartyType)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  return PaymentService.GetPlatformAccount(thirdPartyType);
              }, Logger);
        }

        /// <summary>
        /// 根据租户获取支付账号信息
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public ServiceResult<PaymentBankAccountDTO> GetPaymentBankAccountByMemberId(string memberId, ThirdPartyType type = ThirdPartyType.CPCNConfigSign)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  var result = PaymentService.GetPaymentBankAccountByMemberId(memberId, type);
                  return result;
              }, Logger);
        }

        /// <summary>
        /// 根据租户获取账号信息
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public ServiceResult<List<PaymentBankAccountDTO>> GetPaymentAccounts(string memberId)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  var result = PaymentService.GetPaymentAccounts(memberId);
                  return result;
              }, Logger);
        }

        public ServiceResult<Dictionary<string, List<ThirdPartyType>>> GetMembersPaymentAccountTypes(string memberIds)
        {
            var postData = KC.Common.SerializeHelper.FromJson<List<string>>(memberIds);
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  var result = PaymentService.GetMembersPaymentAccountTypes(postData);
                  return result;
              }, Logger);
        }

        

        /// <summary>
        /// 查询最后一条成功的流水记录
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="operationType"></param>
        /// <param name="referenceId"></param>
        /// <returns></returns>
        public ServiceResult<PaymentTradeRecord> GetTradeRecordsByLastSuccessRecord(string memberId, PaymentOperationType operationType, int referenceId)
        {
            return ServiceWrapper.Invoke(
            ServiceName,
            MethodBase.GetCurrentMethod().Name,
            () =>
            {
                return PaymentService.GetTradeRecordsByLastSuccessRecord(memberId, operationType, referenceId);
            }, Logger);
        }


        /// <summary>
        /// 根据支付订单Id获取支付记录
        /// </summary>
        /// <param name="paymentOrderId"></param>
        /// <returns></returns>
        [HttpGet]
        public ServiceResult<OnlinePaymentRecordDTO> GetOnlinePaymentRecordByPaymentId(string paymentOrderId)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  return PaymentService.GetOnlinePaymentRecordByPaymentId(paymentOrderId);
              }, Logger);
        }


        /// <summary>
        /// 根据paymentOrderId 检查该支付订单是否支付成功
        /// </summary>
        /// <param name="paymentOrderId"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ServiceResult<bool> IsOnlinePaymentSuccess(string paymentOrderId)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  return PaymentService.IsOnlinePaymentSuccess(paymentOrderId);
              }, Logger);
        }

        /// <summary>
        /// 检查默认的支付是否开通
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpGet]
        public ServiceResult<bool> CheckDefalutPayOpen(string memberId)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  return PaymentService.CheckDefalutPayOpen(memberId);
              }, Logger);
        }

        /// <summary>
        ///  根据MembrId获取租户来源
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        //[HttpGet]
        //public ServiceResult<MemberSource> GetMemberSourceByMemberId(string memberId)
        //{
        //    return ServiceWrapper.Invoke(
        //        ServiceName,
        //        MethodBase.GetCurrentMethod().Name,
        //        () =>
        //        {
        //            return PaymentService.GetMemberSourceByMemberId(memberId);
        //        }, Logger);
        //}

        

        

        /// <summary>
        /// 根据条件获取OnlinePaymentRecord表的记录
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="memberId"></param>
        /// <param name="peeMemberId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> GetOnlinePaymentRecordByPage(DateTime? beginTime, DateTime? endTime, string memberId = "", string peeMemberId = "", int pageIndex = 1, int pageSize = 10)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  return PaymentService.GetOnlinePaymentRecordByPage(beginTime, endTime, memberId, peeMemberId, pageIndex, pageSize);
              }, Logger);
        }

        /// <summary>
        /// 根据条件获取OnlinePaymentRecord表的记录
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isExpended"></param>
        /// <param name="memberId"></param>
        /// <param name="memberIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> GetOnlinePaymentRecordsByFilter(DateTime? beginTime, DateTime? endTime, bool isExpended = false, string memberId = "", string memberIds = null, int pageIndex = 1, int pageSize = 10)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  var data = KC.Common.SerializeHelper.FromJson<List<string>>(memberIds);
                  return PaymentService.GetOnlinePaymentRecordsByFilter(beginTime, endTime, isExpended, memberId, data, pageIndex, pageSize);
              }, Logger);
        }

        /// <summary>
        /// 根据条件获取OnlinePaymentRecord表的记录
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public ServiceResult<PaginatedBaseDTO<OnlinePaymentRecordDTO>> GetOnlinePaymentRecordsTop10(string memberId, int pageIndex = 1, int pageSize = 10)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  return PaymentService.GetOnlinePaymentRecordsTop10(memberId, pageIndex, pageSize);
              }, Logger);
        }

        /// <summary>
        /// 根据订单编号获取现金支付体系
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        //[HttpGet]
        //public ServiceResult<ThirdPartyType> GetCashUsageDetailType(string orderId)
        //{
        //    return ServiceWrapper.Invoke(
        //      ServiceName,
        //      MethodBase.GetCurrentMethod().Name,
        //      () =>
        //      {
        //          return OnlinePaymentRecordService.GetCashUsageDetailType(orderId);
        //      }, Logger);
        //}

        /// <summary>
        /// 根据支付电脑 金额  支付宝/微信 查找是否已经提交扫码入金
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="amount"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        [HttpGet]
        public ServiceResult<OnlinePaymentRecordDTO> GetOnlinePaymentRecordByOrderNoAndAmount(string orderNo, Decimal amount, string bankName)
        {
            return ServiceWrapper.Invoke(
              ServiceName,
              MethodBase.GetCurrentMethod().Name,
              () =>
              {
                  return PaymentService.GetOnlinePaymentRecordByOrderNoAndAmount(orderNo, amount, bankName);
              }, Logger);
        }
    }
}
