using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base
{
    public class Request
    {
        public RequestHead Head { get; set; }

        public RequestBody Body { get; set; }

    }

    public class RequestHead
    {
        public string MethodName { get; set; }
        public string Spid { get; set; }
        public string Appid { get; set; }
        public string Passwd { get; set; }
        public string Timestamp { get; set; }
        public string Authenticator { get; set; }
    }
    public class RequestForCall
    {
        public RequestHead Head { get; set; }
        public RequestBodyForCall Body { get; set; }
    }
    public class RequestBodyForCall
    {
        public string ChargeNbr { get; set; }
        public string Key { get; set; }
        public string DisplayNbr { get; set; }
        public string CallerNbr { get; set; }
        public string CalleeNbr { get; set; }
        public string Record { get; set; }
        public string AnswerOnMedia { get; set; }
        public string DisplayNbrMode { get; set; }
        public string Sessionid { get; set; }
    }

    public class RequestBody
    {
        public string ChargeNbr { get; set; }
        public string Key { get; set; }
        public string DisplayNbr { get; set; }
        public string CalleeNbr { get; set; }
        public string TempletId { get; set; }
        public string vReplay { get; set; }
        public string ReplayTTS { get; set; }
        public string ReplayTimes { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
        public string Value5 { get; set; }
        public string Value6 { get; set; }
    }

    public class Response
    {
        public ResponseHead Head { get; set; }
    }

    public class ResponseHead
    {
        public string Result { get; set; }

        public string ResultDesc { get; set; }

        public string Sessionid { get; set; }
    }
}
