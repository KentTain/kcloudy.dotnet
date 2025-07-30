using System.ComponentModel.DataAnnotations;
using KC.Service.DTO;

namespace KC.Service.DTO.Offering
{
    /// <summary>
    /// 手机归属地
    /// </summary>
    public class CategoryOperationLogDTO : ProcessLogBaseDTO
    {
        [MaxLength(256)]
        public string CategoryName { get; set; }

        public int CategoryId { get; set; }
        public virtual CategoryDTO Category { get; set; }
    }
}
