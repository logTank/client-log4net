using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using log4net;

namespace log4tank.ConsoleTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            //SimpleTest();
            PerformanceTests();
        }

        private static void SimpleTest()
        {
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

        private static void PerformanceTests()
        {
            int numberOfIterations = 1000 * 10;
            int randomSeed = 1;

            Console.WriteLine("Running tests with {0} iterations...", numberOfIterations);

            //ExecuteTest("PerformanceTestFileDebug", numberOfIterations, randomSeed);
            //ExecuteTest("PerformanceTestFileInfo", numberOfIterations, randomSeed);
            //ExecuteTest("PerformanceTestFileWarn", numberOfIterations, randomSeed);
            ExecuteTest("PerformanceTestLogTankDebug", numberOfIterations, randomSeed);
            ExecuteTest("PerformanceTestLogTankInfo", numberOfIterations, randomSeed);
            ExecuteTest("PerformanceTestLogTankWarn", numberOfIterations, randomSeed);
            ExecuteTest("PerformanceTestMixed1", numberOfIterations, randomSeed);
            ExecuteTest("PerformanceTestMixed2", numberOfIterations, randomSeed);
            Console.ReadLine();
        }

        private static void ExecuteTest(string testName, int numberOfIterations, int randomSeed)
        {
            TimeSpan duration;
            PerformanceTester tester = new PerformanceTester(testName, numberOfIterations, randomSeed);

            Console.WriteLine("Running {0} ...", testName);
            duration = tester.RunTests();
            Console.WriteLine("   it took {0} to complete the test, that is {1}ms/iterations", duration, duration.TotalMilliseconds / numberOfIterations);
            Thread.Sleep(30 * 1000);
        }
    }
}