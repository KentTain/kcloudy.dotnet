using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using KC.Model.Job.Table;


namespace KC.Model.Component.Table
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class QueueErrorMessageTable : QueueErrorMessage, Azure.Data.Tables.ITableEntity
    {
        [NotMapped]
        [DataMember]
        public Azure.ETag ETag { get; set; }

    }
}
