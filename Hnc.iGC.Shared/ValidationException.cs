using System;
using System.Runtime.Serialization;

namespace Hnc.iGC
{
    /// <summary>
    /// 加密狗验证异常
    /// </summary>
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }
        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
