using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.SDK
{
    public class HeliPay
    {
        public static Dictionary<string, string> ResponseCodes
        {
            get
            {
                var codes = new Dictionary<string, string>();
                codes.Add("0000", "成功");
                codes.Add("8001", "订单号不唯一");
                codes.Add("8101", "订单金额不正确");
                codes.Add("8102", "订单不存在");
                codes.Add("8103", "订单状态异常");
                codes.Add("8104", "订单对应的渠道未在系统中配置");
                codes.Add("8105", "退款金额超过了订单实付金额");
                codes.Add("8106", "渠道请求交互验签错误");
                codes.Add("8107", "订单已过期");
                codes.Add("8108", "订单已存在,请更换订单号重新下单");
                codes.Add("8109", "商户未开通此银行");
                return codes;
            }
        }

        /// <summary>
        /// 将Dictionary内容排序后输出为键值对字符串,供打印报文使用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string PrintDictionaryToString(Dictionary<string, string> data)
        {
            var builder = new StringBuilder();
            foreach (KeyValuePair<string, string> element in data)
            {
                builder.Append("&" + element.Value);
            }

            return builder.ToString();
        }

    }
}
