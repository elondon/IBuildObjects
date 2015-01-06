using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBuildObjects
{
    public enum LogType
    {
        Error,
        Warning,
        Debug,
        Trace,
        Info
    }

    public class LogMessage
    {
        public LogType LogType { get; protected set; }
        public DateTime? Timestamp { get; protected set; }
        public string Message { get; protected set; }

        public LogMessage(LogType logType, DateTime? timestamp, string message)
        {
            LogType = logType;
            Timestamp = timestamp;
            Message = message;
        }
    }
}
