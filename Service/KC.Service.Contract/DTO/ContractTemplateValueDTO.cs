using KC.Enums.Contract;
using KC.Service.DTO;
using System;
using System.Runtime.Serialization;
 

namespace KC.Service.DTO.Contract
{
    [Serializable]
    [DataContract(IsReference = true)]
    public  class ContractTemplateValueDTO
    {
        /// <summary>
        /// id
        /// </summary>
        [DataMember]
        public int Id { get; set; }
 
        /// <summary>
        /// 交货时间
        /// </summary>
        [DataMember]
        public int Day { get; set; }
        /// <summary>
        /// 质量异议期
        /// </summary>
        [DataMember]
        public int BreakDay { get; set; }

        /// <summary>
        /// 结算期限内
        /// </summary>
        [DataMember]
        public int AccountDayIn { get; set; }

        /// <summary>
        /// 收货后期限内
        /// </summary>
        [DataMember]
        public int DeliveryDayIn { get; set; }
 
        /// <summary>
        /// 其他约定事项
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// 开户银行
        /// </summary>
        [DataMember]
        public string OpenBank { get; set; }
        /// <summary>
        /// 帐    号
        /// </summary>
        [DataMember]
        public string BankAccount { get; set; }
        /// <summary>
        /// 户    名
        /// </summary>
        [DataMember]
        public string BankAccountName { get; set; }
        /// <summary>
        /// 交货方式（作为7类型的留购价大写）
        /// </summary>
        [DataMember]
        public string DeliveryMode { get; set; }
 
        /// <summary>
        /// 处理期限
        /// </summary>
        [DataMember]
        public int HandleDay { get; set; }


        /// <summary>
        /// 变更合同提前日
        /// </summary>
        [DataMember]
        public int ChangeContractDay { get; set; }

        /// <summary>
        /// 违约金百分比(7类型作为留购价)
        /// </summary>
        [DataMember]
        public double PenaltyMoney { get; set; }

        /// <summary>
        /// 整改日期
        /// </summary>
        [DataMember]
        public int RectificDay { get; set; }

        /// <summary>
        /// 调查处理日期
        /// </summary>
        [DataMember]
        public int InvestiHandleDay { get; set; }

        /// <summary>
        /// 延迟一天的违约金百分比
        /// </summary>
        [DataMember]
        public double PenaltyDayMoney { get; set; }

        /// <summary>
        /// 延迟改期日期
        /// </summary>
        [DataMember]
        public int Rescheduling { get; set; }


        /// <summary>
        /// 解决争议方式
        /// </summary>
        [DataMember]
        public string DisputeType { get; set; }
        /// <summary>
        /// 租期
        /// </summary>
        [DataMember]
        public int LeaseTerm { get; set; }
 
        /// <summary>
        /// 起租日投保
        /// </summary>
        [DataMember]
        public int RentingDay { get; set; }

 
        /// <summary>
        /// 保密期限
        /// </summary>
        [DataMember]
        public int SecrecyPeriod { get; set; }

        /// <summary>
        /// 合同标题，暂不使用
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 合同内容，客户自己编辑的固定内容
        /// </summary>
        public string Content { get; set; }
    }
}
