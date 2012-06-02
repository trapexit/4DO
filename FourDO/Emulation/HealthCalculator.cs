using System;
using System.Collections.Generic;
using System.Linq;
using FourDO.Utilities;

namespace FourDO.Emulation
{
	internal enum EmulationHealth
	{
		None = 0,
		Good = 1,
		Okay = 2,
		Bad = 3
	}

	internal class HealthCalculator
	{
		private class CheatRecord
		{
			public long RecordedOn { get; set; }
			public long CheatAmount { get; set; }
		}

		private const int RECORD_DURATION_SECONDS = 2;
		private const double BAD_STATUS_THRESHHOLD = .05;

		private readonly long MaximumRecordDuration;
		private readonly long BadThreshholdTicks;

		private List<CheatRecord> cheats = new List<CheatRecord>();
		private readonly object cheatSemaphore = new object();

		public HealthCalculator()
		{
			MaximumRecordDuration = PerformanceCounter.Frequency * RECORD_DURATION_SECONDS;
			BadThreshholdTicks = (long)(MaximumRecordDuration * BAD_STATUS_THRESHHOLD);
		}

		public EmulationHealth CurrentHealth
		{
			get
			{
				lock (cheatSemaphore)
				{
					// Remove old records
					var currentTicks = PerformanceCounter.Current;
					cheats.RemoveAll(x => currentTicks - x.RecordedOn > MaximumRecordDuration);

					// Get total cheat amount.
					long sum = 0;
					cheats.ForEach(x => sum += x.CheatAmount);

					// If there were no cheats, it's a green light.
					if (sum == 0)
						return EmulationHealth.Good;

					if (sum < BadThreshholdTicks)
						return EmulationHealth.Okay;

					return EmulationHealth.Bad;
				}
			}
		}

		public void Clear()
		{
			cheats.Clear();
		}

		public double CurrentAverage { get; protected set; }

		public void ReportCheat(long cheatAmount)
		{
			cheatAmount = Math.Abs(cheatAmount);
			lock (cheatSemaphore)
			{
				var currentTicks = PerformanceCounter.Current;
				var newRecord = new CheatRecord {RecordedOn = currentTicks, CheatAmount = cheatAmount};
				cheats.Add(newRecord);
			}
		}
	}
}
