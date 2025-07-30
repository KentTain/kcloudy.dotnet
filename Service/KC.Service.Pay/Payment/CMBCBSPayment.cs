using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO.Config;
using KC.Service.DTO.Pay;
using KC.Framework.Extension;
using KC.Framework.SecurityHelper;
using KC.Framework.Tenant;
using KC.Framework.Util;
using KC.Service.Pay.Config;
using KC.Service.Pay.Constants;
using KC.Service.Pay.Response;

namespace KC.Service.Pay
{
    public class CMBCBSPayment : IPayment<CMBCBSConfig,CMBCBSResponse>
    {
        public ConfigEntityDTO GetConfigEntity()
        {
            throw new NotImplementedException();
        }

        private Tenant Tenant { get; set; }
        private IPaymentService PaymentService { get; set; }
        public CMBCBSPayment(Tenant tenant, IPaymentService paymentService)
        {
            Tenant = tenant;
            PaymentService = paymentService;
        }

        public PaymentReturnModel SplicePaymentConfig(ConfigEntityDTO config,string userId, bool isAdminPortal, int orderAmount, string billNo,
            string orderTime, string goodsName = null, string remark = "")
        {
            throw new NotImplementedException();
        }

        public Tuple<PaymentReturnModel, CMBCBSConfig> InitpayConfig(List<ConfigAttributeDTO> attributes)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;
            var cbsConfig = new CMBCBSConfig();
            var config = attributes.Find(m => m.Name.Equals("depositAccounts", StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "招行CBS缺少配置信息：depositAccounts,收款方银行账号";
                LogUtil.LogError(message);
                return new Tuple<PaymentReturnModel, CMBCBSConfig>(model, cbsConfig);
            }
            cbsConfig.depositAccounts = config.Value;
            config = attributes.Find(m => m.Name.Equals("purpose", StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "招行CBS缺少配置信息：purpose,用途";
                LogUtil.LogError(message);
                return new Tuple<PaymentReturnModel, CMBCBSConfig>(model, cbsConfig);
            }
            cbsConfig.purpose = config.Value;
            config = attributes.Find(m => m.Name.Equals("PayUrl", StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "招行CBS缺少配置信息：PayUrl,支付地址";
                LogUtil.LogError(message);
                return new Tuple<PaymentReturnModel, CMBCBSConfig>(model, cbsConfig);
            }
            cbsConfig.PayUrl = config.Value;
            config = attributes.Find(m => m.Name.Equals("PaymentMethod", StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "招行CBS缺少配置信息：PaymentMethod,支付方法";
                LogUtil.LogError(message);
                return new Tuple<PaymentReturnModel, CMBCBSConfig>(model, cbsConfig);
            }
            cbsConfig.PaymentMethod = config.Value;
            config = attributes.Find(m => m.Name.Equals("back_notify_url", StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "招行CBS缺少配置信息：back_notify_url,后台通知URL";
                LogUtil.LogError(message);
                return new Tuple<PaymentReturnModel, CMBCBSConfig>(model, cbsConfig);
            }
            cbsConfig.back_notify_url = config.Value;
            config = attributes.Find(m => m.Name.Equals("Key", StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "招行CBS缺少配置信息：key,key";
                LogUtil.LogError(message);
                return new Tuple<PaymentReturnModel, CMBCBSConfig>(model, cbsConfig);
            }
            cbsConfig.Key = config.Value;

            config = attributes.Find(m => m.Name.Equals("QueryBankAccountInfoByAccountNameMethod", StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "招行CBS缺少配置信息：QueryBankAccountInfoByAccountNameMethod,通过企业名查询银行账号信息";
                LogUtil.LogError(message);
                return new Tuple<PaymentReturnModel, CMBCBSConfig>(model, cbsConfig);
            }
            cbsConfig.QueryBankAccountInfoByAccountNameMethod = config.Value;
            model.Success = true;
            return new Tuple<PaymentReturnModel, CMBCBSConfig>(model, cbsConfig);
        }

        public PaymentReturnModel PaymentFrontUrl(string userId, CMBCBSResponse response, ConfigEntityDTO config)
        {
            throw new NotImplementedException();
        }

        public PaymentReturnModel PaymentBackUrl(string userId, CMBCBSResponse response, ConfigEntityDTO config)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            var message = string.Empty;
            if (string.IsNullOrWhiteSpace(response.erpPaymentId))
            {
                model.ErrorMessage = "erpPaymentId不完整";
                return model;
            }

            var onlinePaymentRecord = PaymentService.GetByPaymentOrderId(response.erpPaymentId);
            if (onlinePaymentRecord == null)
            {
                message = response.erpPaymentId + "支付订单未找到";
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }

            if (config==null||!config.ConfigAttributes.Any())
            {
                message = response.erpPaymentId + "未找到招行CBS配置";
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }

            var configAttributes= InitpayConfig(config.ConfigAttributes);
            if (configAttributes == null)
            {
                message = response.erpPaymentId + "招行CBS配置异常";
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }
            if (!configAttributes.Item1.Success)
            {
                return configAttributes.Item1;
            }

            var initConfig = configAttributes.Item2;
            var sign =
                MD5Provider.Hash(response.erpPaymentId + "|" + response.state + "|" + response.cbs_comment + "|" +
                           initConfig.Key);

            if (!response.sign.Equals(sign, StringComparison.OrdinalIgnoreCase))
            {
                message = response.erpPaymentId + "招行CBS校验异常，可能为非法来源！response.sign=" + response.sign + ";sign=" + sign;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }

            var localNow = DateTime.UtcNow.ToLocalDateTimeStr("yyyyMMddHHmmss");
            onlinePaymentRecord.PayResult = response.state == CBSState.Success ? "1" : "0";//非1都是失败
            onlinePaymentRecord.VerifyResult = response.state == CBSState.Success ? "1" : "2";//1校验成功，2校验失败
            onlinePaymentRecord.PayDatetime = localNow;
            onlinePaymentRecord.ReturnDatetime = localNow;
            onlinePaymentRecord.ErrorCode = response.cbs_comment;
            onlinePaymentRecord.PaymentAmount = onlinePaymentRecord.OrderAmount;

            var updateResult = PaymentService.UpdatePaymentRecord(onlinePaymentRecord);
            if (!updateResult)
            {
                message = string.Format("修改订单{0}支付状态失败", response.erpPaymentId);
                model.ErrorMessage = message;
                LogUtil.LogError(message);
                return model;
            }
            model.Success = true;
            model.HtmlStr = "OK";
            return model;
        }
    }
}
