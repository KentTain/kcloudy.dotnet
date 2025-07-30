using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Offering.Constants;
using KC.Framework.Base;

namespace KC.Model.Offering
{
    /// <summary>
    /// 手机归属地
    /// </summary>
    [Table(Tables.CategoryOperationLog)]
    public class CategoryOperationLog : ProcessLogBase
    {
        [MaxLength(256)]
        public string CategoryName { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
