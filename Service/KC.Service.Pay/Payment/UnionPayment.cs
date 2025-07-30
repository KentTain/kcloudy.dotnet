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
    public class UnionPayment : IPayment<UnionPayConfig,UnionResponse>
    {
        public IPaymentService PaymentService { get; set; }
        public ConfigEntityDTO GetConfigEntity()
        {
            throw new NotImplementedException();
        }

        private Tenant Tenant { get; set; }
        public UnionPayment(Tenant tenant, IPaymentService paymentService)
        {
            Tenant = tenant;
            PaymentService = paymentService;
        }

        public PaymentReturnModel SplicePaymentConfig(ConfigEntityDTO config,string userId, bool isAdminPortal, int orderAmount, string billNo,
            string orderTime, string goodsName = null, string remark = "")
        {
            PaymentReturnModel model = new PaymentReturnModel();
            var configAttributes = config.ConfigAttributes;
            var state = config.State;
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


            var unionPayConfig = initResult.Item2;
            SetStaticConfig(unionPayConfig);
            var paramDic = new Dictionary<string, string>();
            //交易所需参数
            var parameters = new List<string>
            {
                UnionpayConfigConstans.param_version,
                UnionpayConfigConstans.param_encoding,
                UnionpayConfigConstans.param_txnType,
                UnionpayConfigConstans.param_txnSubType,
                UnionpayConfigConstans.param_bizType,
                UnionpayConfigConstans.param_frontUrl,
                UnionpayConfigConstans.param_backUrl,
                UnionpayConfigConstans.param_signMethod,
                UnionpayConfigConstans.param_channelType,
                UnionpayConfigConstans.param_accessType,
                UnionpayConfigConstans.param_merId,
                UnionpayConfigConstans.param_currencyCode
            };
            foreach (var param in parameters)
            {
                var attribute = configAttributes.Find(m => m.Name.Equals(param));
                if (attribute == null)
                {
                    var errorMessage = "Unionpay缺少配置信息：" + param;
                    LogUtil.LogError(errorMessage);
                    model.Success = false;
                    model.ErrorMessage = errorMessage;
                    return model;
                }
                paramDic[param] = attribute.Value;
            }

            paramDic[UnionpayConfigConstans.param_reqReserved] = billNo;
            paramDic[UnionpayConfigConstans.param_txnAmt] = orderAmount.ToString();
            paramDic[UnionpayConfigConstans.param_orderId] = billNo;
            paramDic[UnionpayConfigConstans.param_txnTime] = orderTime;

            var isSign = false;

            try
            {
                isSign = SDKUtil.Sign(Tenant,userId, paramDic, Encoding.UTF8, state);
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
                model.Success = false;
                model.ErrorMessage = ex.Message;
            }

            if (isSign)
            {
                // 将SDKUtil产生的Html文档写入页面，从而引导用户浏览器重定向
                string html = SDKUtil.CreateAutoSubmitForm(SDKConfig.FrontTransUrl, paramDic, Encoding.UTF8);
                model.Success = true;
                model.HtmlStr = html;
            }
            else
            {
                model.Success = false;
                model.ErrorMessage = "支付签名失败，系统异常，请联系商城客服。";
            }

            return model;
        }

        public Tuple<PaymentReturnModel, UnionPayConfig> InitpayConfig(List<ConfigAttributeDTO> attributes)
        {
            var unionPay = new UnionPayConfig();
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;

            var config = attributes.Find(m => m.Name.Equals(UnionpayConfigConstans.CardRequestUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Unionpay缺少配置信息：" + UnionpayConfigConstans.CardRequestUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, UnionPayConfig>(model, unionPay);
            }
            unionPay.CardRequestUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(UnionpayConfigConstans.AppRequestUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Unionpay缺少配置信息：" + UnionpayConfigConstans.AppRequestUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, UnionPayConfig>(model, unionPay);
            }
            unionPay.AppRequestUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(UnionpayConfigConstans.FrontTransUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Unionpay缺少配置信息：" + UnionpayConfigConstans.FrontTransUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, UnionPayConfig>(model, unionPay);
            }
            unionPay.FrontTransUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(UnionpayConfigConstans.EncryptCertName, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Unionpay缺少配置信息：" + UnionpayConfigConstans.EncryptCertName;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, UnionPayConfig>(model, unionPay);
            }
            unionPay.EncryptCert = config.Value;

            config = attributes.Find(m => m.Name.Equals(UnionpayConfigConstans.BackTransUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Unionpay缺少配置信息：" + UnionpayConfigConstans.BackTransUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, UnionPayConfig>(model, unionPay);
            }
            unionPay.BackTransUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(UnionpayConfigConstans.SingleQueryUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Unionpay缺少配置信息：" + UnionpayConfigConstans.SingleQueryUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, UnionPayConfig>(model, unionPay);
            }
            unionPay.SingleQueryUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(UnionpayConfigConstans.FileTransUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Unionpay缺少配置信息：" + UnionpayConfigConstans.FileTransUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, UnionPayConfig>(model, unionPay);
            }
            unionPay.FileTransUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(UnionpayConfigConstans.SignCertName, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Unionpay缺少配置信息：" + UnionpayConfigConstans.SignCertName;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, UnionPayConfig>(model, unionPay);
            }
            unionPay.SignCertPath = config.Value;
            unionPay.SignCertName = config.Value;

            config = attributes.Find(m => m.Name.Equals(UnionpayConfigConstans.SignCertPwd, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Unionpay缺少配置信息：" + UnionpayConfigConstans.SignCertPwd;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, UnionPayConfig>(model, unionPay);
            }
            unionPay.SignCertPwd = config.Value;

            config = attributes.Find(m => m.Name.Equals(UnionpayConfigConstans.BatTransUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Unionpay缺少配置信息：" + UnionpayConfigConstans.BatTransUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, UnionPayConfig>(model, unionPay);
            }
            unionPay.BatTransUrl = config.Value;

            unionPay.ValidateCertDir = string.Format("{0}/PayCert/Unionpay", AppDomain.CurrentDomain.BaseDirectory);

            model.Success = true;

            return new Tuple<PaymentReturnModel, UnionPayConfig>(model, unionPay);

        }

        private void SetStaticConfig(UnionPayConfig config)
        {
            SDKConfig.ValidateCertDir = string.Format("{0}/PayCert/Unionpay", AppDomain.CurrentDomain.BaseDirectory);
            SDKConfig.CardRequestUrl = config.CardRequestUrl;
            SDKConfig.AppRequestUrl = config.AppRequestUrl;
            SDKConfig.FrontTransUrl = config.FrontTransUrl;
            SDKConfig.EncryptCert = SDKConfig.ValidateCertDir + "/" + config.EncryptCert;
            SDKConfig.BackTransUrl = config.BackTransUrl;
            SDKConfig.SingleQueryUrl = config.SingleQueryUrl;
            SDKConfig.FileTransUrl = config.FileTransUrl;
            SDKConfig.SignCertName = config.SignCertName;
            SDKConfig.SignCertPath = SDKConfig.ValidateCertDir + "/" + config.SignCertPath;
            SDKConfig.SignCertPwd = config.SignCertPwd;
            SDKConfig.BatTransUrl = config.BatTransUrl;
        }

        public PaymentReturnModel PaymentFrontUrl(string userId, UnionResponse response, ConfigEntityDTO config)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;
            if (response.HttpMethod == "POST")
            {
                // 使用Dictionary保存参数
                var resData = new Dictionary<string, string>();

                var coll = response.Collection;

                string[] requestItem = coll.AllKeys;

                for (int i = 0; i < requestItem.Length; i++)
                {
                    resData.Add(requestItem[i], coll[requestItem[i]]);
                }

                string respcode = resData["respCode"];
                // 返回报文中不包含UPOG,表示Server端正确接收交易请求,则需要验证Server端返回报文的签名
                var encryptCertName = config.ConfigAttributes.FirstOrDefault(m => m.Name == SDKConstants.EncryptCertName).Value;
                var validate = SDKUtil.Validate(Tenant,userId,encryptCertName,resData, Encoding.UTF8,config.State);
                LogUtil.LogInfo("ValidateCertResult:" + validate);
                if (validate == ValidateCertResult.Success)
                {
                    if ("00".Equals(respcode))
                    {
                        message = "支付成功，请到订单详情界面继续操作。";
                        model.Success = true;
                        model.HtmlStr = message;
                    }
                    else
                    {
                        message = "支付失败";
                        model.ErrorMessage = message;
                    }
                }
                else
                {
                    message =  "商户端验证银联返回报文结果:验证签名失败";
                    model.ErrorMessage = message;
                }

            }
            return model;
        }

        public PaymentReturnModel PaymentBackUrl(string userId, UnionResponse response, ConfigEntityDTO config)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;

            if (response.HttpMethod == "POST")
            {
                // 使用Dictionary保存参数
                var resData = response.ResData;
                if (resData.Count <= 0)
                {
                    var coll = response.Collection;
                    string[] requestItem = coll.AllKeys;
                    for (int i = 0; i < requestItem.Length; i++)
                    {
                        resData.Add(requestItem[i], coll[requestItem[i]]);
                    }
                }

                string respcode = resData[SDKConstants.param_respCode];
                // 返回报文中不包含UPOG,表示Server端正确接收交易请求,则需要验证Server端返回报文的签名
                var encryptCertName = config.ConfigAttributes.FirstOrDefault(m => m.Name == SDKConstants.EncryptCertName).Value;
                var validate = SDKUtil.Validate(Tenant, userId, encryptCertName, resData, Encoding.UTF8,config.State);

                if (validate != ValidateCertResult.Success)
                {
                    LogUtil.LogInfo("ValidateCertResult:" + validate);
                    LogUtil.LogError(string.Format("商户端验证银联返回报文结果:验证签名失败,resData:{0}", SDKUtil.CoverDictionaryToString(resData)));
                }

                var payOrderId = resData[SDKConstants.param_orderId];
                var payAmount = decimal.Parse(resData[SDKConstants.param_txnAmt]);
                var bankNumber = resData[SDKConstants.param_queryId];
                //var paymentService = new OnlinePaymentRecordService();
                var onlinePaymentRecord = PaymentService.GetByPaymentOrderIdAndOrderAmount(payOrderId, payAmount);
                if (onlinePaymentRecord == null)
                {
                    message = payOrderId + "支付订单未找到";
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    return model;
                }

                var paymentResult = validate == ValidateCertResult.Success && "00".Equals(respcode);
                onlinePaymentRecord.ErrorCode = respcode + "," + resData[SDKConstants.param_respMsg];
                onlinePaymentRecord.PayResult = paymentResult ? "1" : respcode;//非1都是失败
                onlinePaymentRecord.VerifyResult = validate == ValidateCertResult.Success ? "1" : "2";//1校验成功，2校验失败
                if (paymentResult)
                {
                    onlinePaymentRecord.PayDatetime = DateTime.UtcNow.Year + resData[SDKConstants.param_traceTime];
                    onlinePaymentRecord.PaymentAmount = payAmount;
                    onlinePaymentRecord.BankName = "银联流水号";
                    onlinePaymentRecord.BankNumber = bankNumber;
                }

                var updateResult = PaymentService.UpdatePaymentRecord(onlinePaymentRecord);
                if (!updateResult)
                {
                    message = string.Format("修改订单{0}支付状态失败", payOrderId);
                    model.ErrorMessage = message;
                    LogUtil.LogError(message);
                    return model;
                }
            }
            model.Success = true;
            model.HtmlStr = "OK";
            return model;
        }
    }
}
