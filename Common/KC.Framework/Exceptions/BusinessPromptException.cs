﻿using System;
using System.Runtime.Serialization;

namespace KC.Framework.Exceptions
{
    /// <summary>
    /// 数据访问层异常类，用于封装业务逻辑层引发的异常，直接返回到View层错误类
    /// </summary>
    [Serializable]
    public class BusinessPromptException : System.Exception
    {
        /// <summary>
        ///     实体化一个 GMF.Component.Tools.BllException 类的新实例
        /// </summary>
        public BusinessPromptException() { }

        /// <summary>
        ///     使用异常消息实例化一个 GMF.Component.Tools.BllException 类的新实例
        /// </summary>
        /// <param name="message">异常消息</param>
        public BusinessPromptException(string message)
            : base(message) { }

        /// <summary>
        ///     使用异常消息与一个内部异常实例化一个 GMF.Component.Tools.BllException 类的新实例
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="inner">用于封装在BllException内部的异常实例</param>
        public BusinessPromptException(string message, System.Exception inner)
            : base(message, inner) { }

        /// <summary>
        ///     使用可序列化数据实例化一个 GMF.Component.Tools.BllException 类的新实例
        /// </summary>
        /// <param name="info">保存序列化对象数据的对象。</param>
        /// <param name="context">有关源或目标的上下文信息。</param>
        protected BusinessPromptException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
