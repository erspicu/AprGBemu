using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

//ref from http://www.codeproject.com/Articles/2635/High-Performance-Timer-in-C
//已依照自己需求,修改與刪除部分code

namespace HresTimer
{
    internal class HiPerfTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);  

		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(out long lpFrequency);
		
		private long startTime, stopTime;
		private long freq;
		
        // Constructor
		public HiPerfTimer()
		{
            startTime = 0;
            stopTime  = 0;

            if (QueryPerformanceFrequency(out freq) == false)
            {
                // high-performance counter not supported 
                throw new Win32Exception(); 
            }
		}
		
		// Start the timer
		public void Restart()
		{
            // lets do the waiting threads there work
            Thread.Sleep(0);

            startTime = 0;
            stopTime = 0;

			QueryPerformanceCounter(out startTime);
		}
		
	
		// Returns the duration of the timer (in seconds)
        public double Duration
        {
        	get
        	{
                QueryPerformanceCounter(out stopTime);
            	return (double)(stopTime - startTime) / (double) freq;
            }
        }
	}
}
