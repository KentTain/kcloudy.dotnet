using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Service.DTO
{
    [Serializable]
    [ProtoContract]
    [DataContract(IsReference = true)]
    public abstract class DataPermitEntityDTO : EntityDTO
    {
        public DataPermitEntityDTO()
        {
            //OrgIds = new List<string>();
            //RoleIds = new List<string>();
            //UserIds = new List<string>();
        }

        /// <summary>
        /// 执行组织Ids
        /// </summary>
        [DataMember]
        public string OrgCodes { get; set; }
        /// <summary>
        /// 执行组织名称
        /// </summary>
        [DataMember]
        public string OrgNames { get; set; }

        /// <summary>
        /// 执行角色Ids
        /// </summary>
        [DataMember]
        public string RoleIds { get; set; }
        /// <summary>
        /// 执行角色名称
        /// </summary>
        [DataMember]
        public string RoleNames { get; set; }

        /// <summary>
        /// 执行人Ids
        /// </summary>
        [DataMember]
        public string UserIds { get; set; }
        /// <summary>
        /// 执行人姓名
        /// </summary>
        [DataMember]
        public string UserNames { get; set; }

        /// <summary>
        /// 需排除的执行人Ids
        /// </summary>
        [DataMember]
        public string ExceptUserIds { get; set; }
        /// <summary>
        /// 需排除的执行人姓名
        /// </summary>
        [DataMember]
        public string ExceptUserNames { get; set; }

    }
}
