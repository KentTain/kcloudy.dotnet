using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Enums.App
{
    [DataContract]
    public enum CallChargeStatus
    {
        [EnumMember]
        [Display(Name = "拨打电话")]
        CallStart = 0,
        [EnumMember]
        [Display(Name = "拨打失败")]
        CallFailed = 1,
        [EnumMember]
        [Display(Name = "获取通话记录")]
        GetCallLog = 2,
        [EnumMember]
        [Display(Name = "获取通话录音")]
        DownloadCallVoice = 3,
    }
}
