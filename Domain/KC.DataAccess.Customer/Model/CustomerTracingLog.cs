using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Enums.CRM;
using KC.Framework.Base;
using KC.Model.Customer.Constants;

namespace KC.Model.Customer
{
    [Table(Tables.CustomerTracingLog)]
    public class CustomerTracingLog : ProcessLogBase
    {
        public CustomerTracingLog()
        {
            TracingType = TracingType.Manual;
        }

        public string ReferenceId { get; set; }
        public int? CustomerId { get; set; }

        [MaxLength(512)]
        public string CustomerName { get; set; }
        public int? CustomerContactId { get; set; }

        [MaxLength(50)]
        public string ContactName { get; set; } 
        public int? ActivityId { get; set; }
        public string ActivityName { get; set; }

        #region 电话

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Caller { get; set; }
        public string CallerPhone { get; set; }
        public string Callee { get; set; }
        public string CalleePhone { get; set; }
        public string CallTime { get; set; }
        public CallState? CallState { get; set; }

        [MaxLength(1000)]
        public string RecordURL { get; set; }

        #endregion

        #region 邮件短信

        public string Title { get; set; }
        public string From { get; set; }
        public string SendTo { get; set; }
        public string CcTo { get; set; }

        #endregion

        public TracingType TracingType { get; set; }

        [ForeignKey("CustomerId")]
        public CustomerInfo CustomerInfo { get; set; }
    }
}
