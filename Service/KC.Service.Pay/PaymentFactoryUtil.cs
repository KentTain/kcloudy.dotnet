using KC.Service.DTO.Config;
using KC.Service.DTO.Pay;
using KC.Enums.Pay;
using KC.Framework.Tenant;
using KC.Service.Pay.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace KC.Service.Pay
{
    public class PaymentFactoryUtil
    {
        public Tenant Tenant { get; set; }
        public IPaymentService PaymentService { get; set; }

        public PaymentFactoryUtil(Tenant tenant, IPaymentService paymentService)
        {
            Tenant = tenant;
            PaymentService = paymentService;
        }

        public PaymentReturnModel Payment(ConfigEntityDTO configEntity, string userId, bool isAdminPortal,
            int orderAmount, string billNo, string orderTime,
            string goodsName = null, string remark = "")
        {
            PaymentReturnModel model = new PaymentReturnModel();
            switch (configEntity.ConfigSign)
            {
                case (int)ThirdPartyType.AllinpayConfigSign:
                    model = new AllinPayment(Tenant, PaymentService).SplicePaymentConfig(configEntity, userId, isAdminPortal, orderAmount, billNo,
                        orderTime, goodsName, remark);
                    break;
                case (int)ThirdPartyType.CMBCBSConfigSign:
                    model = new CMBCBSPayment(Tenant, PaymentService).SplicePaymentConfig(configEntity, userId, isAdminPortal, orderAmount, billNo,
                        orderTime, goodsName, remark);
                    break;
                case (int)ThirdPartyType.FuiouConfigSign:
                    model = new FuiouPayment(Tenant, PaymentService).SplicePaymentConfig(configEntity, userId, isAdminPortal, orderAmount, billNo,
                        orderTime, goodsName, remark);
                    break;
                case (int)ThirdPartyType.HeliPayConfigSign:
                    model = new HeliPayment(Tenant, PaymentService).SplicePaymentConfig(configEntity, userId, isAdminPortal, orderAmount, billNo,
                        orderTime, goodsName, remark);
                    break;
                case (int)ThirdPartyType.UnionpayConfigSign:
                    model = new UnionPayment(Tenant, PaymentService).SplicePaymentConfig(configEntity, userId, isAdminPortal, orderAmount, billNo,
                        orderTime, goodsName, remark);
                    break;
                default:
                    model = new PaymentReturnModel { Success = false, ErrorMessage = "不支持的第三方支付平台。" };
                    break;
            }
            return model;
        }

        public PaymentReturnModel PaymentFrontUrl(string userId, ConfigEntityDTO configEntity, Object response)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            switch (configEntity.ConfigSign)
            {
                case (int)ThirdPartyType.AllinpayConfigSign:
                    model = new AllinPayment(Tenant, PaymentService).PaymentFrontUrl(userId, response as AllinPayResponse, configEntity);
                    break;
                case (int)ThirdPartyType.CMBCBSConfigSign:
                    model = new CMBCBSPayment(Tenant, PaymentService).PaymentFrontUrl(userId, response as CMBCBSResponse, configEntity);
                    break;
                case (int)ThirdPartyType.FuiouConfigSign:
                    model = new FuiouPayment(Tenant, PaymentService).PaymentFrontUrl(userId, response as FuioupayResponse, configEntity);
                    break;
                case (int)ThirdPartyType.HeliPayConfigSign:
                    model = new HeliPayment(Tenant, PaymentService).PaymentFrontUrl(userId, response as HeliPayResponse, configEntity);
                    break;
                case (int)ThirdPartyType.UnionpayConfigSign:
                    model = new UnionPayment(Tenant, PaymentService).PaymentFrontUrl(userId, response as UnionResponse, configEntity);
                    break;
                default:
                    model = new PaymentReturnModel { Success = false, ErrorMessage = "不支持的第三方支付平台。" };
                    break;
            }
            return model;
        }

        public PaymentReturnModel PaymentBackUrl(string userId, ConfigEntityDTO configEntity, Object response)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            switch (configEntity.ConfigSign)
            {
                case (int)ThirdPartyType.AllinpayConfigSign:
                    model = new AllinPayment(Tenant, PaymentService).PaymentBackUrl(userId, response as AllinPayResponse, configEntity);
                    break;
                case (int)ThirdPartyType.CMBCBSConfigSign:
                    model = new CMBCBSPayment(Tenant, PaymentService).PaymentBackUrl(userId, response as CMBCBSResponse, configEntity);
                    break;
                case (int)ThirdPartyType.FuiouConfigSign:
                    model = new FuiouPayment(Tenant, PaymentService).PaymentBackUrl(userId, response as FuioupayResponse, configEntity);
                    break;
                case (int)ThirdPartyType.HeliPayConfigSign:
                    model = new HeliPayment(Tenant, PaymentService).PaymentBackUrl(userId, response as HeliPayResponse, configEntity);
                    break;
                case (int)ThirdPartyType.UnionpayConfigSign:
                    model = new UnionPayment(Tenant, PaymentService).PaymentBackUrl(userId, response as UnionResponse, configEntity);
                    break;
                default:
                    model = new PaymentReturnModel { Success = false, ErrorMessage = "不支持的第三方支付平台。" };
                    break;
            }
            return model;
        }
    }
}
