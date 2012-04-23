using System;
using System.Linq;

namespace FourDO.Emulation
{
	internal class FrameSpeedCalculator
	{
		bool filled;
		int currentSample;
		readonly double[] samples;

		public FrameSpeedCalculator()
			: this(10)
		{
		}

		public FrameSpeedCalculator(int numberOfSamples)
		{
			if (numberOfSamples <= 0)
				throw new ArgumentOutOfRangeException("numberOfSamples");

			samples = new double[numberOfSamples];

			this.Clear();
		}

		public void Clear()
		{
			filled = false;
			currentSample = -1;
			CurrentAverage = 0;
		}

		public double CurrentAverage { get; protected set; }
		
		public double AddSample(double sample)
		{
			currentSample++;
			if (currentSample == samples.Length)
			{
				currentSample = 0;
				filled = true;
			}
			
			if (!filled)
			{
				double total = 0;
				for (int sampleNum = 0; sampleNum <= currentSample; sampleNum++)
				{
					total += samples[sampleNum];
				}
				CurrentAverage = total / (currentSample + 1);
				samples[currentSample] = sample;
				return CurrentAverage;
			}

			double total2 = samples.Sum();
			samples[currentSample] = sample;
			CurrentAverage = total2 / samples.Length;
			return CurrentAverage;
		}
	}
}
