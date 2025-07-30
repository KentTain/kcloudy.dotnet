using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Service.Pay.Config;
using KC.Service.Pay.Response;
using KC.Service.Pay.Constants;
using KC.Service.Pay.SDK;
using KC.Service.DTO.Pay;
using KC.Service.DTO.Config;

namespace KC.Service.Pay
{
    public class AllinPayment : IPayment<AllinpayConfig, AllinPayResponse>
    {
        public IPaymentService PaymentService { get; set; }
        public ConfigEntityDTO GetConfigEntity()
        {
            throw new NotImplementedException();
        }

        private Tenant Tenant { get; set; }
        public AllinPayment(Tenant tenant, IPaymentService paymentService)
        {
            Tenant = tenant;
            PaymentService = paymentService;
        }

        public PaymentReturnModel SplicePaymentConfig(ConfigEntityDTO config, string userId, bool isAdminPortal, int orderAmount, string billNo,
            string orderTime, string goodsName = null, string remark = "")
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
            var alllinPayConfig = initResult.Item2;
            var paramDic = new Dictionary<string, string>();
            paramDic[AllinpayConfigConstans.InputCharset] = alllinPayConfig.InputCharset;
            paramDic[AllinpayConfigConstans.PickupUrl] = alllinPayConfig.PickupUrl;
            paramDic[AllinpayConfigConstans.ReceiveUrl] = alllinPayConfig.ReceiveUrl;
            paramDic[AllinpayConfigConstans.Version] = alllinPayConfig.Version;
            paramDic[AllinpayConfigConstans.Language] = alllinPayConfig.Language;
            paramDic[AllinpayConfigConstans.SignType] = alllinPayConfig.SignType;
            paramDic[AllinpayConfigConstans.MerchantId] = alllinPayConfig.MerchantId;
            paramDic[AllinpayConfigConstans.OrderNo] = billNo;
            paramDic[AllinpayConfigConstans.OrderAmount] = orderAmount.ToString();
            paramDic[AllinpayConfigConstans.OrderCurrency] = alllinPayConfig.OrderCurrency;
            paramDic[AllinpayConfigConstans.OrderDatetime] = orderTime;
            paramDic[AllinpayConfigConstans.PayType] = alllinPayConfig.PayType;
            paramDic[AllinpayConfigConstans.SignMsg] = MD5Provider.Hash(SDKUtil.PrintDictionaryToString(paramDic) + "&key=" + alllinPayConfig.MD5Key, false);
            // 将SDKUtil产生的Html文档写入页面，从而引导用户浏览器重定向
            string html = SDKUtil.CreateAutoSubmitForm(alllinPayConfig.PostUrl, paramDic, Encoding.UTF8);
            model.Success = true;
            model.HtmlStr = html;
            return model;
        }

        #region 载Alliinpay配置文件
        /// <summary>
        /// 加载Alliinpay配置文件
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public Tuple<PaymentReturnModel, AllinpayConfig> InitpayConfig(List<ConfigAttributeDTO> attributes)
        {
            var allinpayConfig = new AllinpayConfig();
            PaymentReturnModel model = new PaymentReturnModel();
            string message = string.Empty;
            model.Success = false;
            var config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.InputCharset, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.InputCharset;
                LogUtil.LogError(message); 
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.InputCharset = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.Language, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.Language;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.Language = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.MerchantId, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.MerchantId;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.MerchantId = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.OrderCurrency, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.OrderCurrency;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.OrderCurrency = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.PayType, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.PayType;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.PayType = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.PickupUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.PickupUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.PickupUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.PostUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.PostUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.PostUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.ReceiveUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.ReceiveUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.ReceiveUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.SignType, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.SignType;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.SignType = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.Version, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.Version;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.Version = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.Key, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.Key;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.MD5Key = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.QueryVersion, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.QueryVersion;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.QueryVersion = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.PayMerchantId, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.PayMerchantId;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.PayMerchantId = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.UserName, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.UserName;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.UserName = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.Password, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.Password;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.Password = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.TrxCode, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.TrxCode;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.TrxCode = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.BusinessCode, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.BusinessCode;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.BusinessCode = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.Level, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.Level;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.Level = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.SigneCert, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.SigneCert;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.SigneCert = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.SigneCertPwd, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.SigneCertPwd;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.SigneCertPwd = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.VerifyCert, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.VerifyCert;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.VerifyCert = config.Value;

            config = attributes.Find(m => m.Name.Equals(AllinpayConfigConstans.ProcessServlet, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Allinpay缺少配置信息：" + AllinpayConfigConstans.ProcessServlet;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
            }
            allinpayConfig.ProcessServlet = config.Value;
            model.Success = true;
            return new Tuple<PaymentReturnModel, AllinpayConfig>(model, allinpayConfig);
        }

        public PaymentReturnModel PaymentFrontUrl(string userId, AllinPayResponse response, ConfigEntityDTO config)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;
            if (!response.PayResult.Equals("1"))
            {
                message = "支付失败," + response.ErrorCode;
                model.ErrorMessage = message;
                LogUtil.LogError(message);
                return model;
            }
            var payOrderId = response.OrderNo;

            if (config == null)
            {
                message = "支付失败，未找到通联支付的配置信息。order_id:" + payOrderId;
                model.ErrorMessage = message;
                LogUtil.LogError(message);
                return model;
            }

            var attrs = config.ConfigAttributes;
            if (!attrs.Any())
            {
                message = "支付失败，未找到通联支付的配置信息。order_id:" + payOrderId;
                model.ErrorMessage = message;
                LogUtil.LogError(message);
                return model;
            }

            var md5KeyConfig = attrs.ToList().FirstOrDefault(m => m.Name.Equals(AllinpayConfigConstans.MD5Key,
                                    StringComparison.CurrentCultureIgnoreCase));
            if (md5KeyConfig == null)
            {
                message = "支付失败，未找到md5Key。order_id:" + payOrderId;
                model.ErrorMessage = message;
                LogUtil.LogError(message);
                return model;
            }
            var md5Key = md5KeyConfig.Value;
            var encryptStr = string.Format("{0}={1}&{2}={3}&{4}={5}&{6}={7}&{8}={9}&{10}={11}&{12}={13}&{14}={15}&{16}={17}&{18}={19}&{20}={21}&{22}={23}&{24}={25}&{26}={27}"
                , AllinpayConfigConstans.MerchantId, response.MerchantId
                , AllinpayConfigConstans.Version, response.Version
                , AllinpayConfigConstans.Language, response.Language
                , AllinpayConfigConstans.SignType, response.SignType
                , AllinpayConfigConstans.PayType, response.PayType
                , AllinpayConfigConstans.PaymentOrderId, response.PaymentOrderId
                , AllinpayConfigConstans.OrderNo, response.OrderNo
                , AllinpayConfigConstans.OrderDatetime, response.OrderDatetime
                , AllinpayConfigConstans.OrderAmount, response.OrderAmount
                , AllinpayConfigConstans.PayDatetime, response.PayDatetime
                , AllinpayConfigConstans.PayAmount, response.PayAmount
                , AllinpayConfigConstans.PayResult, response.PayResult
                , AllinpayConfigConstans.ReturnDatetime, response.ReturnDatetime
                , AllinpayConfigConstans.MD5Key, md5Key
                );
            var md5 = MD5Provider.Hash(encryptStr);
            if (!md5.Equals(response.SignMsg, StringComparison.OrdinalIgnoreCase))
            {
                message = "支付失败，md5校验失败。";
                LogUtil.LogError(message);
                return model;
            }
            model.HtmlStr= "支付成功，请到原界面继续操作。";
            model.Success = true;
            return model;
        }

        public PaymentReturnModel PaymentBackUrl(string userId, AllinPayResponse response, ConfigEntityDTO config)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;
            if (!response.PayResult.Equals("1"))
            {
                message = "支付失败,Code:" + response.ErrorCode;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }
            var payOrderId = response.OrderNo;
            var payAmount = decimal.Parse(response.PayAmount);

            if (config == null)
            {
                message = "支付失败，未找到通联支付的配置信息。order_id:" + payOrderId;
                model.ErrorMessage = message;
                LogUtil.LogError(message);
                return model;
            }

            var attrs = config.ConfigAttributes;
            if (!attrs.Any())
            {
                message = "支付失败，未找到通联支付的配置信息。order_id:" + payOrderId;
                model.ErrorMessage = message;
                LogUtil.LogError(message);
                return model;
            }


            var md5KeyConfig = attrs.ToList().FirstOrDefault(m => m.Name.Equals(AllinpayConfigConstans.MD5Key,
                                    StringComparison.CurrentCultureIgnoreCase));
            if (md5KeyConfig == null)
            {
                message = "支付失败，未找到md5Key。order_id:" + payOrderId;
                model.ErrorMessage = message;
                LogUtil.LogError(message);
                return model;
            }
            var md5Key = md5KeyConfig.Value;

            var encryptStr = string.Format("{0}={1}&{2}={3}&{4}={5}&{6}={7}&{8}={9}&{10}={11}&{12}={13}&{14}={15}&{16}={17}&{18}={19}&{20}={21}&{22}={23}&{24}={25}&{26}={27}"
                , AllinpayConfigConstans.MerchantId, response.MerchantId
                , AllinpayConfigConstans.Version, response.Version
                , AllinpayConfigConstans.Language, response.Language
                , AllinpayConfigConstans.SignType, response.SignType
                , AllinpayConfigConstans.PayType, response.PayType
                , AllinpayConfigConstans.PaymentOrderId, response.PaymentOrderId
                , AllinpayConfigConstans.OrderNo, response.OrderNo
                , AllinpayConfigConstans.OrderDatetime, response.OrderDatetime
                , AllinpayConfigConstans.OrderAmount, response.OrderAmount
                , AllinpayConfigConstans.PayDatetime, response.PayDatetime
                , AllinpayConfigConstans.PayAmount, response.PayAmount
                , AllinpayConfigConstans.PayResult, response.PayResult
                , AllinpayConfigConstans.ReturnDatetime, response.ReturnDatetime
                , AllinpayConfigConstans.MD5Key, md5Key
                );
            var md5 = MD5Provider.Hash(encryptStr);
            if (!md5.Equals(response.SignMsg, StringComparison.OrdinalIgnoreCase))
            {
                message = "支付失败，md5校验失败。";
                model.ErrorMessage = message;
                LogUtil.LogError(message);
                return model;
            }

            //var paymentService = new OnlinePaymentRecordService();
            var onlinePaymentRecord = PaymentService.GetByPaymentOrderIdAndOrderAmount(payOrderId, payAmount);
            if (onlinePaymentRecord == null)
            {
                message = payOrderId + "支付订单未找到";
                LogUtil.LogError(message);
                return model;
            }
            var paymentResult = true;
            onlinePaymentRecord.ErrorCode = response.ErrorCode;
            onlinePaymentRecord.PayResult = "1";
            onlinePaymentRecord.VerifyResult = paymentResult ? "1" : "2";//1校验成功，2校验失败
            onlinePaymentRecord.BankName = "通联支付流水号";
            onlinePaymentRecord.BankNumber = response.PaymentOrderId;
            onlinePaymentRecord.ReturnDatetime = response.ReturnDatetime;
            onlinePaymentRecord.PayDatetime = response.PayDatetime;
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

        #endregion
    }
}
