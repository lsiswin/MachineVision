using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision.Core.Extensions
{
    public static class SetTimeHelper
    {
        public static double SetTime(Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
