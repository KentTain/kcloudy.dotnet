using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Model.Customer.Constants;

namespace KC.Model.Customer
{
    [Table(Tables.NotificationApplication)]
    public class NotificationApplication : Entity
    {
        [Key]
        public int ApplicationId { get; set; }
        /// <summary>
        /// 申请人Id
        /// </summary>
        public string ApplicantUserId { get; set; }
        /// <summary>
        /// 申请人名称
        /// </summary>
        public string ApplicantUserName { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplicantDateTime { get; set; }
        public WorkflowBusStatus Status { get; set; }

        public string SendTo { get; set; }
        public string CcTo { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string CustomerIds { get; set; }
        public string CustomerNames { get; set; }
        /// <summary>
        /// SMS/Email
        /// </summary>
        public string viewName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }   
    }
}
