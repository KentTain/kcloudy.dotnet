using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO.Config;
using KC.Service.DTO.Pay;

namespace KC.Service.Pay
{
    public interface IPayment<T, in TResponse>
    {
        ConfigEntityDTO GetConfigEntity();

        /// <summary>
        /// 拼接支付信息为html
        /// </summary>
        /// <param name="config"></param>
        /// <param name="isAdminPortal"></param>
        /// <param name="orderAmount"></param>
        /// <param name="billNo"></param>
        /// <param name="orderTime"></param>
        /// <param name="goodsName"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        PaymentReturnModel SplicePaymentConfig(ConfigEntityDTO config, string userId, bool isAdminPortal, int orderAmount, string billNo, string orderTime,
            string goodsName = null, string remark = "");

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        Tuple<PaymentReturnModel, T> InitpayConfig(List<ConfigAttributeDTO> attributes);

        /// <summary>
        /// 前台通知用户
        /// </summary>
        PaymentReturnModel PaymentFrontUrl(string userId,TResponse response, ConfigEntityDTO config);

        /// <summary>
        /// 支付系统后台调用，保存支付结果到数据库
        /// </summary>
        PaymentReturnModel PaymentBackUrl(string userId, TResponse response, ConfigEntityDTO config);

    }
}
