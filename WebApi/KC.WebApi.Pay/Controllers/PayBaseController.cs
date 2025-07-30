using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KC.Service.DTO.Pay;
using KC.Enums.Pay;
using KC.Framework.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KC.WebApi.Pay.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public abstract class PayBaseController : Web.Controllers.WebApiBaseController
    {
        public PayBaseController(
            Tenant tenant, IServiceProvider serviceProvider,
            ILogger logger)
            : base(tenant, serviceProvider, logger)
        {
        }

        /// <summary>
        /// 拼接流水记录
        /// </summary>
        /// <param name="ptnSrl">系统流水号</param>
        /// <param name="retSrl">中金返回流水号</param>
        /// <param name="interfaceName">接口名</param>
        /// <param name="paymentOperationType">操作类型</param>
        /// <param name="isSuccess">请求是否成功</param>
        /// <param name="message">返回的信息</param>
        /// <param name="retCode">返回码</param>
        /// <returns></returns>
        public PaymentTradeRecordDTO AddPaymentTradeRecord(string postXML, string returnXML, string memberId, string ptnSrl, string retSrl, string interfaceName,
            PaymentOperationType paymentOperationType, bool isSuccess, string message, string retCode, ThirdPartyType thirdPartyType, string userName)
        {
            PaymentTradeRecordDTO tradeRecord = new PaymentTradeRecordDTO();
            tradeRecord.SrlNo = ptnSrl;
            tradeRecord.RetSrlNo = retSrl;
            tradeRecord.MemberId = memberId;
            tradeRecord.InterfaceName = interfaceName;
            tradeRecord.OperationType = paymentOperationType;
            tradeRecord.IsSuccess = isSuccess;
            tradeRecord.Message = message;
            tradeRecord.RetCode = retCode;
            tradeRecord.PaymentType = thirdPartyType;
            tradeRecord.CreatedBy = userName;
            tradeRecord.PostXML = postXML;
            tradeRecord.ReturnXML = returnXML;
            return tradeRecord;
        }

        /// <summary>
        /// 添加OnlinePaymentRecords
        /// </summary>
        /// <param name="paymentOrderId">支付订单号</param>
        /// <param name="orderNo">业务订单号</param>
        /// <param name="orderAmount">金额</param>
        /// <param name="paymentAmount">金额</param>
        /// <param name="orderDateTime">支付订单时间</param>
        /// <param name="payDateTime">支付时间</param>
        /// <param name="payResult">支付结果</param>
        /// <param name="errorCode">支付代码</param>
        /// <param name="bankNumber">流水号</param>
        /// <param name="configId">配置Id</param>
        /// <returns></returns>
        public OnlinePaymentRecordDTO AddOnlinePaymentRecords(string paymentOrderId, string orderNo, decimal orderAmount, decimal paymentAmount, string orderDateTime, string payDateTime,
            string payResult, string errorCode, string bankNumber, int configId, string memberId, string paymentMethod, ThirdPartyType payType)
        {
            OnlinePaymentRecordDTO onlinePaymentRecord = new OnlinePaymentRecordDTO();
            onlinePaymentRecord.PaymentOrderId = paymentOrderId;
            onlinePaymentRecord.OrderNo = orderNo;
            onlinePaymentRecord.OrderAmount = orderAmount;
            onlinePaymentRecord.PaymentAmount = paymentAmount;
            onlinePaymentRecord.OrderDatetime = orderDateTime;
            onlinePaymentRecord.PayDatetime = payDateTime;
            onlinePaymentRecord.PayResult = payResult;
            onlinePaymentRecord.ErrorCode = errorCode;
            onlinePaymentRecord.BankNumber = bankNumber;
            onlinePaymentRecord.ConfigId = configId;

            onlinePaymentRecord.CurrencyType = "CNY";
            onlinePaymentRecord.PaymentMethod = paymentMethod;
            onlinePaymentRecord.MemberId = memberId;
            onlinePaymentRecord.NextSearchTime = DateTime.UtcNow;
            onlinePaymentRecord.PaymentType = payType;
            return onlinePaymentRecord;
        }

        public void SetOnlinePaymentRecordSearchCount(OnlinePaymentRecordDTO onlinePaymentRecord)
        {
            var searchCount = onlinePaymentRecord.SearchCount;
            var nextSearchTime = onlinePaymentRecord.NextSearchTime;
            switch (searchCount)
            {
                case 0:
                    nextSearchTime = DateTime.UtcNow.AddMinutes(5);
                    break;
                case 1:
                    nextSearchTime = DateTime.UtcNow.AddMinutes(10);
                    break;
                case 2:
                    nextSearchTime = DateTime.UtcNow.AddMinutes(30);
                    break;
                case 3:
                    nextSearchTime = DateTime.UtcNow.AddHours(1);
                    break;
                case 4:
                    nextSearchTime = DateTime.UtcNow.AddHours(2);
                    break;
                case 5:
                    nextSearchTime = DateTime.UtcNow.AddHours(6);
                    break;
                default:
                    nextSearchTime = DateTime.UtcNow.AddDays(1);
                    break;
            }
            onlinePaymentRecord.NextSearchTime = nextSearchTime;
            onlinePaymentRecord.SearchCount = onlinePaymentRecord.SearchCount + 1;
        }
    }
}
