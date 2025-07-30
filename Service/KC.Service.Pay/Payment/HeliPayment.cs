using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Common;
using KC.Service.DTO.Config;
using KC.Service.DTO.Pay;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Service.Pay.Config;
using KC.Service.Pay.Constants;
using KC.Service.Pay.Response;
using KC.Service.Pay.SDK;

namespace KC.Service.Pay
{
    public class HeliPayment : IPayment<HeliPayConfig,HeliPayResponse>
    {
        public IPaymentService PaymentService { get; set; }
        public ConfigEntityDTO GetConfigEntity()
        {
            throw new NotImplementedException();
        }

        private Tenant Tenant { get; set; }
        public HeliPayment(Tenant tenant, IPaymentService paymentService)
        {
            Tenant = tenant;
            PaymentService = paymentService;
        }

        public PaymentReturnModel SplicePaymentConfig(ConfigEntityDTO config, string userId, bool isAdminPortal, int orderAmount, string billNo,
            string orderTime, string goodsName = null,string remark = "")
        {
            PaymentReturnModel model = new PaymentReturnModel();
            var configAttributes = config.ConfigAttributes;
            if (configAttributes == null)
            {
                model.Success = false;
                model.ErrorMessage = "支付配置信息错误1";
            }
            var initResult = InitpayConfig(configAttributes);
            model = initResult.Item1;
            if (!model.Success)
            {
                return model;
            }
            var heliPayConfig = initResult.Item2;
            //交易所需参数
            var paramDic = new Dictionary<string, string>();
            paramDic[HeliPayConfigConstans.P1_bizType] = heliPayConfig.P1_bizType;
            paramDic[HeliPayConfigConstans.P2_customerNumber] = heliPayConfig.P2_customerNumber;
            paramDic[HeliPayConfigConstans.P3_userId] = heliPayConfig.P3_userId;
            paramDic[HeliPayConfigConstans.P4_orderId] = billNo;
            paramDic[HeliPayConfigConstans.P5_timestamp] = orderTime;
            paramDic[HeliPayConfigConstans.P6_goodsName] = goodsName;
            paramDic[HeliPayConfigConstans.P7_goodsDesc] = string.Empty;
            paramDic[HeliPayConfigConstans.P8_orderAmount] = (orderAmount / 100.00).ToString();

            paramDic[HeliPayConfigConstans.P9_period] = heliPayConfig.P9_period;
            paramDic[HeliPayConfigConstans.P10_periodUnit] = heliPayConfig.P10_periodUnit;
            paramDic[HeliPayConfigConstans.P11_callbackUrl] = heliPayConfig.P11_callbackUrl;
            paramDic[HeliPayConfigConstans.P12_serverCallbackUrl] = heliPayConfig.P12_serverCallbackUrl;
            //paramDic[HeliPayConfigConstans.P13_orderIp] = IPHelper.GetServerIp;
            paramDic[HeliPayConfigConstans.P14_productCode] = heliPayConfig.P14_productCode;
            paramDic[HeliPayConfigConstans.P15_desc] = remark;
            paramDic[HeliPayConfigConstans.signType] = heliPayConfig.signType;
            paramDic[HeliPayConfigConstans.sign] = MD5Provider.Hash(HeliPay.PrintDictionaryToString(paramDic) + "&" + heliPayConfig.SignKey);
            //paramDic[HeliPayConstants.P6_goodsName] = HttpUtility.UrlEncode(goodsName);
            //paramDic[HeliPayConstants.P15_desc] = HttpUtility.UrlEncode(remark);
            var html = SDKUtil.CreateAutoSubmitForm(heliPayConfig.PostUrl, paramDic, Encoding.UTF8);

            model.Success = true;
            model.HtmlStr = html;

            return model;
        }

        public Tuple<PaymentReturnModel, HeliPayConfig> InitpayConfig(List<ConfigAttributeDTO> attributes)
        {
            var heliPay = new HeliPayConfig();
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;
            var config = attributes.Find(m => m.Name.Equals(HeliPayConfigConstans.PostUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "HeliPay缺少配置信息：" + HeliPayConfigConstans.PostUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);
            }
            heliPay.PostUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(HeliPayConfigConstans.P1_bizType, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "HeliPay缺少配置信息：" + HeliPayConfigConstans.P1_bizType;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);
            }
            heliPay.P1_bizType = config.Value;

            config = attributes.Find(m => m.Name.Equals(HeliPayConfigConstans.P2_customerNumber, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "HeliPay缺少配置信息：" + HeliPayConfigConstans.P2_customerNumber;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);
            }
            heliPay.P2_customerNumber = config.Value;

            config = attributes.Find(m => m.Name.Equals(HeliPayConfigConstans.P3_userId, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "HeliPay缺少配置信息：" + HeliPayConfigConstans.P3_userId;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);
            }
            heliPay.P3_userId = config.Value;

            config = attributes.Find(m => m.Name.Equals(HeliPayConfigConstans.P9_period, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "HeliPay缺少配置信息：" + HeliPayConfigConstans.P9_period;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);
            }
            heliPay.P9_period = config.Value;

            config = attributes.Find(m => m.Name.Equals(HeliPayConfigConstans.P10_periodUnit, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "HeliPay缺少配置信息：" + HeliPayConfigConstans.P10_periodUnit;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);
            }
            heliPay.P10_periodUnit = config.Value;

            config = attributes.Find(m => m.Name.Equals(HeliPayConfigConstans.P11_callbackUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "HeliPay缺少配置信息：" + HeliPayConfigConstans.P11_callbackUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);
            }
            heliPay.P11_callbackUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(HeliPayConfigConstans.P12_serverCallbackUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "HeliPay缺少配置信息：" + HeliPayConfigConstans.P12_serverCallbackUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);
            }
            heliPay.P12_serverCallbackUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(HeliPayConfigConstans.P14_productCode, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "HeliPay缺少配置信息：" + HeliPayConfigConstans.P14_productCode;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);
            }
            heliPay.P14_productCode = config.Value;

            config = attributes.Find(m => m.Name.Equals(HeliPayConfigConstans.signType, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "HeliPay缺少配置信息：" + HeliPayConfigConstans.signType;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);
            }
            heliPay.signType = config.Value;

            config = attributes.Find(m => m.Name.Equals(HeliPayConfigConstans.SignKey, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "HeliPay缺少配置信息：" + HeliPayConfigConstans.SignKey;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);
            }
            heliPay.SignKey = config.Value;

            model.Success = true;

            return new Tuple<PaymentReturnModel, HeliPayConfig>(model, heliPay);

        }

        public PaymentReturnModel PaymentFrontUrl(string userId, HeliPayResponse response, ConfigEntityDTO config)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;
            try
            {
                LogUtil.LogInfo("HeliPayPaymentFrontUrl=" + SerializeHelper.ToJson(response));
                if (!response.rt2_retCode.Equals("0000"))
                {
                    message = "支付失败,Code:" + response.rt2_retCode + ",失败信息:" +
                              HeliPay.ResponseCodes.First(m => m.Key.Equals(response.rt2_retCode)).Value;
                    LogUtil.LogError(message);
                    model.Success = false;
                    return model;

                }
                var payOrderId = response.rt6_orderId;

                if (config == null)
                {
                    message = "支付失败，未找到平台提供的支付方式。order_id:" + payOrderId;
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    return model;
                }

                var attrs = config.ConfigAttributes;
                if (!attrs.Any())
                {
                    message = "支付失败，未找到合利宝支付的配置信息。order_id:" + payOrderId;
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    return model;
                }

                var md5KeyConfig = attrs.ToList().FirstOrDefault(m => m.Name.Equals(HeliPayConfigConstans.SignKey,
                                        StringComparison.CurrentCultureIgnoreCase));
                if (md5KeyConfig == null)
                {
                    message = "支付失败，未找到合利宝支付的配置信息。order_id:" + payOrderId;
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    return model;
                }
                var md5Key = md5KeyConfig.Value;
                var encryptStr = string.Format("&{0}&{1}&{2}&{3}&{4}&{5}&{6}&{7}&{8}&{9}&{10}&{11}&{12}&{13}"
                    , response.rt1_bizType, response.rt2_retCode
                    , response.rt3_retMsg, response.rt4_customerNumber
                    , response.rt5_userId, response.rt6_orderId
                    , response.rt7_orderAmount, response.rt8_orderStatus
                    , response.rt9_serialNumber, response.rt10_completeDate
                    , response.rt11_productCode, response.rt12_desc
                    , response.signType, md5Key);
                var md5 = MD5Provider.Hash(encryptStr);
                if (!md5.Equals(response.sign, StringComparison.OrdinalIgnoreCase))
                {
                    message = "支付失败，md5校验失败。";
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    return model;
                }
                model.Success = true;
                model.HtmlStr = "支付成功，请到原界面继续操作。";
                return model;
            }
            catch (Exception e)
            {
                LogUtil.LogError("前台通知客户接口出现异常");
                LogUtil.LogException(e);
                message = "支付失败。" + e.Message;
                model.ErrorMessage = message;
                return model;
            }

        }

        public PaymentReturnModel PaymentBackUrl(string userId, HeliPayResponse response, ConfigEntityDTO config)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;
            try
            {
                //LogUtil.LogInfo("HeliPayPaymentFrontUrl=" + SerializeHelper.ToJson(response));
                if (!response.rt2_retCode.Equals("0000"))
                {
                    message = "支付失败,Code:" + response.rt2_retCode + ",失败信息:" +
                              HeliPay.ResponseCodes.First(m => m.Key.Equals(response.rt2_retCode)).Value;
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    model.Success = false;
                    return model;

                }
                var payOrderId = response.rt6_orderId;
                var payAmount = decimal.Parse(response.rt7_orderAmount) * 100;

                if (config == null)
                {
                    message = "支付失败，未找到平台提供的支付方式。order_id:" + payOrderId;
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    return model;
                }

                var attrs = config.ConfigAttributes;
                if (!attrs.Any())
                {
                    message = "支付失败，未找到合利宝支付的配置信息。order_id:" + payOrderId;
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    return model;
                }

                var md5KeyConfig = attrs.ToList().FirstOrDefault(m => m.Name.Equals(HeliPayConfigConstans.SignKey,
                                        StringComparison.CurrentCultureIgnoreCase));
                if (md5KeyConfig == null)
                {
                    message = "支付失败，未找到合利宝支付的配置信息。order_id:" + payOrderId;
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    return model;
                }
                var md5Key = md5KeyConfig.Value;
                var encryptStr = string.Format("&{0}&{1}&{2}&{3}&{4}&{5}&{6}&{7}&{8}&{9}&{10}&{11}&{12}&{13}"
                    , response.rt1_bizType, response.rt2_retCode
                    , response.rt3_retMsg, response.rt4_customerNumber
                    , response.rt5_userId, response.rt6_orderId
                    , response.rt7_orderAmount, response.rt8_orderStatus
                    , response.rt9_serialNumber, response.rt10_completeDate
                    , response.rt11_productCode, response.rt12_desc
                    , response.signType, md5Key);
                var md5 = MD5Provider.Hash(encryptStr);
                if (!md5.Equals(response.sign, StringComparison.OrdinalIgnoreCase))
                {
                    message = "支付失败，md5校验失败。";
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    return model;
                }

                var onlinePaymentRecord = PaymentService.GetByPaymentOrderIdAndOrderAmount(payOrderId, payAmount);
                if (onlinePaymentRecord == null)
                {
                    message = payOrderId + "支付订单未找到";
                    LogUtil.LogError(message);
                    return model;
                }
                //if (!string.IsNullOrWhiteSpace(onlinePaymentRecord.ReturnDatetime))//重复通知
                //    return Content("success");
                var paymentResult = true;
                onlinePaymentRecord.ErrorCode = response.rt2_retCode;
                onlinePaymentRecord.PayResult = "1";
                onlinePaymentRecord.VerifyResult = paymentResult ? "1" : "2";//1校验成功，2校验失败
                onlinePaymentRecord.BankName = "合利宝支付流水号";
                onlinePaymentRecord.BankNumber = response.rt9_serialNumber;
                var localNow = DateTime.UtcNow.AddHours(8);
                DateTime.TryParse(response.rt10_completeDate, out localNow);
                var localNowStr = localNow.ToString("yyyyMMddHHmmss");
                onlinePaymentRecord.ReturnDatetime = localNowStr;
                onlinePaymentRecord.PayDatetime = localNowStr;
                onlinePaymentRecord.PaymentAmount = payAmount;

                var updateResult = PaymentService.UpdatePaymentRecord(onlinePaymentRecord);
                if (!updateResult)
                {
                    message = string.Format("修改订单{0}支付状态失败", payOrderId);
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    return model;
                }
                model.Success = true;
                model.HtmlStr = "OK";
                return model;
            }
            catch (Exception e)
            {

                LogUtil.LogError("前台通知客户接口出现异常");
                LogUtil.LogException(e);
                message = "支付失败。" + e.Message;
                model.ErrorMessage = message;
                return model;
            }
        }
    }
}
