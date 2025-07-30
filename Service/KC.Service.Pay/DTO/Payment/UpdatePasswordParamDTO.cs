using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO.Pay
{
    public class UpdatePasswordParamDTO : PayBaseParamDTO
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        [DataMember]
        public string OldPassword { get; set; }
    }
}
