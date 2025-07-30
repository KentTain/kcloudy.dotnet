using KC.Framework.Base;
using KC.Model.Training.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KC.Model.Training
{
    /// <summary>
    /// 选课件
    /// </summary>
    [Table(Tables.BooksInCourses)]
    public class BooksInCourses : EntityBase
    {
        public int BookId { get; set; }

        public int CourseId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}
