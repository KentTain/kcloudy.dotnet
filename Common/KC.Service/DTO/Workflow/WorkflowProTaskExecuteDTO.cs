using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using System.Runtime.Serialization;
using KC.Framework.Extension;
using KC.Service.Enums.Workflow;
using KC.Common;

namespace KC.Service.DTO.Workflow
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowProTaskExecuteDTO : EntityBaseDTO
    {
        public WorkflowProTaskExecuteDTO()
        {
        }

        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 执行人Id
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ExecuteUserId { get; set; }
        /// <summary>
        /// 执行人姓名
        /// </summary>
        [MaxLength(50)]
        [DataMember]
        public string ExecuteUserName { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        [DataMember]
        public DateTime ExecuteDateTime { get; set; }

        /// <summary>
        /// 下一节点的审核人Ids
        /// </summary>
        //[MaxLength(4000)] 
        [DataMember]
        public string NextAuditorUserIds { get; set; }
        /// <summary>
        /// 下一节点的审核人姓名列表
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string NextAuditorUserNames { get; set; }

        /// <summary>
        /// 流程任务执行状态
        /// </summary>
        [DataMember]
        public WorkflowExecuteStatus ExecuteStatus { get; set; }
        [DataMember]
        public string ExecuteStatusString { get { return ExecuteStatus.ToDescription(); } }

        /// <summary>
        /// 执行附件
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string ExecuteFileBlob { get; set; }
        /// <summary>
        /// 附件对象
        /// </summary>
        [DataMember]
        public BlobInfoDTO ExecuteFile
        {
            get
            {
                if (ExecuteFileBlob.IsNullOrEmpty())
                    return null;

                return SerializeHelper.FromJson<BlobInfoDTO>(ExecuteFileBlob);
            }
        }

        /// <summary>
        /// 执行描述
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string ExecuteRemark { get; set; }

        [DataMember]
        public Guid TaskId { get; set; }
        [DataMember]
        public string TaskCode { get; set; }
        [DataMember]
        public string TaskName { get; set; }
        [DataMember]
        public WorkflowProTaskDTO Task { get; set; }
    }
}
