using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace log4tank.ConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            var logger = LogManager.GetLogger("TestLogger");

            logger.Debug(new MessageType() { DateProp = DateTime.Now, IntProp = 1, StringProp = "Debug Message" });
            logger.Info(new MessageType() { DateProp = DateTime.Now, IntProp = 2, StringProp = "Info Message" });
            logger.Warn(new MessageType() { DateProp = DateTime.Now, IntProp = 3, StringProp = "Warn Message" });

            logger.Error(new MessageType() { DateProp = DateTime.Now, IntProp = 4, StringProp = "Error Message" },
                new InvalidOperationException("Invalid Operation Exception Message"));
            logger.Fatal(new MessageType() { DateProp = DateTime.Now, IntProp = 5, StringProp = "Fatal Message" },
                new ArgumentException("Argument Exception Message", "Param Name", new Exception("Inner Exception")));

            Console.WriteLine("Finished Logging");
            Console.ReadLine();
        }

        private class MessageType
        {
            public string StringProp { get; set; }

            public int IntProp { get; set; }

            public DateTime DateProp { get; set; }

            public override string ToString()
            {
                return string.Format("{0} - {1} - {2}", StringProp, IntProp, DateProp);
            }
        }
    }
}