using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace KC.Framework.Base
{
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class ProcessLogBase : EntityBase
    {
        public ProcessLogBase()
        {
            OperateDate = DateTime.UtcNow;
            Type = ProcessLogType.Success;
        }
        /// <summary>
        /// 日志Id
        /// </summary>
        [Key]
        [DataMember]
        public int ProcessLogId { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        [DataMember]
        public ProcessLogType Type { get; set; }
        /// <summary>
        /// 当前操作对象的UserId
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string OperatorId { get; set; }
        /// <summary>
        /// 当前操作对象的DisplayName
        /// </summary>
        [MaxLength(50)]
        [DataMember]
        public string Operator { get; set; }
        /// <summary>
        /// 日志生成日期（UTC时间）
        /// </summary>
        [DataMember]
        public System.DateTime OperateDate { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        [DataMember]
        [MaxLength(4000)]
        public string Remark { get; set; }
    }
}
