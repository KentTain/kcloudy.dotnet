using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.App;
using KC.Framework.Base;
using KC.Framework.Tenant;
using KC.Model.App.Constants;

namespace KC.Model.App
{
    [Table(Tables.ApplicationLog)]
    public class ApplicationLog : ProcessLogBase
    {
        public AppLogType appLogType { get; set; }

        public Guid ApplicationId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [MaxLength(128)]
        public string ApplicationName { get; set; }
    }
}
