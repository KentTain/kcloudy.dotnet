using KC.Enums.Training;
using KC.Framework.Base;
using KC.Model.Training.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace KC.Model.Training
{
    /// <summary>
    /// 学生
    /// </summary>
    [Table(Tables.Student)]
    public class Student : Entity
    {
        public Student()
        {
            CourseSelects = new List<StudentsInCourses>();
        }

        [Key]
        public int StudentId { get; set; }
        /// <summary>
        /// 登录用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        [MaxLength(12)]
        public string IdentityCard { get; set; }

        /// <summary>
        /// 学生状态
        /// </summary>
        public AccountStatus Status { get; set; }

        public ICollection<StudentsInCourses> CourseSelects { get; set; }
        [NotMapped]
        public IEnumerable<Course> Courses => CourseSelects.Select(e => e.Course);
    }
}
