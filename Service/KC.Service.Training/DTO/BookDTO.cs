using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Training
{
    /// <summary>
    /// 课件
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class BookDTO : EntityDTO
    {
        public BookDTO()
        {
            CourseIds = new List<int>();
            Courses = new List<CourseDTO>();
        }
        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public int BookId { get; set; }
        /// <summary>
        /// <summary>
        /// 书名
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// <summary>
        /// 地址
        /// </summary>
        [DataMember] 
        public string Url { get; set; }
        /// <summary>
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember] 
        public string Description { get; set; }

        [DataMember]
        public List<int> CourseIds { get; set; }
        [DataMember]
        public IEnumerable<CourseDTO> Courses { get; set; }
    }
}
