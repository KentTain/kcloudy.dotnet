using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace KC.Service
{
    /// <summary>
    ///     业务操作结果信息类，对操作结果进行封装
    /// </summary>
    [Serializable, DataContract(IsReference = true)]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class ServiceResult<T>
    {
        #region 构造函数
        public ServiceResult()
        {
            ResultType = ServiceResultType.Success;
            HttpStatusCode = System.Net.HttpStatusCode.OK;
            _success = ResultType == ServiceResultType.Success;
        }

        /// <summary>
        ///     初始化一个 业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        public ServiceResult(ServiceResultType resultType)
            : this(resultType, System.Net.HttpStatusCode.OK)
        {
        }

        /// <summary>
        ///     初始化一个 业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="httpStatus">响应状态</param>
        public ServiceResult(ServiceResultType resultType, System.Net.HttpStatusCode httpStatus)
        {
            ResultType = resultType;
            HttpStatusCode = httpStatus;
            _success = resultType == ServiceResultType.Success;
        }

        /// <summary>
        ///     初始化一个 定义返回消息的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="message">业务返回消息</param>
        public ServiceResult(ServiceResultType resultType, string message)
            : this(resultType)
        {
            this.message = message;
        }

        /// <summary>
        ///     初始化一个 定义返回消息的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="httpStatus">响应状态</param>
        /// <param name="message">业务返回消息</param>
        public ServiceResult(ServiceResultType resultType, System.Net.HttpStatusCode httpStatus, string message)
            : this(resultType, httpStatus)
        {
            this.message = message;
        }

        /// <summary>
        ///     初始化一个 定义返回消息与附加数据的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="message">业务返回消息</param>
        /// <param name="result">业务返回数据</param>
        public ServiceResult(ServiceResultType resultType, string message, T result)
            : this(resultType, message)
        {
            Result = result;
        }

        /// <summary>
        ///     初始化一个 定义返回消息与附加数据的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="message">业务返回消息</param>
        /// <param name="result">业务返回数据</param>
        public ServiceResult(ServiceResultType resultType, System.Net.HttpStatusCode httpStatus, string message, T result)
            : this(resultType, httpStatus, message)
        {
            Result = result;
        }

        /// <summary>
        ///     初始化一个 定义返回消息与日志消息的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="message">业务返回消息</param>
        /// <param name="logMessage">业务日志记录消息</param>
        public ServiceResult(ServiceResultType resultType, string message, string logMessage)
            : this(resultType, message)
        {
            LogMessage = logMessage;
        }

        /// <summary>
        ///     初始化一个 定义返回消息与日志消息的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="message">业务返回消息</param>
        /// <param name="logMessage">业务日志记录消息</param>
        public ServiceResult(ServiceResultType resultType, System.Net.HttpStatusCode httpStatus, string message, string logMessage)
            : this(resultType, httpStatus, message)
        {
            LogMessage = logMessage;
        }

        /// <summary>
        ///     初始化一个 定义返回消息、日志消息与附加数据的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="message">业务返回消息</param>
        /// <param name="logMessage">业务日志记录消息</param>
        /// <param name="result">业务返回数据</param>
        public ServiceResult(ServiceResultType resultType, string message, string logMessage, T result)
            : this(resultType, message, logMessage)
        {
            Result = result;
        }

        /// <summary>
        ///     初始化一个 定义返回消息、日志消息与附加数据的业务操作结果信息类 的新实例
        /// </summary>
        /// <param name="resultType">业务操作结果类型</param>
        /// <param name="message">业务返回消息</param>
        /// <param name="logMessage">业务日志记录消息</param>
        /// <param name="result">业务返回数据</param>
        public ServiceResult(ServiceResultType resultType, System.Net.HttpStatusCode httpStatus, string message, string logMessage, T result)
            : this(resultType, httpStatus, message, logMessage)
        {
            Result = result;
        }

        #endregion

        #region 属性
        [DataMember]
     
        public System.Net.HttpStatusCode HttpStatusCode { get; set; }
        /// <summary>
        ///     获取或设置 操作结果类型
        /// </summary>
        [DataMember]

        public ServiceResultType ResultType { get; set; }

        /// <summary>
        ///     获取或设置 操作返回信息
        /// </summary>
        [DataMember]
  
        public string message { get; set; }

        /// <summary>
        ///     获取或设置 操作返回的日志消息，用于记录日志
        /// </summary>
        [DataMember]

        public string LogMessage { get; set; }

        /// <summary>
        ///     获取或设置 操作结果附加信息
        /// </summary>
        [DataMember]

        public T Result { get; set; }

        private bool _success;
        [DataMember]
        public bool success
        {
            get
            {
                return _success;
            }
            set
            {
                //如果不加set方法,protobuf序列化到缓存的时候会失败,原因好像是不能序列化readonly
                _success = value;
            }
        }

        #endregion
    }

    /// <summary>
    ///     表示业务操作结果的枚举
    /// </summary>
    [Description("业务操作结果的枚举")]
    [Serializable]
    [DataContract]
    public enum ServiceResultType
    {
        /// <summary>
        ///     操作成功
        /// </summary>
        [Description("操作成功。")]
        [EnumMember]
        Success,

        /// <summary>
        ///     操作取消或操作没引发任何变化
        /// </summary>
        [Description("操作没有引发任何变化，提交取消。")]
        [EnumMember]
        NoChanged,

        /// <summary>
        ///     参数错误
        /// </summary>
        [Description("参数错误。")]
        [EnumMember]
        ParamError,

        /// <summary>
        ///     指定参数的数据不存在
        /// </summary>
        [Description("指定参数的数据不存在。")]
        [EnumMember]
        QueryNull,

        /// <summary>
        ///     权限不足
        /// </summary>
        [Description("当前用户权限不足，不能继续操作。")]
        [EnumMember]
        PurviewLack,

        /// <summary>
        ///     非法操作
        /// </summary>
        [Description("非法操作。")]
        [EnumMember]
        IllegalOperation,

        /// <summary>
        ///     警告
        /// </summary>
        [Description("警告")]
        [EnumMember]
        Warning,

        /// <summary>
        ///     操作引发错误
        /// </summary>
        [Description("操作引发错误。")]
        [EnumMember]
        Error,
    }
}
