using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using KC.Framework.Extension;
using KC.Common;
using KC.Service.Enums.Workflow;

namespace KC.Service.DTO.Workflow
{
    /// <summary>
    /// 流程发起提交模型
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowExecuteData : EntityBaseDTO
    {
        public WorkflowExecuteData()
        {
            Id = Guid.NewGuid();
            FormData = new List<WorkflowProFieldDTO>();
        }

        [DataMember]
        public Guid Id { get; set; }

        #region 流程定义
        [DataMember]
        public Guid WorkflowDefId { get; set; }

        /// <summary>
        /// 流程编码：wfd2012020200001
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string WorkflowDefCode { get; set; }

        /// <summary>
        /// 流程版本：wfv2012020200001
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string WorkflowDefVersion { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string WorkflowDefName { get; set; }
        #endregion

        #region 任务数据
        [DataMember]
        public Guid TaskId { get; set; }
        [DataMember]
        public string TaskCode { get; set; }
        [DataMember]
        public string TaskName { get; set; }
        /// <summary>
        /// 节点类型：0：Start-开始；1：Task-任务、2：Condition-条件、3：SubFlow-子流程、4：End-结束
        /// </summary>
        [DataMember]
        public WorkflowNodeType TaskType { get; set; }

        [DataMember]
        public string TaskTypeString { get { return TaskType.ToDescription(); } }

        /// <summary>
        /// 通知人Id
        /// </summary>
        [DataMember]
        public string NotifyUserIds { get; set; }
        /// <summary>
        /// 通知人姓名
        /// </summary>
        [MaxLength(4000)]
        [DataMember]
        public string NotifyUserNames { get; set; }
        #endregion 

        #region 任务审核人数据
        /// <summary>
        /// 同意人Ids
        /// </summary>
        [DataMember]
        public string AgreeUserIds { get; set; }
        /// <summary>
        /// 同意人姓名
        /// </summary>
        [DataMember]
        public string AgreeUserNames { get; set; }

        /// <summary>
        /// 不同意人Ids
        /// </summary>
        [DataMember]
        public string DisagreeUserIds { get; set; }
        /// <summary>
        /// 不同意人姓名
        /// </summary>
        [DataMember]
        public string DisagreeUserNames { get; set; }

        /// <summary>
        /// 未处理人Ids
        /// </summary>
        [DataMember]
        public string UnProcessUserIds { get; set; }
        /// <summary>
        /// 未处理人姓名
        /// </summary>
        [DataMember]
        public string UnProcessUserNames { get; set; }

        /// <summary>
        /// 下一节点的所有审核人Ids
        /// </summary>
        [DataMember]
        public string AllUserIds { get; set; }
        /// <summary>
        /// 下一节点的所有审核人姓名列表
        /// </summary>
        [DataMember]
        public string AllUserNames { get; set; }
        #endregion

        #region  任务执行数据

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
        #endregion

        /// <summary>
        /// 表单数据
        /// </summary>
        [DataMember]
        public List<WorkflowProFieldDTO> FormData { get; set; }

    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowStartExecuteData : WorkflowExecuteData
    {
        public WorkflowStartExecuteData()
        {
        }

        #region 设置表单详情的Api地址

        /// <summary>
        /// 表单接入方式 </br>
        ///     0 ：ModelDefinition--表单数据接入
        ///     1 ：FormAddress--表单地址接入
        ///     99：None--无
        /// </summary>
        public WorkflowFormType WorkflowFormType { get; set; }
        /// <summary>
        /// FormType=FormAddress时：应用表单可访问地址
        /// </summary>
        [MaxLength(4000)]
        public string AppFormDetailApiUrl { get; set; }
        /// <summary>
        /// FormType=FormAddress时：应用表单可访问的QueryString
        /// </summary>
        [MaxLength(4000)]
        public string AppFormDetailQueryString { get; set; }
        #endregion

        #region 设置应用回调Api地址
        /// <summary>
        /// 业务审批通过后，应用回调地址
        /// </summary>
        [MaxLength(4000)]
        public string AppAuditSuccessApiUrl { get; set; }
        /// <summary>
        /// 业务审批退回后，应用回调地址
        /// </summary>
        [MaxLength(4000)]
        public string AppAuditReturnApiUrl { get; set; }

        /// <summary>
        /// 业务审批需要传出的QueryString
        /// </summary>
        [MaxLength(4000)]
        public string AppAuditQueryString { get; set; }

        #endregion

    }
}
