using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace IBuildObjects
{
    public class LogRoll
    {
        public long Id { get; protected set; }
        public List<LogMessage> LogMessages { get; protected set; } 

        public LogRoll(long id)
        {
            Id = id;
            LogMessages = new List<LogMessage>();
        }

        public void WriteToFile(string file)
        {
            using (var streamWriter = new StreamWriter(file))
            {
                foreach (var logMessage in LogMessages)
                {
                    switch (logMessage.LogType)
                    {
                        case LogType.Info:
                            streamWriter.WriteLine(logMessage.Timestamp + " - Info: " + logMessage.Message);
                            break;
                        case LogType.Trace:
                            streamWriter.WriteLine(logMessage.Timestamp + " - Trace: " + logMessage.Message);
                            break;
                        case LogType.Debug:
                            streamWriter.WriteLine(logMessage.Timestamp + " - Debug: " + logMessage.Message);
                            break;
                        case LogType.Warning:
                            streamWriter.WriteLine(logMessage.Timestamp + " - Warning: " + logMessage.Message);
                            break;
                        case LogType.Error:
                            streamWriter.WriteLine(logMessage.Timestamp + " - Error: " + logMessage.Message);
                            break;
                    }
                }
            }
        }
    }
}
