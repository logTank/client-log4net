using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using log4net;

namespace log4tank.ConsoleTest
{
    internal class PerformanceTester
    {
        private ILog _logger;
        private Random _random;
        private int _numberOfIterations;

        public PerformanceTester(string loggerName, int numberOfIterations, int randomSeed)
        {
            _logger = LogManager.GetLogger(loggerName);
            _random = new Random(randomSeed);
            _numberOfIterations = numberOfIterations;
        }

        public TimeSpan RunTests()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            RunTestsInternal();

            sw.Stop();
            return sw.Elapsed;
        }

        private void RunTestsInternal()
        {
            for (int i = 0; i < _numberOfIterations; i++)
            {
                PerformOneTest();
            }
        }

        private void PerformOneTest()
        {
            double chance = _random.NextDouble();

            if (chance < 0.5)
            {
                _logger.DebugFormat("Performance Test Debug Message - chance was {0}", chance);
            }
            else if (chance < 0.8)
            {
                _logger.InfoFormat("Performance Test Info Message - chance was {0}", chance);
            }
            else if (chance < 0.9)
            {
                _logger.WarnFormat("Performance Test Warn Message - chance was {0}", chance);
            }
            else if (chance < 0.97)
            {
                _logger.Error(string.Format("Performance Test Error Message - chance was {0}", chance),
                    new Exception("Performance Test Error Exception"));
            }
            else
            {
                _logger.Error(string.Format("Performance Test Fatal Message - chance was {0}", chance),
                    new Exception("Performance Test Fatal Exception", new Exception("Performance Test Fatal Inner Exception")));
            }
        }
    }
}