using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Service.DTO;

namespace KC.Service.Workflow.DTO
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class WorkflowProRequestLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public Guid ProcessId { get; set; }

        /// <summary>
        /// 流程编码：wfd2012020200001
        /// </summary>
        [MaxLength(20)]
        [DataMember]
        public string ProcessCode { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [MaxLength(512)]
        [DataMember]
        public string ProcessName { get; set; }
        /// <summary>
        /// 请求类型：0：流程完成后；1：流程回退
        /// </summary>
        public int RequestType { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        [MaxLength(2000)]
        [DataMember]
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求数据参数
        /// </summary>
        [DataMember]
        public string RequestPostData { get; set; }

        /// <summary>
        /// 请求返回数据
        /// </summary>
        [DataMember]
        public string RequestResultData { get; set; }

        /// <summary>
        /// 请求错误数据
        /// </summary>
        [DataMember]
        public string RequestErrorData { get; set; }
    }
}
