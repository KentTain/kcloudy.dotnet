using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Service.DTO;

namespace KC.Service.DTO.Search
{
    /// <summary>
    /// 树结构判断名称是否重复的查询条件
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class TreeNodeNameExistsDTO : EntityBaseDTO
    {

        /// <summary>
        /// 需要排除的Id
        /// </summary>
        [DataMember]
        public Int32 Id { get; set; }
        /// <summary>
        /// 已选Id
        /// </summary>
        [DataMember]
        public Int32 PId { get; set; }
        /// <summary>
        /// 需要查询的名称
        /// </summary>
        [DataMember]
        public String Name { get; set; }
    }
}
