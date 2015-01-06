using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBuildObjects
{
    public abstract class ObjectLoggerBase : IObjectLogger
    {
        private Dictionary<long, LogRoll> _logs;

        protected ObjectLoggerBase()
        {
            _logs = new Dictionary<long, LogRoll>();
        }

        public LogRoll GetLogRoll(long id)
        {
            return _logs[id];
        }

        public void ClearLog(long id)
        {
            _logs[id] = new LogRoll(id);
        }

        public void ClearAllLogs()
        {
           _logs = new Dictionary<long, LogRoll>(); 
        }

        public void Trace(string message, long id)
        {
            if (!_logs.ContainsKey(id))
                _logs[id] = new LogRoll(id);

            _logs[id].LogMessages.Add(new LogMessage(LogType.Trace, DateTime.Now, message));
            Trace(message);
        }

        public void Debug(string message, long id)
        {
            if (!_logs.ContainsKey(id))
                _logs[id] = new LogRoll(id);

            _logs[id].LogMessages.Add(new LogMessage(LogType.Debug, DateTime.Now, message));
            Debug(message);
        }

        public void Info(string message, long id)
        {
            if (!_logs.ContainsKey(id))
                _logs[id] = new LogRoll(id);

            _logs[id].LogMessages.Add(new LogMessage(LogType.Info, DateTime.Now, message));
            Info(message);
        }

        public void Warn(string message, long id)
        {
            if (!_logs.ContainsKey(id))
                _logs[id] = new LogRoll(id);

            _logs[id].LogMessages.Add(new LogMessage(LogType.Warning, DateTime.Now, message));
            Warn(message);
        }

        public void Error(string message, long id)
        {
            if (!_logs.ContainsKey(id))
                _logs[id] = new LogRoll(id);

            _logs[id].LogMessages.Add(new LogMessage(LogType.Error, DateTime.Now, message));
            Error(message);
        }

        public abstract void Trace(string message);
        public abstract void Debug(string message);
        public abstract void Info(string message);
        public abstract void Warn(string message);
        public abstract void Error(string message);
    }
}
