using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;
using KC.Model.Workflow.Constants;

namespace KC.Model.Workflow
{
    [Table(Tables.WorkflowProRequestLog)]
    public class WorkflowProRequestLog : ProcessLogBase
    {
        public Guid ProcessId { get; set; }


        /// <summary>
        /// 流程编码：wfd2012020200001
        /// </summary>
        [MaxLength(20)]
        public string ProcessCode { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [MaxLength(512)]
        public string ProcessName { get; set; }

        /// <summary>
        /// 请求类型：0：流程完成后；1：流程回退
        /// </summary>
        public int RequestType { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [MaxLength(2000)]
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求数据参数
        /// </summary>
        public string RequestPostData { get; set; }

        /// <summary>
        /// 请求返回数据
        /// </summary>
        public string RequestResultData { get; set; }

        /// <summary>
        /// 请求错误数据
        /// </summary>
        public string RequestErrorData { get; set; }
    }
}
