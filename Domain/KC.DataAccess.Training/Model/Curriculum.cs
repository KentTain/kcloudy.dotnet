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
    /// 课程安排
    /// </summary>
    [Table(Tables.Curriculum)]
    public class Curriculum : EntityBase
    {
        [Key]
        public int CurriculumId { get; set; }

        /// <summary>
        /// 上课时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime CourceDate { get; set; }
        /// <summary>
        /// 上课时长
        /// </summary>
        public int CourceDuration { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        public string QrCode { get; set; }

        public int ClassRoomId { get; set; }
        [ForeignKey("ClassRoomId")]
        public ClassRoom ClassRoom { get; set; }

        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}
