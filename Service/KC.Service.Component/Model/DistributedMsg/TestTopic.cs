using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Base;

namespace KC.Model.Component.DistributedMsg
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class TestTopic : EntityBase
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime CreatedTime { get; set; }
    }
}
