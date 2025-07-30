using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.Service.Base
{
    public class ShjResult
    {
        public string result { get; set; }
        public int error { get; set; }
        public ShjMessageResult data { get; set; }
    }

    public class ShjMessageResult
    {
        public ShjMessageResult()
        {
            msg = new List<ShjCallLog>();
        }

        public int total { get; set; }
        public List<ShjCallLog> msg { get; set; }
    }

    /// <summary>
    /// 当Event为Cdr的时候说明该条数据是通话记录，包括：
    ///     Source：主叫号码
    ///     Destination：被叫号码
    ///     DestinationContext：目的地context（执行的上下文）
    ///     CallerID：主叫认息
    ///     Channel：主叫通道
    /// 	DestinationChannel：被叫通道
    /// 	LastApplication：最后执行的应用
    /// 	LastData：最后执行context
    /// 	StartTime：呼叫开始的时间（0000-00-00 00:00:00）
    /// 	Duration：整个呼叫的时间，从呼叫开始到结束，使用秒来计费
    ///     BillableSeconds：整个呼叫通话的时间，从呼叫接通开始到结束，使用秒来计费
    ///     Disposition：呼叫状态（接通-'ANSWERED'，没有接通-'NO ANSWER'，忙音-'BUSY',失败-'FAILED'）
    ///     UniqueID：通话记录的唯一标识
    ///     UserField：录音文件名
    ///     Exten：电话呼出信元，即呼出电话时，cdr信息有此参数，表示分机号
    ///     Dname：被叫分机座席名称
    ///     Sname：主叫分机座席名称
    ///     CallType：呼叫类型，呼叫类型：internal内部通话；in呼入；out呼出
    ///     CID：出局CID，呼出时带此参数
    ///     DID：被叫号码，即公司的外线号码，呼入时带此参数
    ///     Hangup：挂断方，Caller表示主叫先挂断,Callee表示被叫先挂断
    /// 当Event为UserEvent的时候根据UserEvent取值不同具体又分为服务评价数据、示忙示闲数据
    ///     ①当UserEvent为MixComment时是服务评价数据
    ///         Source：主叫号码
    ///         Destination：被叫号码
    ///         UniqueID：	呼叫中心产生的唯一标识，此值可以与通话记录中UniqueID关联
    ///         Key：		客户评价时输入的按键号，第二次才产生（长度2 字符或整型）
    ///         UserField：	录音文件名（长度200 字符）
    ///     ②当UserEvent为MixDnd时是服务评价数据
    ///         Exten：示忙示闲分机
    ///         DND：示忙示闲类型，当DND为“-1”时，说明是示闲，其他状态时为示忙
    /// </summary>
    public class ShjCallLog
    {
        /// <summary>
        /// 通话记录的唯一标识
        /// </summary>
        public string UniqueID { get; set; }
        /// <summary>
        /// Event为Cdr：说明该条数据是通话记录
        /// Event为UserEvent：根据UserEvent取值不同具体又分为服务评价数据、示忙示闲数据
        /// </summary>
        public string Event { get; set; }
        public string UserEvent { get; set; }
        public string Privilege { get; set; }
        public string AccountCode { get; set; }
        public string Server { get; set; }

        #region 通话记录: Event = Cdr
        /// <summary>
        /// 主叫号码（长度80 字符）
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 被叫号码（长度80 字符）
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// 目的地context（执行的上下文）（长度80 字符）
        /// </summary>
        public string DestinationContext { get; set; }
        /// <summary>
        /// 主叫号码（长度20 字符）
        /// </summary>
        public string CallerID { get; set; }
        /// <summary>
        /// 主叫通道(长度80 字符)
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 被叫通道(长度80 字符)
        /// </summary>
        public string DestinationChannel { get; set; }
        /// <summary>
        /// 最后执行的应用(长度80 字符)
        /// </summary>
        public string LastApplication { get; set; }
        /// <summary>
        /// 最后执行context(长度80 字符)
        /// </summary>
        public string LastData { get; set; }
        /// <summary>
        /// 呼叫开始的时间（0000-00-00 00:00:00）
        /// </summary>
        public DateTime StartTime { get; set; }
        public DateTime? AnswerTime { get; set; }
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 整个呼叫的时间，从呼叫开始到结束，使用秒来计费（数据类型：整数）
        /// </summary>
        public long Duration { get; set; }
        /// <summary>
        /// 整个呼叫通话的时间，从呼叫接通开始到结束，使用秒来计费（数据类型：整数）
        /// </summary>
        public long BillableSeconds { get; set; }
        /// <summary>
        /// 呼叫状态:
        ///     接通-'ANSWERED'
        ///     没有接通-'NO ANSWER'
        ///     忙音-'BUSY'
        ///     失败-'FAILED'
        /// </summary>
        public string Disposition { get; set; }

        public string AMAFlags { get; set; }

        /// <summary>
        /// 录音文件名
        /// </summary>
        public string UserField { get; set; }
        /// <summary>
        /// 被叫分机座席名称
        /// </summary>
        public string Dname { get; set; }
        /// <summary>
        /// 主叫分机座席名称
        /// </summary>
        public string Sname { get; set; }
        /// <summary>
        /// 呼叫类型，呼叫类型：internal内部通话；in呼入；out呼出
        /// </summary>
        public string CallType { get; set; }
        /// <summary>
        /// 出局CID，呼出时带此参数
        /// </summary>
        public string CID { get; set; }
        /// <summary>
        /// 被叫号码，即公司的外线号码，呼入时带此参数
        /// </summary>
        public string DID { get; set; }
        /// <summary>
        /// 挂断方
        ///     Caller表示主叫先挂断
        ///     Callee表示被叫先挂断
        /// </summary>
        public string Hangup { get; set; }

        public string MixExtVar { get; set; }
        #endregion

        #region 服务评价数据: Event = UserEvent && UserEvent = MixComment
        /// <summary>
        /// 客户评价时输入的按键号，第二次才产生（长度2 字符或整型）
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 被叫号码（长度20 字符）
        /// </summary>
        public string CalleeID { get; set; }
        #endregion

        #region 服务评价数据: Event = UserEvent && UserEvent = MixDnd
        public string Action { get; set; }
        /// <summary>
        /// 通话记录:     电话呼出信元，即呼出电话时，cdr信息有此参数，表示分机号
        /// 服务评价数据: 示忙示闲分机
        /// </summary>
        public string Exten { get; set; }
        /// <summary>
        /// 示忙示闲类型，当DND为“-1”时，说明是示闲，其他状态时为示忙
        /// </summary>
        public string DND { get; set; }
        #endregion
    }
}
