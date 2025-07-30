using KC.Service.DTO;
using KC.Model.Training;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Training
{
    [Serializable, DataContract(IsReference = true)]
    public class TeacherDTO : EntityBaseDTO
    {
        public TeacherDTO()
        {
            CourseIds = new List<int>();
            Courses = new List<CourseDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public int TeacherId { get; set; }
        /// <summary>
        /// 登录用户ID
        /// </summary>
        [DataMember]
        public string UserId { get; set; }
        /// <summary>
        /// <summary>
        /// 姓名
        /// </summary>
        [DataMember] 
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [DataMember] 
        public string Sex { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        [DataMember] 
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [DataMember] 
        public string Email { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        [MaxLength(12)]
        [DataMember] 
        public string IdentityCard { get; set; }

        [DataMember]
        public List<int> CourseIds { get; set; }

        [DataMember]
        public string CourseName { get; set; }

        [DataMember]
        public IEnumerable<CourseDTO> Courses { get; set; }
    }
}
