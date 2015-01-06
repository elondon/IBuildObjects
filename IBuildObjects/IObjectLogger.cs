using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBuildObjects
{
    public interface IObjectLogger : ILogger
    {
        LogRoll GetLogRoll(long id);

        void Trace(string message, long id);
        void Debug(string message, long id);
        void Info(string message, long id);
        void Warn(string message, long id);
        void Error(string message, long id);
    }
}
