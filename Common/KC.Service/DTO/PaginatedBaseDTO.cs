using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;

namespace KC.Service.DTO
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class PaginatedBaseDTO<T> :EntityBaseDTO where T : EntityBaseDTO
    {
        public PaginatedBaseDTO() { }
        public PaginatedBaseDTO(int pageIndex, int pageSize, int total, List<T> rows)
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
