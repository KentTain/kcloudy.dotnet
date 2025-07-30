using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace KC.Framework.Base
{
    [Serializable]
    [DataContract]
    public enum SmsType
    {
        /// <summary>
        /// 通知
        /// </summary>
        [Description("通知")]
        [EnumMember]
        Notice = 0,

        /// <summary>
        /// 营销
        /// </summary>
        [Description("营销")]
        [EnumMember]
        Marketing = 1,

        /// <summary>
        /// 语音验证码
        /// </summary>
        [Description("语音验证码")]
        [EnumMember]
        VoiceCaptcha = 2
    }
}
