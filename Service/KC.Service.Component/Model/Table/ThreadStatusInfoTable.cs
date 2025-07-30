using System;

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using KC.Model.Job.Table;

namespace KC.Model.Component.Table
{
    public class ThreadStatusInfoTable : ThreadStatusInfo, Azure.Data.Tables.ITableEntity
    {
        [NotMapped]
        [DataMember]
        public Azure.ETag ETag { get; set; }
    }
}
