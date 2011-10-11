using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FourDO.Utilities
{
    public static class PerformanceCounter
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private static readonly long frequency;

        static PerformanceCounter()
        {
            if(QueryPerformanceFrequency(out frequency) == false)
            {
                // System doesn't support it. Screw that!
                // Just use ticks.
                frequency = 1000;
            }
        }
        
        public static long Current
        {
            get
            {
                long sample;
                if (QueryPerformanceCounter(out sample) == false)
                {
                    // If the system doesn't support performance counters, then use ticks.
                    sample = DateTime.Now.Ticks;
                }
                return sample;
            }
        }

        public static long Frequency
        {
            get
            {
                return frequency;
            }
        }
    }
}
