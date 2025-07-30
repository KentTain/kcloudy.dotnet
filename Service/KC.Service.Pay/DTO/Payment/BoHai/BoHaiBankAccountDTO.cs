using KC.Enums.Pay;
using KC.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay.BoHai
{
    public class BoHaiBankAccountDTO : PayBaseParamDTO
    {
        [DataMember]
        public int Id { get; set; }


        /// <summary>
        /// 银行编号
        /// </summary>
        [DataMember]
        public string BankId { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        [DataMember]
        public string BankName { get; set; }

        /// <summary>
        /// 银行账号(卡号)
        /// </summary>
        [DataMember]
        public string AccountNum { get; set; }

        /// <summary>
        /// 开户名称
        /// </summary>
        [DataMember]

        public string AccountName { get; set; }

        /// <summary>
        /// 账户类型(1: 对公; 2: 对私)
        /// </summary>
        [DataMember]
        public int AccountType { get; set; }

        /// <summary>
        /// 银行账户(卡)类型
        /// A:企业一般结算账户，默认
        /// B:企业电子账户
        /// </summary>
        [DataMember]
        public string BankAccountType { get; set; }

        /// <summary>
        /// 开户证件类型
        /// </summary>
        [DataMember]
        public string CardType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [DataMember]

        public string CardNumber { get; set; }

        /// <summary>
        /// 跨行标示(1:本行;2:跨行)默认 2 跨行
        /// </summary>
        [DataMember]
        public string CrossMark { get; set; }

        /// <summary>
        /// 开户网点编号
        /// </summary>
        [DataMember]
        public string OpenBankCode { get; set; }

        /// <summary>
        /// 开户网点名称
        /// </summary>
        [DataMember]

        public string OpenBankName { get; set; }

        /// <summary>
        /// 开户网点省份编号
        /// </summary>
        [DataMember]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 开户网点省份名称
        /// </summary>
        [DataMember]
        public string ProvinceName { get; set; }

        /// <summary>
        /// 开户网点城市编号
        /// </summary>
        [DataMember]
        public string CityCode { get; set; }

        /// <summary>
        /// 开户网点城市名称
        /// </summary>
        [DataMember]
        public string CityName { get; set; }

        /// <summary>
        /// 银行账户状态
        /// </summary>
        [DataMember]
        public BankAccountState BankState { get; set; }

        [DataMember]
        public string BankAccountStateStr
        {
            get
            {
                return BankState.ToDescription();
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

    }
}
