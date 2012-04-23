namespace FourDO.Utilities
{
	public class PerformanceStopWatch
	{
		private bool started;
		private long originalSample;
		private long totalSampleLength;
		private double? totalSampleSeconds;

		private readonly object publicSemaphore = new object();

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
				return this.totalSampleSeconds ?? 0;
			}
		}


		public double TotalMilliseconds
		{
			get
			{
				this.EnsureCalculated();
				return (this.totalSampleSeconds ?? 0) * 1000;
			}
		}

		private void EnsureCalculated()
		{
			if (!this.totalSampleSeconds.HasValue)
				this.totalSampleSeconds = this.totalSampleLength / (double)PerformanceCounter.Frequency;
		}
	}
}
