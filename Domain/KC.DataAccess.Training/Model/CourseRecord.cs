using KC.Enums.Training;
using KC.Framework.Base;
using KC.Model.Training.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KC.Model.Training
{
    /// <summary>
    /// 上课记录
    /// </summary>
    [Table(Tables.CourseRecord)]
    public class CourseRecord : EntityBase
    {
        [Key]
        public int CourseRecordId { get; set; }
        /// <summary>
        /// 课程安排Id
        /// </summary>
        public int CurriculumId { get; set; }
        /// <summary>
        /// 教室Id
        /// </summary>
        public int ClassRoomId { get; set; }
        /// <summary>
        /// 教室名称
        /// </summary>
        public string ClassRoom { get; set; }
        /// <summary>
        /// 课程Id
        /// </summary>
        public int CourseId { get; set; }
        /// <summary>
        /// 课程名称
        /// </summary>
        public string Course { get; set; }
        /// <summary>
        /// 老师Id
        /// </summary>
        public int TeacherId { get; set; }
        /// <summary>
        /// 老师名称
        /// </summary>
        public string TeacherName { get; set; }
        /// <summary>
        /// 学生Id
        /// </summary>
        public int StudentId { get; set; }
        /// <summary>
        /// 学生名称
        /// </summary>
        public string StudentName { get; set; }
        /// <summary>
        /// 打卡时间
        /// </summary>
        public DateTime CreatedDateTime { get; set; }
        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime ConfirmDateTime { get; set; }
        /// <summary>
        /// 上课时长
        /// </summary>
        public int CourseDuration { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public CourseRecordStatus Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
