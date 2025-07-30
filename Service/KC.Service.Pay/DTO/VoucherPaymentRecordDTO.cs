using KC.Enums.Pay;
using KC.Framework.Extension;
using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.DTO.Pay
{
    public class VoucherPaymentRecordDTO : EntityDTO
    {
        public int Id { get; set; }

        public string OrderId { get; set; }

        public decimal Amounts { get; set; }

        public DateTime PaymentDate { get; set; }

        public VoucherSubmitStatus Status { get; set; }

        /// <summary>
        /// 债务人
        /// </summary>
        public string Debtor { get; set; }

        public string DebtorTenantName { get; set; }

        public string DebtorBankNumber { get; set; }

        public string DebtorBank { get; set; }

        public string DebtorSignature { get; set; }

        public string DebtorSocialCreditCode { get; set; }

        /// <summary>
        /// 债权人
        /// </summary>
        public string Creditor { get; set; }

        public string CreditorTenantName { get; set; }

        public string CreditorBank { get; set; }

        public string CreditorBankNumber { get; set; }

        public string CreditorSocialCreditCode { get; set; }

        public string CreditorSignature { get; set; }

        public string PayableNumber { get; set; }

        /// <summary>
        /// 白条编号
        /// </summary>
        public string CertificateId { get; set; }

        /// <summary>
        /// 交易合同编号
        /// </summary>
        public string ContractNo { get; set; }

        public bool CanTransferable { get; set; }

        public string VoucherId { get; set; }

        public string StatusText
        {
            get
            {

                return Status.ToDescription();
            }
        }

        public string PaymentDateText
        {
            get
            {
                return PaymentDate.ToString("yyyy-MM-dd");
            }
        }

        public string CreditUsageId { get; set; }

        public string FinancialInstitutionTenantName { get; set; }

        public string FinancialInstitutionName { get; set; }

    }
}
