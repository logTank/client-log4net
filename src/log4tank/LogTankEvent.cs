using System;
using System.Collections.Generic;
using System.Text;
using log4net.Core;
using log4net.Util;

namespace log4tank
{
    internal class LogTankEvent
    {
        public LogTankEvent(LoggingEvent loggingEvent)
        {
            Message = GetMessageObject(loggingEvent.MessageObject);
            Error = loggingEvent.ExceptionObject;
            LoggerName = loggingEvent.LoggerName;
            LogLevel = GetLogLevel(loggingEvent.Level);
            TimeStamp = loggingEvent.TimeStamp;
            ThreadName = loggingEvent.ThreadName;
        }

        public object Message { get; private set; }

        public Exception Error { get; set; }

        public string LoggerName { get; set; }

        public string LogLevel { get; set; }

        public DateTime TimeStamp { get; set; }

        public string ThreadName { get; set; }

        private static object GetMessageObject(object messageObject)
        {
            if (messageObject != null && messageObject is SystemStringFormat)
            {
                return messageObject.ToString();
            }
            else
            {
                return messageObject;
            }
        }

        private static string GetLogLevel(Level logLevel)
        {
            if (logLevel == null)
            {
                return null;
            }
            else
            {
                return logLevel.Name;
            }
        }
    }
}