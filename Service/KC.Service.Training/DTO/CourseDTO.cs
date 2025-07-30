using KC.Service.DTO;
using KC.Service.DTO.Training;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Training
{
    /// <summary>
    /// 课程
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class CourseDTO : EntityDTO
    {
        public CourseDTO()
        {
            Curriculums = new List<CurriculumDTO>();
            Books = new List<BookDTO>();
            Students = new List<StudentDTO>();
            Teachers = new List<TeacherDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public int CourseId { get; set; }
        /// <summary>
        /// <summary>
        /// 课程名
        /// </summary>
        [DataMember] 
        public string Name { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        [DataMember] 
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        [DataMember] 
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 人数限制
        /// </summary>
        [DataMember] 
        public int StudentLimit { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int TeacherId { get; set; }
        [DataMember]
        public TeacherDTO Teacher { get; set; }
        [DataMember]
        public List<CurriculumDTO> Curriculums { get; set; }

        [DataMember]
        public IEnumerable<BookDTO> Books { get; set; }
        [DataMember]
        public IEnumerable<StudentDTO> Students { get; set; }
        [DataMember]
        public IEnumerable<TeacherDTO> Teachers { get; set; }
    }
}
