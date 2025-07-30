using System;
using System.Runtime.Serialization;
using KC.Framework.Base;

namespace KC.Model.Component.DistributedMsg
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class SyncElectronicBillStatus : Entity
    {
        public SyncElectronicBillStatus()
        {
        }

        /// <summary>
        /// 电子票据在数据库中的Id
        /// </summary>
        [DataMember]
        public int? DbElectronicBillId { get; set; }
        /// <summary>
        /// 电子票据Id
        /// </summary>
        [DataMember]
        public string ElectronicBillId { get; set; }
        /// <summary>
        /// 银行接口的请求地址
        /// </summary>
        [DataMember]
        public string RequestUrl { get; set; }
        /// <summary>
        /// 银行接口的请求的报文
        /// </summary>
        [DataMember]
        public string RequestXml { get; set; }
    }
}
