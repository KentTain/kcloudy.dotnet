using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using KC.Framework.Extension;
using KC.Enums.CRM;
using KC.Service.DTO;

namespace KC.Service.DTO.Customer
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class CustomerTracingLogDTO : ProcessLogBaseDTO
    {
        [DataMember]
        public string ReferenceId { get; set; }

        [DataMember]
        public int? CustomerId { get; set; }

        [MaxLength(512)]
        [DataMember]
        public string CustomerName { get; set; }

        [DataMember]
        public int? CustomerContactId { get; set; }

        [MaxLength(50)]
        [DataMember]
        public string ContactName { get; set; }

        #region 电话

        [DataMember]
        public DateTime? StartTime { get; set; }

        [DataMember]
        public DateTime? EndTime {
            get
            {
                if (StartTime.HasValue && !string.IsNullOrEmpty(CallTime))
                {
                    var hour = Convert.ToInt32(CallTime.Split(':')[0]);
                    var min = Convert.ToInt32(CallTime.Split(':')[1]);
                    var second = Convert.ToInt32(CallTime.Split(':')[2]);
                    return StartTime.Value.Add(new TimeSpan(0, hour, min, second));
                }
                return null;
            }
        }

        [DataMember]
        public string Caller { get; set; }

        [DataMember]
        public string CallerPhone { get; set; }

        [DataMember]
        public string Callee { get; set; }

        [DataMember]
        public string CalleePhone { get; set; }

        [DataMember]
        public string CallTime { get; set; }

        [DataMember]
        public CallState? CallState { get; set; }

        [MaxLength(1000)]
        [DataMember]
        public string RecordURL { get; set; }

        #endregion

        #region 邮件短信

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string SendTo { get; set; }

        [DataMember]
        public string CcTo { get; set; }

        #endregion

        [DataMember]
        public TracingType TracingType { get; set; }

        [XmlIgnore]
        public string TracingTypeName
        {
            get { return TracingType.ToDescription(); }
        }

        [XmlIgnore]
        public string CallStateName
        {
            get
            {
                if (CallState.HasValue)
                {
                    return CallState.ToDescription();
                }
                return string.Empty;
            }
        }

        public string CompanyTypeName { get; set; }

        public string CallDuration
        {
            get
            {
                if (StartTime.HasValue && EndTime.HasValue)
                {
                 TimeSpan duration=   EndTime.Value.Subtract(StartTime.Value);
                    return duration.ToString(@"hh\:mm\:ss");
                }
                return string.Empty;
            }
        }
    }
}
