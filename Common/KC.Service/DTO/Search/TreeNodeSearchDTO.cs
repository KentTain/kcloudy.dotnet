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
    /// 树结构的查询条件
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    public class TreeNodeSearchDTO : EntityBaseDTO
    {
        /// <summary>
        /// 需要查询的名称
        /// </summary>
        [DataMember]
        public String Name { get; set; }
        /// <summary>
        /// 需要排除的Id
        /// </summary>
        [DataMember]
        public Int32 ExcludeId { get; set; }
        /// <summary>
        /// 已选Id
        /// </summary>
        [DataMember]
        public Int32 SelectedId { get; set; }
        /// <summary>
        /// 是否包含所有分类
        /// </summary>
        [DataMember]
        public Boolean HasAll { get; set; }
        /// <summary>
        /// 是否包含根节点
        /// </summary>
        [DataMember]
        public Boolean HasRoot { get; set; }
        /// <summary>
        /// 树结构最深层级
        /// </summary>
        [DataMember]
        public Int32 MaxLevel { get; set; }

    }
}
