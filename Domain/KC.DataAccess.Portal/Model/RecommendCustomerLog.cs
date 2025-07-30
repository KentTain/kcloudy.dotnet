using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Portal.Constants;
using KC.Framework.Base;

namespace KC.Model.Portal
{
    /// <summary>
    /// 商品日志
    /// </summary>
    [Table(Tables.RecommendCustomerLog)]
    public class RecommendCustomerLog : ProcessLogBase
    {
        public int RecommendId { get; set; }
        /// <summary>
        /// 引用商品/招采Id
        /// </summary>
        [MaxLength(20)]
        public string RecommendRefCode { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        [MaxLength(20)]
        public string RecommendCode { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [MaxLength(256)]
        public string RecommendName { get; set; }
    }
}
