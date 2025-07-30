using KC.Framework.Base;
using KC.Model.Pay.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Pay
{
    [Table(Tables.PaymentAttachment)]
    public class PaymentAttachment : Entity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        public string BusinessNumber{ get; set; }

        [MaxLength(128)]
        public string BlobId { get; set; }

        [MaxLength(128)]
        public string FileName { get; set; }

        [MaxLength(1024)]
        public string Url { get; set; }
    }
}
