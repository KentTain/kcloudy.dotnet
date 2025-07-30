using KC.Service.DTO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Model.Training
{
    /// <summary>
    /// 教室
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class ClassRoomDTO : EntityDTO
    {
        public ClassRoomDTO()
        {
            Curriculums = new List<CurriculumDTO>();
        }

        [DataMember]
        public bool IsEditMode { get; set; }

        [DataMember]
        public int ClassRoomId { get; set; }
        /// <summary>
        /// <summary>
        /// 教室名
        /// </summary>
        [DataMember] 
        public string Name { get; set; }
        /// <summary>
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember] 
        public string Description { get; set; }

        public ICollection<CurriculumDTO> Curriculums { get; set; }
    }
}
