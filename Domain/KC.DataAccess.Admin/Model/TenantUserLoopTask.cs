using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Model.Admin.Constants;
using KC.Framework.Base;

namespace KC.Model.Admin
{
    /// <summary>
    /// 租户定时任务
    /// </summary>
    [Table(Tables.TenantUserLoopTask)]
    public class TenantUserLoopTask : Entity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(128)]
        public string BusinessId { get; set; }

        [MaxLength(128)]
        public string Status { get; set; }

        [MaxLength(128)]
        public string Type { get; set; }

        [MaxLength(128)]
        public string TenantName { get; set; }
    }
}
