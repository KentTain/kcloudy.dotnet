using System;
using System.Runtime.Serialization;

namespace KC.Service.Component.DTO
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class NLogEntityDTO: AzureTableEntityDTO
    {   [DataMember]
        public string Exception { get; set; }
        [DataMember]
        public string ExceptionData { get; set; }
        [DataMember]
        public string InnerException { get; set; }
        [DataMember]
        public string Level { get; set; }
        [DataMember]
        public string LoggerName { get; set; }
        [DataMember]
        public string LogTimeStamp { get; set; }
        [DataMember]
        public string MachineName { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string MessageWithLayout { get; set; }
        [DataMember]
        public string StackTrace { get; set; }
    }
}
