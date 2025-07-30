using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Service.DTO;

namespace KC.Service.DTO.Offering
{
    /// <summary>
    /// 手机归属地
    /// </summary>
    public class OfferingOperationLogDTO : ProcessLogBaseDTO
    {
        [MaxLength(16)]
        public string OfferingCode { get; set; }

        [MaxLength(256)]
        public string OfferingName { get; set; }

        public int OfferingId { get; set; }
        public virtual OfferingDTO Offering { get; set; }
    }
}
