using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Constants
{
    public class CMBCBSConfigConstans
    {
        public const string PaymentMethod = "PaymentMethod";
        //支付地址
        public const string PayUrl = "PayUrl";
        //用途
        public const string purpose = "purpose";
        //收款方银行账号
        public const string depositAccounts = "depositAccounts";
        //Key
        public const string Key = "Key";
        //CBS通知地址
        public const string back_notify_url = "back_notify_url";
        //通过企业名查询银行账号信息
        public const string QueryBankAccountInfoByAccountNameMethod = "QueryBankAccountInfoByAccountNameMethod";
    }
}
