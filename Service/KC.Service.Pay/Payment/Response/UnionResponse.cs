using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Pay.Response
{
    public class UnionResponse
    {
        [DataMember]
        public NameValueCollection Collection { get; set; }
        [DataMember]
        public string HttpMethod { get; set; }
        [DataMember]
        public Dictionary<string, string> ResData { get; set; }
    }
}
