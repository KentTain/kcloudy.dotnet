using System;
using System.Collections.Generic;
using System.Text;
using KC.Service.DTO;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace KC.Service.DTO.Blog
{
    [Serializable, DataContract(IsReference = true)]
    public class CategoryDTO : EntityBaseDTO
    {
        [DataMember]
        public bool IsEditMode { get; set; }
        [DataMember]
        public int Id { get; set; }
        [MaxLength(64)]
        [DataMember]
        public string Name { get; set; }
        [MaxLength(2046)]
        [DataMember]
        public string Remark { get; set; }
        [DataMember]
        public int OrderBy { get; set; }
    }
}
