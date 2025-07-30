using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Model.Training.Constants
{
    public sealed class Tables
    {
        private const string Prx = "trn_";

        public const string Teacher = Prx + "Teacher";
        public const string Student = Prx + "Student";
        public const string ClassRoom = Prx + "ClassRoom";
        public const string Course = Prx + "Course";
        public const string Book = Prx + "Book";

        public const string Curriculum = Prx + "Curriculum";
        public const string CourseRecord = Prx + "CourseRecord";

        public const string BooksInCourses = Prx + "BooksInCourses";
        public const string StudentsInCourses = Prx + "StudentsInCourses";
        public const string TeachersInCourses = Prx + "TeachersInCourses";
    }
}
