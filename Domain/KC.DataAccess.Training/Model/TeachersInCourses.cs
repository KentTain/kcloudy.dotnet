using KC.Framework.Base;
using KC.Model.Training.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KC.Model.Training
{
    /// <summary>
    /// 选课
    /// </summary>
    [Table(Tables.TeachersInCourses)]
    public class TeachersInCourses : EntityBase
    {
        public int TeacherId { get; set; }

        public int CourseId { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}
