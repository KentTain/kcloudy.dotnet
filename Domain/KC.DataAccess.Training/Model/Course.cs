using KC.Enums.Training;
using KC.Framework.Base;
using KC.Model.Training.Constants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace KC.Model.Training
{
    /// <summary>
    /// 课程
    /// </summary>
    [Table(Tables.Course)]
    public class Course : Entity
    {
        public Course()
        {
            Curriculums = new List<Curriculum>();
            BookSelects = new List<BooksInCourses>();
            StudentSelects = new List<StudentsInCourses>();
            TeacherSelects = new List<TeachersInCourses>();
        }

        [Key]
        public int CourseId { get; set; }
        /// <summary>
        /// <summary>
        /// 课程名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 课程状态
        /// </summary>
        public CourseStatus Status { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 人数限制
        /// </summary>
        public int StudentLimit { get; set; }

        /// <summary>
        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "结束时间")]
        public string Description { get; set; }

        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }

        public ICollection<Curriculum> Curriculums { get; set; }

        public ICollection<BooksInCourses> BookSelects { get; set; }
        [NotMapped]
        public IEnumerable<Book> Books => BookSelects.Select(e => e.Book);

        public ICollection<StudentsInCourses> StudentSelects { get; set; }
        [NotMapped]
        public IEnumerable<Student> Students => StudentSelects.Select(e => e.Student);

        public ICollection<TeachersInCourses> TeacherSelects { get; set; }
        [NotMapped]
        public IEnumerable<Teacher> Teachers => TeacherSelects.Select(e => e.Teacher);
    }
}
