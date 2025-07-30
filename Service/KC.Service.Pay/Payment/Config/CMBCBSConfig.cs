using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Config
{
    public class CMBCBSConfig
    {
        public string CompanyName { get; set; }

        public string BankNumber { get; set; }

        public string BankAddress { get; set; }

        public string BankType { get; set; }

        public string OpenBankName { get; set; }




        /// <summary>
        /// 支付方法
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// 通过企业名查询银行账号信息
        /// </summary>
        public string QueryBankAccountInfoByAccountNameMethod { get; set; }

        /// <summary>
        /// 支付地址
        /// </summary>
        public string PayUrl { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        public string purpose { get; set; }

        /// <summary>
        /// 收款方银行账号
        /// </summary>
        public string depositAccounts { get; set; }


        public string Key { get; set; }

        /// <summary>
        /// 回调地址
        /// </summary>
        public string back_notify_url { get; set; }


    }
}
