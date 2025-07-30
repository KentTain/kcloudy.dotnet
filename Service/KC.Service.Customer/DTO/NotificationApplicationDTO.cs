using System;
using System.Runtime.Serialization;
using KC.Framework.Base;
using KC.Service.DTO;

namespace KC.Service.DTO.Customer
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class NotificationApplicationDTO : EntityDTO
    {
        [DataMember]
        public int ApplicationId { get; set; }
        /// <summary>
        /// 申请人Id
        /// </summary>
        [DataMember]
        public string ApplicantUserId { get; set; }
        /// <summary>
        /// 申请人名称
        /// </summary>
        [DataMember]
        public string ApplicantUserName { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        [DataMember]
        public DateTime ApplicantDateTime { get; set; }
        [DataMember]
        public WorkflowBusStatus Status { get; set; }
        [DataMember]
        public string SendTo { get; set; }
        [DataMember]
        public string CcTo { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string CustomerIds { get; set; }
        [DataMember]
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
