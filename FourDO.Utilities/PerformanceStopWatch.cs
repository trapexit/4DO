using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourDO.Utilities
{
	public class PerformanceStopWatch
	{
		private bool started = false;
		private long originalSample = 0;
		private long totalSampleLength = 0;
		private double? totalSampleSeconds = null;

		private object publicSemaphore = new object();

		public void Start()
		{
			lock(this.publicSemaphore)
			{
				this.originalSample = PerformanceCounter.Current;
				this.started = true;
			}
		}

		public void Stop()
		{
			lock (this.publicSemaphore)
			{
				if (!this.started)
					return;
				this.totalSampleLength += (PerformanceCounter.Current - this.originalSample);
				this.totalSampleSeconds = null;
				this.started = false;
			}
		}

		public double TotalSeconds
		{
			get
			{
				this.EnsureCalculated();
				return this.totalSampleSeconds.Value;
			}
		}


		public double TotalMilliseconds
		{
			get
			{
				this.EnsureCalculated();
				return this.totalSampleSeconds.Value * 1000;
			}
		}

		private void EnsureCalculated()
		{
			if (!this.totalSampleSeconds.HasValue)
				this.totalSampleSeconds = this.totalSampleLength / (double)PerformanceCounter.Frequency;
		}
	}
}
