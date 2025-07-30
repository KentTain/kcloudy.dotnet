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
    [Table(Tables.StudentsInCourses)]
    public class StudentsInCourses : EntityBase
    {
        public int StudentId { get; set; }

        public int CourseId { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}
