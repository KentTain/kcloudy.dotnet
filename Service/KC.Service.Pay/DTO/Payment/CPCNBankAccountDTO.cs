using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class CPCNBankAccountDTO : EntityDTO
    {

        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 中金的资金账号
        /// </summary>
        [DataMember]
        public string AccountNo { get; set; }

        /// <summary>
        /// 户名
        /// </summary>
        [DataMember]
        public string AccountName { get; set; }

        /// <summary>
        /// 银行电子账号（跨行收款账号）
        /// </summary>
        [DataMember]
        public string BankEId { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [DataMember]
        public string MemberId { get; set; }

        /// <summary>
        /// 资金账户状态 1开户 2销户 3已绑卡 4 解绑
        /// </summary>
        [DataMember]
        public int State { get; set; }

        /// <summary>
        /// 绑定银行账户的ID   BankAccount表的Id
        /// </summary>
        [DataMember]
        public int BindBankAccountId { get; set; }

        /// <summary>
        /// 绑定的银行卡号
        /// </summary>
        [DataMember]
        public string BindBankAccount { get; set; }

        /// <summary>
        /// 绑定的银行卡户名
        /// </summary>
        [DataMember]
        public string BindBankAccountName { get; set; }

        /// <summary>
        /// 绑定的银行编号
        /// </summary>
        [DataMember]
        public string BindBankId { get; set; }

        /// <summary>
        /// 绑定银行的名称
        /// </summary>
        [DataMember]
        public string BindBankName { get; set; }

        /// <summary>
        /// 电子账户归属支行号
        /// </summary>
        [DataMember]
        public string OpenBankCode { get; set; }

        /// <summary>
        /// 电子账户归属支行名称
        /// </summary>
        [DataMember]
        public string OpenBankName { get; set; }

        /// <summary>
        /// 第三方支付的类型
        /// </summary>
        [DataMember]
        public ThirdPartyType PaymentType { get; set; }

        /// <summary>
        /// 账号总额
        /// </summary>
        [DataMember]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 冻结金额
        /// </summary>
        [DataMember]
        public decimal FreezeAmount { get; set; }

        /// <summary>
        /// 平台计算的总金额
        /// </summary>
        [DataMember]
        public decimal CFWinTotalAmount { get; set; }

        /// <summary>
        /// 平台计算的冻结金额
        /// </summary>
        [DataMember]
        public decimal CFWinFreezeAmount { get; set; }

        /// <summary>
        /// 金额更新时间
        /// </summary>
        [DataMember]
        public DateTime AmountUpdateTime { get; set; }

        /// <summary>
        /// 可用余额，单位元
        /// </summary>
        public decimal SysAvailableBalance
        {
            get { return (CFWinTotalAmount - CFWinFreezeAmount) / 100; }
        }

        /// <summary>
        /// 冻结资金，单位元
        /// </summary>
        public decimal SysFrozenAmount
        {
            get { return CFWinFreezeAmount / 100; }
        }


        /// <summary>
        /// 是否平台的账号
        /// </summary>
        [DataMember]
        public bool IsPlatformAccount { get; set; }

        public string PaymentTypeName
        {
            get
            {
                return PaymentType.ToDescription();
            }
        }

    }
}
