using KC.Framework.Base;
using KC.Model.Training.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KC.Model.Training
{
    /// <summary>
    /// 教室
    /// </summary>
    [Table(Tables.ClassRoom)]
    public class ClassRoom : Entity
    {
        public ClassRoom()
        {
            Curriculums = new List<Curriculum>();
        }


        [Key]
        public int ClassRoomId { get; set; }
        /// <summary>
        /// <summary>
        /// 教室名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public ICollection<Curriculum> Curriculums { get; set; }
    }
}
