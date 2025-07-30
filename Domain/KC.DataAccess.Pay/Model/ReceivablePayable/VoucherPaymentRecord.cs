using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Model.Pay.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Pay
{
    [Serializable]
    [DataContract(IsReference = true)]
    [Table(Tables.VoucherPaymentRecord)]
    public class VoucherPaymentRecord : Entity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        public string OrderId { get; set; }

        [MaxLength(128)]
        public string PayableNumber { get; set; }

        public decimal Amounts { get; set; }

        public DateTime PaymentDate { get; set; }

        public VoucherSubmitStatus Status { get; set; }

        /// <summary>
        /// 债务人
        /// </summary>
        [MaxLength(128)]
        public string Debtor { get; set; }

        [MaxLength(128)]
        public string DebtorBankNumber { get; set; }

        [MaxLength(128)]
        public string DebtorBank { get; set; }

        [MaxLength(128)]
        public string DebtorSocialCreditCode { get; set; }

        /// <summary>
        /// 债权人
        /// </summary>
        [MaxLength(128)]
        public string Creditor { get; set; }

        [MaxLength(128)]
        public string CreditorBank { get; set; }

        [MaxLength(128)]
        public string CreditorBankNumber { get; set; }

        [MaxLength(128)]
        public string CreditorSocialCreditCode { get; set; }

        [MaxLength(128)]
        public string VoucherId { get; set; }

        [MaxLength(128)]
        public string CreditUsageId { get; set; }

        [MaxLength(128)]
        public string FinancialInstitutionTenantName { get; set; }

        [MaxLength(128)]
        public string FinancialInstitutionName { get; set; }

    }
}
