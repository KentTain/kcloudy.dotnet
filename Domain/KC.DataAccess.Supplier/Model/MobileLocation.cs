using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Supplier.Constants;
using KC.Framework.Base;

namespace KC.Model.Supplier
{
    /// <summary>
    /// 手机归属地
    /// </summary>
    [Table(Tables.MobileLocation)]
    public class MobileLocation : EntityBase
    {
        [Key]
        public int Id { get; set; }

        public string Mobile { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Corp { get; set; }

        public string AreaCode { get; set; }

        public string PostCode { get; set; }
    }
}
