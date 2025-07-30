using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KC.Framework.Base
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class VodUploadSetting : ICloneable
    {
        /// <summary>
        /// 文件Id
        /// </summary>
        [DataMember]
        [ProtoMember(1)]
        public string RequestId { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public string VideoId { get; set; }

        [DataMember]
        [ProtoMember(3)]
        public string UploadEndpoint { get; set; }
        [DataMember]
        [ProtoMember(4)]
        public string UploadBucket { get; set; }
        [DataMember]
        [ProtoMember(5)]
        public string UploadFileName { get; set; }

        [DataMember]
        [ProtoMember(5)]
        public string AccessKeyId { get; set; }
        [DataMember]
        [ProtoMember(6)]
        public string AccessKeySecret { get; set; }
        [DataMember]
        [ProtoMember(7)]
        public string SecurityToken { get; set; }
        [DataMember]
        [ProtoMember(8)]
        public TimeSpan Expiration { get; set; }

        [DataMember]
        [ProtoMember(9)]
        public string ReferenceId1 { get; set; }
        [DataMember]
        [ProtoMember(10)]
        public string ReferenceId2 { get; set; }
        [DataMember]
        [ProtoMember(11)]
        public string ReferenceId3 { get; set; }
        [DataMember]
        [ProtoMember(12)]
        public string ReferenceId4 { get; set; }
        [DataMember]
        [ProtoMember(13)]
        public string ReferenceId5 { get; set; }
        [DataMember]
        [ProtoMember(14)]
        public string ReferenceId6 { get; set; }


        public BlobInfo Clone()
        {
            return (BlobInfo)this.MemberwiseClone();
        }
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
