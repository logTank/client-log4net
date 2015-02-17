using System;
using System.Collections.Generic;
using System.Text;
using log4net.Core;

namespace log4tank
{
    internal class LogTankEvent
    {
        public LogTankEvent(LoggingEvent loggingEvent)
        {
            Message = loggingEvent.MessageObject;
            Error = loggingEvent.ExceptionObject;
            LoggerName = loggingEvent.LoggerName;
            LogLevel = loggingEvent.Level.Name;
            TimeStamp = loggingEvent.TimeStamp;
            ThreadName = loggingEvent.ThreadName;
        }

        public object Message { get; private set; }

        public Exception Error { get; set; }

        public string LoggerName { get; set; }

        public string LogLevel { get; set; }

        public DateTime TimeStamp { get; set; }

        public string ThreadName { get; set; }
    }
}