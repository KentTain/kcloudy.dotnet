using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using KC.Framework.Base;
using ProtoBuf;

namespace KC.Service.Base
{
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract]
    public class SmsConfig : ConfigBase
    {
        public SmsConfig()
        {
            ConfigType = Framework.Base.ConfigType.SmsConfig;
            Action = "send";
        }

        #region Base
        [ProtoMember(1)]
        [DataMember]
        public SmsType? Type { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string ProviderName { get; set; }
        /// <summary>
        /// 短信服务器地址
        /// </summary>
        [ProtoMember(3)]
        [DataMember]
        public string SmsUrl { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
        [ProtoMember(4)]
        [DataMember]
        public string UserId { get; set; }
        /// <summary>
        /// 发送用户帐号
        /// </summary>
        [ProtoMember(5)]
        [DataMember]
        public string UserAccount { get; set; }
        /// <summary>
        /// 发送帐号密码
        /// </summary>
        [ProtoMember(6)]
        [DataMember]
        public string Password { get; set; }
        /// <summary>
        /// 定时发送时间,为空表示立即发送，定时发送格式2010-10-24 09:08:10
        /// </summary>
        [ProtoMember(7)]
        [DataMember]
        public string SendTime { get; set; }
        /// <summary>
        /// 发送任务命令,设置为固定的:send
        /// </summary>
        [ProtoMember(8)]
        [DataMember]
        public string Action { get; set; }
        /// <summary>
        /// 扩展子号,请先询问配置的通道是否支持扩展子号，如果不支持，请填空。子号只能为数字，且最多5位数。
        /// </summary>
        [ProtoMember(9)]
        [DataMember]
        public string Extno { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public string Signature { get; set; }
        #endregion

        #region 华为接口
        [ProtoMember(11)]
        public string MethodName { get; set; }
        [ProtoMember(12)]
        public string Spid { get; set; }
        [ProtoMember(13)]
        public string Appid { get; set; }
        [ProtoMember(14)]
        public string Passwd { get; set; }
        [ProtoMember(15)]
        public string ChargeNbr { get; set; }
        [ProtoMember(16)]
        public string Key { get; set; }
        [ProtoMember(17)]
        public string DisplayNbr { get; set; }
        [ProtoMember(18)]
        public string TempletId { get; set; }
        [ProtoMember(19)]
        public string vReplay { get; set; }
        [ProtoMember(20)]
        public string ReplayTTS { get; set; }
        [ProtoMember(21)]
        public string ReplayTimes { get; set; }
        [ProtoMember(22)]
        [DataMember]
        public string Value1 { get; set; }
        [ProtoMember(23)]
        [DataMember]
        public string Value2 { get; set; }
        [ProtoMember(24)]
        [DataMember]
        public string Value3 { get; set; }
        [ProtoMember(25)]
        [DataMember]
        public string Value4 { get; set; }
        [ProtoMember(26)]
        public string Value5 { get; set; }
        [ProtoMember(27)]
        public string Value6 { get; set; }

        #endregion

        public override string GetConfigObjectXml()
        {
            var doc = GetDocumentRoot();

            SerializeToXml(doc.Root);
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        protected new XElement SerializeToXml(XElement element)
        {
            base.SerializeToXml(element);

            element.Add(new XElement("Type", this.Type != null? this.Type.ToString() : ""));
            element.Add(new XElement("ProviderName", this.ProviderName));
            element.Add(new XElement("SmsUrl", this.SmsUrl));
            element.Add(new XElement("UserId", this.UserId));
            element.Add(new XElement("UserAccount", this.UserAccount));
            element.Add(new XElement("Password", this.Password));
            element.Add(new XElement("SendTime", this.SendTime));
            element.Add(new XElement("Action", this.Action));
            element.Add(new XElement("Extno", this.Extno));
            element.Add(new XElement("Signature", this.Signature));

            element.Add(new XElement("MethodName", this.MethodName));
            element.Add(new XElement("Spid", this.Spid));
            element.Add(new XElement("Appid", this.Appid));
            element.Add(new XElement("Passwd", this.Passwd));
            element.Add(new XElement("ChargeNbr", this.ChargeNbr));
            element.Add(new XElement("Key", this.Key));
            element.Add(new XElement("DisplayNbr", this.DisplayNbr));
            element.Add(new XElement("TempletId", this.TempletId));
            element.Add(new XElement("vReplay", this.vReplay));
            element.Add(new XElement("ReplayTTS", this.ReplayTTS));
            element.Add(new XElement("ReplayTimes", this.ReplayTimes));

            element.Add(new XElement("Value1", this.Value1));
            element.Add(new XElement("Value2", this.Value2));
            element.Add(new XElement("Value3", this.Value3));
            element.Add(new XElement("Value4", this.Value4));
            element.Add(new XElement("Value5", this.Value5));
            element.Add(new XElement("Value6", this.Value6));

            return element;
        }

        public SmsConfig(string xml)
            : base(xml)
        {
            try
            {
                var element = base.GetRootElement(xml);

                #region base
                if (element.Element("Type") != null)
                    Type = (SmsType) Enum.Parse(typeof (SmsType), element.Element("ProviderName").Value);
                ProviderName = element.Element("ProviderName") != null
                    ? element.Element("ProviderName").Value
                    : string.Empty;
                SmsUrl = element.Element("SmsUrl") != null
                    ? element.Element("SmsUrl").Value 
                    : string.Empty;
                UserId = element.Element("UserId") != null
                    ? element.Element("UserId").Value
                    : string.Empty;
                UserAccount = element.Element("UserAccount") != null
                    ? element.Element("UserAccount").Value
                    : string.Empty;
                Password = element.Element("Password") != null
                    ? element.Element("Password").Value
                    : string.Empty;
                SendTime = element.Element("SendTime") != null
                    ? element.Element("SendTime").Value
                    : string.Empty;
                Action = element.Element("Action") != null
                    ? element.Element("Action").Value
                    : string.Empty;
                Extno = element.Element("Extno") != null
                    ? element.Element("Extno").Value
                    : string.Empty;
                Signature = element.Element("Signature") != null
                    ? element.Element("Signature").Value
                    : string.Empty;
                #endregion

                #region 华为接口
                MethodName = element.Element("MethodName") != null
                    ? element.Element("MethodName").Value
                    : string.Empty;
                Spid = element.Element("Spid") != null
                    ? element.Element("Spid").Value
                    : string.Empty;
                Appid = element.Element("Appid") != null
                    ? element.Element("Appid").Value
                    : string.Empty;
                Passwd = element.Element("Passwd") != null
                    ? element.Element("Passwd").Value
                    : string.Empty;
                ChargeNbr = element.Element("ChargeNbr") != null
                    ? element.Element("ChargeNbr").Value
                    : string.Empty;
                Key = element.Element("Key") != null
                    ? element.Element("Key").Value
                    : string.Empty;
                DisplayNbr = element.Element("DisplayNbr") != null
                    ? element.Element("DisplayNbr").Value
                    : string.Empty;
                TempletId = element.Element("TempletId") != null
                    ? element.Element("TempletId").Value
                    : string.Empty;
                vReplay = element.Element("vReplay") != null
                    ? element.Element("vReplay").Value
                    : string.Empty;
                ReplayTTS = element.Element("ReplayTTS") != null
                    ? element.Element("ReplayTTS").Value
                    : string.Empty;
                ReplayTimes = element.Element("ReplayTimes") != null
                    ? element.Element("ReplayTimes").Value
                    : string.Empty;

                Value1 = element.Element("Value1") != null
                    ? element.Element("Value1").Value
                    : string.Empty;
                Value2 = element.Element("Value2") != null
                    ? element.Element("Value3").Value
                    : string.Empty;
                Value3 = element.Element("Value3") != null
                    ? element.Element("Value3").Value
                    : string.Empty;
                Value4 = element.Element("Value4") != null
                    ? element.Element("Value4").Value
                    : string.Empty;
                Value5 = element.Element("Value5") != null
                    ? element.Element("Value5").Value
                    : string.Empty;
                Value6 = element.Element("Value6") != null
                    ? element.Element("Value6").Value
                    : string.Empty;
                #endregion
            }
            catch (Exception)
            {
                throw new Exception("Invalid SmsConfig Definition");
            }
        }
    }

    [Serializable, DataContract(IsReference = true)]
    public class SmsResult
    {
        /// <summary>
        /// 状态 success/faild
        /// </summary>
        [DataMember]
        public string returnstatus { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        [DataMember]
        public string message { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        [DataMember]
        public double remainpoint { get; set; }

        /// <summary>
        /// 任务序列Id
        /// </summary>
        [DataMember]
        public string taskID { get; set; }

        /// <summary>
        /// 成功短信数：当成功后返回提交成功短信数
        /// </summary>
        [DataMember]
        public int successCounts { get; set; }
    }
}
