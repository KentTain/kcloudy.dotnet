using KC.Service.DTO;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Blog
{
    [Serializable, DataContract(IsReference = true)]
    public class SettingDTO : PropertyAttributeBaseDTO
    {
        public const string Setting_Title = "title";
        public const string Setting_AboutMe = "aboutme";
        public const string Setting_Email = "email";
        [MaxLength(128)]
        [DataMember]
        public string UserId { get; set; }
        [MaxLength(128)]
        [DataMember]
        public int UserName { get; set; }
    }
}
