using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.Pay;
using KC.Framework.Base;
using KC.Model.Pay.Constants;

namespace KC.Model.Pay
{
    /// <summary>
    /// 应付账款
    /// </summary>
    [Table(Tables.Payable)]
    public class Payable : Entity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        public string PayableNumber { get; set; }

        public PayableType Type { get; set; }

        public PayableSource Source { get; set; }

        [MaxLength(128)]
        public string OrderId { get; set; }

        public decimal OrderAmount { get; set; }

        public decimal PayableAmount { get; set; }

        public decimal AlreadyPayAmount { get; set; }

        public DateTime StartDate { get; set; }

        [MaxLength(128)]
        public string Customer { get; set; }

        [MaxLength(128)]
        public string CustomerTenant { get; set; }

        [MaxLength(1024)]
        public string Remark { get; set; }

        public int UsePoint { get; set; }

        public decimal PointEqualMoney { get; set; }

    }

}
