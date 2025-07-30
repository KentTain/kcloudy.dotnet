using KC.Enums.Contract;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
 

namespace KC.Service.DTO.Contract
{
    [Serializable]
    [DataContract(IsReference = true)]
    public  class ContractTemplateOrderDTO
    {
        /// <summary>
        /// 采购方
        /// </summary>
        [DataMember]
        public string CustomerName { get; set; }

        /// <summary>
        /// 供货方
        /// </summary>
        [DataMember]
        public string VendorName { get; set; }
        /// <summary>
        /// 承租人
        /// </summary>
        [DataMember]
        public string Lessee { get; set; }

        /// <summary>
        /// 甲方法定代表人
        /// </summary>
        [DataMember]
        public string JiaLegal { get; set; }

        /// <summary>
        /// 乙方法定代表人
        /// </summary>
        [DataMember]
        public string YiLegal { get; set; }
        /// <summary>
        /// 乙方法定代表人身份证
        /// </summary>
        [DataMember]
        public string YiLegalNO { get; set; }

        /// <summary>
        /// 采购方地址
        /// </summary>
        [DataMember]
        public string CustomerAddress1 { get; set; }

        /// <summary>
        /// 采购方联系人
        /// </summary>
        [DataMember]
        public string CustomerContactPerson { get; set; }

        /// <summary>
        /// 采购方联系方式
        /// </summary>
        [DataMember]
        public string CustomerContactPhone { get; set; }

        /// <summary>
        /// 供货方地址
        /// </summary>
        [DataMember]
        public string VendorAddress1 { get; set; }

        /// <summary>
        /// 供货方联系人
        /// </summary>
        [DataMember]
        public string VendorContactPerson { get; set; }
        /// <summary>
        /// 供货方联系方式
        /// </summary>
        [DataMember]
        public string VendorContactPhone { get; set; }
        /// <summary>
        /// 供货方电子邮箱
        /// </summary>
        [DataMember]
        public string VendorContactEmail { get; set; }

        /// <summary>
        /// 收货地点
        /// </summary>
        [DataMember]
        public string ShipToAddress1 { get; set; }
        /// <summary>
        /// 收货联系人
        /// </summary>
        [DataMember]
        public string ShipToContactPerson { get; set; }

        /// <summary>
        /// 收货联系电话
        /// </summary>
        [DataMember]
        public string ShipToContactPhone { get; set; }
        /// <summary>
        /// 收货电子邮箱
        /// </summary>
        [DataMember]
        public string ShipToContactEmail { get; set; }


        /// <summary>
        /// 交易方式
        /// </summary>
        [DataMember]
        public string TransactionType { get; set; }


        /// <summary>
        /// 结算方式
        /// </summary>
        [DataMember]
        public string SOTypeName { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        [DataMember]
        public string DocNetAmount { get; set; }

        /// <summary>
        /// 订单金额大写
        /// </summary>
        [DataMember]
        public string DocNetAmountStr { get; set; }
        /// <summary>
        /// 预付款
        /// </summary>
        [DataMember]
        public string AdvanceCharge { get; set; }

        /// <summary>
        /// 预付款大写
        /// </summary>
        [DataMember]
        public string AdvanceChargeStr { get; set; }
        /// <summary>
        /// 保证金
        /// </summary>
        [DataMember]
        public string CautionMoney { get; set; }

        /// <summary>
        /// 保证金大写
        /// </summary>
        [DataMember]
        public string CautionMoneyStr { get; set; }

        /// <summary>
        /// 产品:货物名称、型号、数量、价格
        /// </summary>
        [DataMember]
        public List<Goods> Goodslist { get; set; }

        /// <summary>
        ///还款计划表
        /// </summary>
        [DataMember]
        public List<RepaymentPlan> RepaymentPlanlist { get; set; }
 
        /// <summary>
        /// 分期期数
        /// </summary>
        [DataMember]
        public string StagingNum { get; set; }

        /// <summary>
        /// 还款计划总额
        /// </summary>
        [DataMember]
        public string RepayPlanAmount { get; set; }

        /// <summary>
        /// 还款计划总额大写
        /// </summary>
        [DataMember]
        public string RepayPlanAmountStr { get; set; }
    }
    public class RepaymentPlan
    {
        /// <summary>
        /// 支付时间
        /// </summary>
        [DataMember]
        public string PayTime { get; set; }


        /// <summary>
        /// 每期应付
        /// </summary>
        [DataMember]
        public string EachPeriod { get; set; }
 
        /// <summary>
        /// 期数
        /// </summary>
        [DataMember]
        public string TheTerm { get; set; }
    }
    public class Goods
    {
        /// <summary>
        /// 货物名称
        /// </summary>
        [DataMember]
        public string PartDescription { get; set; }


        /// <summary>
        /// 单价（含税）
        /// </summary>
        [DataMember]
        public string UnitPriceStr { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public string SalesQuantityStr { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        [DataMember]
        public string OfferingModel { get; set; }
        /// <summary>
        /// 备品
        /// </summary>
        [DataMember]
        public string SparePartsStr { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [DataMember]
        public string Unit { get; set; }
        /// <summary>
        /// 金额（含税）
        /// </summary>
        [DataMember]
        public string AmountStr { get; set; }
    }
}
