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
    /// 课件
    /// </summary>
    [Table(Tables.Book)]
    public class Book : Entity
    {
        public Book()
        {
            BookSelects = new List<BooksInCourses>();
        }

        [Key]
        public int BookId { get; set; }
        /// <summary>
        /// <summary>
        /// 书名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public ICollection<BooksInCourses> BookSelects { get; set; }
        [NotMapped]
        public IEnumerable<Course> Courses => BookSelects.Select(e => e.Course);
    }
}
