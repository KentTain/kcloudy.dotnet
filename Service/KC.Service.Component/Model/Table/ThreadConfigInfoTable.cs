using System;
using System.Collections.Generic;

using KC.Model.Job.Table;

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace KC.Model.Component.Table
{
    public class ThreadConfigInfoTable : ThreadConfigInfo, Azure.Data.Tables.ITableEntity
    {
        [NotMapped]
        [DataMember]
        public Azure.ETag ETag { get; set; }

    }
}
