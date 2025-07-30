using KC.Service.DTO;
using KC.Framework.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Pay
{
    [Serializable, DataContract(IsReference = true)]
    public class CertificateDTO : EntityDTO
    {
        public CertificateDTO()
        {
        }

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public decimal ConsumptionMoney { get; set; }
        [DataMember]
        public string ConsumptionMoneyStr { get; set; }
        /// <summary>
        /// 凭证号
        /// </summary>
        [DataMember]
        public string CertificateId { get; set; }

        [DataMember]
        public string VoucherStatusStr { get; set; }

        [DataMember]
        public string TransferStatusStr { get; set; }

        [DataMember]
        public string OrderTypeStr { get; set; }

        [DataMember]
        public string RepayStatusStr { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        [DataMember]
        public WorkflowBusStatus WorkFlowStatus { get; set; }
        [DataMember]
        public string WorkFlowStatusStr { get; set; }
        /// <summary>
        /// 同步状态
        /// </summary>
        [DataMember]
        public SyncStatus SyncStatus { get; set; }
        [DataMember]
        public string SyncStatusStr { get; set; }
        /// <summary>
        /// 客户id
        /// </summary>
        [DataMember]
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户TenantName
        /// </summary>
        [DataMember]
        public string CustomerUserName { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        [DataMember]
        public string CustomerDisplayName { get; set; }
        /// <summary>
        /// 需还总金额
        /// </summary>
        [DataMember]
        public decimal RepayTotalAmount { get; set; }
        [DataMember]
        public string RepayTotalAmountStr { get; set; }
        [DataMember]
        public string RepayTotalAmountToUpper { get; set; }
        /// <summary>
        /// 已还金额
        /// </summary>
        [DataMember]
        public decimal RepayAmount { get; set; }
        [DataMember]
        public string RepayAmountStr { get; set; }
        [DataMember]
        public string RepayAmountToUpper { get; set; }
        /// <summary>
        /// 有效日期
        /// </summary>
        [DataMember]
        public DateTime RepayDate { get; set; }
        [DataMember]
        public string RepayDateStr { get; set; }
        /// <summary>
        /// 期限
        /// </summary>
        [DataMember]
        public int TheTerm { get; set; }
        /// <summary>
        /// 提醒日期
        /// </summary>
        [DataMember]
        public DateTime RemindDate { get; set; }
        public string RemindDateStr { get; set; }
        /// <summary>
        /// 开证时间
        /// </summary>
        [DataMember]
        public string EffectiveTime { get; set; }
        /// <summary>
        /// 是否已经提醒
        /// </summary>
        [DataMember]
        public bool IsRemind { get; set; }
        [DataMember]
        public string IsRemindStr { get; set; }

        [DataMember]
        public string ReferenceId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }

        /// <summary>
        /// shop使用，应付编号
        /// </summary>
        [DataMember]
        public string PaymentInfo { get; set; }
        /// <summary>
        /// 是否可以转让
        /// </summary>
        [DataMember]
        public bool IsTransfer { get; set; }

        [DataMember]
        public string IsTransferStr { get; set; }

        /// <summary>
        /// 操作人Id
        /// </summary>
        public string OperatorId { get; set; }
        /// <summary>
        /// 操作人名称
        /// </summary>
        public string OperatorDisplayName { get; set; }

        /// <summary>
        /// 对应区块链编号 用去查询存储在区块链上的凭证信息
        /// </summary>
        [DataMember]
        public string BlockchainNumber { get; set; }

        #region 转让信息
        /// <summary>
        /// 转让单号
        /// </summary>
        [DataMember]
        public string TransferId { get; set; }
        [DataMember]
        public string TransferCertificateId { get; set; }
        [DataMember]
        public DateTime? TransferTime { get; set; }
        /// <summary>
        /// 转让租户编号
        /// </summary>
        [DataMember]
        public string Transfer { get; set; }
        /// <summary>
        /// 转让显示名称
        /// </summary>
        [DataMember]
        public string TransferName { get; set; }

        /// <summary>
        /// 转让前债务人Tenant
        /// </summary>
        [DataMember]
        public string TransferDebtorTenant { get; set; }

        /// <summary>
        /// 转让前债务人
        /// </summary>
        [DataMember]
        public string TransferDebtor { get; set; }
        #endregion



        /// <summary>
        /// 债务人
        /// </summary>
        [DataMember]
        public string Debtor { get; set; }
        [DataMember]
        public string DebtorTenant { get; set; }
        /// <summary>
        /// 债务人开户行
        /// </summary>
        [DataMember]
        public string DebtorBank { get; set; }
        /// <summary>
        /// 债务人银行卡号
        /// </summary>
        [DataMember]
        public string DebtorBankNumber { get; set; }
        /// <summary>
        /// 债务人统一社会信用代码
        /// </summary>
        [DataMember]
        public string DebtorSocialCreditCode { get; set; }
        /// <summary>
        /// 债务人签名
        /// </summary>
        [DataMember]
        [Obsolete("未使用 重复字段 转用 BuyerCertificate")]
        public string DebtorSignature { get; set; }
        /// <summary>
        /// 债务人印章
        /// </summary>
        public string DebtorSignaturePicture { get; set; }
        /// <summary>
        /// 债权人
        /// </summary>
        [DataMember]
        public string Creditor { get; set; }
        /// <summary>
        /// 债权人租户编号
        /// </summary>
        [DataMember]
        public string CreditorTenant { get; set; }
        /// <summary>
        /// 债权人签名
        /// </summary>
        [DataMember]
        [Obsolete("重复字段 请用：SellerCertificate")]
        public string CreditorSignature { get; set; }
        /// <summary>
        /// 债务人统一社会信用代码
        /// </summary>
        [DataMember]
        public string CreditorSocialCreditCode { get; set; }
        /// <summary>
        /// 债权人银行卡号
        /// </summary>
        [DataMember]
        public string CreditorBankNumber { get; set; }
        /// <summary>
        /// 债权人开户行
        /// </summary>
        [DataMember]
        public string CreditorBank { get; set; }
        /// <summary>
        /// 债务人印章
        /// </summary>
        public string CreditorSignaturePicture { get; set; }

        /// <summary>
        /// 买方签名
        /// </summary>
        [DataMember]
        public string BuyerCertificate { get; set; }
        [DataMember]
        public DateTime? BuyerCertificateTime { get; set; }

        /// <summary>
        /// 卖方签名
        /// </summary>
        [DataMember]
        public string SellerCertificate { get; set; }
        [DataMember]
        public DateTime? SellerCertificateTime { get; set; }

        /// <summary>
        /// 平台签名
        /// </summary>
        [DataMember]
        public string PlatformCertificate { get; set; }
        [DataMember]
        public DateTime? PlatformCertificateTime { get; set; }

        /// <summary>
        /// 签名原始数据
        /// </summary>
        [DataMember]
        public string CertificateSignature { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        [DataMember]
        public string ContractNo { get; set; }

        /// <summary>
        /// 采购单编号
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }



        [DataMember]
        public string GeneratePDF { get; set; }


        [DataMember]
        public string PayOrderNo { get; set; }
        [DataMember]
        public string SubjectCN { get; set; }
        [DataMember]
        public string SerialNumber { get; set; }

        /// <summary>
        /// 领用时间
        /// </summary>
        [DataMember]
        public string UseTime { get; set; }

        /// <summary>
        /// 领用金额
        /// </summary>
        public decimal UseAmount { get; set; }
        [DataMember]
        public string UseAmountStr { get; set; }

        /// <summary>
        /// 领用编号
        /// </summary>
        [DataMember]
        public string UseId { get; set; }

        /// <summary>
        /// 这是标识是转让的凭证
        /// </summary>
        [DataMember]
        public bool AlreadyTransferred { get; set; }


        /// <summary>
        /// 担保方额度领用编号
        /// </summary>
        [DataMember]
        public string VouchOrderId { get; set; }

        /// <summary>
        /// 担保方tenantname
        /// </summary>
        [DataMember]
        public string VouchTenantName { get; set; }
        /// <summary>
        /// 担保方公司显示名称
        /// </summary>
        [DataMember]
        public string VouchTenantDisplayName { get; set; }

        [DataMember]
        public string CreditUsageDetailId { get; set; }
        
    }
}
