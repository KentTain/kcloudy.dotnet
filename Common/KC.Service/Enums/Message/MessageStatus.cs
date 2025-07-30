using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Enums.Message
{
    [DataContract]
    public enum MessageStatus
    {
        [EnumMember]
        [Display(Name = "未读")]
        Unread = 0,
        [EnumMember]
        [Display(Name = "已读")]
        Read = 1,
        [EnumMember]
        [Display(Name = "已删除")]
        Deleted = 2
    }
}
