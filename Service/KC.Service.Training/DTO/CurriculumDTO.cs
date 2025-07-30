using KC.Framework.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Training
{
    /// <summary>
    /// 课程安排
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class CurriculumDTO : EntityBase
    {
        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public int CurriculumId { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime CourceDate { get; set; }
        /// <summary>
        /// 上课时长
        /// </summary>
        [DataMember] 
        public int CourceDuration { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        [DataMember] 
        public string QrCode { get; set; }
        [DataMember]
        public int ClassRoomId { get; set; }
        [DataMember]
        public ClassRoomDTO ClassRoom { get; set; }
        [DataMember]
        public int CourseId { get; set; }
        [DataMember]
        public CourseDTO Course { get; set; }
    }
}
