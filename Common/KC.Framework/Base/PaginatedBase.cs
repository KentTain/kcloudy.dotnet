using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Framework.Base
{
    [Serializable, DataContract(IsReference = true)]
    //[ProtoContract]
    public class PaginatedBase<T> : EntityBase where T : EntityBase
    {
        public PaginatedBase() { }
        public PaginatedBase(int pageIndex, int pageSize, int total, List<T> rows, decimal? sum = null)
        {
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
            this.total = total;
            this.rows = rows;
        }

        [DataMember]
        public int pageIndex { get; set; }
        [DataMember]
        public int pageSize { get; set; }
        [DataMember]
        public int total { get; set; }
        [DataMember]
        public List<T> rows { get; set; }
    }
}
