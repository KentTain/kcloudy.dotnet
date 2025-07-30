using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Offering.Constants;
using KC.Framework.Base;

namespace KC.Model.Offering
{
    /// <summary>
    /// 手机归属地
    /// </summary>
    [Table(Tables.OfferingOperationLog)]
    public class OfferingOperationLog : ProcessLogBase
    {
        [MaxLength(16)]
        public string OfferingCode { get; set; }

        [MaxLength(256)]
        public string OfferingName { get; set; }

        public int OfferingId { get; set; }
        [ForeignKey("OfferingId")]
        public virtual Offering Offering { get; set; }
    }
}
