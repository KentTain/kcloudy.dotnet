using KC.Enums.Training;
using KC.Framework.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Training
{
    /// <summary>
    /// 上课记录
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class CourseRecordDTO : EntityBase
    {
        [DataMember]
        public int CourseRecordId { get; set; }
        /// <summary>
        /// 课程安排Id
        /// </summary>
        [DataMember] 
        public int CurriculumId { get; set; }
        /// <summary>
        /// 教室Id
        /// </summary>
        [DataMember] 
        public int ClassRoomId { get; set; }
        /// <summary>
        /// 教室名称
        /// </summary>
        [DataMember] 
        public string ClassRoom { get; set; }
        /// <summary>
        /// 课程Id
        /// </summary>
        [DataMember] 
        public int CourseId { get; set; }
        /// <summary>
        /// 课程名称
        /// </summary>
        [DataMember] 
        public string Course { get; set; }
        /// <summary>
        /// 老师Id
        /// </summary>
        [DataMember] 
        public int TeacherId { get; set; }
        /// <summary>
        /// 老师名称
        /// </summary>
        [DataMember] 
        public string TeacherName { get; set; }
        /// <summary>
        /// 学生Id
        /// </summary>
        [DataMember] 
        public int StudentId { get; set; }
        /// <summary>
        /// 学生名称
        /// </summary>
        [DataMember] 
        public string StudentName { get; set; }
        /// <summary>
        /// 打卡时间
        /// </summary>
        [DataMember] 
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// 确认时间
        /// </summary>
        [DataMember] 
        public DateTime ConfirmDateTime { get; set; }
        /// <summary>
        /// 上课时长
        /// </summary>
        [DataMember] 
        public int CourceDuration { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember] 
        public CourseRecordStatus Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember] 
        public string Remark { get; set; }

    }
}
