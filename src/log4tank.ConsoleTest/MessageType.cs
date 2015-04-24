using System;
using System.Collections.Generic;
using System.Text;

namespace log4tank.ConsoleTest
{
    internal class MessageType
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