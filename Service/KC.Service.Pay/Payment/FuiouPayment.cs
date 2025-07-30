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
using KC.Service.Pay.SDK;

namespace KC.Service.Pay
{
    public class FuiouPayment : IPayment<FuioupayConfig, FuioupayResponse>
    {
        public IPaymentService PaymentService { get; set; }
        public ConfigEntityDTO GetConfigEntity()
        {
            throw new NotImplementedException();
        }

        private Tenant Tenant { get; set; }
        public FuiouPayment(Tenant tenant, IPaymentService paymentService)
        {
            Tenant = tenant;
            PaymentService = paymentService;
        }

        public PaymentReturnModel SplicePaymentConfig(ConfigEntityDTO config, string userId, bool isAdminPortal, int orderAmount, string billNo,
            string orderTime, string goodsName = null, string remark = "")
        {
            goodsName = string.Empty;//产品名称
            var goodsUrl = string.Empty;//商品展示网址
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
            var fuiouPayConfig = initResult.Item2;
            var paramDic = new Dictionary<string, string>();
            paramDic[FuioupayConfigConstans.mchnt_cd] = fuiouPayConfig.mchnt_cd;
            paramDic[FuioupayConfigConstans.mchnt_key] = fuiouPayConfig.mchnt_key;
            paramDic[FuioupayConfigConstans.page_notify_url] = fuiouPayConfig.page_notify_url;
            paramDic[FuioupayConfigConstans.back_notify_url] = fuiouPayConfig.back_notify_url;
            paramDic[FuioupayConfigConstans.order_valid_time] = fuiouPayConfig.order_valid_time;
            paramDic[FuioupayConfigConstans.iss_ins_cd] = fuiouPayConfig.iss_ins_cd;
            paramDic[FuioupayConfigConstans.ver] = fuiouPayConfig.ver;
            paramDic[FuioupayConfigConstans.order_pay_type] = fuiouPayConfig.order_pay_type;

            paramDic[FuioupayConfigConstans.order_id] = billNo;
            paramDic[FuioupayConfigConstans.order_amt] = orderAmount.ToString();
            paramDic[FuioupayConfigConstans.rem] = remark;
            paramDic[FuioupayConfigConstans.goods_name] = goodsName;
            paramDic[FuioupayConfigConstans.goods_display_url] = goodsUrl;
            //mchnt_cd+"|" +order_id+"|"+order_amt+"|"+order_pay_type+"|"+page_notify_url+"|"+back_notify_url+"|"+order_valid_time+"|"+iss_ins_cd+"|"+goods_name+"|"+"+goods_display_url+"|"+rem+"|"+ver+"|"+mchnt_key
            paramDic[FuioupayConfigConstans.md5] = MD5Provider.Hash(fuiouPayConfig.mchnt_cd + "|" + billNo + "|" + orderAmount + "|" + fuiouPayConfig.order_pay_type + "|" + fuiouPayConfig.page_notify_url + "|" + fuiouPayConfig.back_notify_url + "|" + fuiouPayConfig.order_valid_time + "|" + fuiouPayConfig.iss_ins_cd + "|" + goodsName + "|" + goodsUrl + "|" + remark + "|" + fuiouPayConfig.ver + "|" + fuiouPayConfig.mchnt_key);

            // 将SDKUtil产生的Html文档写入页面，从而引导用户浏览器重定向
            string html = SDKUtil.CreateAutoSubmitForm(fuiouPayConfig.PostUrl + "/" + FuioupayMethod.PayMethod, paramDic, Encoding.UTF8);
            model.Success = true;
            model.HtmlStr = html;

            return model;
        }

        public Tuple<PaymentReturnModel, FuioupayConfig> InitpayConfig(List<ConfigAttributeDTO> attributes)
        {
            var fuioupayConfig = new FuioupayConfig();
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;
            var config = attributes.Find(m => m.Name.Equals(FuioupayConfigConstans.mchnt_cd, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Fuioupay缺少配置信息：" + FuioupayConfigConstans.mchnt_cd;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, FuioupayConfig>(model, fuioupayConfig);
            }
            fuioupayConfig.mchnt_cd = config.Value;

            config = attributes.Find(m => m.Name.Equals(FuioupayConfigConstans.PostUrl, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Fuioupay缺少配置信息：" + FuioupayConfigConstans.PostUrl;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, FuioupayConfig>(model, fuioupayConfig);
            }
            fuioupayConfig.PostUrl = config.Value;

            config = attributes.Find(m => m.Name.Equals(FuioupayConfigConstans.mchnt_key, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Fuioupay缺少配置信息：" + FuioupayConfigConstans.mchnt_key;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, FuioupayConfig>(model, fuioupayConfig);
            }
            fuioupayConfig.mchnt_key = config.Value;

            config = attributes.Find(m => m.Name.Equals(FuioupayConfigConstans.page_notify_url, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Fuioupay缺少配置信息：" + FuioupayConfigConstans.page_notify_url;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, FuioupayConfig>(model, fuioupayConfig);
            }
            fuioupayConfig.page_notify_url = config.Value;

            config = attributes.Find(m => m.Name.Equals(FuioupayConfigConstans.back_notify_url, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Fuioupay缺少配置信息：" + FuioupayConfigConstans.back_notify_url;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, FuioupayConfig>(model, fuioupayConfig);
            }
            fuioupayConfig.back_notify_url = config.Value;

            config = attributes.Find(m => m.Name.Equals(FuioupayConfigConstans.order_valid_time, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Fuioupay缺少配置信息：" + FuioupayConfigConstans.order_valid_time;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, FuioupayConfig>(model, fuioupayConfig);
            }
            fuioupayConfig.order_valid_time = config.Value;

            config = attributes.Find(m => m.Name.Equals(FuioupayConfigConstans.iss_ins_cd, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Fuioupay缺少配置信息：" + FuioupayConfigConstans.iss_ins_cd;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, FuioupayConfig>(model, fuioupayConfig);
            }
            fuioupayConfig.iss_ins_cd = config.Value;

            config = attributes.Find(m => m.Name.Equals(FuioupayConfigConstans.ver, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Fuioupay缺少配置信息：" + FuioupayConfigConstans.ver;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, FuioupayConfig>(model, fuioupayConfig);
            }
            fuioupayConfig.ver = config.Value;

            config = attributes.Find(m => m.Name.Equals(FuioupayConfigConstans.order_pay_type, StringComparison.CurrentCultureIgnoreCase));
            if (config == null)
            {
                message = "Fuioupay缺少配置信息：" + FuioupayConfigConstans.order_pay_type;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return new Tuple<PaymentReturnModel, FuioupayConfig>(model, fuioupayConfig);
            }
            fuioupayConfig.order_pay_type = config.Value;
            model.Success = true;
            return new Tuple<PaymentReturnModel, FuioupayConfig>(model, fuioupayConfig);
        }

        public PaymentReturnModel PaymentFrontUrl(string userId, FuioupayResponse response, ConfigEntityDTO config)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;
            if (!response.order_pay_code.Equals("0000"))
            {
                message = "支付失败,Code:" + response.order_pay_code + ",msg:" + response.order_pay_error + ";order_id:" +
                          response.order_id;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }

            if (config == null)
            {
                message = "支付失败，富友支付信息不完整。order_id:" + response.order_id;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }

            var attrs = config.ConfigAttributes;
            if (!attrs.Any())
            {
                message = "支付失败，富友支付信息不完整。order_id:" + response.order_id;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }

            var mchntKeyConfig =
                    attrs.ToList()
                        .FirstOrDefault(
                            m =>
                                m.Name.Equals(FuioupayConfigConstans.mchnt_key,
                                    StringComparison.CurrentCultureIgnoreCase));
            if (mchntKeyConfig == null)
            {
                message = "支付失败，未找到mchnt_key。order_id:" + response.order_id;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }
            var mchntKey = mchntKeyConfig.Value;
            var encryptStr = response.mchnt_cd + "|" + response.order_id + "|" + response.order_date +
                             "|" + response.order_amt + "|" + response.order_st + "|" +
                             response.order_pay_code + "|" + response.order_pay_error + "|" +
                             response.resv1 + "|" + response.fy_ssn + "|" + mchntKey;
            var md5 = MD5Provider.Hash(encryptStr);
            if (!md5.Equals(response.md5, StringComparison.OrdinalIgnoreCase))
            {
                message = "支付失败，md5校验失败。";
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }


            model.Success = true;
            model.HtmlStr = "支付成功，请到订单详情界面继续操作。";
            return model;
        }

        public PaymentReturnModel PaymentBackUrl(string userId, FuioupayResponse response, ConfigEntityDTO config)
        {
            PaymentReturnModel model = new PaymentReturnModel();
            model.Success = false;
            string message = string.Empty;

            if (!response.order_pay_code.Equals("0000"))
            {
                message = "支付失败,Code:" + response.order_pay_code + ",msg:" + response.order_pay_error + ";order_id:" +
                          response.order_id;
                model.ErrorMessage = message;
                LogUtil.LogError(message);
                return model;
            }
            var payOrderId = response.order_id;
            var payAmount = decimal.Parse(response.order_amt);

            if (config == null)
            {
                message = "支付失败，富友支付信息不完整。order_id:" + response.order_id;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }

            var attrs = config.ConfigAttributes;
            if (!attrs.Any())
            {
                message = "支支付失败，富友支付信息不完整。order_id:" + response.order_id;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }

            var mchntKeyConfig =
                    attrs.ToList()
                        .FirstOrDefault(
                            m =>
                                m.Name.Equals(FuioupayConfigConstans.mchnt_key,
                                    StringComparison.CurrentCultureIgnoreCase));
            if (mchntKeyConfig == null)
            {
                message = "支付失败，未找到mchnt_key。order_id:" + response.order_id;
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }
            var mchntKey = mchntKeyConfig.Value;

            var encryptStr = response.mchnt_cd + "|" + response.order_id + "|" + response.order_date +
                             "|" + response.order_amt + "|" + response.order_st + "|" +
                             response.order_pay_code + "|" + response.order_pay_error + "|" +
                             response.resv1 + "|" + response.fy_ssn + "|" + mchntKey;
            var md5 = MD5Provider.Hash(encryptStr);
            if (!md5.Equals(response.md5, StringComparison.OrdinalIgnoreCase))
            {
                message = "支付失败，md5校验失败。";
                LogUtil.LogError(message);
                model.ErrorMessage = message;
                return model;
            }

            var onlinePaymentRecord = PaymentService.GetByPaymentOrderIdAndOrderAmount(payOrderId, payAmount);
            if (onlinePaymentRecord == null)
            {
                message = payOrderId + "支付订单未找到";
                model.ErrorMessage = message;
                LogUtil.LogError(message);
                return model;
            }
            var localNow = DateTime.UtcNow.ToLocalDateTimeStr("yyyyMMddHHmmss");
            var paymentResult = FuiouOrderState.AlreadyPaid == response.order_st;
            onlinePaymentRecord.ErrorCode = response.order_pay_code + "," + response.order_pay_error;
            onlinePaymentRecord.PayResult = FuiouOrderState.AlreadyPaid == response.order_st ? "1" : response.order_st;//非1都是失败
            onlinePaymentRecord.VerifyResult = paymentResult ? "1" : "2";//1校验成功，2校验失败
            onlinePaymentRecord.BankName = "富友流水号";
            onlinePaymentRecord.BankNumber = response.fy_ssn;
            onlinePaymentRecord.ReturnDatetime = localNow;
            if (paymentResult)
            {
                onlinePaymentRecord.PayDatetime = localNow;
                onlinePaymentRecord.PaymentAmount = payAmount;
            }

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

    }
}
